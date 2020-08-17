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

namespace App.Pages
{
    [UI("工作流管理")]
    [Auth(Powers.Admin)]
    public partial class Workflows : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Grid1
                .SetControlColumns(false, true, true, true, false)
                .SetWindow(800, 300)
                .SetPowers(true, false, false, false)
                .SetUrls("WorkflowForm.aspx")
                .AddColumn<WFStep>(t => t.Status, 80, "状态ID")
                .AddColumn<WFStep>(t => t.StatusName, 100, "状态")
                .AddColumn<WFStep>(t => t.RoutesText, 300, "路由")
                .AddEnumColumn<WFStep>(t => t.Power, 100, "所需权限")
                .AddColumn<WFStep>(t => t.TypeName, 100, "节点类型")
                //.AddColumn<WFStep>(t => t.Action, 200, "动作")
                .InitGrid<WFStep>(this.BindGrid, Panel1, t => t.StatusName)
                ;
            if (!IsPostBack)
            {
                // 流程类别
                UI.BindEnum(this.ddlType, typeof(WFType), "--流程类型--");
                var type = Asp.GetQuery<WFType>("type");
                type = type ?? WFType.Feedback;
                UI.SetValue(this.ddlType, type);
                UI.SetVisibleByQuery("search", this.btnSearch, this.ddlType);

                this.Grid1.SetSortPage<WFStep>(SiteConfig.Instance.PageSize, t => t.Status, true);
                BindGrid();
            }
        }

        // 绑定网格
        private void BindGrid()
        {
            var type = UI.GetEnum<WFType>(this.ddlType);
            IQueryable<WFStep> q = Workflow.GetFlow(type).Steps.AsQueryable();
            Grid1.Bind(q);
        }

        // 查找
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}
