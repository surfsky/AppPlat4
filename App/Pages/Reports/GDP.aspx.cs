using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using System.Linq;
using System.Data.Entity;
//using EntityFramework.Extensions;
using App.DAL;
using App.Controls;
using App.Utils;
using App.Controls.ECharts;
using App.Components;


namespace App.Reports
{
    [UI("示例报表（GDP数据）")]
    [Auth(Powers.ReportView, Powers.ReportNew, Powers.ReportEdit, Powers.ReportDelete)]
    public partial class GDP : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Grid1
                .SetPowers(this.Auth)
                .SetUrls(typeof(RptGDP))
                .AddColumns(typeof(RptGDP))
                .InitGrid<RptGDP>(BindGrid, Panel1, t=>t.Quarter)
                ;

            if (!IsPostBack)
            {
                this.Grid1.SetSortPage<RptGDP>(SiteConfig.Instance.PageSize, t => t.Quarter, true);
                BindGrid();
            }
        }


        // 绑定数据
        void BindGrid()
        {
            var data = GetQuery();
            Grid1.Bind(data);
        }
        IQueryable<RptGDP> GetQuery()
        {
            return RptGDP.SearchByQuarter(this.txtFrom.Text, this.txtTo.Text);
        }

        // 搜索
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SwitchView(true, false);
            BindGrid();
        }

        //-------------------------------------------------------
        // 导出
        //-------------------------------------------------------
        // 导出excel
        protected void Grid1_Export(object sender, EventArgs e)
        {
            this.Grid1.ExportExcel(GetQuery().ToList());
        }


        //-------------------------------------------------------
        // 图表
        //-------------------------------------------------------
        // 图表1
        protected void btnChart1_Click(object sender, EventArgs e)
        {
            SwitchView(false, true);
            var data = GetQuery().ToList();
            List<LineSeries> series = new List<LineSeries>();
            series.Add(new LineSeries { name = "GDP增速", dataField = "GDPInc",             symbol = Symbol.emptyCircle });
            series.Add(new LineSeries { name = "一产增速", dataField = "FirstIndustryInc",  symbol = Symbol.circle });
            series.Add(new LineSeries { name = "二产增速", dataField = "SecondIndustryInc", symbol = Symbol.emptyRect });
            series.Add(new LineSeries { name = "三产增速", dataField = "ThirdIndustryInc",   symbol = Symbol.rect });
            EChart.BuildLineChart(data, "GDP及三次产业增加值增长对比", "Quarter", series).Render(Chart1.ClientID);
        }

        // 图表2
        protected void btnChart2_Click(object sender, EventArgs e)
        {
            SwitchView(false, true);
            var data = GetQuery().ToList();
            List<LineSeries> series = new List<LineSeries>();
            series.Add(new LineSeries { name = "温州增速", dataField = "GDPInc",      symbol = Symbol.emptyCircle });
            series.Add(new LineSeries { name = "全国增速", dataField = "CountryInc",  symbol = Symbol.circle });
            series.Add(new LineSeries { name = "浙江增速", dataField = "ZheJiangInc", symbol = Symbol.emptyRect });
            EChart.BuildLineChart(data, "温州与全国、全省GDP逐季增长对比", "Quarter", series).Render(Chart1.ClientID);
        }


        // 切换视图
        private void SwitchView(bool showGrid, bool showChart)
        {
            this.Grid1.Hidden = !showGrid;
            this.Chart1.Hidden = !showChart;
        }

    }
}
