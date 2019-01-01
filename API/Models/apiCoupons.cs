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
    }
}