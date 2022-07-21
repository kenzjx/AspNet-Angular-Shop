using Application.Features.Categories.Dto;

namespace Application.Features.Categories.Queries;

public class CategoriesListVm
{
    public IList<CategoryDto> Categories { get; set; }

    public int Count { get; set; }
}