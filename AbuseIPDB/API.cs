using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AbuseIPDB
{
    internal static class API
    {
        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            HttpMethod method,
            string path,
            object obj,
            bool absoluteUrl = false)
        => await Request(cl, method, path, await obj.Serialize(), absoluteUrl: absoluteUrl);

        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            HttpMethod method,
            string path,
            Stream stream,
            string fieldName,
            string fileName,
            bool absoluteUrl = false)
        => await Request(cl, method, path, new StreamContent(stream), fieldName, fileName, absoluteUrl);

        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            HttpMethod method,
            string path,
            HttpContent content = null,
            string fieldName = null,
            string fileName = null,
            bool absoluteUrl = false)
        {
            using HttpRequestMessage req = new(method, absoluteUrl ? path : string.Concat(Constants.BaseUri, path));

            if (!string.IsNullOrEmpty(fieldName) && !string.IsNullOrEmpty(fileName))
            {
                req.Content = new MultipartFormDataContent()
                {
                    { content, fieldName, fileName }
                };
            }
            else req.Content = content;

            HttpResponseMessage res = await cl.SendAsync(req);

            MediaTypeHeaderValue contentType = res.Content.Headers.ContentType;
            if (!absoluteUrl && contentType.MediaType != "application/json")
            {
                bool includePreview = contentType.MediaType.StartsWith("text/");
                string preview = includePreview ? $"\nPreview: {await res.GetPreview()}" : null;

                throw new AbuseIPDBException($"Expected response to be JSON, but received '{contentType.MediaType}'{preview}");
            }

            if (res.IsSuccessStatusCode) return res;

            AbuseIPDBError[] errors = ((await res.Deseralize<AbuseIPDBErrorContainer>())?.Errors)
                ?? throw new AbuseIPDBException($"Failed to request {method} {path}, received status code {res.StatusCode}");

            string suffix = errors.Length == 1 ? "" : "s";
            StringBuilder sb = new();
            
            sb.AppendLine($"Failed to request {method} {path}, received {errors.Length} API error{suffix}.");
            for (int i = 0; i < errors.Length; i++)
            {
                AbuseIPDBError error = errors[i];
                error.Index = i;
                sb.Append($"[#{i + 1}] (status code: {error.Status}) {error.Detail}");

                if (error.Source is null || error.Source.Parameter is null) { sb.AppendLine(); continue; }
                sb.AppendLine($", source parameter: {error.Source.Parameter}");
            }

            AbuseIPDBException ex = new(sb.ToString(), errors);

            if (res.StatusCode == HttpStatusCode.TooManyRequests)
            {
                RetryConditionHeaderValue retry = res.Headers.RetryAfter;
                if (retry is not null) ex.RetryAfter = (int)retry.Delta.Value.TotalMilliseconds;
            }

            throw ex;
        }
    }
}