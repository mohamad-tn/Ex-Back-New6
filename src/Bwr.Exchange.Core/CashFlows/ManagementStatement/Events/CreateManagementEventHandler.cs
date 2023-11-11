using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Bwr.Exchange.CashFlows.ManagementStatement.Services;
using System.Threading.Tasks;

namespace Bwr.Exchange.CashFlows.ManagementStatement.Events
{
    public class CreateManagementEventHandler : IAsyncEventHandler<CreateManagementEventData>, ITransientDependency
    {
        private readonly IManagementMnager _managementMnager;

        public CreateManagementEventHandler(IManagementMnager managementMnager)
        {
            _managementMnager = managementMnager;
        }

        public async Task HandleEventAsync(CreateManagementEventData eventData)
        {
            var management = new Management();

            #region Set Management
            management.AfterChange = eventData.AfterChange;
            management.BeforChange = eventData.BeforChange;
            management.Date = eventData.Date;
            management.ChangeDate = eventData.ChangeDate;
            management.Type = (ManagementItemType)eventData.Type;
            management.Amount = eventData.Amount;
            management.PaymentType = (Transfers.PaymentType?)eventData.PaymentType;
            management.ChangeType = (ChangeType)eventData.ChangeType;
            management.Number = eventData.Number;
            management.TreasuryActionType = (TreasuryActions.TreasuryActionType?)eventData.TreasuryActionType;
            management.MainAccount = eventData.MainAccount;
            management.AmountOfFirstCurrency = eventData.AmountOfFirstCurrency;
            management.AmoutOfSecondCurrency = eventData.AmoutOfSecondCurrency;
            management.PaidAmountOfFirstCurrency = eventData.PaidAmountOfFirstCurrency;
            management.ReceivedAmountOfFirstCurrency = eventData.ReceivedAmountOfFirstCurrency;
            management.PaidAmountOfSecondCurrency = eventData.PaidAmountOfSecondCurrency;
            management.ReceivedAmountOfSecondCurrency = eventData.ReceivedAmountOfSecondCurrency;
            management.Commission = eventData.Commission;
            management.FirstCurrencyId = eventData.FirstCurrencyId;
            management.SecondCurrencyId = eventData.SecondCurrencyId;
            management.CurrencyId = eventData.CurrencyId;
            management.ClientId = eventData.ClientId;
            management.UserId = eventData.UserId;
            management.CompanyId = eventData.CompanyId;
            management.SenderId = eventData.SenderId;
            management.BeneficiaryId = eventData.BeneficiaryId;
            management.ToCompanyId = eventData.ToCompanyId;
            management.ActionType = (ExchangeCurrencies.ActionType?)eventData.ActionType;
            #endregion

            await _managementMnager.CreateAsync(management);
        }
    }
}
