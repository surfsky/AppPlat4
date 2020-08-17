﻿using System;
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

namespace App
{
    /// <summary>
    /// 用户及权限相关
    /// </summary>
    public partial class Common
    {
        //-----------------------------------------
        // 登录注销
        //-----------------------------------------
        /// <summary>登录成功：注册在线、获取角色、创建cookie验票、刷新Common.UserPowers   </summary>
        public static void LoginSuccess(User user, double cookieDurationHours)
        {
            user = User.GetDetail(user.ID);
            user.LastLoginDt = DateTime.Now;
            user.Save();
            Common.UserUpline(user.ID);
            Common.LoginUser = user;

            // 将用户角色字符串保存到cookie验票里去（在Global中读取）
            string[] roles = user.RoleIds.Select(r => r.ToString()).ToArray();
            AuthHelper.Login(user.Name, roles, DateTime.Now.AddHours(cookieDurationHours));
        }

        public static void Logout()
        {
            AuthHelper.Logout();
        }

        //-----------------------------------------
        // 当前用户
        //-----------------------------------------
        /// <summary>当前登录用户</summary>
        public static User LoginUser
        {
            get { return Asp.GetSessionData<User>("LoginUser", () => User.GetDetail(name: AuthHelper.GetLoginUserName())); }
            set { Asp.Session["LoginUser"] = value; }
        }

        /// <summary>刷新登录用户</summary>
        public static void RefreshLoginUser()
        {
            Common.LoginUser = DAL.User.GetDetail(name: AuthHelper.GetLoginUserName());
        }

        //-----------------------------------------
        // 尝试获取（会抛出异常）
        //-----------------------------------------
        /// <summary>尝试获取用户（若userId为空则返回当前登录用户；若不为空且是他人，则检测是否具有相应访问权限）。失败会抛出异常。</summary>
        public static User TryGetUser(long? userId, Powers? power)
        {
            if (!Common.CheckLogin())           throw new Exception("未登录");
            if (userId == null)                 return Common.LoginUser;
            if (userId == Common.LoginUser.ID)  return Common.LoginUser;
            if (!Common.CheckPower(power))      throw new Exception("无权访问 " + power.GetTitle());

            var user = User.GetDetail(userId.Value);
            if (user == null) throw new Exception("无此用户");
            return user;
        }




        //-----------------------------------------
        // 在线用户
        //-----------------------------------------
        /// <summary>用户上线</summary>
        public static void UserUpline(long userId)
        {
            var now = DateTime.Now;
            var ip = Asp.ClientIP;
            Online.Regist(userId, "", ip, now);
            Messenger.SendToComet(Components.CometMessageType.Online, Online.GetOnlines());
            Asp.Session[Common.SESSION_ONLINE_UPDATE_TIME] = now;
        }

        /// <summary>用户活动中（记录到Online表）</summary>
        /// <remarks>5分钟刷新一次用户最后活动数据，避免过于频繁修改</remarks>
        public static void UserActive()
        {
            var username = Asp.User?.Identity?.Name;
            if (username.IsEmpty())
                return;

            var minutes = 5;  // 延缓更新时间
            var now = DateTime.Now;
            var ip = Asp.ClientIP;
            var lastUpdateDt = HttpContext.Current.Session[Common.SESSION_ONLINE_UPDATE_TIME];
            if (lastUpdateDt == null || (Convert.ToDateTime(lastUpdateDt).Subtract(now).TotalMinutes > minutes))
            {
                Asp.Session[Common.SESSION_ONLINE_UPDATE_TIME] = now;
                Online.Update(username, ip, now);
            }
        }



    }
}
