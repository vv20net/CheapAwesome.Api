using System;
using System.Collections.Generic;
using System.Text;

namespace CheapAwesome.Supplier.BargainsForCouples.Dto
{
    public class AvailabilityDto
    {
        public HotelDto Hotel { get; set; }
        public IList<RateDto> Rates { get; set; }
    }
}
