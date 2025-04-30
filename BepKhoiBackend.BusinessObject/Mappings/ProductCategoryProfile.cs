using AutoMapper;
using BepKhoiBackend.BusinessObject.dtos.ProductCategoryDto;
using BepKhoiBackend.DataAccess.Models;

namespace BepKhoiBackend.BusinessObject.Mappings
{
    public class ProductCategoryProfile : Profile
    {
        public ProductCategoryProfile()
        {
            CreateMap<ProductCategory, ProductCategoryDto>().ReverseMap();
        }
    }
}
