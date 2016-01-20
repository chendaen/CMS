using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace CMS.WebUI.Models
{
    public class ApplicationUser : IdentityUser
    {        

        [Display(Name ="用户名")]
        public override string UserName { get; set; }

        /// <summary>
        /// 住址
        /// </summary>
        [Display(Name = "住址")]
        public string Address { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        [Display(Name = "年龄")]
        public int Age { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        [Display(Name = "城市")]
        public string City { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        //用户状态：1-使用中  0-冻结中
        [Display(Name = "用户状态")]
        public int UserState { get; set; }

        // 可以通过向 ApplicationUser 类添加更多属性来为用户添加配置文件数据。若要了解详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=317594。
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<CMS.WebUI.Models.UserStates> DbUserStates { get; set; }

        public System.Data.Entity.DbSet<CMS.WebUI.Models.UserCardViewModel> DbUserCard { get; set; }
    }
}