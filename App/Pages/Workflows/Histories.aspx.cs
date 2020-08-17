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
using App.Entities;

namespace App.Pages
{
    [UI("操作历史")]
    [Auth(AuthLogin =true)]
    [Param("key", "对应的主表UniID")]
    [Param("pv", "查看权限")]
    [Param("pn", "新建权限")]
    [Param("pe", "编辑权限")]
    [Param("pd", "删除权限")]
    public partial class Histories : PageBase
    {
        /*
        <f:WindowField WindowID="Window1" HeaderText="经手人" Width="100px" 
            DataTextField="UserName" 
            DataIFrameUrlFields="User.ID"  DataIFrameUrlFormatString="UserForm.aspx?md=view&id={0}" 
            />
        <f:BoundField DataField="CreateDt"    SortField="CreateDt" HeaderText="创建时间" Width="140px" />
        <f:BoundField DataField="Status"      SortField="Status" HeaderText="状态" Width="100px" />
        <f:BoundField DataField="UserMobile"  SortField="UserMobile" HeaderText="经手人手机" Width="100px" />
        <f:BoundField DataField="Remark"      SortField="Remark" HeaderText="备注" Width="140px" ExpandUnusedSpace="true" />
        */
        protected void Page_Load(object sender, EventArgs e)
        {
            var key = Asp.GetQueryString("key");
            if (key.IsEmpty())
            {
                Asp.Fail("缺少 key 参数");
                return;
            }

            // 权限由参数录入
            this.Auth.ViewPower   = Asp.GetQuery<Powers>("pv") ?? Powers.OrderView;
            this.Auth.NewPower    = Asp.GetQuery<Powers>("pn") ?? Powers.OrderNew;
            this.Auth.EditPower   = Asp.GetQuery<Powers>("pe") ?? Powers.OrderEdit;
            this.Auth.DeletePower = Asp.GetQuery<Powers>("pd") ?? Powers.OrderDelete;

            //
            var newUrl = string.Format("HistoryForm.aspx?md=new&type={0}&key={1}", Asp.GetQueryString("type"), Asp.GetQueryString("key"));
            Grid1.SetPowers(this.Auth)
                .SetUrls(
                    newUrl,
                    "HistoryView.aspx?id={0}",
                    "HistoryForm.aspx?md=edit&id={0}")
                .AddWindowColumn<History>(t => t.UserId, t => t.UserName, "UserForm.aspx?md=view&id={0}", 100, "经手人")
                .AddColumn<History>(t => t.CreateDt, 150, "创建时间", "{0:yyyy-MM-dd HH:mm}")
                .AddColumn<History>(t => t.Status, 100, "状态")
                .AddColumn<History>(t => t.UserMobile, 100, "经手人手机")
                .AddColumn<History>(t => t.Remark, 100, "备注")
                .InitGrid<History>(BindGrid, Panel1, t => t.Status)
                ;
            if (!IsPostBack)
            {
                this.Grid1.SetSortPage<History>(SiteConfig.Instance.PageSize, t => t.CreateDt, true);
                BindGrid();
                UI.SetVisibleByQuery("search", this.tbUser, this.dpCreate, this.btnSearch, tbMobile);
                UI.SetVisibleByQuery("flowType", this.btnFlow);
            }
        }


        //-------------------------------------------------
        // Grid
        //-------------------------------------------------
        private void BindGrid()
        {
            var key = Asp.GetQueryString("key");
            var user = UI.GetText(tbUser);
            var mobile = UI.GetText(tbMobile);
            var createDt = UI.GetDate(this.dpCreate);
            IQueryable<History> q = History.Search(
                key: key,
                userName: user,
                userMobile: mobile,
                startDt: createDt
                );
            Grid1.Bind(q);
        }

        //-------------------------------------------------
        // 工具栏
        //-------------------------------------------------
        // 查找
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        // 显示流程
        protected void btnFlow_Click(object sender, EventArgs e)
        {
            /*
            var key = Asp.GetQueryString("key");
            var order = GetOrder(key);
            var type = order.Type;
            */
            var type = Asp.GetQuery<WFType>("type");
            if (type != null)
            {
                var url = Urls.GetWorkflowsUrl(type.Value);
                UI.ShowWindow(this.Grid1.Win, url, "流程", 800, 700, CloseAction.Hide);
            }
        }



    }
}
