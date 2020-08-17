using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using EntityFramework.Extensions;
using System.Data.Entity;
using System.ComponentModel;
using App.Utils;
using System.Linq.Expressions;
using App.Entities;
using Z.EntityFramework.Plus;

namespace App.DAL
{
    /// <summary>
    /// 订单行对应的资产
    /// </summary>
    [UI("商城", "订单细项对应的资产")]
    public class OrderItemAsset : EntityBase<OrderItemAsset>
    {
        [UI("用户")]     public long? UserID { get; set; }
        [UI("订单行")]   public long? OrderItemID { get; set; }
        [UI("资产")]     public long? AssetID { get; set; }

        public virtual User User { get; set; }
        public virtual OrderItem OrderItem { get; set; }
        public virtual UserAsset Asset { get; set; }

        /// <summary>批量删除</summary>
        public static void DeleteBatch(long orderItemId)
        {
            Set.Where(t => t.OrderItemID == orderItemId).Delete();
        }

        /// <summary>导出</summary>
        public override object Export(ExportMode type = ExportMode.Normal)
        {
            return new
            {
                this.ID,
                this.UserID,
                this.OrderItemID,
                this.OrderItem?.OrderID,
                this.AssetID,
                this.Asset?.Name,
                this.Asset?.Remark,
                this.Asset?.SerialNo,
                this.Asset?.InsuranceStartDt,
                this.Asset?.InsuranceEndDt,
            };
        }

        // 查询
        public static IQueryable<OrderItemAsset> Search(
            long? userId = null, string userName = null,
            long? orderItemId = null,
            string serialNo = null
            )
        {
            IQueryable<OrderItemAsset>  q = Set.Include(t => t.User).Include(t => t.OrderItem).Include(t => t.Asset);
            if (userName.IsNotEmpty())  q = q.Where(t => t.User.NickName.Contains(userName));
            if (orderItemId != null)    q = q.Where(t => t.OrderItemID == orderItemId);
            if (userId != null)         q = q.Where(t => t.UserID == userId);
            if (serialNo.IsNotEmpty())  q = q.Where(t => t.Asset.SerialNo == serialNo);
            return q;
        }
    }

    /// <summary>
    /// 用户资产信息表（可以关联到订单项的扩展表）
    /// </summary>
    public class UserAsset : EntityBase<UserAsset>
    {
        [UI("用户")]         public long? UserID { get; set; }
        [UI("名称")]         public string Name { get; set; }
        [UI("串号")]         public string SerialNo { get; set; }
        [UI("备注")]         public string Remark { get; set; }

        [UI("维保开始时间")] public DateTime? InsuranceStartDt { get; set; }
        [UI("维保结束时间")] public DateTime? InsuranceEndDt { get; set; }

        // 冗余字段
        [UI("销售商")]       public long? ShopID { get; set; }

        // 关联属性
        [UI("用户")]         public virtual User User { get; set; }
        [UI("销售商")]       public virtual Shop Shop { get; set; }


        // 计算属性
        [UI("是否在维保期内")]
        public bool InInsurance
        {
            get
            {
                if (InsuranceEndDt == null)
                    return false;
                return DateTime.Now <= InsuranceEndDt;
            }
        }


        // 导出
        public override object Export(ExportMode type = ExportMode.Normal)
        {
            return new
            {
                this.ID,
                this.Name,
                this.Remark,
                this.SerialNo,
                this.InsuranceStartDt,
                this.InsuranceEndDt,
                ShopAbbrName = this.Shop?.AbbrName,

                UserID          = type.HasFlag(ExportMode.Detail) ? this.UserID : null,
                CreateDt        = type.HasFlag(ExportMode.Detail) ? this.CreateDt : null,
                ShopID         = type.HasFlag(ExportMode.Detail) ? this.ShopID : null,
                UserNickName    = type.HasFlag(ExportMode.Detail) ? this.User?.NickName : null,
                Shop           = type.HasFlag(ExportMode.Detail) ? this.Shop?.Name : null,
            };
        }


        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        /// <summary>获取详情</summary>
        public new static UserAsset GetDetail(long id)
        {
            return Set
                .Include(t => t.User)
                .Include(t => t.Shop)
                .Where(t => t.ID == id)
                .FirstOrDefault()
                ;
        }


        // 查询
        public static IQueryable<UserAsset> Search(
            long? userId = null, string userName=null,
            DateTime? startDt = null, 
            DateTime? endDt = null,
            string serialNo = null
            )
        {
            IQueryable<UserAsset> q = Set.Include(t => t.User).Include(t=>t.Shop);
            if (userName.IsNotEmpty())            q = q.Where(t => t.User.NickName.Contains(userName));
            if (userId != null)                   q = q.Where(t => t.UserID == userId);
            if (startDt != null)                  q = q.Where(t => t.CreateDt >= startDt);
            if (endDt != null)                    q = q.Where(t => t.CreateDt <= endDt);
            if (serialNo.IsNotEmpty())            q = q.Where(t => t.SerialNo == serialNo);
            return q;
        }

        /// <summary>新增用户资产</summary>
        public static UserAsset Add(
            long userId, string name, string serialNo, string remark, long? shopId,
            long? orderItemId, DateTime? insuranceStartDt)
        {
            var oi = OrderItem.GetDetail(orderItemId ?? -1);
            var o = new UserAsset();
            o.UserID = userId;
            o.Name = name;
            o.SerialNo = serialNo;
            o.Remark = remark;
            o.CreateDt = DateTime.Now;
            o.ShopID = shopId;
            o.InsuranceStartDt = insuranceStartDt;
            if (oi != null)
            {
                o.ShopID = oi.Order?.ShopID;
            }
            o.Save();

            // 维保时间
            if (insuranceStartDt != null)
                o.SetInsurance(insuranceStartDt.Value, oi.ProductSpec);
            return o;
        }

        /// <summary>设置维保日期范围</summary>
        public UserAsset SetInsurance(DateTime startDt, ProductSpec spec)
        {
            var days = spec.InsuranceDays ?? 60;
            this.InsuranceStartDt = startDt;
            this.InsuranceStartDt = startDt.AddDays(days);
            return this.Save();
        }

        /// <summary>续保（延长保修结束日期）</summary>
        public UserAsset AddInsurance(ProductSpec spec)
        {
            var today = DateTime.Today;
            int days = spec.InsuranceDays ?? 60;
            if (this.InsuranceStartDt == null)
                this.InsuranceStartDt = today;
            var endDt = this.InsuranceEndDt ?? today;
            this.InsuranceEndDt = endDt.AddDays(days);
            return this.Save();
        }
    }
}