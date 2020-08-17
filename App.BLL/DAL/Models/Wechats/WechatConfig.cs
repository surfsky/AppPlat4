using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.DAL;
using System.Configuration;
using System.Drawing;
//using EntityFramework.Extensions;
using App.Utils;
using App.Components;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entities;

namespace App.DAL
{
    /// <summary>微信配置</summary>
    [UI("系统", "微信配置")]
    public class WechatConfig : EntityBase<WechatConfig>
    {
        //-------------------------------------------
        // 微信公众号 
        //-------------------------------------------
        [UI("微信公众号", "AppId")]         public string OPAppID         { get; set; }
        [UI("微信公众号", "AppSecret")]     public string OPAppSecret     { get; set; }
        [UI("微信公众号", "PayUrl")]        public string OPPayUrl        { get; set; }
        [UI("微信公众号", "PushToken")]     public string OPPushToken     { get; set; }
        [UI("微信公众号", "PushKey")]       public string OPPushKey       { get; set; }
        [UI("微信公众号", "TokenServer")]   public string OPTokenServer   { get; set; }

        //-------------------------------------------
        // 微信小程序 
        //-------------------------------------------
        [UI("微信小程序", "AppId")]         public string MPAppID         { get; set; }
        [UI("微信小程序", "AppSecret")]     public string MPAppSecret     { get; set; }
        [UI("微信小程序", "PayUrl")]        public string MPPayUrl        { get; set; }
        [UI("微信小程序", "PushToken")]     public string MPPushToken     { get; set; }
        [UI("微信小程序", "PushKey")]       public string MPPushKey       { get; set; }
        [UI("微信小程序", "TokenServer")]   public string MPTokenServer   { get; set; }


        //-------------------------------------------
        // 微信支付 
        //-------------------------------------------
        [UI("微信支付", "MchId")]           public string MchId           { get; set; }
        [UI("微信支付", "MchKey")]          public string MchKey          { get; set; }
    }

}