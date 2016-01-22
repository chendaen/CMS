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
    public class BusinessController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public BusinessController()
        {
        }

        public BusinessController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Business
        public ActionResult Index()
        {
            var varusers = UserManager.Users.ToList();
            UserManager.Dispose();
            return View(varusers);

            //return View();
        }

        public ActionResult CardList(string UserName)
        {
            if (!string.IsNullOrEmpty(UserName))
            {
                var user = UserManager.FindByName(UserName);
                ViewBag.UserName = UserName;
                string userId = user.Id;

                List<UserCardViewModel> UserCardList = new List<UserCardViewModel>();
                //UserCardViewModel model = new UserCardViewModel();

                ApplicationDbContext db = new ApplicationDbContext();
                var userCardList = db.DbUserCard.ToList();
                foreach(UserCardViewModel UC in userCardList)
                {
                    if(userId==UC.UserID)
                    {
                        UserCardList.Add(UC);
                    }
                }

                
                //model.ID = Guid.NewGuid().ToString().ToUpper();
                //model.CardNo = Guid.NewGuid().ToString("N").ToUpper();
                //model.UserID = user.Id;

                return View(UserCardList);
            }
            else
            {
                return RedirectToAction("UserList", new { Message = "" });
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CardList(UserCardViewModel model)
        //{
        //    List<UserCardViewModel> UserCardList = new List<UserCardViewModel>();

        //    if (ModelState.IsValid)
        //    {
        //        if (model != null)
        //        {
        //            var user = UserManager.FindById(model.UserID);
        //            ViewBag.UserName = user.UserName;

        //            RechargeViewModels recharge = new RechargeViewModels();
        //            recharge.CardNo = model.CardNo;

        //            return View("RechargeCard", recharge);
        //                //return RedirectToAction("UserList", new { Message = MakeCardMessageId.BindingCard });
                    
        //        }
        //    }
        //    return View(model);
        //}

        public ActionResult RechargeCard(string CardNo)
        {
            RechargeEx RE = new RechargeEx();
            RE.CardNo = CardNo;
            

            ApplicationDbContext db = new ApplicationDbContext();
            var RechargeCardList = db.DbCardBalance.ToList();

            //List<RechargeViewModels> RechargeCardList = new List<RechargeViewModels>();

            foreach (CardBalanceViewModels Rm in RechargeCardList)
            {
                if(CardNo==Rm.CardNo)
                {
                    RE.Balance = Rm.Balance;
                }
            }

            //RE.Balance = 300;   余额
            //RE.Money = 100;     金额

            return View(RE);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RechargeCard(RechargeEx model)
        {            
            if (ModelState.IsValid)
            {
                if (model != null)
                {
                    ApplicationDbContext db = new ApplicationDbContext();
                    //检查该卡是否已充过值
                    List<CardBalanceViewModels> CBVMList = new List<CardBalanceViewModels>();
                    CBVMList = db.DbCardBalance.ToList();
                    foreach(CardBalanceViewModels CB in CBVMList)
                    {
                        if(CB.CardNo==model.CardNo)
                        {
                            db.DbCardBalance.Remove(CB);
                        }
                    }

                    //记录卡余额
                    CardBalanceViewModels CBVM = new CardBalanceViewModels();
                    CBVM.Balance = model.Balance + model.Money;
                    CBVM.CardNo = model.CardNo;
                    db.DbCardBalance.Add(CBVM);

                    //记录充值过程
                    RechargeViewModels RVM = new RechargeViewModels();
                    RVM.CardNo = model.CardNo;
                    RVM.DateTime = DateTime.Now;
                    RVM.Money = model.Money;
                    RVM.Operate = "充值";
                    RVM.Operator = User.Identity.GetUserName();

                    db.DbRechargeCard.Add(RVM);
                    var result = db.SaveChanges();
                    if (result >= 1)
                    {
                        model.Balance = CBVM.Balance;
                        return View("RechargeSucceed", model);
                    }
                }
            }
            return View(model);
        }


    }
}