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
    [UI("反馈管理")]
    [Auth(Powers.FeedBackView, Powers.FeedBackNew, Powers.FeedBackEdit, Powers.FeedBackDelete)]
    public partial class Feedbacks : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Grid1
                .SetPowers(this.Auth)
                .SetUrls("FeedbackForm.aspx")
                .AddColumn<Feedback>(t => t.Title, 400, "标题")
                .AddWindowColumn<Feedback>(t => t.UniID, t => t.StatusName, "/Pages/Workflows/Histories.aspx?md=view&key={0}&type=FeedBack", 100, "状态")
                .AddColumn<Feedback>(t => t.TypeName, 100, "类型")
                .AddColumn<Feedback>(t => t.User, 100, "提交者")
                .AddColumn<Feedback>(t => t.AppName, 100, "应用")
                .AddColumn<Feedback>(t => t.AppVersion, 100, "应用版本")
                .AddColumn<Feedback>(t => t.AppModule, 100, "应用模块")
                .AddColumn<Feedback>(t => t.Contacts, 100, "联系方式")
                .AddColumn<Feedback>(t => t.CreateDt, 140, "创建时间", "{0:yyyy-MM-dd HH:mm}")
                .AddColumn<Feedback>(t => t.UpdateDt, 140, "修改时间", "{0:yyyy-MM-dd HH:mm}")
                .InitGrid<Feedback>(this.BindGrid, Panel1, t => t.Title)
                .RowDataBound += Feedbacks_RowDataBound;
                ;
            if (!IsPostBack)
            {
                UI.BindEnum(ddlType,   typeof(FeedType), "--全部类别--");
                UI.BindEnum(ddlStatus, typeof(FeedbackStatus), "--全部状态--");
                UI.BindEnum(ddlApp,    typeof(FeedApp), "--全部应用--");
                this.Grid1.SetSortPage<Feedback>(SiteConfig.Instance.PageSize, t => t.CreateDt, false);
                this.btnFlow.OnClientClick +=  this.Grid1.Win.GetShowReference(Urls.GetWorkflowsUrl(WFType.Feedback));
                BindGrid();
            }
        }

        private void Feedbacks_RowDataBound(object sender, GridRowEventArgs e)
        {
            var data = e.DataItem as Feedback;
            var column = this.Grid1.FindColumn("StatusName") as FineUIPro.BoundField;
            if (column != null)
            {
                int n = column.ColumnIndex;
                var status = data.Status;
                if (status == FeedbackStatus.Finish)
                    e.Values[n] = string.Format("<div style=\"color:green\">{0}</div>", data.StatusName);
                else if (status == FeedbackStatus.Suspend)
                    e.Values[n] = string.Format("<div style=\"color:lightblue\">{0}</div>", data.StatusName);
                else if (status == FeedbackStatus.Cancel)
                    e.Values[n] = string.Format("<div style=\"color:pink\">{0}</div>", data.StatusName);
            }
        }


        // 绑定网格
        private void BindGrid()
        {
            var type = UI.GetEnum<FeedType>(ddlType);
            var status = UI.GetEnum<FeedbackStatus>(ddlStatus);
            var app = UI.GetEnum<FeedApp>(ddlApp);
            var appVersion = UI.GetText(tbAppVersion);
            var keyword = UI.GetText(tbKeyword);

            IQueryable<Feedback> q = Feedback.Search(
                keyword: keyword,
                type: type,
                status : status,
                app : app,
                appVersion : appVersion
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
