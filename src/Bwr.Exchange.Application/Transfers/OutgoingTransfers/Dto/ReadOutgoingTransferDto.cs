using Abp.Application.Services.Dto;
using Bwr.Exchange.Customers.Dto;
using Bwr.Exchange.Settings.Branches;
using Bwr.Exchange.Settings.Branches.Dto;
using Bwr.Exchange.Settings.Clients.Dto;
using Bwr.Exchange.Settings.Companies.Dto;
using Bwr.Exchange.Settings.Countries.Dto;
using Bwr.Exchange.Settings.Currencies.Dto;
using Bwr.Exchange.Settings.Treasury.Dto;

namespace Bwr.Exchange.Transfers.OutgoingTransfers.Dto
{
    public class ReadOutgoingTransferDto : EntityDto
    {
        public int number { get; set; }
        public int paymentType { get; set; }
        public double amount { get; set; }
        public double commission { get; set; }
        public double companyCommission { get; set; }
        public double clientCommission { get; set; }
        public string date { get; set; }
        public double receivedAmount { get; set; }
        public string instrumentNo { get; set; } //رقم الصك
        public string reason { get; set; }
        public string note { get; set; }
        public int status { get; set; }

        public int countryId { get; set; }
        public CountryDto country { get; set; }

        public int currencyId { get; set; }
        public CurrencyDto currency { get; set; }

        public int? treasuryId { get; set; }
        public TreasuryDto treasury { get; set; }


        public int? beneficiaryId { get; set; }
        public CustomerDto beneficiary { get; set; }

        public int? senderId { get; set; }
        public CustomerDto sender { get; set; }
        public int? fromCompanyId { get; set; }
        public CompanyDto fromCompany { get; set; }

        public int? toCompanyId { get; set; }
        public CompanyDto toCompany { get; set; }

        public int? fromClientId { get; set; }
        public ClientDto fromClient { get; set; }

        public int? fromBranchId { get; set; }
        public BranchDto fromBranch { get; set; }

        public int? toBranchId { get; set; }
        public BranchDto toBranch { get; set; }
    }
}
