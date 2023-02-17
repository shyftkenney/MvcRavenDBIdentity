using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using MvcRavenDBIdentity.Infrastructure.Mediatr;
using MvcRavenDBIdentity.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MvcRavenDBIdentity.Features.User
{
    public class SignIn
    {
        public class Command : AnonRequest<SignInResult>
        {
            public SignInModel model { get; set; }
        }

        internal class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.model.Email).NotEmpty();
                RuleFor(x => x.model.Password).NotEmpty();
            }
        }

        internal class Handler : IRequestHandler<Command, SignInResult>
        {
            private readonly SignInManager<AppUser> signInManager;

            public Handler(SignInManager<AppUser> SignInManager)
            {
                signInManager = SignInManager;
            }

            public async Task<SignInResult> Handle(Command command, CancellationToken cancellationToken)
            {
                var signInResult = await signInManager.PasswordSignInAsync(command.model.Email, command.model.Password, true, false);
                return signInResult;
            }
        }
    }
}
