using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WCF.MNepal.Models;
using WCF.MNepal.Utilities;

namespace WCF.MNepal
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class topupAPI
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
                    ResponseFormat = WebMessageFormat.Json
                    )]
        public string MerchantTransaction(Stream input)
        {
            StreamReader sr = new StreamReader(input);
            string s = sr.ReadToEnd();
            sr.Dispose();
            NameValueCollection qs = HttpUtility.ParseQueryString(s);

            string vid = qs["vid"];
            string destmobile = qs["destmobile"];
            string Amount = qs["Amount"];
            string SourceMobile = qs["SourceMobile"]; 
            string TraceId = qs["TraceId"];
            string CreatDate = qs["CreatedDate"];
            string mobile = qs["mobile"];
            string createdTimeDate = qs["createdTimeDate"];
            DateTime CreatedDate = Convert.ToDateTime(CreatDate);
            string selectADSL = qs["selectADSL"]; //Unlimited //VolumeBased

            //MerchantTransaction merchantTransaction = new MerchantTransaction(vid, "NCELL", destmobile, Convert.ToInt32(Amount), SourceMobile, TraceId, "P", CreatedDate, "", "", mobile, createdTimeDate);

            //PassDataToMerchantTransactionController(merchantTransaction); //insert merchantdetail to db and status to pending
            //                                                              // string topUpResult =TopupMerchantVerfication(merchantTransaction);
            //Task<string> topUpResult = TopupMerchantVerfication(merchantTransaction); // call soapRequest function

            string topupId = "";
            string topupStatus = "";
            string topupResult = "";

            //for (int i = 0; i < 6; i++)
            //{
            //    Console.WriteLine("Sleep for 1 second!");

            //    topupId = topUpResult.Id.ToString();
            //    topupStatus = topUpResult.Status.ToString();
            //    topupResult = topUpResult.Result.ToString();

            //    Thread.Sleep(1000);
            //}

            //return topupStatus;

            //FOR NCELL
            if (vid == "10") //&& statusCode == "200"
            {
                MerchantTransaction merchantTransaction = new MerchantTransaction(vid, "NCELL", destmobile, Convert.ToInt32(Amount), SourceMobile, TraceId, "P", CreatedDate, "", "", mobile, createdTimeDate);

                PassDataToMerchantTransactionController(merchantTransaction); //insert merchantdetail to db and status to pending
                                                                              // string topUpResult =TopupMerchantVerfication(merchantTransaction);
                Task<string> topUpResult = TopupMerchantVerfication(merchantTransaction); // call soapRequest function

                //string topupId = "";
                //string topupStatus = "";
                //string topupResult = "";

                for (int i = 0; i < 6; i++)
                {
                    Console.WriteLine("Sleep for 1 second!");

                    topupId = topUpResult.Id.ToString();
                    topupStatus = topUpResult.Status.ToString();
                    topupResult = topUpResult.Result.ToString();

                    Thread.Sleep(1000);
                }

            }

            //FOR NTC
            if (vid == "2") //&& statusCode == "200"
            {
                MerchantTransaction merchantTransaction = new MerchantTransaction(vid, "NTC", destmobile, Convert.ToInt32(Amount), SourceMobile, TraceId, "P", CreatedDate, "", "", mobile, createdTimeDate);

                PassDataToMerchantTransactionController(merchantTransaction); //insert merchantdetail to db and status to pending
                                                                              // string topUpResult =TopupMerchantVerfication(merchantTransaction);
                Task<string> topUpResult = NTCTopupMerchantVerfication(merchantTransaction); // call soapRequest function

                //string topupId = "";
                //string topupStatus = "";
                //string topupResult = "";

                for (int i = 0; i < 6; i++)
                {
                    Console.WriteLine("Sleep for 1 second!");

                    topupId = topUpResult.Id.ToString();
                    topupStatus = topUpResult.Status.ToString();
                    topupResult = topUpResult.Result.ToString();

                    Thread.Sleep(1000);
                }

            }
            //FOR NT LANDLINE
            if (vid == "7") //&& statusCode == "200"
            {
                MerchantTransaction merchantTransaction = new MerchantTransaction(vid, "PSTN", destmobile, Convert.ToInt32(Amount), SourceMobile, TraceId, "P", CreatedDate, "", "", mobile, createdTimeDate);

                PassDataToMerchantTransactionController(merchantTransaction); //insert merchantdetail to db and status to pending
                                                                              // string topUpResult =TopupMerchantVerfication(merchantTransaction);
                Task<string> topUpResult = NTCTopupMerchantVerfication(merchantTransaction); // call soapRequest function

                //string topupId = "";
                //string topupStatus = "";
                //string topupResult = "";

                for (int i = 0; i < 6; i++)
                {
                    Console.WriteLine("Sleep for 1 second!");

                    topupId = topUpResult.Id.ToString();
                    topupStatus = topUpResult.Status.ToString();
                    topupResult = topUpResult.Result.ToString();

                    Thread.Sleep(1000);
                }

            }
            //FOR ADSL UNLIMITED
            if (vid == "1"  && selectADSL == "Unlimited") //&& statusCode == "200"
            {
                MerchantTransaction merchantTransaction = new MerchantTransaction(vid, "ADSL_UNLIMITED", destmobile, Convert.ToInt32(Amount), SourceMobile, TraceId, "P", CreatedDate, "", "", mobile, createdTimeDate);

                PassDataToMerchantTransactionController(merchantTransaction); //insert merchantdetail to db and status to pending
                                                                              // string topUpResult =TopupMerchantVerfication(merchantTransaction);
                Task<string> topUpResult = ADSLUnlimitedTopupMerchantVerfication(merchantTransaction); // call soapRequest function

                //string topupId = "";
                //string topupStatus = "";
                //string topupResult = "";

                for (int i = 0; i < 6; i++)
                {
                    Console.WriteLine("Sleep for 1 second!");

                    topupId = topUpResult.Id.ToString();
                    topupStatus = topUpResult.Status.ToString();
                    topupResult = topUpResult.Result.ToString();

                    Thread.Sleep(1000);
                }
            }
            //FOR ADSL VOLUME BASED
            if (vid == "1"  && selectADSL == "VolumeBased") //&& statusCode == "200"
            {
                MerchantTransaction merchantTransaction = new MerchantTransaction(vid, "ADSL_VOLUMEBASED", destmobile, Convert.ToInt32(Amount), SourceMobile, TraceId, "P", CreatedDate, "", "", mobile, createdTimeDate);

                PassDataToMerchantTransactionController(merchantTransaction); //insert merchantdetail to db and status to pending
                                                                              // string topUpResult =TopupMerchantVerfication(merchantTransaction);
                Task<string> topUpResult = ADSLVolBasedTopupMerchantVerfication(merchantTransaction); // call soapRequest function

                //string topupId = "";
                //string topupStatus = "";
                //string topupResult = "";

                for (int i = 0; i < 6; i++)
                {
                    Console.WriteLine("Sleep for 1 second!");

                    topupId = topUpResult.Id.ToString();
                    topupStatus = topUpResult.Status.ToString();
                    topupResult = topUpResult.Result.ToString();

                    Thread.Sleep(1000);
                }

            }

            return topupStatus;

        }


        #region Verify Merchant

        public async Task<string> TopupMerchantVerfication(MerchantTransaction mNTransaction)
        {
            MerchantReference.CommonInterfaceSoapClient commonInterfaceSoapClient = new MerchantReference.CommonInterfaceSoapClient();
            MerchantReference.SubmitRequestRequest submitRequest = new MerchantReference.SubmitRequestRequest();
            MerchantReference.HeaderInfo HeaderInfo = new MerchantReference.HeaderInfo();
            MerchantReference.RequestData input = new MerchantReference.RequestData();

            MerchantReference.SubmitRequestResponse submitRequestResponse = new MerchantReference.SubmitRequestResponse();
            ////Testing Header NCELL
            HeaderInfo.H1 = "1";
            HeaderInfo.H2 = "98FE1564-1E60-4C58-9CB1-5E36E0094407";
            HeaderInfo.H3 = "D546D65E-D8B9-4Ef8-9842-3C6d522cFCC3";

            //Live Header NCELL
            //HeaderInfo.H1 = "5";
            //HeaderInfo.H2 = "5C86A0ED-8542-43D3-8E08-64048F8EF104";
            //HeaderInfo.H3 = "36484FA3-AD31-4C25-8617-56BC3205B3A5";

            //HeaderInfo.H1 = System.Web.Configuration.WebConfigurationManager.AppSettings["H1"];
            //HeaderInfo.H2 = System.Web.Configuration.WebConfigurationManager.AppSettings["H2"];
            //HeaderInfo.H3 = System.Web.Configuration.WebConfigurationManager.AppSettings["H3"];

            submitRequest.serviceId = "001"; //Service ID
            submitRequest.serviceCode = "NCELL"; // Service Name
            input.Field1 = mNTransaction.PAN; //?Mobile Number
            input.Field2 = mNTransaction.STAN; //?Retrival Reference

            //input.Field3 = mNTransaction.CreatedDate.ToString("dd/MM/yyyy HH:mm"); //Created Date
            input.Field3 = mNTransaction.CreatedTimeDate; //Created Date

            input.Field4 = mNTransaction.Amount;//Amount
            input.Field5 = mNTransaction.MobileNumber; // Mobile Number (Testing No: 8800000238)

            submitRequestResponse = await commonInterfaceSoapClient.SubmitRequestAsync(HeaderInfo, submitRequest.serviceId, submitRequest.serviceCode, input);
            string responseCode = submitRequestResponse.SubmitRequestResult.ResultCode;

            string resultDescription = submitRequestResponse.SubmitRequestResult.ResultDescription;

            if (responseCode != "")
            {
                if (responseCode == "000") { mNTransaction.Status = "T"; } else { mNTransaction.Status = "F"; }
                mNTransaction.ResponseCode = responseCode;
                mNTransaction.ResponseDescription = resultDescription;
                UpdateMerchantTransactionController(mNTransaction);  // Update and insert responsecode and description to db 
            }
            return responseCode;
        }
        #endregion

        #region Verify Merchant NTC
        public async Task<string> NTCTopupMerchantVerfication(MerchantTransaction mNTransaction)
        {
            MerchantReference.CommonInterfaceSoapClient commonInterfaceSoapClient = new MerchantReference.CommonInterfaceSoapClient();
            MerchantReference.SubmitRequestRequest submitRequest = new MerchantReference.SubmitRequestRequest();
            MerchantReference.HeaderInfo HeaderInfo = new MerchantReference.HeaderInfo();
            MerchantReference.RequestData input = new MerchantReference.RequestData();

            MerchantReference.SubmitRequestResponse submitRequestResponse = new MerchantReference.SubmitRequestResponse();

            string mobNumber = mNTransaction.MobileNumber;
            string serviceCode = "POSTPAID";

            //NT PREPAID Check
            if (mobNumber.Substring(0, 3) == "984" || mobNumber.Substring(0, 3) == "986")
            {
                serviceCode = "PREPAID";
            }

            //NT Landline Check
            if (mNTransaction.MerchantID == "7")
            {
                serviceCode = "PSTN";
                //mNTransaction.MobileNumber = mNTransaction.MobileNumber.Substring(1);
            }

            //test server ko header
            HeaderInfo.H1 = "1";
            HeaderInfo.H2 = "98FE1564-1E60-4C58-9CB1-5E36E0094407";
            HeaderInfo.H3 = "D546D65E-D8B9-4Ef8-9842-3C6d522cFCC3";

            //HeaderInfo.H1 = "5";
            //HeaderInfo.H2 = "5C86A0ED-8542-43D3-8E08-64048F8EF104";
            //HeaderInfo.H3 = "36484FA3-AD31-4C25-8617-56BC3205B3A5";

            //HeaderInfo.H1 = System.Web.Configuration.WebConfigurationManager.AppSettings["H1"];
            //HeaderInfo.H2 = System.Web.Configuration.WebConfigurationManager.AppSettings["H2"];
            //HeaderInfo.H3 = System.Web.Configuration.WebConfigurationManager.AppSettings["H3"];

            submitRequest.serviceId = "002"; //Service ID
            submitRequest.serviceCode = serviceCode; // Service Name
            input.Field1 = mNTransaction.PAN; //?Mobile Number
            input.Field2 = mNTransaction.STAN; //?Retrival Reference

            //input.Field3 = mNTransaction.CreatedDate.ToString("dd/MM/yyyy HH:mm"); //Created Date
            input.Field3 = mNTransaction.CreatedTimeDate; //Created Date
            input.Field4 = mNTransaction.Amount;//Amount
            input.Field5 = mNTransaction.MobileNumber; // Mobile Number (Testing No: 8800000238)

            submitRequestResponse = await commonInterfaceSoapClient.SubmitRequestAsync(HeaderInfo, submitRequest.serviceId, submitRequest.serviceCode, input);
            string responseCode = submitRequestResponse.SubmitRequestResult.ResultCode;
            string resultDescription = submitRequestResponse.SubmitRequestResult.ResultDescription;
            if (responseCode != "")
            {
                if (responseCode == "000") { mNTransaction.Status = "T"; } else { mNTransaction.Status = "F"; }
                mNTransaction.ResponseCode = responseCode;
                mNTransaction.ResponseDescription = resultDescription;
                UpdateMerchantTransactionController(mNTransaction);  // Update and insert responsecode and description to db 
            }
            return responseCode;
        }
        #endregion

        #region Verify Merchant ADSL Unlimited
        public async Task<string> ADSLUnlimitedTopupMerchantVerfication(MerchantTransaction mNTransaction)
        {
            MerchantReference.CommonInterfaceSoapClient commonInterfaceSoapClient = new MerchantReference.CommonInterfaceSoapClient();
            MerchantReference.SubmitRequestRequest submitRequest = new MerchantReference.SubmitRequestRequest();
            MerchantReference.HeaderInfo HeaderInfo = new MerchantReference.HeaderInfo();
            MerchantReference.RequestData input = new MerchantReference.RequestData();

            MerchantReference.SubmitRequestResponse submitRequestResponse = new MerchantReference.SubmitRequestResponse();

            string mobNumber = mNTransaction.MobileNumber;
            string serviceCode = "ADSLU";

            //test server ko header
            HeaderInfo.H1 = "1";
            HeaderInfo.H2 = "98FE1564-1E60-4C58-9CB1-5E36E0094407";
            HeaderInfo.H3 = "D546D65E-D8B9-4Ef8-9842-3C6d522cFCC3";

            //HeaderInfo.H1 = "5";
            //HeaderInfo.H2 = "5C86A0ED-8542-43D3-8E08-64048F8EF104";
            //HeaderInfo.H3 = "36484FA3-AD31-4C25-8617-56BC3205B3A5";

            //HeaderInfo.H1 = System.Web.Configuration.WebConfigurationManager.AppSettings["H1"];
            //HeaderInfo.H2 = System.Web.Configuration.WebConfigurationManager.AppSettings["H2"];
            //HeaderInfo.H3 = System.Web.Configuration.WebConfigurationManager.AppSettings["H3"];

            submitRequest.serviceId = "002"; //Service ID
            submitRequest.serviceCode = serviceCode; // Service Name
            input.Field1 = mNTransaction.PAN; //?Mobile Number
            input.Field2 = mNTransaction.STAN; //?Retrival Reference

            //input.Field3 = mNTransaction.CreatedDate.ToString("dd/MM/yyyy HH:mm"); //Created Date
            input.Field3 = mNTransaction.CreatedTimeDate; //Created Date
            input.Field4 = mNTransaction.Amount;//Amount
            input.Field5 = mNTransaction.MobileNumber; // Mobile Number (Testing No: 8800000238)

            submitRequestResponse = await commonInterfaceSoapClient.SubmitRequestAsync(HeaderInfo, submitRequest.serviceId, submitRequest.serviceCode, input);
            string responseCode = submitRequestResponse.SubmitRequestResult.ResultCode;
            string resultDescription = submitRequestResponse.SubmitRequestResult.ResultDescription;
            if (responseCode != "")
            {
                if (responseCode == "000") { mNTransaction.Status = "T"; } else { mNTransaction.Status = "F"; }
                mNTransaction.ResponseCode = responseCode;
                mNTransaction.ResponseDescription = resultDescription;
                UpdateMerchantTransactionController(mNTransaction);  // Update and insert responsecode and description to db 
            }
            return responseCode;
        }
        #endregion

        #region Verify Merchant ADSL Volume Based
        public async Task<string> ADSLVolBasedTopupMerchantVerfication(MerchantTransaction mNTransaction)
        {
            MerchantReference.CommonInterfaceSoapClient commonInterfaceSoapClient = new MerchantReference.CommonInterfaceSoapClient();
            MerchantReference.SubmitRequestRequest submitRequest = new MerchantReference.SubmitRequestRequest();
            MerchantReference.HeaderInfo HeaderInfo = new MerchantReference.HeaderInfo();
            MerchantReference.RequestData input = new MerchantReference.RequestData();

            MerchantReference.SubmitRequestResponse submitRequestResponse = new MerchantReference.SubmitRequestResponse();

            string mobNumber = mNTransaction.MobileNumber;
            string serviceCode = "ADSLV";

            //test server ko header
            HeaderInfo.H1 = "1";
            HeaderInfo.H2 = "98FE1564-1E60-4C58-9CB1-5E36E0094407";
            HeaderInfo.H3 = "D546D65E-D8B9-4Ef8-9842-3C6d522cFCC3";

            //HeaderInfo.H1 = "5";
            //HeaderInfo.H2 = "5C86A0ED-8542-43D3-8E08-64048F8EF104";
            //HeaderInfo.H3 = "36484FA3-AD31-4C25-8617-56BC3205B3A5";

            //HeaderInfo.H1 = System.Web.Configuration.WebConfigurationManager.AppSettings["H1"];
            //HeaderInfo.H2 = System.Web.Configuration.WebConfigurationManager.AppSettings["H2"];
            //HeaderInfo.H3 = System.Web.Configuration.WebConfigurationManager.AppSettings["H3"];

            submitRequest.serviceId = "002"; //Service ID
            submitRequest.serviceCode = serviceCode; // Service Name
            input.Field1 = mNTransaction.PAN; //?Mobile Number
            input.Field2 = mNTransaction.STAN; //?Retrival Reference

            //input.Field3 = mNTransaction.CreatedDate.ToString("dd/MM/yyyy HH:mm"); //Created Date
            input.Field3 = mNTransaction.CreatedTimeDate; //Created Date
            input.Field4 = mNTransaction.Amount;//Amount
            input.Field5 = mNTransaction.MobileNumber; // Mobile Number (Testing No: 8800000238)

            submitRequestResponse = await commonInterfaceSoapClient.SubmitRequestAsync(HeaderInfo, submitRequest.serviceId, submitRequest.serviceCode, input);
            string responseCode = submitRequestResponse.SubmitRequestResult.ResultCode;
            string resultDescription = submitRequestResponse.SubmitRequestResult.ResultDescription;
            if (responseCode != "")
            {
                if (responseCode == "000") { mNTransaction.Status = "T"; } else { mNTransaction.Status = "F"; }
                mNTransaction.ResponseCode = responseCode;
                mNTransaction.ResponseDescription = resultDescription;
                UpdateMerchantTransactionController(mNTransaction);  // Update and insert responsecode and description to db 
            }
            return responseCode;
        }
        #endregion


        public string PassDataToMerchantTransactionController(MerchantTransaction userInfo)
        {
            string results = "false";
            try
            {
                int result = CustActivityUtils.InsertMerchantTransaction(userInfo);
                if (result > 0)
                {
                    results = "true";
                }
                else
                {
                    results = "false";
                }

            }
            catch (Exception ex)
            {
                results = "false";
            }


            return results;
        }
        public string UpdateMerchantTransactionController(MerchantTransaction userInfo)
        {
            string results = "false";
            try
            {
                int result = CustActivityUtils.UpdateStatusMerchantTransaction(userInfo);
                if (result > 0)
                {
                    results = "true";
                }
                else
                {
                    results = "false";
                }

            }
            catch (Exception ex)
            {
                results = "false";
            }


            return results;
        }


        #region Test Verify Merchant

        protected async Task<string> VerifyMerchant(MerchantTransaction merchantTransaction)
        {
            MerchantReference.CommonInterfaceSoapClient commonInterfaceSoapClient = new MerchantReference.CommonInterfaceSoapClient();
            MerchantReference.SubmitRequestRequest submitRequest = new MerchantReference.SubmitRequestRequest();
            MerchantReference.HeaderInfo HeaderInfo = new MerchantReference.HeaderInfo();
            MerchantReference.RequestData input = new MerchantReference.RequestData();

            MerchantReference.SubmitRequestResponse submitRequestResponse = new MerchantReference.SubmitRequestResponse();

            //Test server purano data
            //HeaderInfo.H1 = "1";
            //HeaderInfo.H2 = "98FE1564-1E60-4C58-9CB1-5E36E0094407";
            //HeaderInfo.H3 = "D546D65E-D8B9-4Ef8-9842-3C6d522cFCC3";

            HeaderInfo.H1 = "5";
            HeaderInfo.H2 = "5C86A0ED-8542-43D3-8E08-64048F8EF104";
            HeaderInfo.H3 = "36484FA3-AD31-4C25-8617-56BC3205B3A5";

            submitRequest.serviceId = "001";
            submitRequest.serviceCode = "NCELL";

            input.Field1 = "9813999353";
            input.Field2 = "721335032887";
            input.Field3 = "20190801160216";
            input.Field4 = "50";
            input.Field5 = "8800000238";

            submitRequestResponse = await commonInterfaceSoapClient.SubmitRequestAsync(HeaderInfo, submitRequest.serviceId, submitRequest.serviceCode, input);
            string responseCode = submitRequestResponse.SubmitRequestResult.ResultCode;
            string resultDescription = submitRequestResponse.SubmitRequestResult.ResultDescription;
            if (responseCode != "")
            {
                if (responseCode == "200") { merchantTransaction.Status = "T"; } else { merchantTransaction.Status = "F"; }
                merchantTransaction.ResponseCode = responseCode;
                merchantTransaction.ResponseDescription = resultDescription;
                UpdateMerchantTransactionController(merchantTransaction);
            }
            return responseCode;
        }
        #endregion

    }
}
