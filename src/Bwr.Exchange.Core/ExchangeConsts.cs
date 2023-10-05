using Bwr.Exchange.Debugging;

namespace Bwr.Exchange
{
    public class ExchangeConsts
    {
        public const string LocalizationSourceName = "Exchange";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;


        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public static readonly string DefaultPassPhrase =
            DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "971e314e469643e0a499dd0bf45e986e";
    }
}
