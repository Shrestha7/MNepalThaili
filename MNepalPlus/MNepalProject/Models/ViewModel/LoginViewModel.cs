﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNepalProject.Models.ViewModel
{
    public class LoginViewModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public bool rememberMe { get; set; }
    }
}