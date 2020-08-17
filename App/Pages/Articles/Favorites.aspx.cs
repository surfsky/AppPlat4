using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
//using EntityFramework.Extensions;
using App.DAL;
using App.Utils;
using App.Components;
using App.Controls;

namespace App.Pages
{
    [UI("关注管理")]
    [Auth(Powers.FavoriteView, Powers.FavoriteNew, Powers.FavoriteEdit, Powers.FavoriteDelete)]
    public partial class Favorites : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Grid1
                .SetPowers(this.Auth)
                .AddThrumbnailColumn<ArticleDirFavorite>(t => t.ArticleDir.Icon, 40)
                .AddColumn<ArticleDirFavorite>(t => t.ArticleDir.FullName, 400, "知识库目录")
                .AddColumn<ArticleDirFavorite>(t => t.TypeName, 80, "来源")
                .AddColumn<ArticleDirFavorite>(t => t.Dept.Name, 80, "部门")
                .AddColumn<ArticleDirFavorite>(t => t.User.NickName, 80, "用户")
                //.AddCheckColumn<UserFavorite>(t => t.ShowInHome, 80, "在首页显示")
                .AddColumn<ArticleDirFavorite>(t => t.Seq, 50, "排序")
                .AddColumn<ArticleDirFavorite>(t => t.CreateDt, 200, "创建时间")
                //.AddColumn<UserFavorite>(t => t.Remark, 400, "备注")
                .SetUrls("FavoriteForm.aspx")
                .InitGrid<ArticleDirFavorite>(this.BindGrid, Panel1, t => t.CreateDt)
                ;
            if (!IsPostBack)
            {
                UI.BindEnum(ddlType, typeof(FavoriteType), "来源");
                UI.SetVisibleByQuery("search", this.tbUser, this.dpCreate, this.btnSearch);
                this.Grid1.SetSortPage<ArticleDirFavorite>(SiteConfig.Instance.PageSize, t => t.Seq, true);
                BindGrid();
            }
        }


        // Grid
        private void BindGrid()
        {
            var type = UI.GetEnum<FavoriteType>(this.ddlType);
            var userId = Asp.GetQueryLong("userId");
            var user = UI.GetText(tbUser);
            var createDt = UI.GetDate(this.dpCreate);
            IQueryable<ArticleDirFavorite> q = ArticleDirFavorite.Search(
                userId: userId,
                userName:user, 
                type: type,
                startDt: createDt
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
