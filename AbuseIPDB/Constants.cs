using System;

namespace AbuseIPDB
{
    internal class Constants
    {
        /// <summary>
        /// The version of the API to send requests to.
        /// </summary>
        public const int Version = 2;
        /// <summary>
        /// The preferred HTTP request version to use.
        /// </summary>
        public static readonly Version HttpVersion = new(2, 0);
        /// <summary>
        /// The <c>User-Agent</c> header value to send along requests.
        /// </summary>
        public const string UserAgent = "AbuseIPDB C# Client - actually-akac/AbuseIPDB";
        /// <summary>
        /// The base URI to send requests to.
        /// </summary>
        public static readonly Uri BaseUri = new($"https://api.abuseipdb.com/api/v{Version}/");
        /// <summary>
        /// The maximum string length when displaying a preview of a response body.
        /// </summary>
        public const int PreviewMaxLength = 500;
    }
}