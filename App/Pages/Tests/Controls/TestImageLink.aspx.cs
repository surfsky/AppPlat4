using System;
using System.Web.UI;
using System.Linq;
using FineUIPro;
using App.Components;
using App.Entities;
using App.Controls;
using App.DAL;

namespace App.Tests
{
    /// <summary>
    /// </summary>
    [Auth(Powers.Admin)]
    public partial class TestImageLink : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindGrid();
            }
        }

        // 绑定网格
        private void BindGrid()
        {
            IQueryable<Res> q = Res.Search(null).SortAndPage("ID", "INC", 0, 10);
            Grid1.Bind(q);
        }
    }
}
