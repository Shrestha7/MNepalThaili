﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThailiMerchantApp.Models
{
    public class JsonParse
    {
        public string d { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string PP { get; set; }
        public string front { get; set; }
        public string back { get; set; }
        public Exception Exception { get; set; }
       
    }

    
}