using System.Web.Mvc;

namespace CustApp.Controllers
{
    public class PrivacyPolicyController : Controller
    {
        [HttpGet]
        public ActionResult PrivacyPolicy()
        {

            return View();
        }


        public ActionResult ReportFraud()
        {

            return View();
        }

    }
}