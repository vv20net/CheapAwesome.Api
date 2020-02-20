using CheapAwesome.Core.Common.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace CheapAwesome.Core.Common.Model
{
    public class HotelRate
    {
        public HotelRateType RateType { get; set; }
        public BoardType BoardType { get; set; }
        public decimal FinalPrice { get; set; }
    }
}
