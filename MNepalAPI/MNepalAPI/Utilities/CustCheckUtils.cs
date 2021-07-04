using MNepalAPI.UserModel;
using MNepalProject.Models;
using System;
using System.Data;

namespace MNepalAPI.Utilities
{
    public class CustCheckUtils
    {    

        public static string GetName(string MobileNumber)
        {
            string name = "Customer";
            try
            {
                DataTable dataTable = GetCustInfo(MobileNumber);
                name = dataTable.Rows[0]["FName"].ToString();// for FULL FIRST NAME
                if (name == "" || name == null)
                {
                    name = "Customer";
                }
            }
            catch (Exception e)
            {
                return name;
            }

            return name;
        }

        #region Customer Details

        public static DataTable GetCustInfo(string cmobile)
        {
            var objModel = new CustCheckerUserModel();
            return objModel.GetCustUserInfo(cmobile);
        }

        #endregion

        public static DataTable GetCustUserInfo(string cmobile)
        {
            var objModel = new CustCheckerUserModel();
            var objCustUserInfo = new MNClientExt
            {
                UserName = cmobile
            };
            return objModel.GetCustUserCheckInfo(objCustUserInfo);
        }

        #region Merchant Chercker
        public static bool GetMerchantUserCheckInfo(string cmobile)
        {
            var objModel = new CustCheckerUserModel();
            var objCustUserInfo = new MNClientExt
            {
                UserName = cmobile
            };
            return objModel.GetMerchantUserCheckInfo(objCustUserInfo).Rows.Count > 0;
        }
        #endregion

    }
}