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
    public class TaiKhoanController : BaseController
    {
        private QLTCEntities db = new QLTCEntities();

        // GET: TaiKhoan
        [RoleAccept(eRole.ADMIN)]
        public ActionResult Index()
        {
            return View(db.TaiKhoans.ToList().Where(q => q.Active != false).ToList());
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string oldpass, string newpass, string repass)
        {
            if (newpass != repass)
            {
                ModelState.AddModelError("", "Xác nhận mật khẩu không chính xác.");
            }
            if (newpass.Length < 8)
            {
                ModelState.AddModelError("", "Nhập mật khẩu có ít nhất 08 tí tự.");
            }
            if (oldpass.Length < 0)
            {
                ModelState.AddModelError("", "Vui lòng nhập mật khẩu cũ.");
            }
            if (ModelState.ContainsKey("") == false)
            {
                var db = new Models.QLTCEntities();
                var acc = db.TaiKhoans.FirstOrDefault(q => q.IDTaiKhoan == Account.IDTaiKhoan);
                if (acc.Password == oldpass)
                {
                    acc.Password = repass;
                    db.SaveChanges();
                    return RedirectToAction("Logout", "Login", new { area = "" });
                }
                ModelState.AddModelError("", "Mật khẩu cũ không chính xác");
            }
            return View();
        }

        // GET: TaiKhoan/Edit/5
        public ActionResult Edit(int? id)
        {
            TaiKhoan obj = new TaiKhoan();
            if (id > 0)
            {
                obj = db.TaiKhoans.Find(id);
                if (obj == null)
                {
                    return HttpNotFound();
                }
            }
            return View(obj);
        }

        // POST: TaiKhoan/Edit/5
        [HttpPost]
        public ActionResult Edit(TaiKhoan obj)
        {
            if (ModelState.IsValid)
            {
                var res = this.RequestAndSaveImage();
                if (res.Key)
                {
                    obj.Avatar = res.Value;
                }

                if (obj.IDTaiKhoan > 0)
                {
                    db.Entry(obj).State = EntityState.Modified;
                }
                else
                {
                    db.TaiKhoans.Add(obj);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        [RoleAccept(eRole.ADMIN)]
        public JsonResult Delete(int? id)
        {
            TaiKhoan obj = db.TaiKhoans.Find(id);
            //db.TaiKhoans.Remove(obj);
            obj.Active = false;
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
    }
}
