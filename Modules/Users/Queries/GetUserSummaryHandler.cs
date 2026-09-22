using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Users.Contracts;
using youtube.Modules.Users.Data;
using youtube.SharedKernel;

namespace youtube.Modules.Users.Queries
{
    public class GetUserSummaryHandler :
        IRequestHandler<GetUserSummaryQuery, Result<UserSummaryDto>>,
        IRequestHandler<GetUsersSummariesQuery, Dictionary<int, UserSummaryDto>>
    {
        private readonly UsersDbContext _dbContext;

        public GetUserSummaryHandler(UsersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<UserSummaryDto>> Handle(GetUserSummaryQuery request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == request.UserId)
                .Select(u => new UserSummaryDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Name = u.Name,
                    Email = u.Email,
                    CreatedAt = u.CreateAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return Result<UserSummaryDto>.Failure("User not found", 404);
            }

            return Result<UserSummaryDto>.Success(user);
        }

        public async Task<Dictionary<int, UserSummaryDto>> Handle(GetUsersSummariesQuery request, CancellationToken cancellationToken)
        {
            var userIds = request.UserIds?.Distinct().ToList() ?? new List<int>();
            if (!userIds.Any())
            {
                return new Dictionary<int, UserSummaryDto>();
            }

            return await _dbContext.Users
                .AsNoTracking()
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new UserSummaryDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Name = u.Name,
                    Email = u.Email,
                    CreatedAt = u.CreateAt
                })
                .ToDictionaryAsync(u => u.Id, cancellationToken);
        }
    }
}
