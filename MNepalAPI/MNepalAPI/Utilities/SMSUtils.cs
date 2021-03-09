using MNepalAPI.Models;
using MNepalAPI.UserModels;
using System.Data;

namespace MNepalAPI.Utilities
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

        


        

    }
}