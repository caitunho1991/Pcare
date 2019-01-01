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
    public class BlogsController : BaseApiController
    {
        public apiBlogsResponse res = new apiBlogsResponse();
        /// <summary>
        /// Về chúng tôi
        /// </summary>
        /// <returns></returns>
        [Route("Blogs/About")]
        [HttpGet]
        public IHttpActionResult About()
        {
            try
            {
                var link = "";
                if (_context.Blogs.Any(x => x.ID == 1) && string.IsNullOrEmpty(_context.Blogs.Single(x => x.ID == 1).Content))
                {
                    link = CMS_Helper.Settings("base_url");
                }
                else
                {
                    link = CMS_Helper.Settings("base_url") + "/Web/Home/About";
                }
                return Content(HttpStatusCode.OK, response.Ok(res.GetLink(link), "Lấy link bài viết thành công."));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
            }
        }
        /// <summary>
        /// Điều khoản điều kiện
        /// </summary>
        /// <returns></returns>
        [Route("Blogs/Terms")]
        [HttpGet]
        public IHttpActionResult Terms()
        {
            try
            {
                var link = CMS_Helper.Settings("base_url") + "/Web/Home/Terms";
                return Content(HttpStatusCode.OK, response.Ok(res.GetLink(link), "Lấy link bài viết thành công."));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
            }
        }
        /// <summary>
        /// Hướng dẫn sử dụng
        /// </summary>
        /// <returns></returns>
        [Route("Blogs/Support")]
        [HttpGet]
        public IHttpActionResult Support()
        {
            try
            {
                var link = CMS_Helper.Settings("base_url") + "/Web/Home/Support";
                return Content(HttpStatusCode.OK, response.Ok(res.GetLink(link), "Lấy link bài viết thành công."));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
            }
        }
    }
}
