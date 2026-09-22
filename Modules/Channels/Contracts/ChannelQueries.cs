using MediatR;
using youtube.SharedKernel;

namespace youtube.Modules.Channels.Contracts
{
    public record GetChannelByUserIdQuery(int UserId) : IRequest<Result<ChannelSummaryDto>>;
    public record GetChannelSummaryQuery(int ChannelId) : IRequest<Result<ChannelSummaryDto>>;
    public record VerifyChannelOwnershipQuery(int ChannelId, int UserId) : IRequest<bool>;
    public record IsSubscribedQuery(int ChannelId, int UserId) : IRequest<bool>;
}
