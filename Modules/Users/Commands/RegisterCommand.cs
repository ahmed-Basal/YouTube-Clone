using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using youtube.Modules.Users.Entities;
using youtube.SharedKernel;
using youtube.SharedKernel.Events;

namespace youtube.Modules.Users.Commands
{
    public record RegisterCommand(string Name, string UserName, string Email, string Password) : IRequest<RegisterResult>;

    public class RegisterResult
    {
        public bool Succeeded { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    public class RegisterHandler : IRequestHandler<RegisterCommand, RegisterResult>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMediator _mediator;

        public RegisterHandler(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IMediator mediator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mediator = mediator;
        }

        public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = new AppUser
            {
                Name = request.Name,
                UserName = request.UserName?.ToLower(),
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return new RegisterResult
                {
                    Succeeded = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            await _userManager.AddToRoleAsync(user, SD.UserRole);

            // Publish UserRegisteredNotification so Channels module can create channel asynchronously/decoupled
            await _mediator.Publish(new UserRegisteredNotification(user.Id, user.UserName, user.Name, user.Email), cancellationToken);

            // Sign in user with claims
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(ClaimTypes.GivenName, user.Name ?? string.Empty),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Role, SD.UserRole)
            };

            await _signInManager.SignInWithClaimsAsync(user, new AuthenticationProperties(), claims);

            return new RegisterResult { Succeeded = true };
        }
    }
}
