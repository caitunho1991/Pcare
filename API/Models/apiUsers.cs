using CMS_Lib;
using CMS_Lib.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class VM_User_Login
    {
        [Required(ErrorMessage = "Vui lòng nhập username")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Vui lòng cung cấp vị trí hiện tại của bạn")]
        public string Lat { get; set; }
        [Required(ErrorMessage = "Vui lòng cung cấp vị trí hiện tại của bạn")]
        public string Lng { get; set; }
        public bool IsDoctor { get; set; }
        public string TokenFacebook { get; set; }
        public string TokenDevice { get; set; }

        public bool Login()
        {
            try
            {
                var GroupId = this.IsDoctor == true ? 1 : 2;
                using (FL_DoctorEntities __context = new FL_DoctorEntities())
                {
                    User user = new User();
                    if (!string.IsNullOrEmpty(TokenFacebook))
                    {
                        //login with facebook
                        if (__context.Users.Any(x => x.TokenFacebook.Equals(this.TokenFacebook)))
                        {
                            user = __context.Users.Single(x => x.TokenFacebook.Equals(this.TokenFacebook));
                        }
                        else
                        {
                            return false;
                        }
                    }
                    if (__context.Users.Any(x => x.Username.Equals(this.Username) && x.Password.Equals(this.Password)))
                    {
                        user = __context.Users.Single(x => x.Username.Equals(this.Username) && x.Password.Equals(this.Password));
                    }
                    else
                    {
                        return false;
                    }
                    int token_time = 0;
                    int.TryParse(CMS_Helper.Settings("cms_token_time"), out token_time);
                    if (user != null && DateTime.Compare((DateTime)user.ExpireTokenLogin, DateTime.Now) <= 0)
                    {
                        user.TokenLogin = CMS_Helper.GenerateGUID();
                        user.ExpireTokenLogin = DateTime.Now.AddDays(token_time);
                    }
                    user.Lat = this.Lat;
                    user.Lng = this.Lng;
                    user.TokenDevice = this.TokenDevice;
                    user.TokenAutoLogin = CMS_Helper.Base64Encode(user.TokenDevice + " - " + user.Password + " - " + (string.IsNullOrEmpty(user.Username) == true ? user.TokenFacebook : user.Username));
                    __context.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
    public class VM_User_Register
    {
        [Required(ErrorMessage = "Vui lòng nhập tên.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập họ và tên đệm.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập ngày sinh.")]
        public string DateofBirth { get; set; }
        /* 1- male, 2-female, 3-other*/
        public int Sex { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập email.")]
        public string Email { get; set; }
        public string TokenFacebook { get; set; }
        public string IdCardNumber { get; set; }/*CMND*/
        public string IdCard { get; set; }/*Img CMND*/
        public string LicenseId { get; set; }/*Img Giấy phép hành nghề*/
        public int MajorId { get; set; }/**/
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [StringLength(255, ErrorMessage = "Mật khẩu tối thiểu 5 ký tự", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public int TypeRegister { get; set; }/*type=0 => email, type=1 => phone*/
        public bool IsDoctor { get; set; }
        [Required(ErrorMessage = "Vui lòng cung cấp vị trí hiện tại của bạn")]
        public string Lat { get; set; }
        [Required(ErrorMessage = "Vui lòng cung cấp vị trí hiện tại của bạn")]
        public string Lng { get; set; }
        public string TokenDevice { get; set; }

        public bool Register()
        {
            try
            {
                using (FL_DoctorEntities __context = new FL_DoctorEntities())
                {
                    User user = new User();
                    //user.Address = this.Address;
                    user.Balance = CMS_Helper.Base64Encode("0");
                    user.DateCreated = DateTime.Now;
                    user.DateOfBirth = DateTime.ParseExact(this.DateofBirth, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    user.Email = this.Email;
                    user.FirstName = this.FirstName;
                    user.LastName = this.LastName;
                    user.FullName = this.LastName + " " + this.FirstName;
                    if (this.IsDoctor == true)
                    {
                        user.GroupID = __context.GroupUsers.Single(x => x.Code.Equals("doctor")).ID;
                        user.MajorID = this.MajorId;
                    }
                    else
                    {
                        user.GroupID = __context.GroupUsers.Single(x => x.Code.Equals("patient")).ID;
                    }
                    user.GUID = CMS_Helper.GenerateGUID();
                    user.IdCardNumber = this.IdCardNumber;
                    user.Lat = this.Lat;
                    user.Lng = this.Lng;
                    user.Password = this.Password;
                    user.PhoneNumber = this.PhoneNumber;
                    user.Sex = this.Sex;
                    user.TokenDevice = this.TokenDevice;
                    //if (!string.IsNullOrEmpty(this.ThumbAvatar))
                    //{
                    //    user.ThumbAvatar = CMS_Helper.ConvertBase64ToImage(this.ThumbAvatar, "Avatar_" + user.PhoneNumber);
                    //}
                    if (!string.IsNullOrEmpty(this.LicenseId))
                    {
                        user.ThumbLicense = CMS_Helper.ConvertBase64ToImage(this.LicenseId, "License_" + user.PhoneNumber);
                    }
                    if (!string.IsNullOrEmpty(this.IdCard))
                    {
                        user.ThumbIdCard = CMS_Helper.ConvertBase64ToImage(this.IdCard, "IDCard_" + user.PhoneNumber);
                    }
                    /****************/
                    if (!string.IsNullOrEmpty(this.TokenFacebook))
                    {
                        if (__context.Users.Any(x => x.TokenFacebook.Equals(this.TokenFacebook)))
                        {
                            return false;
                        }
                        user.TokenFacebook = this.TokenFacebook;
                    }
                    else
                    {
                        if (this.TypeRegister == 0)
                        {
                            user.IsVerifyEmail = true;
                            user.Username = user.Email;
                        }
                        else if (this.TypeRegister == 1)
                        {
                            user.IsVerifyPhone = true;
                            user.Username = user.PhoneNumber;
                        }
                    }
                    user.TokenAutoLogin = CMS_Helper.Base64Encode(user.TokenDevice + " - " + user.Password + " - " + (string.IsNullOrEmpty(user.Username) == true ? user.TokenFacebook : user.Username));

                    __context.Users.Add(user);
                    __context.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
    public class VM_User_UpdateAvatar
    {
        [Required(ErrorMessage = "Chuỗi hình ảnh không đúng hoặc không có. Vui lòng kiểm tra lại.")]
        public string Thumbnail { get; set; }

        public bool UpdateAvatar(string token)
        {
            try
            {
                using (FL_DoctorEntities __context = new FL_DoctorEntities())
                {
                    var user = __context.Users.Single(x => x.TokenLogin.Equals(token));
                    user.ThumbAvatar = CMS_Helper.ConvertBase64ToImage(this.Thumbnail, "Avatar-" + user.IdCardNumber + "-" + CMS_Helper.MD5(DateTime.Now.ToString()));
                    __context.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
    public class VM_User_Edit
    {
        [Required(ErrorMessage = "Vui lòng nhập tên.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập họ và tên đệm.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập ngày sinh.")]
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập email.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string IDCardNumber { get; set; }
        public int Sex { get; set; }
        public string ThumbLicense { get; set; }
        public string ThumbIdCard { get; set; }

        public bool UpdateProfile(string token)
        {
            try
            {
                using (FL_DoctorEntities __context = new FL_DoctorEntities())
                {
                    var user = __context.Users.Single(x => x.TokenLogin.Equals(token));
                    user.FirstName = this.FirstName;
                    user.LastName = this.LastName;
                    user.FullName = user.LastName + " " + user.FirstName;
                    user.DateOfBirth = this.DateOfBirth;
                    user.Email = this.Email;
                    user.PhoneNumber = this.PhoneNumber;
                    if (!string.IsNullOrEmpty(Address))
                    {
                        user.Address = this.Address;
                    }
                    if (!string.IsNullOrEmpty(IDCardNumber))
                    {
                        user.IdCardNumber = this.IDCardNumber;
                    }
                    if (this.Sex > 0)
                    {
                        user.Sex = this.Sex;
                    }
                    if (!string.IsNullOrEmpty(ThumbLicense))
                    {
                        user.ThumbLicense = CMS_Helper.ConvertBase64ToImage(this.ThumbLicense, "CardID_");
                    }
                    if (!string.IsNullOrEmpty(ThumbIdCard))
                    {
                        user.ThumbIdCard = CMS_Helper.ConvertBase64ToImage(this.ThumbIdCard, "CardID_");
                    }
                    __context.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
    public class VM_User_ChangePassword
    {
        [Required(ErrorMessage = "")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "")]
        public string NewPassword { get; set; }
        [Compare("NewPassword", ErrorMessage = "")]
        public string ConfirmNewPassword { get; set; }

        public bool UpdatePassword(string token)
        {
            try
            {
                using (FL_DoctorEntities __context = new FL_DoctorEntities())
                {
                    var user = __context.Users.Single(x => x.TokenLogin.Equals(token));
                    user.Password = CMS_Helper.MD5(this.NewPassword);
                    __context.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }

    public class VM_User_Response
    {
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public string IDCardNumber { get; set; }
        public string ImgAvatar { get; set; }
        public string ImgLicenseId { get; set; }
        public string ImgIdCard { get; set; }
        public string TokenLogin { get; set; }
        public bool IsDoctor { get; set; }
        public bool Active { get; set; }
        public string Balance { get; set; }
        public string MajorCode { get; set; }
        public int Sex { get; set; }
        public string TokenAutoLogin { get; set; }

        public VM_User_Response SingleResponse(string token)
        {
            using (FL_DoctorEntities __context = new FL_DoctorEntities())
            {
                return __context.Users.Where(x => x.TokenLogin.Equals(token)).Select(x => new VM_User_Response
                {
                    FullName = x.FullName,
                    TokenLogin = x.TokenLogin,
                    IsDoctor = (x.GroupUser.Code == "doctor" ? true : false),
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    IDCardNumber = x.IdCardNumber,
                    DateOfBirth = DateOfBirth,
                    Sex = (int)x.Sex,
                    UserName = x.Username,
                    Active = (bool)x.Active,
                    Balance = x.Balance,
                    ImgAvatar = string.IsNullOrEmpty(x.ThumbAvatar) == true ? "" : "/Uploads/" + x.ThumbAvatar,
                    ImgLicenseId = string.IsNullOrEmpty(x.ThumbLicense) == true ? "" : "/Uploads/" + x.ThumbLicense,
                    ImgIdCard = string.IsNullOrEmpty(x.Thumbnail) == true ? "" : "/Uploads/" + x.ThumbIdCard,
                    MajorCode = x.MajorID.ToString()
                }).Single();
            }
        }

        public List<VM_User_Response> ListResponse()
        {
            using (FL_DoctorEntities __context = new FL_DoctorEntities())
            {
                return __context.Users.Select(x => new VM_User_Response
                {
                    FullName = x.FullName,
                    TokenLogin = x.TokenLogin,
                    IsDoctor = (x.GroupUser.Code == "doctor" ? true : false),
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    IDCardNumber = x.IdCardNumber,
                    DateOfBirth = DateOfBirth,
                    Sex = (int)x.Sex,
                    UserName = x.Username,
                    Active = (bool)x.Active,
                    Balance = x.Balance,
                    ImgAvatar = string.IsNullOrEmpty(x.ThumbAvatar) == true ? "" : "/Uploads/" + x.ThumbAvatar,
                    ImgLicenseId = string.IsNullOrEmpty(x.ThumbLicense) == true ? "" : "/Uploads/" + x.ThumbLicense,
                    ImgIdCard = string.IsNullOrEmpty(x.Thumbnail) == true ? "" : "/Uploads/" + x.ThumbIdCard,
                    MajorCode = x.MajorID.ToString()
                }).ToList();
            }
        }
    }
}