using CheapAwesome.Core.Common.Model;
using CheapAwesome.Supplier.BargainsForCouples.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheapAwesome.Supplier.BargainsForCouples.Mapper
{
    public abstract class HotelAvailabilityMapper
    {
        public static HotelAvailability Map(AvailabilityDto dto, int nights)
        {
            var result = new HotelAvailability 
            {
                Hotel = new HotelInfo 
                {
                    Name = dto?.Hotel?.Name,
                    Rating = dto?.Hotel?.Rating ?? 0
                }
            };

            result.Rates = new List<HotelRate>();

            if (dto.Rates != null)
            {
                foreach (var rateDto in dto.Rates)
                {
                    var rate = new HotelRate();

                    switch (rateDto.BoardType)
                    {
                        case "No Meals":
                            rate.BoardType = Core.Common.Enum.BoardType.NoMeals;
                            break;

                        case "Half Board":
                            rate.BoardType = Core.Common.Enum.BoardType.HalfBoard;
                            break;

                        case "Full Board":
                            rate.BoardType = Core.Common.Enum.BoardType.FullBoard;
                            break;
                    }

                    rate.RateType = rateDto.RateType == "PerNight" ? Core.Common.Enum.HotelRateType.PerNight : Core.Common.Enum.HotelRateType.Stay;

                    rate.FinalPrice = rate.RateType == Core.Common.Enum.HotelRateType.PerNight
                                        ? rateDto.Value * nights
                                        : rateDto.Value;

                    result.Rates.Add(rate);
                }
            }

            return result;
        }

        public static IList<HotelAvailability> Map(IList<AvailabilityDto> dtoList, int nights)
        {
            return dtoList.Select(dto => Map(dto, nights)).ToList();
        }
    }
}
