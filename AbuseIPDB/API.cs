using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AbuseIPDB
{
    public static class API
    {
        public const int MaxRetries = 3;
        public const int RetryDelay = 1000 * 3;
        public const int ExtraDelay = 1000;
        public const int PreviewMaxLength = 500;

        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            string url,
            HttpMethod method,
            string json,
            HttpStatusCode target = HttpStatusCode.OK)
        => await Request(cl, url, method, new StringContent(json, Encoding.UTF8, "application/json"), target);

        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            string url,
            HttpMethod method,
            Stream stream,
            string fieldName,
            string fileName,
            HttpStatusCode target = HttpStatusCode.OK)
        => await Request(cl, url, method, new StreamContent(stream), target, fieldName, fileName);

        public static async Task<HttpResponseMessage> Request
        (
            this HttpClient cl,
            string path,
            HttpMethod method,
            HttpContent content = null,
            HttpStatusCode target = HttpStatusCode.OK,
            string fieldName = null,
            string fileName = null)
        {
            int retries = 0;

            HttpResponseMessage res = null;

            while (res is null || !target.HasFlag(res.StatusCode) && retries < MaxRetries)
            {
                HttpRequestMessage req = new(method, path);

                if (content is not StreamContent) req.Content = content;
                else req.Content = new MultipartFormDataContent()
                {
                    { content, fieldName, fileName }
                };

                res = await cl.SendAsync(req);

                MediaTypeHeaderValue contentType = res.Content.Headers.ContentType;
                if (contentType.MediaType != "application/json")
                {
                    bool includePreview = contentType.MediaType.StartsWith("text/");
                    string preview = null;

                    if (includePreview)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        preview = $"\nPreview: {data[..Math.Min(data.Length, PreviewMaxLength)]}";
                    }
                    
                    throw new AbuseIPDBException($"Expected response to be JSON, but received '{contentType.MediaType}'{preview}");
                }

                if (!target.HasFlag(res.StatusCode) && (int)res.StatusCode >= 500) await Task.Delay(RetryDelay);
                else break;

                retries++;
            }

            if (!target.HasFlag(res.StatusCode))
            {
                AbuseIPDBError[] errors = (await res.Deseralize<AbuseIPDBErrorContainer>())?.Errors;
                if (errors is null) throw new AbuseIPDBException($"Failed to request {method} {path}, expected one of the following status codes: {string.Join(", ", res.StatusCode.GetFlags())} but received {res.StatusCode}");

                string suffix = errors.Length == 1 ? "" : "s";

                StringBuilder sb = new();
                sb.AppendLine($"Failed to request {method} {path}, received {errors.Length} API error{suffix}.");
                for (int i = 0; i < errors.Length; i++)
                {
                    AbuseIPDBError error = errors[i];
                    error.Index = i;
                    sb.Append($"[#{i + 1}] (Status Code: {error.Status}) {error.Detail}");

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

            return res;
        }

        public static async Task<T> Deseralize<T>(this HttpResponseMessage res, JsonSerializerOptions options = null)
        {
            string json = await res.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(json)) throw new("Response content is empty, can't parse as JSON.");
            
            try
            {
                return JsonSerializer.Deserialize<T>(json, options);
            }
            catch (Exception ex)
            {
                throw new($"Exception while parsing JSON: {ex.GetType().Name} => {ex.Message}\nJSON preview: {json[..Math.Min(json.Length, PreviewMaxLength)]}");
            }
        }
    }
}