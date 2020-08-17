using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using App.HttpApi;
using App.Core;
using App;

namespace $rootnamespace$
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // HttpApi 自定义访问校验
            HttpApiConfig.Instance.OnAuth += (ctx, method, attr, token) =>
            {
                if (attr.AuthToken && Token.Check(token) == null)
                    throw new HttpApiException(404, "Token failure.");
            };
        }
    }
}