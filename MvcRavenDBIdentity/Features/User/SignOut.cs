using MediatR;
using Microsoft.AspNetCore.Identity;
using MvcRavenDBIdentity.Infrastructure.Mediatr;
using MvcRavenDBIdentity.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MvcRavenDBIdentity.Features.User
{
    public class SignOut
    {
        public class Command : AuthRequest<bool> { }

        internal class Handler : IRequestHandler<Command, bool>
        {
            private readonly SignInManager<AppUser> signInManager;

            public Handler(SignInManager<AppUser> SignInManager)
            {
                signInManager = SignInManager;
            }

            public async Task<bool> Handle(Command command, CancellationToken cancellationToken)
            {
                await this.signInManager.SignOutAsync();
                return true;
            }
        }
    }
}
