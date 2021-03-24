using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using MNepalAPI.Helper;
using Serilog;

namespace MNepalAPI.Controllers
{
    public class DefaultController : ApiController
    {
        //public readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        //public readonly Logger Logger = LogManager.GetLogger("database");
        // GET: Default
        //public ActionResult Index()
        //{
        //    try
        //    {
        //        Logger.Debug("Hi I am NLog Debug Level");
        //        Logger.Info("Hi I am NLog Info Level");
        //        Logger.Warn("Hi I am NLog Warn Level");
        //        throw new NullReferenceException();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex.InnerException, "Hi I am NLog Error Level");
        //        Logger.Fatal(ex.InnerException, "Hi I am NLog Fatal Level");
        //        throw;
        //    }
        //}

        public ActionResult Index()
        {
            try
            {
                HelperStoreSqlLog.WriteDebug(null, "test ");
                HelperStoreSqlLog.WriteWarning(null, "test1 ");
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                Log.Error("Username", "11");
                HelperStoreSqlLog.WriteError(e, "teee");
                HelperStoreSqlLog.WriteFatal(e, "Fateeeeal");
                HelperStoreSqlLog.WriteVerbose(e, "Verboeeeese");

            }
            return null;
        }

    }
}
