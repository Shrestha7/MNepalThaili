﻿using System.Web.Mvc;

namespace CustApp.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Error404(string error)
        {
            return View();
        }

        public ActionResult Error500()
        {
            return View();
        }
    }
}