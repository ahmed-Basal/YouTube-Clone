using MediatR;
using youtube.SharedKernel;

namespace youtube.Modules.Videos.Contracts
{
    public record GetVideoSummaryQuery(int VideoId) : IRequest<Result<VideoSummaryDto>>;
    public record GetVideosByChannelQuery(int ChannelId) : IRequest<List<VideoSummaryDto>>;
    public record GetCategoriesQuery : IRequest<List<CategoryDto>>;
}
