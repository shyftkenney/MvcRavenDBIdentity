using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using MvcRavenDBIdentity.Infrastructure.Mediatr;
using MvcRavenDBIdentity.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MvcRavenDBIdentity.Features.User
{
    public class ChangeEmail
    {
        public class Command : AuthRequest<IdentityResult>
        {
            public string OldEmail { get; set; }
            public string NewEmail { get; set; }
        }

        internal class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.OldEmail).NotEmpty();
                RuleFor(x => x.NewEmail).NotEmpty();
            }
        }

        internal class Handler : IRequestHandler<Command, IdentityResult>
        {
            private readonly Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager;
            public Handler(Microsoft.AspNetCore.Identity.UserManager<AppUser> UserManager)
            {
                userManager = UserManager;
            }

            public async Task<IdentityResult> Handle(Command command, CancellationToken cancellationToken)
            {
                var user = await userManager.FindByEmailAsync(command.OldEmail);
                user.Email = command.NewEmail;
                var updateResult = await userManager.UpdateAsync(user);
                return updateResult;

            }
        }
    }
}


