using AutoMapper;
using Bwr.Exchange.Settings.ExchangePrices.Dto;

namespace Bwr.Exchange.Settings.ExchangePrices.Map
{
    public class ExchangePriceMapProfile : Profile
    {
        public ExchangePriceMapProfile()
        {
            CreateMap<ExchangePrice, ExchangePriceDto>();
            CreateMap<ExchangePriceDto, ExchangePrice>();
            CreateMap<ExchangePrice, UpdateExchangePriceDto>();
            CreateMap<UpdateExchangePriceDto, ExchangePrice>();
        }
    }
}
