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
    [UI("文章访问及点赞管理")]
    [Auth(Powers.ArticleView, Powers.ArticleEdit, Powers.ArticleEdit, Powers.ArticleEdit)]
    public partial class ArticleVisits : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Grid1
                .SetPowers(false, false, false, false)
                //.SetUrls("ArticleVisitForm.aspx")
                //.AddColumn<ArticleVisit>(t => t.Article.Title, 300, "文章")
                //.AddColumn<ArticleVisit>(t => t.User.NickName, 100, "用户")
                .AddLinkColumn<ArticleVisit>(t => t.Article.ID, t => t.Article.Title, "Article.aspx?id={0}", 300, "文章")
                .AddColumn<ArticleVisit>(t => t.Article.Dir.FullName, 300, "目录")
                .AddWindowColumn<ArticleVisit>(t => t.UserID, t => t.User.NickName, "UserForm.aspx?id={0}&md=view", 100, "用户")
                .AddColumn<ArticleVisit>(t => t.User.Dept.Name, 200, "部门")
                .AddColumn<ArticleVisit>(t => t.User.RoleNames, 200, "角色")
                .AddColumn<ArticleVisit>(t => t.User.Mobile, 100, "手机")
                .AddColumn<ArticleVisit>(t => t.CreateDt, 100, "日期", "{0:yyyy-MM-dd}")
                .AddCheckColumn<ArticleVisit>(t => t.Approvel, 100, "是否点赞")
                .AddColumn<ArticleVisit>(t => t.VisitCnt, 100, "查看数")
                .InitGrid<ArticleVisit>(BindGrid, Panel1, t => t.Article.Title)
                .AddCheckColumn<ArticleVisit>(t => t.Article.IsRequir, 100, "是否应知应会")
                ;
            if (!IsPostBack)
            {
                UI.BindBool(ddlIsRequir, "是", "否", "--请选择--", null);
                this.pbArticleDir.UrlTemplate = Urls.ArticleDirs;
                this.pbDept.UrlTemplate = Urls.Depts;
                UI.SetVisibleByQuery("search", this.btnSearch,  this.tbArticle, this.tbUser, this.pbArticleDir);
                this.Grid1.SetSortPage<ArticleVisit>(SiteConfig.Instance.PageSize, t => t.CreateDt, false);
                BindGrid();
            }
        }


        // 绑定网格
        private void BindGrid()
        {
            Grid1.Bind(Query());
        }

        private IQueryable<ArticleVisit> Query()
        {
            var article = UI.GetText(this.tbArticle);
            var user = UI.GetText(this.tbUser);
            var startDt = UI.GetDate(this.dpStart);
            var articleType = ArticleType.Knowledge;
            var deptId = UI.GetLong(this.pbDept);
            var articleDirIds = UI.GetLongs(pbArticleDir);
            var isRequir = UI.GetBool(ddlIsRequir);

            IQueryable<ArticleVisit> q = ArticleVisit.Search(null, null, article, user, startDt, deptId, articleType, articleDirIds, isRequir);
            return q;
        }

        // 查找
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        // 导出
        protected void btnExport_Click(object sender, EventArgs e)
        {
            var data = Query().Sort(this.Grid1).ToList().Cast(t => t.Export());
            var fileName = string.Format("访问日志-{0:yyyyMMddHHmm}.xls", DateTime.Now);
            this.Grid1.ExportExcel(data, fileName);
        }
    }
}
