using MediatR;
using youtube.Modules.Interactions.Contracts;
using youtube.Modules.Interactions.Data;
using youtube.Modules.Interactions.Entities;
using youtube.SharedKernel;

namespace youtube.Modules.Interactions.Commands
{
    public record AddCommentCommand(int VideoId, int UserId, string Text) : IRequest<Result<CommentDto>>;

    public class AddCommentHandler : IRequestHandler<AddCommentCommand, Result<CommentDto>>
    {
        private readonly InteractionsDbContext _context;

        public AddCommentHandler(InteractionsDbContext context)
        {
            _context = context;
        }

        public async Task<Result<CommentDto>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                return Result<CommentDto>.Failure("Comment cannot be empty");
            }

            var comment = new Comment
            {
                VideoId = request.VideoId,
                AppUserId = request.UserId,
                Content = request.Text.Trim(),
                PostAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<CommentDto>.Success(new CommentDto
            {
                Id = comment.Id,
                VideoId = comment.VideoId,
                AppUserId = comment.AppUserId,
                Content = comment.Content,
                PostAt = comment.PostAt
            });
        }
    }
}
