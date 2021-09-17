using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WCF.MNepal.Models;
using WCF.MNepal.UserModels;

namespace WCF.MNepal.Utilities
{
    public class SMSUtils
    {
        public static DataTable GetSMSAlert(string alertType)
        {
            var objModel = new SMSUserModels();
            var objUserInfo = new SMSMsg
            {
                AlertType = alertType,
                Mode = "GM" //Get Msg
            };
            return objModel.GetSMSInformation(objUserInfo);
        }

        public static DataTable GetSMSEnableCheck(string mid)
        {
            var objModel = new SMSUserModels();
            var objUserInfo = new SMSMsg
            {
                MID = mid,
                Mode = "MLE" //Get SMS Enable Check
            };
            return objModel.GetSMSEnableCheck(objUserInfo);
        }


        public static DataTable GetEmailEnableCheck(string mid)
        {
            var objModel = new SMSUserModels();
            var objUserInfo = new SMSMsg
            {
                MID = mid,
                Mode = "EMLE" //Get Email Enable Check
            };
            return objModel.GetSMSEnableCheck(objUserInfo);
        }
    }
}