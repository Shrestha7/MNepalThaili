﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MNepalWeb.Controllers
{
    public class RechargeController : Controller
    {
        // GET: Recharge
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NTPrepaid()
        {
            return View();
        }
    }
}