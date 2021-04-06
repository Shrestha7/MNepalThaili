using MNepalProject.Models;
using System.Net;

namespace WCF.MNepal.ErrorMsg
{
    public class ErrorResult
    {
        public string Errorlst(string result, string pin)
        {
            string statusCode = "";
            string message = string.Empty;
            string failedmessage = string.Empty;
            ErrorMessage em = new ErrorMessage();
            MNFundTransfer mnft = new MNFundTransfer();

            if ((result == "Trace ID Repeated") || (result == "Limit Exceed")
                || (result == "Invalid Source User") || (result == "Invalid Destination User")
                || (result == "Invalid Product Request") || (result == "Please try again") || (result == "")
                || (result == "Error in ResponeCode:Data Not Available") 
                || (result == "GatewayTimeout"))
            {
                result = "Sorry for the inconvenience. Service not available temporarily. Please try again later.";
                statusCode = "400";
                message = result;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result); //"parameters missing/invalid"
                failedmessage = message;
            }           
            if (result == "Invalid PIN")
            {
                statusCode = "400";
                message = result + " " + pin;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, "Invalid Pin");
                failedmessage = message; //result + pin;
            }
            if (result == "111")
            {
                statusCode = result;
                message = em.Error_111/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "114")
            {
                statusCode = result;
                message = em.Error_114/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "115")
            {
                statusCode = result;
                message = em.Error_115/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "116")
            {
                statusCode = result;
                message = em.Error_116/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "119")
            {
                statusCode = result;
                message = em.Error_119/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "121")
            {
                statusCode = result;
                message = em.Error_121/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "163")
            {
                statusCode = result;
                message = em.Error_163/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "180")
            {
                statusCode = result;
                message = em.Error_180/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "181")
            {
                statusCode = result;
                message = em.Error_181/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "182")
            {
                statusCode = result;
                message = em.Error_182/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "183")
            {
                statusCode = result;
                message = em.Error_183/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "184")
            {
                statusCode = result;
                message = em.Error_184/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "185")
            {
                statusCode = result;
                message = em.Error_185/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "186")
            {
                statusCode = result;
                message = em.Error_186/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "187")
            {
                statusCode = result;
                message = em.Error_187/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "188")
            {
                statusCode = result;
                message = em.Error_188/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "189")
            {
                statusCode = result;
                message = em.Error_189/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "190")
            {
                statusCode = result;
                message = em.Error_190/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "800")
            {
                statusCode = result;
                message = em.Error_800/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "902")
            {
                statusCode = result;
                message = em.Error_902/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "904")
            {
                statusCode = result;
                message = em.Error_904/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "906")
            {
                statusCode = result;
                message = em.Error_906/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "907")
            {
                statusCode = result;
                message = em.Error_907/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "909")
            {
                statusCode = result;
                message = em.Error_909/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "911")
            {
                statusCode = result;
                message = em.Error_911/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "913")
            {
                statusCode = result;
                message = em.Error_913/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "90")
            {
                statusCode = result;
                message = em.Error_90/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "91")
            {
                statusCode = result;
                message = em.Error_91/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "92")
            {
                statusCode = result;
                message = em.Error_92/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "94")
            {
                statusCode = result;
                message = em.Error_94/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "95")
            {
                statusCode = result;
                message = em.Error_95/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "98")
            {
                statusCode = result;
                message = em.Error_98/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            if (result == "99")
            {
                statusCode = result;
                message = em.Error_99/* + " " + result*/;
                failedmessage = message;
                mnft.ResponseStatus(HttpStatusCode.BadRequest, result);
            }
            
            return failedmessage;

        }
    }
}