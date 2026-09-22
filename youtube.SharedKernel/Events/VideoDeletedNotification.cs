using MediatR;

namespace youtube.SharedKernel.Events
{
    public record VideoDeletedNotification(int VideoId, int ChannelId) : INotification;
}
