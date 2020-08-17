using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Text.RegularExpressions;
using App.Controls;
using App.DAL;
using App.Core;
using App.Components;
using System.IO;
using System.Reflection;

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
        public readonly static Version Version = Assembly.GetCallingAssembly().GetName().Version;  //  typeof(Global).Assembly



        //-----------------------------------------
        // 所有集合
        //-----------------------------------------
        /// <summary>所有产品</summary>
        public static List<Product> AllProducts
        {
            get { return Product.Search(onShelf: true).Sort(t => t.Name).ToList(); }
        }

        public static List<User> AllUsers
        {
            get { return User.Set.Where(t => t.InUsed!=false).Sort(t => t.NickName).ToList(); }
        }

        public static List<EnumInfo> AllRoles
        {
            get { return typeof(Role).GetEnumInfos();}
        }

        public static List<Shop> AllShops
        {
            get { return Shop.Search().Sort(t => t.Name).ToList(); }
        }

        //-----------------------------------------
        // 授权集合
        //-----------------------------------------
        /// <summary>获取自己授权控制的角色</summary>
        public static List<EnumInfo> AllowedRoles
        {
            get
            {
                if (Common.LoginUser.Name == "admin")
                    return typeof(Role).GetEnumInfos();

                var roles = Common.LoginUser.Roles;
                //if (!roles.Contains(Role.Employees))
                //    roles.Add(Role.Employees);
                return roles.ToEnumInfos();
            }
        }


        /// <summary>当前用户授权管理区域</summary>
        public static List<Area> AllowedAreas
        {
            get
            {
                if (Common.LoginUser.HasPower(Power.Admin))
                    return DAL.Area.All;
                List<Area> items = new List<Area>();
                foreach(var area in Common.LoginUser.GetManageAreas())  // ?? Common.LoginUser.Shop?.AreaID;
                {
                    foreach (var child in Area.GetRecursive(area.ID))
                    {
                        if (!items.Contains(child))
                            items.Add(child);
                    }
                }
                return items;
            }
        }
    }
}
