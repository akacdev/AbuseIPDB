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
        private static AbuseIPDBClient Client;

        public static async Task Main()
        {
            Console.WriteLine("You need an API key to be able to interact with the API. Create one at https://www.abuseipdb.com/account/api.");
            Console.Write("Enter your API key: ");
            string key = Console.ReadLine();
            
            Client = new(key);

            Console.WriteLine();
            Console.WriteLine($"> Checking IP address");
            CheckedIP check = await Client.Check("1.1.1.1", true, 90);
            
            Console.WriteLine($"IP: {check.IPAddress}");
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

                foreach (IPReport ipReport in check.Reports)
                {
                    string preview = ipReport.Comment.Trim().Replace("\n", "");
                    Console.WriteLine($"Reported from {ipReport.ReporterCountryName} ({ipReport.ReporterCountryCode}){(string.IsNullOrEmpty(preview) ? "" : $" for '{preview}'")}");
                }
            }

            Console.WriteLine();
            Console.WriteLine($"> Requesting up to the first 300 reports of an IP");
            IPReport[] reports = await Client.GetReports("91.240.118.222", 300, 90);

            Console.WriteLine($"Received {reports.Length} reports, submitted from the following countries: {string.Join(", ", reports.Select(x => x.ReporterCountryCode).Distinct().OrderBy(x => x))}");

            Console.WriteLine();
            Console.WriteLine($"> Downloading a 10k IP blacklist and saving it into 'blacklist.txt'");
            BlacklistedIP[] ips = await Client.GetBlacklist(10000);

            File.WriteAllLines("blacklist.txt", ips.Select(x => x.IPAddress));

            Console.WriteLine();
            Console.WriteLine($"> Reporting an IP address");

            ReportedIP report;
            try
            {
                report = await Client.Report("127.0.0.1", [IPReportCategory.WebSpam, IPReportCategory.SSH], "Test Report");
                Console.WriteLine($"Successfully reported {report.IPAddress}, abuse confidence score: {report.AbuseConfidence}");
            }
            catch (AbuseIPDBException ex)
            {
                Console.WriteLine($"Received an API exception while attempting to submit a report.");

                if (ex.Errors is null || ex.Errors.Length == 0) Console.WriteLine(ex.Message);
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
            CheckedBlock checkedBlock = await Client.CheckBlock("186.2.163.0/24", 30);

            Console.WriteLine($"Network Address: {checkedBlock.NetworkAddress}");
            Console.WriteLine($"Netmask: {checkedBlock.Netmask}");
            Console.WriteLine($"Range: {checkedBlock.MinAddress} - {checkedBlock.MaxAddress}");
            Console.WriteLine($"Possible Hosts: {checkedBlock.PossibleHostCount}");
            Console.WriteLine($"Address Space: {checkedBlock.AddressSpace}");

            Console.WriteLine($"Reported Addresses: {checkedBlock.ReportedIPs.Length}");
            for (int i = 0; i < checkedBlock.ReportedIPs.Length; i++)
            {
                CheckBlockIP ip = checkedBlock.ReportedIPs[i];
                Console.WriteLine($"[#{i + 1}] - {ip.IPAddress} with {ip.AbuseConfidence}% abuse confidence");
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
            BulkReport bulkReport = await Client.BulkReport(stream);

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
                ClearedAddress cleared = await Client.ClearAddress($"127.0.0.{i}");
                reportsDeleted += cleared.ReportsDeleted;
            }

            Console.WriteLine($"Successfully deleted {reportsDeleted} test reports.");

            Console.WriteLine();
            Console.WriteLine("Demo finished");
            Console.ReadKey();
        }
    }
}