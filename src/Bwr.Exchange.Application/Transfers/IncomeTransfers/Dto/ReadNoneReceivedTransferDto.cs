using Bwr.Exchange.Customers.Dto;
using Bwr.Exchange.Settings.Clients.Dto;
using Bwr.Exchange.Settings.Companies.Dto;
using Bwr.Exchange.Settings.Currencies.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bwr.Exchange.Transfers.IncomeTransfers.Dto
{
    public class ReadNoneReceivedTransferDto
    {
        public int id { get; set; }
        public int status { get; set; }
        public int number { get; set; }

        #region To Company
        public int? toCompanyId { get; set; }
        public ReadCompanyDto ToCompany { get; set; }
        #endregion

        #region To Client
        public int? toClientId { get; set; }
        public ReadClientDto toClient { get; set; }
        #endregion

        #region Currency
        public int currencyId { get; set; }
        public ReadCurrencyDto currency { get; set; }
        #endregion

        #region Beneficiary
        public int? beneficiaryId { get; set; }
        public ReadCustomerDto beneficiary { get; set; }
        #endregion

        #region Sender
        public int? senderId { get; set; }
        public ReadCustomerDto sender { get; set; }
        #endregion

        #region Income Transfer
        public int? incomeTransferId { get; set; }
        public ReadIncomeTransferDto incomeTransfer { get; set; }
        #endregion

        public int paymentType { get; set; }
        public double amount { get; set; }
        public double commission { get; set; }
        public double companyCommission { get; set; }
        public double clientCommission { get; set; }
    }
}
