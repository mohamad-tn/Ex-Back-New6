using Abp.Application.Services.Dto;

namespace Bwr.Exchange.ExchangeCurrencies.Dto
{
    public class UpdateExchangeCurrencyDto : EntityDto
    {
        public int Number { get; set; }
        public double AmountOfFirstCurrency { get; set; }
        public double AmoutOfSecondCurrency { get; set; }
        public double PaidAmountOfFirstCurrency { get; set; }
        public double ReceivedAmountOfFirstCurrency { get; set; }
        public double PaidAmountOfSecondCurrency { get; set; }
        public double ReceivedAmountOfSecondCurrency { get; set; }
        public double ExchangePrice { get; set; }
        public int PaymentType { get; set; }
        public int ActionType { get; set; }
        public string Note { get; set; }
        public string Date { get; set; }
        public int? FirstCurrencyId { get; set; }
        public int? SecondCurrencyId { get; set; }
        public int? MainCurrencyId { get; set; }
        public int? ClientId { get; set; }
        public int? CompanyId { get; set; }
        //public int BranchId { get; set; }

    }
}
