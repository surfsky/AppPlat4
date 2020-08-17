using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
//using EntityFramework.Extensions;
using App.DAL;
using System.Collections;
using App.Components;
using App.Controls;
using App.Utils;
using System.IO;

namespace App.Pages
{
    [UI("关于")]
    [Auth(AuthLogin=true)]
    public partial class About : PageBase
    {
        //------------------------------------------
        // init
        //------------------------------------------
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var path = Server.MapPath("/readme.md");
                if (File.Exists(path))
                {
                    var text = File.ReadAllText(path);
                    this.lblMarkdown.Text = new MarkdownSharp.Markdown().Transform(text);
                }
            }
        }


    }
}
