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

        public string Login()
        {
            try
            {
                using (FL_DoctorEntities __context = new FL_DoctorEntities())
                {
                    var GroupId = this.IsDoctor == true ? __context.GroupUsers.Single(x=>x.Code.Equals("doctor")).ID : __context.GroupUsers.Single(x => x.Code.Equals("patient")).ID;
                    User user = new User();
                    if (!string.IsNullOrEmpty(TokenFacebook))
                    {
                        //login with facebook
                        if (__context.Users.Any(x => x.TokenFacebook.Equals(this.TokenFacebook) && x.GroupID == GroupId))
                        {
                            user = __context.Users.Single(x => x.TokenFacebook.Equals(this.TokenFacebook) && x.GroupID == GroupId);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    if (__context.Users.Any(x => x.Username.Equals(this.Username) && x.Password.Equals(this.Password) && x.GroupID == GroupId))
                    {
                        user = __context.Users.Single(x => x.Username.Equals(this.Username) && x.Password.Equals(this.Password) && x.GroupID == GroupId);
                    }
                    else
                    {
                        return null;
                    }
                    int token_time = 0;
                    int.TryParse(CMS_Helper.Settings("token_time"), out token_time);
                    if (user != null && user.ExpireTokenLogin == null || DateTime.Compare((DateTime)user.ExpireTokenLogin, DateTime.Now) <= 0)
                    {
                        user.TokenLogin = CMS_Helper.GenerateGUID();
                        user.ExpireTokenLogin = DateTime.Now.AddDays(token_time);
                    }
                    user.Lat = this.Lat;
                    user.Lng = this.Lng;
                    user.TokenDevice = this.TokenDevice;
                    user.TokenAutoLogin = CMS_Helper.Base64Encode(user.TokenDevice + " - " + user.Password + " - " + (string.IsNullOrEmpty(user.Username) == true ? user.TokenFacebook : user.Username));
                    __context.SaveChanges();
                    return user.GUID;
                }
            }
            catch (Exception e)
            {
                return null;
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

        public string Register()
        {
            try
            {
                using (FL_DoctorEntities __context = new FL_DoctorEntities())
                {
                    User user = new User();
                    /****************/
                    if (!string.IsNullOrEmpty(this.TokenFacebook))
                    {
                        if (__context.Users.Any(x => x.TokenFacebook.Equals(this.TokenFacebook)))
                        {
                            return null;
                        }
                        user.TokenFacebook = this.TokenFacebook;
                        if (__context.Users.Any(x => x.TokenFacebook.Equals(user.TokenFacebook)))
                        {
                            return null;
                        }
                    }
                    else
                    {
                        if (this.TypeRegister == 0)
                        {
                            user.IsVerifyEmail = true;
                            user.Username = this.Email;
                        }
                        else if (this.TypeRegister == 1)
                        {
                            user.IsVerifyPhone = true;
                            user.Username = this.PhoneNumber;
                        }
                        if (__context.Users.Any(x => x.Username.Equals(user.Username)))
                        {
                            return null;
                        }
                    }
                    
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
                        user.Active = false;
                    }
                    else
                    {
                        user.GroupID = __context.GroupUsers.Single(x => x.Code.Equals("patient")).ID;
                        user.Active = true;
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
                    user.TokenAutoLogin = CMS_Helper.Base64Encode(user.TokenDevice + " - " + user.Password + " - " + (string.IsNullOrEmpty(user.Username) == true ? user.TokenFacebook : user.Username));

                    __context.Users.Add(user);
                    __context.SaveChanges();
                    return user.GUID;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
    public class VM_User_UpdateAvatar
    {
        [Required(ErrorMessage = "Chuỗi hình ảnh không đúng hoặc không có. Vui lòng kiểm tra lại.")]
        public string Thumbnail { get; set; }

        public string UpdateAvatar(string token)
        {
            try
            {
                using (FL_DoctorEntities __context = new FL_DoctorEntities())
                {
                    if (__context.Users.Any(x=>x.TokenLogin.Equals(token)))
                    {
                        var user = __context.Users.Single(x => x.TokenLogin.Equals(token));
                        user.ThumbAvatar = CMS_Helper.ConvertBase64ToImage(this.Thumbnail, "Avatar-" + user.IdCardNumber + "-" + CMS_Helper.MD5(DateTime.Now.ToString()));
                        __context.SaveChanges();
                        return user.GUID;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
    public class VM_User_Edit
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

        public string UpdateProfile(string token)
        {
            try
            {
                using (FL_DoctorEntities __context = new FL_DoctorEntities())
                {
                    if (__context.Users.Any(x=>x.TokenLogin.Equals(token)))
                    {
                        var user = __context.Users.Single(x => x.TokenLogin.Equals(token));
                        user.FirstName = this.FirstName;
                        user.LastName = this.LastName;
                        user.FullName = user.LastName + " " + user.FirstName;
                        user.DateOfBirth = DateTime.ParseExact(this.DateofBirth, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        user.Email = this.Email;
                        user.PhoneNumber = this.PhoneNumber;
                        //if (!string.IsNullOrEmpty(this.Address))
                        //{
                        //    user.Address = this.Address;
                        //}
                        if (!string.IsNullOrEmpty(this.IdCardNumber))
                        {
                            user.IdCardNumber = this.IdCardNumber;
                        }
                        if (this.Sex > 0)
                        {
                            user.Sex = this.Sex;
                        }
                        if (!string.IsNullOrEmpty(LicenseId))
                        {
                            user.ThumbLicense = CMS_Helper.ConvertBase64ToImage(this.LicenseId, "CardID_");
                        }
                        if (!string.IsNullOrEmpty(IdCard))
                        {
                            user.ThumbIdCard = CMS_Helper.ConvertBase64ToImage(this.IdCard, "CardID_");
                        }
                        __context.SaveChanges();
                        return user.GUID;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
    public class VM_User_ChangePassword
    {
        //[Required(ErrorMessage = "")]
        //public string OldPassword { get; set; }
        //[Required(ErrorMessage = "")]
        //public string NewPassword { get; set; }
        //[Compare("NewPassword", ErrorMessage = "")]
        //public string ConfirmNewPassword { get; set; }

        public string Password { get; set; }

        public string UpdatePassword(string token)
        {
            try
            {
                using (FL_DoctorEntities __context = new FL_DoctorEntities())
                {
                    var user = __context.Users.Single(x => x.TokenLogin.Equals(token));
                    user.Password = CMS_Helper.MD5(this.Password);
                    __context.SaveChanges();
                    return user.GUID;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
    public class VM_User_GetUserType
    {
        public int MajorCode { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
    }
    public class VM_User_UpdateStatus
    {
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string UpdateStatusUser(string token)
        {
            using (FL_DoctorEntities __context = new FL_DoctorEntities())
            {
                if (__context.Users.Any(x => x.TokenLogin.Equals(token)))
                {
                    var user = __context.Users.Single(x => x.TokenLogin.Equals(token));
                    user.Active = !(user.Active);
                    user.Lat = this.Lat;
                    user.Lng = this.Lng;
                    __context.SaveChanges();

                    return user.GUID;
                }
                else
                {
                    return null;
                }
            }
        }
    }
    public class VM_User_Verify
    {
        public string UserName { get; set; }
        public string VerifyUser()
        {
            using (FL_DoctorEntities __context = new FL_DoctorEntities())
            {
                if (__context.Users.Any(x=>x.Username.Equals(this.UserName)))
                {
                    var user = __context.Users.Single(x=>x.Username.Equals(this.UserName));
                    user.TokenForgotPassword = CMS_Helper.GenerateGUID();
                    user.ExpireTokenForgotPassword = DateTime.Now;
                    __context.SaveChanges();
                    return user.TokenForgotPassword;
                }
                return null;
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
        public string ThumbAvatar { get; set; }
        public string ThumbLicense { get; set; }
        public string ThumbIdCard { get; set; }
        public string TokenLogin { get; set; }
        public bool IsDoctor { get; set; }
        public bool Active { get; set; }
        public string Balance { get; set; }
        public string MajorCode { get; set; }
        public int Sex { get; set; }
        public string TokenAutoLogin { get; set; }
        
        public VM_User_Response SingleResponse(string user)
        {
            using (FL_DoctorEntities __context = new FL_DoctorEntities())
            {
                return __context.Users.Where(x => x.GUID.Equals(user) || x.TokenLogin.Equals(user) || x.TokenForgotPassword.Equals(user)).Select(x => new VM_User_Response
                {
                    FullName = x.FullName,
                    TokenLogin = x.TokenLogin,
                    IsDoctor = (x.GroupUser.Code == "doctor" ? true : false),
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    IDCardNumber = x.IdCardNumber,
                    DateOfBirth = x.DateOfBirth.ToString(),
                    Sex = (int)x.Sex,
                    UserName = x.Username,
                    Active = (bool)x.Active,
                    Balance = x.Balance,
                    ThumbAvatar = string.IsNullOrEmpty(x.ThumbAvatar) == true ? "" : "/Uploads/" + x.ThumbAvatar,
                    ThumbLicense = string.IsNullOrEmpty(x.ThumbLicense) == true ? "" : "/Uploads/" + x.ThumbLicense,
                    ThumbIdCard = string.IsNullOrEmpty(x.ThumbIdCard) == true ? "" : "/Uploads/" + x.ThumbIdCard,
                    MajorCode = x.MajorID.ToString(),
                    TokenAutoLogin = x.TokenAutoLogin
                }).Single();
            }
        }
        public List<VM_User_Response> ListWithRadiusResponse(string user, int MajorCode)
        {
            using (FL_DoctorEntities __context = new FL_DoctorEntities())
            {
                double condition_radius = 0;
                decimal global_min_fee_doctor = 0;
                var flag = CMS_Helper.Settings("global_min_fee_doctor");
                decimal.TryParse(flag, out global_min_fee_doctor);
                flag = CMS_Helper.Settings("global_condition_radius");
                double.TryParse(flag, out condition_radius);

                var patient = __context.Users.Single(x=>x.GUID.Equals(user) || x.TokenLogin.Equals(user));

                var GroupId = __context.GroupUsers.Single(x=>x.Code.Equals("doctor")).ID;
                var listUser = __context.Users.Where(x => x.MajorID == MajorCode && x.GroupID == GroupId  && x.Active == true && x.IsApprove == true && x.IsBanned != true).ToList();
                List<User> tmp = new List<User>();
                var radius = 0.0;
                foreach (var item in listUser)
                {
                    radius = CMS_Helper.DistanceTo(double.Parse(patient.Lat), double.Parse(patient.Lng), double.Parse(item.Lat), double.Parse(item.Lng));
                    if (radius <= condition_radius)
                    {
                        tmp.Add(item);
                    }
                }

                return tmp.Select(x => new VM_User_Response
                {
                    FullName = x.FullName,
                    TokenLogin = x.TokenLogin,
                    IsDoctor = (x.GroupUser.Code == "doctor" ? true : false),
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    IDCardNumber = x.IdCardNumber,
                    DateOfBirth = x.DateOfBirth.ToString(),
                    Sex = (int)x.Sex,
                    UserName = x.Username,
                    Active = (bool)x.Active,
                    Balance = x.Balance,
                    ThumbAvatar = string.IsNullOrEmpty(x.ThumbAvatar) == true ? "" : "/Uploads/" + x.ThumbAvatar,
                    ThumbLicense = string.IsNullOrEmpty(x.ThumbLicense) == true ? "" : "/Uploads/" + x.ThumbLicense,
                    ThumbIdCard = string.IsNullOrEmpty(x.ThumbIdCard) == true ? "" : "/Uploads/" + x.ThumbIdCard,
                    MajorCode = x.MajorID.ToString(),
                    TokenAutoLogin = x.TokenAutoLogin
                }).ToList();
            }
        }
        
    }
}