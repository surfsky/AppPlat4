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

namespace App.Admins
{
    /*
    <Columns>
        <f:HyperLinkField HeaderText = "" Text="<img src='/res/icon/eye.png'/>" DataNavigateUrlFields="ID" DataNavigateUrlFormatString="Article.aspx?id={0}" Width="30" Target="_blank" ToolTip="查看" />
        <f:ImageField HeaderText = "图片" DataImageUrlField="CoverImage" Width="150px" MinWidth="30" ImageHeight="30" ImageWidth="30" />
        <f:BoundField HeaderText = "类别" DataField="TypeName" SortField="Type" Width="100px" ColumnID="Type" />
        <f:BoundField HeaderText = "标题" DataField="Title" SortField="Title" Width="300px" />
        <f:BoundField HeaderText = "作者" DataField="Author" SortField="Author" Width="100px" />
        <f:BoundField HeaderText = "访问次数" DataField="VisitCnt" SortField="VisitCnt" Width="100px" />
        <f:BoundField HeaderText = "日期" DataField="CreateDt" SortField="PostDt" Width="150px" />
        <f:BoundField HeaderText = "缓存(秒)" DataField="CacheSeconds" Width="100px" />

        <f:BoundField HeaderText = "母版" DataField="MotherID" Width="100px" />
        <f:BoundField HeaderText = "母版插槽" DataField="MotherSlot" Width="100px" />
        <f:BoundField HeaderText = "路由路径" DataField="RoutePath" Width="100px" />
        <f:BoundField HeaderText = "内容摘要" DataField="Summary" SortField="Summary" Width="100px" ExpandUnusedSpace="true" />
    </Columns>
    */

    [UI("文章管理")]
    [Auth(Powers.ArticleView, Powers.ArticleNew, Powers.ArticleEdit, Powers.ArticleDelete)]
    [Param("replyId", "回复文章ID", typeof(long))]
    [Param("type", "文章类型", typeof(ArticleType))]
    [Param("typeGroup", "文章分组", typeof(string), Remark ="Page|Site|FAQ|Doc")]
    public partial class Articles : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            var newUrl = string.Format("ArticleForm.aspx?md=new&replyId={0}&type={1}&typeGroup={2}",
                Asp.GetQueryLong("replyId"),
                Asp.GetQuery<ArticleType>("type"),
                Asp.GetQueryString("typeGroup")
                );
            this.Grid1
                .SetPowers(this.Auth)
                .SetUrls(
                    newUrl,
                    "ArticleForm.aspx?md=view&id={0}",
                    "ArticleForm.aspx?md=edit&id={0}"
                    )
                .AddLinkColumn<DAL.Article>(t => t.ID, null, "article.aspx?id={0}", 50, "查看", "", Icon.Eye)
                .AddWindowColumn<DAL.Article>(t => t.ID, null, "ArticlePush.aspx?id={0}", 60, "推送", "推送")
                .AddThrumbnailColumn<DAL.Article>(t => t.CoverImage, 40, "图片")
                .AddColumn<DAL.Article>(t => t.Title, 300, "标题")
                .AddColumn<DAL.Article>(t => t.TypeName, 70, "类别")
                .AddColumn<DAL.Article>(t => t.Dir.FullName, 200, "目录")
                .AddColumn<DAL.Article>(t => t.AuthorName, 100, "作者")
                .AddColumn<DAL.Article>(t => t.CreateDt, 100, "日期", "{0:yyyy-MM-dd}")
                .AddEnumColumn<DAL.Article>(t => t.Status, 80)
                .AddColumn<DAL.Article>(t => t.VisitCnt, 60, "访问")
                .AddColumn<DAL.Article>(t => t.ApprovalCnt, 60, "点赞")
                .AddColumn<DAL.Article>(t => t.ReplyCnt, 60, "回帖")
                .AddColumn<DAL.Article>(t => t.Weight, 60, "权重")
                .AddColumn<DAL.Article>(t => t.CacheSeconds, 60, "缓存(秒)")
                .InitGrid<DAL.Article>(BindGrid, Panel1, t => t.Title)
                ;
            Grid1.Width = 300;
            Grid1.Height = 400;
            if (!IsPostBack)
            {
                UI.SetVisibleByQuery("search",
                    this.btnSearch, this.tbAuthor, this.tbTitle,
                    this.dpStart, this.dpEnd,
                    this.ddlType, this.pbDir, this.tbKeywords
                    );
                this.Grid1.SetSortPage<DAL.Article>(SiteConfig.Instance.PageSize, t => t.CreateDt, false);

                // 如果有应答ID，表示这是评论
                var replyId = Asp.GetQuery<long>("replyId");
                if (replyId != null)
                {
                    UI.SetValue(this.ddlType, ArticleType.Reply);
                    UI.SetVisible(false, this.ddlType);
                    UI.SetGridColumnVisible(this.Grid1, nameof(DAL.Article.Type), false);
                }

                // 限制了类型
                var typeGroup = Asp.GetQueryString("typeGroup");
                UI.BindEnum(ddlType, typeof(ArticleType), "--类别--", typeGroup);
                if (typeGroup == "FAQ")
                {
                    this.pbDir.Hidden = true;
                }

                // 指定了类型
                var type = Asp.GetQuery<ArticleType>("type");
                if (type != null)
                {
                    UI.SetValue(this.ddlType, type);
                    UI.SetEnable(false, this.ddlType);
                }

                //
                BindGrid();
            }
        }


        // 绑定网格
        private void BindGrid()
        {
            string author = tbAuthor.Text.Trim();
            string title = tbTitle.Text.Trim();
            var startDt = dpStart.SelectedDate;
            var endDt = dpEnd.SelectedDate;
            var dirIds = UI.GetLongs(pbDir);
            var keywords = UI.GetText(tbKeywords);
            var replyId = Asp.GetQuery<long>("replyId");
            var type = UI.GetEnum<ArticleType>(ddlType);
            var types = UI.GetAll<ArticleType>(ddlType);


            IQueryable<DAL.Article> q = DAL.Article.Search(
                type,
                types, author, title, startDt, endDt, 
                replyId: replyId,
                dirIds: dirIds, 
                keywords:keywords
                );
            Grid1.Bind(q);
        }

        // 查找
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}
