using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteMVC.Models;

namespace WebsiteMVC.Controllers
{
    public class ThuChiController : BaseController
    {
        private QLTCEntities db = new QLTCEntities();

        public ActionResult Index(string stimeStart, string stimeEnd, string submit)
        {
            DateTime timeEnd = DateTime.Now;
            DateTime timeStart = timeEnd.Subtract(TimeSpan.FromDays(30));
            if (DateTime.TryParse(stimeStart, out timeStart) == false) timeStart = timeEnd.Subtract(TimeSpan.FromDays(30));
            if (DateTime.TryParse(stimeEnd, out timeEnd) == false) timeEnd = DateTime.Now;
            timeEnd = new DateTime(timeEnd.Year, timeEnd.Month, timeEnd.Day, 23, 59, 59);
            ViewBag.timeStart = timeStart.ToString("yyyy-MM-dd");
            ViewBag.timeEnd = timeEnd.ToString("yyyy-MM-dd");

            var data = db.ThuChis.Where(q => q.CreateTime >= timeStart && q.CreateTime <= timeEnd && q.State != Notify.CANCLE).ToList();

            if (submit == "excel")
            {
                using (var stream = new MemoryStream())
                {
                    var wb = new XLWorkbook(Server.MapPath("/Content/Excels/thuchi.xlsx"));
                    string nameFile = string.Format("THU CHI.xlsx");
                    var ws = wb.Worksheets.Worksheet(1);
                    for (int i = 0; i < data.Count; i++)
                    {
                        var item = data[i];
                        int index = 1;
                        ws.Cell(1 + i, index++).Value = item.MaThuChi;
                        ws.Cell(1 + i, index++).Value = item.Quy.MaQuy;
                        ws.Cell(1 + i, index++).Value = item.SoTien;
                        ws.Cell(1 + i, index++).Value = item.CreateTime;
                        ws.Cell(1 + i, index++).Value = item.CreateBy + " " + item.TaiKhoan.FullName;
                    }
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nameFile);
                }
            }

            var gr = data.GroupBy(q => new DateTime(q.CreateTime.Value.Year, q.CreateTime.Value.Month, 1));
            string d = "[";
            bool first = true;
            for (DateTime date = new DateTime(timeStart.Year, timeStart.Month, 1); date <= timeEnd; date = date.AddMonths(1))
            {
                d += $"{(first ? "" : ",")}['{date.ToString("MM/yyyy")}',{gr.FirstOrDefault(q => q.Key == date)?.Where(q => q.MaThuChi.StartsWith("THU_")).Sum(q => q.SoTien) ?? 0},{gr.FirstOrDefault(q => q.Key == date)?.Where(q => q.MaThuChi.StartsWith("THU_") == false).Sum(q => q.SoTien) ?? 0}]";
                first = false;
            }
            d += "]";
            ViewBag.data = d;
            return View(data);
        }

        // GET: ThuChi/Edit/5
        public ActionResult Edit(int? id)
        {
            ThuChi obj = new ThuChi()
            {
                State = Notify.INIT
            };
            if (id > 0)
            {
                obj = db.ThuChis.Find(id);
                obj.Thu = obj.MaThuChi.StartsWith("THU_");
                obj.MaThuChi = obj.MaThuChi.Remove(0, 4);
            }
            ViewBag.IDQuys = db.Quys.Where(q => q.State != Notify.CANCLE).CreateSelectList(q => q.IDQuy, q => q.mName, obj.IDQuy);
            return View(obj);
        }

        // POST: ThuChi/Edit/5
        [HttpPost]
        public ActionResult Edit(ThuChi obj)
        {
            var chane = (obj.Thu ? 1 : -1) * obj.SoTien;
            obj.MaThuChi = $"{(obj.Thu ? "THU" : "CHI")}_{obj.MaThuChi}";
            if (obj.IDThuChi > 0)
            {
                db.Entry(obj).State = EntityState.Modified;
            }
            else
            {
                obj.CreateBy = Account.IDTaiKhoan;
                obj.CreateTime = DateTime.Now;
                obj.State = Notify.INIT;
                db.ThuChis.Add(obj);
            }
            if (db.SaveChanges() > 0)
            {
                UpdateQuy(obj.IDQuy ?? 0, chane);
            }
            return RedirectToAction("Index");
        }

        // POST: ThuChi/Delete/5
        public JsonResult Delete(int id)
        {
            ThuChi obj = db.ThuChis.Find(id);
            db.ThuChis.Remove(obj);
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
            db.ThuChis.Find(id).State = state;
            return Json(db.SaveChanges() > 0);
        }
    }
}
