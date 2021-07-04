using MNepalAPI.Models;
using MNepalAPI.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MNepalAPI.Utilities
{
    public class NEAUtilities
    {
        public static int NEARequest(NEABranch nea)
        {
            var objresNEAModel = new NEAUserModel();
            var objresCIPSInfo = new NEABranch
            {
                serviceId = nea.serviceId,
                serviceCode = nea.serviceCode,
                field1 = nea.field1,
                field2 = nea.field2,
                field3 = nea.field3,
                field4 =nea.field4,
                field5 = nea.field5,
                userName = nea.userName,
                retrivalReference = nea.retrivalReference,
                additionalData = nea.additionalData
                
            };
            return objresNEAModel.NEARequestInfo(objresCIPSInfo);
        }
    }
}