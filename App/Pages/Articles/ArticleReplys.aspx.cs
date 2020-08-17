using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
//using EntityFramework.Extensions;
using App;
using App.DAL;
using App.Utils;
using App.Components;
using App.Controls;

namespace App.Pages
{
    [UI("回帖统一管理")]
    [Auth(Powers.ArticleView)]
    public partial class ArticleReplys : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Grid1
                .SetPowers(Powers.ArticleView, false, Powers.ArticleEdit, Powers.ArticleDelete)
                .SetUrls("ArticleForm.aspx")
                .AddThrumbnailColumn<DAL.Article>(t => t.CoverImage, 40, "图片")
                .AddColumn<DAL.Article>(t => t.Title, 300, "标题")
                .AddColumn<DAL.Article>(t => t.TypeName, 70, "类别")
                .AddColumn<DAL.Article>(t => t.Dir.FullName, 200, "目录")
                .AddColumn<DAL.Article>(t => t.AuthorName, 100, "作者")
                .AddColumn<DAL.Article>(t => t.CreateDt, 100, "日期", "{0:yyyy-MM-dd}")
                .AddColumn<DAL.Article>(t => t.VisitCnt, 60, "访问")
                .AddColumn<DAL.Article>(t => t.ApprovalCnt, 60, "点赞")
                .AddColumn<DAL.Article>(t => t.ReplyCnt, 60, "回帖")
                .AddCheckColumn<Article>(t => t.IsValid, 100, "有效评论")
                .AddWindowColumn<DAL.Article>(t => t.ReplyID, null, "Article.aspx?id={0}", 60, "原文", "原文")
                .InitGrid<DAL.Article>(BindGrid, Grid1, t => t.Title)
                ;
            if (!IsPostBack)
            {
                this.Grid1.SetSortPage<DAL.Article>(SiteConfig.Instance.PageSize, t => t.CreateDt, false);
                BindGrid();
            }
        }


        // 绑定网格
        private void BindGrid()
        {
            IQueryable<DAL.Article> q = DAL.Article.Search(ArticleType.Reply);
            Grid1.Bind(q);
        }
    }
}
