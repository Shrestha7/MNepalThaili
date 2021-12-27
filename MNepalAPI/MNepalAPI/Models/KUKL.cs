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
    }


    public class KUKLBillDetailResponse
    {
        public object address { get; set; }
        public float penalty { get; set; }
        public string name { get; set; }
        public string connection_no { get; set; }
        public float net_amount { get; set; }
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
        public int txnAmount { get; set; }
        public string bankId { get; set; }
        public string txnDate { get; set; }
        public string branchcode { get; set; }
        public string module { get; set; }
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




}