using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Bwr.Exchange.Authorization
{
    public class ExchangeAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);

            //Users
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Users_Create);
            context.CreatePermission(PermissionNames.Pages_Users_Edit, L("EditUser"));
            context.CreatePermission(PermissionNames.Pages_Users_Delete, L("DeleteUser"));
            context.CreatePermission(PermissionNames.Pages_Users_ResetPassword, L("ResetPassword"));
            context.CreatePermission(PermissionNames.Pages_Users_ChangePermissions, L("ChangePermissions"));

            //Roles
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            context.CreatePermission(PermissionNames.Pages_Roles_Create, L("CreateNewRole"));
            context.CreatePermission(PermissionNames.Pages_Roles_Edit, L("EditRole"));
            context.CreatePermission(PermissionNames.Pages_Roles_Delete, L("DeleteRole"));

            //Countries
            context.CreatePermission(PermissionNames.Pages_Countries, L("Countries"));
            context.CreatePermission(PermissionNames.Pages_Countries_Create, L("CreateNewCountry"));
            context.CreatePermission(PermissionNames.Pages_Countries_Edit, L("EditCountry"));
            context.CreatePermission(PermissionNames.Pages_Countries_Delete, L("DeleteCountry"));

            //Currencies
            context.CreatePermission(PermissionNames.Pages_Currencies, L("Currencies"));
            context.CreatePermission(PermissionNames.Pages_Currencies_Create, L("CreateNewCurrency"));
            context.CreatePermission(PermissionNames.Pages_Currencies_Edit, L("EditCurrency"));
            context.CreatePermission(PermissionNames.Pages_Currencies_Delete, L("DeleteCurrency"));

            //TreasuryBalances
            context.CreatePermission(PermissionNames.Pages_InitialBalance, L("InitialBalance"));
            context.CreatePermission(PermissionNames.Pages_TreasuryBalances, L("TreasuryBalances"));
            context.CreatePermission(PermissionNames.Pages_TreasuryBalances_Create, L("CreateNewBalance"));
            context.CreatePermission(PermissionNames.Pages_TreasuryBalances_Edit, L("EditBalance"));
            //context.CreatePermission(PermissionNames.Pages_TreasuryBalances_Delete, L("DeleteBalance"));

            //Incomes
            context.CreatePermission(PermissionNames.Pages_Incomes, L("Incomes"));
            context.CreatePermission(PermissionNames.Pages_Incomes_Create, L("CreateNewIncome"));
            context.CreatePermission(PermissionNames.Pages_Incomes_Edit, L("EditIncome"));
            context.CreatePermission(PermissionNames.Pages_Incomes_Delete, L("DeleteIncome"));

            //Expenses
            context.CreatePermission(PermissionNames.Pages_Expenses, L("Expenses"));
            context.CreatePermission(PermissionNames.Pages_Expenses_Create, L("CreateNewExpense"));
            context.CreatePermission(PermissionNames.Pages_Expenses_Edit, L("EditExpense"));
            context.CreatePermission(PermissionNames.Pages_Expenses_Delete, L("DeleteExpense"));

            //ExchangePrices
            context.CreatePermission(PermissionNames.Pages_ExchangePrices, L("ExchangePrices"));
            context.CreatePermission(PermissionNames.Pages_ExchangePrices_Create, L("CreateNewExchangePrice"));
            context.CreatePermission(PermissionNames.Pages_ExchangePrices_Edit, L("EditExchangePrice"));
            context.CreatePermission(PermissionNames.Pages_ExchangePrices_Delete, L("DeleteExchangePrice"));

            //Companies
            context.CreatePermission(PermissionNames.Pages_Companies, L("Companies"));
            context.CreatePermission(PermissionNames.Pages_Companies_Create, L("CreateNewCompany"));
            context.CreatePermission(PermissionNames.Pages_Companies_Edit, L("EditCompany"));
            context.CreatePermission(PermissionNames.Pages_Companies_Delete, L("DeleteCompany"));

            //Clients
            context.CreatePermission(PermissionNames.Pages_Clients, L("Clients"));
            context.CreatePermission(PermissionNames.Pages_Clients_Create, L("CreateNewClient"));
            context.CreatePermission(PermissionNames.Pages_Clients_Edit, L("EditClient"));
            context.CreatePermission(PermissionNames.Pages_Clients_Delete, L("DeleteClient"));

            //Commisions
            context.CreatePermission(PermissionNames.Pages_Commisions, L("Commisions"));
            context.CreatePermission(PermissionNames.Pages_Commisions_Create, L("CreateNewCommision"));
            context.CreatePermission(PermissionNames.Pages_Commisions_Edit, L("EditCommision"));
            context.CreatePermission(PermissionNames.Pages_Commisions_Delete, L("DeleteCommision"));

            //OutgoingTransfers
            context.CreatePermission(PermissionNames.Pages_OutgoingTransfers, L("OutgoingTransfers"));
            context.CreatePermission(PermissionNames.Pages_OutgoingTransfers_Create, L("CreateNewOutgoingTransfer"));
            context.CreatePermission(PermissionNames.Pages_OutgoingTransfers_Edit, L("EditOutgoingTransfer"));

            //Branches
            context.CreatePermission(PermissionNames.Pages_Branches, L("Branches"));
            context.CreatePermission(PermissionNames.Pages_Branches_Create, L("CreateNewBranche"));
            context.CreatePermission(PermissionNames.Pages_Branches_Edit, L("EditBranche"));
            context.CreatePermission(PermissionNames.Pages_Branches_Delete, L("DeleteBranche"));

            //IncomeTransferNotAccepted
            context.CreatePermission(PermissionNames.Pages_IncomeTransferNotAccepted, L("IncomeTransferNotAccepted"));
            context.CreatePermission(PermissionNames.Pages_IncomeTransferNotAccepted_Accept, L("AcceptBranchTransfer"));
            context.CreatePermission(PermissionNames.Pages_IncomeTransferNotAccepted_Reject, L("RejectBranchTransfer"));

            //BranchTransfersCounter
            context.CreatePermission(PermissionNames.Pages_BranchTransfersCounter, L("BranchTransfersCounter"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, ExchangeConsts.LocalizationSourceName);
        }
    }
}
