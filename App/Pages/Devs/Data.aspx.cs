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
using App.Components;
using App.Controls;
using App.Utils;
using System.Reflection;
using System.Linq.Expressions;

namespace App.Pages
{
    [UI("数据模型列表")]
    [Auth(Powers.Admin)]
    public partial class Data : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Grid1.SortField = "Group";
                BindGrid();
            }
        }

        // 查找
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        //-----------------------------------------------
        // Grid
        //-----------------------------------------------
        // 绑定网格
        private void BindGrid()
        {
            var types = AppContext.EntityTypes;
            var name = UI.GetText(this.tbName);
            if (name.IsNotEmpty())
                types = types.Search(t => t.Name.Contains(name, true));

            var sort = Grid1.SortField;
            var sortDirection = Grid1.SortDirection;
            Grid1.DataSource = types.AsQueryable().Sort(sort, sortDirection);
            Grid1.DataBind();
        }

        /// <summary>排序</summary>
        protected void Grid1_Sort(object sender, GridSortEventArgs e)
        {
            BindGrid();
        }

        // 数据窗口链接
        protected void Grid1_PreRowDataBound(object sender, GridPreRowEventArgs e)
        {
            var data = e.DataItem as App.DAL.EntityType;
            var auth = new AuthAttribute(Powers.Admin, Powers.Admin, Powers.Admin, Powers.Admin);

            // 数据列
            var dataField = Grid1.FindColumn("Data") as FineUIPro.WindowField;
            if (dataField != null)
            {
                var url = Urls.GetDatasUrl(data.Type, "", null, auth);
                dataField.DataIFrameUrlFormatString = url;
            }

            // 数据模型列
            var dataModelField = Grid1.FindColumn("Model") as FineUIPro.WindowField;
            if (dataModelField != null)
            {
                var url = Urls.GetDataModelUrl(data.Type, auth);
                dataModelField.DataIFrameUrlFormatString = url;
            }

            // UI设置列
            var uiField = Grid1.FindColumn("UI") as FineUIPro.WindowField;
            if (uiField != null)
            {
                var url = Urls.GetUISettingsUrl(data.Type);
                uiField.DataIFrameUrlFormatString = url;
            }
        }

    }
}
