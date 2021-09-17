using System;
using System.Collections.Generic;

namespace CustApp.Models
{
    public class CIPS
    {
        public class ConnectIPSTokenResponse
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public string refresh_token { get; set; }
            public int expires_in { get; set; }
            public string scope { get; set; }
            public string customer_details { get; set; }

            public DateTime timestamp { get; set; }
            public int status { get; set; }
            public string error { get; set; }
            public string message { get; set; }
            public string path { get; set; }
        }


        public class CIPSChargeList
        {
            public GetCharge[] charegeList { get; set; }
        }

        public class GetCharge
        {
            public string scheme { get; set; }
            public string currency { get; set; }
            public float maxAmt { get; set; }
            public float minChargeAmt { get; set; }
            public float maxChargeAmt { get; set; }
            public int percent { get; set; }
        }


        //public class GetBankList
        //{
        //    public BankList[] bankList { get; set; }
        //}

        public class BankList
        {
            public string bankId { get; set; }
            public string bankName { get; set; }
        }


        //public class GetBranchList
        //{
        //    public BranchList[] Property1 { get; set; }
        //}

        public class BranchList
        {
            public string branchId { get; set; }
            public string bankId { get; set; }
            public string branchName { get; set; }
        }

        public class ConnectIPSDetail
        {
            public string senderAccountName { get; set; }
            public string senderAccountNumber { get; set; }
            public decimal amount { get; set; }
            public string beneficiaryAccountNumber { get; set; }
            public string beneficiaryAccountName { get; set; }
            public string bankId { get; set; }
            public string bankBranchId { get; set; }
            public string transactionDetail { get; set; }
            public string bankName { get; set; }
            public string branchName { get; set; }
            public string pin { get; set; }
            public string TokenUnique { get; set; }
        }

        public class Cipsbatchdetail
        {
            public string batchId { get; set; }
            public decimal batchAmount { get; set; }
            public int batchCount { get; set; }
            public string batchCrncy { get; set; }
            public string categoryPurpose { get; set; }
            public string debtorAgent { get; set; }
            public string debtorBranch { get; set; }
            public string debtorName { get; set; }
            public string debtorAccount { get; set; }
            public string debtorIdType { get; set; }
            public string debtorIdValue { get; set; }
            public string debtorAddress { get; set; }
            public string debtorPhone { get; set; }
            public string debtorMobile { get; set; }
            public string debtorEmail { get; set; }
        }

        public class Cipstransactiondetaillist
        {
            public string instructionId { get; set; }
            public string endToEndId { get; set; }
            public decimal amount { get; set; }
            public string creditorAgent { get; set; }
            public string creditorBranch { get; set; }
            public string creditorName { get; set; }
            public string creditorAccount { get; set; }
            public string creditorIdType { get; set; }
            public string creditorIdValue { get; set; }
            public string creditorAddress { get; set; }
            public string creditorPhone { get; set; }
            public string creditorMobile { get; set; }
            public string creditorEmail { get; set; }
            public int addenda1 { get; set; }
            public string addenda2 { get; set; }
            public string addenda3 { get; set; }
            public string addenda4 { get; set; }
        }

        public class ConnectIPS
        {
            public Cipsbatchdetail cipsBatchDetail { get; set; }
            public List<Cipstransactiondetaillist> cipsTransactionDetailList { get; set; }
            public string token { get; set; }
            public string username { get; set; }
        }

        public class ConnectIPSResponse
        {
            public Cipsbatchresponse cipsBatchResponse { get; set; }
            public Cipstxnresponselist[] cipsTxnResponseList { get; set; }
        }

        public class Cipsbatchresponse
        {
            public string responseCode { get; set; }
            public string responseMessage { get; set; }
            public string batchId { get; set; }
            public string debitStatus { get; set; }
            public int id { get; set; }
        }

        public class Cipstxnresponselist
        {
            public string responseCode { get; set; }
            public string responseMessage { get; set; }
            public int id { get; set; }
            public string instructionId { get; set; }
            public string creditStatus { get; set; }
        }

        public class CheckPin
        {
            public string userName { get; set; }
            public string pin { get; set; }
            public string message { get; set; }
        }

        public class ValidateCreditorBankAccount
        {
            public string bankId { get; set; }
            public string accountId { get; set; }
            public string accountName { get; set; }
            public string branchId { get; set; }
            public string currency { get; set; }
            public string responseCode { get; set; }
            public string responseMessage { get; set; }
            public int matchPercentate { get; set; }
            public string baseUrl { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string TokenUnique { get; set; }
            public string pin { get; set; }
        }

    }
}