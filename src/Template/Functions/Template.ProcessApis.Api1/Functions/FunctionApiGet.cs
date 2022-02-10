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

namespace Template.ProcessApis.Api1.Functions
{
    internal class FunctionApiGet
    {
        private readonly IAccessTokenProvider _tokenProvider;
        private readonly IService1 _service1;

        public FunctionApiGet(IAccessTokenProvider tokenProvider, IService1 service1)
        {
            _tokenProvider = tokenProvider ?? throw new ArgumentException(nameof(tokenProvider));
            _service1 = service1 ?? throw new ArgumentException(nameof(service1));
        }

        [FunctionName("FunctionApiGet")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "functionapiget/{commandNumber:int}")] HttpRequest request,
            [FromRoute] int commandNumber,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger for FunctionApiGet.");

            Result authorizationResult;
            if (!(authorizationResult = await _tokenProvider.ValidateTokenAsync(request)).IsSuccess)
            {
                return new ObjectResult(new { IsSuccess = false, authorizationResult.Message }) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            try
            {
                return new OkObjectResult(await _service1.GetAsync(commandNumber));
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
