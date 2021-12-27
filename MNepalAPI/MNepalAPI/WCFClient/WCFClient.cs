using System;
using System.Collections.Generic;
using System.Configuration;
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
                var uri = Path.Combine(ConfigurationManager.AppSettings["WCFService"], action);
                _res = await client.PostAsync(new Uri(uri), content);
                string responseBody = await _res.Content.ReadAsStringAsync();
                return responseBody;
            }
        }

        public async Task<string> KUKLSendRequest(FormUrlEncodedContent content)
        {
            HttpResponseMessage _res = new HttpResponseMessage();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient())
            {
                var action = "kukl.svc/executepayment";
                var uri = Path.Combine(ConfigurationManager.AppSettings["WCFService"], action);
                _res = await client.PostAsync(new Uri(uri), content);
                string responseBody = await _res.Content.ReadAsStringAsync();
                return responseBody;
            }
        }
    }
}