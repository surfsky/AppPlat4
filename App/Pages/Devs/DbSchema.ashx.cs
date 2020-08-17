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

namespace App.Pages
{
    [UI("数据字典（数据库结构）")]
    [Auth(Powers.Admin)]
    public class DbSchema : HandlerBase
    {
        public override void Process(HttpContext context)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["db"].ConnectionString;
            string database = RegexHelper.Search(connectionString, @"Initial Catalog=(?<Cata>\w*)", "Cata");
            string owner = "dbo";
            string title = "数据字典";

            context.Response.ContentType = "text/html";
            Write(WebHelper.BuildBootstrapCss());
            Write("<h1>{0}</h1>", title);
            Write("<p>{0:yyyy-MM-dd HH:mm:ss}</p>", System.DateTime.Now);
            SqlServerFetcher f = new SqlServerFetcher(connectionString);
            foreach (var t in f.GetTables())
            {
                var fullName = string.Format("{0}.{1}", t.Schema, t.Name);
                Write("<h1>{0}</h1>", fullName);
                Write("<table class='table table-sm table-hover table-bordered'>");
                Write("<thead>");
                Write("<tr><td width='200'>名称</td><td width='150'>类型</td><td width='150'>可为空</td><td width='150'>字符长度</td><td width='150'>数字精度</td><td>主键</td><td>备注</td></tr>");
                Write("</thead>");
                foreach (var c in f.GetColumns2008(database, owner, t.Name))
                {
                    Write("<tr><td>{0}&nbsp;</td><td>{1}&nbsp;</td><td>{2}&nbsp;</td><td>{3}&nbsp;</td><td>{4}&nbsp;</td><td>{5}&nbsp;</td><td>{6}&nbsp;</td></tr>",
                        c.Name,
                        c.DataType,
                        c.IsNullbable,
                        c.CharacterMaximumLength,
                        c.NumericPrecision,
                        c.Identity,
                        c.Description
                        );
                }
                Write("</table>");
            }
        }


    }
}