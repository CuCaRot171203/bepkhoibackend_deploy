using AutoMapper;
using BepKhoiBackend.BusinessObject.dtos.UnitDto;
using BepKhoiBackend.DataAccess.Models;

namespace BepKhoiBackend.BusinessObject.Mappings
{
    public class UnitProfile : Profile
    {
        public UnitProfile()
        {
            CreateMap<Unit, UnitDto>().ReverseMap();
        }
    }
}
