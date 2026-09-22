using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Videos.Data;

namespace youtube.Modules.Videos.Commands
{
    public record IncrementVideoViewsCommand(int VideoId) : IRequest<bool>;

    public class IncrementVideoViewsHandler : IRequestHandler<IncrementVideoViewsCommand, bool>
    {
        private readonly VideosDbContext _context;

        public IncrementVideoViewsHandler(VideosDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(IncrementVideoViewsCommand request, CancellationToken cancellationToken)
        {
            var rows = await _context.Videos
                .Where(v => v.Id == request.VideoId)
                .ExecuteUpdateAsync(s => s.SetProperty(v => v.Views, v => v.Views + 1), cancellationToken);

            return rows > 0;
        }
    }
}
