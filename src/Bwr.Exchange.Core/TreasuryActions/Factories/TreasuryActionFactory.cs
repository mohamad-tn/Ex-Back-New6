using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Bwr.Exchange.Transfers.IncomeTransfers.Services.Interfaces;
using Bwr.Exchange.TreasuryActions.Services;
using Bwr.Exchange.TreasuryActions.Services.Implement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bwr.Exchange.TreasuryActions.Factories
{
    public class TreasuryActionFactory : ITreasuryActionFactory
    {
        private readonly IRepository<TreasuryAction> _treasuryActionRepository;
        private readonly IIncomeTransferDetailManager _incomeTransferDetailManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        
        public TreasuryActionFactory(
            IRepository<TreasuryAction> treasuryActionRepository,
            IIncomeTransferDetailManager incomeTransferDetailManager, 
            IUnitOfWorkManager unitOfWorkManager)
        {
            _treasuryActionRepository = treasuryActionRepository;
            _incomeTransferDetailManager = incomeTransferDetailManager;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public ITreasuryActionDomainService CreateService(TreasuryAction input)
        {
            //صرف
            if (input.ActionType == TreasuryActionType.Spend)
            {
                //الحساب الرئيسي مصاريف عامة وجهة الصرف الصندوق
                if(input.MainAccount == MainAccountType.Expense && input.TreasuryId != null)
                    return new SpendMainAccountExpensesExchangePartyTreasuryDomainService(_treasuryActionRepository, _unitOfWorkManager, input);

                //الحساب الرئيسي مصاريف عامة وجهة الصرف العميل
                if (input.MainAccount == MainAccountType.Expense && input.ExchangePartyClientId != null)
                    return new SpendMainAccountExpensesExchangePartyClientDomainService(_treasuryActionRepository, _unitOfWorkManager, input);

                //الحساب الرئيسي مصاريف عامة وجهة الصرف الشركة
                if (input.MainAccount == MainAccountType.Expense && input.ExchangePartyCompanyId != null)
                    return new SpendMainAccountExpensesExchangePartyCompanyDomainService(_treasuryActionRepository, _unitOfWorkManager, input);
                
                //الحساب الرئيسي ذمم عملاء وجهة الصرف الصندوق
                if (input.MainAccount == MainAccountType.ClientReceivables && input.TreasuryId != null)
                    return new SpendMainAccountClientExchangePartyTreasuryDomainService(_treasuryActionRepository, _unitOfWorkManager, input);

                //الحساب الرئيسي ذمم عملاء وجهة الصرف العملاء
                if (input.MainAccount == MainAccountType.ClientReceivables && input.ExchangePartyClientId != null)
                    return new SpendMainAccountClientExchangePartyClientDomainService(_treasuryActionRepository, _unitOfWorkManager, input);

                //الحساب الرئيسي ذمم عملاء وجهة الصرف الشركة
                if (input.MainAccount == MainAccountType.ClientReceivables && input.ExchangePartyCompanyId != null)
                    return new SpendMainAccountClientExchangePartyCompanyDomainService(_treasuryActionRepository, _unitOfWorkManager, input);

                //الحساب الرئيسي ذمم شركات وجهة الصرف الصندوق
                if (input.MainAccount == MainAccountType.CompanyReceivables && input.TreasuryId != null)
                    return new SpendMainAccountCompanyExchangePartyTreasuryDomainService(_treasuryActionRepository, _unitOfWorkManager, input);

                //الحساب الرئيسي ذمم شركات وجهة الصرف الشركة
                if (input.MainAccount == MainAccountType.CompanyReceivables && input.ExchangePartyCompanyId != null)
                    return new SpendMainAccountCompanyExchangePartyCompanyDomainService(_treasuryActionRepository, _unitOfWorkManager, input);

                //الحساب الرئيسي ذمم شركات وجهة الصرف العملاء
                if (input.MainAccount == MainAccountType.CompanyReceivables && input.ExchangePartyClientId != null)
                    return new SpendMainAccountCompanyExchangePartyClientDomainService(_treasuryActionRepository, _unitOfWorkManager, input);

                //الحساب الرئيسي حوالة مباشرة وجهة الصرف الصندوق الرئيسي 
                if (input.MainAccount == MainAccountType.DirectTransfer && input.TreasuryId != null)
                    return new SpendMainAccountDirectTransferExchangePartyTreasuryDomainService(_treasuryActionRepository, _unitOfWorkManager, _incomeTransferDetailManager, input);

                //الحساب الرئيسي حوالة مباشرة وجهة الصرف العميل  
                if (input.MainAccount == MainAccountType.DirectTransfer && input.ExchangePartyClientId != null)
                    return new SpendMainAccountDirectTransferExchangePartyClientDomainService(_treasuryActionRepository, _unitOfWorkManager, _incomeTransferDetailManager, input);

                //الحساب الرئيسي حوالة مباشرة وجهة الصرف الشركة  
                if (input.MainAccount == MainAccountType.DirectTransfer && input.ExchangePartyCompanyId != null)
                    return new SpendMainAccountDirectTransferExchangePartyCompanyDomainService(_treasuryActionRepository, _unitOfWorkManager, _incomeTransferDetailManager, input);

            }

            //قبض
            if (input.ActionType == TreasuryActionType.Receipt)
            {
                //الحساب الرئيسي إيرادات عامة وجهة الصرف الصندوق
                if (input.MainAccount == MainAccountType.Income && input.TreasuryId != null)
                    return new ReceiptMainAccountIncomesExchangePartyTreasuryDomainService(_treasuryActionRepository, _unitOfWorkManager, input);

                //الحساب الرئيسي إيرادات عامة وجهة الصرف العميل
                if (input.MainAccount == MainAccountType.Income && input.ExchangePartyClientId != null)
                    return new ReceiptMainAccountIncomesExchangePartyClientDomainService(_treasuryActionRepository, _unitOfWorkManager, input);

                //الحساب الرئيسي إيرادات عامة وجهة الصرف الشركة
                if (input.MainAccount == MainAccountType.Income && input.ExchangePartyCompanyId != null)
                    return new ReceiptMainAccountIncomesExchangePartyCompanyDomainService(_treasuryActionRepository, _unitOfWorkManager, input);

                //الحساب الرئيسي ذمم عملاء وجهة الصرف الصندوق
                if (input.MainAccount == MainAccountType.ClientReceivables && input.TreasuryId != null)
                    return new ReceiptMainAccountClientExchangePartyTreasuryDomainService(_treasuryActionRepository, _unitOfWorkManager, input);

                //الحساب الرئيسي ذمم عملاء وجهة الصرف العميل
                if (input.MainAccount == MainAccountType.ClientReceivables && input.ExchangePartyClientId != null)
                    return new ReceiptMainAccountClientExchangePartyClientDomainService(_treasuryActionRepository, _unitOfWorkManager, input);

                //الحساب الرئيسي ذمم عملاء وجهة الصرف العميل
                if (input.MainAccount == MainAccountType.ClientReceivables && input.ExchangePartyCompanyId != null)
                    return new ReceiptMainAccountClientExchangePartyCompanyDomainService(_treasuryActionRepository, _unitOfWorkManager, input);

                //الحساب الرئيسي ذمم شركات وجهة الصرف الشركة
                if (input.MainAccount == MainAccountType.CompanyReceivables && input.TreasuryId != null)
                    return new ReceiptMainAccountCompanyExchangePartyTreasuryDomainService(_treasuryActionRepository, _unitOfWorkManager, input);

                //الحساب الرئيسي ذمم شركات وجهة الصرف الشركة
                if (input.MainAccount == MainAccountType.CompanyReceivables && input.ExchangePartyCompanyId != null)
                    return new ReceiptMainAccountCompanyExchangePartyCompanyDomainService(_treasuryActionRepository, _unitOfWorkManager, input);

                //الحساب الرئيسي ذمم شركات وجهة الصرف العميل
                if (input.MainAccount == MainAccountType.CompanyReceivables && input.ExchangePartyClientId != null)
                    return new ReceiptMainAccountCompanyExchangePartyClientDomainService(_treasuryActionRepository, _unitOfWorkManager, input);

            }

            return null;
        }
    }
}
