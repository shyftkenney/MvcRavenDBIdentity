using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using MvcRavenDBIdentity.Infrastructure.Mediatr;
using MvcRavenDBIdentity.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MvcRavenDBIdentity.Features.User
{
    public class ChangeUserRoles
    {
        //Because this happens when a user registers make it Anon
        //Users are blocked from abusing this at the controller level
        public class Command : AnonRequest<bool>
        {
            public ChangeRolesModel model { get; set; }
            public string Email { get; set; }
        }

        internal class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.model.Roles).NotEmpty();
            }
        }

        internal class Handler : IRequestHandler<Command, bool>
        {
            private readonly SignInManager<AppUser> signInManager;
            private readonly UserManager<AppUser> userManager;
            public Handler(SignInManager<AppUser> SignInManager, UserManager<AppUser> UserManager)
            {
                signInManager = SignInManager;
                userManager = UserManager;
            }

            public async Task<bool> Handle(Command command, CancellationToken cancellationToken)
            {

                try
                {
                    var currentUser = await this.userManager.FindByEmailAsync(command.Email);
                    var currentRoles = await this.userManager.GetRolesAsync(currentUser);

                    // Add any new roles.
                    var newRoles = command.model.Roles.Except(currentRoles).ToList();
                    await this.userManager.AddToRolesAsync(currentUser, newRoles);

                    // Remove any old roles we're no longer in.
                    var removedRoles = currentRoles.Except(command.model.Roles).ToList();
                    await this.userManager.RemoveFromRolesAsync(currentUser, removedRoles);

                    // After we change roles, we need to call SignInAsync before AspNetCore Identity picks up the new roles.
                    await this.signInManager.SignInAsync(currentUser, true);

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
    }
}

