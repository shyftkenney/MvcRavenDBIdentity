using MvcRavenDBIdentity.Models;
using System;
using System.Linq;

namespace MvcRavenDBIdentity.Infrastructure.Guards
{
    public static class AuthorizationGuard
    {
        public static void AffirmClaim(AppUser user, string claim)
        {
            if (string.IsNullOrWhiteSpace(claim))
                throw new ArgumentException(nameof(claim));

            var claims = user.Claims.Select(x => x.ClaimValue).ToArray();
            if (claims.Length == 0)
                throw new UnauthorizedAccessException();

            var exists = claims.Contains(claim);

            if (exists == false)
                throw new UnauthorizedAccessException();
        }
    }
}
