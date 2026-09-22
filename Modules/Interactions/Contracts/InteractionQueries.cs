using MediatR;
using System.Collections.Generic;

namespace youtube.Modules.Interactions.Contracts
{
    public record GetVideoCommentsQuery(int VideoId) : IRequest<List<CommentDto>>;
    public record GetVideoReactionsQuery(int VideoId, int? CurrentUserId = null) : IRequest<VideoReactionsDto>;
}
