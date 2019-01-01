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
    public class BaseApiController : ApiController
    {
        public apiResponse response = new apiResponse();
        public FL_DoctorEntities _context = new FL_DoctorEntities();
        public string domain = System.Configuration.ConfigurationManager.AppSettings["domain"];

        protected Boolean checkAuth(string token)
        {
            var acc = _context.Users.SingleOrDefault(x => x.TokenLogin.Equals(token));
            int token_time = 0;
            int.TryParse(CMS_Helper.Settings("cms_token_time"), out token_time);
            if (!string.IsNullOrEmpty(token) && acc != null)
            {
                if (DateTime.Compare((DateTime)acc.ExpireTokenLogin, DateTime.Now) > 0)
                {
                    acc.ExpireTokenLogin = DateTime.Now.AddDays(token_time);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
