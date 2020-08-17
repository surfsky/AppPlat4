using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Text.RegularExpressions;
using App.Controls;
using App.DAL;
using App.Core;
using App.Components;
using System.IO;
using System.Text;

namespace App
{
    /// <summary>
    /// 工具
    /// </summary>
    public class WebTool
    {
        /// <summary>Js格式化</summary>
        public static string BeautifyJs(string source)
        {
            if (source == null) return "";
            var jsb = new JSBeautifyLib.JSBeautify(source, new JSBeautifyLib.JSBeautifyOptions());
            return jsb.GetResult();
        }

        //
        //const string JqueryJs     = "/Res/jquery-3.3.1.min.js";
        //const string BootStrapCss = "/Res/bootstrap/4.0.0/bootstrap.css";
        //const string BootStrapJs  = "/Res/bootstrap/4.0.0/bootstrap.js";
        const string JqueryJs     = "https://cdn.staticfile.org/jquery/3.2.1/jquery.min.js";
        const string BootStrapCss = "https://cdn.staticfile.org/twitter-bootstrap/4.1.0/css/bootstrap.min.css";
        const string BootStrapJs  = "https://cdn.staticfile.org/twitter-bootstrap/4.1.0/js/bootstrap.min.js";
        const string PopperJs = "https://cdn.staticfile.org/popper.js/1.12.5/umd/popper.min.js";

        /// <summary>构建 Bootstrap css 样式</summary>
        /// <param name="full">是否包含附属脚本和meta</param>
        public static string BuildBootstrapCss(bool full=true)
        {
            var css = $@"
                <link rel=""stylesheet"" href=""{BootStrapCss}"">
                <style>
                    body {{padding: 20px;}}
                    h1 {{font-size:1.8rem;}}
                    h2 {{font-size:1.6rem;}}
                    h3 {{font-size:1.4rem;}}
                    form {{width: 100%;}}
                    thead {{background-color: ghostwhite;}}
                </style>
                ";
            if (!full)
                return css;
            return $@"
                <head>
                <meta charset=""utf-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
                {css}
                <script src=""{JqueryJs}""></script>
                <script src=""{PopperJs}""></script>
                <script src=""{BootStrapJs}""></script>
                <head>
                ";
        }

        /// <summary>构建 Bootstrap 表格</summary>
        public static string BuildBootstrapTable<T>(List<T> data)
        {
            var type = typeof(T) == typeof(Object) ? data.GetItemType() : typeof(T);
            var ui = new UISetting(type);
            var sb = new StringBuilder();
            sb.AppendFormat("<table class='table table-sm table-hover'>");

            // 标题
            sb.AppendFormat("<thead><tr>");
            foreach (var item in ui.Items)
                if (item.Type.IsBasicType())
                    sb.AppendFormat("<td>{0}</td>",  item.Title);
            sb.AppendFormat("</tr></thead>");

            // 数据
            foreach (var d in data)
            {
                sb.AppendFormat("<tr>");
                foreach (var item in ui.Items)
                {
                    if (item.Type.IsBasicType())
                        sb.AppendFormat("<td>{0}</td>", d.GetValue(item.Field.Name));
                }
                sb.AppendFormat("</tr>");
            }
            sb.AppendFormat("</tr></table>");
            return sb.ToString();
        }

    }
}
