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
    /// <summary>
    /// 缓存方式
    /// </summary>
    public enum CacheMode
    {
        [UI("文件")] File = 0,
        [UI("内存")] Memory = 1,
    }

    /// <summary>网站配置</summary>
    [UI("系统", "网站配置")]
    public class SiteConfig : EntityBase<SiteConfig>
    {
        //-------------------------------------------
        // 常量
        //-------------------------------------------
        // 图片路径及尺寸设置
        public string DefaultImage          = "/Res/Images/defaultPicture.png";
        public string DefaultUserImage      = "/Res/Images/defaultUser.png";
        public string DefaultArticleImage   = "/Res/Images/defaultArticle.png";
        public string DefaultClassImage     = "/Res/Images/defaultClass.png";
        public string DefaultProductImage   = "/Res/Images/defaultProduct.png";
        public string DefaultShopImage      = "/Res/Images/defaultShop.png";
        public string DefaultAppBgImage     = "/Res/Images/defaultAppBg.png";
        public string DefaultAppBannerImage = "/Res/Images/defaultAppBanner.png";
        public string DefaultLogoImage      = "/Res/logoWhite.png";
        public string DefaultVideo          = "/Res/about.mp4";
        public string DefaultDirImage       = "/Res/Images/defaultDir.png";
        public Size   SizeLogo              = new Size(20, 20);
        public Size   SizeLogoDark          = new Size(64, 64);
        public Size   SizeSmallImage        = new Size(80, 80);
        public Size   SizeMiddleImage       = new Size(200, 200);
        public Size   SizeBigImage          = new Size(600, 600);
        public Size   SizeAppBanner         = new Size(750, 500);
        public Size   SizeAppBg             = new Size(750, 1334);
        public Size   SizeDirImage          = new Size(155, 155);


        //-------------------------------------------
        // 网站基础信息
        //-------------------------------------------
        [UI("基础", "名称")]                public string Name                  { get; set; } = "AppPlat";
        [UI("基础", "简称")]                public string AbbrName              { get; set; } = "AppPlat";
        [UI("基础", "域名")]                public string Domain                { get; set; } = "AppPlat.cc";
        [UI("基础", "拥有者")]              public string Owner                 { get; set; } = "AppPlat.cc";
        [UI("基础", "版权")]                public string Copyright             { get; set; } = "版权所有 2019，保留所有权利";
        [UI("基础", "备案号")]              public string ICP                   { get; set; } = "浙ICP备1号-1";

        // UI
        [UI("界面", "图标")]                public string Logo                  { get; set; } = "";
        [UI("界面", "图标（深色）")]        public string LogoDark              { get; set; } = "";
        [UI("界面", "登陆背景")]            public string LoginBg               { get; set; } = "/Res/login/img/bg.jpg";
        [UI("界面", "样式主题")]            public string Theme                 { get; set; } = "";
        [UI("界面", "帮助菜单")]            public string HelpList              { get; set; } = "";

        // 数据
        [UI("数据", "默认密码")]            public string DefaultPassword       { get; set; } = "";
        [UI("数据", "分页大小")]            public int    PageSize              { get; set; } = 50;
        [UI("数据", "可上传文件类型")]      public string UpFileTypes           { get; set; }
        [UI("数据", "可上传文件大小（M）")] public long?  UpFileSize            { get; set; } = 50;

        // 性能
        [UI("性能", "缓存模式")]            public CacheMode? CacheMode          { get; set; } = DAL.CacheMode.File;
        [UI("性能", "文件缓存期（分钟）")]  public double?   FileCacheMinutes    { get; set; } = 60*24;
        [UI("性能", "内存缓存期（分钟）")]  public double?   MemoryCacheMinutes  { get; set; } = 30;

        // 防护
        [UI("防护", "Cookie有效期（小时）")] public double?  CookieHours          { get; set; } = 24*7;
        [UI("防护", "签名Key")]              public string    SignKey             { get; set; } = "PageSignKey";
        [UI("防护", "签名有效期（分钟）")]   public double?   SignMinutes         { get; set; } = 120;
        [UI("防护", "访频限制（平均每秒次数）")]   public int?   VisitDensity     { get; set; } = 20;
        [UI("防护", "超频封IP（分钟）")]           public int?   BanMinutes       { get; set; } = 30;

        // 表单UI
        public override UISetting FormUI()
        {
            var ui = new UISetting<SiteConfig>(true);
            ui.SetEditorImage(t => t.Logo, SizeLogo);
            ui.SetEditorImage(t => t.LogoDark, SizeLogo);
            ui.SetEditorImage(t => t.LoginBg, SizeBigImage);
            ui.SetEditor(t => t.HelpList).SetEditor(EditorType.TextArea, 200);
            //ui.Set(t => t.Theme).Type = typeof(FineUIPro.Theme);
            return ui.BuildGroups();
        }

        //-------------------------------------------
        // 方法
        //-------------------------------------------
        /// <summary>初始化数据</summary>
        public override void Init()
        {
            var item = SiteConfig.Instance;
            if (item.Name.IsEmpty())                item.Name = "AppPlat";
            if (item.PageSize == 0)                 item.PageSize = 50;
            if (item.CacheMode.IsEmpty())           item.CacheMode = DAL.CacheMode.File;
            if (item.FileCacheMinutes.IsEmpty())    item.FileCacheMinutes = 60*24;
            if (item.MemoryCacheMinutes.IsEmpty())  item.MemoryCacheMinutes = 60;
            if (item.SignKey.IsEmpty())             item.SignKey = Guid.NewGuid().ToString("N").Substring(0, 8);
            if (item.SignMinutes.IsEmpty())         item.SignMinutes = 120;
            if (item.VisitDensity.IsEmpty())        item.VisitDensity = 15;
            if (item.BanMinutes.IsEmpty())          item.BanMinutes = 30;
            if (item.UpFileTypes.IsEmpty())         item.UpFileTypes = ".gif, .png, .jpg, .jpeg, .bmp, .mp3, .mp4, .doc, .docx, .xls, .xlsx, .ppt, .pptx, .pdf, .cdr";
            if (item.UpFileSize.IsEmpty())          item.UpFileSize = 50;
            if (item.CookieHours.IsEmpty())         item.CookieHours = 24*7;
            if (item.HelpList.IsEmpty())
                item.HelpList = @"[{
                        'Text': '万年历',
                        'Icon': 'Calendar',
                        'ID': 'wannianli',
                        'URL': '~/admins/help/wannianli.htm'
                    },
                    {
                        'Text': '科学计算器',
                        'Icon': 'Calculator',
                        'ID': 'jisuanqi',
                        'URL': '~/admins/help/jisuanqi.htm'
                    },
                    {
                        'Text': '系统帮助',
                        'Icon': 'Help',
                        'ID': 'help',
                        'URL': '~/admins/article.aspx?id=3'
                    }]";
            item.Save();
        }

        /// <summary>修改后更新缓存</summary>
        public override void AfterChange(EntityOp op)
        {
            base.AfterChange(op);
            IO.RemoveCache("WatermarkImage");
        }
    }
}