using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCF.MNepal.Models
{
    public class KUKL
    {
        public class KUKLBillRequest
        {
            public string username { get; set; }
            public string connectionNo { get; set; }
            public string merchantId { get; set; }
            public string txnReferenceNo { get; set; }
            public int txnAmount { get; set; }
            public string bankId { get; set; }
            public string txnDate { get; set; }
            public string branchcode { get; set; }
            public string module { get; set; }
            public string tId { get; set; }
            public string sc { get; set; }
            public string mobile { get; set; }
            public string da { get; set; }
            public string pin { get; set; }
            public string destBranchCode { get; set; }
            public string tokenId { get; set; }
            public string merchantType { get; set; }
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
    }
}