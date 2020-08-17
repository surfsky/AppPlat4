using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
//using EntityFramework;
//using EntityFramework.Extensions;
using Newtonsoft.Json;
using App.Utils;
using App.Wechats;
using App.Components;
using App.Entities;

namespace App.DAL
{
    /// <summary>广告位置</summary>
    public enum AdvertPlace : int
    {
        [UI("顶部广告栏")]  MainTopBanner = 0,
        [UI("主屏弹窗")]    MainPopup = 1
    }

    /// <summary>广告状态</summary>
    public enum AdvertStatus : int
    {
        [UI("已创建")]  Created = 0,
        [UI("已激活")]  Active = 1,
        [UI("已过期")]  Expired = 2,
    }

    /// <summary>
    /// 广告表（展示在客户端广告，有位置、图文、有效期、关联商品文章等属性，可以独立运行存在。）
    /// </summary>
    [UI("商城", "广告")]
    public class Advert : EntityBase<Advert>, IDeleteLogic
    {
        // 位置及状态
        [UI("是否在用")]          public bool? InUsed { get; set; } = true;
        [UI("类别")]              public AdvertPlace? Place { get; set; }
        [UI("状态")]              public AdvertStatus?  Status { get; set; }     // 需定时逻辑去检测是否在用状态
        [UI("位置")]              public string  PlaceName { get { return this.Place.GetTitle(); } }
        [UI("状态")]              public string  StatusName { get { return this.Status.GetTitle(); } }

        // 广告默认就是带了图文标题图片的
        [UI("标题")]              public string Title { get; set; }
        [UI("内容")]              public string Body { get; set; }
        [UI("封面图片")]          public string CoverImage { get; set; }
        [UI("作者")]              public long?  CreatorID { get; set; }
        [UI("顺序")]              public int?   Seq { get; set; }

        // 关联
        [UI("门店")]              public long?   ShopID { get; set; }
        [UI("关联文章")]          public long?   ArticleID { get; set; }
        [UI("关联商品")]          public long?   ProductID { get; set; }

        // 启用时间段和状态
        [UI("开始时间")]          public DateTime? StartDt { get; set; }
        [UI("结束时间")]          public DateTime? EndDt { get; set; }

        // 导航属性
        public virtual User Creator { get; set; }
        public virtual Shop Shop { get; set; }
        public virtual Product Product { get; set; }
        public virtual Article Article { get; set; }



        //-------------------------------------------
        // 方法
        //-------------------------------------------
        public override object Export(ExportMode type=ExportMode.Normal)
        {
            return new
            {
                this.ID,
                this.Title,
                this.CoverImage,
                this.Status,
                this.StatusName,
                this.Seq,
                this.CreatorID,
                Creator = this.Creator?.NickName,
                this.ShopID,
                ShopName = this.Shop?.AbbrName,
                this.ProductID,
                ProductName = this.Product?.Name,
                this.ArticleID,
                ArticleTitle = this.Article?.Title,
                this.CreateDt,
                this.StartDt,
                this.EndDt,
                Images = type.HasFlag(ExportMode.Detail) ? this.Images : null,
                Body = type.HasFlag(ExportMode.Detail) ? this.Body : null,
            };
        }

        /// <summary>查找</summary>
        public static IQueryable<Advert> Search(AdvertPlace? place, long? shopId = null, AdvertStatus? status = null, string title = "")
        {
            IQueryable<Advert> q = Set.Include(t => t.Shop).Include(t => t.Product).Include(t => t.Article);
            q = q.Where(t => t.InUsed != false);
            if (shopId != null)         q = q.Where(t => t.ShopID == shopId);
            if (place != null)          q = q.Where(t => t.Place == place);
            if (title.IsNotEmpty())     q = q.Where(t => t.Title.Contains(title));
            if (status != null)         q = q.Where(t => t.Status == status);
            return q.Sort(t => t.Seq);
        }

        /// <summary>修正状态</summary>
        public override object FixItem()
        {
            if (this.Status == AdvertStatus.Expired)
                return this;
            var now = DateTime.Now;
            if (StartDt != null && EndDt != null)
            {
                if (now >= StartDt && now <= EndDt) Status = AdvertStatus.Active;
                if (now > EndDt)                    Status = AdvertStatus.Expired;
            }
            this.Save();
            return this;
        }
    }
}