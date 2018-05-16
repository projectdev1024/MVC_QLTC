using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteMVC.Models;

namespace WebsiteMVC.Controllers
{
    public class ThanhToanController : BaseController
    {
        private QLTCEntities db = new QLTCEntities();

        // GET: ThanhToan
        public ActionResult Index(int id)
        {
            var thanhToans = db.ThanhToans.Where(q => q.IDCongNo == id);
            ViewBag.IDCongNo = id;
            ViewBag.congno = db.CongNoes.Find(id);
            return View(thanhToans.ToList());
        }

        // GET: ThanhToan/Edit/5
        public ActionResult Edit(int? id, int? IDCongNo)
        {
            ThanhToan thanhToan = null;
            if (id.HasValue)
            {
                thanhToan = db.ThanhToans.Find(id);
            }
            else
            {
                thanhToan = new ThanhToan()
                {
                    IDCongNo = IDCongNo,
                    CongNo = db.CongNoes.Find(IDCongNo)
                };
            }
            if (thanhToan == null)
            {
                return HttpNotFound();
            }
            return View(thanhToan);
        }

        // POST: ThanhToan/Edit/5
        [HttpPost]
        public ActionResult Edit(ThanhToan thanhToan)
        {
            if (thanhToan.IDThanhToan > 0)
            {
                db.Entry(thanhToan).State = EntityState.Modified;
            }
            else
            {
                thanhToan.CreateTime = DateTime.Now;
                thanhToan.CreateBy = Account.IDTaiKhoan;
                thanhToan.State = Notify.INIT;
                db.ThanhToans.Add(thanhToan);
            }
            var congno = db.CongNoes.Find(thanhToan.IDCongNo);
            congno.Payed += thanhToan.SoTienTra;
            var phaitra = congno.SoTien * ((decimal)(congno.LaiSuat ?? 0) + 100) / 100;
            if (congno.Payed >= phaitra)
            {
                congno.State = Notify.SUCCESS;
            }
            db.SaveChanges();
            return RedirectToAction("Index", "CongNo", new { thanhToan.IDCongNo });
        }

        public JsonResult Delete(int id)
        {
            ThanhToan thanhToan = db.ThanhToans.Find(id);
            db.ThanhToans.Remove(thanhToan);
            return Json(db.SaveChanges());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult State(int id, byte state)
        {
            db.ThanhToans.Find(id).State = state;
            return Json(db.SaveChanges() > 0);
        }
    }
}
