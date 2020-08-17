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
using System.Text;

namespace App
{
    /// <summary>
    /// 安全相关
    /// </summary>
    public partial class Common
    {

        public static PageMode? PageMode => Asp.GetQuery<PageMode>("md");





        /// <summary>页面访问失败</summary>
        public static void ShowFail(string format, params object[] args)
        {
            var text = Utils.GetText(format, args);
            Asp.Response.Write(text);
            Asp.Response.End();
        }


        //-----------------------------------------
        // 尝试获取（会抛出异常）
        //-----------------------------------------
        /// <summary>尝试获取用户（若userId为空则返回当前登录用户；若不为空且是他人，则检测是否具有相应访问权限）。失败会抛出异常。</summary>
        public static User TryGetUser(long? userId, Power? power)
        {
            if (!Common.CheckLogin()) throw new Exception("未登录");
            if (userId == null) return Common.LoginUser;
            if (userId == Common.LoginUser.ID) return Common.LoginUser;
            if (!Common.CheckPower(power)) throw new Exception("无权访问 " + power.GetTitle());

            var user = User.GetDetail(userId.Value);
            if (user == null) throw new Exception("无此用户");
            return user;
        }

        /// <summary>尝试获取订单，无订单或无权限返回异常</summary>
        public static Order TryGetOrder(long orderId)
        {
            var userId = Common.LoginUser.ID;
            var order = Order.Get(orderId);
            if (order == null)
                throw new Exception("无此订单");
            else if (order.UserID != userId && Common.CheckPower(Power.OrderEdit))
                throw new Exception("你无权修改他人订单");
            return order;
        }


        /// <summary>尝试更改订单状态（无权限或无订单或报异常）</summary>
        public static Order TryChangeOrderStatus<T>(long orderId, T status, string remark = "", string image1 = "", string image2 = "", string image3 = "") where T : struct
        {
            var o = Common.TryGetOrder(orderId);

            // 传图像、改状态
            var userId = Common.LoginUser.ID;
            var urls = Uploader.UploadBase64Images("Orders", image1, image2, image3);
            var item = o.ChangeStatusEnum(status, userId, remark, urls);
            return item;
        }


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
                    Common.ShowFail("缺少参数 {0}, 类型 {1}, {2}", p.Name, p.Type.GetTypeString(), p.Remark);
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
        public static bool CheckPower(Power? power)
        {
            if (power == null)             return true;
            if (LoginUser == null)         return false;
            if (LoginUser.Name == "admin") return true;
            return LoginUser.Powers.Contains(power.Value);
        }

        /// <summary>检测页面访问权限</summary>
        /// <param name="power">PowerType? 或 Boolean</param>
        public static bool CheckPagePower(object power)
        {
            if (power is Power?)
                return CheckPagePower(power as Power?);
            if (power is bool)
                return (bool)power;
            return true;
        }
        /// <summary>检测页面访问权限</summary>
        public static bool CheckPagePower(Power? power)
        {
            if (!Common.CheckPower(power))
            {
                ShowFail(Common.CHECK_POWER_FAIL_PAGE_MESSAGE);
                return false;
            }
            return true;
        }


        /// <summary>无管理员权限的，只显示自己归属的门店数据</summary>
        public static bool CheckShop(long? shopId)
        {
            var user = LoginUser;
            if (shopId == null)                 return true;
            if (user == null)                   return false;
            if (user.Name == "admin")           return true;
            if (user.HasPower(Power.Admin))     return true;
            if (user.ShopID == shopId)          return true;
            return false;
        }


        //-----------------------------------------
        // 控件限制
        //-----------------------------------------
        /*
        /// <summary>无管理员权限的，只显示自己归属的门店数据</summary>
        public static void LimitShop(FineUIPro.DropDownList ddlShop)
        {
            var user = Common.LoginUser;
            UI.SetValue(ddlShop, user.ShopID);
            if (!user.HasRole(RoleType.Admin))
                ddlShop.Readonly = true;
        }
        */

        /// <summary>无管理员权限的，只显示自己归属的门店数据</summary>
        public static void LimitShop(PopupBox pbShop)
        {
            var user = Common.LoginUser;
            UI.SetValue(pbShop, user.Shop, t => t.ID, t => t.AbbrName);
            if (!user.HasPower(Power.Admin))
                pbShop.Readonly = true;
        }
    }
}
