using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteMVC.Models;

namespace WebsiteMVC.Controllers
{
    public class BaseController : Controller
    {
        public TaiKhoan Account { get; protected set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Account = Models.LoginHelper.GetAccount();
            if (Account == null)
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new
                {
                    Controller = "Login",
                    Action = "Index",
                    Area = ""
                }));
            }
            base.OnActionExecuting(filterContext);
        }

        public ActionResult NotPermistion()
        {
            return View();
        }

        public void UpdateQuy(int IDQuy, decimal? MoneyPlus)
        {
            if (MoneyPlus.HasValue && MoneyPlus != 0)
            {
                using (var db = new QLTCEntities())
                {
                    var quy = db.Quys.FirstOrDefault(q => q.IDQuy == IDQuy);
                    if (quy != null)
                    {
                        quy.Money += MoneyPlus;
                        db.SaveChanges();
                    }
                }
            }
        }
    }


    public class RoleAcceptAttribute : ActionFilterAttribute
    {
        public RoleAcceptAttribute(params eRole[] roles)
        {
            this.Roles = roles;
        }

        public eRole[] Roles { get; protected set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Roles.Length > 0 && filterContext.Controller is BaseController controlerBase)
            {
                if (controlerBase.Account.POSITION != "ADMIN" && Roles.Any(q => q.ToString() == controlerBase.Account.POSITION) == false)
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new
                    {
                        Controller = "Base",
                        Action = "NotPermistion",
                        Area = ""
                    }));
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}