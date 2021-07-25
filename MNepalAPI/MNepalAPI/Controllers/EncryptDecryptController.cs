using MNepalAPI.BasicAuthentication;
using MNepalAPI.Helper;
using MNepalAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Serialization;

namespace MNepalAPI.Controllers
{
    [MyBasicAuthenticationFilter]
    public class EncryptDecryptController : ApiController
    {
        #region Encrypt
        [Route("api/EncryptDecrypt/EncryptData")]
        [HttpPost]
        public async Task<HttpResponseMessage> EncryptData(Encrypt encryptDecrypt)
        {
            var data = new Encrypt
            {
                id = encryptDecrypt.id,
                firstName = encryptDecrypt.firstName,
                lastName = encryptDecrypt.lastName
            };

            var json = JsonConvert.SerializeObject(data);

            RsaEncryption rsa = new RsaEncryption();
            var encryptData = rsa.Encrypt(json);

            JSONResponse response = new JSONResponse();
            response.payload = encryptData;
            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
        #endregion

        #region Decrypt
        [Route("api/EncryptDecrypt/DecryptData")]
        [HttpPost]
        public async Task<HttpResponseMessage> DecryptData(Decrypt encryptDecrypt)
        {
            RsaEncryption rsa = new RsaEncryption();
            var data = rsa.Decrypt(encryptDecrypt.encryptedText);
            return Request.CreateResponse(HttpStatusCode.OK, JsonConvert.DeserializeObject(data));
        }
        #endregion



    }
}
