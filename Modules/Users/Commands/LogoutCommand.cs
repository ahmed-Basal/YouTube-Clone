using MediatR;
using Microsoft.AspNetCore.Identity;
using youtube.Modules.Users.Entities;

namespace youtube.Modules.Users.Commands
{
    public record LogoutCommand : IRequest<bool>;

    public class LogoutHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly SignInManager<AppUser> _signInManager;

        public LogoutHandler(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await _signInManager.SignOutAsync();
            return true;
        }
    }
}
