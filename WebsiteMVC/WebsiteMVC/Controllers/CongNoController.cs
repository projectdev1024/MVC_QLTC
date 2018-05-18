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
    public class CongNoController : BaseController
    {
        private QLTCEntities db = new QLTCEntities();

        // GET: CongNo
        public ActionResult Index(string stimeStart, string stimeEnd)
        {
            DateTime timeEnd = DateTime.Now;
            DateTime timeStart = timeEnd.Subtract(TimeSpan.FromDays(30));
            if (DateTime.TryParse(stimeStart, out timeStart) == false) timeStart = timeEnd.Subtract(TimeSpan.FromDays(30));
            if (DateTime.TryParse(stimeEnd, out timeEnd) == false) timeEnd = DateTime.Now;
            timeEnd = new DateTime(timeEnd.Year, timeEnd.Month, timeEnd.Day, 23, 59, 59);
            ViewBag.timeStart = timeStart.ToString("yyyy-MM-dd");
            ViewBag.timeEnd = timeEnd.ToString("yyyy-MM-dd");

            var data = db.CongNoes.Where(q => q.CreateTime >= timeStart && q.CreateTime <= timeEnd && q.State != Notify.CANCLE).ToList();

            var gr = data.GroupBy(q => new DateTime(q.CreateTime.Value.Year, q.CreateTime.Value.Month, 1));
            string d = "[";
            bool first = true;
            for (DateTime date = new DateTime(timeStart.Year, timeStart.Month, 1); date <= timeEnd; date = date.AddMonths(1))
            {
                d += $"{(first ? "" : ",")}['{date.ToString("MM/yyyy")}',{gr.FirstOrDefault(q => q.Key == date)?.Sum(q => q.ConNo) ?? 0},{gr.FirstOrDefault(q => q.Key == date)?.Sum(q => q.Payed) ?? 0}]";
                first = false;
            }
            d += "]";
            ViewBag.data = d;
            return View(data);
        }

        // GET: CongNo/Edit/5
        public ActionResult Edit(int? id)
        {
            CongNo obj = new CongNo()
            {
                State = Notify.INIT
            };
            if (id > 0)
            {
                obj = db.CongNoes.Find(id);
                obj.Cong = obj.MaCongNo.StartsWith("CONG_");
                obj.MaCongNo = obj.MaCongNo.StartsWith("NO_") ? obj.MaCongNo.Remove(0, "NO_".Length) : (obj.MaCongNo.StartsWith("CONG_") ? obj.MaCongNo.Remove(0, "CONG_".Length) : obj.MaCongNo);
            }
            ViewBag.IDQuys = db.Quys.Where(q => q.State != Notify.CANCLE).CreateSelectList(q => q.IDQuy, q => q.mName, obj.IDQuy);
            return View(obj);
        }

        // POST: CongNo/Edit/5
        [HttpPost]
        public ActionResult Edit(CongNo obj)
        {
            obj.MaCongNo = $"{ (obj.Cong ? "CONG" : "NO")}_{obj.MaCongNo}";
            if (obj.IDCongNo > 0)
            {
                db.Entry(obj).State = EntityState.Modified;
            }
            else
            {
                obj.State = Notify.INIT;
                obj.CreateBy = Account.IDTaiKhoan;
                obj.CreateTime = DateTime.Now;
                obj.ConNo = obj.Tra1Ngay * (obj.NgayTra - obj.CreateTime).Value.Days ?? 0 ;
                obj.Payed = 0;
                db.CongNoes.Add(obj);
            }

            if (db.SaveChanges() > 0)
            {
                UpdateQuy(obj.IDQuy ?? 0, (obj.Cong ? -1 : 1) * obj.SoTien);
            }
            return RedirectToAction("Index");
        }

        // POST: CongNo/Delete/5
        public JsonResult Delete(int id)
        {
            CongNo obj = db.CongNoes.Find(id);
            obj.State = Notify.CANCLE;
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
            db.CongNoes.Find(id).State = state;
            return Json(db.SaveChanges() > 0);
        }
    }
}
