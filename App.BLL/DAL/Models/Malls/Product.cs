using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
//using EntityFramework;
//using EntityFramework.Extensions;
using App.Utils;
using App;
using Newtonsoft.Json;
using System.Linq.Expressions;
using App.Entities;
using Z.EntityFramework.Plus;

namespace App.DAL
{
    /// <summary>商品类别（更复杂的话，请用商品目录表替代）</summary>
    public enum ProductType : int
    {
        [UI("维修", typeof(RepairStatus))]    Repair = 1,
        [UI("保险", typeof(InsuranceStatus))] Insurance = 2,
        [UI("真机检验", typeof(CheckStatus))] Check = 3,
        //[UI("商品", typeof(GoodsStatus))]     Goods = 4,
        [UI("积分商品", typeof(GoodsStatus))] Score = 5,
    }



    /// <summary>
    /// 商品概述信息。商品规格价格库存信息位于 ProductSpec 表中。
    /// </summary>
    [UI("商城", "商品")]
    public class Product : EntityBase<Product>
    {
        // 基础属性
        [UI("店ID")]         public long? ShopID { get; set; }
        [UI("商品类型")]     public ProductType? Type { get; set; }
        [UI("名称")]         public string Name { get; set; }
        [UI("描述")]         public string Description { get; set; }
        [UI("图片")]         public string CoverImage { get; set; }
        [UI("条码")]         public string BarCode { get; set; }
        [UI("协议")]         public string Protocol { get; set; }

        // 统一设置和计量
        [UI("是否上架")]     public bool? OnShelf { get; set; } = true;
        [UI("销售数")]       public int? SaleCnt { get; set; } = 0;
        [UI("好评数")]       public int? PositiveCnt { get; set; } = 0;

        // 价格库存
        [UI("价格")]         public double? Price { get; set; } = 0;
        [UI("原价")]         public double? RawPrice { get; set; } = 0;
        [UI("库存")]         public int?    Stock { get; set; } = 9999;

        // 规格名称
        [UI("规格名称1")]    public string SpecName1 { get; set; } = "颜色";
        [UI("规格名称2")]    public string SpecName2 { get; set; } = "尺寸";
        [UI("规格名称3")]    public string SpecName3 { get; set; } = "规格";


        // 导航属性
        [UI("商店")]         public virtual Shop Shop { get; set; }
        [UI("规格")]
        public List<ProductSpec> GetSpecs()
        {
            return ProductSpec.Search(this.ID).OrderBy(t => t.Seq).ToList();
        }

        [NotMapped]
        public string Summary
        {
            get
            {
                string onshelfText = OnShelf.Value ? "" : ", 已下架";
                return string.Format("{0} ({1}{2})", Name, Type.GetTitle(), onshelfText);
            }
        }

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


        public override object Export(ExportMode type=ExportMode.Normal)
        {
            return new
            {
                this.ID,
                this.Type,
                TypeName = this.Type.GetTitle(),
                this.Name,
                this.Summary,
                this.Description,
                this.CreateDt,
                this.CoverImage,
                this.BarCode,
                this.PositiveCnt,
                this.SaleCnt,
                this.ShopID,
                ShopName = this.Shop?.AbbrName,
                this.SpecName1,
                this.SpecName2,
                this.SpecName3,
                this.Price,
                this.RawPrice,
                this.Stock,

                Protocol = type.HasFlag(ExportMode.Detail) ? this.Protocol : null,
                Specs    = type.HasFlag(ExportMode.Detail) ? this.GetSpecs().Cast(t => t.Export(type)) : null,
                Images   = type.HasFlag(ExportMode.Detail) ? this.Images : null
            };
        }

        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        /// <summary>获取产品详情</summary>
        public static Product GetDetail(long? id = null, string barCode = "")
        {
            var q = Set.Include(t => t.Shop);
            if (id != null)
                return q.Where(t => t.ID == id).FirstOrDefault();
            else
                return q.Where(t => t.BarCode == barCode).FirstOrDefault();
        }


        /// <summary>查询</summary>
        [Param("name", "商品名称")]
        [Param("type", "商品类型")]
        [Param("onShelf", "是否上架")]
        public static IQueryable<Product> Search(string name="", ProductType? type=null, bool? onShelf=null)
        {
            IQueryable<Product> q = Set.Include(t => t.Shop);
            if (!name.IsEmpty())   q = q.Where(t => t.Name.Contains(name));
            if (type != null)      q = q.Where(t => t.Type == type);
            if (onShelf != null)   q = q.Where(t => t.OnShelf == onShelf);
            return q;
        }

        /// <summary>上下架</summary>
        public static void ChangeShelf(List<long> ids, bool onShelf)
        {
            Set.Where(t => ids.Contains(t.ID)).Update(t => new Product { OnShelf = onShelf });
        }



        //---------------------------------------------------
        // 创建示例商品
        //---------------------------------------------------
        /// <summary>创建示例商品</summary>
        public static List<Product> CreateProducts(ProductType? type, string imageUrl)
        {
            if (type == ProductType.Repair)
                return CreateRepairProducts(imageUrl);
            if (type == ProductType.Insurance)
                return CreateInsuranceProducts(imageUrl);
            if (type == ProductType.Check)
                return CreateCheckProducts(imageUrl);
            return new List<Product>();
        }

        static List<Product> CreateRepairProducts(string imageUrl)
        {
            var p = new Product();
            p.Name = "手机维修";
            p.Type =  ProductType.Repair;
            p.CreateDt = DateTime.Now;
            p.CoverImage = imageUrl;
            p.Description = "";
            p.BarCode = "";
            p.PositiveCnt = 0;
            p.SaleCnt = 0;
            p.ShopID = null;
            p.SpecName1 = "机型";
            p.SpecName2 = "维修项目";
            p.SpecName3 = "";
            p.Save();

            var spec = new ProductSpec();
            spec.ProductID = p.ID;
            spec.Code = Guid.NewGuid().ToString("N");
            spec.Seq = 0;
            spec.CoverImage = imageUrl;
            spec.Spec1 = "iPhone";
            spec.Spec2 = "换屏";
            spec.Price = 800;
            spec.RawPrice = 1200;
            spec.Stock = 99999;
            spec.Data = 1;
            spec.InsuranceDays = 365;
            spec.Save();

            spec = new ProductSpec();
            spec.ProductID = p.ID;
            spec.Code = Guid.NewGuid().ToString("N");
            spec.Seq = 1;
            spec.CoverImage = imageUrl;
            spec.Spec1 = "iPhone";
            spec.Spec2 = "换电池";
            spec.Price = 200;
            spec.RawPrice = 300;
            spec.Stock = 99999;
            spec.Data = 1;
            spec.InsuranceDays = 365;
            spec.Save();

            return new List<Product>() { p };
        }

        // 创建保险商品
        static List<Product> CreateInsuranceProducts(string imageUrl)
        {
            var p = new Product();
            p.Name = "续保";
            p.Type = ProductType.Insurance;
            p.CreateDt = DateTime.Now;
            p.CoverImage = imageUrl;
            p.Description = "";
            p.BarCode = "";
            p.PositiveCnt = 0;
            p.SaleCnt = 0;
            p.ShopID = null;
            p.SpecName1 = "机型";
            p.SpecName2 = "时长";
            p.SpecName3 = "";
            p.Save();

            var spec = new ProductSpec();
            spec.ProductID = p.ID;
            spec.Code = Guid.NewGuid().ToString("N");
            spec.Seq = 0;
            spec.CoverImage = imageUrl;
            spec.Spec1 = "iPhone";
            spec.Spec2 = "半年";
            spec.Price = 800;
            spec.RawPrice = 800;
            spec.Stock = 99999;
            spec.Data = 182;
            spec.InsuranceDays = 182;
            spec.Save();

            spec = new ProductSpec();
            spec.ProductID = p.ID;
            spec.Code = Guid.NewGuid().ToString("N");
            spec.Seq = 1;
            spec.CoverImage = imageUrl;
            spec.Spec1 = "iPhone";
            spec.Spec2 = "一年";
            spec.Price = 1600;
            spec.RawPrice = 1600;
            spec.Stock = 99999;
            spec.Data = 365;
            spec.InsuranceDays = 365;
            spec.Save();

            return new List<Product>() { p };
        }

        // 创建真机校验商品
        static List<Product> CreateCheckProducts(string imageUrl)
        {
            var p = new Product();
            p.Name = "真机验证";
            p.Type = ProductType.Check;
            p.CreateDt = DateTime.Now;
            p.CoverImage = imageUrl;
            p.Description = "";
            p.BarCode = "";
            p.PositiveCnt = 0;
            p.SaleCnt = 0;
            p.ShopID = null;
            p.SpecName1 = "机型";
            p.SpecName2 = "";
            p.SpecName3 = "";
            p.Save();

            var spec = new ProductSpec();
            spec.ProductID = p.ID;
            spec.Code = Guid.NewGuid().ToString("N");
            spec.Seq = 0;
            spec.CoverImage = imageUrl;
            spec.Spec1 = "iPhone X";
            spec.Spec2 = "";
            spec.Price = 99;
            spec.RawPrice = 199;
            spec.Stock = 99999;
            spec.Data = 1;
            spec.InsuranceDays = 365;
            spec.Save();

            spec = new ProductSpec();
            spec.ProductID = p.ID;
            spec.Code = Guid.NewGuid().ToString("N");
            spec.Seq = 1;
            spec.CoverImage = imageUrl;
            spec.Spec1 = "iPhone XS";
            spec.Spec2 = "";
            spec.Price = 99;
            spec.RawPrice = 199;
            spec.Stock = 99999;
            spec.Data = 1;
            spec.InsuranceDays = 365;
            spec.Save();

            return new List<Product>() { p };
        }
    }
}