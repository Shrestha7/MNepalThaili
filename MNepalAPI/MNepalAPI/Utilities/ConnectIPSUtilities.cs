using MNepalAPI.Models;
using MNepalAPI.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNepalAPI.Utilities
{
    public class ConnectIPSUtilities
    {
        public static int ConnectIPS(ConnectIPSTokenResponse connectIPSToken)
        {
            var objresConnectIPSModel = new ConnectIPSUserModel();
            var objresConnectIPSInfo = new ConnectIPSToken
            {

                access_token = connectIPSToken.access_token,
                token_type = connectIPSToken.token_type,
                refresh_token = connectIPSToken.refresh_token,
                expires_in = connectIPSToken.expires_in,
                scope = connectIPSToken.scope,
                customer_details = connectIPSToken.customer_details

            };
            return objresConnectIPSModel.CIPSTokenInfo(objresConnectIPSInfo);
        }
    }
}