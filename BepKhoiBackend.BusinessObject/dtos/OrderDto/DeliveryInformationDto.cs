using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.BusinessObject.dtos.OrderDto
{
    public class DeliveryInformationDto
    {
        public int DeliveryInformationId { get; set; }
        public string ReceiverName { get; set; } = null!;
        public string ReceiverPhone { get; set; } = null!;
        public string ReceiverAddress { get; set; } = null!;
        public string? DeliveryNote { get; set; }
    }
}
