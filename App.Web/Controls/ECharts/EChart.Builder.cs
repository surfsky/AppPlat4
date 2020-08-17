using FineUIPro;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using App.Utils;
using App.Entities;

namespace App.Controls.ECharts
{
    /// <summary>
    /// EChart 渲染
    /// </summary>
    public partial class EChart
    {
        /// <summary>创建折线图</summary>
        public static EChart BuildLineChart(List<StatItem> data, string title, string stepName, string valueName)
        {
            // 排序、找到刻度列表、系列名称列表
            data = data.OrderBy(t => t.Step).OrderBy(t => t.Name).ToList();
            List<string> steps = data.Select(t => t.Step).Distinct().ToList();
            List<string> seriesNames = data.Select(t => t.Name).Distinct().ToList();

            // 遍历数据构建系列数组
            List<LineSeries> lineSeriesList = new List<LineSeries>();
            foreach (var seriesName in seriesNames)
            {
                // 找到或创建系列
                var series = lineSeriesList.Find(t => t.name == seriesName);
                if (series == null)
                {
                    series = new LineSeries(seriesName);
                    lineSeriesList.Add(series);
                }

                // 填充系列的数据
                foreach (var step in steps)
                {
                    var value = data.FirstOrDefault(t => t.Name == seriesName && t.Step == step)?.Value;
                    series.data.Add(value.ToText());
                }
            }

            // 构建图表
            var chart = new GridChart();
            chart.title.text = title;
            chart.legend.data = lineSeriesList.Select(t => t.name).ToList();
            chart.xAxis.name = stepName;
            chart.xAxis.data = steps;
            chart.yAxis.name = valueName;

            // 系列
            var barSeriesList = lineSeriesList.Cast(t => new BarSeries(t));
            lineSeriesList.ForEach(t => chart.series.Add(t));
            //barSeriesList.ForEach(t => chart.series.Add(t));
            return chart;
        }

        /// <summary>构建饼图</summary>
        public static PieChart BuildPieChart(List<StatItem> data, string title)
        {
            var pieData = new List<PieSeriesData>();
            foreach (var item in data)
                pieData.Add(new PieSeriesData(item.Name, item.Value.ToText()));
            PieChart chart = new PieChart();
            chart.title = new Controls.ECharts.Title(title);
            chart.legend.data = data.Select(t => t.Name).Distinct().ToList();
            chart.series.Add(new PieSeries(title, pieData));
            return chart;
        }

        /// <summary>构建网格图表（数据已经弄成交叉报表, 如：step, value1, value2, value3, .....）</summary>
        public static EChart BuildLineChart(IList data, string title, string stepName, List<LineSeries> series)
        {
            // 遍历数据，填充x轴值和系列值
            List<string> steps = new List<string>();
            foreach (var item in data)
            {
                steps.Add(item.GetValue(stepName).ToText());
                for (int i = 0; i < series.Count; i++)
                    series[i].data.Add(item.GetValue(series[i].dataField).ToText());
            }

            // 构建图表
            var chart = new GridChart();
            chart.title.text = title;
            chart.legend.data = series.Select(t => t.name).ToList();
            chart.xAxis.data = steps;
            series.ForEach(t => chart.series.Add(t));
            return chart;
        }

    }
}