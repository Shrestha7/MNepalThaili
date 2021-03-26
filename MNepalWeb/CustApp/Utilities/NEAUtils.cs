using CustApp.UserModels;
using System.Collections.Generic;

namespace CustApp.Utilities
{
    public class NEAUtils
    {
        #region
        public static Dictionary<string, string> GetNEAName()
        {
            var objNEAModel = new NEAUserModel();

            return objNEAModel.GetNEAName();
        }

        #endregion
    }
}