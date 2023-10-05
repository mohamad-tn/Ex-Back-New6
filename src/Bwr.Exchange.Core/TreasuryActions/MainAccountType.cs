namespace Bwr.Exchange.TreasuryActions
{
    public enum MainAccountType
    {
        /// <summary>
        /// ذمم عملاء
        /// </summary>
        ClientReceivables,
        /// <summary>
        /// ذمم شركات
        /// </summary>
        CompanyReceivables,
        /// <summary>
        /// إيرادات عامة
        /// </summary>
        Income,
        /// <summary>
        /// مصاريف عامة
        /// </summary>
        Expense,
        /// <summary>
        /// حوالات مباشرة
        /// </summary>
        DirectTransfer
    }
}
