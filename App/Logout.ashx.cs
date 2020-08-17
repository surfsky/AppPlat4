using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.DAL;
using System.Web.Security;
using App.Utils;
using App.Controls;
using App.Components;

namespace App.Handlers
{
    [UI("注销并返回登录页面")]
    [Auth(Ignore = true)]
    public class Logout : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            Common.Logout();
            FormsAuthentication.RedirectToLoginPage();
        }
    }
}