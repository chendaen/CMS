using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace CMS.WebUI.Models
{
    public class UserCardViewModel
    {
        [Required]
        [Display(Name = "编号")]
        public string ID { get; set; }

        [Required]
        [Display(Name = "用户编号")]
        public string UserID { get; set; }

        [Required]
        [Display(Name = "卡号")]
        public string CardNo { get; set; }
    }
}