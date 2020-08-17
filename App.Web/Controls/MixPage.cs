using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Entities;
using App.Utils;
using FineUIPro;

namespace App.Controls
{
    [UI("混合页面（包含网格和弹窗）")]
    [Param("v", "页面类型。Grid | Form")]
    public class MixPage<T> : PageBase
        where T : EntityBase
    {
        protected FormPro form;
        protected GridPro grid;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grid = new GridPro() { ID = "grid", WinWidth = 800 };
            form = new FormPro() { ID = "form" };
            this.Form.Controls.Add(grid);
            this.Form.Controls.Add(form);
            Show(grid, form);
        }

        private void Show(GridPro grid, FormPro form)
        {
            var v = Asp.GetQuery<ViewType>("v");
            if (v == ViewType.Form)
            {
                grid.Hidden = true;
                ShowForm(form);
                PageManager.Instance.AutoSizePanelID = form.ID;
            }
            else
            {
                form.Hidden = true;
                ShowGrid(grid);
                PageManager.Instance.AutoSizePanelID = grid.ID;
            }
        }

        /// <summary>显示网格</summary>
        protected void ShowGrid(GridPro grid)
        {
            var ui = GetGridUI();
            var url = new Url(Request.RawUrl);
            url["v"] = ViewType.Form.ToString();
            grid.SetUI(ui).SetPowers(this.Auth).SetUrls(url.ToString()).Build();
            if (!IsPostBack)
            {
                grid.SetSortPage(DAL.SiteConfig.Instance.PageSize);
                grid.BindGrid();
            }
        }

        /// <summary>显示表单</summary>
        protected void ShowForm(FormPro form)
        {
            var ui = GetFormUI();
            form.Build(ui);
        }

        /// <summary>获取网格UI</summary>
        protected virtual UISetting GetGridUI()
        {
            return new UISetting<T>(true);
        }
        /// <summary>获取表单UI</summary>
        protected virtual UISetting GetFormUI()
        {
            return new UISetting<T>(true);
        }
    }

}
