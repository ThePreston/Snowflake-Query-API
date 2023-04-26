using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Net.Http;
using Microsot.Snowflake.Services.Common;
using Microsot.Snowflake.Services;
using Microsoft.Snowflake.API.Models;
using System.Collections.Generic;

namespace Microsoft.Snowflake.API
{
    public class Request
    {
        private readonly ILogger<Request> _logger;

        private readonly IRepoService _svc;

        public Request(ILogger<Request> log, IRepoService _repoService)
        {
            _logger = log;
            _svc = _repoService;
        }

        [FunctionName("GetData")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "GetData" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ReqBody), Required = true, Description =  "The **queryToExecute** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<IDictionary<string, object>>), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(Exception), Description = "Exception")]
        public async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger GetData");

            try
            {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                
                var data = JsonConvert.DeserializeObject<ReqBody>(requestBody);
                
                if (data?.queryToExecute is null)
                    return HttpUtilities.RESTResponse(data?.queryToExecute);

                return HttpUtilities.RESTResponse(_svc.GetData(data.queryToExecute));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return HttpUtilities.RESTResponse(ex);

            }

        }

        [FunctionName("Execute")]
        [OpenApiOperation(operationId: "execute", tags: new[] { "Execute" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ReqBody), Required = true, Description = "The **queryToExecute** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(int), Description = "The OK response")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(System.Exception), Description = "Exception")]
        public async Task<HttpResponseMessage> Execute(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger Execute.");

            try
            {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var data = JsonConvert.DeserializeObject<ReqBody>(requestBody);

                if (data?.queryToExecute is null)
                    return HttpUtilities.RESTResponse(data?.queryToExecute);

                return HttpUtilities.RESTResponse(_svc.Execute(data.queryToExecute));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return HttpUtilities.RESTResponse(ex);

            }

        }
    }
}