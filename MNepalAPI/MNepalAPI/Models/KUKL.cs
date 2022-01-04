using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNepalAPI.Models
{

    public class KUKLBillDetail
    {
        public string connectionNo { get; set; }
        public string branchcode { get; set; }
        public string module { get; set; }
        public string applicationId { get; set; }
        
    }


    public class KUKLBillDetailResponse
    {
        public object address { get; set; }
        public float penalty { get; set; }
        public string name { get; set; }
        public string connection_no { get; set; }
        public float net_amount { get; set; }
        public string applicationId { get; set; }
        public string result { get; set; }
        public string billMonth { get; set; }
        public string board_amount { get; set; }
        public string areaNo { get; set; }
    }


    public class KUKL
    {
        public string branch { get; set; }
        public string branchCode { get; set; }
    }


    public class KUKLBillRequest
    {
        public string username { get; set; }
        public string connectionNo { get; set; }
        public string merchantId { get; set; }
        public string txnReferenceNo { get; set; }
        public string txnAmount { get; set; }
        public string bankId { get; set; }
        public string txnDate { get; set; }
        public string branchcode { get; set; }
        public string module { get; set; }
        public string tId { get; set; }
        public string sc { get; set; }
        public string mobile { get; set; }
        public string da { get; set; }
        public string pin { get; set; }
      
        public string tokenId { get; set; }
        public string merchantType { get; set; }
    }


    public class KUKLBillResponse
    {
        public string result { get; set; }
        public float amount { get; set; }
        public string txnReferenceNo { get; set; }
        public string recNo { get; set; }
        public string connectionNo { get; set; }
        public string recdate { get; set; }
    }


    public class KUKLPaymentTxnRequest
    {
        public string txnReferenceNo { get; set; }
        public string branchcode { get; set; }
        public string module { get; set; }
    }

    public class KUKLPaymentTxnResponse
    {
        public string result { get; set; }
        public float amount { get; set; }
        public string txnReferenceNo { get; set; }
        public string recNo { get; set; }
        public string connectionNo { get; set; }
        public string recdate { get; set; }
    }

    public class KUKLResponse
    {
        public string AmounttransferredBalance { get; set; }
        public string availableBalance { get; set; }
        public string message { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }

    public class BillPaymentMode
    {
        public string billPaymentMode { get; set; }
        public string paymentCode { get; set; }
    }

}