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
        //TODO : read from config
        private const string BASE_URL = "https://webbedsdevtest.azurewebsites.net/api";
        //TODO : read from config
        private const string SECRET_CODE = "aWH1EX7ladA8C/oWJX5nVLoEa4XKz2a64yaWVvzioNYcEo8Le8caJw==";

        private readonly HttpClient Client;

        public BargainsForCouplesHotelAvailabilityService(HttpClient httpClient)
        {
            Client = httpClient;
        }

        public async Task<IList<HotelAvailability>> GetHotelAvailabilitiesAsync(int destinationId, int nights, CancellationToken cancellationToken)
        {
            var url = $"{BASE_URL}/findBargain?destinationId={destinationId}&nights={nights}&code={SECRET_CODE}";

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