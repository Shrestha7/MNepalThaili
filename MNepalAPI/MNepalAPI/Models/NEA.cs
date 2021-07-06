using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNepalAPI.Models
{
    public class NEA
    {
        public string neaID { get; set; }
        public string neaBranchCode { get; set; }
        public string neaBranchName { get; set; }
        public string tid { get; set; }
        public string sc { get; set; }
        public string username { get; set; }
        public string amount { get; set; }
        public string destinationNumber { get; set; }
        public string remark { get; set; }
        public string src { get; set; }
        public string scn { get; set; }
        public string neaCustomerId { get; set; }
        public string neaCustomerName { get; set; }
        public string merchantName { get; set; }
        public string merchantType { get; set; }
    }

    public class NEABranch
    {
        public string serviceId { get; set; }
        public string serviceCode { get; set; }
        public string field1 { get; set; }
        public string field2 { get; set; }
        public string field3 { get; set; }
        public string field4 { get; set; }
        public string field5 { get; set; }
        public string retrivalReference { get; set; }
        public string additionalData { get; set; }
        public string userName { get; set; }

        public string MyProperty { get; set; }

    }

    public class NEABranchResponse
    {
        public string ResultCode { get; set; }
        public string ResultDescription { get; set; }
        public string AdditionalData { get; set; }
    }


    public class AdditionalData
    {
        public Branch[] branch { get; set; }
    }

    public class Branch
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class NEABranchResult
    {
        public string resultCode { get; set; }
        public string resultDescription { get; set; }
        public Branch[] branch { get; set; }
    }


    public class JsonResult
    {
        public string d { get; set;}
    }

    public class JsonDetails
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
    }






}