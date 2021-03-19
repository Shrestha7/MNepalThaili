using MNepalAPI.BasicAuthentication;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                var re = Request;
                var headers = re.Headers;

                if (headers.Contains("token"))
                {
                    //from header
                    string authorizationToken = headers.GetValues("token").First();
                    if (authorizationToken == null || authorizationToken == "")
                        return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");

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
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Invalid Token");
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
