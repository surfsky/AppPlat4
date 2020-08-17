using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data.Entity;
using System.ComponentModel;
using System.Linq.Expressions;
using App.Utils;
using App.Entities;

namespace App.DAL
{
    /// <summary>
    /// 订单细项表（对应一个产品规格，多个用户资产）
    /// </summary>
    [UI("商城", "订单细项")]
    public class OrderItem : EntityBase<OrderItem>
    {
        // 基础信息
        [UI("明目")]                   public string Title { get; set; }
        [UI("订单")]                   public long? OrderID { get; set; }
        [UI("商品规格")]               public long? ProductSpecID { get; set; }
        [UI("数量")]                   public int? Amount { get; set; }
        [UI("金额")]                   public double? Money { get; set; }

        // 冗余信息（都可以从 ProductSpecID 推导出）
        [UI("订单序列号")]             public string OrderSerialNo { get; set; }
        [UI("商品类别")]               public ProductType? ProductType { get; set; }
        [UI("商品")]                   public long? ProductID { get; set; }
        [UI("商品名称")]               public string ProductName { get; set; }
        [UI("商品规格名称")]           public string ProductSpecName { get; set; }
        [UI("商品规格编码")]           public string ProductSpecCode { get; set; }
        [UI("单价")]                   public double? Price { get; set; }


        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
        public virtual ProductSpec ProductSpec { get; set; }

        public List<UserAsset> GetAssets()
        {
            return OrderItemAsset.Search(orderItemId: this.ID).ToList().Cast(t => t.Asset);
        }

        public override object Export(ExportMode type = ExportMode.Normal)
        {
            return new
            {
                this.ID,
                this.OrderID,
                this.OrderSerialNo,
                this.Title,
                this.Price,
                this.Amount,
                this.Money,
                this.ProductID,
                this.ProductSpecCode,
                this.ProductSpecID,
                Assets = type.HasFlag(ExportMode.Detail) ? GetAssets().Cast(t => t.Export()) : null,
                Type = this.Product?.Type,
                ProductName = this.Product?.Name,
                ProductImage = this.Product?.CoverImage,
                ProductSpec = this.ProductSpec?.Name,
                ProductSpecImage = this.ProductSpec?.CoverImage
            };
        }



        //-----------------------------------------------
        // 实例方法
        //-----------------------------------------------
        /// <summary>获取详情</summary>
        public new static OrderItem GetDetail(long id)
        {
            var item = Set.Include(t => t.Order).Include(t => t.ProductSpec).Include(t => t.Product).FirstOrDefault(t => t.ID == id);
            return item?.FixItem();
        }

        /// <summary>填补冗余字段数据</summary>
        new OrderItem FixItem()
        {
            var spec = this.ProductSpec ?? ProductSpec.GetDetail(this.ProductSpecID.Value);
            if (spec != null)
            {
                this.ProductID = spec.ProductID;
                this.ProductName = spec.Product.Name;
                this.ProductType = spec.Product.Type;
                this.ProductSpecCode = spec.Code;
                this.ProductSpecName = spec.Name;
                this.ProductSpec = spec;
                this.Product = spec.Product;
                this.Save();
            }
            return this;
        }


        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        // 查找
        public static IQueryable<OrderItem> Search(long? orderId, string title="", long? productId=null)
        {
            IQueryable<OrderItem> q = Set.Include(t => t.ProductSpec).Include(t => t.Product);
            if (orderId != null)        q = q.Where(t => t.OrderID == orderId);
            if (productId != null)      q = q.Where(t => t.ProductID == productId);
            if (!title.IsEmpty()) q = q.Where(t => t.Title.Contains(title));
            return q;
        }

        // 重载保存方法
        public new OrderItem Save(bool isNew)
        {
            // 填写推导出来的信息
            var spec = ProductSpec.GetDetail(this.ProductSpecID.Value);
            if (spec != null)
            {
                this.ProductID = spec.ProductID;
                this.ProductType = spec.Product.Type;
                this.ProductName = spec.Product.Name;
                this.ProductSpecName = spec.Name;
                this.ProductSpecCode = spec.Code;
                this.Price = spec.Price;

                // 修改商品销售信息
                if (isNew)
                    spec.Sale(this.Amount.Value);
            }
            this.Save();

            // 修改订单统计信息
            var order = Order.Get(this.OrderID);
            this.OrderSerialNo = order.SerialNo;
            order?.UpdateStat();
            return this;
        }

    }
}