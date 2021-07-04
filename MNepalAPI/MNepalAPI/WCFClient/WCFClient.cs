using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace MNepalAPI
{
    public class WCFClient
    {/// <summary>
     /// 
     /// </summary>
     /// <param name="content"></param>
     /// <returns></returns>
        public async Task<string> SendRequest(FormUrlEncodedContent content)
        {
            HttpResponseMessage _res = new HttpResponseMessage();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient())
            {
                var action = "nea.svc/request";
                //var uri = Path.Combine("https://10.129.153.42/WCF.MNepal/", action);
                var uri = Path.Combine("https://localhost:44389/", action);



                _res = await client.PostAsync(new Uri(uri), content);
                string responseBody = _res.StatusCode.ToString() + " ," + await _res.Content.ReadAsStringAsync();
                return responseBody;
            }
        }
    }
}