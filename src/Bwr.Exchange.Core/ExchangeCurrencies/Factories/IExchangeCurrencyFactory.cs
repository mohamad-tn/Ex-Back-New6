using Abp.Dependency;
using Bwr.Exchange.ExchangeCurrencies.Services.Interfaces;
using Bwr.Exchange.Transfers;

namespace Bwr.Exchange.ExchangeCurrencies.Factories
{
    public interface IExchangeCurrencyFactory : ITransientDependency
    {
        IExchangeCurrencyDomainService CreateService(PaymentType paymentType);
    }
}
