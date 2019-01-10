using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class TransactionsController : BaseApiController
    {
        VM_Transaction_Respone res = new VM_Transaction_Respone();

        /// <summary>
        /// Lấy danh sách toàn bộ đơn hàng
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Order/{token}")]
        public IHttpActionResult GetListOrder(string token)
        {
            try
            {
                if (!checkAuth(token))
                {
                    return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
                }
                if (res.CheckTransaction(token)!=true)
                {
                    return Content(HttpStatusCode.BadRequest, response.BadRequest("Tài khoản không có giao dịch hoặc không đúng."));
                }
                return Content(HttpStatusCode.OK, response.Ok(res.ListResponseWithType(token, "order"), "Lấy danh sách giao dịch thành công."));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
            }
        }

        /// <summary>
        /// Lấy danh sách đơn hàng đã được xác nhận
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Order/GetListOrderConfirm/{token}")]
        public IHttpActionResult GetListOrderConfirm(string token)
        {
            try
            {
                if (!checkAuth(token))
                {
                    return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
                }
                if (res.CheckTransaction(token) != true)
                {
                    return Content(HttpStatusCode.BadRequest, response.BadRequest("Tài khoản không có giao dịch hoặc không đúng."));
                }
                return Content(HttpStatusCode.OK, response.Ok(res.ListResponseWithTypeAndStatus(token, "order", "success"), "Lấy danh sách giao dịch đã xác nhận thành công."));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
            }
        }

        /// <summary>
        /// Lấy danh sách đơn hàng hủy
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Order/GetListOrderCancel/{token}")]
        public IHttpActionResult GetListOrderCancel(string token)
        {
            try
            {
                if (!checkAuth(token))
                {
                    return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
                }

                if (res.CheckTransaction(token) != true)
                {
                    return Content(HttpStatusCode.BadRequest, response.BadRequest("Tài khoản không có giao dịch hoặc không đúng."));
                }
                return Content(HttpStatusCode.OK, response.Ok(res.ListResponseWithTypeAndStatus(token, "order", "cancel"), "Lấy danh sách giao dịch đã xác nhận thành công."));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
            }
        }

        /// <summary>
        /// Lấy danh sách đơn hàng chưa được xác nhận
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Order/GetListOrderNotConfirm/{token}")]
        public IHttpActionResult GetListOrderNotConfirm(string token)
        {
            try
            {
                if (!checkAuth(token))
                {
                    return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
                }
                if (res.CheckTransaction(token) != true)
                {
                    return Content(HttpStatusCode.BadRequest, response.BadRequest("Tài khoản không có giao dịch hoặc không đúng."));
                }
                return Content(HttpStatusCode.OK, response.Ok(res.ListResponseWithTypeAndStatus(token, "order", "order_create"), "Lấy danh sách giao dịch chưa xác nhận thành công."));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết đơn hàng
        /// </summary>
        /// <param name="OrderNumber"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Order/{OrderNumber}/{token}")]
        public IHttpActionResult GetOrder(string TransactionCode, string token)
        {
            try
            {
                if (!checkAuth(token))
                {
                    return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
                }

                if (res.CheckTransaction(token) != true)
                {
                    return Content(HttpStatusCode.BadRequest, response.BadRequest("Tài khoản không có giao dịch hoặc không đúng."));
                }
                return Content(HttpStatusCode.OK, response.Ok(res.SingleResponse(token, TransactionCode), "Lấy thông tin giao dịch thành công."));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
            }
        }

        /// <summary>
        /// Tạo mới đơn hàng
        /// </summary>
        /// <param name="req"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Order/{token}")]
        public IHttpActionResult Order(VM_Transaction req, string token)
        {
            try
            {
                if (!checkAuth(token))
                {
                    return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
                }
                var transactionCode = req.Create(token);
                if (string.IsNullOrEmpty(transactionCode))
                {
                    return Content(HttpStatusCode.OK, response.Ok(null, "Giao dịch thất bại. Vui lòng kiểm tra lại.", false));
                }
                return Content(HttpStatusCode.OK, response.Ok(res.SingleResponse(token, transactionCode), "Giao dịch thực hiện thành công."));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
            }
        }

        ///// <summary>
        ///// Xác nhận đơn hàng
        ///// </summary>
        ///// <param name="OrderNumber"></param>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //[HttpPut]
        //[Route("Order/Confirm/{OrderNumber}/{token}")]
        //public IHttpActionResult ConfirmOrder(string OrderNumber, string token)
        //{
        //    try
        //    {

        //        if (!checkAuth(token))
        //        {
        //            return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
        //        }
        //        var idAccount = _context.Accounts.SingleOrDefault(x => x.TokenLogin.Equals(token)).ID;
        //        var order = _context.Orders.Where(x => (int)x.idReceive == idAccount && x.idOrderType == 1 && x.code.Equals(OrderNumber)).SingleOrDefault();
        //        var tmp_dateofbirth = ((DateTime)order.dateCreate).ToString("dd/MM/yyyy");
        //        var tmp_datecreate = ((DateTime)order.Account.BirthDay).ToString("dd/MM/yyyy");
        //        var res = _context.Orders.Where(x => (int)x.idReceive == idAccount && x.idOrderType == 1 && x.code.Equals(OrderNumber)).Select(x => new VM_Order_Respone
        //        {
        //            OrderNumber = x.code,
        //            DateCreate = tmp_datecreate,
        //            DoctorName = x.Account1.FullName,
        //            MajorDescription = x.Account1.Product.shortDesc,
        //            PatientName = x.Account.FullName,
        //            Sex = (int)x.Account.Sex,
        //            DateOfBirth = tmp_dateofbirth,
        //            Address = x.Account.Address,
        //            PhoneNumber = x.Account.PhoneNumber
        //        }).SingleOrDefault();
        //        if (order.OrderStatus.Count() > 1)
        //        {
        //            return Ok(responseSingle.Ok(res, "Xác nhận đơn hàng không thành công. Vui lòng kiểm tra lại"));
        //        }
        //        var order_status = _context.OrderStatus.SingleOrDefault(x => x.code.Contains("order_confirm"));
        //        order.OrderStatus.Add(order_status);
        //        order.idOrderStatus = order_status.id;
        //        _context.SaveChanges();

        //        var tokenPatient = _context.Accounts.SingleOrDefault(x => x.ID == order.idBuyer).DeviceToken;
        //        var tokenDoctor = _context.Accounts.SingleOrDefault(x => x.ID == order.idReceive).DeviceToken;
        //        CMS_Lib.PushNotify(tokenPatient, "PCare", "Lịch hẹn " + order.code + " đã được xác nhận thành công.", "patient");
        //        //CMS_Lib.PushNotify(tokenDoctor, "PCare", "Lịch hẹn " + order.code + " đã được xác nhận thành công.", "doctor");
        //        return Ok(responseSingle.Ok(res, "Xác nhận đơn hàng thành công."));
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
        //    }
        //}

        ///// <summary>
        ///// Hủy đơn hàng
        ///// </summary>
        ///// <param name="OrderNumber"></param>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //[HttpPut]
        //[Route("Order/Cancel/{OrderNumber}/{token}")]
        //public IHttpActionResult CancelOrder(string OrderNumber, string token)
        //{
        //    try
        //    {
        //        if (!checkAuth(token))
        //        {
        //            return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
        //        }
        //        var acc = _context.Accounts.SingleOrDefault(x => x.TokenLogin.Equals(token));
        //        Order order = new Order();
        //        if (acc.Group.Code.Equals("doctor"))
        //        {
        //            order = _context.Orders.FirstOrDefault(x => x.idReceive == acc.ID && x.idOrderType == 1 && x.code.Equals(OrderNumber));

        //        }
        //        if (acc.Group.Code.Equals("patient"))
        //        {
        //            order = _context.Orders.FirstOrDefault(x => x.idBuyer == acc.ID && x.idOrderType == 1 && x.code.Equals(OrderNumber));
        //        }
        //        var tmp_dateofbirth = ((DateTime)order.dateCreate).ToString("dd/MM/yyyy");
        //        var tmp_datecreate = ((DateTime)order.Account.BirthDay).ToString("dd/MM/yyyy");
        //        var res = _context.Orders.Where(x => x.idReceive == acc.ID && x.idOrderType == 1 && x.code.Equals(OrderNumber)).Select(x => new VM_Order_Respone
        //        {
        //            OrderNumber = x.code,
        //            DateCreate = tmp_datecreate,
        //            DoctorName = x.Account1.FullName,
        //            MajorDescription = x.Account1.Product.shortDesc,
        //            PatientName = x.Account.FullName,
        //            Sex = (int)x.Account.Sex,
        //            DateOfBirth = tmp_dateofbirth,
        //            Address = x.Account.Address,
        //            PhoneNumber = x.Account.PhoneNumber
        //        }).SingleOrDefault();
        //        if (order.OrderStatus.Count() > 1)
        //        {
        //            return Ok(responseSingle.Ok(res, "Hủy đơn hàng không thành công. Xin vui lòng kiểm tra lại.", false));
        //        }
        //        //check time ko cho hủy đơn hàng
        //        int timeout_cancel_order = 0 - int.Parse(CMS_Lib.Resource("global_timeout_cancel_order"));
        //        if (DateTime.Compare(DateTime.Now.AddMinutes(timeout_cancel_order), (DateTime)order.dateCreate) >= 0)
        //        {
        //            return Ok(responseSingle.Ok(res, "Hủy đơn hàng không thành công. Quá thời gia hủy đơn hàng.", false));
        //        }
        //        var order_status = _context.OrderStatus.SingleOrDefault(x => x.code.Equals("order_cancel"));
        //        order.OrderStatus.Add(order_status);
        //        order.idOrderStatus = order_status.id;
        //        //hoàn tiền lại cho bác sỹ
        //        var account = _context.Accounts.SingleOrDefault(x => x.ID == order.idReceive);
        //        account.Balance = account.Balance + order.totalPay;
        //        _context.SaveChanges();
        //        if (acc.GroupId == 1)
        //        {
        //            var tokenPatient = _context.Accounts.SingleOrDefault(x => x.ID == order.idBuyer).DeviceToken;
        //            CMS_Lib.PushNotify(tokenPatient, "PCare", "Lịch hẹn " + order.code + " đã được hủy", "patient");
        //        }
        //        if (acc.GroupId == 2)
        //        {
        //            var tokenDoctor = _context.Accounts.SingleOrDefault(x => x.ID == order.idReceive).DeviceToken;
        //            CMS_Lib.PushNotify(tokenDoctor, "PCare", "Lịch hẹn " + order.code + " đã được hủy", "doctor");
        //        }


        //        return Ok(responseSingle.Ok(res, "Hủy đơn hàng thành công."));
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
        //    }
        //}

        ///// <summary>
        ///// Giao dịch rút tiền
        ///// </summary>
        ///// <param name="req"></param>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("Order/CashWithDrawal/{token}")]
        //public IHttpActionResult CashWithDrawal(VM_CashWithDrawal req, string token)
        //{
        //    try
        //    {
        //        Response<VN_Response_CashWithDrawal> response = new Response<VN_Response_CashWithDrawal>();
        //        if (!checkAuth(token))
        //        {
        //            return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            return Ok(response.BadRequest(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
        //        }
        //        var account = _context.Accounts.SingleOrDefault(x => x.TokenLogin.Equals(token));
        //        if (account.Group.Code.Equals("doctor"))
        //        {
        //            var order = req.ConvertModel();
        //            order.idBuyer = account.ID;
        //            order.idReceive = 1;
        //            order.idOrderType = 2;
        //            if (order.totalPay <= 0)
        //            {
        //                return Ok(response.BadRequest("Số tiền rút phải lớn hơn 0. Xin vui lòng kiểm tra lại"));
        //            }
        //            if (order.totalPay > account.Balance)
        //            {
        //                return Ok(response.BadRequest("Tài khoản không đủ số dư để rút tiền. Xin vui lòng kiểm tra lại"));
        //            }
        //            _context.Orders.Add(order);
        //            account.Balance = account.Balance - order.totalPay;
        //            var orderStatus = _context.OrderStatus.SingleOrDefault(x => x.code.Contains("cashwithdrawal_create"));
        //            order.OrderStatus.Add(orderStatus);
        //            _context.SaveChanges();
        //            order.code = CMS_Security.createTransactionIDString(order.id);
        //            _context.SaveChanges();
        //            var displayTotalPay = string.Format("{0:0,000 vnd}", order.totalPay);
        //            var res = _context.Orders.Where(x => x.code.Equals(order.code)).Select(x => new VN_Response_CashWithDrawal
        //            {
        //                Card_Bank = x.card_bank,
        //                Card_FullName = x.card_fullname,
        //                Card_Number = x.card_number,
        //                Total = (decimal)x.total,
        //                DisplayTotal = displayTotalPay,
        //                Note = x.note,
        //                OrderNumber = x.code
        //            }).SingleOrDefault();

        //            return Ok(response.Ok(res, "Lệnh rút tiền đã được thực hiện. Xin cám ơn."));
        //        }
        //        return Ok(response.BadRequest("Tài khoản của bạn không thỏa điều kiện rút tiền. Xin vui lòng kiểm tra lại."));
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
        //    }
        //}

        ///// <summary>
        ///// Giao dịch nạp tiền
        ///// </summary>
        ///// <param name="req"></param>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("Order/Recharge/{token}")]
        //public IHttpActionResult Recharge(string token)
        //{
        //    try
        //    {
        //        Response<VN_Response_CashWithDrawal> response = new Response<VN_Response_CashWithDrawal>();
        //        if (!checkAuth(token))
        //        {
        //            return Content(HttpStatusCode.Unauthorized, response.UnAuthorize("Tài khoản không đúng hoặc không có quyền truy cập. Vui lòng kiểm tra lại."));
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            return Ok(response.BadRequest(string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))));
        //        }
        //        var account = _context.Accounts.SingleOrDefault(x => x.TokenLogin.Equals(token));
        //        if (account.Group.Code.Equals("doctor"))
        //        {
        //            var req = new VM_CashWithDrawal();
        //            var order = req.ConvertModel();
        //            order.idOrderType = 3;
        //            order.idBuyer = account.ID;
        //            order.idReceive = 1;
        //            order.idProduct = null;
        //            if (order.totalPay <= 0)
        //            {
        //                return Ok(response.BadRequest("Số tiền nạp phải lớn hơn 0. Xin vui lòng kiểm tra lại"));
        //            }
        //            _context.Orders.Add(order);
        //            var orderStatus = _context.OrderStatus.SingleOrDefault(x => x.code.Contains("recharge_create"));
        //            order.OrderStatus.Add(orderStatus);
        //            _context.SaveChanges();
        //            order.code = CMS_Security.createTransactionIDString(order.id);
        //            _context.SaveChanges();

        //            var displayTotalPay = string.Format("{0:0,000 vnd}", order.totalPay);
        //            var res = _context.Orders.Where(x => x.code.Equals(order.code)).Select(x => new VN_Response_CashWithDrawal
        //            {
        //                Card_Bank = x.card_bank,
        //                Card_FullName = x.card_fullname,
        //                Card_Number = x.card_number,
        //                Total = (decimal)x.total,
        //                DisplayTotal = displayTotalPay,
        //                Note = x.note,
        //                OrderNumber = x.code
        //            }).SingleOrDefault();

        //            return Ok(response.Ok(res, "Lệnh nạp tiền đã được thực hiện. Xin cám ơn."));
        //        }
        //        return Ok(response.BadRequest("Tài khoản của bạn không thỏa điều kiện nạp tiền. Xin vui lòng kiểm tra lại."));
        //    }
        //    catch (Exception e)
        //    {
        //        return Content(HttpStatusCode.BadRequest, response.BadRequest("Có lỗi trong quá trình xử lý."));
        //    }
        //}
    }
}
