using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.DAL;
using System.Configuration;
using System.Drawing;
using EntityFramework.Extensions;
using App.Core;

namespace App.Components
{
    /// <summary>
    /// 网站配置信息
    /// 方案：存储在单一 Config 表中
    /// 优点：
    ///     配置集中存储；
    ///     调用简洁，如SiteConfig.Title = "XXX";
    /// 缺点：
    ///     界面要手工写；
    ///     键要手工分配好避免冲突；
    ///     本类的代码量较大
    /// </summary>
    [Obsolete("请使用SiteConfig")]
    public class SiteConfigOld
    {
        //-------------------------------------------
        // 常量
        //-------------------------------------------
        // 图片路径及尺寸设置
        public static string DefaultImage          = "~/Res/Images/defaultPicture.png";
        public static string DefaultUserImage      = "~/Res/Images/defaultUser.png";
        public static string DefaultArticleImage   = "~/Res/Images/defaultArticle.png";
        public static string DefaultClassImage     = "~/Res/Images/defaultClass.png";
        public static string DefaultProductImage   = "~/Res/Images/defaultProduct.png";
        public static string DefaultStoreImage     = "~/Res/Images/defaultStore.png";
        public static string DefaultAppBgImage     = "~/Res/Images/defaultAppBg.png";
        public static string DefaultAppBannerImage = "~/Res/Images/defaultAppBanner.png";
        public static string DefaultLogoImage      = "~/Res/logoWhite.png";
        public static string DefaultVideo          = "~/Res/about.mp4";
        public static Size   SizeLogo              = new Size(20, 20);
        public static Size   SizeLogoDark          = new Size(64, 64);
        public static Size   SizeSmallImage        = new Size(80, 80);
        public static Size   SizeMiddleImage       = new Size(200, 200);
        public static Size   SizeBigImage          = new Size(600, 600);
        public static Size   SizeAppBanner         = new Size(750, 500);
        public static Size   SizeAppBg             = new Size(750, 1334);


        //-------------------------------------------
        // 数据库存储
        //-------------------------------------------
        // 站点相关
        [UI("站点", "名称")]                public static string Name                  { get { return Get("Site", "Name"); }                      set { Set("Site", "Name", value); } }
        [UI("站点", "标题")]                public static string Title                 { get { return Get("Site", "Title"); }                     set { Set("Site", "Title", value); } }
        [UI("站点", "域名")]                public static string Domain                { get { return Get("Site", "Domain"); }                    set { Set("Site", "Domain", value); } }
        [UI("站点", "备案号")]              public static string ICP                   { get { return Get("Site", "ICP"); }                       set { Set("Site", "ICP", value); } }
        [UI("站点", "缓存模式")]            public static string CacheMode             { get { return Get("Site", "CacheMode"); }                 set { Set("Site", "CacheMode", value); } }
        [UI("站点", "设备编号")]            public static string MachineID             { get { return Get("Site", "MachineID"); }                 set { Set("Site", "MachineID", value); } }

        [UI("站点", "图标")]                public static string Logo                  { get { return Get("Site", "Logo"); }                      set { Set("Site", "Logo", value); } }
        [UI("站点", "图标（深色）")]        public static string LogoDark              { get { return Get("Site", "LogoDark"); }                  set { Set("Site", "LogoDark", value); } }
        [UI("站点", "帮助菜单")]            public static string HelpList              { get { return Get("Site", "HelpList"); }                  set { Set("Site", "HelpList", value); } }
        [UI("站点", "菜单样式")]            public static string MenuType              { get { return Get("Site", "MenuType"); }                  set { Set("Site", "MenuType", value); } }
        [UI("站点", "样式主题")]            public static string Theme                 { get { return Get("Site", "Theme"); }                     set { Set("Site", "Theme", value); } }
        [UI("站点", "默认密码")]            public static string DefaultPassword       { get { return Get("Site", "DefaultPassword"); }           set { Set("Site", "DefaultPassword", value); } }
        [UI("站点", "分页大小")]            public static int    PageSize              { get { return Get("Site", "PageSize").ParseInt().Value; } set { Set("Site", "PageSize", value); } }

        // App
        [UI("APP", "背景")]                 public static string AppBackground         { get { return Get("App",  "AppBackground"); }              set { Set("App", "AppBackground", value); } }


        // 阿里短信 
        [UI("阿里短信", "签名")]            public static string AliSmsSignName        { get { return Get("AliSms", "SignName"); }                set { Set("AliSms", "SignName", value); } }
        [UI("阿里短信", "Key")]             public static string AliSmsAccessKeyId     { get { return Get("AliSms", "AccessKeyId"); }             set { Set("AliSms", "AccessKeyId", value); } }
        [UI("阿里短信", "Secret")]          public static string AliSmsAccessKeySecret { get { return Get("AliSms", "AccessKeySecret"); }         set { Set("AliSms", "AccessKeySecret", value); } }
        [UI("阿里短信", "编号-注册")]       public static string AliSmsRegist          { get { return Get("AliSms", "No.Regist"); }               set { Set("AliSms", "No.Regist", value); } }
        [UI("阿里短信", "编号-校验")]       public static string AliSmsVerify          { get { return Get("AliSms", "No.Verify"); }               set { Set("AliSms", "No.Verify", value); } }
        [UI("阿里短信", "编号-改密码")]     public static string AliSmsChangePassword  { get { return Get("AliSms", "No.ChangePassword"); }       set { Set("AliSms", "No.ChangePassword", value); } }
        [UI("阿里短信", "编号-改信息")]     public static string AliSmsChangeInfo      { get { return Get("AliSms", "No.ChangeInfo"); }           set { Set("AliSms", "No.ChangeInfo", value); } }
        [UI("阿里短信", "编号-通知")]       public static string AliSmsNotify          { get { return Get("AliSms", "No.Notify"); }               set { Set("AliSms", "No.Notify", value); } }

        //-------------------------------------------
        // 微信公众号 
        //-------------------------------------------
        [UI("微信公众号", "AppId")]         public static string WechatOPAppID         { get { return Get("WechatOP", "AppID"); }         set { Set("WechatOP", "AppID", value); } }
        [UI("微信公众号", "AppSecret")]     public static string WechatOPAppSecret     { get { return Get("WechatOP", "AppSecret"); }     set { Set("WechatOP", "AppSecret", value); } }
        [UI("微信公众号", "PayUrl")]        public static string WechatOPPayUrl        { get { return Get("WechatOP", "PayUrl"); }        set { Set("WechatOP", "PayUrl", value); } }
        [UI("微信公众号", "PushToken")]     public static string WechatOPPushToken     { get { return Get("WechatOP", "PushToken"); }     set { Set("WechatOP", "PushToken", value); } }
        [UI("微信公众号", "PushKey")]       public static string WechatOPPushKey       { get { return Get("WechatOP", "PushKey"); }       set { Set("WechatOP", "PushKey", value); } }
        [UI("微信公众号", "TokenServer")]   public static string WechatOPTokenServer   { get { return Get("WechatOP", "TokenServer"); }   set { Set("WechatOP", "TokenServer", value); } }

        //-------------------------------------------
        // 微信小程序 
        //-------------------------------------------
        [UI("微信小程序", "AppId")]         public static string WechatMPAppID         { get { return Get("WechatMP", "AppID"); }         set { Set("WechatMP", "AppID", value); } }
        [UI("微信小程序", "AppSecret")]     public static string WechatMPAppSecret     { get { return Get("WechatMP", "AppSecret"); }     set { Set("WechatMP", "AppSecret", value); } }
        [UI("微信小程序", "PayUrl")]        public static string WechatMPPayUrl        { get { return Get("WechatMP", "PayUrl"); }        set { Set("WechatMP", "PayUrl", value); } }
        [UI("微信小程序", "PushToken")]     public static string WechatMPPushToken     { get { return Get("WechatMP", "PushToken"); }     set { Set("WechatMP", "PushToken", value); } }
        [UI("微信小程序", "PushKey")]       public static string WechatMPPushKey       { get { return Get("WechatMP", "PushKey"); }       set { Set("WechatMP", "PushKey", value); } }
        [UI("微信小程序", "TokenServer")]   public static string WechatMPTokenServer   { get { return Get("WechatMP", "TokenServer"); }   set { Set("WechatMP", "TokenServer", value); } }


        //-------------------------------------------
        // 微信支付 
        //-------------------------------------------
        [UI("微信支付", "MchId")]           public static string WechatMchId           { get { return Get("WechatPay", "MchId"); }        set { Set("WechatPay", "MchId", value); } }
        [UI("微信支付", "MchKey")]          public static string WechatMchKey          { get { return Get("WechatPay", "MchKey"); }       set { Set("WechatPay", "MchKey", value); } }



        //-------------------------------------------
        // 辅助方法
        //-------------------------------------------
        // 静态构造函数
        static SiteConfigOld()
        {
            if (Title.IsEmpty())            Title = "网站管理系统";
            if (Logo.IsEmpty())             Logo = DefaultLogoImage;
            if (LogoDark.IsEmpty())         LogoDark = DefaultLogoImage;
            if (DefaultPassword.IsEmpty())  DefaultPassword = "123456";
            if (AppBackground.IsEmpty())    AppBackground = DefaultAppBannerImage;
        }

        // 读写（含缓存处理）
        static string Get(string cate, string name)
        {
            var key = string.Format("cache-{0}-{1}", cate, name);
            return IO.GetCache(key, ()=> Config.GetValue(cate, name));
        }
        static void Set(string cate, string name, object value)
        {
            var key = string.Format("cache-{0}-{1}", cate, name);
            var v = IO.GetCache<string>(key);
            if (v != value.ToString())
            {
                IO.SetCache(key, value.ToString());
                Config.SetValue(cate, name, value);
            }
        }
    }
}