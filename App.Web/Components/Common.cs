using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Text.RegularExpressions;
using App.Controls;
using App.DAL;
using App.Utils;
using App.Components;
using System.IO;
using System.Reflection;
using App.Entities;

namespace App
{


    /// <summary>
    /// 公共方法和全局变量常量（Asp.net 网站相关）
    /// 本文件存储公共常量和属性
    /// </summary>
    public static partial class Common
    {
        //-------------------------------------
        // 常量
        //-------------------------------------
        public readonly static string SESSION_VERIFYCODE = "session_code";                  // 验证码Session名称
        public readonly static string SESSION_ONLINE_UPDATE_TIME = "OnlineUpdateTime";      // 在线人数最后更新时间
        public readonly static string CHECK_POWER_FAIL_ACTION_MESSAGE = "您无权进行此操作！";
        public readonly static string CHECK_POWER_FAIL_PAGE_MESSAGE = string.Format("您无权访问此页面！请重新<a href='{0}'>登录</a>", FormsAuthentication.LoginUrl);
        public readonly static Version Version = Assembly.GetCallingAssembly()?.GetName()?.Version;  //  typeof(Global).Assembly


        public static Org CurrentOrg
        {
            get { return Asp.GetSession("CurrentOrg") as Org; }
            set { Asp.SetSession("CurrentOrg", value); }
        }

    }
}
