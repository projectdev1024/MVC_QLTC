using Newtonsoft.Json;
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
    public class QuyController : BaseController
    {
        private QLTCEntities db = new QLTCEntities();

        // GET: Quy
        public ActionResult Index(string stimeStart, string stimeEnd)
        {
            DateTime timeEnd = DateTime.Now;
            DateTime timeStart = timeEnd.Subtract(TimeSpan.FromDays(30));
            if (DateTime.TryParse(stimeStart, out timeStart) == false) timeStart = timeEnd.Subtract(TimeSpan.FromDays(30));
            if (DateTime.TryParse(stimeEnd, out timeEnd) == false) timeEnd = DateTime.Now;
            timeEnd = new DateTime(timeEnd.Year, timeEnd.Month, timeEnd.Day, 23, 59, 59);
            ViewBag.timeStart = timeStart.ToString("yyyy-MM-dd");
            ViewBag.timeEnd = timeEnd.ToString("yyyy-MM-dd");

            var data = new List<TangGiam>();

            data.AddRange((from congno in db.CongNoes.Where(q => q.CreateTime >= timeStart && q.CreateTime <= timeEnd && q.State != Notify.CANCLE)
                           select new TangGiam
                           {
                               Time = congno.CreateTime,
                               Thu = congno.MaCongNo.StartsWith("NO_") ? congno.SoTien : 0,
                               Chi = congno.MaCongNo.StartsWith("CONG_") ? congno.SoTien : 0,
                           }));

            data.AddRange((from thuchi in db.ThuChis.Where(q => q.CreateTime >= timeStart && q.CreateTime <= timeEnd && q.State != Notify.CANCLE)
                           select new TangGiam
                           {
                               Time = thuchi.CreateTime,
                               Thu = thuchi.MaThuChi.StartsWith("THU") ? thuchi.SoTien : 0,
                               Chi = thuchi.MaThuChi.StartsWith("CHI") ? thuchi.SoTien : 0,
                           }
                           ));

            data.AddRange((from thuchi in db.TamUngs.Where(q => q.CreateTime >= timeStart && q.CreateTime <= timeEnd && q.State != Notify.CANCLE)
                           select new TangGiam
                           {
                               Time = thuchi.CreateTime,
                               Thu = 0,
                               Chi = thuchi.SoTienDaUng
                           }));

            var gr = data.GroupBy(q => new DateTime(q.Time.Value.Year, q.Time.Value.Month, 1));
            string d = "[";
            bool first = true;
            for (DateTime date = new DateTime(timeStart.Year, timeStart.Month, 1); date <= timeEnd; date = date.AddMonths(1))
            {
                d += $"{(first ? "" : ",")}['{date.ToString("MM/yyyy")}',{gr.FirstOrDefault(q => q.Key == date)?.Sum(q => q.Thu) ?? 0},{gr.FirstOrDefault(q => q.Key == date)?.Sum(q => q.Chi) ?? 0}]";
                first = false;
            }
            d += "]";
            ViewBag.data = d;
            return View(db.Quys.Where(q => q.State != Notify.CANCLE));
        }

        // GET: Quy/Edit/5
        public ActionResult Edit(int? id)
        {
            Quy obj = new Quy();
            if (id > 0)
            {
                obj = db.Quys.Find(id);
            }
            return View(obj);
        }

        // POST: Quy/Edit/5
        [HttpPost]
        public ActionResult Edit(Quy obj)
        {
            if (obj.IDQuy > 0)
            {
                db.Entry(obj).State = EntityState.Modified;
            }
            else
            {
                obj.State = Notify.INIT;
                obj.CreateBy = Account.IDTaiKhoan;
                obj.CreateTime = DateTime.Now;
                db.Quys.Add(obj);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Quy/Delete/5
        public JsonResult Delete(int id)
        {
            Quy obj = db.Quys.Find(id);
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
            db.Quys.Find(id).State = state;
            return Json(db.SaveChanges() > 0);
        }
    }

    public class TangGiam
    {
        public decimal? Thu { get; set; }
        public decimal? Chi { get; set; }
        public DateTime? Time { get; set; }
    }
}
