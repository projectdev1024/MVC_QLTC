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
    public class TamUngController : BaseController
    {
        private QLTCEntities db = new QLTCEntities();

        // GET: TamUng
        public ActionResult Index()
        {
            return View(db.TamUngs.ToList());
        }

        // GET: TamUng/Edit/5
        public ActionResult Edit(int? id)
        {
            TamUng obj = new TamUng()
            {
                State = Notify.INIT
            };
            if (id > 0)
            {
                obj = db.TamUngs.Find(id);
            }
            ViewBag.IDQuys = db.Quys.Where(q => q.State != Notify.CANCLE).CreateSelectList(q => q.IDQuy, q => q.mName, obj.IDQuy);
            return View(obj);
        }

        // POST: TamUng/Edit/5
        [HttpPost]
        public ActionResult Edit(TamUng obj)
        {
            var chane = -1 * obj.SoTienDaUng;
            if (obj.IDTamUng > 0)
            {
                db.Entry(obj).State = EntityState.Modified;
            }
            else
            {
                obj.CreateBy = Account.IDTaiKhoan;
                obj.CreateTime = DateTime.Now;
                obj.State = Notify.INIT;
                db.TamUngs.Add(obj);
            }
            if (db.SaveChanges() > 0)
            {
                UpdateQuy(obj.IDQuy ?? 0, chane);
            }
            return RedirectToAction("Index");
        }

        // POST: TamUng/Delete/5
        public JsonResult Delete(int id)
        {
            TamUng obj = db.TamUngs.Find(id);
            db.TamUngs.Remove(obj);
            return Json(db.SaveChanges());
        }

        public JsonResult Success(int id)
        {
            TamUng obj = db.TamUngs.Find(id);
            obj.State = Notify.SUCCESS;
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
            db.TamUngs.Find(id).State = state;
            return Json(db.SaveChanges() > 0);
        }
    }
}
