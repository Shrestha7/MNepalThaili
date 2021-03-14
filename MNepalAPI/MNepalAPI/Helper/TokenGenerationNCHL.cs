using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace MNepalAPI.Helper
{
    public class TokenGenerationNCHL
    {
        public string getSignature(string concateStr, string publicKeyLocation)
        {
            // concateStr = "MERCHANTID=10,APPID=MER-10-APP-12,APPNAME=Test,TXNID=159690820507240602,TXNDATE=24-07-2018,TXNCRNCY=NPR,TXNAMT=10000,REFERENCEID=159690,REMARKS= Topup-test,PARTICULARS=Topup-Test,TOKEN=TOKEN"
            byte[] buffer = Encoding.UTF8.GetBytes(concateStr);
            HashAlgorithm hash = SHA256.Create();
            byte[] hashValue = hash.ComputeHash(buffer);
            Console.WriteLine("SHA256 hash computed::" + Convert.ToBase64String(hashValue));
            return Convert.ToBase64String(signContent(hashValue, publicKeyLocation));
        }

        public byte[] signContent(byte[] hashValue, string publicKeyLocation)
        {
            string passwordNPI = ConfigurationManager.AppSettings["NPIPassword"];
            try
            {
                RSACng key = new System.Security.Cryptography.RSACng();
                X509Certificate2 publicCert = new X509Certificate2(publicKeyLocation, passwordNPI, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
                X509Certificate2 privateCert = null;
                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);
                foreach (X509Certificate2 cert in store.Certificates)
                {
                    if (cert.GetCertHashString() == publicCert.GetCertHashString())
                        privateCert = cert;
                }

                //RSACng key = new RSACng();
                key = privateCert.GetRSAPrivateKey() as RSACng;

                // key.FromXmlString(privateCert.PrivateKey.ToXmlString(true));
                byte[] signature = key.SignHash(hashValue, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                //byte[] signature = key.SignHash(hashValue, CryptoConfig.MapNameToOID("SHA256"));
                key = (System.Security.Cryptography.RSACng)publicCert.GetRSAPublicKey();
                if (!key.VerifyHash(hashValue, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
                    throw new CryptographicException();
                //Console.WriteLine("Digital Signature Computed::" + Convert.ToBase64String(signature));
                return signature;
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);//todo return msg
                throw;
            }
        }
    }
}