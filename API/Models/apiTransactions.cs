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