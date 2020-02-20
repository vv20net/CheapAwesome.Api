using CheapAwesome.Core.Common.Interface;
using CheapAwesome.Core.Common.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CheapAwesome.Core.Services
{
    public class HotelAvailabilityService : IHotelAvailabilityService
    {
        private readonly IServiceProvider ServiceProvider;
        private readonly IEnumerable<IHotelAvailabilitySupplier> Suppliers;

        public HotelAvailabilityService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;

            Suppliers = ServiceProvider.GetServices<IHotelAvailabilitySupplier>();
        }

        public IList<HotelAvailability> GetAvailabilities(int destinationId, int nights)
        {
            //TODO : get timeout duration as parameter
            int timeout = 1000;
            var cancellationTokenSource = new CancellationTokenSource(timeout);
            var cancellationToken = cancellationTokenSource.Token;

            var tasks = new List<Task<IList<HotelAvailability>>>();

            foreach (var supplier in Suppliers)
            {
                var task = GetAvailabilitiesFromSupplier(supplier, destinationId, nights, cancellationToken);
                tasks.Add(task);
            }

            try
            {
                Task.WaitAll(tasks.ToArray(), timeout, cancellationToken);
            }
            catch (Exception ex)
            {
                // TODO : Log
            }

            var result = new List<HotelAvailability>();

            var completedTasks = tasks.Where(t => t.IsCompletedSuccessfully);
            foreach (var task in completedTasks)
            {
                result.AddRange(task.Result);
            }

            // TODO : validate 
            // TODO : combine and sort

            return result;
        }

        private async Task<IList<HotelAvailability>> GetAvailabilitiesFromSupplier(IHotelAvailabilitySupplier supplier, int destinationId, int nights, CancellationToken cancellationToken)
        {
            return await supplier.GetHotelAvailabilitiesAsync(destinationId, nights, cancellationToken);
        }
    }
}
