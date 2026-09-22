using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Videos.Data;
using youtube.SharedKernel.Events;

namespace youtube.Modules.Videos.Commands
{
    public record DeleteVideoCommand(int VideoId, int ChannelId) : IRequest<DeleteVideoResult>;

    public class DeleteVideoResult
    {
        public int StatusCode { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }

    public class DeleteVideoHandler : IRequestHandler<DeleteVideoCommand, DeleteVideoResult>
    {
        private readonly VideosDbContext _context;
        private readonly IMediator _mediator;

        public DeleteVideoHandler(VideosDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<DeleteVideoResult> Handle(DeleteVideoCommand request, CancellationToken cancellationToken)
        {
            var video = await _context.Videos
                .FirstOrDefaultAsync(v => v.Id == request.VideoId && v.ChannelId == request.ChannelId, cancellationToken);

            if (video == null)
            {
                return new DeleteVideoResult
                {
                    StatusCode = 404,
                    Title = "Not Found",
                    Message = "Video not found or you do not have permission to delete it"
                };
            }

            _context.Videos.Remove(video);
            await _context.SaveChangesAsync(cancellationToken);

            // Publish event so Interactions module can clean up reactions & comments
            await _mediator.Publish(new VideoDeletedNotification(request.VideoId, request.ChannelId), cancellationToken);

            return new DeleteVideoResult
            {
                StatusCode = 200,
                Title = "Success",
                Message = "Video deleted successfully"
            };
        }
    }
}
