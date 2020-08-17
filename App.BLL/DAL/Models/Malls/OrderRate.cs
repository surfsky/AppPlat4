using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data.Entity;
using System.ComponentModel;
using System.Linq.Expressions;
using App.Utils;
using System.Drawing;
using App.Entities;

namespace App.DAL
{
    /// <summary>
    /// 评价等级
    /// </summary>
    public enum RateType : int
    {
        [UI("差评")] Pool = 1,
        [UI("低评")] Low = 2,
        [UI("中评")] Middle = 3,
        [UI("好评")] Good = 4,
        [UI("优秀")] Excellent = 5,
    }

    /// <summary>
    /// 订单评价
    /// </summary>
    [UI("商城", "订单评价")]
    public class OrderRate : EntityBase<OrderRate>
    {
        // 基础信息
        [UI("订单")]                   public long? OrderID { get; set; }
        [UI("用户")]                   public long? UserID { get; set; }
        [UI("说明")]                   public string Comment { get; set; }
        [UI("评价")]                   public int? Rate { get; set; }
        [UI("评价")]                   public string RateName { get { return ((RateType?)Rate).GetTitle(); } }

        // 导航信息
        public virtual Order Order { get; set; }
        public virtual User User { get; set; }

        // 导出
        public override object Export(ExportMode type = ExportMode.Normal)
        {
            return new
            {
                this.ID,
                this.OrderID,
                this.Rate,
                this.RateName,
                this.Comment,
                this.Order?.Summary,
                this.CreateDt,
                User?.NickName,
                User?.Avatar
            };
        }



        //-----------------------------------------------
        // 实例方法
        //-----------------------------------------------
        /// <summary>获取详情</summary>
        public new static OrderRate GetDetail(long id)
        {
            var item = Set.Include(t => t.Order).Include(t => t.User).FirstOrDefault(t => t.ID == id);
            return item;
        }

        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        /// <summary>查找</summary>
        public static IQueryable<OrderRate> Search(long? shopId, long? orderId)
        {
            IQueryable<OrderRate> q = Set.Include(t => t.Order).Include(t => t.User);
            if (orderId != null)  q = q.Where(t => t.OrderID == orderId);
            if (shopId != null)   q = q.Where(t => t.Order.ShopID == shopId);
            return q;
        }

        /// <summary>新增评论</summary>
        public static OrderRate Add(long orderId, RateType rate, string comment, long? userId=null, List<string> images=null)
        {
            var r = new OrderRate();
            r.OrderID = orderId;
            r.Rate = (int)rate;
            r.Comment = comment;
            r.CreateDt = DateTime.Now;
            if (userId == null)
            {
                var order = Order.Get(orderId);
                userId = order?.UserID;
            }
            r.UserID = userId;
            r.Save();

            // 增加附件
            if (images != null)
                r.AddRes(images);
            return r;
        }
    }
}