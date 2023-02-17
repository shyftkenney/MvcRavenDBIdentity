using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcRavenDBIdentity.Features.User;
using MvcRavenDBIdentity.Models;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MvcRavenDBIdentity.Controllers
{
    public class AccountController : Controller
    {

        private readonly IMediator _mediator;
        public AccountController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpGet]
        [Route("Account/SignIn/{returnUrl?}")]
        public IActionResult SignIn(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInModel model, string returnUrl = "")
        {
            SignIn.Command cmdSignIn = new();
            cmdSignIn.model = model;
            var signInResult = await this._mediator.Send(cmdSignIn);
            if (signInResult.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            var reason = signInResult.IsLockedOut ? "Your user is locked out" :
                signInResult.IsNotAllowed ? "Your user is not allowed to sign in" :
                signInResult.RequiresTwoFactor ? "2FA is required" :
                "Bad user name or password";
            return RedirectToAction("SignInFailure", new { reason });
        }

        [HttpGet]
        public IActionResult SignInFailure(string reason)
        {
            ViewBag.FailureReason = reason;
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            CreateUser.Command cmdCreateUser = new();
            cmdCreateUser.model = model;
            var createUserResult = await this._mediator.Send(cmdCreateUser);
            if (!createUserResult.Succeeded)
            {
                var errorString = string.Join(", ", createUserResult.Errors.Select(e => e.Description));
                return RedirectToAction("RegisterFailure", new { reason = errorString });
            }

            // Add user to a default role.
            ChangeUserRoles.Command cmdChangeUserRoles = new();
            cmdChangeUserRoles.model = new();
            cmdChangeUserRoles.model.Roles = new()
            {
                AppUser.ManagerRole
            };
            cmdChangeUserRoles.Email = model.Email;
            var changeRolesResult = await this._mediator.Send(cmdChangeUserRoles);

            // Sign user in and go home.
            SignIn.Command cmdSignIn = new();
            cmdSignIn.model = new();
            cmdSignIn.model.Password = model.Password;
            cmdSignIn.model.Email = model.Email;
            var signInResult = await this._mediator.Send(cmdSignIn);
            if (signInResult.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult RegisterFailure(string reason)
        {
            ViewBag.FailureReason = reason;
            return View();
        }

        // Must be logged in to reach this page.
        [Authorize]
        [HttpGet]
        public IActionResult ChangeRoles()
        {
            return View();
        }

        [Authorize] 
        [HttpPost]
        public async Task<IActionResult> ChangeRoles(ChangeRolesModel model, string Email)
        {
            ChangeUserRoles.Command cmdChangeUserRoles = new();
            cmdChangeUserRoles.model = model;
            cmdChangeUserRoles.Email = Email;
            var changeRolesResult = await this._mediator.Send(cmdChangeUserRoles);
            if (changeRolesResult)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            SignOut.Command cmdSignOut = new();
            var changeRolesResult = await this._mediator.Send(cmdSignOut);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(string oldEmail, string newEmail)
        {
            ChangeEmail.Command cmdChangeEmail = new();
            cmdChangeEmail.OldEmail = oldEmail;
            cmdChangeEmail.NewEmail = newEmail;
            var updateResult = await this._mediator.Send(cmdChangeEmail);
            return Json(updateResult);
        }
    }
}