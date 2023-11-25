# AbuseIPDB ⛔

![](https://raw.githubusercontent.com/actually-akac/AbuseIPDB/master/AbuseIPDB/icon.svg)

An async C# library for interacting with the v2 AbuseIPDB API.

## Usage
This library provides an easy interface for interacting with the v2 AbuseIPDB API.
You can use this on a server to automate your malicious IP adress reports or checks.

To get started, import the library into your solution with either the NuGet Package Manager or the dotnet CLI.
```rust
dotnet add package AbuseIPDB
```

For the primary classes to become available, import the used namespace.
```csharp
using AbuseIPDB;
```

An API key is required to interact with the API. Create your own key at: https://www.abuseipdb.com/account/api

Need more examples? Under the `Example` directory you can find a working demo project that implements this library.
 
## Properties
- Built for **.NET 8**, **.NET 7** and **.NET 6**
- Fully **async**
- Complete coverage of the **FREE** and **Premium** v2 API
- Extensive **XML documentation**
- **No external dependencies** (makes use of built-in `HttpClient` and `JsonSerializer`)
- **Custom exceptions** (`AbuseIPDBException`) for easy debugging
- Parsing of custom AbuseIPDB errors
- Example project to demonstrate all capabilities of the library

## Features
- Lookup an existing IP address details and see the confidence of abuse
- Retrieve past reports
- Download a calculated IP address blacklist file
- Easily report malicious IP addresses attacking your server
- Check reported IP addresses within an entire CIDR block

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

## References
- Official website: https://www.abuseipdb.com
- API docs: https://docs.abuseipdb.com

*This is a community-ran library. Not affiliated with Marathon Studios, Inc.*