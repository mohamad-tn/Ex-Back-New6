
namespace Bwr.Exchange.Settings.Currencies.Dto
{
    public class ReadCurrencyDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool isMainCurrency { get; set; }
        public int? branchId { get; set; }

    }
}
