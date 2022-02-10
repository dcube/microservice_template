using Microsoft.Azure.Services.AppAuthentication;
using System.Net.Http.Headers;

namespace Template.Core.Handlers
{
    public class AuthorizationHandler : DelegatingHandler
    {
        private readonly string _functionKey;

        public AuthorizationHandler(string functionKey)
        {
            _functionKey = functionKey;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            string accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(resource: "https://management.azure.com", cancellationToken: cancellationToken);


            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            if (!request.Headers.Contains("x-functions-key"))
            {
                request.Headers.Add("x-functions-key", _functionKey);
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
