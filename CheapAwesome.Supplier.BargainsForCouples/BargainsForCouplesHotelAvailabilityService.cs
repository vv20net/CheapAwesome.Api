using CheapAwesome.Core.Common.Interface;
using CheapAwesome.Core.Common.Model;
using CheapAwesome.Supplier.BargainsForCouples.Dto;
using CheapAwesome.Supplier.BargainsForCouples.Mapper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CheapAwesome.Supplier.BargainsForCouples
{
    public class BargainsForCouplesHotelAvailabilityService : IHotelAvailabilitySupplier
    {
        private readonly HttpClient Client;
        private readonly BargainsForCouplesSettings Settings;

        public BargainsForCouplesHotelAvailabilityService(HttpClient httpClient, BargainsForCouplesSettings settings)
        {
            Client = httpClient;
            Settings = settings;
        }

        public async Task<IList<HotelAvailability>> GetHotelAvailabilitiesAsync(int destinationId, int nights, CancellationToken cancellationToken)
        {
            var url = $"{Settings.BaseUrl}/findBargain?destinationId={destinationId}&nights={nights}&code={Settings.SecretCode}";

            var httpResponse = await Client.GetAsync(url, cancellationToken);
            var responseJson = await httpResponse.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            var availabilityList = JsonSerializer.Deserialize<IList<AvailabilityDto>>(responseJson, options);

            return HotelAvailabilityMapper.Map(availabilityList, nights);
        }
    }
}