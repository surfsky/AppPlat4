using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
using App.Controls;
using App.DAL;
using App.Utils;
using App.Components;

namespace App.Pages
{
    [UI("商品规格管理")]
    [Auth(Powers.ProductView, Powers.ProductNew, Powers.ProductEdit, Powers.ProductDelete)]
    [Param("productId", "产品ID")]
    public partial class ProductSpecs : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Asp.GetQueryString("productId").IsEmpty())
            {
                Asp.Fail("缺少 productId 参数");
                return;
            }

            // 产品规格
            var productId = Asp.GetQueryLong("productId");
            var product = Product.Get(productId);
            this.Grid1
                .SetPowers(this.Auth)
                .SetUrls(
                    "~/Pages/ProductSpecForm.aspx?md=new&productId=" + productId.ToText(),
                    "~/Pages/ProductSpecForm.aspx?md=view&id={0}",
                    "~/Pages/ProductSpecForm.aspx?md=edit&id={0}")
                .InitGrid<ProductSpec>(BindGrid, Grid1, t => t.Name)
                ;
            this.Grid1.FindColumn("Spec1").HeaderText = product.SpecName1.IsEmpty() ? "规格1" : product.SpecName1;
            this.Grid1.FindColumn("Spec2").HeaderText = product.SpecName2.IsEmpty() ? "规格2" : product.SpecName2;
            this.Grid1.FindColumn("Spec3").HeaderText = product.SpecName3.IsEmpty() ? "规格3" : product.SpecName3;
            if (!IsPostBack)
            {
                 this.Grid1.SetSortPage<ProductSpec>(SiteConfig.Instance.PageSize, t => t.Seq, false);
                 BindGrid();
                 UI.SetVisibleByQuery("search");
            }
        }

        // 产品规格表
        private void BindGrid()
        {
            var productId = Asp.GetQueryLong("productId");
            IQueryable<ProductSpec> q = ProductSpec.Search(productId);
            Grid1.Bind(q);
        }

        // 删除（下架）
        protected void Grid1_Delete(object sender, List<long> ids)
        {
            ProductSpec.ChangeShelf(ids, false);
        }
    }
}
