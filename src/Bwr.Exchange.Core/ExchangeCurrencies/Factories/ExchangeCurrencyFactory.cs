using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Bwr.Exchange.ExchangeCurrencies.Services.Implements;
using Bwr.Exchange.ExchangeCurrencies.Services.Interfaces;
using Bwr.Exchange.Settings.ExchangePrices;
using Bwr.Exchange.Settings.Treasuries;
using Bwr.Exchange.Settings.Treasuries.Services;
using Bwr.Exchange.Transfers;
using System;

namespace Bwr.Exchange.ExchangeCurrencies.Factories
{
    public class ExchangeCurrencyFactory : IExchangeCurrencyFactory
    {
        private readonly IRepository<ExchangeCurrency> _exchangeCurrencyRepository;
        private readonly IRepository<ExchangePrice> _exchangePriceRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Treasury> _treasuryRepository;
        private readonly ITreasuryManager _treasuryManager;
        public ExchangeCurrencyFactory(IRepository<ExchangeCurrency> exchangeCurrencyRepository, IRepository<ExchangePrice> exchangePriceRepository, IUnitOfWorkManager unitOfWorkManager, IRepository<Treasury> treasuryRepository, ITreasuryManager treasuryManager)
        {
            _exchangeCurrencyRepository = exchangeCurrencyRepository;
            _exchangePriceRepository = exchangePriceRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _treasuryRepository = treasuryRepository;
            _treasuryManager = treasuryManager;
        }

        public IExchangeCurrencyDomainService CreateService(PaymentType paymentType)
        {
            switch (paymentType)
            {
                case PaymentType.Cash:
                    return new CashExchangeCurrencyService(_exchangeCurrencyRepository, _exchangePriceRepository, _treasuryRepository, _unitOfWorkManager, _treasuryManager);
                case PaymentType.Client:
                    return new ClientExchangeCurrencyService(_exchangeCurrencyRepository, _exchangePriceRepository, _treasuryRepository, _unitOfWorkManager, _treasuryManager);
                case PaymentType.Company:
                    return new CompanExchangeCurrencyService(_exchangeCurrencyRepository, _exchangePriceRepository, _unitOfWorkManager);
                default:
                    return null;
            }
        }
    }
}
