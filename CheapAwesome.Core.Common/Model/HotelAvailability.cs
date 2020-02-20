using System;
using System.Collections.Generic;
using System.Text;

namespace CheapAwesome.Core.Common.Model
{
    public class HotelAvailability
    {
        public HotelInfo Hotel { get; set; }
        public IList<HotelRate> Rates { get; set; }
    }
}
