using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WCF.MNepal.Models;
using WCF.MNepal.UserModels;

namespace WCF.MNepal.Utilities
{
    public class ErrorUtils
    {
        public static DataTable GetErrorMsgInfo(string errorCode)
        {
            var objUserModel = new ErrorUserModel();
            var objModelInfo = new ErrorInfo
            {
                ErrorCode = errorCode,
                Mode = "EM"
            };
            return objUserModel.GetErrorInformation(objModelInfo);
        }
    }
}