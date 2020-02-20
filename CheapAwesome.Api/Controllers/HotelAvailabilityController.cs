using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheapAwesome.Core.Common.Interface;
using CheapAwesome.Core.Common.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CheapAwesome.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HotelAvailabilityController : ControllerBase
    {
        private readonly ILogger<HotelAvailabilityController> Logger;
        private readonly IHotelAvailabilityService HotelAvailabilityService;

        public HotelAvailabilityController(ILogger<HotelAvailabilityController> logger, IHotelAvailabilityService hotelAvailabilityService)
        {
            Logger = logger;
            HotelAvailabilityService = hotelAvailabilityService;
        }

        [HttpGet]
        public IEnumerable<HotelAvailability> Get(int destinationId, int nights)
        {
            var result = HotelAvailabilityService.GetAvailabilities(destinationId, nights);

            return result;
        }
    }
}
