using MNepalAPI.BasicAuthentication;
using MNepalAPI.Connection;
using MNepalAPI.Helper;
using MNepalAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MNepalAPI.Controllers
{
    [MyBasicAuthenticationFilter]
    public class KUKLController : ApiController
    {
        #region
        [Route("api/KUKL/KUKLBillDetails")]
        [HttpPost]
        public async Task<HttpResponseMessage> KUKLBillDetails(KUKLBillDetail details)
        {
            try
            {

                // Serialize our concrete class into a JSON String
                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(details));
                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    var KUKLBaseURL = ConfigurationManager.AppSettings["KUKLBaseURL"];
                    var KUKLPaymentUserName = ConfigurationManager.AppSettings["KUKLAuthorizationUserName"];
                    var KUKLPaymentPassword = ConfigurationManager.AppSettings["KUKLAuthorizationPassword"];
                    var byteArray = Encoding.ASCII.GetBytes(KUKLPaymentUserName + ":" + KUKLPaymentPassword);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; }; //to remove ssl error

                    var httpResponse = await httpClient.PostAsync(KUKLBaseURL + "KUKL/getLatestLedgerDataByConnNum", httpContent);
                    //response
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();

                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var json = JsonConvert.DeserializeObject<KUKLBillDetailResponse>(responseContent);
                        int result = KUKLBillInfo(json);
                        return Request.CreateResponse(HttpStatusCode.OK, json);
                    }
                    return Request.CreateResponse(httpResponse.StatusCode, JsonConvert.DeserializeObject(responseContent));


                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region GetKUKLBranch
        [Route("api/KUKL/GetKUKLBranch")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetKUKLBranch()
        {
            DataTable kuklBranch = KUKLBranch();
            var jsonSerialize = JsonConvert.SerializeObject(kuklBranch);
            var kuklList = JsonConvert.DeserializeObject<List<KUKL>>(jsonSerialize);
            var ascKUKLList = kuklList.OrderBy(x => x.branch);
            return Request.CreateResponse(HttpStatusCode.OK, ascKUKLList);
        }
        #endregion

        #region KUKLBillPayment
        [Route("api/KUKL/KUKLBillPayment")]
        [HttpPost]
        public async Task<HttpResponseMessage> KUKLBillPayment(KUKLBillRequest bill)
        {
            RandomCodeGenerator randomCodeGenerator = new RandomCodeGenerator();
            try
            {
                var kuklObject = new KUKLBillRequest
                {
                    username = bill.username,
                    connectionNo = bill.connectionNo,
                    merchantId = ConfigurationManager.AppSettings["KUKLMerchantId"],
                    txnReferenceNo = randomCodeGenerator.CreateRandomCodeWithString(30),
                    txnAmount = bill.txnAmount,
                    bankId = ConfigurationManager.AppSettings["KUKLBankId"],
                    txnDate = DateTime.Now.ToString("yyyy-MM-dd"),
                    branchcode = bill.branchcode,
                    module = bill.module
                };


                //request
                int request = RequestKUKLInfo(kuklObject);

                // Serialize our concrete class into a JSON String
                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(kuklObject));
                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    var KUKLBaseURL = ConfigurationManager.AppSettings["KUKLBaseURL"];
                    var KUKLPaymentUserName = ConfigurationManager.AppSettings["KUKLPaymentUserName"];
                    var KUKLPaymentPassword = ConfigurationManager.AppSettings["KUKLPaymentPassword"];
                    var byteArray = Encoding.ASCII.GetBytes(KUKLPaymentUserName + ":" + KUKLPaymentPassword);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; }; //to remove ssl error

                    var httpResponse = await httpClient.PostAsync(KUKLBaseURL + "KUKL/PostOnlinePaymentDataByConnNum", httpContent);
                    //response
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();

                    if (httpResponse.StatusCode == HttpStatusCode.OK && responseContent!="{}")
                    {
                        var json = JsonConvert.DeserializeObject<KUKLPaymentTxnResponse>(responseContent);
                        int result = ResponseKUKLInfo(json);

                        //SMS
                        string messagereply = "";
                        try
                        {
                            //FOR CUSTOMER
                            try
                            {
                                //Alert Dynamic
                                string AlertType = "KUKL";

                                //FOR CUSTOMER SMS                                     
                                #region FOR CUSTOMER SMS

                                CustomerSMS customerSMS = new CustomerSMS();
                                string cSMS = customerSMS.CustSMSEnable(AlertType, kuklObject.username.Trim(), "", kuklObject.txnAmount.ToString(), "", "", kuklObject.txnDate.ToString());
                                if (cSMS == "false")
                                {

                                }
                                else
                                {

                                }

                                #endregion

                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }

                        }
                        catch (Exception ex)
                        {
                            // throw ex
                            string statusCode = "400";
                            string message = ex.Message;
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, json);
                    }
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Details");


                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region
        [Route("api/KUKL/KUKLPaymentTxnStatus")]
        [HttpPost]
        public async Task<HttpResponseMessage> KUKLPaymentTxnStatus(KUKLPaymentTxnRequest txnStatus)
        {
            try
            {
                var kuklTxn = new KUKLPaymentTxnRequest
                {
                    txnReferenceNo = txnStatus.txnReferenceNo,
                    branchcode = txnStatus.branchcode,
                    module = txnStatus.module
                };
                // Serialize our concrete class into a JSON String
                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(kuklTxn));
                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                using (var httpClient = new HttpClient())
                {
                    var KUKLBaseURL = ConfigurationManager.AppSettings["KUKLBaseURL"];
                    var KUKLAuthorizationUserName = ConfigurationManager.AppSettings["KUKLAuthorizationUserName"];
                    var KUKLAuthorizationPassword = ConfigurationManager.AppSettings["KUKLAuthorizationPassword"];
                    var byteArray = Encoding.ASCII.GetBytes(KUKLAuthorizationUserName + ":" + KUKLAuthorizationPassword);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; }; //to remove ssl error

                    var httpResponse = await httpClient.PostAsync(KUKLBaseURL + "KUKL/PaymentTxStatus", httpContent);
                    //response
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();

                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var json = JsonConvert.DeserializeObject(responseContent);
                        return Request.CreateResponse(HttpStatusCode.OK, json);
                    }
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid Details");


                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region KUKLBranch
        public DataTable KUKLBranch()
        {
            DataTable dtableResult = null;
            SqlConnection conn = null;
            try
            {
                using (conn = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("[s_MNKUKLBranch]", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            // set the CommandTimeout
                            da.SelectCommand.CommandTimeout = 60;  // seconds
                            using (DataSet dataset = new DataSet())
                            {
                                da.Fill(dataset);
                                if (dataset.Tables.Count > 0)
                                {
                                    dtableResult = dataset.Tables[0];
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

            return dtableResult;
        }
        #endregion

        //KUKL Bill Details save to db
        #region KUKLBillDetail
        public int KUKLBillInfo(KUKLBillDetailResponse objreqKUKL)
        {
            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNKUKLBillDetails]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@address", objreqKUKL.address= (objreqKUKL.address==null?"":objreqKUKL.address));
                        sqlCmd.Parameters.AddWithValue("@penalty", objreqKUKL.penalty);
                        sqlCmd.Parameters.AddWithValue("@name", objreqKUKL.name);
                        sqlCmd.Parameters.AddWithValue("@connectionNo", objreqKUKL.connection_no);
                        sqlCmd.Parameters.AddWithValue("@netAmount", objreqKUKL.net_amount);
                        

                        ret = sqlCmd.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (sqlCon != null)
                {
                    sqlCon.Close();
                }
            }
            return ret;
        }
        #endregion

        //Request save to db
        #region KUKLRequestData
        public int RequestKUKLInfo(KUKLBillRequest objreqKUKL)
        {
            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNKUKLRequest]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@username", objreqKUKL.username);
                        sqlCmd.Parameters.AddWithValue("@connectionNo", objreqKUKL.connectionNo);
                        sqlCmd.Parameters.AddWithValue("@merchantId", objreqKUKL.merchantId.ToString());
                        sqlCmd.Parameters.AddWithValue("@txnReferenceNo", objreqKUKL.txnReferenceNo);
                        sqlCmd.Parameters.AddWithValue("@txnAmount", objreqKUKL.txnAmount);
                        sqlCmd.Parameters.AddWithValue("@bankId", objreqKUKL.bankId);
                        sqlCmd.Parameters.AddWithValue("@txnDate", objreqKUKL.txnDate);
                        sqlCmd.Parameters.AddWithValue("@branchcode", objreqKUKL.branchcode);
                        sqlCmd.Parameters.AddWithValue("@module", objreqKUKL.module);

                        ret = sqlCmd.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (sqlCon != null)
                {
                    sqlCon.Close();
                }
            }
            return ret;
        }
        #endregion

        //Response save to db
        #region KUKLResponseData
        public int ResponseKUKLInfo(KUKLPaymentTxnResponse objresKUKL)
        {
            SqlConnection sqlCon = null;
            int ret;
            try
            {
                using (sqlCon = new SqlConnection(DatabaseConnection.ConnectionString()))
                {
                    sqlCon.Open();
                    using (SqlCommand sqlCmd = new SqlCommand("[s_MNKUKLResponse]", sqlCon))
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;

                        sqlCmd.Parameters.AddWithValue("@result", objresKUKL.result);
                        sqlCmd.Parameters.AddWithValue("@amount", objresKUKL.amount);
                        sqlCmd.Parameters.AddWithValue("@txnReferenceNo", objresKUKL.txnReferenceNo);
                        sqlCmd.Parameters.AddWithValue("@recNo", objresKUKL.recNo);
                        sqlCmd.Parameters.AddWithValue("@connectionNo", objresKUKL.connectionNo);
                        sqlCmd.Parameters.AddWithValue("@recdate", objresKUKL.recdate);

                        ret = sqlCmd.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (sqlCon != null)
                {
                    sqlCon.Close();
                }
            }
            return ret;
        }
        #endregion



        


    }
}