using CMS_Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class apiCouponsResponse
    {
        public string CouponDescription { get; set; }
        public decimal CouponValue { get; set; }
        public bool IsPercent { get; set; }
        public bool CheckCoupon(string token, string CouponCode)
        {
            using (FL_DoctorEntities __context = new FL_DoctorEntities())
            {
                var coupon = __context.Coupons.SingleOrDefault(x => x.Code.ToUpper().Equals(CouponCode.ToUpper()));
                var acc = __context.Users.SingleOrDefault(x => x.TokenLogin.Equals(token));
                if (coupon != null)
                {
                    //this statement check the coupon has expiry date
                    if (DateTime.Compare(DateTime.Now, (DateTime)coupon.DateStart) > 0 && DateTime.Compare(DateTime.Now, (DateTime)coupon.DateEnd) < 0)
                    {
                        if (!__context.TransactionCoupons.Any(x => x.Transaction.SenderID == acc.ID && x.CouponID == coupon.ID))
                        {
                            return true;
                        }
                        return false;
                    }
                    return false;
                }
                return false;
            }
        }

        public apiCouponsResponse SingleResponse(string CouponCode)
        {
            using (FL_DoctorEntities __context = new FL_DoctorEntities())
            {
                return __context.Coupons.Where(x => x.Code.Equals(CouponCode)).Select(x=> new apiCouponsResponse {
                    CouponDescription = x.Description,
                    CouponValue = (decimal)x.Value == null ? ((decimal)x.PercentValue == null ? 0 : (decimal)x.PercentValue) : (decimal)x.Value,
                    IsPercent = (decimal)x.Value == null ? ((decimal)x.PercentValue == null ? false : true) : false
                }).Single();
            }
        } 
    }
}