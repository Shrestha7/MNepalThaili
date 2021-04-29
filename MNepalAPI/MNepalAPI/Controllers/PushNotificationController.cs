using FirebaseAdmin.Messaging;
using MNepalAPI.BasicAuthentication;
using MNepalAPI.Helper;
using MNepalAPI.Utilities;
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
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static MNepalAPI.Models.Notifications;
using Message = MNepalAPI.Models.Notifications.Message;

namespace MNepalAPI.Controllers
{
    [MyBasicAuthenticationFilter]
    public class PushNotificationController : ApiController
    {
        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString);

        [Route("api/PushNotificatin/PushNotification")]
        [HttpPost]

        /// get specific data
      //  Select* from MNNotifications FULL OUTER JOIN  MNNotification_FirebaseToken ON MNNotifications.MobileNo =  MNNotification_FirebaseToken.MobileNo
        public async Task<HttpResponseMessage> PushNotification([FromBody] Notificationsobject notifications)
        {
            try
            {
                var notificationsobject = new Notificationsobject
                {
                    to = notifications.to,
                    data = new Data
                    {
                        extra_information = notifications.data.extra_information,
                        redirectUrl = notifications.data.redirectUrl
                    },
                    notification = new Models.Notifications.Notification
                    {
                        title = notifications.notification.title,
                        text = notifications.notification.text,
                        click_action = "OPEN_ACTIVITY_1"
                    }
                };

                // Serialize our concrete class into a JSON String
                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(notificationsobject));

                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    var AuthorizationKey = ConfigurationManager.AppSettings["AuthorizationKey"];
                    var AuthorizationKeyValue = ConfigurationManager.AppSettings["AuthorizationKeyValue"];
                    var NotificationPostUrl = ConfigurationManager.AppSettings["NotificationPostUrl"];
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationKey, AuthorizationKeyValue);

                    // Do the actual request and await the response
                    var httpResponse = await httpClient.PostAsync(NotificationPostUrl, httpContent);
                    var a = httpResponse;
                    var b = a;

                    // If the response contains content we want to read it!
                    if (httpResponse.Content != null)
                    {
                        var responseContent = await httpResponse.Content.ReadAsStringAsync();

                        //Deserialize Object to get object value
                        var result = JsonConvert.DeserializeObject<Response>(responseContent);
                        var message_Id = result.message_id;

                        NotificationModel notification = new NotificationModel();
                        notification.title = notifications.notification.title;
                        notification.text = notifications.notification.text;
                        notification.imageName = notifications.data.extra_information;
                        notification.redirectUrl = notifications.data.redirectUrl;
                        notification.pushNotificationDate = DateTime.Now;
                        notification.messageId = message_Id;

                        //Database
                        int resultsPayments = NotificationUtilities.Notification(notification);
                        if (resultsPayments == -1)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, new { notification });
                        }
                        //return Request.CreateResponse(HttpStatusCode.OK,responseContent);
                        // From here on you could deserialize the ResponseContent back again to a concrete C# type using Json.Net
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                HelperStoreSqlLog.WriteError(ex, "PushNotification");
            }
            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Unauthorized");

        }


        [HttpPost]
        [Route("api/PushNotificatin/SendNotification")]
        public async Task<HttpResponseMessage> SendNotification([FromBody] NotificationsobjectRegistration notifications)
        {
            try
            {
                // Create a list containing up to 500 registration tokens.
                // These registration tokens come from the client FCM SDKs.

                int registrationLength = notifications.to.Count;
                int registrationLengthRemainder = registrationLength % 500;
                registrationLength -= registrationLengthRemainder;
                List<string> registrationTokens = new List<string>();
                if (registrationLength >= 500)
                { 
                    for (int k = 0; k < 500; k++)
                    {
                        registrationTokens.Add(notifications.to[k]);
                    }
                
                    for (int i = 0; i < registrationLength; i += 500)
                    {
                       
                        var response = NotificationFunction(notifications, i, 500,registrationTokens);
                        return await response;
                    }
                }
                else
                {
                    if (registrationLengthRemainder >= 0)
                    {
                        for (int k = 0; k < registrationLengthRemainder; k++)
                        {
                            registrationTokens.Add(notifications.to[k]);
                        }
                        var response = NotificationFunction(notifications, registrationLengthRemainder, registrationLengthRemainder,registrationTokens);
                        return await response;
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
              
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        private async Task<HttpResponseMessage> NotificationFunction(NotificationsobjectRegistration notifications,int i, int j,List<string> registrationTokens)
        {
            string registrationTokenString = string.Join(",", registrationTokens.ToArray());
            try
            {
                var messageInformation = new Message()
                {
                    to = registrationTokenString,
                    data = new Data
                    {
                        extra_information = notifications.data.extra_information,
                        redirectUrl = notifications.data.redirectUrl
                    },
                    notification = new Models.Notifications.Notification
                    {
                        title = notifications.notification.title,
                        text = notifications.notification.text,
                        click_action = "OPEN_ACTIVITY_1"
                    }
                };

                var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(messageInformation));

                // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    var AuthorizationKey = ConfigurationManager.AppSettings["AuthorizationKey"];
                    var AuthorizationKeyValue = ConfigurationManager.AppSettings["AuthorizationKeyValue"];
                    var NotificationPostUrl = ConfigurationManager.AppSettings["NotificationPostUrl"];
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationKey, AuthorizationKeyValue);

                    // Do the actual request and await the response
                    var httpResponse = await httpClient.PostAsync(NotificationPostUrl, httpContent);
                    var a = httpResponse;
                    var b = a;

                    // If the response contains content we want to read it!
                    if (httpResponse.Content != null)
                    {
                        var responseContent = await httpResponse.Content.ReadAsStringAsync();

                        //Deserialize Object to get object value
                        var result = JsonConvert.DeserializeObject<Response>(responseContent);
                        var message_Id = result.message_id;

                        NotificationModel notification = new NotificationModel();
                        notification.title = notifications.notification.title;
                        notification.text = notifications.notification.text;
                        notification.imageName = notifications.data.extra_information;
                        notification.redirectUrl = notifications.data.redirectUrl;
                        notification.pushNotificationDate = DateTime.Now;
                        notification.messageId = "65jhvjhvhgvghvhgvhgvghvgh";

                        //Database
                          int resultsPayments = NotificationUtilities.Notification(notification);
                        if (resultsPayments == -1)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, new { notification });
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.InternalServerError);
                        }
                        //return Request.CreateResponse(HttpStatusCode.OK,responseContent);
                        // From here on you could deserialize the ResponseContent back again to a concrete C# type using Json.Net
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.InternalServerError);
                    }
                }
             
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        [Route("api/PushNotificatin/GetAllNotification")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllNotification(string mobileNumber)
        {
            try
            {
                SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString);
                string command = "select * from MNNotifications order by Id desc";
                string createdDate = "SELECT CreatedDate from MNClient WHERE MobileNo = '9851020495'";

                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(command, cn);
                SqlDataAdapter createdDateData = new SqlDataAdapter(createdDate, cn);
                DataSet ds = new DataSet();
                DataSet createdDataSet = new DataSet();

             
                da.Fill(ds);
                createdDateData.Fill(createdDataSet);
                List<NotificationModel> notificationsList = new List<NotificationModel>();
                string createdDateString = "";
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    NotificationModel notifications = new NotificationModel();

                    foreach (DataRow drrr in createdDataSet.Tables[0].Rows)
                    {
                        createdDateString = drrr["CreatedDate"].ToString();
                    }
                    if ((DateTime.Compare(Convert.ToDateTime(createdDateString), Convert.ToDateTime(dr["NotificationDate"]))) < 0)
                    {
                        notifications.title = dr["Title"].ToString();
                        notifications.text = dr["Text"].ToString();
                        notifications.imageName = dr["ImageName"].ToString();
                        notifications.redirectUrl = dr["RedirectUrl"].ToString();
                        notifications.pushNotificationDate = Convert.ToDateTime(dr["NotificationDate"]);
                        notifications.messageId = dr["MessageId"].ToString();
                        notificationsList.Add(notifications);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { notificationsList });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        //[Route("api/PushNotification/NotificationImage")]
        //[HttpPost]
        //public async Task<HttpResponseMessage> NotificationImage()
        //{
        //    Dictionary<string, object> dict = new Dictionary<string, object>();
        //    try
        //    {

        //        var httpRequest = HttpContext.Current.Request;
        //        var filePath="";

        //        foreach (string file in httpRequest.Files)
        //        {
        //            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

        //            var postedFile = httpRequest.Files[file];
        //            if (postedFile != null && postedFile.ContentLength > 0)
        //            {

        //                int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB

        //                IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
        //                var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
        //                var extension = ext.ToLower();
        //                if (!AllowedFileExtensions.Contains(extension))
        //                {

        //                    var message = string.Format("Please Upload image of type .jpg,.gif,.png.");

        //                    dict.Add("error", message);
        //                    return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
        //                }
        //                else if (postedFile.ContentLength > MaxContentLength)
        //                {

        //                    var message = string.Format("Please Upload a file upto 1 mb.");

        //                    dict.Add("error", message);
        //                    return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
        //                }
        //                else
        //                {
        //                    Guid guid = Guid.NewGuid();
        //                    string imageurl = guid + extension;
        //                    //  where you want to attach your imageurl

        //                    //if needed write the code to update the table

        //                    filePath = HttpContext.Current.Server.MapPath("~/NotificationImage/" + imageurl);
        //                    //Userimage myfolder name where i want to save my image
        //                    postedFile.SaveAs(filePath);

        //                }
        //            }

        //            var imageUrl = filePath;
        //            return Request.CreateErrorResponse(HttpStatusCode.Created, imageUrl); ;
        //        }
        //        var res = string.Format("Please Upload a image.");
        //        dict.Add("error", res);
        //        return Request.CreateResponse(HttpStatusCode.NotFound, dict);
        //    }
        //    catch (Exception ex)
        //    {
        //        var res = string.Format("some Message");
        //        dict.Add("error", res);
        //        return Request.CreateResponse(HttpStatusCode.NotFound, dict);
        //    }
        //}
    }
}
