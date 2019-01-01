using CMS_Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class apiMajorsResponse
    {
        public int MayjorCode { get; set; }
        public string MayjorName { get; set; }
        public string MayjorDescription { get; set; }
        public apiMajorsResponse SingleResponse()
        {
            using (FL_DoctorEntities __context = new FL_DoctorEntities())
            {
                return __context.Majors.Where(x => x.Active == true).Select(y => new apiMajorsResponse
                {
                    MayjorCode = y.ID,
                    MayjorName = y.Name,
                    MayjorDescription = y.ShortDescription
                }).Single();
            }
        }
        public List<apiMajorsResponse> ListResponse()
        {
            using (FL_DoctorEntities __context = new FL_DoctorEntities())
            {
                return __context.Majors.Where(x => x.Active == true).Select(y => new apiMajorsResponse
                {
                    MayjorCode = y.ID,
                    MayjorName = y.Name,
                    MayjorDescription = y.ShortDescription
                }).ToList();
            }
        }
    }
}