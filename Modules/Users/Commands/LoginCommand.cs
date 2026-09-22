using MediatR;
using Microsoft.AspNetCore.Identity;
using youtube.Modules.Users.Entities;

namespace youtube.Modules.Users.Commands
{
    public record LoginCommand(string UserName, string Password, string ReturnUrl = null) : IRequest<LoginResult>;

    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public string ErrorMessage { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class LoginHandler : IRequestHandler<LoginCommand, LoginResult>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public LoginHandler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserName))
            {
                return new LoginResult { Succeeded = false, ErrorMessage = "Username or Email is required." };
            }

            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(request.UserName);
            }

            if (user == null)
            {
                return new LoginResult { Succeeded = false, ErrorMessage = "Invalid username or password" };
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return new LoginResult
                {
                    Succeeded = true,
                    ReturnUrl = request.ReturnUrl
                };
            }

            return new LoginResult { Succeeded = false, ErrorMessage = "Invalid username or password" };
        }
    }
}
