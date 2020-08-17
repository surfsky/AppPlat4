using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Kingsoc.Data.Metadata.SqlServer;
using System.Resources;
using System.Configuration;
using System.Text.RegularExpressions;
using App.Utils;
using App.DAL;
using App.Components;
using App.Controls;
using System.Data.Entity;
using System.Text;
using System.Reflection;

namespace App.Pages
{
    [UI("数据模型")]
    [Auth(Powers.Admin)]
    [Param("tp", "实体类类型")]
    public class DataModel : HandlerBase
    {
        // 入口
        public override void Process(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            Write(WebHelper.BuildBootstrapCss());
            var typeName = Asp.GetQueryString("tp");
            if (typeName.IsEmpty())
            {
                // 默认输出 AppContext 成员
                WriteTitle();
                foreach (var entityType in AppContext.EntityTypes)
                    WriteType(entityType.Type);
            }
            else
            {
                // 若指定类型名，则输出该类的成员
                Write("<input type='button' class='btn btn-primary btn-sm'  value='Back' onclick='javascript: if(history.length>0) history.back();'/></br></br>");
                var type = Reflector.GetType(typeName, "", false);
                if (type != null)
                    WriteType(type);
            }
        }

        /// <summary>输出标题</summary>
        private void WriteTitle()
        {
            Write("<h1>{0}</h1>", "DataModel");
            Write("<p>{0:yyyy-MM-dd HH:mm:ss}</p>", System.DateTime.Now);
        }

        /// <summary>输出实体类型的成员</summary>
        private void WriteType(Type type)
        {
            // 类型及继承关系
            Write("<h1>");
            Write("<a href='{0}' style='text-decoration:none'>{1}</a>", Urls.GetDataModelUrl(type), type.GetTypeString().HtmlEncode());
            var hasColon = false;  // 是否已经有冒号了
            if (type.BaseType != null)
            {
                var t = type.BaseType;
                var itemType = t;
                var text = t.GetTypeString();
                if (t.IsGenericType)
                    itemType = t.GetGenericTypeDefinition();
                else if (t.IsNullable())
                    itemType = t.GetNullableDataType();

                //var itemType = t.IsList() ? t.GetGenericDataType() : t.GetNullableDataType();
                Write(" : <a href='{0}' style='text-decoration:none'>{1}</a>", Urls.GetDataModelUrl(itemType), text.HtmlEncode());
                hasColon = true;
            }
            foreach (var ifc in type.GetInterfaces())
            {
                if (!hasColon)
                {
                    Write(" : ");
                    hasColon = true;
                }
                else
                    Write(", ");
                Write("<a href='{0}' style='text-decoration:none'>{1}</a>", Urls.GetDataModelUrl(ifc), ifc.GetTypeString().HtmlEncode());
            }
            Write("</h1>");
            if (type.Name != type.GetTitle())
                Write($"<h5>{type.GetTitle()}</h5>");
            Write("<br/>");

            // 输出枚举成员
            if (type.IsEnum())
            {
                Write("<h2>成员</h2>");
                Write("<table class='table table-sm table-hover table-bordered'>");
                Write("<thead><tr><td>ID</td><td>Value</td><td>Title</td><td>Group</td></tr></thead>");
                foreach (var i in EnumHelper.GetEnumInfos(type))
                    Write($"<tr><td>{i.ID}</td><td>{i.Value}</td><td>{i.Title}</td><td>{i.Group}</td></tr>");
                Write("</table>");
                return;
            }

            // 属性
            Write("<h2>属性</h2>");
            Write("<table class='table table-sm table-hover table-bordered'>");
            Write("<thead><tr><td>名称</td><td>类型</td><td>说明</td><td>值</td></tr></thead>");
            foreach (var p in type.GetProperties().AsQueryable().Sort(t => t.Name))
            {
                // 跳过非直属属性
                var t = p.PropertyType;
                if (p.DeclaringType != type)
                    continue;

                //
                var itemType = t.IsList() ? t.GetGenericDataType() : t.GetNullableDataType();
                Write("<tr><td>{0}</td><td><a href='{1}'>{2}</a></td><td>{3}</td><td>{4}</td></tr>",
                    p.Name,
                    Urls.GetDataModelUrl(itemType),
                    t.GetTypeString().HtmlEncode(),
                    p.GetTitle(),
                    Reflector.GetEnumString(t)
                    );
            }
            Write("</table>");

            // 方法
            Write("<h2>方法</h2>");
            Write("<table class='table table-sm table-hover table-bordered'>");
            Write("<thead><tr><td>名称</td></tr></thead>");
            foreach (var m in type.GetMethods().AsQueryable().OrderBy(t=> t.IsStatic).ThenBy(t => t.Name))
            {
                if (m.DeclaringType != type)
                    continue;
                if (m.IsSpecialName)
                    continue;

                var methodString = m.GetMethodString();
                if (m.IsStatic && !methodString.Contains("static"))
                    methodString = "static " + methodString;

                Write("<tr><td>{0}</td></tr>", methodString.HtmlEncode());
            }
            Write("</table>");
        }
    }
}