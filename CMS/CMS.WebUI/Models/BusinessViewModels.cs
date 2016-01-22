using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;

namespace CMS.WebUI.Models
{
    public class RechargeViewModels
    {
        [Required]
        [Display(Name = "编号")]
        public int ID { get; set; }

        [Required]
        [Display(Name = "卡号")]
        public string CardNo { get; set; }

        [Required]
        [Display(Name = "时间")]
        public DateTime DateTime { get; set; }

        [Required]
        [Display(Name = "金额")]
        public double Money { get; set; }

        [Required]
        [Display(Name = "操作")]
        public string Operate { get; set; }

        [Required]
        [Display(Name = "操作人")]
        public string Operator { get; set; }
    }

    public class CardBalanceViewModels
    {
        [Required]
        [Display(Name = "编号")]
        public int ID { get; set; }

        [Required]
        [Display(Name = "卡号")]
        public string CardNo { get; set; }

        [Required]
        [Display(Name = "余额")]
        public double Balance { get; set; }
    }

    //仅用做给页面传值
    public class RechargeEx
    {
        [Required]
        [Display(Name = "卡号")]
        public string CardNo { get; set; }

        [Required]
        [Display(Name = "金额")]
        public double Money { get; set; }

        [Required]
        [Display(Name = "余额")]
        public double Balance { get; set; }
    }
}