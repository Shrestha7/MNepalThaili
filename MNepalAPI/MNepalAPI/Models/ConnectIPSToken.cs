﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNepalAPI.Models
{
    public class ConnectIPSToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
        public string customer_details { get; set; }
    }


}