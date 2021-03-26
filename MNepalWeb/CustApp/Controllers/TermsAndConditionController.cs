using System.Web.Mvc;

namespace CustApp.Controllers
{
    public class TermsAndConditionController : Controller
    {
        // GET: TermsAndCondition
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}