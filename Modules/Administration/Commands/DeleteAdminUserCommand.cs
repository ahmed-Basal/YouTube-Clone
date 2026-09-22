using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Users.Data;
using youtube.Modules.Users.Entities;
using youtube.SharedKernel.Events;

namespace youtube.Modules.Administration.Commands
{
    public record DeleteAdminUserCommand(int TargetUserId, int CurrentUserId) : IRequest<DeleteAdminUserResult>;

    public class DeleteAdminUserResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class DeleteAdminUserHandler : IRequestHandler<DeleteAdminUserCommand, DeleteAdminUserResult>
    {
        private readonly UsersDbContext _usersContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMediator _mediator;

        public DeleteAdminUserHandler(
            UsersDbContext usersContext,
            UserManager<AppUser> userManager,
            IMediator mediator)
        {
            _usersContext = usersContext;
            _userManager = userManager;
            _mediator = mediator;
        }

        public async Task<DeleteAdminUserResult> Handle(DeleteAdminUserCommand request, CancellationToken cancellationToken)
        {
            if (request.TargetUserId == request.CurrentUserId)
            {
                return new DeleteAdminUserResult
                {
                    Success = false,
                    Message = "You cannot delete your own admin account."
                };
            }

            var user = await _usersContext.Users
                .FirstOrDefaultAsync(u => u.Id == request.TargetUserId, cancellationToken);

            if (user == null)
            {
                return new DeleteAdminUserResult
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            // Publish event so Channels, Videos, and Interactions can clean up
            await _mediator.Publish(new UserDeletedNotification(request.TargetUserId), cancellationToken);

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errStr = string.Join(", ", result.Errors.Select(e => e.Description));
                return new DeleteAdminUserResult { Success = false, Message = errStr };
            }

            return new DeleteAdminUserResult
            {
                Success = true,
                Message = $"User '{user.Name ?? user.UserName}' and associated data deleted successfully."
            };
        }
    }
}
