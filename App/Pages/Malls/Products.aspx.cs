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
        <f:ImageField DataImageUrlField="CoverImage" HeaderText="图片" Hidden="false" ImageHeight="30" ImageWidth="30" MinWidth="30" />
        <f:BoundField DataField="Name" SortField="Name" Width="200px" HeaderText="名称" />
        <f:BoundField DataField="Type" SortField="Type" ColumnID="Type" HeaderText="类型"  />
        <f:BoundField DataField="OnShelf" SortField="OnShelf" HeaderText="上架" ColumnID="OnShelf" />
        <f:BoundField DataField="SaleCnt" SortField="SaleCnt" HeaderText="销售次数" />
        <f:BoundField DataField="PositiveCnt" SortField="PositiveCnt" HeaderText="好评数" />
        <f:BoundField DataField="CreateDt" SortField="CreateDt" HeaderText="创建日期" Width="200px" />
        <f:BoundField DataField="SpecName1" HeaderText="规格1标题" />
        <f:BoundField DataField="SpecName2" HeaderText="规格2标题" />
        <f:BoundField DataField="SpecName3" HeaderText="规格3标题" />
        <f:BoundField DataField="Shop.AbbrName" SortField="Shop.AbbrName" Width="200px" HeaderText="店铺" />
        <f:BoundField DataField="Remark" ExpandUnusedSpace="true" HeaderText="备注" />
    </Columns>
     */
    /// <summary>
    /// 健身卡管理页面
    /// </summary>
    [Auth(Powers.ProductView, Powers.ProductNew, Powers.ProductEdit, Powers.ProductDelete)]
    public partial class Products : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Grid1
                .SetPowers(this.Auth)
                .SetUrls("ProductForm.aspx")
                .AddThrumbnailColumn<Product>(t=> t.CoverImage, 40)
                .AddColumn<Product>(t=>t.Name, 200, "名称")

                .AddEnumColumn<Product>(t => t.Type, 80, "类型")
                //.AddFuncColumn(t => (t as Product).Type.GetDescription(), 80, "类型")

                .AddBoolColumn<Product>(t => t.OnShelf, 80, "状态", "上架", "下架")
                //.AddCheckColumn<Product>(t => t.OnShelf, 80, "状态")
                //.AddFuncColumn(t => (t as Product).OnShelf==true ? "上架" : "下架", 80, "状态")

                .AddColumn<Product>(t => t.SaleCnt, 80, "销售次数")
                .AddColumn<Product>(t => t.PositiveCnt, 80, "好评数")
                .AddColumn<Product>(t => t.CreateDt, 200, "创建时间")
                .AddColumn<Product>(t => t.SpecName1, 80, "规格1标题")
                .AddColumn<Product>(t => t.SpecName2, 80, "规格2标题")
                .AddColumn<Product>(t => t.SpecName3, 80, "规格3标题")
                .AddColumn<Product>(t => t.Shop.AbbrName, 80, "店铺")
                .AddColumn<Product>(t => t.Description, 80, "备注")
                .InitGrid<Product>(BindGrid, Panel1, t => t.Name)
                ;
            if (!IsPostBack)
            {
                UI.BindEnum(this.ddlType, typeof(ProductType), "--商品类别--", selectedId: null);
                UI.BindBool(this.ddlOnShelf, "上架", "下架", "--是否上架--", true);
                this.Grid1.SetSortPage<Product>(SiteConfig.Instance.PageSize, t => t.CreateDt, false);
                BindGrid();
                UI.SetVisibleByQuery("search", this.btnSearch, this.tbName, this.ddlType, this.ddlOnShelf);
            }
        }

        // 绑定网格
        private void BindGrid()
        {
            var name = UI.GetText(tbName);
            var type = UI.GetEnum<ProductType>(ddlType);
            var onShelf = UI.GetBool(this.ddlOnShelf);
            IQueryable<Product> q = Product.Search(name:name, type:type, onShelf:onShelf);
            Grid1.Bind(q);
        }


        // 关闭本窗口
        protected void btnClose_Click(object sender, EventArgs e)
        {
            PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
        }

        // 删除
        protected void Grid1_Delete(object sender, List<long> ids)
        {
            Product.ChangeShelf(ids, false);
        }

        // 查找
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}
