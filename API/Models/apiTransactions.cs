using CMS_Lib;
using CMS_Lib.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace API.Models
{
    public class VM_Transaction
    {
        public string GuidDoctor { get; set; }
        public string Coupon { get; set; }

        public string Create(string token)
        {
            try
            {
                using (FL_DoctorEntities __context = new FL_DoctorEntities())
                {
                    if (__context.Users.Any(x => x.TokenLogin.Equals(token) && x.GroupUser.Code.Equals("patient")) == true && __context.Users.Any(x=>x.GUID.Equals(this.GuidDoctor) && x.GroupUser.Code.Equals("doctor")))
                    {
                        var patient = __context.Users.Single(x=>x.TokenLogin.Equals(token));
                        var doctor = __context.Users.Single(x=>x.GUID.Equals(this.GuidDoctor));
                        Coupon coupon = new Coupon();
                        if (__context.Coupons.Any(x=>x.Code.Equals(this.Coupon)))
                        {
                            coupon = __context.Coupons.Single(x=>x.Code.Equals(this.Coupon));
                        }
                        var doctorBalance = CMS_Helper.ConvertStringToDecimal(CMS_Helper.Base64Decode(doctor.Balance));
                        if ( doctorBalance >= doctor.Major.Price && doctorBalance >= CMS_Helper.ConvertStringToDecimal(CMS_Helper.Settings("services_fee")))
                        {
                            Transaction t = new Transaction();
                            t.DateCreated = DateTime.UtcNow;
                            var tmp = doctorBalance - (decimal)doctor.Major.Price;
                            t.EndingBalanceReceiver = CMS_Helper.Base64Encode(tmp.ToString());
                            doctor.Balance = t.EndingBalanceReceiver;//update balance doctor
                            t.SenderID = patient.ID;
                            t.ReceiverID = doctor.ID;
                            var tmpTotal = doctor.Major.Price;
                            t.TotalAmount = CMS_Helper.Base64Encode(tmpTotal.ToString());
                            #region Check Coupon
                            decimal? tmpTotalDiscount = 0;
                            if (coupon!=null && (DateTime.Compare(DateTime.Now, (DateTime)coupon.DateStart) >= 0) && (DateTime.Compare(DateTime.Now, (DateTime)coupon.DateEnd)) <= 0 && coupon.Count > 0)
                            {
                                if (coupon.PercentValue != null)
                                {
                                    tmpTotalDiscount = tmpTotal * (decimal)coupon.PercentValue / 100;
                                    t.TotalDiscount = CMS_Helper.Base64Encode(tmpTotalDiscount.ToString());
                                }
                                else
                                {
                                    tmpTotalDiscount = coupon.Value;
                                    t.TotalDiscount = CMS_Helper.Base64Encode(tmpTotalDiscount.ToString());
                                }
                            }
                            #endregion
                            var tmpTotalPaid = tmpTotal - tmpTotalDiscount;
                            t.TotalPaid = CMS_Helper.Base64Encode(tmpTotalPaid.ToString());
                            t.TransactionTypeID = __context.TransactionTypes.Single(x=>x.Code.Equals("order")).ID;
                            #region Transaction Status
                            var transactionStatus = __context.TransactionStatus.Single(x => x.Code.Equals("order_create"));
                            TransactionTransactionStatu tts = new TransactionTransactionStatu();
                            tts.TransactionID = t.ID;
                            tts.TransactionStatusID = transactionStatus.ID;
                            tts.DateCreated = DateTime.UtcNow;
                            t.TransactionTransactionStatus.Add(tts);
                            __context.Transactions.Add(t);
                            #endregion
                            t.TransactionCode = CMS_Helper.createTransactionIDString(t.ID);
                            #region Transaction Detail
                            TransactionDetail transactionDetail = new TransactionDetail();
                            transactionDetail.ProductName = doctor.Major.Name;
                            transactionDetail.Quantity = 1;
                            transactionDetail.Price = CMS_Helper.Base64Encode(doctor.Major.Price == null ? "0" : doctor.Major.Price.ToString());
                            transactionDetail.TransactionID = t.ID;
                            var tmp_total = doctor.Major.Price * transactionDetail.Quantity;
                            transactionDetail.Total = CMS_Helper.Base64Encode(tmp_total.ToString()) ;
                            t.TransactionDetails.Add(transactionDetail);
                            #endregion
                            __context.SaveChanges();
                            return t.TransactionCode;
                        }
                        return null;
                    }
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
    public class VM_Transaction_Respone
    {
        public string OrderNumber { get; set; }
        public string DateCreate { get; set; }
        public string DoctorName { get; set; }
        public string MajorDescription { get; set; }
        public string PatientName { get; set; }
        public int Sex { get; set; }
        public string DateOfBirth { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string OrderStatusName { get; set; }
        public string OrderStatusCode { get; set; }

        public bool CheckTransaction(string token)
        {
            try
            {
                using (FL_DoctorEntities __context = new FL_DoctorEntities())
                {
                    if (__context.Transactions.Any(x=>x.User.TokenLogin.Equals(token)))
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public VM_Transaction_Respone SingleResponse(string token, string TransactionNumber)
        {
            try
            {
                using (FL_DoctorEntities __context = new FL_DoctorEntities())
                {
                    if (!__context.Transactions.Any(x => x.User.TokenLogin.Equals(token)))
                    {
                        return null;
                    }
                    return __context.Transactions.Where(x => x.User.TokenLogin.Equals(token) && x.TransactionCode.Equals(TransactionNumber)).Join(
                        __context.TransactionStatus,
                        transaction => transaction.TransactionTransactionStatus.OrderByDescending(x => x.ID).First().TransactionStatusID,
                        transactionStatus => transactionStatus.ID,
                        (transaction, transactionStatus) => new VM_Transaction_Respone
                        {
                            OrderNumber = transaction.TransactionCode,
                            DateCreate = transaction.DateCreated.ToString(),
                            DoctorName = transaction.User1.FullName,
                            MajorDescription = transaction.User1.Major.Name,
                            PatientName = transaction.User.FullName,
                            Sex = (int)transaction.User.Sex,
                            DateOfBirth = transaction.User.DateOfBirth.ToString(),
                            Address = transaction.User.Address,
                            PhoneNumber = transaction.User.PhoneNumber,
                            OrderStatusName = transactionStatus.Name,
                            OrderStatusCode = transactionStatus.Code
                        }
                    ).Single();
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<VM_Transaction_Respone> ListResponse(string token)
        {
            try
            {
                using (FL_DoctorEntities __context = new FL_DoctorEntities())
                {
                    if (!__context.Transactions.Any(x => x.User.TokenLogin.Equals(token)))
                    {
                        return null;
                    }
                    return __context.Transactions.Where(x=>x.User.TokenLogin.Equals(token)).Join(
                        __context.TransactionStatus,
                        transaction => transaction.TransactionTransactionStatus.OrderByDescending(x => x.ID).First().TransactionStatusID,
                        transactionStatus => transactionStatus.ID,
                        (transaction, transactionStatus) => new VM_Transaction_Respone
                        {
                            OrderNumber = transaction.TransactionCode,
                            DateCreate = transaction.DateCreated.ToString(),
                            DoctorName = transaction.User1.FullName,
                            MajorDescription = transaction.User1.Major.Name,
                            PatientName = transaction.User.FullName,
                            Sex = (int)transaction.User.Sex,
                            DateOfBirth = transaction.User.DateOfBirth.ToString(),
                            Address = transaction.User.Address,
                            PhoneNumber = transaction.User.PhoneNumber,
                            OrderStatusName = transactionStatus.Name,
                            OrderStatusCode = transactionStatus.Code
                        }
                    ).ToList();
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<VM_Transaction_Respone> ListResponseWithType(string token, string TransactionType)
        {
            try
            {
                using (FL_DoctorEntities __context = new FL_DoctorEntities())
                {
                    if (!__context.Transactions.Any(x => x.User.TokenLogin.Equals(token)))
                    {
                        return null;
                    }
                    return __context.Transactions.Where(x=>x.TransactionType.Code.Equals(TransactionType) && x.User.TokenLogin.Equals(token)).Join(
                        __context.TransactionStatus,
                        transaction => transaction.TransactionTransactionStatus.OrderByDescending(x => x.ID).First().TransactionStatusID,
                        transactionStatus => transactionStatus.ID,
                        (transaction, transactionStatus) => new VM_Transaction_Respone
                        {
                            OrderNumber = transaction.TransactionCode,
                            DateCreate = transaction.DateCreated.ToString(),
                            DoctorName = transaction.User1.FullName,
                            MajorDescription = transaction.User1.Major.Name,
                            PatientName = transaction.User.FullName,
                            Sex = (int)transaction.User.Sex,
                            DateOfBirth = transaction.User.DateOfBirth.ToString(),
                            Address = transaction.User.Address,
                            PhoneNumber = transaction.User.PhoneNumber,
                            OrderStatusName = transactionStatus.Name,
                            OrderStatusCode = transactionStatus.Code
                        }
                    ).ToList();
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<VM_Transaction_Respone> ListResponseWithTypeAndStatus(string token, string  TransactionType, string TransactionStatus)
        {
            try
            {
                using (FL_DoctorEntities __context = new FL_DoctorEntities())
                {
                    if (!__context.Transactions.Any(x => x.User.TokenLogin.Equals(token)))
                    {
                        return null;
                    }
                    return __context.Transactions.Where(x => x.TransactionType.Code.Equals(TransactionType) && x.User.TokenLogin.Equals(token)).Join(
                        __context.TransactionStatus,
                        transaction => transaction.TransactionTransactionStatus.OrderByDescending(x => x.ID).First().TransactionStatusID,
                        transactionStatus => transactionStatus.ID,
                        (transaction, transactionStatus) => new VM_Transaction_Respone
                        {
                            OrderNumber = transaction.TransactionCode,
                            DateCreate = transaction.DateCreated.ToString(),
                            DoctorName = transaction.User1.FullName,
                            MajorDescription = transaction.User1.Major.Name,
                            PatientName = transaction.User.FullName,
                            Sex = (int)transaction.User.Sex,
                            DateOfBirth = transaction.User.DateOfBirth.ToString(),
                            Address = transaction.User.Address,
                            PhoneNumber = transaction.User.PhoneNumber,
                            OrderStatusName = transactionStatus.Name,
                            OrderStatusCode = transactionStatus.Code
                        }
                    ).Where(y=>y.OrderStatusCode.Equals(TransactionStatus)).ToList();
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }

    public class VM_Transaction_CashWithDrawal
    {
        [Required(ErrorMessage = "Vui lòng nhập tên chủ thẻ")]
        public string Card_FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số tài khoản")]
        public string Card_Number { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập thông tin ngân hàng.")]
        public string Card_Bank { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số tiền cần giao dịch")]
        [DataType(DataType.Currency, ErrorMessage = "Số tiền phải lơn hơn 0 và ko có chứa ký tự alphabet. Vui lòng kiểm tra lại.")]
        public decimal Total { get; set; }

        public string Note { get; set; }

        public VM_Transaction_CashWithDrawal()
        {
            FL_DoctorEntities _context = new FL_DoctorEntities();
            decimal total = 0;
            decimal.TryParse(_context.Resources.FirstOrDefault(x => x.Code.Equals("global_card_fee")).Value, out total);
            this.Card_FullName = _context.Resources.FirstOrDefault(x => x.Code.Equals("global_card_fullname")).Value;
            this.Card_Number = _context.Resources.FirstOrDefault(x => x.Code.Equals("global_card_number")).Value;
            this.Card_Bank = _context.Resources.FirstOrDefault(x => x.Code.Equals("global_card_bank")).Value;
            this.Note = _context.Resources.FirstOrDefault(x => x.Code.Equals("global_card_note")).Value;
            this.Total = total;
        }
        }
    public class VN_Response_CashWithDrawal : VM_Transaction_CashWithDrawal
    {
        public string OrderNumber { get; set; }
        public string DisplayTotal { get; set; }
    }
}