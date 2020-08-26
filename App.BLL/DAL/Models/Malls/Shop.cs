using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data.Entity;
using App.Utils;
//using EntityFramework.Extensions;
using App.Entities;

namespace App.DAL
{
    /// <summary>
    /// 商店
    /// </summary>
    [UI("商城", "商店")]
    public class Shop : EntityBase<Shop>, IDeleteLogic
    {
        [UI("母店")]                public long? ParentID { get; set; }
        [UI("是否在用")]            public bool? InUsed { get; set; } = true;
        [UI("城市")]                public long? AreaID { get; set; }
        [UI("名称")]                public string Name { get; set; }
        [UI("简写")]                public string AbbrName { get; set; }
        [UI("地址")]                public string Addr { get; set; }
        [UI("GPS(x,y)")]            public string GPS { get; set; }
        [UI("电话")]                public string Tel { get; set; }
        [UI("封面图片")]            public string CoverImage { get; set; } = "~/Res/images/nopicture.jpg";//默认图片
        [UI("说明")]                public string Description { get; set; }

        [UI("城市", ExportMode.None)]
        public virtual Area Area { get; set; }


        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        // 查找
        public static IQueryable<Shop> Search(string name = "", long? areaId = null)
        {
            var areaIds = Area.GetDescendants(areaId).Cast(t => t.ID);

            IQueryable<Shop> q = Set.Include(t => t.Area).Where(t=> t.InUsed != false);
            if (!String.IsNullOrEmpty(name))  q = q.Where(t => t.Name.Contains(name));
            if (areaId != null)               q = q.Where(t => areaIds.Contains(t.AreaID.Value));
            return q;
        }
    }
}