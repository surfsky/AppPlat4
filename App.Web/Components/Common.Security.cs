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
using System.Text;

namespace App
{
    /// <summary>
    /// 安全相关
    /// </summary>
    public partial class Common
    {

        public static PageMode? PageMode => Asp.GetQuery<PageMode>("md");


        //-----------------------------------------
        // 检测页面参数
        //-----------------------------------------
        /// <summary>检测页面请求参数。若指定参数不存在且是必须的，则输出错误。</summary>
        public static bool CheckPageParams(IHttpHandler page)
        {
            var ps = page.GetType().GetAttributes<ParamAttribute>();
            foreach (var p in ps)
            {
                var s = Asp.GetQueryString(p.Name);
                if (p.Required && s.IsEmpty())
                {
                    Asp.Fail("缺少参数 {0}, 类型 {1}, {2}", p.Name, p.Type.GetTypeString(), p.Remark);
                    return false;
                }
            }
            return true;
        }

        //-----------------------------------------
        // 检测验证码
        //-----------------------------------------
        /// <summary>检测图像验证码</summary>
        public static bool CheckVerifyImage(string verifyCode)
        {
            verifyCode = verifyCode.ToLower();
            var session = Asp.GetSession(Common.SESSION_VERIFYCODE);
            if (session != null && verifyCode == session.ToString().ToLower())
                return true;
            return false;
        }

        /// <summary>校验短信验证码</summary>
        public static VerifyCodeStatus CheckVerifySMS(string mobile, string code)
        {
            var vCode = VerifyCode.GetCode(mobile);
            if (vCode == null || vCode.ExpireDt < DateTime.Now)
                return VerifyCodeStatus.Expired;
            if (vCode.Code != code)
                return VerifyCodeStatus.Wrong;
            return VerifyCodeStatus.Ok;
        }

        //-----------------------------------------
        // 检测用户、角色、权限
        //-----------------------------------------
        /// <summary>检查是否登录</summary>
        public static bool CheckLogin()
        {
            return AuthHelper.IsLogin();
            //return (LoginUser != null);
        }

        
        /// <summary>检查当前用户是否拥有某个权限</summary>
        public static bool CheckPower(Powers? power)
        {
            if (power == null)             return true;
            if (LoginUser == null)         return false;
            if (LoginUser.Name == "admin") return true;
            return LoginUser.HasPower(power.Value);
        }

        /// <summary>检测页面访问权限</summary>
        /// <param name="power">PowerType? 或 Boolean</param>
        public static bool CheckPagePower(object power)
        {
            if (power is Powers?)
                return CheckPagePower(power as Powers?);
            if (power is bool)
                return (bool)power;
            return true;
        }
        /// <summary>检测页面访问权限</summary>
        public static bool CheckPagePower(Powers? power)
        {
            if (!Common.CheckPower(power))
            {
                Asp.Fail(Common.CHECK_POWER_FAIL_PAGE_MESSAGE);
                return false;
            }
            return true;
        }
    }
}
