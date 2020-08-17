using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Controls.ECharts
{

    //---------------------------------------
    // 饼图
    //---------------------------------------
    /// <summary>饼图</summary>
    /// <example>
    /// var chart = new PieChart();
    /// chart.title = new Title("同名数量统计", "纯属虚构");
    /// chart.legend.data = new List<string> { "赵", "钱", "孙", "李", "周" };
    /// PieSeries serie = new PieSeries(
    ///     "姓名",
    ///     new List<PieSerieData>()
    ///     {
    ///         new PieSerieData("赵", 9),
    ///         new PieSerieData("钱", 80),
    ///         new PieSerieData("孙", 43),
    ///         new PieSerieData("李", 23),
    ///         new PieSerieData("周", 21)
    ///     }
    /// );
    /// chart.series.Add(serie);
    /// </example>
    public class PieChart : EChart
    {
        public PieChart()
        {
            this.tooltip.trigger = "item";
            this.tooltip.formatter = "{b} : {c} ({d}%)";
        }
    }


    /// <summary>饼图系列</summary>
    public class PieSeries : Series
    {
        public List<PieSeriesData> data { get; set; } = new List<PieSeriesData>();
        public string radius = "55%";
        public string[] center = { "40%", "50%" };
        public object itemStyle = new
        {
            emphasis = new
            {
                shadowBlur = 10,
                shadowOffsetX = 0,
                shadowColor = "rgba(0, 0, 0, 0.5)"
            }
        };

        public PieSeries() { type = "pie"; }
        public PieSeries(string name, List<PieSeriesData> data)
        {
            type = "pie";
            this.name = name;
            this.data = data;
        }
    }

    /// <summary>饼图系列的数据</summary>
    public class PieSeriesData
    {
        public string name { get; set; }
        public string value { get; set; }
        public PieSeriesData(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
    }

}