using MNepalAPI.Utilities;
using System;
using System.Data;

namespace MNepalAPI.Helper
{
    public class SMSEnable
    {
        public string GetFinalMessage(string Message, string[] Params)
        {
            int ParamCnt = 1;
            int indx = 0;
            int strtIndex = -1;
            string srchString = "%s";

            while (
                ((indx = Message.IndexOf(srchString, strtIndex + 1)) != -1) &&
                (Params.Length > ParamCnt)
                )
            {
                Message = Message.Remove(indx, srchString.Length);
                Message = Message.Insert(indx, Params[ParamCnt].ToString());
                ParamCnt++;
                strtIndex = indx;
            }

            return Message;
        }


    }
}