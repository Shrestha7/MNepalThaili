using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNepalAPI.Models
{
    public class Encrypt
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }

    }

    public class Decrypt
    {
        public string encryptedText { get; set; }
    }

    public class JSONResponse
    {
        public string payload { get; set; }
    }
}