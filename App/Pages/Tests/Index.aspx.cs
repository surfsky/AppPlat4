using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using App.Components;
using App.Controls;
using App.DAL;
using System.Data.Entity;
using App;
using App.Utils;
using FineUIPro;
using System.IO;

namespace App.Pages
{
    /// <summary>
    /// 测试索引页面（也可以用 Explorer.aspx 替代）
    /// </summary>
    [Auth(Powers.Admin)]
    public partial class Index : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        // 绑定网格
        private void BindGrid()
        {
            var filter = new string[] { ".aspx", ".htm", ".html" };
            var path = Path.GetDirectoryName(HttpContext.Current.Request.RawUrl);
            var physicalPath = MapPath(path);
            var files = new DirectoryInfo(physicalPath).GetFiles("*.*", SearchOption.AllDirectories)
                .Search(t => filter.Contains(t.Extension.ToLower()))
                .AsQueryable()
                .Select( t => new { Name = t.FullName.TrimStart(physicalPath, false).ToWebPath()})
                .OrderBy(t => t.Name)
                .ToList()
                ;
            Grid1.DataSource = files;
            Grid1.DataBind();
        }


    }
}