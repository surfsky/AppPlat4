using App.Utils;
using FineUIPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace App.Controls.ECharts
{
    //---------------------------------------
    // 公共
    //---------------------------------------
    /// <summary>EChart 图表基类</summary>
    public partial class EChart
    {
        public Title   title   { get; set; } = new Title();
        public Tooltip tooltip { get; set; } = new Tooltip();
        public Toolbox toolbox { get; set; } = new Toolbox();
        public Legend  legend  { get; set; } = new Legend();
        public List<Series> series { get; set; } = new List<Series>();

        /// <summary>生成图表的脚本到客户端</summary>
        public void Render(string clientId, bool useFineUI = true)
        {
            var option = this.ToJson();
            var script = string.Format("echarts.init(document.getElementById('{0}')).setOption({1});", clientId, option);
            if (useFineUI)
                PageContext.RegisterStartupScript(script);
            else
                (HttpContext.Current.Handler as Page).ClientScript.RegisterStartupScript(null, "EChart", script, true);
        }
    }

    /// <summary>标题</summary>
    public class Title
    {
        public string text { get; set; }
        public string subtext { get; set; }
        public string x { get; set; } = "center";
        public Title() { }
        public Title(string text, string subtext="")
        {
            this.text = text;
            this.subtext = subtext;
        }
    }

    /// <summary>提示</summary>
    public class Tooltip
    {
        public string trigger { get; set; }  // = "axis";  // item
        public string formatter { get; set; }
        public object axisPointer = new
        {
            type = "cross",
            crossStyle = new { color = "#999" }
        };
    }

    /// <summary>工具栏</summary>
    public class Toolbox
    {
        public bool show { get; set; } = true;
        public object feature = new
            {
                dataView    = new {show = true, readOnly = false},
                restore     = new {show= true},
                saveAsImage = new {show= true}
            };
    }



    /// <summary>图例</summary>
    public class Legend
    {
        public string orient { get; set; } //= "vertical";
        public string type { get; set; }   //= "scroll";
        public string x { get; set; }      //= "right";
        public int? left { get; set; }
        public int? right { get; set; }
        public int? top { get; set; } = 50;
        public int? bottom { get; set; }
        public List<string> data { get; set; } = new List<string>();
    }

    /// <summary>系列基类</summary>
    public class Series
    {
        /// <summary>名称</summary>
        public string name { get; set; }

        /// <summary>类型：bar|line</summary>
        public string type { get; set; }

        /// <summary>属性名</summary>
        [JsonIgnore]
        public string dataField { get; set; }

        // Symbol
        /// <summary>标记类型</summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public Symbol symbol { get; set; } = Symbol.circle;
        public bool showSymbol { get; set; } = true;
        public bool showAllSymbol { get; set; } = false;
        public int symbolSize { get; set; } = 2;
    }

    /// <summary>系列标记点展示方式</summary>
    public enum Symbol
    {
        emptyCircle,
        circle,
        emptyRect,
        rect,
        diamond,
        triangle,

        // 以下三种易混淆或不好看，不用
        //roundRect,
        //pin,
        //arrow
    }


}