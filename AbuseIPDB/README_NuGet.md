# AbuseIPDB ⛔

![](https://raw.githubusercontent.com/actually-akac/AbuseIPDB/master/AbuseIPDB/icon.svg)

An async C# library for interacting with the v2 AbuseIPDB API.

## Usage
Available on NuGet as `AbuseIPDB`, methods can be found under the class `AbuseIPDBClient`.

You will need to create your own API key: https://www.abuseipdb.com/account/api

https://www.nuget.org/packages/AbuseIPDB

## Features
- Made with **.NET 6**
- Fully **async**
- Full coverage of the **FREE** and **Premium** v2 API
- Deep **documentation**
- **No external dependencies** (uses integrated HTTP and JSON)
- Easily lookup IP addresses and **report abuse**
- **Custom exceptions** (`AbuseIPDBException`) for advanced catching
- Parsing of server errors
- Automatic request retries
- Example project to demonstrate all capabilities of the library

## Example
Under the `Example` directory you can find a working demo project that implements this library.

## Code Samples

### Creating a new AbuseIPDB API client
```csharp
AbuseIPDBClient client = new("API KEY");
```

### Checking an IP
```csharp
IpCheckResult check = await client.Check("1.1.1.1", true, 90);
```

### Reporting an abusive IP
```csharp
IpReportResult report = await client.Report("127.0.0.1", new IpReportCategory[] { IpReportCategory.WebSpam, IpReportCategory.SSH }, "Test Report");
```

### Bulk-reporting many IPs at once
```csharp
BulkReportResult bulkReport = await client.BulkReport(stream);
```

### Requesting reports for an IP
```csharp
IpReport[] reports = await client.GetReports("91.240.118.222", 300, 90);
```

### Downloading a blacklist
```csharp
BlacklistedIp[] ips = await client.GetBlacklist(10000);
```

### Checking a CIDR block for recently reported IPs
```csharp
CheckBlockResult checkBlock = await client.CheckBlock("186.2.163.0/24", 30);
```

### Clearing reports on an IP
```csharp
ClearAddressResult clearAddress = await client.ClearAddress("127.0.0.1");
```

## Available methods
- Task\<BlacklistedIp[]> GetBlacklist(int limit = 10000, int? confidenceMinimum = null, string[] onlyCountries = null, string[] exceptCountries = null)
- Task\<BulkReportResult> BulkReport(Stream csvStream)
- Task\<CheckBlockResult> CheckBlock(string network, int maxAge)
- Task\<ClearAddressResult> ClearAddress(string ip)
- Task\<IpCheckResult> Check(string ip, bool verbose = true, int maxAge = 90)
- Task\<IpReportResult> Report(string ip, IpReportCategory[] categories, string comment)
- Task\<IpReport[]> GetReports(string ip, int limit = 100, int maxAge = 90)

## Resources
* Website: https://www.abuseipdb.com
* NuGet: https://www.nuget.org/packages/AbuseIPDB