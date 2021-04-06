using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WCF.MNepal.Models;
using WCF.MNepal.UserModels;

namespace WCF.MNepal.Utilities
{
    public class MerchantUtils
    {
        #region Merchant List

        public static DataTable GetMerchantInfo(string catID)
        {
            var objModel = new MerchantUserModel();
            var objMerchantInfo = new MerchantModel
            {
                CatID = catID,
                Mode = "ML"
            };
            return objModel.GetMerchantListInfo(objMerchantInfo);
        }

        #endregion

        #region Get Date of Original transaction for topup reversal

        public static string GetDate(string RetReference)
        {
            var objModel = new MerchantUserModel();
            var objMerchantInfo = new MerchantModel
            {
                RetReference = RetReference
            };
            return objModel.GetDate(objMerchantInfo);
        }

        #endregion
    }
}