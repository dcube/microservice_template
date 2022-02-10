using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Template.Domain;
using Template.Domain.Constants;

namespace Template.Core.Token
{
    public class AccessTokenProvider : IAccessTokenProvider
    {
        private readonly string issuer;
        private readonly string audience;
        private readonly string wellKnownEndpoint;

        private readonly ILogger<AccessTokenProvider> logger;

        public AccessTokenProvider(IConfiguration configuration, ILogger<AccessTokenProvider> logger)
        {
            string issuer = configuration[GlobalConfigurationKeys.ISSUER] ?? throw new ArgumentNullException(nameof(issuer));
            string tenantId = configuration[GlobalConfigurationKeys.TENANT] ?? throw new ArgumentNullException(nameof(tenantId));
            string audience = configuration[GlobalConfigurationKeys.AUDIENCE] ?? throw new ArgumentNullException(nameof(audience));
            string instance = configuration[GlobalConfigurationKeys.INSTANCE] ?? throw new ArgumentNullException(nameof(instance));

            this.issuer = $"{issuer}{tenantId}/";
            this.audience = audience;
            this.wellKnownEndpoint = $"{instance}{tenantId}/v2.0/.well-known/openid-configuration";

            this.logger = logger;
        }

        public async Task<Result<ClaimsPrincipal>> ValidateTokenAsync(HttpRequest request)
        {
            var authorizationHeader = (string)request.Headers["Authorization"];

            var result = new Result<ClaimsPrincipal>()
            {
                IsSuccess = false,
                Message = "Le token n'est pas valide"
            };


#if DEBUG
            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                return new Result<ClaimsPrincipal>()
                {
                    IsSuccess = true
                };
            }
#endif
            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                result.Message = Errors.AUTHENTICATION_HEADER_EMPTY;
                logger.LogError(result.Message);

            }
            else if (!authorizationHeader.ToLower().StartsWith("bearer"))
            {
                result.Message = Errors.AUTHENTICATION_HEADER_BAD_FORMAT;
                logger.LogError(result.Message);
            }
            else
            {
                try
                {
                    var oidcWellknownEndpoints = await GetOIDCWellknownConfigurationAsync();

                    var token = authorizationHeader.Replace("bearer ", "", StringComparison.OrdinalIgnoreCase);

                    var handler = new JwtSecurityTokenHandler();
                    var tokenResult = handler.ValidateToken(token, new TokenValidationParameters()
                    {
                        RequireSignedTokens = true,
                        ValidAudience = audience,
                        ValidateAudience = true,
                        ValidIssuer = issuer,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        IssuerSigningKeys = oidcWellknownEndpoints.SigningKeys
                    }, out var securityToken);

                    if (tokenResult != null && tokenResult.Identity != null && tokenResult.Identity.IsAuthenticated)
                    {
                        result.Value = tokenResult;
                        result.IsSuccess = true;
                    }
                }
                catch (SecurityTokenInvalidAudienceException ex)
                {
                    result.Message = Errors.AUTHENTICATION_TOKEN_INVALID_AUDIENCE;
                    logger.LogError(ex, result.Message);
                }
                catch (SecurityTokenDecryptionFailedException ex)
                {
                    result.Message = Errors.AUTHENTICATION_TOKEN_BAD_ENCRYPTION;
                    logger.LogError(ex, result.Message);
                }
                catch (SecurityTokenExpiredException ex)
                {
                    result.Message = Errors.AUTHENTICATION_TOKEN_EXPIRED;
                    logger.LogError(ex, result.Message);
                }
                catch (Exception ex)
                {
                    result.Message = Errors.AUTHENTICATION_UNKNOWN_ERROR;
                    logger.LogError(ex, result.Message);
                }
            }

            return result;
        }

        private async Task<OpenIdConnectConfiguration> GetOIDCWellknownConfigurationAsync()
        {
            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(wellKnownEndpoint, new OpenIdConnectConfigurationRetriever());

            return await configurationManager.GetConfigurationAsync();
        }
    }
}
