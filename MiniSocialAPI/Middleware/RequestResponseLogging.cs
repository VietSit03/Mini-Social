using System.Diagnostics;
using System.Text;

namespace MiniSocialAPI.MiniSocialAPI.Middleware
{
    public class RequestResponseLogging
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLogging> _logger;

        public RequestResponseLogging(RequestDelegate next, ILogger<RequestResponseLogging> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            context.Request.EnableBuffering();
            string requestBody = await ReadRequestBody(context.Request);
            context.Request.Body.Position = 0;

            // Capture response
            var originalResponseBody = context.Response.Body;
            await using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            await _next(context); // Invoke next middleware

            stopwatch.Stop();

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(originalResponseBody);

            var logObject = new
            {
                Method = context.Request.Method,
                Path = context.Request.Path,
                StatusCode = context.Response.StatusCode,
                ElapsedMs = stopwatch.ElapsedMilliseconds,
                RequestBody = requestBody,
                ResponseBody = responseBody
            };

            if (context.Response.StatusCode >= 400)
            {
                _logger.LogWarning("HTTP {Method} {Path} => {StatusCode} ({ElapsedMs}ms)\nRequest: {RequestBody}\nResponse: {ResponseBody}",
                    logObject.Method, logObject.Path, logObject.StatusCode, logObject.ElapsedMs,
                    logObject.RequestBody, logObject.ResponseBody);
            }
            else
            {
                _logger.LogInformation("HTTP {Method} {Path} => {StatusCode} ({ElapsedMs}ms)",
                    logObject.Method, logObject.Path, logObject.StatusCode, logObject.ElapsedMs);
            }
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);
            return body;
        }
    }
}
