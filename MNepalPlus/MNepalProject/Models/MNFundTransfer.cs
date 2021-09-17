using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNepalProject.Models
{
    public class MNFundTransfer : ReplyMessage
    {
        public MNFundTransfer(string tid, string sc, string mobile, string sa, string amount, string da, string note, string pin, string sourcechannel, string billNo, string studName, string merchantType)
        {
            this.tid = tid;
            this.sc = sc;
            this.mobile = mobile;
            this.sa = sa;
            this.amount = amount;
            this.da = da;
            this.note = note;
            this.pin = pin;
            this.sourcechannel = sourcechannel;
            //add for merchant
            this.billNo = billNo;
            this.studName = studName;
            this.merchantType = merchantType;
        }

        public MNFundTransfer(string tid, string sc, string mobile, string sa, string amount, string da, string note, string pin, string sourcechannel, string reverseStatus, string merchantType)
        {
            this.tid = tid;
            this.sc = sc;
            this.mobile = mobile;
            this.sa = sa;
            this.amount = amount;
            this.da = da;
            this.note = note;
            this.pin = pin;
            this.sourcechannel = sourcechannel;
            this.reverseStatus = reverseStatus;
            this.merchantType = merchantType;
        }

        public MNFundTransfer(string tid, string sc, string mobile, string sa, string amount, string da, string note, string pin, string sourcechannel, string merchantType)
        {
            this.tid = tid;
            this.sc = sc;
            this.mobile = mobile;
            this.sa = sa;
            this.amount = amount;
            this.da = da;
            this.note = note;
            this.pin = pin;
            this.sourcechannel = sourcechannel;
            this.merchantType = merchantType;
        }
        public MNFundTransfer(string tid, string sc, string mobile, string sa, string amount, string da, string note, string pin, string sourcechannel)
        {
            this.tid = tid;
            this.sc = sc;
            this.mobile = mobile;
            this.sa = sa;
            this.amount = amount;
            this.da = da;
            this.note = note;
            this.pin = pin;
            this.sourcechannel = sourcechannel;
        }



        public MNFundTransfer(string tid, string sc, string mobile, string sa, string amount, string da, string note, string pin, string sourcechannel, string merchantType, string Desc1New, string Desc1RevNew, string RemarkRevNew)
        {
            this.tid = tid;
            this.sc = sc;
            this.mobile = mobile;
            this.sa = sa;
            this.amount = amount;
            this.da = da;
            this.note = note;
            this.pin = pin;
            this.sourcechannel = sourcechannel;
            this.merchantType = merchantType;

            this.Desc1New = Desc1New;
            this.Desc1RevNew = Desc1RevNew;
            this.RemarkRevNew = RemarkRevNew;
        }


        public MNFundTransfer(string tid, string sc, string mobile, string sa, string amount, string da, string note, string pin, string sourcechannel, string reverseStatus, string merchantType, string Desc1New, string Desc1RevNew, string RemarkRevNew)
        {
            this.tid = tid;
            this.sc = sc;
            this.mobile = mobile;
            this.sa = sa;
            this.amount = amount;
            this.da = da;
            this.note = note;
            this.pin = pin;
            this.sourcechannel = sourcechannel;
            this.reverseStatus = reverseStatus;
            this.merchantType = merchantType;
            this.Desc1New = Desc1New;
            this.Desc1RevNew = Desc1RevNew;
            this.RemarkRevNew = RemarkRevNew;
        }

        public MNFundTransfer(string tid, string sc, string mobile, string sa, string amount, string da, string note, string pin, string sourcechannel, string destBranchCode, string scn, string consumerId,string customerName, string merchantType,string merchantName)
        {
            this.tid = tid;
            this.sc = sc;
            this.mobile = mobile;
            this.sa = sa;
            this.amount = amount;
            this.da = da;
            this.note = note;
            this.pin = pin;
            this.sourcechannel = sourcechannel;
            //add for merchant
            this.destBranchCode = destBranchCode;
            this.scn = scn;
            this.consumerId = consumerId;
            this.customerName = customerName;
            this.merchantType = merchantType;
            this.merchantName = merchantName;
        }

        public MNFundTransfer(string tid, string sc, string mobile, string sa, string amount, string da, string note, string pin, string sourcechannel, string merchantType, string Desc1New, string Desc1RevNew, string RemarkRevNew, string Desc2New, string account, string special2)
        {
            this.tid = tid;
            this.sc = sc;
            this.mobile = mobile;
            this.sa = sa;
            this.amount = amount;
            this.da = da;
            this.note = note;
            this.pin = pin;
            this.sourcechannel = sourcechannel;
            this.merchantType = merchantType;

            this.Desc1New = Desc1New;
            this.Desc1RevNew = Desc1RevNew;
            this.RemarkRevNew = RemarkRevNew;


            this.Desc2New = Desc2New;
            this.account = account;
            this.special2 = special2;
        }


        public MNFundTransfer(string tid, string sc, string mobile, string sa, string amount, string da, string note, string pin, string sourcechannel, string reverseStatus, string merchantType, string Desc1New, string Desc1RevNew, string RemarkRevNew, string Desc2New, string account, string special2)
        {
            this.tid = tid;
            this.sc = sc;
            this.mobile = mobile;
            this.sa = sa;
            this.amount = amount;
            this.da = da;
            this.note = note;
            this.pin = pin;
            this.sourcechannel = sourcechannel;
            this.reverseStatus = reverseStatus;
            this.merchantType = merchantType;
            this.Desc1New = Desc1New;
            this.Desc1RevNew = Desc1RevNew;
            this.RemarkRevNew = RemarkRevNew;


            this.Desc2New = Desc2New;
            this.account = account;
            this.special2 = special2;


        }
        public MNFundTransfer()
        {

        }

        public string tid { get; set; }
        public string mobile { get; set; }
        public string sc { get; set; }

        public string sa { get; set; }

        public string da { get; set; }
        public string vid { get; set; }
        public string description1 { get; set; }
        public string description2 { get; set; }
        public string description3 { get; set; }

        public string amount { get; set; }

        public string pin { get; set; }
        public string note { get; set; }
        public string sourcechannel { get; set; }
        public string prod { get; set; }

        //add for merchant
        public string studName { get; set; }
        public string billNo { get; set; }
        public string merchantType { get; set; }

        public string reverseStatus { get; set; }
        public string enteredAtDate { get; set; }
        public string retrievalReferenceNew { get; set; }
        public string special2 { get; set; }
        public string Desc1New { get; set; }
        public string Desc1RevNew { get; set; }
        public string RemarkRevNew { get; set; }
        public string destBranchCode { get; set; }
        public string scn { get; set; }
        public string consumerId { get; set; }
        public string customerName { get; set; }
        public string merchantName { get; set; }
        public string Desc2New { get; set; }
        public string account { get; set; }
        public bool valid()
        {
            if (this.tid != "" && this.sc != "" && this.mobile != "" && this.amount != "")
                return true;
            else
                return false;
        }

    }
}
















