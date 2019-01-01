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
                if (req.Login())
                {
                    return Content(HttpStatusCode.OK, response.Ok(res.SingleResponse(req.Username), "Đăng nhập thành công."));
                }
                return Content(HttpStatusCode.BadRequest, response.BadRequest("Tên đăng nhập hoặc mật khẩu không đúng. Vui lòng kiểm tra lại."));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadGateway, response.BadRequest("Có lỗi trong quá trình xử lý"));
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
                if (req.Register())
                {
                    return Content(HttpStatusCode.OK, response.Ok(res.SingleResponse(req.Email), ""));
                }
                return Content(HttpStatusCode.OK, response.Ok(null, "Tài khoản đã tồn tại trong hệ thống hoặc dữ liệu không đúng. Vui lòng kiểm tra lại.", false));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadGateway, response.BadRequest("Có lỗi trong quá trình xử lý"));
            }
        }

        ///// <summary>
        ///// Đăng xuất tài khoản
        ///// </summary>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("Accounts/Logout/{token}")]
        //public IHttpActionResult LogOut(string token)
        //{
        //    try
        //    {
        //        if (!checkAuth(token))
        //        {
        //            return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
        //        }
        //        var acc = _context.Accounts.SingleOrDefault(x => x.TokenLogin.Equals(token));
        //        if (acc != null)
        //        {
        //            acc.TokenLogin = "";
        //            acc.ExpireTokenLogin = DateTime.Now;
        //            _context.SaveChanges();
        //            return Content(HttpStatusCode.OK, response.Ok(null, "Đăng xuất tài khoản thành công."));
        //        }
        //        return Content(HttpStatusCode.OK, response.Ok(null, "Tài khoản của bạn không tồn tại.", false));
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
        //    }
        //}

        ///// <summary>
        ///// Cập nhật thông tin tài khoản
        ///// </summary>
        ///// <param name="req"></param>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //[HttpPut]
        //[Route("Accounts/UpdateProfile/{token}")]
        //public IHttpActionResult UpdateProfile(VM_User_Edit req, string token)
        //{
        //    try
        //    {
        //        if (!checkAuth(token))
        //        {
        //            return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
        //        }
        //        if (req.UpdateProfile(token))
        //        {

        //        }
        //        return Content(HttpStatusCode.OK, response.Ok(res.SingleResponse(token), "Cập nhật thông tin tài khoản thành công."));
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
        //    }
        //}

        ///// <summary>
        ///// Lấy danh sách bác sỹ theo loại bệnh
        ///// </summary>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("GetListDoctor/{token}")]
        //public IHttpActionResult GetListAccountWithType(VM_Req_GetDoctor req, string token)
        //{
        //    try
        //    {
        //        if (!checkAuth(token))
        //        {
        //            return Ok(response.NoAuth(null, "Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
        //        }
        //        #region Get list doctor with radius
        //        double condition_radius = 0;
        //        decimal global_min_fee_doctor = 0;
        //        var flag = _context.Resources.FirstOrDefault(x => x.Code.Equals("global_min_fee_doctor")).Value;
        //        decimal.TryParse(flag, out global_min_fee_doctor);
        //        flag = _context.Resources.FirstOrDefault(x => x.Code.Equals("global_condition_radius")).Value;
        //        double.TryParse(flag, out condition_radius);
        //        var patient = _context.Accounts.SingleOrDefault(x => x.TokenLogin.Equals(token));
        //        var product = _context.Products.SingleOrDefault(x => x.id == req.MajorCode);
        //        //update lat lng patient
        //        patient.Lat = req.Lat;
        //        patient.Lng = req.Lng;
        //        _context.SaveChanges();

        //        List<Account> lstDoctor;
        //        if (req.MajorCode != null || req.MajorCode > 0)
        //        {
        //            lstDoctor = _context.Accounts.Where(x => x.ProductId == req.MajorCode && x.Balance >= product.price && x.GroupId == 1 && x.Balance >= global_min_fee_doctor && x.IsActive == true && x.IsApprove == true && x.IsBanned != true).ToList();
        //        }
        //        else
        //        {
        //            lstDoctor = _context.Accounts.Where(x => x.GroupId == 1 && x.Balance > global_min_fee_doctor && x.IsActive == true && x.IsApprove == true && x.IsBanned != true).ToList();
        //        }
        //        List<Account> tmp = new List<Account>();
        //        var radius = 0.0;
        //        foreach (var item in lstDoctor)
        //        {
        //            //radius = CMS_Lib.Radius(patient.Lat, patient.Lng, item.Lat, item.Lng) * 0.621371192;
        //            radius = CMS_Lib.DistanceTo(double.Parse(patient.Lat), double.Parse(patient.Lng), double.Parse(item.Lat), double.Parse(item.Lng));

        //            if (radius <= condition_radius)
        //            {
        //                tmp.Add(item);
        //            }
        //        }
        //        #endregion
        //        var list = tmp.Select(x => new VM_Res_Account
        //        {
        //            FullName = x.FullName,
        //            MajorName = x.Product.name,
        //            Fee = x.Product.price.ToString(),
        //            Lat = x.Lat,
        //            Lng = x.Lng,
        //            ThumbnailUrl = string.IsNullOrEmpty(x.Thumbnail) == true ? "" : domain + "/Uploads/" + x.Thumbnail,
        //            OrderCount = (int)x.OrderCount,
        //            GUID = x.GUID,
        //            MajorCode = x.Product.id,
        //            ThumbnailIDCard = string.IsNullOrEmpty(x.ThumbnailIDCard) == true ? "" : domain + "/Uploads/" + x.ThumbnailIDCard,
        //            ThumbnailLicense = string.IsNullOrEmpty(x.ThumbnailLicense) == true ? "" : domain + "/Uploads/" + x.ThumbnailLicense,
        //            Radius = condition_radius
        //        }).ToList();
        //        return Ok(responseAccount.Ok(list, "Lấy danh sách bác sỹ thành công."));
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(HttpStatusCode.BadGateway, response.BadRequest("Có lỗi trong quá trình xử lý"));
        //    }
        //}

        ///// <summary>
        ///// Lấy thông tin bác sỹ
        ///// </summary>
        ///// <param name="GUID"></param>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("GetDoctor/GUID/{token}")]
        //public IHttpActionResult GetDoctor(string GUID, string token)
        //{
        //    try
        //    {
        //        if (!checkAuth(token))
        //        {
        //            return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
        //        }
        //        var list = _context.Accounts.Where(x => x.GroupId == 1 && x.IsActive == true && x.GUID.Equals(GUID) && x.IsApprove == true).Select(x => new VM_Res_Account
        //        {
        //            FullName = x.FullName,
        //            MajorName = x.Product.name,
        //            Fee = x.Product.price.ToString(),
        //            Lat = x.Lat,
        //            Lng = x.Lng,
        //            ThumbnailUrl = string.IsNullOrEmpty(x.Thumbnail) == true ? "" : domain + "/Uploads/" + x.Thumbnail,
        //            OrderCount = (int)x.OrderCount,
        //            GUID = x.GUID,
        //            MajorCode = x.Product.id,
        //            ThumbnailIDCard = string.IsNullOrEmpty(x.ThumbnailIDCard) == true ? "" : domain + "/Uploads/" + x.ThumbnailIDCard,
        //            ThumbnailLicense = string.IsNullOrEmpty(x.ThumbnailLicense) == true ? "" : domain + "/Uploads/" + x.ThumbnailLicense
        //        }).ToList();
        //        return Ok(responseAccount.Ok(list, "Lấy thông tin bác sỹ thành công"));
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(HttpStatusCode.BadGateway, response.BadRequest("Có lỗi trong quá trình xử lý"));
        //    }
        //}

        ///// <summary>
        ///// Đổi mật khẩu tài khoản
        ///// </summary>
        ///// <param name="Password"></param>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //[HttpPut]
        //[Route("Accounts/ChangePassword/{token}")]
        //public IHttpActionResult ChangePassword(string Password, string token)
        //{
        //    try
        //    {
        //        if (!checkAuth(token))
        //        {
        //            return Ok(response.NoAuth(null, "Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
        //        }

        //        if (!_context.Accounts.Any(x => x.TokenLogin.Contains(token)))
        //        {
        //            return Ok(response.BadRequest("Tài khoản không tồn tại trong hệ thống. Xin vui lòng kiểm tra lại email hoặc số điện thoại."));
        //        }
        //        var tmp = _context.Accounts.SingleOrDefault(x => x.TokenLogin.Equals(token));

        //        tmp.Password = Password;
        //        _context.SaveChanges();
        //        var DateOfBirth = ((DateTime)tmp.BirthDay).ToString("dd/MM/yyyy");
        //        var dataResponse = _context.Accounts.Where(x => x.TokenLogin.Equals(token)).Select(x => new VM_Res_Account_Short
        //        {
        //            FullName = x.FullName,
        //            Token = x.TokenLogin,
        //            IsDoctor = x.GroupId == 1 ? true : false,
        //            FirstName = x.FirstName,
        //            LastName = x.LastName,
        //            IDCardNumber = x.IDCard,
        //            DateOfBirth = DateOfBirth,
        //            Sex = (int)x.Sex,
        //            UserName = string.IsNullOrEmpty(x.Email) == true ? x.PhoneNumber : x.Email,
        //            Active = (bool)x.IsActive,
        //            ImgIdCard = string.IsNullOrEmpty(x.ThumbnailIDCard) == true ? "" : domain + "/Uploads/" + x.ThumbnailIDCard,
        //            ImgLicenseId = string.IsNullOrEmpty(x.ThumbnailLicense) == true ? "" : domain + "/Uploads/" + x.ThumbnailLicense,
        //            ImgAvatar = string.IsNullOrEmpty(x.Thumbnail) == true ? "" : domain + "/Uploads/" + x.Thumbnail,
        //            MajorCode = x.ProductId,
        //            MajorName = x.Product.name
        //        }).SingleOrDefault();
        //        return Ok(response.Ok(dataResponse, "Thay đổi mật khẩu thành công."));
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(HttpStatusCode.BadGateway, response.BadRequest("Có lỗi trong quá trình xử lý"));
        //    }
        //}

        ///// <summary>
        ///// Thay đổi trạng thái bác sỹ
        ///// </summary>
        ///// <param name="Password"></param>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //[HttpPut]
        //[Route("Accounts/ChangeStatusDoctor/{token}")]
        //public IHttpActionResult ChangeStatusDoctor(VM_Req_Status_Doctor req, string token)
        //{
        //    try
        //    {
        //        if (!checkAuth(token))
        //        {
        //            return Ok(response.NoAuth(null, "Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
        //        }
        //        var acc = _context.Accounts.FirstOrDefault(x => x.TokenLogin.Equals(token));
        //        req.ConvertModelToData(acc);
        //        _context.SaveChanges();
        //        var DateOfBirth = ((DateTime)acc.BirthDay).ToString("dd/MM/yyyy");
        //        var dataResponse = _context.Accounts.Where(x => x.TokenLogin.Equals(token)).Select(x => new VM_Res_Account_Short
        //        {
        //            FullName = x.FullName,
        //            Token = x.TokenLogin,
        //            IsDoctor = x.GroupId == 1 ? true : false,
        //            FirstName = x.FirstName,
        //            LastName = x.LastName,
        //            IDCardNumber = x.IDCard,
        //            DateOfBirth = DateOfBirth,
        //            Sex = (int)x.Sex,
        //            UserName = string.IsNullOrEmpty(x.Email) == true ? x.PhoneNumber : x.Email,
        //            Active = (bool)x.IsActive,
        //            ImgIdCard = string.IsNullOrEmpty(x.ThumbnailIDCard) == true ? "" : domain + "/Uploads/" + x.ThumbnailIDCard,
        //            ImgLicenseId = string.IsNullOrEmpty(x.ThumbnailLicense) == true ? "" : domain + "/Uploads/" + x.ThumbnailLicense,
        //            ImgAvatar = string.IsNullOrEmpty(x.Thumbnail) == true ? "" : domain + "/Uploads/" + x.Thumbnail,
        //            MajorCode = x.ProductId,
        //            MajorName = x.Product.name
        //        }).SingleOrDefault();

        //        return Ok(response.Ok(dataResponse, "Thay trang thái của bác sỹ thành công."));
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(HttpStatusCode.BadGateway, response.BadRequest("Có lỗi trong quá trình xử lý"));
        //    }
        //}

        ///// <summary>
        ///// Đổi ảnh đại diện người dùng
        ///// </summary>
        ///// <param name="strImg"></param>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //[HttpPut]
        //[Route("Accounts/UpdateImage/{token}")]
        //public IHttpActionResult UploadImg(VM_User_UpdateAvatar avatar, string token)
        //{
        //    try
        //    {
        //        if (!checkAuth(token))
        //        {
        //            return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
        //        }
        //        var acc = _context.Users.SingleOrDefault(x => x.TokenLogin.Equals(token));
        //        acc.Thumbnail = CMS_Helper.ConvertBase64ToImage(avatar.Thumbnail, "CardID_" + CMS_Helper.MD5(acc.IdCardNumber + DateTime.Now.ToString()));
        //        _context.SaveChanges();

        //        return Content(HttpStatusCode.OK, response.Ok(res.SingleResponse(token), "Thay đổi ảnh đại diện thành công."));
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(HttpStatusCode.BadGateway, response.BadRequest("Có lỗi trong quá trình xử lý"));
        //    }
        //}

        ///// <summary>
        ///// Kiểm tra số điện thoại trong hệ thống
        ///// </summary>
        ///// <param name="req"></param>
        ///// <returns></returns>
        //[Route("Accounts/VerifyAccount")]
        //[HttpPost]
        //public IHttpActionResult VerifyAccount(VM_Account_Req_Verify req)
        //{
        //    try
        //    {
        //        Response<VM_Account_Res_Verify> response = new Response<VM_Account_Res_Verify>();
        //        if (string.IsNullOrEmpty(req.Username))
        //        {
        //            return Ok(response.BadRequest("Số điện thoại hoặc email không hợp lệ. Vui lòng kiểm tra lại"));
        //        }
        //        if (_context.Accounts.Any(x => x.PhoneNumber.Equals(req.Username) || x.Email.ToUpper().Equals(req.Username.ToUpper())))
        //        {
        //            var acc = _context.Accounts.SingleOrDefault(x => x.PhoneNumber.Equals(req.Username) || x.Email.ToUpper().Equals(req.Username.ToUpper()));
        //            if (acc != null)
        //            {
        //                acc.TokenAutoLogin = CMS_Security.SHA1(acc.DeviceToken + "-" + acc.Password + "-" + acc.PhoneNumber);
        //                acc.TokenLogin = CMS_Security.GenerateGUID().ToString();
        //                int token_time = 0;
        //                int.TryParse(CMS_Lib.Resource("cms_token_time"), out token_time);
        //                acc.ExpireTokenLogin = DateTime.Now.AddDays(token_time);
        //                _context.SaveChanges();

        //            }
        //            var res = _context.Accounts.Where(x => x.PhoneNumber.Equals(req.Username) || x.Email.ToUpper().Equals(req.Username.ToUpper())).Select(x => new VM_Account_Res_Verify
        //            {
        //                Token = x.TokenLogin
        //            }).SingleOrDefault();
        //            return Ok(response.Ok(res, "Xác thực tài khoản thành công."));
        //        }
        //        return Ok(response.Ok(null, "Tài khoản không tồn tại trong hệ thống.", false));
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý"));
        //    }
        //}
        #endregion

    }
}
