# AbuseIPDB ⛔

![](https://www.abuseipdb.com/img/abuseipdb-logo.svg)

An async C# library for interacting with the v2 AbuseIPDB API.

## Usage
Available on NuGet as `AbuseIPDB`, methods can be found under the class `AbuseIPDBClient`.

Create your own API key: https://www.abuseipdb.com/account/api

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
CheckedIP check = await client.Check("1.1.1.1", true, 90);
```

### Reporting an abusive IP
```csharp
ReportedIP report = await client.Report("127.0.0.1", new IPReportCategory[] { IPReportCategory.WebSpam, IPReportCategory.SSH }, "Test Report");
```

### Bulk-reporting many IPs at once with a CSV stream
```csharp
BulkReport bulkReport = await client.BulkReport(stream);
```

### Requesting reports for an IP
```csharp
IPReport[] reports = await client.GetReports("91.240.118.222", 300, 90);
```

### Downloading a blacklist
```csharp
BlacklistedIP[] ips = await client.GetBlacklist(10000);
```

### Checking a CIDR block for recently reported IPs
```csharp
CheckedBlock checkedBlock = await client.CheckBlock("186.2.163.0/24", 30);
```

### Clearing reports on an IP
```csharp
ClearedAddress cleared = await client.ClearAddress("127.0.0.1");
```

## Available methods
- Task\<BlacklistedIP[]> **GetBlacklist**( int limit = 10000, int? confidenceMinimum = null, string[] onlyCountries = null, string[] exceptCountries = null)
- Task\<BulkReport> **BulkReport**(Stream csvStream)
- Task\<ClearedAddress> **ClearAddress**(string ip)
- Task\<CheckedBlock> **CheckBlock**(string network, int maxAge)
- Task\<CheckedIP> **Check**(string ip, bool verbose = true, int maxAge = 90)
- Task\<IPReport[]> **GetReports**(string ip, int limit = 100, int maxAge = 90)
- Task\<ReportedIP> **Report**(string ip, IPReportCategory[] categories, string comment)

## Resources
Website: https://www.abuseipdb.com

*This is a community-ran library. Not affiliated with Marathon Studios, Inc.*