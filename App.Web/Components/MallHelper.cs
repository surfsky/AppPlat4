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
    /// 商城相关
    /// </summary>
    public class MallHelper
    {
        /// <summary>尝试获取订单，无订单或无权限返回异常</summary>
        public static Order TryGetOrder(long orderId)
        {
            var userId = Common.LoginUser.ID;
            var order = Order.Get(orderId);
            if (order == null)
                throw new Exception("无此订单");
            else if (order.UserID != userId && Common.CheckPower(Powers.OrderEdit))
                throw new Exception("你无权修改他人订单");
            return order;
        }


        /// <summary>尝试更改订单状态（无权限或无订单或报异常）</summary>
        public static Order TryChangeOrderStatus<T>(long orderId, T status, string remark = "", string image1 = "", string image2 = "", string image3 = "") where T : struct
        {
            var o = MallHelper.TryGetOrder(orderId);

            // 传图像、改状态
            var userId = Common.LoginUser.ID;
            var urls = Uploader.UploadBase64Images("Orders", image1, image2, image3);
            var item = o.ChangeStatusEnum(status, userId, remark, urls);
            return item;
        }


        /// <summary>无管理员权限的，只显示自己归属的门店数据</summary>
        public static bool CheckShop(long? shopId)
        {
            var user = Common.LoginUser;
            if (shopId == null) return true;
            if (user == null) return false;
            if (user.Name == "admin") return true;
            if (user.HasPower(Powers.Admin)) return true;
            if (user.ShopID == shopId) return true;
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
            if (!user.HasPower(Powers.Admin))
                pbShop.Readonly = true;
        }

    }
}
