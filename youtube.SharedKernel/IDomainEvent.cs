using MediatR;

namespace youtube.SharedKernel
{
    public interface IDomainEvent : INotification
    {
        DateTime OccurredOn { get; }
    }
}
