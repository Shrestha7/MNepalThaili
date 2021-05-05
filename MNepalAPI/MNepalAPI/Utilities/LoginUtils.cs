using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MNepalAPI.UserModel;

namespace MNepalAPI.Utilities
{
    public class LoginUtils
    {
        //start for 3 time wrong pin attempt
        public static int SetPINTries(string userName, string mode)
        {
            var objModel = new LoginUserModels();

            return objModel.SetPINCount(userName, mode);
        }

        #region GetPINBlockTime
        public static bool GetPINBlockTime(string userName)
        {
            var objModel = new LoginUserModels();

            return objModel.GetPINBlockTime(userName) > 0;

        }
        #endregion

        #region GetMessage
        public static string GetMessage(string MsgID)
        {
            var objModel = new LoginUserModels();

            return objModel.GetMessage(MsgID);

        }
        #endregion
    }


}