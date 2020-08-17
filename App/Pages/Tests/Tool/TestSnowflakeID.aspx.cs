using System;
using System.Web.UI;
using System.Collections.Generic;
using FineUIPro;
using App.Controls;
using App.Utils;
using System.Text;
using App.DAL;
using App.Components;

namespace App.Tests
{
    /// <summary>
    /// </summary>
    [Auth(Powers.Admin)]
    public partial class TestSnowflakeID : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var snow = new SnowflakeID(1, 2010);

            // 生成ID列表
            var ids = new List<long>();
            for (int i=0; i<10000; i++)
            {
                var id = snow.NewID();
                ids.Add(id);
            }

            // 格式化输出
            var sb = new StringBuilder();
            var n = 0;
            foreach (var id in ids)
            {
                string line = string.Format("{0:0000} {1}         {2}", n++, id, id.ToBitString());
                sb.AppendLine(line);
            }
            this.tbIDS.Text = sb.ToString();
        }
    }
}
