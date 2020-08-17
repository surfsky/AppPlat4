using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using EntityFramework.Extensions;
using App.Utils;
using App.Entities;

namespace App.DAL
{

    /// <summary>
    /// 微信小程序表单请求（小程序发送表单时记录，后台发送模板消息时使用）
    /// </summary>
    [UI("微信", "微信小程序表单请求数据")]
    public class WechatMPForm : EntityBase<WechatMPForm>
    {
        [UI("FormID")]      public string FormID   { get; set; }
        [UI("UserId")]      public long? UserID    { get; set; }
        [UI("OpenID")]      public string OpenID   { get; set; }
        [UI("UnionID")]     public string UnionID  { get; set; }
        [UI("订单ID")]      public long? OrderID   { get; set; }
        [UI("使用次数")]    public int? Times      { get; set; } = 0;

        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        /// <summary>查找</summary>
        public static IQueryable<WechatMPForm> Search(long? userId=null, string openId="", string unionId="")
        {
            IQueryable<WechatMPForm> q = Set;
            if (userId != null)                q = q.Where(l => l.UserID == userId);
            if (openId.IsNotEmpty())           q = q.Where(t => t.OpenID == openId);
            if (unionId.IsNotEmpty())          q = q.Where(t => t.UnionID == unionId);
            return q;
        }

        /// <summary>获取</summary>
        public static WechatMPForm GetForm(long? userId=null, string openId="")
        {
            var item = Search(userId, openId)
                .Where(t => t.Times == null || t.Times == 0)
                .Sort(t => t.CreateDt)
                .FirstOrDefault()
                ;
            if (item != null)
            {
                item.Times = item.Times.Inc(1);
                item.Save();
                return item;
            }
            return null;
        }

        /// <summary>创建</summary>
        public static WechatMPForm Create(long userId, string formId, long? orderId=null)
        {
            var user = User.Get(userId);
            if (user != null)
            {
                var item = new WechatMPForm();
                item.UserID = userId;
                item.FormID = formId;
                item.CreateDt = DateTime.Now;
                item.OpenID = user.WechatMPID;
                item.UnionID = user.WechatUnionID;
                item.OrderID = orderId;
                item.Times = 0;
                return item.Save();
            }
            return null;
        }
    }
}