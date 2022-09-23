using System;
using System.Text.Json.Serialization;

namespace AbuseIPDB
{
    /// <summary>
    /// A category for IP address reports.
    /// </summary>
    public enum IpReportCategory
    {
        /// <summary>
        /// IP address is involved in: DNS Compromise
        /// <para><b>AbuseIPDB Definition:</b> Altering DNS records resulting in improper redirection.</para>
        /// </summary>
        DNSCompromise = 1,
        /// <summary>
        /// IP address is involved in: DNS Poisoning
        /// <para><b>AbuseIPDB Definition:</b>  Falsifying domain server cache (cache poisoning).</para>
        /// </summary>
        DNSpoisoning = 2,
        /// <summary>
        /// IP address is involved in: Fraud Orders
        /// <para><b>AbuseIPDB Definition:</b> Fraudulent orders.</para>
        /// </summary>
        FraudOrders = 3,
        /// <summary>
        /// IP address is involved in: DDoS Attack
        /// <para><b>AbuseIPDB Definition:</b> Participating in distributed denial-of-service (usually part of botnet).</para>
        /// </summary>
        DDoSAttack = 4,
        /// <summary>
        /// IP address is involved in: FTP Brute-Force
        /// </summary>
        FTPBruteForce = 5,
        /// <summary>
        /// IP address is involved in: Ping of Death
        /// <para><b>AbuseIPDB Definition:</b> Oversized IP packet.</para>
        /// </summary>
        PingOfDeath = 6,
        /// <summary>
        /// IP address is involved in: Phishing
        /// <para><b>AbuseIPDB Definition:</b> Phishing websites and/or email.</para>
        /// </summary>
        Phishing = 7,
        /// <summary>
        /// IP address is involved in: Fraud VoIP
        /// </summary>
        FraudVoIP = 8,
        /// <summary>
        /// IP address is involved in: Open Proxy
        /// <para><b>AbuseIPDB Definition:</b> Open proxy, open relay, or Tor exit node.</para>
        /// </summary>
        OpenProxy = 9,
        /// <summary>
        /// IP address is involved in: Web Spam
        /// <para><b>AbuseIPDB Definition:</b> Comment/forum spam, HTTP referer spam, or other CMS spam.</para>
        /// </summary>
        WebSpam = 10,
        /// <summary>
        /// IP address is involved in: Email Spam
        /// <para><b>AbuseIPDB Definition:</b> Spam email content, infected attachments, and phishing emails.</para>
        /// </summary>
        EmailSpam = 11,
        /// <summary>
        /// IP address is involved in: Blog Spam
        /// <para><b>AbuseIPDB Definition:</b> CMS blog comment spam.</para>
        /// </summary>
        BlogSpam = 12,
        /// <summary>
        /// IP address is involved in: VPN IP
        /// <para><b>AbuseIPDB Definition:</b> Conjunctive category.</para>
        /// </summary>
        VPN = 13,
        /// <summary>
        /// IP address is involved in: Port Scan
        /// <para><b>AbuseIPDB Definition:</b> Scanning for open ports and vulnerable services.</para>
        /// </summary>
        PortScan = 14,
        /// <summary>
        /// IP address is involved in: Hacking
        /// </summary>
        Hacking = 15,
        /// <summary>
        /// IP address is involved in: SQL Injection
        /// <para><b>AbuseIPDB Definition:</b> Attempts at SQL injection.</para>
        /// </summary>
        SQLInjection = 16,
        /// <summary>
        /// IP address is involved in: Spoofing
        /// <para><b>AbuseIPDB Definition:</b> Email sender spoofing.</para>
        /// </summary>
        Spoofing = 17,
        /// <summary>
        /// IP address is involved in: Brute Force
        /// <para><b>AbuseIPDB Definition:</b> Credential brute-force attacks on webpage logins and services like SSH, FTP, SIP, SMTP, RDP, etc. This category is seperate from DDoS attacks.</para>
        /// </summary>
        BruteForce = 18,
        /// <summary>
        /// IP address is involved in: Bad Web Bot
        /// <para><b>AbuseIPDB Definition:</b> Webpage scraping (for email addresses, content, etc) and crawlers that do not honor robots.txt. Excessive requests and user agent spoofing can also be reported here.</para>
        /// </summary>
        BadWebBot = 19,
        /// <summary>
        /// IP address is involved in: Exploited Host
        /// <para><b>AbuseIPDB Definition:</b> Host is likely infected with malware and being used for other attacks or to host malicious content. The host owner may not be aware of the compromise. This category is often used in combination with other attack categories.</para>
        /// </summary>
        ExploitedHost = 20,
        /// <summary>
        /// IP address is involved in: Web App Attack
        /// <para><b>AbuseIPDB Definition:</b> Attempts to probe for or exploit installed web applications such as a CMS like WordPress/Drupal, e-commerce solutions, forum software, phpMyAdmin and various other software plugins/solutions.</para>
        /// </summary>
        WebAppAttack = 21,
        /// <summary>
        /// IP address is involved in: SSH Brute-Forcé
        /// <para><b>AbuseIPDB Definition:</b> Secure Shell (SSH) abuse. Use this category in combination with more specific categories.</para>
        /// </summary>
        SSH = 22,
        /// <summary>
        /// IP address is involved in: IoT Targeted
        /// <para><b>AbuseIPDB Definition:</b> Abuse was targeted at an "Internet of Things" type device. Include information about what type of device was targeted in the comments.</para>
        /// </summary>
        IoTTargeted = 23
    }

    /// <summary>
    /// A container for <see cref="AbuseIPDBError"/>.
    /// </summary>
    public class AbuseIPDBErrorContainer
    {
        [JsonPropertyName("errors")]
        public AbuseIPDBError[] Errors { get; set; }
    }

    /// <summary>
    /// The AbuseIPDB error data.
    /// </summary>
    public class AbuseIPDBError
    {
        /// <summary>
        /// A human friendly description of the error.
        /// </summary>
        [JsonPropertyName("detail")]
        public string Detail { get; set; }

        /// <summary>
        /// A status code corresponding to this error.
        /// </summary>
        [JsonPropertyName("status")]
        public int Status { get; set; }

        /// <summary>
        /// The location where this error has occurred.
        /// </summary>
        [JsonPropertyName("source")]
        public AbuseIPDBErrorSource Source { get; set; }

        /// <summary>
        /// The index of this error.
        /// </summary>
        public int Index { get; set; }
    }

    /// <summary>
    /// A location where this error has occurred.
    /// </summary>
    public class AbuseIPDBErrorSource
    {
        /// <summary>
        /// The API parameter that caused this error.
        /// </summary>
        [JsonPropertyName("parameter")]
        public string Parameter { get; set; }
    }

    /// <summary>
    /// The result of a clear address request.
    /// </summary>
    public class ClearedAddress
    {
        /// <summary>
        /// How many reports were deleted.
        /// </summary>
        [JsonPropertyName("numReportsDeleted")]
        public int ReportsDeleted { get; set; }
    }

    /// <summary>
    /// A container for <see cref="ClearedAddress"/>.
    /// </summary>
    public class ClearedAddressContainer
    {
        [JsonPropertyName("data")]
        public ClearedAddress Data { get; set; }
    }

    /// <summary>
    /// The class representing an invalid bulk report.
    /// </summary>
    public class InvalidReport
    {
        /// <summary>
        /// The error that was caused.
        /// </summary>
        [JsonPropertyName("error")]
        public string Error { get; set; }

        /// <summary>
        /// The input that was provided.
        /// </summary>
        [JsonPropertyName("input")]
        public string Input { get; set; }

        /// <summary>
        /// The row number where the error occurred.
        /// </summary>
        [JsonPropertyName("rowNumber")]
        public int RowNumber { get; set; }
    }

    /// <summary>
    /// The result of a bulk report request.
    /// </summary>
    public class BulkReport
    {
        /// <summary>
        /// How many requests were successfully saved.
        /// </summary>
        [JsonPropertyName("savedReports")]
        public int SavedReports { get; set; }

        /// <summary>
        /// An array of <see cref="InvalidReport"/> with all reported IPs that were rejected.
        /// </summary>
        [JsonPropertyName("invalidReports")]
        public InvalidReport[] InvalidReports { get; set; }
    }

    /// <summary>
    /// A container for <see cref="BulkReport"/>.
    /// </summary>
    public class BulkReportContainer
    {
        [JsonPropertyName("data")]
        public BulkReport Data { get; set; }
    }

    /// <summary>
    /// A reported IP address
    /// </summary>
    public class CheckedBlockIp
    {
        /// <summary>
        /// The IP address that was reported.
        /// </summary>
        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// How many reports this IP address has.
        /// </summary>
        [JsonPropertyName("numReports")]
        public int ReportCount { get; set; }

        /// <summary>
        /// When was the latest report submitted for this IP address.
        /// </summary>
        [JsonPropertyName("mostRecentReport")]
        public DateTime MostRecentReport { get; set; }

        /// <summary>
        /// The abuse confidence score, as percentage.
        /// </summary>
        [JsonPropertyName("abuseConfidenceScore")]
        public int AbuseConfidence { get; set; }

        /// <summary>
        /// The <c>ISO 3166 alpha-2</c> country code where this IP address is located.
        /// </summary>
        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; }
    }

    /// <summary>
    /// The result of a check block request.
    /// </summary>
    public class CheckedBlock
    {
        /// <summary>
        /// The network address, example: <c>186.2.163.0</c>.
        /// </summary>
        [JsonPropertyName("networkAddress")]
        public string NetworkAddress { get; set; }

        /// <summary>
        /// The netmask of this network, example: <c>255.255.255.0</c>.
        /// </summary>
        [JsonPropertyName("netmask")]
        public string Netmask { get; set; }

        /// <summary>
        /// The minimum IP address in this range, example: <c>186.2.163.1</c>.
        /// </summary>
        [JsonPropertyName("minAddress")]
        public string MinAddress { get; set; }

        /// <summary>
        /// The maximum IP address in this range, example: <c>186.2.163.254</c>.
        /// </summary>
        [JsonPropertyName("maxAddress")]
        public string MaxAddress { get; set; }

        /// <summary>
        /// How many possible hosts there can be in the network, example: <c>254</c>.
        /// </summary>
        [JsonPropertyName("numPossibleHosts")]
        public int PossibleHostCount { get; set; }

        /// <summary>
        /// The description of this address space, example: <c>Internet</c>.
        /// </summary>
        [JsonPropertyName("addressSpaceDesc")]
        public string AddressSpace { get; set; }

        /// <summary>
        /// An array of <see cref="CheckedBlockIp"/> with all recently reported IP addresses in this network.
        /// </summary>
        [JsonPropertyName("reportedAddress")]
        public CheckedBlockIp[] ReportedIps { get; set; }
    }

    /// <summary>
    /// A container for <see cref="CheckedBlock"/>.
    /// </summary>
    public class CheckedBlockContainer
    {
        [JsonPropertyName("data")]
        public CheckedBlock Data { get; set; }
    }

    /// <summary>
    /// A payload for submitting an IP address report.
    /// </summary>
    public class IpReportPayload
    {
        /// <summary>
        /// The IP address to report.
        /// </summary>
        [JsonPropertyName("ip")]
        public string IpAddress { get; set; }

        /// <summary>
        /// A comma separated string with IDs of matching abuse categories.
        /// </summary>
        [JsonPropertyName("categories")]
        public string Categories { get; set; }

        /// <summary>
        /// A comment describing the abusive activity. Should be shorter than or equal to 1024 characters.
        /// </summary>
        [JsonPropertyName("comment")]
        public string Comment { get; set; }
    }

    /// <summary>
    /// The result of an IP reporting request.
    /// </summary>
    public class ReportedIp
    {
        /// <summary>
        /// The IP address that has just been reported.
        /// </summary>
        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// The newly calculated IP address abuse confidence score as percentage.
        /// </summary>
        [JsonPropertyName("abuseConfidenceScore")]
        public int AbuseConfidence { get; set; }
    }

    /// <summary>
    /// A container for <see cref="ReportedIp"/>.
    /// </summary>
    public class ReportedIpContainer
    {
        [JsonPropertyName("data")]
        public ReportedIp Data { get; set; }
    }

    /// <summary>
    /// A blacklisted IP address.
    /// </summary>
    public class BlacklistedIp
    {
        /// <summary>
        /// The blacklisted IP address.
        /// </summary>
        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// The <c>ISO 3166 alpha-2</c> country code where this IP address is located.
        /// </summary>
        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; }

        /// <summary>
        /// The abuse confidence score for this IP address as percentage.
        /// </summary>
        [JsonPropertyName("abuseConfidenceScore")]
        public int AbuseConfidence { get; set; }

        /// <summary>
        /// When was this IP address last reported.
        /// </summary>
        [JsonPropertyName("lastReportedAt")]
        public DateTime LastReportedAt { get; set; }
    }

    /// <summary>
    /// Metadata information about this blacklist.
    /// </summary>
    public class BlacklistMeta
    {
        /// <summary>
        /// When was this blacklist last generated at.
        /// </summary>
        [JsonPropertyName("generatedAt")]
        public DateTime GeneratedAt { get; set; }
    }

    /// <summary>
    /// A container for <see cref="BlacklistMeta"/> and <see cref="BlacklistedIp"/>.
    /// </summary>
    public class BlacklistContainer
    {
        [JsonPropertyName("meta")]
        public BlacklistMeta Meta { get; set; }

        [JsonPropertyName("data")]
        public BlacklistedIp[] Data { get; set; }
    }

    /// <summary>
    /// A page of reports for an IP address.
    /// </summary>
    public class IpReportsPage
    {
        /// <summary>
        /// How many reports there are in total.
        /// </summary>
        [JsonPropertyName("total")]
        public int Total { get; set; }

        /// <summary>
        /// The number of this page.
        /// </summary>
        [JsonPropertyName("page")]
        public int Page { get; set; }

        /// <summary>
        /// The number of the last page.
        /// </summary>
        [JsonPropertyName("lastPage")]
        public int LastPage { get; set; }

        /// <summary>
        /// The amount of reports in this page.
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; }

        /// <summary>
        /// How many reports are returned per page.
        /// </summary>
        [JsonPropertyName("perPage")]
        public int PerPage { get; set; }

        /// <summary>
        /// The URL to access the next page.
        /// </summary>
        [JsonPropertyName("nextPageUrl")]
        public string NextPageUrl { get; set; }

        /// <summary>
        /// The URL to acess the previous page.
        /// </summary>
        [JsonPropertyName("previousPageUrl")]
        public string PreviousPageUrl { get; set; }

        /// <summary>
        /// An <see cref="IpReport"/> array with the data for this page.
        /// </summary>
        [JsonPropertyName("results")]
        public IpReport[] Results { get; set; }
    }

    /// <summary>
    /// A container for <see cref="IpReportsPage"/>.
    /// </summary>
    public class IpReportsContainer
    {
        [JsonPropertyName("data")]
        public IpReportsPage Data { get; set; }
    }

    /// <summary>
    /// A result of an IP check request.
    /// </summary>
    public class CheckedIp
    {
        /// <summary>
        /// The IP address that's being checked.
        /// </summary>
        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        /// <summary>
        /// Whether this is a public or private IP address.
        /// </summary>
        [JsonPropertyName("isPublic")]
        public bool IsPublic { get; set; }

        /// <summary>
        /// The version of this IP address (4/6).
        /// </summary>
        [JsonPropertyName("ipVersion")]
        public int IpVersion { get; set; }

        /// <summary>
        /// <para>Whether this is a whitelisted IP address.</para>
        /// Whitelisted netblocks are typically owned by trusted entities, such as Google or Microsoft who may use them for search engine spiders.<br/>
        /// However, these same entities sometimes also provide cloud servers and mail services which are easily abused.<br/>
        /// Pay special attention when trusting or distrusting these IPs.<br/>
        /// <i>Source: AbuseIPDB</i>
        /// </summary>
        [JsonPropertyName("isWhitelisted")]
        public bool IsWhitelisted { get; set; }

        /// <summary>
        /// The abuse confidence score for this IP address as percentage.
        /// </summary>
        [JsonPropertyName("abuseConfidenceScore")]
        public int AbuseConfidence { get; set; }

        /// <summary>
        /// The <c>ISO 3166 alpha-2</c> country code where this IP address is located.
        /// </summary>
        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; }

        /// <summary>
        /// The country name where this IP address is located.
        /// </summary>
        [JsonPropertyName("countryName")]
        public string CountryName { get; set; }

        /// <summary>
        /// The usage type of this IP address, example: <c>Data Center/Web Hosting/Transit</c>.
        /// </summary>
        [JsonPropertyName("usageType")]
        public string UsageType { get; set; }

        /// <summary>
        /// The Internet Service Provider of this IP address.
        /// </summary>
        [JsonPropertyName("isp")]
        public string ISP { get; set; }

        /// <summary>
        /// The domain name associated with this IP address.
        /// </summary>
        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        /// <summary>
        /// The hostnames that are pointing to this IP address.
        /// </summary>
        [JsonPropertyName("hostnames")]
        public string[] Hostnames { get; set; }

        /// <summary>
        /// The total amount of reports that have been submitted for this IP address in the time period.
        /// </summary>
        [JsonPropertyName("totalReports")]
        public int TotalReports { get; set; }

        /// <summary>
        /// The amount of distinct users that have ever reported this IP address.
        /// </summary>
        [JsonPropertyName("numDistinctUsers")]
        public int DistinctUserCount { get; set; }

        /// <summary>
        /// When the newest report for this IP address has been submitted.
        /// </summary>
        [JsonPropertyName("lastReportedAt")]
        public DateTime LastReportedAt { get; set; }

        /// <summary>
        /// An array of <see cref="IpReport"/> containing the recent reports for this IP address. You can also use the <see cref="AbuseIPDBClient"/><c>.GetReports()</c> method.
        /// </summary>
        [JsonPropertyName("reports")]
        public IpReport[] Reports { get; set; }
    }

    /// <summary>
    /// An IP address report.
    /// </summary>
    public class IpReport
    {
        /// <summary>
        /// When was this report submitted.
        /// </summary>
        [JsonPropertyName("reportedAt")]
        public DateTime ReportedAt { get; set; }

        /// <summary>
        /// A short comment explaining the abusive activity from this IP address.
        /// </summary>
        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// The report categories for the abusive activity.
        /// </summary>
        [JsonPropertyName("categories")]
        public IpReportCategory[] Categories { get; set; }

        /// <summary>
        /// The ID of the reporter.<br/>
        /// You can view their profile by going to <a href="https://www.abuseipdb.com/user/">https://www.abuseipdb.com/user/</a> and appending the user ID.
        /// </summary>
        [JsonPropertyName("reporterId")]
        public int ReporterId { get; set; }

        /// <summary>
        /// The <c>ISO 3166 alpha-2</c> country code of the reporter.
        /// </summary>
        [JsonPropertyName("reporterCountryCode")]
        public string ReporterCountryCode { get; set; }

        /// <summary>
        /// The country name of the reporter.
        /// </summary>
        [JsonPropertyName("reporterCountryName")]
        public string ReporterCountryName { get; set; }
    }

    /// <summary>
    /// A container for <see cref="CheckedIp"></see>.
    /// </summary>
    public class CheckedIpContainer
    {
        [JsonPropertyName("data")]
        public CheckedIp Data { get; set; }
    }
}