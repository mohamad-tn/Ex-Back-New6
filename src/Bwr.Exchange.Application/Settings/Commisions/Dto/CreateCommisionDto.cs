namespace Bwr.Exchange.Settings.Commisions.Dto
{
    public class CreateCommisionDto
    {
        public double From { get; set; }
        public double To { get; set; }
        public double Value { get; set; }
        public double Percentage { get; set; }
        public int CurrencyId { get; set; }
    }
}
