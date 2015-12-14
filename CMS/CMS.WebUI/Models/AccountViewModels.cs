using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CMS.WebUI.Models
{
    public class LoginViewModel
    {
        //[Required]
        //[Display(Name = "电子邮件")]
        //[EmailAddress]
        //public string Email { get; set; }

        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住我?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Display(Name = "住址")]
        public string Address { get; set; }

        [Display(Name = "年龄")]
        public int Age { get; set; }

        [Display(Name = "城市")]
        public string City { get; set; }

        //用户状态：1-使用中  0-冻结中
        [Display(Name = "用户状态")]
        public int UserState { get; set; }


        [Required(ErrorMessage = "用户名不能为空")]
        [Display(Name = "用户名[*]")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "电子邮件[*]")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码[*]")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码[*]")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
    }
}