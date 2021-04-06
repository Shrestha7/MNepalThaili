using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WCF.MNepal.Utilities;

namespace WCF.MNepal.Helper
{
    public class SMSEnable
    {
        public string IsSMSEnableCheck(string vid)
        {
            string SMSEnable = "";
            if (!string.IsNullOrEmpty(vid))
            {
                DataTable dtSMSEnable = SMSUtils.GetSMSEnableCheck(vid);
                if (dtSMSEnable != null)
                {
                    int i = 0;
                    if (dtSMSEnable.Rows.Count != 0)
                    {
                        for (i = 0; i < dtSMSEnable.Rows.Count; i++)
                        {
                            SMSEnable = Convert.ToString(dtSMSEnable.Rows[i]["SMSStatus"]);
                        }
                        return SMSEnable;
                    }
                }
                else
                {
                    return SMSEnable;
                }
            }

            return SMSEnable;
        }


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