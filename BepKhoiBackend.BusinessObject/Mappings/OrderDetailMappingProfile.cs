using AutoMapper;
using BepKhoiBackend.BusinessObject.dtos.OrderDetailDto;
using BepKhoiBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.Mappings
{
    public class OrderDetailMappingProfile : Profile
    {
        public OrderDetailMappingProfile()
        {
            CreateMap<OrderDetail, OrderDetailDto>();
        }
    }
}
