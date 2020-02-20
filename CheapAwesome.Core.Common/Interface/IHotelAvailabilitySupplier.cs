using CheapAwesome.Core.Common.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CheapAwesome.Core.Common.Interface
{
    public interface IHotelAvailabilitySupplier
    {
        Task<IList<HotelAvailability>> GetHotelAvailabilitiesAsync(int destinationId, int nights, CancellationToken cancellationToken);
    }
}
