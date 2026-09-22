using MediatR;
using youtube.SharedKernel;

namespace youtube.Modules.Users.Contracts
{
    public record GetUserSummaryQuery(int UserId) : IRequest<Result<UserSummaryDto>>;
    public record GetUsersSummariesQuery(IEnumerable<int> UserIds) : IRequest<Dictionary<int, UserSummaryDto>>;
}
