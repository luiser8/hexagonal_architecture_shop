using System.Security.Claims;

namespace HexagonalShop.API.Middleware;

public static class IdentityMiddleware {
    public static string Get(IEnumerable<ClaimsIdentity> values) {
        ArgumentNullException.ThrowIfNull(values);
        var identityUserId = values?.FirstOrDefault()?.Claims?.FirstOrDefault()?.Value ?? string.Empty;
        return identityUserId ?? "";
    }
}