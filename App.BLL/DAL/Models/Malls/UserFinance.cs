using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using EntityFramework.Extensions;
using System.Data.Entity;
using System.ComponentModel;
using App.Utils;
using App.Entities;


namespace App.DAL
{
    /// <summary>
    /// 财务类型
    /// </summary>
    public enum FinanceType : int
    {
        [UI("消费")] Consume = 0,
        [UI("预存")] Prestore = 1,
        [UI("提现")] Withdraw = 2,
        [UI("提成")] Bonus = 3,
    }

    /// <summary>
    /// 用户财务记录
    /// </summary>
    [UI("商城", "用户财务记录")]
    public class UserFinance : EntityBase<UserFinance>
    {
        [UI("类型")]                     public FinanceType? Type { get; set; }
        [UI("用户")]                     public long? UserID { get; set; }
        [UI("订单")]                     public long? OrderID { get; set; }
        [UI("费用")]                     public double? Money { get; set; } = 0;
        [UI("备注")]                     public string Remark { get; set; }

        // 关联属性
        [UI("用户")] public virtual User User { get; set; }
        [UI("订单")] public virtual Order Order { get; set; }


        //----------------------------------------------------
        // 基础操作
        //----------------------------------------------------
        /// <summary>获取详细对象</summary>
        public override object Export(ExportMode type = ExportMode.Normal)
        {
            return new
            {
                this.ID,
                this.Type,
                this.UserID,
                this.CreateDt,
                this.OrderID,
                this.Money,
                this.Remark,
                this.User?.NickName,
                this.Order?.Summary
            };
        }

        /// <summary>获取详细对象</summary>
        public new static UserFinance GetDetail(long id)
        {
            return Set.Include(t => t.User).Include(t => t.Order).FirstOrDefault(t => t.ID == id);
        }


        /// <summary>查询</summary>
        public static IQueryable<UserFinance> Search(
            long? userId = null, string userName = null,
            FinanceType? type = null,
            DateTime? startDt = null,
            DateTime? endDt = null,
            long? orderId = null
            )
        {
            IQueryable<UserFinance> q = Set.Include(t => t.User).Include(t => t.Order);
            if (!userName.IsEmpty()) q = q.Where(t => t.User.NickName.Contains(userName));
            if (userId != null)            q = q.Where(t => t.UserID == userId);
            if (type != null)              q = q.Where(t => t.Type == type);
            if (startDt != null)           q = q.Where(t => t.CreateDt >= startDt);
            if (endDt != null)             q = q.Where(t => t.CreateDt <= endDt);
            if (orderId != null)           q = q.Where(t => t.OrderID == orderId);
            return q;
        }

        /// <summary>新增</summary>
        /// <param name="money">金额（正数为收入，负数为支出）</param>
        public static UserFinance Add(FinanceType type, long? userId, double? money, long? orderId, string remark="")
        {
            var user = User.Get(userId);
            if (user != null)
            {
                // 新增财务记录
                var item = new UserFinance();
                item.Type = type;
                item.UserID = userId;
                item.OrderID = orderId;
                item.Money = money;
                item.Remark = remark;
                item.CreateDt = DateTime.Now;
                item.Save();

                if (money != null)
                    user.CalcFinance(money.Value);
                return item;
            }
            return null;
        }

    }
}