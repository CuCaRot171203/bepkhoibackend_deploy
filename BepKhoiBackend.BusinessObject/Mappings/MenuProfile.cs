using AutoMapper;
using BepKhoiBackend.BusinessObject.dtos.MenuDto;
using BepKhoiBackend.DataAccess.Models;

namespace BepKhoiBackend.BusinessObject.Mappings
{
    public class MenuProfile : Profile
    {
        public MenuProfile()
        {
            CreateMap<Menu, MenuDto>().ReverseMap();
            CreateMap<Menu, MenuCustomerDto>();
            CreateMap<CreateMenuDto, Menu>();
            CreateMap<UpdateMenuDto, Menu>();
            CreateMap<UpdatePriceDto, Menu>();
        }
    }
}
