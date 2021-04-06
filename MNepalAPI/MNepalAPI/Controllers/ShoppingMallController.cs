using MNepalAPI.BasicAuthentication;
using MNepalAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MNepalAPI.Controllers
{
    [MyBasicAuthenticationFilter]
    public class ShoppingMallController : ApiController
    {
        #region ShoppingMallRequest
        [Route("api/ShoppingMall/ShoppingMallRequest")]
        [HttpPost]
        public async Task<HttpResponseMessage> ShoppingMallRequest(ShoppingMall shoppingMall)
        {
            var merchantId = shoppingMall.MerchantId;
            var itemCode = shoppingMall.ItemCode;
            var price = shoppingMall.Price;

            ShoppingMall shopping = new ShoppingMall();
            shopping.MerchantId = merchantId;
            shopping.ItemCode = itemCode;
            shopping.Price = price;

            return Request.CreateResponse(HttpStatusCode.OK, shopping);
        }
        #endregion
    }
}
