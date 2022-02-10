using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Template.Domain;
using Template.Core.Token;
using System;
using Template.ProcessApis.Api1.Services;
using Template.Domain.Constants;
using Newtonsoft.Json;
using Template.ProcessApis.Api1.RequestModel;

namespace Template.ProcessApis.Api1.Functions
{
    internal class FunctionApiPost
    {
        private readonly IAccessTokenProvider _tokenProvider;
        private readonly IService1 _service1;

        public FunctionApiPost(IAccessTokenProvider tokenProvider, IService1 service1)
        {
            _tokenProvider = tokenProvider ?? throw new ArgumentException(nameof(tokenProvider));
            _service1 = service1 ?? throw new ArgumentException(nameof(service1));
        }

        [FunctionName("FunctionApiPost")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "functionapipost/{numeroCommande:int}")] HttpRequest request,
            [FromRoute] int numeroCommande,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger for FunctionApi.");

            string requestBody;
            Result authorizationResult;
            OrderRequest order;

            if (!(authorizationResult = await _tokenProvider.ValidateTokenAsync(request)).IsSuccess)
            {
                return new ObjectResult(new { IsSuccess = false, authorizationResult.Message }) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            if (string.IsNullOrWhiteSpace(requestBody = await new StreamReader(request.Body).ReadToEndAsync()))
            {
                return new BadRequestObjectResult(new { IsSuccess = false, Message = Errors.POST_EMPTY_REQUEST });
            }
            log.LogInformation(requestBody);

            if((order = JsonConvert.DeserializeObject<OrderRequest>(requestBody)) == null)
            {
                return new BadRequestObjectResult(new { IsSuccess = false, Message = Errors.POST_BAD_REQUEST_CONTENT });
            }

            try
            {
                return new OkObjectResult(_service1.Save(order));
            }
            catch (Exception ex)
            {

                string errorMessage = $"Erreur lors de l'enregistrement";
                log.LogError(ex, errorMessage);
                return new BadRequestObjectResult(new Result()
                {
                    IsSuccess = false,
                    Message = errorMessage
                });
            }
        }
    }
}
