using AutoMapper;
using Bwr.Exchange.ExchangeCurrencies.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bwr.Exchange.ExchangeCurrencies.Map
{
    public class ExchangeCurrencyMapProfile : Profile
    {
        public ExchangeCurrencyMapProfile()
        {
            CreateMap<ExchangeCurrency, ExchangeCurrencyDto>();
            CreateMap<ExchangeCurrencyDto, ExchangeCurrency>();
            CreateMap<ExchangeCurrency, CreateExchangeCurrencyDto>();
            CreateMap<CreateExchangeCurrencyDto, ExchangeCurrency>();
            CreateMap<ExchangeCurrency, UpdateExchangeCurrencyDto>();
            CreateMap<UpdateExchangeCurrencyDto, ExchangeCurrency>();
        }
    }
}
