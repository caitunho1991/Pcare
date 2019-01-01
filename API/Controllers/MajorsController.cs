using API.Models;
using CMS_Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class MajorsController : BaseApiController
    {
        public apiMajorsResponse res = new apiMajorsResponse();
        [HttpGet]
        [Route("API/GetListMajor")]
        public IHttpActionResult GetListMajor()
        {
            try
            {
                return Content(HttpStatusCode.OK, response.Ok(res.ListResponse(),"Lấy danh sách chuyên khoa khám bệnh thành công"));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
            }
        }


        [HttpPost]
        [Route("Test")]
        public IHttpActionResult Test(string deviceToken)
        {
            try
            {
                var a = CMS_Helper.PushNotify(deviceToken, "PCare Title", "PCare Body");
                return Content(HttpStatusCode.OK, response.Ok(null, "Push notify với firebase thành công"));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
            }
        }
    }
}
