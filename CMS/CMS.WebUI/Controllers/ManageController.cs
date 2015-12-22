using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using CMS.WebUI.Models;

namespace CMS.WebUI.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "已更改你的密码。"
                : message == ManageMessageId.SetPasswordSuccess ? "已设置你的密码。"
                : message == ManageMessageId.SetTwoFactorSuccess ? "已设置你的双重身份验证提供程序。"
                : message == ManageMessageId.Error ? "出现错误。"
                : message == ManageMessageId.AddPhoneSuccess ? "已添加你的电话号码。"
                : message == ManageMessageId.RemovePhoneSuccess ? "已删除你的电话号码。"
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            return View(model);
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.OldPassword.Equals(model.NewPassword))
            {
                ModelState.AddModelError(string.Empty, "新密码不能和原密码一致");
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        #region 用户管理
        /// <summary>
        /// 用户管理列表
        /// </summary>
        /// <returns></returns>
        public ActionResult UserList()
        {
            var varusers = UserManager.Users.ToList();
            UserManager.Dispose();
            return View(varusers);

            //var user = UserManager.Users.First();
            //user.Age = 11;
            //UserManager.Update(user);
            //return View(user);
        }

        public ActionResult UserEdit(string UserName)
        {
            if(!string.IsNullOrEmpty(UserName))
            { 
            var user = UserManager.FindByName(UserName);
            return View(user);
            }else
            {
                return RedirectToAction("UserList", new { Message = ManageMessageId.UpdateUserInfo });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserEdit(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                if (model != null)
                {
                    //重置密码
                    //string token = UserManager.GeneratePasswordResetToken(model.Id);
                    //UserManager.ResetPassword(model.Id, token, "Allegion@123");

                    //判断需要修改的字段
                    if (TryUpdateModel(model, new string[] { "Address", "Age", "City", "Email", "UserName" }))
                    {
                        //UserManage里已经存了ApplicationUser实例(UserList方法) 这里再Update另一个实例会有问题，需要赋值给里边，再Update里边的实例。----个人理解
                        var usermodel = UserManager.FindById(model.Id);
                        usermodel.Address = model.Address;
                        usermodel.Age = model.Age;
                        usermodel.City = model.City;
                        usermodel.Email = model.Email;
                        usermodel.UserName = model.UserName;
                        //Update
                        var result = UserManager.Update(usermodel);
                        if(result.Succeeded)
                        {
                            return RedirectToAction("UserList", new { Message = ManageMessageId.UpdateUserInfo });
                        }
                    }
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        public ActionResult UserCreate()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserCreate([Bind(Exclude = "age,Email")]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName };

                if (!string.IsNullOrEmpty(model.Address))
                {
                    user.Address = model.Address;
                }
                if (!string.IsNullOrEmpty(model.Age.ToString()))
                {
                    user.Age = model.Age;
                }
                if (!string.IsNullOrEmpty(model.City))
                {
                    user.City = model.City;
                }
                if (!string.IsNullOrEmpty(model.Email))
                {
                    user.Email = model.Email;
                }
                if (!string.IsNullOrEmpty(model.UserState.ToString()))
                {
                    user.UserState = model.UserState;
                }

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    
                    return RedirectToAction("UserList", new { Message = ManageMessageId.CreateUserInfo });
                }
                AddErrors(result);
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        public ActionResult ResetPassword(string ID)
        {
            if (!string.IsNullOrEmpty(ID))
            {
                var user = UserManager.FindById(ID);
                ResetPasswordViewModel re = new ResetPasswordViewModel();
                re.ID = user.Id;
                re.NewPassword = "";
                re.ConfirmPassword = "";
                return View(re);
            }else
            {
                return RedirectToAction("UserList", new { Message = ManageMessageId.ResetPassword });
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model != null)
                {
                    //重置密码
                    string token = UserManager.GeneratePasswordResetToken(model.ID);
                    var result = UserManager.ResetPassword(model.ID, token, model.NewPassword);
                    if (result.Succeeded)
                    {
                        //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        return RedirectToAction("UserList", new { Message = ManageMessageId.ResetPassword });
                    }
                    AddErrors(result);
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        public ActionResult UserDelete(string ID)
        {
            if (!string.IsNullOrEmpty(ID))
            {
                var user = UserManager.FindById(ID);
                return View(user);
            }else
            {
                return RedirectToAction("UserList", new { Message = ManageMessageId.DeleteUserInfo });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserDelete(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                if (model != null)
                {
                    var user = UserManager.FindById(model.Id);
                    var result = UserManager.Delete(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("UserList", new { Message = ManageMessageId.DeleteUserInfo });
                    }                    
                }
            }
            
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        public ActionResult UserSuspend(string ID)
        {
            if (!string.IsNullOrEmpty(ID))
            {
                ApplicationDbContext db = new ApplicationDbContext();

                var usermodel = UserManager.FindById(ID);
                ViewBag.UserStates = new SelectList(db.DbUserStates, "StateID", "StateName", usermodel.UserState);

                return View(usermodel);
            }else
            {
                return RedirectToAction("UserList", new { Message = ManageMessageId.SuspendUserInfo });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserSuspend(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                if (model != null)
                {
                    //重置密码
                    //string token = UserManager.GeneratePasswordResetToken(model.Id);
                    //UserManager.ResetPassword(model.Id, token, "Allegion@123");

                    //判断需要修改的字段
                    if (TryUpdateModel(model, new string[] { "UserState"}))
                    {
                        //UserManage里已经存了ApplicationUser实例(UserList方法) 这里再Update另一个实例会有问题，需要赋值给里边，再Update里边的实例。----个人理解
                        var usermodel = UserManager.FindById(model.Id);
                        usermodel.UserState = model.UserState;
                        
                        //Update
                        var result = UserManager.Update(usermodel);
                        if (result.Succeeded)
                        {
                            return RedirectToAction("UserList", new { Message = ManageMessageId.SuspendUserInfo });
                        }
                    }
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        #endregion

        #region 帮助程序
               

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }
        public enum ManageMessageId
        {        
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error,
                /// <summary>
                /// 更新用户信息
                /// </summary>
            UpdateUserInfo,
            CreateUserInfo,
            ResetPassword,
            DeleteUserInfo,
            SuspendUserInfo

        }
        #endregion

    }
}