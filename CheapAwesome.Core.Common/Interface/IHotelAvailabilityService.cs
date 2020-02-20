using CheapAwesome.Core.Common.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace CheapAwesome.Core.Common.Interface
{
    public interface IHotelAvailabilityService
    {
        IList<HotelAvailability> GetAvailabilities(int destinationId, int nights);
    }
}
