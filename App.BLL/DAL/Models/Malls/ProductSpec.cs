//using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Newtonsoft.Json;
using System.Linq;
using System.Data.Entity;
using App.Utils;
using App.Entities;
using Z.EntityFramework.Plus;

namespace App.DAL
{
    /// <summary>
    /// 商品规格价格库存表
    /// </summary>
    [UI("商城", "产品规格")]
    public class ProductSpec : EntityBase<ProductSpec>
    {
        [UI("产品ID")]       public long? ProductID { get; set; }
        [UI("货号")]         public string Code { get; set; } = Guid.NewGuid().ToString("N");
        [UI("顺序")]         public int? Seq { get; set; } = 0;
        [UI("是否上架")]     public bool? OnShelf { get; set; } = true;

        // 规格
        [UI("图片")]         public string CoverImage { get; set; }
        [UI("规格1")]        public string Spec1 { get; set; }
        [UI("规格2")]        public string Spec2 { get; set; }
        [UI("规格3")]        public string Spec3 { get; set; }

        // 价格库存
        [UI("价格")]         public double? Price { get; set; } = 0;
        [UI("原价")]         public double? RawPrice { get; set; } = 0;
        [UI("库存")]         public int?    Stock { get; set; } = 9999;

        // 数量
        [UI("数量")]         public int? Data { get; set; } = 1;
        [UI("质保天数")]     public int? InsuranceDays { get; set; } = 365;

        // 虚拟属性
        [UI("产品")]         public virtual Product Product { get; set; }

        // 计算属性
        [UI("折扣")]
        public string Discount
        {
            get
            {
                if (RawPrice == null || RawPrice == 0.0) return "";
                else return string.Format("{0:0.0}折", Price * 10.0 / RawPrice.Value);
            }
        }

        [UI("名称")]
        public string Name
        {
            get {return string.Format("{0} {1} {2}", Spec1, Spec2, Spec3).Trim();}
        }

        [UI("全称")]
        public string FullName
        {
            get { return string.Format("{0} {1} {2} {3}", Product?.Name, Spec1, Spec2, Spec3).Trim(); }
        }

        [UI("概述2")]
        public string Text
        {
            get { return string.Format("{0} {1} {2}, ￥{3}, 库存{4}", Spec1, Spec2, Spec3, Price, Stock); }
        }


        //-----------------------------------------------
        // 方法
        //-----------------------------------------------
        public override object Export(ExportMode type)
        {
            return new 
            {
                ProductID,
                Code,
                Seq,
                OnShelf,
                CoverImage,
                Spec1,
                Spec2,
                Spec3,
                Price,
                RawPrice,
                Stock,
                Data,
                InsuranceDays,
            };
        }

        /// <summary>上下架</summary>
        public static void ChangeShelf(List<long> ids, bool onShelf)
        {
            Set.Where(t => ids.Contains(t.ID)).Update(t => new ProductSpec { OnShelf = onShelf });
        }


        /// <summary>获取详情</summary>
        public new static ProductSpec GetDetail(long id)
        {
            return Set.Include(t => t.Product).FirstOrDefault(t => t.ID == id);
        }

        /// <summary>查询</summary>
        public static IQueryable<ProductSpec> Search(long? productId)
        {
            IQueryable<ProductSpec> q = Set.Include(t => t.Product);
            if (productId != null) q = q.Where(t => t.ProductID == productId);
            q = q.Where(t => t.OnShelf == true);
            return q;
        }

        /// <summary>销售商品几件</summary>
        public void Sale(int cnt)
        {
            if (cnt > this.Stock)
                throw new Exception(this.FullName + "库存不足");

            // 规格库存减少
            this.Stock = this.Stock.Dec(cnt);
            this.Save();

            // 产品销售增加
            var p = Product.Get(this.ProductID);
            p.SaleCnt = p.SaleCnt.Inc(cnt);
            p.Save();
        }

        /// <summary>回收商品几件</summary>
        public void Recyle(int cnt)
        {
            // 规格库存增加
            this.Stock = this.Stock.Inc(cnt);
            this.Save();

            // 产品销售减少
            var p = Product.Get(this.ProductID);
            p.SaleCnt = p.SaleCnt.Dec(cnt);
            p.Save();
        }
    }
}