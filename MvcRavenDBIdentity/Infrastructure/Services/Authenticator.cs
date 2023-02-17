using Microsoft.AspNetCore.Http;
using MvcRavenDBIdentity.Infrastructure.Guards;
using MvcRavenDBIdentity.Models;
using Raven.Client.Documents;
using System;
using System.Linq;
using System.Security.Claims;

namespace MvcRavenDBIdentity.Infrastructure.Services
{
    internal class Authenticator
    {
        public AppUser User => _user.Value;

        private readonly IHttpContextAccessor _ctx;
        private readonly IDocumentStore _store;
        private readonly Lazy<AppUser> _user;

        public Authenticator(IHttpContextAccessor ctx, IDocumentStore store)
        {
            _ctx = ctx;
            _store = store;

            _user ??= new Lazy<AppUser>(AuthenticateUser);
        }

        private AppUser AuthenticateUser()
        {
            AuthenticationGuard.AgainstNull(_ctx.HttpContext?.User?.Identity);

            AppUser nullUser = new();

            AuthenticationGuard.Affirm(_ctx.HttpContext?.User.Identity.IsAuthenticated);

            var ci = _ctx.HttpContext?.User.Identity as ClaimsIdentity;
            AuthenticationGuard.AgainstNull(ci);

            var emailClaim = ci.Claims.SingleOrDefault(c => c.Type == "email")
                                ?? ci.Claims.SingleOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
            AuthenticationGuard.AgainstNull(emailClaim);

            string email = emailClaim.Value;
            AuthenticationGuard.AgainstNullOrEmpty(email);

            using var session = _store.OpenSession();
            AppUser user = session.Query<AppUser>().SingleOrDefault(x => x.Email == email);
            if (user == null)
            {
                user = new AppUser { Email = email };
                session.Store(user);
                session.SaveChanges();
            }

            AuthenticationGuard.AgainstNull(user);

            return user;
        }
    }
}
