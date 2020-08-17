using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data.Entity;
using Newtonsoft.Json;
//using EntityFramework.Extensions;
using System.Text.RegularExpressions;
using App.Utils;
using System.Reflection;
using App.Entities;

namespace App.DAL
{
    [UI("文档", "文章查看及点评")]
    public class ArticleVisit : EntityBase<ArticleVisit>
    {
        [UI("文章")]   public long? ArticleID { get; set; }
        [UI("用户")]   public long? UserID { get; set; }
        [UI("点赞")]   public bool? Approvel { get; set; }
        [UI("查看数")] public int? VisitCnt { get; set; }


        [UI("文章")] public virtual Article Article { get; set; }
        [UI("用户")] public virtual User User { get; set; }

        //
        [UI("文章标题")] public string ArticleTitle => Article?.Title;
        [UI("用户名")]   public string UserName => User?.NickName;

        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        public override object Export(ExportMode type = ExportMode.Normal)
        {
            // 注：无法给匿名对象加上 UIAttribute，无法给网格页面显示合适的标题
            // 可参考 https://www.cnblogs.com/yjmyzz/archive/2011/11/13/2247600.html
            // 其实这一段代码，编译器自动生成了一个类，调用时只做赋值处理
            // 如果是自己处理的话，需要先创建类，再赋值，就是有点担心性能问题，可考虑将动态创建的类型缓存起来
            var o = new
            {
                this.ID,
                this.ArticleID,
                this.UserID,
                this.Approvel,
                this.Article?.Title,
                this.User?.NickName,
                this.User?.DeptID,
                this.User?.Mobile,
                this.CreateDt,
                this.UpdateDt,
                this.VisitCnt,
                DeptName = this.User?.Dept?.Name,
                ArticleType = this.Article?.Type,
            };
            return o;
            //return this;
        }
        public override UISetting GridUI()
        {
            var ui = new UISetting<ArticleVisit>(false);
            ui.SetColumn(t => t.ID);
            ui.SetColumn(t => t.ArticleID);
            ui.SetColumn(t => t.UserID);
            ui.SetColumn(t => t.Approvel);
            ui.SetColumn(t => t.ArticleTitle);
            ui.SetColumn(t => t.UserName);
            return ui;
        }

        /// <summary>获取文章（并将查看次数增1）</summary>
        public static ArticleVisit Get(long articleId, long userId)
        {
            return Set.FirstOrDefault(t => t.ArticleID == articleId && t.UserID == userId);
        }

        /// <summary>获取或创建访问记录</summary>
        public static ArticleVisit GetOrCreate(long articleId, long userId)
        {
            var item = Get(articleId, userId);
            if (item == null)
                item = new ArticleVisit() { ArticleID = articleId, UserID = userId};
            item.VisitCnt = item.VisitCnt.Inc(1);
            item.Save();
            return item;
        }


        [Searcher]
        [Param("articleId", "文章ID")]
        [Param("userId", "用户ID")]
        [Param("deptId", "部门ID", ValueType=typeof(Dept))]
        [Param("article", "文章标题")]
        [Param("user", "用户昵称")]
        [Param("startDt", "开始日期")]
        [Param("isRequir", "应知应会")]
        public static IQueryable<ArticleVisit> Search(
            long? articleId, long? userId, string article, string user, 
            DateTime? startDt, long? deptId, ArticleType? articleType, List<long> dirIds, bool? isRequir
            )
        {
            var deptIds = Dept.GetDescendants(deptId).Cast(t => t.ID);

            IQueryable<ArticleVisit> q = Set.Include(t => t.Article).Include(t => t.User).Include(t => t.User.Dept);
            if (articleId != null)      q = q.Where(t => t.ArticleID == articleId);
            if (userId != null)         q = q.Where(t => t.UserID == userId);
            if (deptId != null)         q = q.Where(t => deptIds.Contains(t.User.DeptID.Value));
            if (article.IsNotEmpty())   q = q.Where(t => t.Article.Title.Contains(article));
            if (user.IsNotEmpty())      q = q.Where(t => t.User.NickName.Contains(user));
            if (startDt != null)        q = q.Where(t => t.CreateDt > startDt);
            if (articleType != null)    q = q.Where(t => t.Article.Type == articleType);
            if (dirIds.IsNotEmpty())
            {
                dirIds = ArticleDir.GetDescendantIds(dirIds);
                q = q.Where(t => dirIds.Contains(t.Article.DirID.Value));
            }
            if (isRequir != null) q = q.Where(t => t.Article.IsRequir == isRequir);
            return q;
        }

    }
}