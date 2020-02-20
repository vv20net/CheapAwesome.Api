using CheapAwesome.Core.Common.Interface;
using CheapAwesome.Core.Common.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
        private readonly ILogger<HotelAvailabilityService> Logger;
        private readonly IServiceProvider ServiceProvider;
        private readonly IEnumerable<IHotelAvailabilitySupplier> Suppliers;
        private readonly ICacheService CacheService;
        private readonly int TimeoutDurationInMilliseconds;
        private readonly int CacheTimeoutDurationInSeconds;

        public HotelAvailabilityService(IServiceProvider serviceProvider, ILogger<HotelAvailabilityService> logger, ICacheService cacheService)
        {
            ServiceProvider = serviceProvider;

            Suppliers = ServiceProvider.GetServices<IHotelAvailabilitySupplier>();

            Logger = logger ?? NullLogger<HotelAvailabilityService>.Instance;

            CacheService = cacheService;

            // TODO : read from config
            TimeoutDurationInMilliseconds = 1000;
            CacheTimeoutDurationInSeconds = 10;
        }

        public IList<HotelAvailability> GetAvailabilities(int destinationId, int nights)
        {
            var cacheKey = $"{destinationId}|{nights}";
            return CacheService.GetOrAdd(cacheKey, 
                () => GetAvailabilitiesImpl(destinationId, nights), 
                (result) => result != null && result.Count > 0, 
                TimeSpan.FromSeconds(CacheTimeoutDurationInSeconds));
        }

        private IList<HotelAvailability> GetAvailabilitiesImpl(int destinationId, int nights)
        {
            var cancellationTokenSource = new CancellationTokenSource(TimeoutDurationInMilliseconds);
            var cancellationToken = cancellationTokenSource.Token;

            var result = new List<HotelAvailability>();

            var tasks = new List<Task<IList<HotelAvailability>>>();

            foreach (var supplier in Suppliers)
            {
                var task = GetAvailabilitiesFromSupplier(supplier, destinationId, nights, cancellationToken);
                tasks.Add(task);
            }

            try
            {
                Task.WaitAll(tasks.ToArray(), TimeoutDurationInMilliseconds, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Data cannot be gathered from one or more suppliers.");
            }

            var completedTasks = tasks.Where(t => t.IsCompletedSuccessfully);
            foreach (var task in completedTasks)
            {
                result.AddRange(task.Result);
            }

            result = ValidateResults(result);

            return result;
        }

        private List<HotelAvailability> ValidateResults(List<HotelAvailability> hotelAvailabilities)
        {
            return hotelAvailabilities
                    .Select(availability => new HotelAvailability { Hotel = availability.Hotel, Rates = availability.Rates?.Where(rate => rate.FinalPrice > 0).ToList() })
                    .Where(availability => !string.IsNullOrEmpty(availability.Hotel?.Name) && availability.Rates?.Count > 0)
                    .ToList();
        }

        private async Task<IList<HotelAvailability>> GetAvailabilitiesFromSupplier(IHotelAvailabilitySupplier supplier, int destinationId, int nights, CancellationToken cancellationToken)
        {
            return await supplier.GetHotelAvailabilitiesAsync(destinationId, nights, cancellationToken);
        }
    }
}
