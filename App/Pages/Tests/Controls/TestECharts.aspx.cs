using App.Utils;
using App.HttpApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using App.Components;

namespace App.Controls.ECharts
{
    [UI("ECharts 测试")]
    [Auth(Ignore =true)]
    public partial class TestECharts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [HttpApi]
        /// <summary>获取温度数据（折线图）</summary>
        public object GetLineData()
        {
            GridChart chart = new GridChart();
            chart.title = new Title("未来一周气温变化", "纯属虚构");
            chart.legend.data = new List<string> { "最高温度", "最低温度" };

            // 轴定义
            chart.xAxis.name = "星期";
            chart.yAxis.name = "温度";
            chart.xAxis.data = new List<string> { "周一", "周二", "周三", "周四", "周五", "周六", "周日" };

            // Y 轴的数据及展示方式
            var s1 = new LineSeries("最高温度", new List<string> { "11", "11", "15", "13", "12", "13", "10" });
            var s2 = new LineSeries("最低温度", new List<string> { "1", "-3", "3", "-2", "2", "2", "1" });
            chart.series.Add(s1);
            chart.series.Add(s2);
            chart.visualMap.Add(new Visual() { max = 20, seriesIndex = 0 });
            chart.visualMap.Add(new Visual() { max = 10, seriesIndex = 1 });
            return chart;
        }

        [HttpApi]
        /// <summary>获取饼图数据</summary>
        public object GetPieData()
        {
            var chart = new PieChart();
            chart.title = new Title("同名数量统计", "纯属虚构");
            chart.legend.data = new List<string> { "赵", "钱", "孙", "李", "周" };
            PieSeries serie = new PieSeries(
                "姓名",
                new List<PieSeriesData>()
                {
                    new PieSeriesData("赵", "9"),
                    new PieSeriesData("钱", "80"),
                    new PieSeriesData("孙", "43"),
                    new PieSeriesData("李", "23"),
                    new PieSeriesData("周", "21")
                }
            );
            chart.series.Add(serie);
            return chart;
        }

    }
}