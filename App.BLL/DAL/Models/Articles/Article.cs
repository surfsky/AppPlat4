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
using App.Components;
using App.Entities;
using Z.EntityFramework.Plus;

namespace App.DAL
{
    /// <summary>
    /// 内容类别
    /// </summary>
    public enum ArticleType : int
    {
        [UI("Page",    "回帖")]          Reply = -1,
        [UI("Page",    "网页")]          Page = -2,
        [UI("Page",    "网页母版")]      MotherPage = -3,

        //
        [UI("Site",    "网站协议")]      Protocol = 0,
        [UI("Site",    "网站帮助")]      Help = 1,
        [UI("Site",    "网站公告")]      Annoucement = 2,
        [UI("Site",    "网站新闻")]      News = 3,
        [UI("Site",    "网站活动")]      Activity = 4,

        // FAQ
        [UI("FAQ",     "FAQ BUG")]       FAQBug = 20,
        [UI("FAQ",     "FAQ 新需求")]    FAQRequire = 21,
        [UI("FAQ",     "FAQ 建议")]      FAQSuggest = 22,

        //
        [UI("Doc",     "知识")]          Knowledge = 30,   // 2 ->
    }

    [UI("文章排序方式"), Flags]
    public enum ArticleSortType : int
    {
        [UI("时间逆序")]   Date = 1,
        [UI("权重逆序")]   Weight = 2,
        [UI("热度逆序")]   Visit = 4,
        [UI("匹配程度")]   Match = 8,
    }

    [UI("文章状态")]
    public enum ArticleStatus : int
    {
        [UI("草稿")]   Draft   = 1,
        [UI("已发布")] Publish = 2,
        [UI("已过期")] Expire  = 3,
        //[UI("已删除")] Delete  = 4,
    }


    /// <summary>
    /// 简单的内容管理系统，带图片。
    /// </summary>
    [UI("文档", "文章")]
    [Auth(Powers.ArticleView, Powers.ArticleNew, Powers.ArticleEdit, Powers.ArticleDelete)]
    public class Article : EntityBase<Article>, IDeleteLogic
    {
        // 通用
        [UI("类别")]              public ArticleType? Type { get; set; }
        [UI("目录")]              public long? DirID { get; set; }
        [UI("标题")]              public string Title { get; set; }
        [UI("内容")]              public string Body { get; set; }
        [UI("封面图片")]          public string CoverImage { get; set; }
        [UI("作者ID")]            public long?  AuthorID { get; set; }
        [UI("作者")]              public string AuthorName { get; set; }
        [UI("摘要(纯文本)")]      public string Summary { get; set; }
        [UI("关键字")]            public string Keywords { get; set; }
        [UI("权重")]              public int?   Weight { get; set; } = 0;
        [UI("查看数")]            public int?   VisitCnt { get; set; } = 0;
        [UI("点赞数")]            public int?   ApprovalCnt { get; set; } = 0;
        [UI("回帖数")]            public int?   ReplyCnt { get; set; } = 0;

        // 状态
        [UI("是否在用")]          public bool? InUsed { get; set; } = true;
        [UI("状态")]              public ArticleStatus?  Status { get; set; } = ArticleStatus.Publish;
        [UI("过期时间")]          public DateTime?  ExpireDt { get; set; }


        // 评论
        [UI("被评论的文章")]          public long? ReplyID { get; set; }


        // 网页类（母版和路由）
        [UI("RoutePath")]         public string RoutePath { get; set; } = "";
        [UI("母版")]              public long? MotherID { get; set; }
        [UI("母版插槽名")]        public string MotherSlot { get; set; }
        [UI("缓存秒数")]          public int? CacheSeconds { get; set; } = 0;

        // 计算方法
        [UI("正文（纯文本）")]    public string BodyText   {get {return this.Body?.RemoveTag();}}
        [UI("类型")]              public string TypeName   {get {return this.Type.GetTitle();}}


        // 
        [UI("目录")]              public virtual ArticleDir Dir { get; set; }
        [UI("作者")]              public virtual User Author { get; set; }
        [UI("被评论的文章")]      public virtual Article Reply { get; set; }


        [UI("是否必学")]          public bool? IsRequir { get; set; } = false;
        [UI("是否是有效评论")]    public bool? IsValid { get; set; }

        //-----------------------------------------------
        // 构造函数
        //-----------------------------------------------
        public Article() { }
        /// <summary>创建新文章</summary>
        public Article(ArticleType type, string title, User user)
        {
            this.Type = type;
            this.Title = title;
            this.AuthorID = user?.ID;
            this.AuthorName = user?.NickName;
            this.Status = ArticleStatus.Publish;
        }

        //-----------------------------------------------
        // 重载方法
        //-----------------------------------------------
        /// <summary>保存完毕后，将原文章的回帖数增1</summary>
        public override void AfterChange(EntityOp op)
        {
            if (op == EntityOp.New && this.ReplyID != null)
            {
                // 统计并修正回帖数目
                var article = Article.Get(this.ReplyID);
                if (article != null)
                {
                    article.ReplyCnt = article.GetReplies().Count();
                    article.Save();
                }

            }
            // 发送消息
            if (op == EntityOp.New) 
            {
                if (this.Type == ArticleType.Knowledge)
                    Logic.SendNewArticleMsg(this);
                else if (this.Type == ArticleType.Reply)
                    Logic.SendNewReplyMsg(this);
            }
        }



        /// <summary>导出数据</summary>
        public override object Export(ExportMode type = ExportMode.Normal)
        {
            return new
            {
                this.ID,
                this.ReplyID,
                this.AuthorName,
                this.AuthorID,
                this.Author?.Avatar,
                AuthorDept = this.Author?.Dept?.Name,
                this.CreateDt,
                CreateDtText = this.CreateDt?.ToFriendlyText(),
                this.Title,
                this.Summary,
                this.Keywords,
                this.Type,
                this.TypeName,
                this.VisitCnt,
                this.ApprovalCnt,
                this.ReplyCnt,
                this.Weight,
                this.CoverImage,
                this.DirID,
                this.Status,
                Dir      = this.Dir?.FullName,
                Body     = type.HasFlag(ExportMode.Detail) ? this.Body : null,
                BodyText = type.HasFlag(ExportMode.Detail) ? BodyText : null,
                BodyParts= type.HasFlag(ExportMode.Detail) ? ArticlePart.ToParts(this.Body) : null,
                Files    = type.HasFlag(ExportMode.Detail) ? this.Reses?.Cast(t => t.Export(ExportMode.Simple)) : null,
                //Images   = type.HasFlag(ExportType.Detail) ? this.Images?.Cast(t => t.Export(ExportType.Simple)) : null,
                Replies = type.HasFlag(ExportMode.Detail) ? this.GetReplies().Cast(t => t.Export(ExportMode.Detail)) : null,
            };
        }

        /// <summary>批量修正数据</summary>
        public override int Fix()
        {
            int n = 0;
            n += Set.Where(t => t.InUsed == null).Update(t => new Article() { InUsed = true });                                                  // 修正在用信息
            n += Set.Where(t => t.Status == null && t.InUsed != false).Update(t => new Article() { Status = ArticleStatus.Publish });            // 修正发布状态
            n += Set.Where(t => t.Status != ArticleStatus.Expire && t.ExpireDt != null && t.ExpireDt < DateTime.Now).Update(t => new Article() { Status = ArticleStatus.Expire });   // 修正过期状态
            n += IncludeSet.Where(t => t.Type == ArticleType.Reply && t.DirID == null).Update(t => new Article() { DirID = t.Reply.DirID });     // 修正回帖无目录的记录
            return n;
        }

        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        /// <summary>获取文章（并修改查看记录）</summary>
        /// <param name="visitUserId">访问用户ID。若不为空，则记录访问日志</param>
        public static Article GetDetail(long? id, long? visitUserId)
        {
            if (id == null) return null;
            var item = Set.Include(t => t.Dir).Include(t => t.Author).Include(t=> t.Author.Dept).FirstOrDefault(t => t.ID == id);
            if (visitUserId != null)
                item?.Visit(visitUserId);
            return item;
        }


        /// <summary>查阅文章</summary>
        public ArticleVisit Visit(long? userId)
        {
            // 查看数增加
            this.VisitCnt = this.VisitCnt.Inc(1);
            this.Save();
            
            // 增加或更新查看记录
            return (userId == null) ? null : ArticleVisit.GetOrCreate(this.ID, userId.Value);
        }

        // 设置摘要
        public void SetSummary()
        {
            string txt = this.BodyText;
            this.Summary = txt.IsEmpty() ? "" : txt.Substring(0, Math.Min(100, txt.Length));
        }


        /// <summary>查询文章</summary>
        [Searcher]
        [Param("type", "类型")]
        [Param("author", "作者")]
        [Param("keywords", "关键字")]
        public static IQueryable<DAL.Article> Search(
            ArticleType? type=null,
            List<ArticleType> types = null, string author = null, string title = null, 
            DateTime? startDt = null, DateTime? endDt=null, long? replyId = null,
            List<long> dirIds=null, string keywords="", ArticleStatus? status=null
            )
        {
            IQueryable<Article> q = Set.Include(t => t.Dir).Include(t => t.Author).Include(t => t.Author.Dept).Where(t => t.InUsed != false);
            if (type.IsNotEmpty())             q = q.Where(t => t.Type == type);
            else if (types.IsNotEmpty())       q = q.Where(t => types.Contains(t.Type.Value));

            if (author.IsNotEmpty())           q = q.Where(t => t.AuthorName.Contains(author));
            if (title.IsNotEmpty())            q = q.Where(t => t.Title.Contains(title));
            if (startDt != null)               q = q.Where(t => t.CreateDt >= startDt);
            if (endDt != null)                 q = q.Where(t => t.CreateDt <= endDt);
            if (replyId != null)               q = q.Where(t => t.ReplyID == replyId);
            if (status != null)                q = q.Where(t => t.Status == status);

            // 目录
            if (dirIds.IsNotEmpty())
            {
                dirIds = ArticleDir.GetDescendantIds(dirIds);
                q = q.Where(t => dirIds.Contains(t.DirID.Value));
            }
            // 关键字（构建OR语法）
            if (keywords.IsNotEmpty())
            {
                var keys = keywords.SplitString();
                var condition = EFHelper.False<Article>();
                foreach (var key in keys)
                    condition = condition.Or<Article>(t => t.Keywords.Contains(key) || t.Title.Contains(key) || t.Body.Contains(key));
                q = q.Where(condition);
            }
            return q;
        }

        /// <summary>查询知识库文章</summary>
        public static List<Article> SearchKnowledges(string keywords, List<long> dirIds, ArticleSortType? sort, int pageIndex = 0, int pageSize = 50)
        {
            var q = Search(type: ArticleType.Knowledge, keywords: keywords, dirIds : dirIds, status: ArticleStatus.Publish);
            if (sort == null)                                      q = q.Sort(t => t.CreateDt, false);
            else if (sort.Value.HasFlag(ArticleSortType.Date))     q = q.Sort(t => t.CreateDt, false);
            else if (sort.Value.HasFlag(ArticleSortType.Visit))    q = q.Sort(t => t.VisitCnt, false);
            else if (sort.Value.HasFlag(ArticleSortType.Weight))   q = q.Sort(t => t.Weight,   false);
            else                                                   q = q.Sort(t => t.CreateDt, false);
            q = q.Page(pageIndex, pageSize);
            return q.ToList();
        }

        //-----------------------------------------------
        // 点赞
        //-----------------------------------------------
        /// <summary>点赞文章</summary>
        public Article Approval(long userId, bool approval)
        {
            // 文章点赞数增减
            this.ApprovalCnt = approval ? this.ApprovalCnt.Inc() : this.ApprovalCnt.Dec();
            this.Save();

            // 增加或修改文章查看记录
            var item = ArticleVisit.GetOrCreate(this.ID, userId);
            item.Approvel = approval;
            item.Save();
            return this;
        }

        /// <summary>获取用户对此文章的点评状态</summary>
        public static bool HasApproval(long articleId, long userId)
        {
            var item = ArticleVisit.Get(articleId, userId);
            return (item?.Approvel) ?? false;
        }

        //-----------------------------------------------
        // 回帖
        //-----------------------------------------------
        /// <summary>获取回帖</summary>
        public List<Article> GetReplies()
        {
            return Article.Search(type: ArticleType.Reply, replyId: this.ID).ToList();
        }

        /// <summary>新增回帖（评论）</summary>
        public Article AddReply(string comment, User user)
        {
            var item = new Article(ArticleType.Reply, comment, user);
            item.DirID = this.DirID;
            item.ReplyID = this.ID;
            item.Save();
            return item;
        }

    }
}