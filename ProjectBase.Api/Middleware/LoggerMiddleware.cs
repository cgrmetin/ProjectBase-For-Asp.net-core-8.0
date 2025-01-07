using ProjectBase.Business.Concrete;
using ProjectBase.DTO.Middleware;
using ProjectBase.Entity.Attributes;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Text;

namespace ProjectBase.Api.Middleware
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly IConfiguration _configuration;
        private readonly LoggerService _loggerService;

        public LoggerMiddleware(RequestDelegate requestDelegate, IConfiguration configuration)
        {

            _requestDelegate = requestDelegate;
            _configuration = configuration;

            var loggerRepository = LogManager.GetRepository(Assembly.GetExecutingAssembly());
            XmlConfigurator.Configure(loggerRepository, new FileInfo("log4net.config"));
            _loggerService = new LoggerService();
        }

        public async Task Invoke(HttpContext context)
        {
            var requestTime = DateTime.Now;
            var requestBody = await FormatRequest(context.Request);

            var isLogDisabled = context.Features.Get<IEndpointFeature>().Endpoint.Metadata.Any(p => p is IgnoreFromLogAttribute);

            if (isLogDisabled)
            {
                return;
            }
            try
            {
                if (context.GetEndpoint() != null)
                {
                    await _requestDelegate(context);
                    var orginalBodyStream = context.Response.Body;

                    using (var responseBody = new MemoryStream())
                    {
                        context.Response.Body = responseBody;

                        var response = await FormatResponse(context.Response);

                        var log = new LogDto
                        {
                            AppUser = context.User?.Identity?.Name,
                            Machine = Environment.MachineName,
                            RequestContentBody = requestBody,
                            RequestContentType = context.Request.ContentType,
                            RequestHeaders = JsonConvert.SerializeObject(context.Request.Headers),
                            RequestIpAddress = context.Connection.RemoteIpAddress.ToString(),
                            RequestMethod = context.Request.Method,
                            RequestTimestamp = requestTime,
                            RequestUri = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}",
                            ResponseContentBody = response,
                            ResponseContentType = context.Response.ContentType,
                            ResponseHeaders = JsonConvert.SerializeObject(context.Response.Headers),
                            ResponseStatusCode = context.Response.StatusCode,
                            ResponseTimestamp = DateTime.Now,
                            TransactionId = context.TraceIdentifier
                        };
                        _loggerService.UsageLog(log);
                    }
                }
                else
                {
                    await _requestDelegate(context);
                }
            }
            catch (Exception ex)
            {
                var response = JsonConvert.SerializeObject(ex, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

                var errorLog = new LogDto
                {
                    AppUser = context.User?.Identity?.Name,
                    Machine = Environment.MachineName,
                    RequestContentBody = requestBody,
                    RequestContentType = context.Request.ContentType,
                    RequestHeaders = JsonConvert.SerializeObject(context.Request.Headers),
                    RequestIpAddress = context.Connection.RemoteIpAddress.ToString(),
                    RequestMethod = context.Request.Method,
                    RequestTimestamp = requestTime,
                    RequestUri = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}",
                    ResponseContentBody = JsonConvert.SerializeObject(ex),
                    ResponseContentType = context.Response.ContentType,
                    ResponseHeaders = JsonConvert.SerializeObject(context.Response.Headers),
                    ResponseStatusCode = context.Response.StatusCode,
                    ResponseTimestamp = DateTime.Now,
                    TransactionId = context.TraceIdentifier
                };

                _loggerService.Error(errorLog);
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Allow-Methods", "*");
                context.Response.Headers.Add("Access-Control-Allow-Headers", "*");
            }
        }
        private async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering();
            var body = request.Body;


            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            body.Seek(0, SeekOrigin.Begin);
            request.Body = body;

            return bodyAsText;
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return responseBody;
        }
    }
}
