using CMS_Lib.Data;
using Newtonsoft.Json;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CMS_Lib
{
    public static class CMS_Helper
    {
        #region Email Library
        public static Boolean SendEmail(string email, string title, string content)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtp_server = new SmtpClient(CMS_Helper.Settings("smtp_server"));
                mail.From = new MailAddress(CMS_Helper.Settings("smtp_username"));
                mail.Bcc.Add("victornguyen305@yahoo.com");
                mail.To.Add(email);
                mail.Subject = title;
                mail.Body = content;
                mail.IsBodyHtml = true;
                smtp_server.Port = int.Parse(CMS_Helper.Settings("smtp_port"));
                smtp_server.Credentials = new System.Net.NetworkCredential(CMS_Helper.Settings("smtp_username"), CMS_Helper.Settings("smtp_password"));
                smtp_server.EnableSsl = true;
                smtp_server.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        #endregion

        #region  Image Library
        public static string ConvertBase64ToImage(string strBase64, string filename)
        {
            try
            {
                String path = HttpContext.Current.Server.MapPath("~/Uploads/"); //Path                                                              //Check if directory exist
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
                }
                byte[] imgBytes = Convert.FromBase64String(strBase64);

                using (var imageFile = new FileStream(path +
                    filename + ".jpg", FileMode.Create))
                {
                    imageFile.Write(imgBytes, 0, imgBytes.Length);
                    imageFile.Flush();
                }
                return filename + ".jpg";
            }
            catch (Exception e)
            {
                return "";
            }
        }
        #endregion

        #region Security Library
        public static String GenerateGUID()
        {
            var GUID = Guid.NewGuid().ToString();
            return GUID;
        }

        public static string SHA1(string str)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(str));
                var sb = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string MD5(string str)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(str);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string ConvertString(string stringInput)
        {
            stringInput = stringInput.ToLower();
            string convert = "ĂÂÀẰẦÁẮẤẢẲẨÃẴẪẠẶẬỄẼỂẺÉÊÈỀẾẸỆÔÒỒƠỜÓỐỚỎỔỞÕỖỠỌỘỢƯÚÙỨỪỦỬŨỮỤỰÌÍỈĨỊỲÝỶỸỴĐăâàằầáắấảẳẩãẵẫạặậễẽểẻéêèềếẹệôòồơờóốớỏổởõỗỡọộợưúùứừủửũữụựìíỉĩịỳýỷỹỵđ";
            string To = "aaaaaaaaaaaaaaaaaeeeeeeeeeeeooooooooooooooooouuuuuuuuuuuiiiiiyyyyydaaaaaaaaaaaaaaaaaeeeeeeeeeeeooooooooooooooooouuuuuuuuuuuiiiiiyyyyyd";
            for (int i = 0; i < To.Length; i++)
            {
                stringInput = stringInput.Replace(convert[i], To[i]);
            }
            stringInput = stringInput.Replace("-", "");
            stringInput = stringInput.Replace("&", "");
            stringInput = stringInput.Replace(@"""", "");
            stringInput = stringInput.Replace("/", "");
            stringInput = stringInput.Replace("(", "");
            stringInput = stringInput.Replace(")", "");
            stringInput = stringInput.Replace("\\", "");
            stringInput = stringInput.Replace(".", "-");
            stringInput = stringInput.Replace(" ", "-");
            stringInput = stringInput.Replace(",", "-");
            stringInput = stringInput.Replace(";", "-");
            stringInput = stringInput.Replace(":", "-");
            stringInput = stringInput.Replace("'", "");
            stringInput = stringInput.Replace("?", "");
            stringInput = stringInput.Replace("%", "-");

            return stringInput;

        }

        /// <summary>
        /// Create Transaction Number
        /// </summary>
        /// <param name="transactionID"></param>
        /// <returns></returns>
        public static string createTransactionIDString(long transactionID)
        {
            string begin = "Order_";
            int lenght = 10;
            string result = begin + transactionID.ToString().PadLeft(lenght, '0');
            return result;

        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        #endregion

        #region Radius, Check PhoneNumber, Push Notify to Mobile
        public static double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }

            return dist;
        }

        public static bool CheckPhoneNumber(string phoneNumber, string countryCode = "VN")
        {
            try
            {
                PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
                var a = phoneUtil.Parse(phoneNumber, "VN");
                PhoneNumber _phoneNumber = phoneUtil.ParseAndKeepRawInput(phoneNumber, "VN");
                return phoneUtil.IsValidNumber(_phoneNumber);
            }
            catch
            {
                return false;
            }
        }

        public static string PushNotify(string token, string title, string body)
        {
            var result = "";
            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            //serverKey - Key from Firebase cloud messaging server  
            tRequest.Headers.Add(string.Format("Authorization: key={0}", "AAAAqIZmlqY:APA91bFKPRc35wwrZ4WgD7y1fxaqp-hvi_nfIHGuVBoZTVRRGHx_yHIzkybu3dknn23MpfffA__K_Ldn-tGxTKePIFu1F14hQGi1dgCWL4Rf2T_hce5NE2S0x3yoVDNiSgmjtB0dhMoe"));
            //Sender Id - From firebase project setting  
            tRequest.Headers.Add(string.Format("Sender: id={0}", "723809375910"));
            tRequest.ContentType = "application/json";
            var payload = new
            {
                to = token,
                priority = "high",
                content_available = true,
                notification = new
                {
                    body = body,
                    title = title,
                    badge = 1
                },
                data = new
                {
                    body = body,
                    title = title,
                    badge = 1
                }
            };

            string postbody = JsonConvert.SerializeObject(payload).ToString();
            Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
            tRequest.ContentLength = byteArray.Length;
            using (Stream dataStream = tRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                using (WebResponse tResponse = tRequest.GetResponse())
                {
                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                    {
                        if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                result = sResponseFromServer;
                            }
                    }
                }
            }
            return result;
        }
        #endregion

        public static decimal ConvertStringToDecimal(string number)
        {
            try
            {
                decimal n = 0;
                decimal.TryParse(number, out n);
                return n;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public static string Settings(string key)
        {
            try
            {
                string value = "";
                if (string.IsNullOrEmpty(key))
                {
                    return value;
                }
                using (FL_DoctorEntities __context = new FL_DoctorEntities())
                {
                    if (__context.Settings.Any(x => x.Active == true && x.Code.Equals(key)))
                    {
                        value = __context.Settings.Single(x => x.Active == true && x.Code.Equals(key)).Value;
                    }
                }
                return value;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }

}
