using MediatR;

namespace youtube.SharedKernel.Events
{
    public record UserRegisteredNotification(int UserId, string UserName, string Name, string Email) : INotification;
}
