using Application.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Application.Features.Categories.Dto;

public class CategoryDto : IMapFrom<Category>
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public byte[] Picture { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Category, CategoryDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.CategoryId))
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.CategoryName));
    }
}