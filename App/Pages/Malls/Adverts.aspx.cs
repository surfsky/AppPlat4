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
    /*
    <Columns>
        <f:ImageField DataImageUrlField="CoverImage" DataImageUrlFormatString="{0}?w=30" HeaderText="图片" Hidden="false" ImageHeight="30" ImageWidth="30" MinWidth="30" />
        <f:BoundField DataField="Title" SortField="Title" Width="200px" HeaderText="标题" />
        <f:BoundField DataField="Type" SortField="Type" HeaderText="类型"  ColumnID="Type" />
        <f:BoundField DataField="Key" ExpandUnusedSpace="true" HeaderText="键值" />
        <f:BoundField DataField="Seq" SortField="Seq" HeaderText="顺序"/>
        <f:BoundField DataField="InUsed" SortField="InUsed" HeaderText="在用" ColumnID="InUsed" />
        <f:BoundField DataField="CreateDt" SortField="CreateDt" HeaderText="创建日期" Width="200px" />
        <f:BoundField DataField="StartDt" SortField="CreateDt" HeaderText="开始日期" Width="200px" />
        <f:BoundField DataField="EndDt" SortField="CreateDt" HeaderText="结束日期" Width="200px" />
    </Columns>
    */

    [UI("广告管理")]
    [Auth(Powers.AdvertView, Powers.AdvertNew, Powers.AdvertEdit, Powers.AdvertDelete)]
    public partial class Adverts : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Grid1
                .SetPowers(this.Auth)
                .SetUrls("AdvertForm.aspx")
                .AddThrumbnailColumn<Advert>(t => t.CoverImage,  40, "图片")
                .AddColumn<Advert>(t => Title, 200, "标题")
                .AddColumn<Advert>(t => t.PlaceName, 80, "位置")
                .AddColumn<Advert>(t => t.StatusName, 80, "状态")
                .AddColumn<Advert>(t => t.Seq, 80, "顺序")
                .AddColumn<Advert>(t => t.Creator.NickName, 80, "创建者")
                .AddColumn<Advert>(t => t.CreateDt, 100, "创建时间", "{0:yyyy-MM-dd}")
                .AddColumn<Advert>(t => t.StartDt, 100, "启用时间", "{0:yyyy-MM-dd}")
                .AddColumn<Advert>(t => t.EndDt, 100, "结束时间", "{0:yyyy-MM-dd}")
                .AddWindowColumn<Advert>(t => t.Shop.ID, t => t.Shop.AbbrName, "ShopForm.aspx?id={0}&md=view", 100, "门店")
                .AddWindowColumn<Advert>(t => t.ProductID, t => t.Product.Name, "productForm.aspx?id={0}&md=view", 100, "关联商品")
                .AddLinkColumn<Advert>(t => t.ArticleID, t => t.Article.Title, "article.aspx?id={0}&md=view", 100, "关联文章")
                .InitGrid<Advert>(BindGrid, Panel1, t => t.Title)
                ;
            if (!IsPostBack)
            {
                UI.BindEnum(this.ddlType, typeof(AdvertPlace), "--全部类别--");
                UI.BindEnum(this.ddlStatus, typeof(AdvertStatus), "--全部状态--");
                this.Grid1.SetSortPage<Advert>(SiteConfig.Instance.PageSize, t => t.StartDt, false);
                BindGrid();
            }
        }

        // 绑定网格
        private void BindGrid()
        {
            var title = UI.GetText(tbTitle);
            var location = UI.GetEnum<AdvertPlace>(this.ddlType);
            var status = UI.GetEnum<AdvertStatus>(this.ddlStatus);
            IQueryable<Advert> q = Advert.Search(location, null, status, title);
            Grid1.Bind(q);
        }


        // 查找
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}
