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

namespace App.Admins
{
    /*
    <f:ImageField DataImageUrlField="Level" ColumnID="Icon" SortField="Level" Width="30px" DataToolTipField="Level" />
    <f:BoundField DataField="LevelName" SortField="LevelName" Width="80px" HeaderText="级别"  />
    <f:BoundField DataField="LogDt" SortField="LogDt" DataFormatString="{0:yyyy-MM-dd HH:mm}" Width="150px" HeaderText="时间" />
    <f:BoundField DataField="Operator" SortField="Logger" Width="100px" HeaderText="操作人" />
    <f:BoundField DataField="From"  SortField="From" HeaderText="数据来源" Width="160px" />
    <f:BoundField DataField="IP"    SortField="IP" HeaderText="客户端IP" Width="140px" />
    <f:BoundField DataField="URL"    SortField="URL" HeaderText="请求地址" Width="220px" />
    <f:BoundField DataField="Method"  SortField="Method" HeaderText="请求方式" Width="60px" />
    <f:BoundField DataField="Summary"  HeaderText="信息" Width="300px" ExpandUnusedSpace="true" />
    */
    [UI("日志管理。只有批量删除功能，不允许手工添加")]
    [Auth(Powers.Log)]
    public partial class Logs : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Grid1.SetPowers(true, false, false, false)
                .SetUrls("LogForm.aspx")
                .AddImageColumn<Log>(t => t.Level, 30)
                .AddEnumColumn<Log>(t => t.Level, 80, "级别")
                .AddColumn<Log>(t => t.LogDt, 150, "时间", "{0:yyyy-MM-dd HH:mm}")
                .AddColumn<Log>(t => t.Operator, 100, "操作人")
                .AddColumn<Log>(t => t.From, 160, "数据来源")
                .AddColumn<Log>(t => t.IP, 140, "IP")
                .AddColumn<Log>(t => t.URL, 140, "URL")
                .AddColumn<Log>(t => t.Method, 140, "请求方式")
                .AddColumn<Log>(t => t.Summary, 140, "概述")
                .InitGrid<Log>(BindGrid, Panel1, t=>t.Message)
                ;
            if (!IsPostBack)
            {
                UI.BindEnum(this.ddlSearchLevel, typeof(LogLevel), "--日志级别--");
                UI.SetButtonByPower(btnDelete, Powers.Log);
                UI.SetVisibleByQuery("search", this.btnSearch, this.tbOperator, this.tbMessage, this.ddlSearchLevel, this.ddlSearchRange);
                this.Grid1.SetSortPage<Log>(SiteConfig.Instance.PageSize, t => t.LogDt, false);
                BindGrid();
            }
        }

        //-------------------------------------------------
        // Grid
        //-------------------------------------------------
        private void BindGrid()
        {
            var from = UI.GetText(tbFrom);
            var user = UI.GetText(tbOperator);
            var msg = UI.GetText(tbMessage);
            var level = UI.GetEnum<LogLevel>(this.ddlSearchLevel);
            var ip = UI.GetText(tbIP);
            DateTime? fromDt = null;
            if (ddlSearchRange.SelectedItemArray.Length > 0)
            {
                var today = DateTime.Today;
                switch (ddlSearchRange.SelectedValue)
                {
                    case "TODAY":      fromDt = today; break;
                    case "LASTWEEK":   fromDt = today.AddDays(-7); break;
                    case "LASTMONTH":  fromDt = today.AddMonths(-1); break;
                    case "LASTYEAR":   fromDt = today.AddYears(-1); break;
                    default: break;
                }
            }

            IQueryable<Log> q = Log.Search(user, msg, level, fromDt, ip, from);
                /*
                .Select(t=>
                    new 
                    {
                        t.ID,
                        t.Level,
                        t.LevelName,
                        t.LogDt
                    });
                */
            Grid1.Bind(q);
        }


        // 行预绑定事件（显示图标列）
        protected void Grid1_PreRowDataBound(object sender, GridPreRowEventArgs e)
        {
            var log = e.DataItem as Log;
            var fld = Grid1.FindColumn("Image-Level") as FineUIPro.ImageField;
            if (fld != null)
            {
                fld.DataImageUrlFormatString = GetIconUrl(log.Level);
            }
        }

        // 获取日志级别对应的图标
        string GetIconUrl(LogLevel? logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug: return string.Format("~/Res/Icon/{0}.png", FineUIPro.Icon.Bug);
                case LogLevel.Info:  return string.Format("~/Res/Icon/{0}.png", FineUIPro.Icon.Comment);
                case LogLevel.Warn:  return string.Format("~/Res/Icon/{0}.png", FineUIPro.Icon.Information);
                case LogLevel.Error: return string.Format("~/Res/Icon/{0}.png", FineUIPro.Icon.Exclamation);
                case LogLevel.Fatal: return string.Format("~/Res/Icon/{0}.png", FineUIPro.Icon.Decline);
                default: return string.Format("~/Res/Icon/{0}.png", FineUIPro.Icon.Information);
            }
        }

        //-------------------------------------------------
        // 工具栏
        //-------------------------------------------------
        // 查找
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        // 批量删除
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            int n = Log.DeleteBatch();
            BindGrid();
            Alert.ShowInTop(string.Format("成功删除{0}条记录", n));
        }

        // 显示日志配置窗口
        protected void btnLogConfig_Click(object sender, EventArgs e)
        {
            var auth = new AuthAttribute(Powers.Log, Powers.Log, Powers.Log, Powers.Log);
            var url = Urls.GetDatasUrl(typeof(LogConfig), "", PageMode.Edit, auth);
            this.Grid1.ShowWindow(url, "日志配置", 800, 500);
        }


    }
}
