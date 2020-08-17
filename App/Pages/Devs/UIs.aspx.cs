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
using App.Entities;

namespace App.Pages
{
    [UI("UI配置管理")]
    [Auth(Powers.Admin)]
    [Param("tp", "实体类型")]
    public partial class UIs : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            var type = Asp.GetQueryString("tp");
            var url = string.Format("UIForm.aspx?tp={0}", type);
            this.Grid1
                .SetPowers(this.Auth)
                .SetUrls(url)
                .AddEnumColumn<XUI>(t => t.Type, 200, "类型")
                .AddColumn<XUI>(t => t.Name, 200, "名称")
                .AddColumn<XUI>(t => t.EntityTypeName, 200, "实体类型")
                .AddColumn<XUI>(t => t.Error, 200, "错误")
                .InitGrid<XUI>(this.BindGrid, Panel1, t => t.EntityTypeName)
                ;
            if (!IsPostBack)
            {
                //
                UI.Bind(ddlEntityType, AppContext.EntityTypes, t => t.FullName, t => t.FullName);
                if (type.IsNotEmpty())
                {
                    UI.SetValue(ddlEntityType, type);
                    UI.SetEnable(false, ddlEntityType);
                }

                this.Grid1.SetSortPage<XUI>(SiteConfig.Instance.PageSize, t => t.EntityTypeName, true);
                BindGrid();
            }
        }

        // 绑定网格
        private void BindGrid()
        {
            var name = UI.GetText(ddlEntityType);
            IQueryable<XUI> q = XUI.Search(null, name);
            Grid1.Bind(q);
        }

        // 查找
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}
