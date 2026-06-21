using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using youtube.core.Entities;
using youtube.viewmodels.account;
using UTlity;

namespace youtube.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManger;
        private readonly SignInManager<AppUser> _signuser;

        public AccountController(UserManager<AppUser> userManger, SignInManager<AppUser> signuser)
        {
            _userManger = userManger;
            _signuser = signuser;
        }

        [HttpGet]
        public IActionResult Login(string returnurl = null)
        {
            var Loginvm = new Login_vm()
            {
                ReturnUrl = returnurl
            };
            return View(Loginvm);
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

            var user = new AppUser
            {
                Name = register.Name,
                UserName = register.UserName.ToLower(),
                Email = register.Email
            };

            var result = await _userManger.CreateAsync(user, register.Password);

            if (result.Succeeded)
            {
                await _userManger.AddToRoleAsync(user, SD.UserRole);
                await SignInUserAsync(user);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(register);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login_vm login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var user = await _userManger.FindByNameAsync(login.UserName) ?? await _userManger.FindByEmailAsync(login.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password");
                return View(login);
            }

            var result = await _signuser.CheckPasswordSignInAsync(user, login.Password, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                await SignInUserAsync(user);

                if (!string.IsNullOrEmpty(login.ReturnUrl) && Url.IsLocalUrl(login.ReturnUrl))
                {
                    return Redirect(login.ReturnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid username or password");
            return View(login);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signuser.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUserAsync(AppUser user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.GivenName, user.Name),
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roles = await _userManger.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            await _signuser.SignInWithClaimsAsync(user, new AuthenticationProperties(), claims);
        }
    }
}
