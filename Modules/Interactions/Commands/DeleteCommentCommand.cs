using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Interactions.Data;

namespace youtube.Modules.Interactions.Commands
{
    public record DeleteCommentCommand(int CommentId, int UserId, bool IsChannelOwner = false) : IRequest<bool>;

    public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand, bool>
    {
        private readonly InteractionsDbContext _context;

        public DeleteCommentHandler(InteractionsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken);

            if (comment == null)
            {
                return false;
            }

            bool isCommentOwner = comment.AppUserId == request.UserId;
            if (!isCommentOwner && !request.IsChannelOwner)
            {
                return false;
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
