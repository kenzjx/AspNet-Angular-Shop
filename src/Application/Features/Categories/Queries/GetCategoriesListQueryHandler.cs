using Application.Abtractions;
using Application.Features.Categories.Dto;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Categories.Queries;

public class GetCategoriesListQueryHandler : IRequestHandler<GetCategoriesListQuery, CategoriesListVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCategoriesListQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CategoriesListVm> Handle(GetCategoriesListQuery request, CancellationToken cancellationToken)
    {
        var categories = await _context.Categories
            .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var vm = new CategoriesListVm()
        {
            Categories = categories,
            Count = categories.Count
        };

        return vm;
    }
}