using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class CouponsController : BaseApiController
    {
        public apiCouponsResponse res = new apiCouponsResponse();
        [HttpGet]
        [Route("Coupon/{CouponCode}/{token}")]
        public IHttpActionResult GetCoupon(string CouponCode, string token)
        {
            try
            {
                if (!checkAuth(token))
                {
                    return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
                }
                if (res.CheckCoupon(token, CouponCode))
                {
                    return Content(HttpStatusCode.OK, response.Ok(res.SingleResponse(CouponCode), "Lấy thông tin mã giảm giá thánh công."));
                }
                return Content(HttpStatusCode.BadRequest, response.BadRequest("Mã giảm giá không đúng hoặc đã hết hạn sử dụng. Vui lòng kiểm tra lại"));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
            }
        }
    }
}
