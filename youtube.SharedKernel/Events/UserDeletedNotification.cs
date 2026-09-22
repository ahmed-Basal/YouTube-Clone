using MediatR;

namespace youtube.SharedKernel.Events
{
    public record UserDeletedNotification(int UserId) : INotification;
}
