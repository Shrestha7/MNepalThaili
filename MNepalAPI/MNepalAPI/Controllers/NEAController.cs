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
    public class NEAController : ApiController
    {
        #region NEACounterList
        [Route("api/NEA/NEACounterList")]
        [HttpGet]
        public async Task<HttpResponseMessage> NEACounterList()
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
                    string command = "SELECT* FROM MNNEALocation(NOLOCK) order by NEABranchName ASC";

                    cn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command, cn);
                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    List<NEA> neaBranchList = new List<NEA>();

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        NEA neaCounter = new NEA();

                        neaCounter.neaID = dr["NeaID"].ToString();
                        neaCounter.neaBranchCode = dr["NEABranchCode"].ToString();
                        neaCounter.neaBranchName = dr["NEABranchName"].ToString();
                        neaBranchList.Add(neaCounter);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, neaBranchList);
                }
            }
            catch (Exception ex)
            {
                HelperStoreSqlLog.WriteError(ex, "NEACounterList");
            }
            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");
        }
        #endregion
    }
}
