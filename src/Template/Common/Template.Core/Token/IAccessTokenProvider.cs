using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Template.Domain;

namespace Template.Core.Token
{
    public interface IAccessTokenProvider
    {
        Task<Result<ClaimsPrincipal>> ValidateTokenAsync(HttpRequest request);
    }
}
