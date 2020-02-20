using CheapAwesome.Core.Common.Interface;
using CheapAwesome.Core.Services;
using CheapAwesome.Core.Services.Caching;
using Moq;
using NUnit.Framework;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CheapAwesome.Core.Common.Model;

namespace CheapAwesome.Tests
{
    public class HotelAvailabilityServiceTests
    {
        private ICacheService CacheService = new MemoryCacheService(new MemoryCacheSettings { Enabled = false, SizeLimit = 0 });

        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Can_Return_WithNoSuppliers()
        {
            var suppliers = new List<IHotelAvailabilitySupplier>();
            var hotelAvailabilityService = new HotelAvailabilityService(suppliers, null, CacheService);

            var result = hotelAvailabilityService.GetAvailabilities(222, 2);
            Assert.IsTrue(result.Count == 0);
        }

        [Test]
        public void Can_Return_WithNullReturningSuplier()
        {
            var suppliers = new List<IHotelAvailabilitySupplier>();

            var supplierMock = new Mock<IHotelAvailabilitySupplier>();
            supplierMock.Setup(x => x.GetHotelAvailabilitiesAsync(It.IsAny<int>(), It.IsAny<int>(), CancellationToken.None))
                .Returns(Task.FromResult<IList<HotelAvailability>>(new List<HotelAvailability>()));

            suppliers.Add(supplierMock.Object);

            var hotelAvailabilityService = new HotelAvailabilityService(suppliers, null, CacheService);

            var result = hotelAvailabilityService.GetAvailabilities(222, 2);
            Assert.IsTrue(result.Count == 0);
        }

        [Test]
        public void Can_Validate_EmptyNamedHotel()
        {
            var suppliers = new List<IHotelAvailabilitySupplier>();

            var list = new List<HotelAvailability>();
            list.Add(new HotelAvailability
            {
                Hotel = new HotelInfo
                {
                    Name = string.Empty,
                    Rating = 3
                },
                Rates = new List<HotelRate> { new HotelRate { BoardType = Core.Common.Enum.BoardType.NoMeals, RateType = Core.Common.Enum.HotelRateType.PerNight, FinalPrice = 112 } }
            });

            var supplierMock = new Mock<IHotelAvailabilitySupplier>();
            supplierMock.Setup(x => x.GetHotelAvailabilitiesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IList<HotelAvailability>>(list));

            suppliers.Add(supplierMock.Object);

            var hotelAvailabilityService = new HotelAvailabilityService(suppliers, null, CacheService);

            var result = hotelAvailabilityService.GetAvailabilities(222, 2);
            Assert.IsTrue(result.Count == 0);
        }

        [Test]
        public void Can_Validate_InvalidPrice()
        {
            var suppliers = new List<IHotelAvailabilitySupplier>();

            var list = new List<HotelAvailability>();
            list.Add(new HotelAvailability
            {
                Hotel = new HotelInfo
                {
                    Name = "Some hotel",
                    Rating = 3
                },
                Rates = new List<HotelRate> { new HotelRate { BoardType = Core.Common.Enum.BoardType.NoMeals, RateType = Core.Common.Enum.HotelRateType.PerNight, FinalPrice = -1 } }
            });

            var supplierMock = new Mock<IHotelAvailabilitySupplier>();
            supplierMock.Setup(x => x.GetHotelAvailabilitiesAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult<IList<HotelAvailability>>(list));

            suppliers.Add(supplierMock.Object);

            var hotelAvailabilityService = new HotelAvailabilityService(suppliers, null, CacheService);

            var result = hotelAvailabilityService.GetAvailabilities(222, 2);
            Assert.IsTrue(result.Count == 0);
        }
    }
}