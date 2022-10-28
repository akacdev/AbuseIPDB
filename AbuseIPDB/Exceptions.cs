using System;

namespace AbuseIPDB
{
    /// <summary>
    /// A custom AbuseIPDB API exception with an <see cref="AbuseIPDBError"/> array containing all parsed API errors.
    /// </summary>
    public class AbuseIPDBException : Exception
    {
        /// <summary>
        /// Milliseconds until you can retry this request again. Provided when there is a ratelimit.
        /// </summary>
        public int? RetryAfter { get; set; }

        /// <summary>
        /// An array of user-friendly API errors.
        /// </summary>
        public AbuseIPDBError[] Errors { get; set; }

        public AbuseIPDBException(string message) : base(message) => Errors = Array.Empty<AbuseIPDBError>();
        public AbuseIPDBException(string message, AbuseIPDBError[] errors) : base(message) => Errors = errors;
    }
}