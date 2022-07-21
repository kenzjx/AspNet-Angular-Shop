using Application.Abtractions;
using Application.Exceptions;
using Domain.Entities;
using MediatR;

namespace Application.Features.Categories;

public class DeleteCategoryCommand : IRequest
{
    public int Id { get; set; }

    public class DeleteCategoryCommandhandler : IRequestHandler<DeleteCategoryCommand>
    {
        private readonly IApplicationDbContext _context;
        
      

        public DeleteCategoryCommandhandler(IApplicationDbContext context)
        {
            _context = context;
        }
        
        

        public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Categories.FindAsync(request.Id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Category), request.Id);
            }

            _context.Categories.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}