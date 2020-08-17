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
using App.Components;
using App.Controls;
using App.Utils;

namespace App.Admins
{
    [UI("序列号发生器管理")]
    [Auth(Powers.Sequence)]
    public partial class Sequences : PageBase
    {
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Grid1
                .SetPowers(this.Auth)
                .SetUrls("SequenceForm.aspx")
                .AddColumn<Sequence>(t => t.TypeName, 120, "类型")
                .AddColumn<Sequence>(t => t.LoopName, 120, "循环模式")
                .AddColumn<Sequence>(t => t.Format, 200, "格式")
                .AddColumn<Sequence>(t => t.LastDt, 200, "最后时间")
                .AddColumn<Sequence>(t => t.LastValue, 120, "最后值")
                .AddColumn<Sequence>(t => t.LastSeq, 200, "最后序列号")
                .InitGrid<Sequence>(this.BindGrid, Grid1, t => t.Format)
                ;
            if (!IsPostBack)
            {
                this.Grid1.SetSortPage<Sequence>(SiteConfig.Instance.PageSize, t => t.ID, true);
                BindGrid();
            }
        }

        // 绑定网格
        private void BindGrid()
        {
            IQueryable<Sequence> q = Sequence.Search(null);
            Grid1.Bind(q);
        }
    }
}
