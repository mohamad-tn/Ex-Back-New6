namespace Bwr.Exchange
{
    public class AppConsts
    {
        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public const string DefaultPassPhrase = "gsKxGZ012HLL3MI5";
    }

    public class ValidationResultMessage
    {
        public const string EmployeeNameAleadyExist = "EmployeeNameAleadyExist";
        public const string YouMustCreateTreasuryFirst = "YouMustCreateTreasuryFirst";
        public const string EmailAddressAleadyExist = "EmailAddressAleadyExist";
        public const string NameAleadyExist = "NameAleadyExist";
        public const string ShortNameAleadyExist = "ShortNameAleadyExist";
        public const string CodeAleadyExist = "CodeAleadyExist";
        public const string YouCanNotAddTheSameStatusForTwoCode = "YouCanNotAddTheSameStatusForTwoCode";
        public const string ThereIsAnotherMeetingAtTheSameTime = "ThereIsAnotherMeetingAtTheSameTime";
        public const string ThereIsAnotherCallAtTheSameTime = "ThereIsAnotherCallAtTheSameTime";
        public const string CommusionAleadyExistForThisCurrency = "CommusionAleadyExistForThisCurrency";
        public const string CurrentUserIsNotAssociatedWithBranchAndTreasury = "CurrentUserIsNotAssociatedWithBranchAndTreasury";
        public const string TheOutgoingTransferHasBeenDisbursedItCannotBeModified = "TheOutgoingTransferHasBeenDisbursedItCannotBeModified";
        public const string TheDestinationBranchDoseNotDealInThisCurrency = "TheDestinationBranchDoseNotDealInThisCurrency";
    }
}
