using AbuseIPDB;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Text;

namespace Example
{
    public static class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("You need an API key to be able to interact with the API. Create one at https://www.abuseipdb.com/account/api.");
            Console.Write("Enter your API key: ");
            string key = Console.ReadLine();

            AbuseIPDBClient client = new(key);

            Console.WriteLine($"> Checking IP address");
            IpCheckResult check = await client.Check("1.1.1.1", true, 90);

            Console.WriteLine($"IP: {check.IpAddress}");
            Console.WriteLine($"Is Public: {check.IsPublic}");
            Console.WriteLine($"Is Whitelisted: {check.IsWhitelisted}");
            Console.WriteLine($"Abuse Confidence: {check.AbuseConfidence}%");
            Console.WriteLine($"Country: {check.CountryName} ({check.CountryCode})");
            Console.WriteLine($"ISP: {check.ISP}");
            Console.WriteLine($"Usage Type: {check.UsageType}");
            Console.WriteLine($"Domain: {check.Domain}");
            Console.WriteLine($"Hostnames: {string.Join(", ", check.Hostnames)}");
            Console.WriteLine($"Total Reports: {check.TotalReports} from {check.DistinctUserCount} distinct users");
            Console.WriteLine($"Last Report: {check.LastReportedAt}");

            if (check.Reports is not null)
            {
                Console.WriteLine();
                Console.WriteLine($"Preview of {check.Reports.Length} reports");

                foreach (IpReport ipReport in check.Reports)
                {
                    string preview = ipReport.Comment.Trim().Replace("\n", "");
                    Console.WriteLine($"Reported from {ipReport.ReporterCountryName} ({ipReport.ReporterCountryCode}){(string.IsNullOrEmpty(preview) ? "" : $" for '{preview}'")}");
                }
            }

            Console.WriteLine();
            Console.WriteLine($"> Requesting up to the first 300 reports of an IP");
            IpReport[] reports = await client.GetReports("91.240.118.222", 300, 90);

            Console.WriteLine($"Received {reports.Length} reports, submitted from the following countries: {string.Join(", ", reports.Select(x => x.ReporterCountryCode).Distinct().OrderBy(x => x))}");

            Console.WriteLine();
            Console.WriteLine($"> Downloading a 10k IP blacklist and saving it into 'blacklist.txt'");
            BlacklistedIp[] ips = await client.GetBlacklist(10000);

            File.WriteAllLines("blacklist.txt", ips.Select(x => x.IpAddress));
            
            Console.WriteLine();
            Console.WriteLine($"> Reporting an IP address");

            IpReportResult report;
            try
            {
                report = await client.Report("127.0.0.1", new IpReportCategory[] { IpReportCategory.WebSpam, IpReportCategory.SSH }, "Test Report");
                Console.WriteLine($"Successfully reported {report.IpAddress}, abuse confidence score: {report.AbuseConfidence}");
            }
            catch (AbuseIPDBException ex)
            {
                Console.WriteLine($"Received an API exception while attempting to submit a report.");

                if (ex.Errors is null) Console.WriteLine(ex.Message);
                else
                {
                    //API error enumeration so that you can easily programatically determine the type of error and act accordingly

                    //Status '429' (Too Many Requests) means that you were ratelimited for exceeding the API quota.
                    //In that case, you can take advantage of the <AbuseIPDBError>.RetryAfter property.
                    //Keep in mind this is nullable, as 'Retry-After' is only provided for daily API ratelimits.

                    foreach (AbuseIPDBError err in ex.Errors)
                    {
                        Console.WriteLine($"[#{err.Index + 1}] ({err.Status}) - {err.Detail}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Received a generic exception while attempting to submit a report.");
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine($"> Checking a CIDR block for recently reported IP addresses");
            CheckBlockResult checkBlock = await client.CheckBlock("186.2.163.0/24", 30);
            Console.WriteLine($"Network Address: {checkBlock.NetworkAddress}");
            Console.WriteLine($"Netmask: {checkBlock.Netmask}");
            Console.WriteLine($"Range: {checkBlock.MinAddress} - {checkBlock.MaxAddress}");
            Console.WriteLine($"Possible Hosts: {checkBlock.PossibleHostCount}");
            Console.WriteLine($"Address Space: {checkBlock.AddressSpace}");

            Console.WriteLine($"Reported Addresses: {checkBlock.ReportedIps.Length}");
            for (int i = 0; i < checkBlock.ReportedIps.Length; i++)
            {
                CheckBlockReportedIp ip = checkBlock.ReportedIps[i];
                Console.WriteLine($"[#{i + 1}] - {ip.IpAddress} with {ip.AbuseConfidence}% abuse confidence");
            }

            Console.WriteLine();
            Console.WriteLine($"> Bulk-Reporting a chunk of testing IP addresses.");

            //For showcase purposes, we're going to artificially create a stream here. In a real world scenario, you would use File.OpenRead.
            //Learn about the CSV file syntax: https://www.abuseipdb.com/bulk-report
            string csv =
                "IP,Categories,ReportDate,Comment" +
                "\n127.0.0.1,\"4,5,7\",2022-09-17T00:00:00+0000,\"Test Bulk-Report\"" +
                "\n127.0.0.2,\"4,5,7\",2022-09-17T00:00:00+0000,\"Test Bulk-Report\"" +
                "\n127.0.0.3,\"4,5,7\",2022-09-17T00:00:00+0000,\"Test Bulk-Report\"" +
                "\n127.0.0.4,\"4,5,7\",2022-09-17T00:00:00+0000,\"Test Bulk-Report\"" +
                "\n127.0.0.5,\"4,5,7\",2022-09-17T00:00:00+0000,\"Test Bulk-Report\"";

            MemoryStream stream = new(Encoding.UTF8.GetBytes(csv));
            BulkReportResult bulkReport = await client.BulkReport(stream);

            Console.WriteLine($"Successfully bulk-reported IPs, saved reports: {bulkReport.SavedReports}");
            Console.WriteLine($"Invalid reports: {bulkReport.InvalidReports.Length}");

            for (int i = 0; i < bulkReport.InvalidReports.Length; i++)
            {
                InvalidReport invalid = bulkReport.InvalidReports[i];

                Console.WriteLine($"[#{i + 1}] - {invalid.Input} at row {invalid.RowNumber} was rejected with reason: {invalid.Error}");
            }

            Console.WriteLine();
            Console.WriteLine($"> Clearing reports on 127.0.0.1-127.0.0.5.");

            int reportsDeleted = 0;
            for (int i = 1; i <= 5; i++)
            {
                ClearAddressResult clearAddress = await client.ClearAddress($"127.0.0.{i}");
                reportsDeleted += clearAddress.ReportsDeleted;
            }

            Console.WriteLine($"Successfully deleted {reportsDeleted} test reports.");
            
            Console.ReadKey();
        }
    }
}