using App.DAL;
using FineUIPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using App.Components;
using App.Controls;
using App.Utils;

namespace App.Pages
{
    [UI("商店管理")]
    [Auth(Powers.ShopView, Powers.ShopNew, Powers.ShopEdit, Powers.ShopDelete)]
    public partial class Shops : PageBase
    {
       /*
        <f:ImageField DataImageUrlField = "CoverImage" HeaderText="图片" Hidden="false" ImageHeight="30" ImageWidth="30" MinWidth="30" />
        <f:BoundField DataField = "City.Name" SortField="City.Name" Width="100px" HeaderText="城市" />
        <f:BoundField DataField = "Name" SortField="Name" Width="300px" HeaderText="名称" />
        <f:BoundField DataField = "AbbrName" SortField="AbbrName" Width="300px" HeaderText="简写" />
        <f:BoundField DataField = "Tel" HeaderText="电话" />
        <f:BoundField DataField = "GPS" HeaderText="GPS位置" />
        <f:BoundField DataField = "Addr" HeaderText="详细地址" ExpandUnusedSpace="true" />
        */
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Grid1
                .SetPowers(this.Auth)
                .SetUrls("ShopForm.aspx")
                .AddThrumbnailColumn<Shop>(t => t.CoverImage, 40, "图片")
                .AddColumn<Shop>(t => t.Area.FullName, 200, "区域")
                .AddColumn<Shop>(t => t.Name, 400, "名称")
                .AddColumn<Shop>(t => t.AbbrName, 300, "简写")
                .AddColumn<Shop>(t => t.Tel, 200, "电话")
                .AddColumn<Shop>(t => t.GPS, 300, "GPS")
                .AddColumn<Shop>(t => t.Addr, 500, "地址")
                .InitGrid<Shop>(this.BindGrid, Panel1, t => t.Name)
                ;
            if (!IsPostBack)
            {
                UI.BindTree(ddlArea, Common.LoginUser.GetAllowedAreas(), t => t.ID, t => t.Name);
                this.Grid1.SetSortPage<Shop>(SiteConfig.Instance.PageSize, t => t.Name, true);
                BindGrid();
                UI.SetVisibleByQuery("search", this.btnSearch, this.tbName);
            }
        }



        // 绑定网格
        private void BindGrid()
        {
            var name = UI.GetText(tbName);
            var areaId = UI.GetLong(ddlArea);
            IQueryable<Shop> q = Shop.Search(name, areaId);
            Grid1.Bind(q);
        }


        // 关闭本窗口
        protected void btnClose_Click(object sender, EventArgs e)
        {
            PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
        }

        // 查找
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}