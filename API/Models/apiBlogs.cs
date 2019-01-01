using CMS_Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class apiBlogsResponse
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public apiBlogsResponse GetLink(string link) {
            using (FL_DoctorEntities __context = new FL_DoctorEntities())
            {
                return __context.Blogs.Where(x => x.Active == true).Select(y => new apiBlogsResponse
                {
                    Title = y.Title,
                    Link = link
                }).Single();
            }
        }
    }
}