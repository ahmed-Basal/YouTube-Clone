using MediatR;
using Microsoft.EntityFrameworkCore;
using youtube.Modules.Administration.DTOs;
using youtube.Modules.Videos.Data;
using youtube.Modules.Videos.Entities;

namespace youtube.Modules.Administration.Commands
{
    public record GetAdminCategoriesQuery : IRequest<List<AdminCategoryDto>>;
    public record CreateAdminCategoryCommand(string Name) : IRequest<(bool Success, string Message, AdminCategoryDto Category)>;
    public record EditAdminCategoryCommand(int Id, string Name) : IRequest<(bool Success, string Message, AdminCategoryDto Category)>;
    public record DeleteAdminCategoryCommand(int Id) : IRequest<(bool Success, string Message)>;

    public class AdminCategoryHandlers :
        IRequestHandler<GetAdminCategoriesQuery, List<AdminCategoryDto>>,
        IRequestHandler<CreateAdminCategoryCommand, (bool Success, string Message, AdminCategoryDto Category)>,
        IRequestHandler<EditAdminCategoryCommand, (bool Success, string Message, AdminCategoryDto Category)>,
        IRequestHandler<DeleteAdminCategoryCommand, (bool Success, string Message)>
    {
        private readonly VideosDbContext _context;

        public AdminCategoryHandlers(VideosDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdminCategoryDto>> Handle(GetAdminCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new AdminCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<(bool Success, string Message, AdminCategoryDto Category)> Handle(CreateAdminCategoryCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return (false, "Category name is required", null);
            }

            var category = new Category { Name = request.Name.Trim() };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            return (true, "Category created successfully", new AdminCategoryDto { Id = category.Id, Name = category.Name });
        }

        public async Task<(bool Success, string Message, AdminCategoryDto Category)> Handle(EditAdminCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            if (category == null)
            {
                return (false, "Category not found", null);
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return (false, "Category name is required", null);
            }

            category.Name = request.Name.Trim();
            await _context.SaveChangesAsync(cancellationToken);

            return (true, "Category updated successfully", new AdminCategoryDto { Id = category.Id, Name = category.Name });
        }

        public async Task<(bool Success, string Message)> Handle(DeleteAdminCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            if (category == null)
            {
                return (false, "Category not found");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);

            return (true, "Category deleted successfully");
        }
    }
}
