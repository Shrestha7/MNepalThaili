using MNepalAPI.Models;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace MNepalAPI.Controllers
{
    /* [MyBasicAuthenticationFilter]
    */
    public class UpdateAppController : ApiController
    {
        const string tableName = "MNAppInfoTable";
        [HttpPost]
        public async Task<HttpResponseMessage> checkForUpdate([FromBody] ForceUpdate forceUpdate)
        {
            try
            {
                ForceUpdate requestData = new ForceUpdate
                {
                    username = forceUpdate.username,
                    versionName = forceUpdate.versionName,
                    versionCode = forceUpdate.versionCode,
                    deviceId = forceUpdate.deviceId,
                    firebaseToken = forceUpdate.firebaseToken

                };

                Regex usernameRegex = new Regex(@"^9\d{9,15}$");
                Regex versionNameRegex = new Regex(@"^\d.{4,10}$");
                Regex versionCodeRegex = new Regex(@"^\d{2,3}$");
                Regex updateAppRegex = new Regex(@"^\d{1}$");
                Regex deviceIdRegex = new Regex(@"^[A-Za-z0-9]{16}");
                Regex firebaseTokenRegex = new Regex(@"([A-Za-z0-9_:]{100,255})");

                if (!usernameRegex.IsMatch(requestData.username))
                {
                    AppUpdateResponse appUpdateResponse = new AppUpdateResponse();
                    appUpdateResponse.data = "Enter valid username";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { appUpdateResponse });

                }
                else if (!versionCodeRegex.IsMatch(requestData.versionCode.ToString()))
                {

                    AppUpdateResponse appUpdateResponse = new AppUpdateResponse();
                    appUpdateResponse.data = "Enter valid version";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { appUpdateResponse });

                }
                else if (!deviceIdRegex.IsMatch(requestData.deviceId.ToString()))
                {
                    AppUpdateResponse appUpdateResponse = new AppUpdateResponse();
                    appUpdateResponse.data = "Enter valid device id";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { appUpdateResponse });

                }
                else if (!firebaseTokenRegex.IsMatch(requestData.firebaseToken.ToString()))
                {
                    AppUpdateResponse appUpdateResponse = new AppUpdateResponse();
                    appUpdateResponse.data = "Enter valid firebase Token";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { appUpdateResponse });

                }
                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(forceUpdate));

                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString);
                string checkTable = "SELECT COUNT(1) FROM " +tableName +" WHERE USERNAME = "+ requestData.username;
               
                string command = "SELECT * from " + tableName + " WHERE Mobile_No = '" + requestData.username +"'";
            
               
                cn.Open();

                SqlDataAdapter selectExecution = new SqlDataAdapter(command, cn);
                //SqlDataAdapter checkTableExecution  = new SqlDataAdapter(checkTable, cn);
             

              //  DataSet ds1 = new DataSet();
            
             DataSet dataSet = new DataSet();
                selectExecution.Fill(dataSet);
                var length = dataSet.Tables[0].Rows.Count;
                if (length < 1)
                {
                    string commandUpdateTable = "INSERT INTO " + tableName + " values (  '" + requestData.username + "' , " + 0 + " , '" + requestData.firebaseToken + "' , '" + requestData.versionCode + "' , '" + requestData.versionName + "' , '" + requestData.deviceId + "');";
                    SqlDataAdapter insertExecution = new SqlDataAdapter();
                    insertExecution.InsertCommand = new SqlCommand(commandUpdateTable, cn);
                    insertExecution.InsertCommand.ExecuteNonQuery();

                    return Request.CreateResponse(HttpStatusCode.Created);
                }
                else
                {
            
                     DataRow dataRow = dataSet.Tables[0].Rows[0];
                     bool updateStatus = Convert.ToBoolean(dataRow["Update_App"]);
                     ForceUpdate databaseData = new ForceUpdate();
                     
                        string commandUpdateTable = "UPDATE " + tableName + " SET Version_Code = '" + requestData.versionCode + "' , Version_Name = '" + requestData.versionName + "' , Firebase_Token = '" + requestData.firebaseToken + "' , Device_Id = '" + requestData.deviceId + "' WHERE Mobile_No = '" + requestData.username + "' ;";
                        SqlDataAdapter updaeExecution = new SqlDataAdapter(commandUpdateTable, cn);
                        updaeExecution.UpdateCommand = new SqlCommand(commandUpdateTable, cn);
                        updaeExecution.UpdateCommand.ExecuteNonQuery();
                       //if update staus is true and both the app version name and app version code are upto date then do not update
                    if (updateStatus && (ConfigurationManager.AppSettings["LatestAppVersion"] == requestData.versionName 
                            || ConfigurationManager.AppSettings["LatestAppVersrsionCode"] == requestData.versionCode)){
                       ////donot update
                        return Request.CreateResponse(HttpStatusCode.Created,"No Updates");
                    }
                    // sice app version name or app version code are not upto date and update is true so update application.
                    else if(updateStatus)
                    {
                        //update
                        return Request.CreateResponse(HttpStatusCode.OK,"Update Application");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NoContent,"Do not update application");
                    }
                }
             
            }
            catch (Exception e)
            {
                Debug.WriteLine("Could Not Verify Object" + e);
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

    }
}
/*{
    "version": "1.0.2",
  "username": "9813828185"
}*/