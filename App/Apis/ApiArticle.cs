using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Net;
using System.ComponentModel;
using System.Text;
using App.Utils;
using App.HttpApi;
using App.Components;
using App.DAL;
using System.IO;
using App.Entities;

namespace App.Apis
{
    [Scope("Article")]
    [Description("文档管理")]
    public class ApiKnowledge
    {
        //--------------------------------------------
        // 关键字
        //--------------------------------------------
        [HttpApi("设置用户关键字", true)]
        public static APIResult SetUserKeywords(string keywords)
        {
            var user = User.GetDetail(Common.LoginUser.ID);
            user.Keywords = keywords.SplitString().Distinct().ToSeparatedString();
            user.Save();
            Common.RefreshLoginUser();
            return user.ToResult();
        }

        [HttpApi("获取热点关键字", true)]
        public static APIResult GetHotKeywords()
        {
            var keywords = ArticleConfig.Instance.Keywords.SplitString();
            return keywords.ToResult();
        }


        //--------------------------------------------
        // 知识库检索
        //--------------------------------------------
        [HttpApi("获取知识库目录", true)]
        public static APIResult GetArticleDirs(long? rootId=null)
        {
            // 请求的目录（包括子目录信息）
            var tree = ArticleDir.All.CloneTree();
            var item = tree.FirstOrDefault(t => t.ParentID == null);
            if (rootId.IsNotEmpty())
                item = tree.FirstOrDefault(t => t.ID == rootId);

            // 检测目录可用性
            var ids = Common.LoginUser.GetAllowedArticleDirIds();  // 许可的目录ID
            ArticleDir.CheckArticleDir(item, ids);
            var items = new List<ArticleDir>() { item };    // 封装成列表给客户端
            return items.ToResult(ExportMode.Normal);
        }


        [HttpApi("检索文章", true)]
        [HttpParam("dirIds", "目录ID，以逗号分隔")]
        public static APIResult GetArticles(string keywords="", string dirIds="", ArticleSortType? sort = null, int pageIndex = 0, int pageSize = 50)
        {
            // 如果用户无授权目录，直接返回空
            var ids = Common.LoginUser.GetAllowedArticleDirIds();
            if (ids.IsEmpty())
                return new List<Article>().ToResult();

            // 如果用户有授权目录，则与请求目录做交集，如果为空则返回空
            var ids2 = dirIds.SplitLong();
            if (ids2.IsNotEmpty())
            {
                ids = ids.Intersect(ids2).ToList();
                if (ids.IsEmpty())
                    return new List<Article>().ToResult();
            }

            // 授权目录存在
            sort = sort ?? ArticleSortType.Date;
            var items = Article.SearchKnowledges(keywords, ids, sort, pageIndex, pageSize);
            return items.ToResult(ExportMode.Normal);
        }

        [HttpApi("获取文章详情", true, CacheSeconds = 60)]
        public static APIResult GetArticle(long articleId)
        {
            var item = Article.GetDetail(articleId, Common.LoginUser?.ID);
            var approval = Article.HasApproval(articleId, Common.LoginUser.ID);

            // 修改附件地址
            if (item.Reses != null)
                foreach (var r in item.Reses)
                    r.Content = Urls.GetDownUrl(r.ID);

            // 采用JObject的方式增加输出属性
            var exp = item.Export(ExportMode.Detail);
            return exp.AddJProperty("Approval", approval).ToResult();
        }


        [HttpApi("获取资源转化的图片列表", true, CacheSeconds = 60)]
        public static APIResult GetResImages(long resId)
        {
            var res = Res.Get(resId);
            var sourceFile = res.Content;
            var targetFolder = string.Format("{0}/{1}", FileCacher.CachePath, sourceFile.MD5());
            var urls = OfficeHelper.MakeOfficeImages(sourceFile, targetFolder);
            urls = urls.Each(t => t = Urls.GetDownUrl(t));
            return urls.ToResult();
        }


        [HttpApi("当前用户是否点赞过这批文章", true)]
        public static APIResult GetArticleApprovals(string articleIds)
        {
            var ids = articleIds.SplitLong();
            var dict = new Dictionary<long, bool>();
            foreach (var id in ids)
            {
                bool approval = Article.HasApproval(id, Common.LoginUser.ID);
                dict.Add(id, approval);
            }
            return dict.ToResult();
        }

        //--------------------------------------------
        // 文章点赞
        //--------------------------------------------
        [HttpApi("文章点赞", true)]
        [HttpParam("articleId", "普通文章、回帖、提问的ID")]
        public static APIResult ArticleApproval(long articleId, bool approval)
        {
            var item = Article.GetDetail(articleId, null);
            item.Approval(Common.LoginUser.ID, approval);
            return item.Export(ExportMode.Detail).AddJProperty("Approval", approval).ToResult();
        }

        //--------------------------------------------
        // 全文检索附件
        //--------------------------------------------
        [HttpApi("全文检索附件", CacheSeconds = 60)]
        public static APIResult GetDocs(string keywords, int pageIndex=0, int pageSize = 100)
        {
            // var root   = @"c:\";
            // var folder = @"c:\files\"; 
            // 经测试 Windows server 2008 R2 多关键字全文检索搞不定，故此处暂时只取第一个关键字
            var root = Asp.MapPath("~/");
            var folder = Asp.MapPath("~/Files/Articles/");
            var keys = keywords.SplitString().Take(1).ToList();
            if (keys.Count == 0)
                throw new Exception("请输入关键字");
            var items = WinSearch.Search(folder, keys, pageIndex, pageSize)
                .Select(t => new 
                {
                    t.ItemName         ,
                    t.ItemType         ,
                    t.KindText         ,
                    t.FileName         ,
                    t.DateModified     ,
                    t.SearchSummary    ,
                    t.SearchRank       ,
                    t.SearchHitCount   ,
                    Size = t.Size.ToSizeText(),
                    ItemUrl = t.ItemPathDisplay.ToRelativePath(root).ToWebPath(),
                });
            return items.ToResult();
        }



        //--------------------------------------------
        // 文章评论
        //--------------------------------------------
        [HttpApi("获取文章回帖列表", true)]
        public static APIResult GetArticleReplies(long? articleId, int pageIndex = 0, int pageSize = 100, string articleDirIds="")
        {
            if (articleId == null)
            {
                var dirIds = articleDirIds.SplitLong();
                var items = Article.Search(ArticleType.Reply, dirIds: dirIds).SortPage(t => t.CreateDt, false, pageIndex, pageSize).ToList();
                return items.ToResult();
            }
            else
            {
                List<Article> items = Article.Get(articleId)?.GetReplies();
                return items.ToResult();
            }
        }

        [HttpApi("删除回帖", true)]
        public static APIResult DeleteArticleReply(long id)
        {
            if (!Common.LoginUser.HasPower(Powers.ArticleDelete))
                throw new HttpException("无此操作权限");
            Article.Delete(id);
            return new APIResult(true, "回帖删除成功");
        }


        [HttpApi("文章回帖", true)]
        [HttpParam("articleId", "普通文章、回帖、提问的ID")]
        [HttpParam("img1", "Base64 图片文本或已上传的图片 url")]
        public static APIResult ArticleReply(long articleId, string comment, string img1, string img2, string img3)
        {
            var article = Article.Get(articleId);
            if (article == null)
                throw new Exception("未找到该文章");

            var reply = article.AddReply(comment, Common.LoginUser);
            var images = Uploader.UploadBase64Images("Articles", new string[] { img1, img2, img3 });
            reply.AddRes(images);
            return reply.ToResult(ExportMode.Detail);
        }



        //--------------------------------------------
        // FAQ
        //--------------------------------------------
        [HttpApi("获取FAQ列表", true, CacheSeconds = 60)]
        public static APIResult GetFAQs(int pageIndex = 0, int pageSize = 50)
        {
            // 获取 FAQ 文章，及一级评论
            var types = new List<ArticleType>() { ArticleType.FAQBug, ArticleType.FAQRequire, ArticleType.FAQSuggest };
            var items = Article.Search(types: types)
                .Sort(t => t.CreateDt, false)
                .Page(pageIndex, pageSize)
                .ToList()
                ;
            return items.ToResult(ExportMode.Detail);
        }

        [HttpApi("新增FAQ", true)]
        [HttpParam("type", "FAQ类型", Description = "FAQBug | FAQRequirement | FAQSuggestion")]
        [HttpParam("img1", "Base64 图片文本或已上传的图片 url")]
        public static APIResult AddFAQ(ArticleType type, string content, string img1, string img2, string img3)
        {
            var item = new Article(type, content, Common.LoginUser);
            item.Save();
            var images = Uploader.UploadBase64Images("Articles", new string[] { img1, img2, img3 });
            item.AddRes(images);
            return item.ToResult(ExportMode.Detail);
        }



        //--------------------------------------------
        // 关注目录
        //--------------------------------------------
        [HttpApi("获取系统默认目录", CacheSeconds=60)]
        public static APIResult GetSystemFavorites()
        {
            return ArticleDirFavorite.GetSystemFavorites().ToResult();
        }

        [HttpApi("获取部门关注目录", AuthLogin=true)]
        public static APIResult GetDeptFavorites(long? deptId=null)
        {
            deptId = deptId ?? Common.LoginUser.DeptID;
            Util.Assert(deptId != null, "请输入部门ID");
            return ArticleDirFavorite.GetDeptFavorites(deptId.Value).ToResult();
        }

        [HttpApi("获取用户关注目录", true)]
        public static APIResult GetUserFavorites(long? userId=null)
        {
            userId = userId ?? Common.LoginUser.ID;
            return ArticleDirFavorite.GetUserFavorites(userId.Value).ToResult();
        }

        [HttpApi("获取关注目录", true)]
        public static APIResult GetFavorites()
        {
            // 尝试获取用户》部门》系统关注目录
            var user = Common.LoginUser;
            var q = ArticleDirFavorite.Search(t => t.UserID == user.ID);
            if (q.Count() != 0)
                return q.Sort(t=>t.Seq, true).ToResult();

            // 部门关注
            if (user.DeptID != null)
            {
                q = ArticleDirFavorite.Search(deptId: user.DeptID);
                if (q.Count() != 0)
                    return q.Sort(t=>t.Seq, true).ToResult();
            }

            // 系统关注
            q = ArticleDirFavorite.Search(type: FavoriteType.System);
            return q.Sort(t=>t.Seq, true).ToResult();
        }

        [HttpApi("设置用户关注目录", true)]
        public static APIResult SetUserFavorites(string dirIds)
        {
            var user = Common.LoginUser;
            List<long> ids = dirIds.SplitLong();
            ArticleDirFavorite.ClearUserFavorites(user.ID);
            foreach (var id in ids)
                ArticleDirFavorite.Add(FavoriteType.User, user.ID, null, id);

            return ArticleDirFavorite.GetUserFavorites(user.ID).ToResult();
        }

        [HttpApi("阅读文章定时记录", true)]
        [HttpParam("userId", "用户ID")]
        [HttpParam("articleId", "文章ID")]
        [HttpParam("interval", "间隔时间，秒")]
        public static APIResult AddStudyRecords(long articleId,int interval = 30)
        {
            var user = Common.LoginUser;
            var item = new ArticleStudy(userID: user.ID, articleID: articleId, interval: interval);
            item.Save();
            return new APIResult(true, "记录成功");
        }
    }
}