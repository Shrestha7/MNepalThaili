using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNepalAPI.Models
{
    public class ConnectIPS
    {
        public Cipsbatchdetail cipsBatchDetail { get; set; }
        public Cipstransactiondetaillist[] cipsTransactionDetailList { get; set; }
        public string token { get; set; }
        public string username { get; set; }
    }

    public class ConnectIPSUserAuthenticaiton
    {
        public string username { get; set; }
        public string password { get; set; }
        public string refresh_token { get; set; }
    }

    public class ConnectIPSTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
        public string customer_details { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
    }

    public class BankList
    {
        public string bankId { get; set; }
        public string bankName { get; set; }

    }

    public class CIPSBnakBranchDetails
    {
        public string branchId { get; set; }
        public string bankId { get; set; }
        public string branchName { get; set; }
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

    public class CIPSResponse
    {
        public string batchResponseCode { get; set; }
        public string batchResponseMessage { get; set; }
        public string batchId { get; set; }
        public string debitStatus { get; set; }
        public int batchResponseId { get; set; }
        public string txnResponseCode{ get; set; }
        public string txnResponseMessage { get; set; }
        public int txnId { get; set; }
        public string instructionId { get; set; }
        public string creditStatus { get; set; }
        public string amount { get; set; }
        public DateTime dateTime { get; set; }
        public string username { get; set; }
        public string thailiUserName { get; set; }
    }

    public class CIPSRequest
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
        public DateTime dateTime { get; set; }
        public int addenda1 { get; set; }
        public string addenda2 { get; set; }
        public string addenda3 { get; set; }
        public string addenda4 { get; set; }
        public string remark { get; set; }
        public string thailiUserName { get; set; }

    }

    public class BasicAuth
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class CheckPin
    {
        public string username { get; set; }
        public string pin { get; set; }
        public string message { get; set; }
    }

    public class MNRequestResponse
    {
        public string originId { get; set; }
        public string originType { get; set; }
        public string serviceCode { get; set; }
        public string sourceBankCode { get; set; }
        public string sourceBranchCode { get; set; }
        public string sourceAccountNumber { get; set; }
        public string destBranchCode { get; set; }
        public string destBankCode { get; set; }
        public string destAccountNumber { get; set; }
        public decimal amount { get; set; }
        public decimal feeAmount { get; set; }
        public string feeId { get; set; }
        public string traceNo { get; set; }
        public DateTime tranDate { get; set; }
        public string tranTime { get; set; }
        public string retrievalReference { get; set; }
        public string responseCode { get; set; }
        public string responseDescription { get; set; }
        public string balance { get; set; }
        public string accountHolderName { get; set; }
        public string miniStmtRecord { get; set; }
        public string tranId { get; set; }
        public string destUsername { get; set; }
        public string desc1 { get; set; }
        public string desc2 { get; set; }
        public string desc3 { get; set; }
        public string reversalStatus { get; set; }
        public string oTraceNo { get; set; }
        public string oTranDateTime { get; set; }
        public string isProcessed { get; set; }
        public string status { get; set; }
        public string fromSMS { get; set; }
        public string remark { get; set; }
        public string smsAlertType { get; set; }
        public DateTime enteredAt { get; set; }
        public int merchantId { get; set; }
        public string uId { get; set; }
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
    }
}