namespace CustApp.Models
{

    public class NEAFundTransfer
    {
        public NEAFundTransfer(string tid, string sc, string mobile, string sa, string amount, string da, string note, string TPin, string SCNo, string NEABranchName, string CustomerID, string Remarks)
        {
            this.tid = tid;
            this.sc = sc;
            this.mobile = mobile;
            this.sa = sa;
            this.amount = amount;
            this.da = da;
            this.TPin = TPin;
            this.note = note;
            this.SCNo = SCNo;
            this.NEABranchName = NEABranchName;
            this.CustomerID = CustomerID;
            this.Remarks = Remarks;
        }



        public NEAFundTransfer()
        {

        }

        public string NEABranchName { get; set; }
        public string NEABranchCode { get; set; }
        public string SCNo { get; set; }
        public string Remarks { get; set; }
        public string CustomerID { get; set; }
        public string tid { get; set; }
        public string mobile { get; set; }
        public string sc { get; set; }
        public string sa { get; set; }
        public string da { get; set; }
        public string amount { get; set; }
        public string TPin { get; set; }
        public string note { get; set; }
        public string TransactionMedium { get; set; }
        public string Mode { get; set; }
        public string CustomerName { get; set; }
        public string TotalAmountDue { get; set; }
        public string ClientCode { get; set; }
        public string UserName { get; set; }
        public string refStan { get; set; }
        public string billNumber { get; set; }
        public string responseCode { get; set; }
        public string retrievalReference { get; set; }
        public string destBranchCode { get; set; }
        public string tokenID { get; set; }
        public string pin { get; set; }
        public string scn { get; set; }
        public string merchantName { get; set; }
        public string merchantType { get; set; }

        public bool valid()
        {
            if (this.tid != "" && this.sc != "" && this.mobile != "" && this.amount != "")
                return true;
            else
                return false;
        }
        //For MNPaypointPayments
        public string description { get; set; }
        public string billDate { get; set; }
        public string billAmount { get; set; }
        public string totalAmount { get; set; }
        public string status { get; set; }
        public string destination { get; set; }

        public string TokenUnique { get; set; }

    }

    public class NEADetails
    {
        public string serviceId { get; set; }
        public string serviceCode { get; set; }
        public string tokenUnique { get; set; }
        public string scNO { get; set; }
        public string neaBranchName { get; set; }
        public string neaBranchCode { get; set; }
        public string custromerId { get; set; }
        public string field1 { get; set; }
        public string field2 { get; set; }
        public string field3 { get; set; }
        public string field4 { get; set; }
        public string field5 { get; set; }
        public string resultCode { get; set; }
        public string resultDescription { get; set; }
        public string additionalData { get; set; }
        public string userName { get; set; }
    }

    public class NEABranchDetails
    {
        public string resultCode { get; set; }
        public string resultDescription { get; set; }
        public Branch[] branch { get; set; }
    }

    public class Branch
    {
        public string code { get; set; }
        public string name { get; set; }
    }


    public class NeaBillResponse
    {
        public string ResultCode { get; set; }
        public string ResultDescription { get; set; }
        public string AdditionalData { get; set; }
    }




}