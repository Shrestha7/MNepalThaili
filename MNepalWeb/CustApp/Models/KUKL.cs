﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustApp.Models
{
    public class KUKL
    {
        public class KUKLBranch
        {
            public string branch { get; set; }
            public string module { get; set; }
            public string tokenUnique { get; set; }
            public string connectionNo { get; set; }
            public string kuklBranchName { get; set; }
            public string branchcode { get; set; }
            public string applicationId { get; set; }
        }


        public class KUKLModule
        {
            public string billPaymentMode { get; set; }
            public string paymentCode { get; set; }
        }

        public class KUKLBillDetails
        {
            public string address { get; set; }
            public string penalty { get; set; }
            public string name { get; set; }
            public string connection_no { get; set; }
            public float net_amount { get; set; }
            public string TPin { get; set; }
            public string Remarks { get; set; }
            public string applicationId { get; set; }
        }

    }
}