using System;
using System.Web.UI;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.SessionState;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using FineUIPro;
using App.Utils;
using App.Components;  // Common.UserActive
using App.DAL;

namespace App.Controls
{
    //-----------------------------------------------------
    // HandlerBase
    //-----------------------------------------------------
    /// <summary>
    /// 处理器基类，集成了以下功能（1）访问权限；（2）访问参数校验；（3）在线用户
    /// </summary>
    public class HandlerBase : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable { get { return true; } }

        /// <summary>权限</summary>
        public AuthAttribute Auth { get; set; }

        /// <summary>处理入口</summary>
        public void ProcessRequest(HttpContext context)
        {
            // 权限校验
            this.Auth = Reflector.GetAttribute<AuthAttribute>(this.GetType());
            if (this.Auth == null)
                this.Auth = new AuthAttribute();  // { AuthSign = true };
            if (!CheckAuth())
                return;
            if (!Common.CheckPageParams(this))
                return;

            // 在线用户
            Common.UserActive();
            Process(context);
        }

        /// <summary>校验权限</summary>
        public bool CheckAuth()
        {
            var url = HttpContext.Current.Request.Url.OriginalString;
            if (this.Auth.AuthLogin && !Common.CheckLogin())
            {
                //Common.ShowFail("Auth login fail");
                HttpContext.Current.Response.Redirect(Urls.Login);
                return false;
            }
            if (this.Auth.AuthSign && !url.CheckSignedUrl())
            {
                Asp.Fail("Auth sign fail");
                return false;
            }
            if (!Common.CheckPower(this.Auth.ViewPower))
            {
                Asp.Fail("Auth power fail");
                return false;
            }
            return true;
        }

        /// <summary>请在子类中override实现</summary>
        public virtual void Process(HttpContext context)
        {
            throw new NotImplementedException("");
        }

        /// <summary>输出</summary>
        protected static void Write(string format, params object[] args)
        {
            Asp.Write(format, args);
        }
    }


    //-----------------------------------------------------
    // PageBase
    //-----------------------------------------------------
    /// <summary>
    /// 页面基类，集成了以下功能（1）访问权限（2）参数校验（2）在线用户（3）主题（4）标题。
    /// 页面访问权限可直接写Attribute: 
    /// 校验访问模式：[Auth(Power.UserView, Power.UserEdit, Power.UserNew, Power.UserDelete)]
    /// 校验登陆状态：[Auth(AuthLogin=true)]
    /// 校验URL签名： [Auth(AuthSign=true)]
    /// </summary>
    public class PageBase : Page
    {
        /// <summary>页面访问权限</summary>
        public AuthAttribute Auth { get; set; }

        /// <summary>页面模式（从ViewState或RequestString中获取）</summary>
        /// <remarks>有没有必要存储在ViewState待考虑，有空再弄吧</remarks>
        public PageMode Mode
        {
            get
            {
                if (ViewState["PageMode"] == null)
                {
                    var mode = Common.PageMode ?? PageMode.Edit;
                    ViewState["PageMode"] = mode.ToString();
                }
                return ViewState["PageMode"].ToString().ParseEnum<PageMode>().Value;
            }
            set
            {
                ViewState["PageMode"] = value.ToString();
            }
        }

        // 初始化
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // 权限校验
            var url = this.Request.RawUrl;
            this.Auth = Reflector.GetAttribute<AuthAttribute>(this.GetType());
            if (this.Auth == null)
                this.Auth = new AuthAttribute();  // { AuthSign = true };
            if (!CheckAuth())
                return;
            if (!Common.CheckPageParams(this))
                return;

            // 在线用户数、页面标题、主题
            Common.UserActive();
            base.Page.Title = DAL.SiteConfig.Instance.Name;
            if (PageManager.Instance != null && DAL.SiteConfig.Instance.Theme != null)
            {
                Theme theme;
                if (!Enum.TryParse<Theme>(DAL.SiteConfig.Instance.Theme, out theme))
                    theme = FineUIPro.Theme.Metro_Blue;
                PageManager.Instance.Theme = theme;
            }

            // 统一样式表
            Asp.RegistCSS("~/res/css/fineuipro.css");
        }



        /// <summary>权限校验</summary>
        public bool CheckAuth()
        {
            // 登陆鉴权及URL签名鉴权
            var url = Request.Url.OriginalString;
            if (this.Auth.AuthLogin && !Common.CheckLogin())
            {
                //Common.ShowFail("Auth login fail");
                Response.Redirect(Urls.Login);
                return false;
            }
            if (this.Auth.AuthSign && !url.CheckSignedUrl())
            {
                Asp.Fail("Auth sign fail");
                return false;
            }

            // 页面访问模式鉴权（若无编辑权限，可自动降权为阅读权限）
            if (this.Mode == PageMode.Edit && !Common.CheckPower(this.Auth.EditPower))
                this.Mode = PageMode.View;
            if (this.Mode == PageMode.View && !Common.CheckPower(this.Auth.ViewPower))
            {
                Asp.Fail("Auth power fail");
                return false;
            }
            if (this.Mode == PageMode.New && !Common.CheckPower(this.Auth.NewPower))
            {
                Asp.Fail("Auth power fail");
                return false;
            }
            return true;
        }

        /// <summary>更改页面模式（修改url并刷新）</summary>
        public static void ChangePageMode(PageMode mode, long id, string info="")
        {
            var url = new Url(HttpContext.Current.Request.RawUrl);
            url["md"] = mode.ToString().ToLower();
            url["id"] = id.ToString();
            url["info"] = info.UrlEncode();
            HttpContext.Current.Response.Redirect(url.ToString());
        }
    }
}
