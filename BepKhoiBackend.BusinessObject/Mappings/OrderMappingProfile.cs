using AutoMapper;
using BepKhoiBackend.BusinessObject.dtos.OrderDetailDto;
using BepKhoiBackend.BusinessObject.dtos.OrderDto;
using BepKhoiBackend.DataAccess.Models;

namespace BepKhoiBackend.BusinessObject.Mappings
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<CreateOrderRequestDto, Order>()
                .ForMember(dest => dest.CreatedTime, otp => otp.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.TotalQuantity, opt => opt.MapFrom(_ => 0))
                .ForMember(dest => dest.AmountDue, opt => opt.MapFrom(_ => 0));


            CreateMap<AddNoteRequest, Order>()
                .ForMember(dest => dest.OrderNote, opt => opt.MapFrom(src => src.OrderNote));
            
            // Mapping
            CreateMap<Order, OrderDto>();
        }
    }
}
