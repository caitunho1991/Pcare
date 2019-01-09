using API.Models;
using CMS_Lib;
using CMS_Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class UsersController : BaseApiController
    {
        #region Service

        public VM_User_Response res = new VM_User_Response();
        /// <summary>
        /// Lấy số dư hiện tại của bác sỹ
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("GetBalance/{token}")]
        [HttpGet]
        public IHttpActionResult GetBalance(string token)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Content(HttpStatusCode.BadRequest, response.BadRequest("Tên đăng nhập hoặc mật khẩu không đúng. Vui lòng kiểm tra lại."));
                }
                if (!checkAuth(token))
                {
                    return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
                }
                return Content(HttpStatusCode.OK, response.Ok(res.SingleResponse(token), "Lấy số dư tài khoản thành công."));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
            }
        }

        /// <summary>
        /// Đăng nhập tài khoản
        /// </summary>
        /// <param name="req">
        /// 1. Username => Phone / Email
        /// 2. Password
        /// 3. Lat => require if it's a account doctor
        /// 4. Lng => require if it's a account doctor
        /// 5.IsDoctor = true => account doctor
        /// </param>
        /// <returns></returns>
        [Route("Accounts/Login")]
        [HttpPost]
        public IHttpActionResult Login(VM_User_Login req)
        {
            try
            {
                if (!string.IsNullOrEmpty(req.TokenFacebook))
                {
                    ModelState["req.Username"].Errors.Clear();
                    ModelState["req.Password"].Errors.Clear();
                }
                if (!ModelState.IsValid)
                {
                    return Content(HttpStatusCode.BadRequest, response.BadRequest(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
                }
                var user = req.Login();
                if (!string.IsNullOrEmpty(user))
                {
                    return Content(HttpStatusCode.OK, response.Ok(res.SingleResponse(user), "Đăng nhập thành công."));
                }
                return Content(HttpStatusCode.BadRequest, response.BadRequest("Tên đăng nhập hoặc mật khẩu không đúng. Vui lòng kiểm tra lại."));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý"));
            }
        }

        /// <summary>
        /// Đăng ký tài khoản
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("Accounts/Register")]
        [HttpPost]
        public IHttpActionResult Register(VM_User_Register req)
        {
            try
            {
                if (!string.IsNullOrEmpty(req.TokenFacebook))
                {
                    ModelState["req.Email"].Errors.Clear();
                    ModelState["req.PhoneNumber"].Errors.Clear();
                    ModelState["req.Password"].Errors.Clear();
                }
                if (!ModelState.IsValid)
                {
                    return Ok(response.BadRequest(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
                }
                var UserGUID = req.Register();
                if (!string.IsNullOrEmpty(UserGUID))
                {
                    return Content(HttpStatusCode.OK, response.Ok(res.SingleResponse(UserGUID), ""));
                }
                return Content(HttpStatusCode.OK, response.Ok(null, "Tài khoản đã tồn tại trong hệ thống hoặc dữ liệu không đúng. Vui lòng kiểm tra lại.", false));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadGateway, response.BadRequest("Có lỗi trong quá trình xử lý"));
            }
        }

        /// <summary>
        /// Đăng xuất tài khoản
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Accounts/Logout/{token}")]
        public IHttpActionResult LogOut(string token)
        {
            try
            {
                if (!checkAuth(token))
                {
                    return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
                }
                var acc = _context.Accounts.SingleOrDefault(x => x.TokenLogin.Equals(token));
                if (acc != null)
                {
                    acc.TokenLogin = "";
                    acc.ExpireTokenLogin = DateTime.Now;
                    _context.SaveChanges();
                    return Content(HttpStatusCode.OK, response.Ok(null, "Đăng xuất tài khoản thành công."));
                }
                return Content(HttpStatusCode.OK, response.Ok(null, "Tài khoản của bạn không tồn tại.", false));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
            }
        }

        /// <summary>
        /// Cập nhật thông tin tài khoản
        /// </summary>
        /// <param name="req"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Accounts/UpdateProfile/{token}")]
        public IHttpActionResult UpdateProfile(VM_User_Edit req, string token)
        {
            try
            {
                if (!checkAuth(token))
                {
                    return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
                }
                var user = req.UpdateProfile(token);
                if (!string.IsNullOrEmpty(user))
                {
                    return Content(HttpStatusCode.OK, response.Ok(res.SingleResponse(token), "Cập nhật thông tin tài khoản thành công."));
                }
                return Content(HttpStatusCode.OK, response.Ok(null, "Cập nhật thông tin tài khoản không thành công.", false));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
            }
        }

        /// <summary>
        /// Lấy danh sách bác sỹ theo loại bệnh
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetListDoctor/{token}")]
        public IHttpActionResult GetListAccountWithType(VM_User_GetUserType req, string token)
        {
            try
            {
                if (!checkAuth(token))
                {
                    return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
                }
                return Content(HttpStatusCode.OK, response.Ok(res.ListWithRadiusResponse(token, req.MajorCode), "Lấy danh sách bác sỹ thành công."));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadGateway, response.BadRequest("Có lỗi trong quá trình xử lý"));
            }
        }

        /// <summary>
        /// Lấy thông tin bác sỹ
        /// </summary>
        /// <param name="GUID"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDoctor/GUID/{token}")]
        public IHttpActionResult GetDoctor(string GUID, string token)
        {
            try
            {
                if (!checkAuth(token))
                {
                    return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
                }
                return Content(HttpStatusCode.OK, response.Ok(res.SingleResponse(GUID), "Lấy thông tin bác sỹ thành công"));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadGateway, response.BadRequest("Có lỗi trong quá trình xử lý"));
            }
        }

        /// <summary>
        /// Đổi mật khẩu tài khoản
        /// </summary>
        /// <param name="Password"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Accounts/ChangePassword/{token}")]
        public IHttpActionResult ChangePassword(VM_User_ChangePassword req, string token)
        {
            try
            {
                if (!checkAuth(token))
                {
                    return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
                }
                var user = req.UpdatePassword(token);
                return Content(HttpStatusCode.OK, response.Ok(res.SingleResponse(user), "Thay đổi mật khẩu thành công."));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadGateway, response.BadRequest("Có lỗi trong quá trình xử lý"));
            }
        }

        /// <summary>
        /// Thay đổi trạng thái bác sỹ
        /// </summary>
        /// <param name="Password"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Accounts/ChangeStatusDoctor/{token}")]
        public IHttpActionResult ChangeStatusDoctor(VM_User_UpdateStatus req, string token)
        {
            try
            {
                if (!checkAuth(token))
                {
                    return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
                }
                var user = req.UpdateStatusUser(token);
                return Content(HttpStatusCode.OK, response.Ok(res.SingleResponse(user), "Thay trang thái của bác sỹ thành công."));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadGateway, response.BadRequest("Có lỗi trong quá trình xử lý"));
            }
        }

        /// <summary>
        /// Đổi ảnh đại diện người dùng
        /// </summary>
        /// <param name="strImg"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Accounts/UpdateImage/{token}")]
        public IHttpActionResult UploadImg(VM_User_UpdateAvatar req, string token)
        {
            try
            {
                if (!checkAuth(token))
                {
                    return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
                }
                var user = req.UpdateAvatar(token);
                return Content(HttpStatusCode.OK, response.Ok(res.SingleResponse(user), "Thay đổi ảnh đại diện thành công."));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadGateway, response.BadRequest("Có lỗi trong quá trình xử lý"));
            }
        }

        /// <summary>
        /// Kiểm tra số điện thoại trong hệ thống
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Route("Accounts/VerifyAccount")]
        [HttpPost]
        public IHttpActionResult VerifyAccount(VM_User_Verify req)
        {
            try
            {
                var user = req.VerifyUser();
                if (!string.IsNullOrEmpty(user))
                {
                    return Content(HttpStatusCode.OK, response.Ok(res.SingleResponse(user), "Xác thực tài khoản thành công."));
                }
                return Content(HttpStatusCode.OK, response.Ok(null, "Tài khoản không tồn tại trong hệ thống.", false));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý"));
            }
        }
        #endregion

    }
}
