using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Controls.ECharts
{

    //---------------------------------------
    // 有XY轴的图表
    //---------------------------------------
    /// <summary>有XY轴的图表</summary>
    /// <example>
    /// LineChart chart = new LineChart();
    /// chart.title = new Title("未来一周气温变化", "纯属虚构");
    /// chart.legend.data = new List<string> { "最高温度", "最低温度" };
    /// 
    /// // 轴定义
    /// chart.xAxis.name = "星期";
    /// chart.yAxis.name = "温度";
    /// chart.xAxis.data = new List<string> { "周一", "周二", "周三", "周四", "周五", "周六", "周日" };
    /// 
    /// // Y 轴的数据及展示方式
    /// var s1 = new LineSeries("最高温度", new List<string> { "11", "11", "15", "13", "12", "13", "10" });
    /// var s2 = new LineSeries("最低温度", new List<string> { "1", "-3", "3", "-2", "2", "2", "1" });
    /// chart.series.Add(s1);
    /// chart.series.Add(s2);
    /// chart.visualMap.Add(new Visual() { max = 20, seriesIndex = 0 });
    /// chart.visualMap.Add(new Visual() { max = 10, seriesIndex = 1 });
    /// </example>
    public class GridChart : EChart
    {
        public Axis xAxis = new Axis() { type = "category" };
        public Axis yAxis = new Axis() { type = "value" };
        public List<Visual> visualMap = new List<Visual>();

        public GridChart()
        {
            tooltip.trigger = "axis";
        }
    }

    /// <summary>渐变视觉样式</summary>
    public class Visual
    {
        public bool show = false;
        public string type = "continuous";
        public int seriesIndex = 0;
        public int min = 0;
        public int max = 400;
    }

    /// <summary>轴</summary>
    public class Axis
    {
        public string name;
        public string type;
        public bool boundaryGap = false;
        public List<string> data = new List<string>();
        public AxisLabel axisLabel = new AxisLabel();
    }
    public class AxisLabel
    {
        public int? rotate;
    }

    /// <summary>折线系列</summary>
    public class LineSeries : Series
    {
        // data
        public List<string> data { get; set; } = new List<string>();


        // axisIndex
        public int? yAxisIndex { get; set; }
        public int? xAxisIndex { get; set; }

        //
        public LineSeries() { type = "line"; }
        public LineSeries(string name)
        {
            this.type = "line";
            this.name = name;
        }
        public LineSeries(string name, List<string> data)
        {
            this.type = "line";
            this.name = name;
            this.data = data;
        }
    }

    /// <summary>
    /// 柱状系列
    /// </summary>
    public class BarSeries : LineSeries
    {
        public BarSeries(LineSeries s)
        {
            this.name = s.name;
            this.type = "bar";
            this.data = s.data;
            this.xAxisIndex = s.xAxisIndex;
            this.yAxisIndex = s.yAxisIndex;
            /*
            this.dataField = s.dataField;
            this.showAllSymbol = s.showAllSymbol;
            this.showSymbol = s.showSymbol;
            this.symbol = s.symbol;
            this.symbolSize = s.symbolSize;
            */
        }
        public BarSeries() { type = "bar"; }
        public BarSeries(string name, List<string> data)
        {
            this.type = "bar";
            this.name = name;
            this.data = data;
        }
    }




}