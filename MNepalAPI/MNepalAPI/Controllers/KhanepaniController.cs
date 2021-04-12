using MNepalAPI.BasicAuthentication;
using MNepalAPI.Helper;
using MNepalAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MNepalAPI.Controllers
{
    [MyBasicAuthenticationFilter]
    public class KhanepaniController : ApiController
    {
        #region KhanepaniList
        [Route("api/Khanepani/KhanepaniList")]
        [HttpGet]
        public async Task<HttpResponseMessage> KhanepaniList()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var UserName = ConfigurationManager.AppSettings["CIPSAuthUserName"];
                    var UserPassword = ConfigurationManager.AppSettings["CIPSAuthPassword"];
                    var byteArray = Encoding.ASCII.GetBytes(UserName + ":" + UserPassword);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString);
                    string command = "SELECT * FROM MNKhanepaniLocation (NOLOCK) order by KpBranchName ASC";

                    cn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command, cn);
                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    List<Khanepani> neaBranchList = new List<Khanepani>();

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Khanepani khanepaniCounter = new Khanepani();

                        khanepaniCounter.kpID = dr["KpID"].ToString();
                        khanepaniCounter.kpBranchCode = dr["KpBranchCode"].ToString();
                        khanepaniCounter.kpBranchName = dr["KpBranchName"].ToString();
                        neaBranchList.Add(khanepaniCounter);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, neaBranchList);
                }
            }
            catch (Exception ex)
            {
                HelperStoreSqlLog.WriteError(ex, "KhanepaniList");

            }

            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");
        }
        #endregion

        #region NepalWaterList
        [Route("api/NepalWater/NepalWaterList")]
        [HttpGet]
        public async Task<HttpResponseMessage> NepalWaterList()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var UserName = ConfigurationManager.AppSettings["CIPSAuthUserName"];
                    var UserPassword = ConfigurationManager.AppSettings["CIPSAuthPassword"];
                    var byteArray = Encoding.ASCII.GetBytes(UserName + ":" + UserPassword);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString);
                    string command = "SELECT * FROM MNNepalWaterLocation (NOLOCK) order by NwBranchName ASC";

                    cn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command, cn);
                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    List<NepalWater> neaBranchList = new List<NepalWater>();

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        NepalWater nepalWaterCounter = new NepalWater();

                        nepalWaterCounter.nwID = dr["NwID"].ToString();
                        nepalWaterCounter.nwBranchCode = dr["NwBranchCode"].ToString();
                        nepalWaterCounter.nwBranchName = dr["NwBranchName"].ToString();
                        neaBranchList.Add(nepalWaterCounter);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, neaBranchList);
                }
            }
            catch (Exception ex)
            {
                HelperStoreSqlLog.WriteError(ex, "NepalWaterList");

            }

            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");
        }
        #endregion
    }
}
