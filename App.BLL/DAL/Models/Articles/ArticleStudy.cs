using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.DAL;
using System.Configuration;
using System.Drawing;
//using EntityFramework.Extensions;
using App.Utils;
using App.Components;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entities;

namespace App.DAL
{
    [UI("文档", "阅读记录")]
    public class ArticleStudy : EntityBase<ArticleStudy>
    {
        [UI("文章")] public long? ArticleID { get; set; }
        [UI("用户")] public long? UserID { get; set; }
        [UI("间隔")] public int Interval { get; set; }


        [UI("文章")] public virtual Article Article { get; set; }
        [UI("用户")] public virtual User User { get; set; }

        [UI("哪天"), NotMapped]
        public DateTime? CreateDays
        {
            get
            {
                return CreateDt.Value.Date;
            }
        }

        //-----------------------------------------------
        // 构造函数
        //-----------------------------------------------
        public ArticleStudy() { }
        /// <summary>创建新文章</summary>
        public ArticleStudy(long? articleID, long? userID, int interval)
        {
            this.ArticleID = articleID;
            this.UserID = userID;
            this.Interval = interval;
        }

        /// <summary>阅读记录</summary>
        [Searcher]
        [Param("userId", "用户ID")]
        [Param("deptId", "部门ID", ValueType = typeof(Dept))]
        [Param("startDt", "开始日期")]
        [Param("endDt", "结束日期")]
        [Param("user", "用户")]
        public static IQueryable<DAL.ArticleStudy> Search(long? deptId = null, long? userId = null, 
            string user = "", DateTime? startDt = null, DateTime? endDt = null, ArticleType? type = null, ArticleStatus? status = null)
        {
            IQueryable<ArticleStudy> q = Set;
            var deptIds = Dept.GetDescendants(deptId).Cast(t => t.ID);
            if (deptId != null) q = q.Where(t => deptIds.Contains(t.User.DeptID.Value));
            if (userId != null) q = q.Where(t => t.UserID == userId);
            if (startDt != null) q = q.Where(t => t.CreateDt >= startDt);
            if (endDt != null) q = q.Where(t => t.CreateDt <= endDt);
            if (user.IsNotEmpty()) q = q.Where(t => t.User.NickName.Contains(user));
            if (type.IsNotEmpty()) q = q.Where(t => t.Article.Type == type);
            if (status != null) q = q.Where(t => t.Article.Status == status);

            return q;
        }

    }
}