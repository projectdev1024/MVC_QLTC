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
    public class CTTamUngController : BaseController
    {
        private QLTCEntities db = new QLTCEntities();

        // GET: CTTamUng
        public ActionResult Index(int? IDTamUng)
        {
            var cTTamUngs = db.CTTamUngs.Where(q => q.IDTamUng == IDTamUng);
            ViewBag.IDTamUng = IDTamUng;
            ViewBag.ToAdd = db.TamUngs.Find(IDTamUng)?.State != Notify.SUCCESS;
            return View(cTTamUngs.ToList());
        }

        public ActionResult Edit(int? id, int? IDTamUng)
        {
            CTTamUng obj = null;
            if (id > 0)
            {
                obj = db.CTTamUngs.Find(id);
            }
            else
            {
                obj = new CTTamUng
                {
                    IDTamUng = IDTamUng,
                    TamUng = db.TamUngs.Find(IDTamUng)
                };
            }
            if (obj == null)
            {
                return HttpNotFound();
            }
            return View(obj);
        }

        // POST: CTTamUng/Edit/5
        [HttpPost]
        public ActionResult Edit(CTTamUng obj)
        {
            if (obj.IDCTTamUng > 0)
            {
                db.Entry(obj).State = EntityState.Modified;
            }
            else
            {
                obj.CreateBy = Account.IDTaiKhoan;
                obj.CreateTime = DateTime.Now;
                obj.State = Notify.INIT;
                db.CTTamUngs.Add(obj);
            }
            db.SaveChanges();
            return RedirectToAction("Index", "TamUng", new { id = obj.IDTamUng });
        }

        public JsonResult Delete(int id)
        {
            CTTamUng cTTamUng = db.CTTamUngs.Find(id);
            db.CTTamUngs.Remove(cTTamUng);
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
            db.CTTamUngs.Find(id).State = state;
            return Json(db.SaveChanges() > 0);
        }
    }
}
