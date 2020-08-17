using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using EntityFramework.Extensions;
using System.Data.Entity;
using System.ComponentModel;
using App.Utils;
using System.Collections;
using App.Entities;

namespace App.DAL
{
    [UI("基础", "用户管辖的区域")]
    public class UserArea : EntityBase<UserArea>
    {
        [UI("用户")]     public long? UserID { get; set; }
        [UI("区域")]     public long? AreaID { get; set; }
        [UI("排序")]     public int?  Seq { get; set; }

        public virtual User User { get; set; }
        public virtual Area Area { get; set; }

        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        /// <summary>导出</summary>
        public override object Export(ExportMode type = ExportMode.Normal)
        {
            return new
            {
                this.ID,
                this.UserID,
                this.User?.NickName,
                this.AreaID,
                this.Area?.Name,
                this.Seq
            };
        }

        /// <summary>获取详情</summary>
        public new static UserArea GetDetail(long id)
        {
            return Set.Include(t => t.User).Include(t => t.Area).Where(t => t.ID == id).FirstOrDefault();
        }

        /// <summary>检索</summary>
        public static IQueryable<UserArea> Search(
            long? userId = null,
            string userName = null,
            long? areaId = null,
            string areaName = null
            )
        {
            IQueryable<UserArea> q = Set.Include(t => t.User).Include(t => t.Area);
            if (userId.IsNotEmpty())          q = q.Where(t => t.UserID == userId);
            if (userName.IsNotEmpty())        q = q.Where(t => t.User.NickName.Contains(userName));
            if (areaId.IsNotEmpty())          q = q.Where(t => t.AreaID == areaId);
            if (areaName.IsNotEmpty())        q = q.Where(t => t.Area.Name.Contains(areaName));
            return q;
        }

    }
}