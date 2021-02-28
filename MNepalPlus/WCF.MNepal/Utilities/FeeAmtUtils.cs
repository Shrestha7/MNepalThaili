using MNepalProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WCF.MNepal.UserModels;

namespace WCF.MNepal.Utilities
{
    public class FeeAmtUtils
    {
        #region Fee Amount Checker

        public static DataTable GetFeeAmInfo(string retref)
        {
            var objModel = new FeeAmountUserModel();
            var objFeeAmInfo = new MNResponse
            {
                RetrievalRef = retref
            };
            return objModel.GetFeeAmountCheckInfo(objFeeAmInfo);
        }

        #endregion
    }
}