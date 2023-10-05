using Abp.Domain.Entities.Auditing;
using Bwr.Exchange.Settings.Currencies;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bwr.Exchange.Settings.Treasuries
{
    public class TreasuryBalance : FullAuditedEntity
    {
        public TreasuryBalance(double initilBalance, int currencyId, int treasuryId)
        {
            InitilBalance = initilBalance;
            TreasuryId = treasuryId;
            CurrencyId = currencyId;
        }
        public double InitilBalance { get; private set; }

        #region Currency
        public int CurrencyId { get; private set; }
        [ForeignKey("CurrencyId")]
        public virtual Currency Currency { get; private set; }
        #endregion

        #region Treasury
        public int TreasuryId { get; private set; }
        [ForeignKey("TreasuryId")]
        public virtual Treasury Treasury { get; private set; }
        #endregion 
    }
}
