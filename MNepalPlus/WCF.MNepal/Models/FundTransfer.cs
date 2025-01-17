﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCF.MNepal
{
    public class FundTransfer
    {
        // public FundTransfer fundtransfer;
        /*
        public FundTransfer(FundTransfer fundtransfer)
        {
            this.fundtransfer = fundtransfer;
        }*/

        public string tid { get; set; }
        public string mobile { get; set; }
        public string sc { get; set; }

        public string sa { get; set; }

        public string da { get; set; }
        public string vid { get; set; }

        public string amount { get; set; }

        public string pin { get; set; }
        public string note { get; set; }
        public string sourcechannel { get; set; }

        public string transactioncode { get; set; }

        /// <summary>
        /// ///////////////////addition for merchant
        /// </summary>
        public string billNo { get; set; }
        public string studName { get; set; }

        public string merchantType { get; set; }

        public string clss { get; set; }
        public string year { get; set; }
        public string month { get; set; }
        public string rollNo { get; set; }
        public string remarks { get; set; }
        public string agentName { get; set; }
        public string Desc1New { get; set; }
        public string Desc1RevNew { get; set; }
        public string RemarkRevNew { get; set; }
        public string merchantName { get; set; }
        public string destBranchCode { get; set; }
        
        public string scn { get; set; }
        public string customerId { get; set; }
        public string customerName { get; set; }
       
        public string special2 { get; set; }
        public string Desc2New { get; set; }
        public string account { get; set; }
        public string module { get; set; }

    }
}