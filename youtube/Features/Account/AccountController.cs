using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using youtube.Modules.Users.Commands;
using youtube.viewmodels.account;

namespace youtube.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IActionResult Login(string returnurl = null)
        {
            var loginVm = new Login_vm
            {
                ReturnUrl = returnurl
            };
            return View(loginVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login_vm login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var result = await _mediator.Send(new LoginCommand(login.UserName, login.Password, login.ReturnUrl));

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(result.ReturnUrl) && Url.IsLocalUrl(result.ReturnUrl))
                {
                    return Redirect(result.ReturnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Invalid username or password");
            return View(login);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register_vm register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }

            var result = await _mediator.Send(new RegisterCommand(register.Name, register.UserName, register.Email, register.Password));

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(register);
        }

        [HttpPost]
        [HttpGet]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _mediator.Send(new LogoutCommand());
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
