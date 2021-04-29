using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNepalAPI.Models
{
    public class ForceUpdate
    {
        public string versionName { get; set; }
        public string versionCode { get; set; }
        public string username { get; set; }
        public string deviceId { get; set; }
        public string firebaseToken { get; set; }
    }
   public class AppUpdateResponse
    {
        public String data { get; set;  }
    }
}