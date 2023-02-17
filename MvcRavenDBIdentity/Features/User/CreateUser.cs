using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using MvcRavenDBIdentity.Infrastructure.Mediatr;
using MvcRavenDBIdentity.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MvcRavenDBIdentity.Features.User
{
    public class CreateUser
    {
        public class Command : AnonRequest<IdentityResult>
        {
            public RegisterModel model { get; set; }
        }

        internal class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.model.Email).NotEmpty();
                RuleFor(x => x.model.Password).NotEmpty();
            }
        }

        internal class Handler : IRequestHandler<Command, IdentityResult>
        {
            private readonly UserManager<AppUser> userManager;

            public Handler(UserManager<AppUser> UserManager)
            {
                userManager = UserManager;
            }

            public async Task<IdentityResult> Handle(Command command, CancellationToken cancellationToken)
            {
                // Create the user.
                var appUser = new AppUser
                {
                    Email = command.model.Email,
                    UserName = command.model.Email
                };

                var createUserResult = await this.userManager.CreateAsync(appUser, command.model.Password);
                return createUserResult;
            }
        }
    }
}

