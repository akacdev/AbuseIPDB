using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace AbuseIPDB
{
    /// <summary>
    /// The main class for interacting with he API. Create an instance of it by calling a constructor.
    /// </summary>
    public class AbuseIPDBClient
    {
        /// <summary>
        /// Base URI to send requests to.
        /// </summary>
        public static readonly Uri BaseUri = new($"https://api.abuseipdb.com/api/v{Constants.Version}/");

        private static readonly HttpClientHandler HttpHandler = new()
        {
            AutomaticDecompression = DecompressionMethods.All
        };

        private readonly HttpClient Client = new(HttpHandler)
        {
            BaseAddress = BaseUri,
            DefaultRequestVersion = new(2, 0)
        };

        private readonly AbuseIPDBClientConfig Config;

        /// <summary>
        /// Create a new instance of the client for interacting with the API.
        /// </summary>
        /// <param name="key">Your AbuseIPDB API key. You can create one at <a href="https://www.abuseipdb.com/account/api"></a>.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public AbuseIPDBClient(string key)
        {
            Config = new()
            {
                Key = key
            };

            if (string.IsNullOrEmpty(Config.Key)) throw new ArgumentNullException(nameof(key), "An empty or null API Key was provided.");

            InitializeClient();
        }

        /// <summary>
        /// Create a new instance of the client for interacting with the API by passing an advanced <see cref="AbuseIPDBClientConfig"/> configuration object.
        /// </summary>
        /// <param name="config"></param>
        public AbuseIPDBClient(AbuseIPDBClientConfig config)
        {
            if (config is null) throw new ArgumentNullException(nameof(config), "Client config object is null.");
            Config = config;

            InitializeClient();
        }

        private void InitializeClient()
        {
            Client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
            Client.DefaultRequestHeaders.UserAgent.ParseAdd(Constants.UserAgent);
            Client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            Client.DefaultRequestHeaders.Add("Key", Config.Key);
        }

        /// <summary>
        /// Check an IP address in the database, and optionally also fetch its past reports.
        /// </summary>
        /// <param name="ip">The IP address to check.</param>
        /// <param name="verbose">When <see langword="true"/>, reports and country names will be includeed in the output.</param>
        /// <param name="maxAge">How old reports, in days, should be considered. Only effective when verbosity is enabled.</param>
        /// <returns>An instance of <see cref="CheckedIP"/> containing all information about the IP address and optionally the recent reports.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<CheckedIP> Check(string ip, bool verbose = true, int maxAge = 90)
        {
            if (string.IsNullOrEmpty(ip)) throw new ArgumentNullException(nameof(ip), "IP address to check is null or empty.");
            if (maxAge <= 0) throw new ArgumentOutOfRangeException(nameof(maxAge), "Max age has to be a positive value.");

            HttpResponseMessage res = await Client.Request(
                HttpMethod.Get,
                $"check?ipAddress={HttpUtility.UrlEncode(ip)}&maxAgeInDays={maxAge}{(verbose ? "&verbose" : "")}");

            return (await res.Deseralize<CheckedIPContainer>()).Data;
        }

        /// <summary>
        /// Get up to the latest <c>maxReports</c> reports for an IP address.
        /// </summary>
        /// <param name="ip">The IP address to get reports for.</param>
        /// <param name="limit">The maximum amount of reports to get.</param>
        /// <param name="maxAge">How old reports, in days, should be considered.</param>
        /// <returns>An array of <see cref="IPReport"/> containing all requested reports. They are ordered descending by date.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<IPReport[]> GetReports(string ip, int limit = 100, int maxAge = 90)
        {
            if (string.IsNullOrEmpty(ip)) throw new ArgumentNullException(nameof(ip), "IP address to use is null or empty.");
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit has to be a positive value.");
            if (maxAge <= 0) throw new ArgumentOutOfRangeException(nameof(maxAge), "Max age has to be a positive value.");

            List<IPReport> output = new(limit);

            int pageNumber = 1;
            int perPage = 100;

            bool next = true;
            while (next)
            {
                HttpResponseMessage res = await Client.Request(
                    HttpMethod.Get,
                    $"reports?ipAddress={HttpUtility.UrlEncode(ip)}&maxAgeInDays={maxAge}&page={pageNumber}&perPage={perPage}",
                    target: HttpStatusCode.OK
                    );

                IPReportsPage page = (await res.Deseralize<IPReportsContainer>()).Data;

                if ((output.Count + page.Results.Length) <= limit) output.AddRange(page.Results);
                else output.AddRange(page.Results.Take(limit - output.Count));

                pageNumber++;

                if (page.NextPageUrl == null || page.Count == 0 || output.Count >= limit) next = false;
            }

            return output.ToArray();
        }

        /// <summary>
        /// Export a blacklist of the IP addresses with the highest abuse confidence score. Some of the features in this API endpoint are limited to subscribers.
        /// </summary>
        /// <param name="confidenceMinimum">(<b>Subscriber Feature</b>) The minimum abuse confidence score for an IP address to be included.</param>
        /// <param name="limit">
        /// (<b>Limited Feature</b>) The maximum amount of IPs to return. Note this is capped by your subscription level.<br/><br/>
        /// 
        /// <list type="table">
        ///     <listheader>
        ///         <term>Subscription</term>
        ///         <description>Limit</description>
        ///     </listheader>
        ///     <item>
        ///         <term>Free</term>
        ///         <description>10,000</description>
        ///     </item>
        ///     <item>
        ///         <term>Basic</term>
        ///         <description>100,000</description>
        ///     </item>
        ///     <item>
        ///         <term>Premium</term>
        ///         <description>500,000</description>
        ///     </item>
        /// </list>
        /// 
        /// </param>
        /// <param name="onlyCountries">
        ///     (<b>Subscriber Feature</b>) A whitelist of countries to include in the export.<br/>
        ///     Note these are formatted as <c>ISO 3166 alpha-2</c> codes.
        /// </param>
        /// <param name="exceptCountries">
        ///     (<b>Subscriber Feature</b>) A blacklist of countries to omit in the export.
        ///     Note these are formatted as <c>ISO 3166 alpha-2</c> codes.
        /// </param>
        /// 
        /// <returns>An array of <see cref="BlacklistedIP"/> with all returned blacklisted IP addresses.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<BlacklistedIP[]> GetBlacklist(
            int limit = 10000,
            int? confidenceMinimum = null,
            string[] onlyCountries = null,
            string[] exceptCountries = null)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit), "Limit has to be a positive value.");
            if (confidenceMinimum.HasValue && (confidenceMinimum.Value < 0 || confidenceMinimum.Value > 100)) throw new ArgumentOutOfRangeException(nameof(confidenceMinimum), "Minimum confidence score has to be a valid percentage value.");

            HttpResponseMessage res = await Client.Request(
                HttpMethod.Get,
                "blacklist" +
                $"?limit={limit}" +
                (!confidenceMinimum.HasValue ? "" : $"&confidenceMinimum={confidenceMinimum.Value}") +
                (onlyCountries is null ? "" : $"&onlyCountries={string.Join(',', onlyCountries)}") +
                (exceptCountries is null ? "" : $"&exceptCountries={string.Join(',', exceptCountries)}"));

            return (await res.Deseralize<BlacklistContainer>()).Data;
        }

        /// <summary>
        /// Submit a new abusive IP report. This is the core functionality of <c>AbuseIPDB</c>.<br/>
        /// <b>Remember to strip all PII (Personally Identifiable Information)</b>. <c>AbuseIPDB</c> is not responsible for PII you reveal.
        /// </summary>
        /// <param name="ip">The IP address to report.</param>
        /// <param name="categories">The categories to use.</param>
        /// <param name="comment">
        ///     A comment describing the malicious activity.<br/>
        ///     This can be <see langword="null"/>, but it's best to provide a detailed reason.
        /// </param>
        /// <returns>An instance of <see cref="ReportedIP"/> containing some information about the submitted report.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<ReportedIP> Report(string ip, IPReportCategory[] categories, string comment)
        {
            if (string.IsNullOrEmpty(ip)) throw new ArgumentNullException(nameof(ip), "IP address to use is null or empty.");

            HttpResponseMessage res = await Client.Request(
                HttpMethod.Post,
                Config.ReportEndpoint ?? "report",
                new IPReportParameters()
                {
                    IPAddress = ip,
                    Categories = string.Join(',', categories.Select(x => (int)x)),
                    Comment = comment
                },
                absoluteUrl: Config.ReportEndpoint is not null);

            if (Config.CustomReportResponseConverter is not null) return await Config.CustomReportResponseConverter(res);
            return (await res.Deseralize<ReportedIPContainer>()).Data;
        }

        /// <summary>
        /// Bulk report many IP addresses at once. This endpoint has separate ratelimits and can accept up to <c>10,000</c> lines of CSV at once.<br/>
        /// If you have a <see cref="byte"/> array or a <see cref="string"/> with the CSV data, you can convert it to a <see cref="MemoryStream"/> and pass that.
        /// </summary>
        /// <param name="csvStream">A <see cref="Stream"/> pointing to the CSV data. This can also be a <see cref="FileStream"/> or <see cref="MemoryStream"/>.</param>
        /// <returns>An instance of <see cref="BulkReport"/> containing the summary of your bulk report.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<BulkReport> BulkReport(Stream csvStream)
        {
            if (csvStream is null) throw new ArgumentNullException(nameof(csvStream), "CSV stream to bulk report is null.");

            HttpResponseMessage res = await Client.Request(
                HttpMethod.Post, Config.BulkReportEndpoint ?? "bulk-report", csvStream, "csv", "reports.csv",
                absoluteUrl: Config.BulkReportEndpoint is not null);

            if (Config.CustomBulkReportResponseConverter is not null) return await Config.CustomBulkReportResponseConverter(res);
            return (await res.Deseralize<BulkReportContainer>()).Data;
        }

        /// <summary>
        /// Check an entire CIDR network for recently reported addresses. 
        /// </summary>
        /// <param name="network">The CIDR network to search for. Example: <c>186.2.163.0/24</c></param>
        /// <param name="maxAge">How old reports, in days, should be considered. On the <b>FREE</b> plan, the limit is <c>30</c> days.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<CheckedBlock> CheckBlock(string network, int maxAge)
        {
            if (string.IsNullOrEmpty(network)) throw new ArgumentNullException(nameof(network), "Network to block check is null or empty.");
            if (maxAge <= 0) throw new ArgumentOutOfRangeException(nameof(maxAge), "Max age has to be a positive value.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Get, $"check-block?network={network}&maxAgeInDays={maxAge}");

            return (await res.Deseralize<CheckedBlockContainer>()).Data;
        }

        /// <summary>
        /// Delete all of your reports on an IP address.
        /// </summary>
        /// <param name="ip">The IP address to clear.</param>
        /// <returns>An instance of <see cref="ClearedAddress"/> containing the amount of reports that have been deleted.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<ClearedAddress> ClearAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip)) throw new ArgumentNullException(nameof(ip), "IP address to clear is null or empty.");

            HttpResponseMessage res = await Client.Request(HttpMethod.Delete, $"clear-address?ipAddress={ip}");

            return (await res.Deseralize<ClearedAddressContainer>()).Data;
        }
    }
}