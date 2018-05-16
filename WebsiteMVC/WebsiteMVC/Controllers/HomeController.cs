using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteMVC.Models;

namespace WebsiteMVC.Controllers
{
    public class HomeController : BaseController
    {
        QLTCEntities db = new QLTCEntities();
        public ActionResult Index()
        {
            var lst = new List<Notify>();
            if (Account.POSITION != "ADMIN") return View(lst);

            lst.AddRange(from x in db.ThuChis.Where(q => q.State == Notify.INIT).AsEnumerable()
                         select new Notify
                         {
                             CreateBy = x.TaiKhoan,
                             CreateTime = x.CreateTime,
                             ID = x.IDThuChi,
                             Message = $"Thêm mới [#{x.MaThuChi}], mục đích: {x.MucDich}",
                             State = Notify.INIT,
                             Type = "[THU CHI]",
                             ControllerName = "ThuChi"
                         });

            lst.AddRange(from x in db.TamUngs.Where(q => q.State == Notify.INIT).AsEnumerable()
                         select new Notify
                         {
                             CreateBy = x.TaiKhoan,
                             CreateTime = x.CreateTime,
                             ID = x.IDTamUng,
                             Message = $"Thêm mới [#{x.MaTamUng}] với tổng ứng {(x.Total ?? 0).ToString("#,###")}VND",
                             State = Notify.INIT,
                             Type = "[TẠM ỨNG]",
                             ControllerName = "TamUng"
                         });

            lst.AddRange(from x in db.CongNoes.Where(q => q.State == Notify.INIT).AsEnumerable()
                         select new Notify
                         {
                             CreateBy = x.TaiKhoan,
                             CreateTime = x.CreateTime,
                             ID = x.IDCongNo,
                             Message = $"Thêm mới [#{x.MaCongNo}], mục đích: {x.MucDich}",
                             State = Notify.INIT,
                             Type = "[CÔNG NỢ]",
                             ControllerName = "CongNo"
                         });

            lst.AddRange(from x in db.Quys.Where(q => q.State == Notify.INIT).AsEnumerable()
                         select new Notify
                         {
                             CreateBy = x.TaiKhoan,
                             CreateTime = x.CreateTime,
                             ID = x.IDQuy,
                             Message = $"Thêm mới quỹ [#{x.MaQuy}], tài khoản {(x.Money?.ToString("#,###"))}VND",
                             State = Notify.INIT,
                             Type = "[QUỸ]",
                             ControllerName = "Quy"
                         });

            return View(lst.OrderByDescending(q => q.CreateTime));
        }

        public ActionResult Task()
        {
            var lst = new List<Notify>();
            if (Account.POSITION != "ADMIN") return View(lst);

            lst.AddRange(from x in db.CTTamUngs.Where(q => q.State == Notify.INIT).AsEnumerable()
                         select new Notify
                         {
                             CreateTime = x.CreateTime,
                             ID = x.IDCTTamUng,
                             Message = $"[#{x.TamUng.MaTamUng}] đã ứng thêm {(x.SoTien?.ToString("#,###"))} VND",
                             State = Notify.INIT,
                             Type = "[TẠM ỨNG]",
                             ControllerName = "CTTamUng"
                         });

            lst.AddRange(from x in db.ThanhToans.Where(q => q.State == Notify.INIT).AsEnumerable()
                         select new Notify
                         {
                             CreateTime = x.CreateTime,
                             ID = x.IDThanhToan,
                             Message = $"[#{x.CongNo.MaCongNo}] đã thanh toán {(x.SoTienTra?.ToString("#,###"))}VND",
                             State = Notify.INIT,
                             Type = "[CÔNG NỢ]",
                             ControllerName = "ThanhToan"
                         });

            return View(lst.OrderByDescending(q => q.CreateTime));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}