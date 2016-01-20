using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using CMS.WebUI.Models;
using System.Data;
using System.Data.OleDb;
using System.Xml;
using System.IO;

namespace CMS.WebUI.Controllers
{
    [Authorize(Roles ="制卡员")]
    public class MakeCardController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public MakeCardController()
        {
        }

        public MakeCardController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: MakeCard
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserList()
        {
            var varusers = UserManager.Users.ToList();
            UserManager.Dispose();
            return View(varusers);

        }

        public ActionResult BindCardList(string UserID)
        {
            if (!string.IsNullOrEmpty(UserID))
            {
                var UserName = UserManager.FindById(UserID).UserName;
                ViewBag.UserName = UserName;

                ApplicationDbContext db = new ApplicationDbContext();
                var bindCardList= db.DbUserCard.ToList();

                List<UserCardViewModel> UserCardList = new List<UserCardViewModel>();

                foreach(UserCardViewModel UC in bindCardList)
                {
                    if (UC.UserID == UserID)
                        UserCardList.Add(UC);
                }

                return View(UserCardList);
            }
            else
            {
                return RedirectToAction("UserList", new { Message = MakeCardMessageId.BindingCard });
            }
        }

        public ActionResult UnBundCard(string ID)
        {
            string UserID = string.Empty;
            List<UserCardViewModel> UserCardList = new List<UserCardViewModel>();

            if (!string.IsNullOrEmpty(ID))
            {
                ApplicationDbContext db = new ApplicationDbContext();
                var bindCardList = db.DbUserCard.ToList();

                foreach (UserCardViewModel UC in bindCardList)
                {
                    UserID = UC.UserID;
                   
                    if (UC.ID == ID)
                    {
                        db.DbUserCard.Remove(UC);
                        var iResult= db.SaveChanges();
                        //if (iResult >= 1)
                        //{
                        //    return RedirectToAction("BindCardList", "MakeCard", UserID);                            
                        //}
                    }
                }

                var UserName = UserManager.FindById(UserID).UserName;
                ViewBag.UserName = UserName;

                bindCardList = db.DbUserCard.ToList();

                foreach (UserCardViewModel UCN in bindCardList)
                {
                    if (UCN.UserID == UserID)
                        UserCardList.Add(UCN);
                }
            }
            return View("BindCardList",UserCardList);
        }

        public ActionResult BindingCard(string UserName)
        {
            if (!string.IsNullOrEmpty(UserName))
            {
                var user = UserManager.FindByName(UserName);
                ViewBag.UserName = UserName;

                UserCardViewModel model = new UserCardViewModel();
                model.ID = Guid.NewGuid().ToString().ToUpper();
                model.CardNo = Guid.NewGuid().ToString("N").ToUpper();
                model.UserID = user.Id;

                return View(model);
            }
            else
            {
                return RedirectToAction("UserList", new { Message = MakeCardMessageId.BindingCard });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BindingCard(UserCardViewModel model)
        {
            List<UserCardViewModel> UserCardList = new List<UserCardViewModel>();

            if (ModelState.IsValid)
            {
                if (model != null)
                {
                    var user = UserManager.FindById(model.UserID);
                    ViewBag.UserName = user.UserName;

                    ApplicationDbContext db = new ApplicationDbContext();
                    db.DbUserCard.Add(model);
                    var result = db.SaveChanges();
                    if (result == 1)
                    {
                        var bindCardList = db.DbUserCard.ToList();
                        foreach (UserCardViewModel UCN in bindCardList)
                        {
                            if (UCN.UserID == model.UserID)
                                UserCardList.Add(UCN);
                        }
                        return View("BindCardList", UserCardList);
                        //return RedirectToAction("UserList", new { Message = MakeCardMessageId.BindingCard });
                    }
                }
            }
            return View(model);
        }

        public enum MakeCardMessageId
        {
            BindingCard,
            UnbUnbundlingCard
        }
    }
}