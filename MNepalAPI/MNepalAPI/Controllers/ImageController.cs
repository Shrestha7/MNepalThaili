using MNepalAPI.BasicAuthentication;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
    public class ImageController : ApiController
    {
        #region SliderImage
        [Route("api/Image/SliderImage")]
        [HttpGet]
       
        public async Task<HttpResponseMessage> SliderImage()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var UserName = ConfigurationManager.AppSettings["CIPSAuthUserName"];
                    var UserPassword = ConfigurationManager.AppSettings["CIPSAuthPassword"];
                    var byteArray = Encoding.ASCII.GetBytes(UserName + ":" + UserPassword);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    string dirPath = System.Web.Hosting.HostingEnvironment.MapPath(ConfigurationManager.AppSettings["SliderImageSource"]);
                    string imageSourcePath = ConfigurationManager.AppSettings["SliderPath"];
                    List<string> files = new List<string>();
                    DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
                    foreach (FileInfo fInfo in dirInfo.GetFiles())
                    {
                        files.Add(imageSourcePath + fInfo.Name);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, files);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

    }
}
