using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
//using EntityFramework.Extensions;
using System.Linq.Expressions;
using FineUIPro;
using App.Entities;
using App.Utils;

namespace App.Controls
{
    /// <summary>
    /// 自动创建的控件
    /// </summary>
    public partial class GridPro: FineUIPro.Grid
    {
        //-------------------------------------------------
        // Window
        //-------------------------------------------------
        /// <summary>显示窗口</summary>
        public void ShowWindow(string url, string title, int? width = null, int? height = null)
        {
            UI.ShowWindow(this._window, url, this.NewText, width, height);
        }

        /// <summary>设置窗口</summary>
        public GridPro SetWindow(int width, int height, string title = "编辑", string winId = "window1", CloseAction closeAction = CloseAction.HidePostBack)
        {
            this.WinTitle = title;
            this.WinWidth = width;
            this.WinHeight = height;
            this.WinCloseAction = closeAction;
            this.WinID = winId;

            AddWindow();
            this.Win.Title = title;
            this.Win.Width = width;
            this.Win.Height = height;
            this.Win.CloseAction = closeAction;
            this.Win.ID = winId;
            return this;
        }

        /*
        <f:Window ID = "Window1" runat="server" IsModal="true" Hidden="true" Target="Top" 
            Width="700px" Height="600px" 
            EnableResize="true" EnableMaximize="true" EnableClose="false"
            EnableIFrame="true" IFrameUrl="about:blank" 
            OnClose="Window1_Close"
            />
        */
        /// <summary>添加窗口</summary>
        public GridPro AddWindow(string winId = "")
        {
            if (_window != null)
                return this;

            if (winId.IsEmpty())
                winId = "Window1";
            this.WinID = winId;
            _window = new FineUIPro.Window()
            {
                ID = this.WinID,
                Title = this.WinTitle,
                Width = this.WinWidth,
                Height = this.WinHeight,
                CloseAction = this.WinCloseAction,
                IsModal = true,
                Hidden = true,
                Target = FineUIPro.Target.Top,
                EnableResize = true,
                EnableMaximize = true,
                EnableClose = true,
                EnableIFrame = true,
                IFrameUrl = "about:blank"
            };
            _window.Close += (s, e) =>
            {
                if (WindowClose != null)
                    WindowClose(this, null);
                else
                {
                    //PageContext.RegisterStartupScript(_window.GetHideReference());
                    ShowData();
                }
            };
            this.Controls.Add(_window);
            return this;
        }

        //-------------------------------------------------
        // 工具栏按钮
        //-------------------------------------------------
        /*
        <Toolbars>
            <f:Toolbar ID = "Toolbar1" runat="server">
                <Items>
                    <f:Button runat="server" ID="btnNew"            Icon="Add"       Text="新增"  />
                    <f:Button runat="server" ID="btnDeleteSelected" Icon="Delete"    Text="删除"       ConfirmText="确定删除选定记录？"/>
                    <f:Button runat="server" ID="btnExportExcel"    Icon="PageExcel" Text="导出Excel"  EnableAjax="false" DisableControlBeforePostBack="false" />
                    <f:ToolbarFill runat = "server" />
                </Items>
            </f:Toolbar>
        </Toolbars>
        */
        protected void AddToolbarButtons<T>()
            where T : EntityBase<T> // class, IID
        {
            // 如果工具栏为空，则尝试插在网格第一个工具栏内
            if (this.Toolbar == null)
            {
                if (this.Toolbars.Count == 0)
                    this.Toolbars.Add(new FineUIPro.Toolbar());
                this.Toolbar = this.Toolbars[0];
            }

            // 填满左侧
            if (RelayoutToolbar)
                this.Toolbar.Items.Insert(0, new FineUIPro.ToolbarFill());

            // 关闭按钮、导出按钮、批量删除按钮、新增按钮、 选择按钮
            if (AllowClose)         AddCloseButton();
            if (AllowExport)        AddExportButton<T>();
            if (AllowBatchDelete)   AddBatchDeleteButton<T>();
            if (AllowNew)           AddNewButton();
            AddSelectButton();
        }


        /// <summary>添加选择按钮</summary>
        public GridPro AddSelectButton()
        {
            if (this.Mode != PageMode.Select)
                return this;

            var btn = new FineUIPro.Button() { Icon = FineUIPro.Icon.Accept, Text = this.SelectText };
            btn.Click += (s, e) =>
            {
                if (this.Select != null)
                    Select(this, null);
                else
                {
                    var ids = this.GetSelectedIds();
                    var names = this.GetSelectedNames();
                    var txt = names.ToSeparatedString();
                    var script = string.Format("{0}{1}",
                        ActiveWindow.GetWriteBackValueReference(txt, ids.ToSeparatedString()),
                        ActiveWindow.GetHideReference()
                        );
                    PageContext.RegisterStartupScript(script);
                }
            };
            this.Toolbar.Items.Insert(0, btn);
            return this;
        }

        // 关闭按钮
        public GridPro AddCloseButton()
        {
            var btn = new FineUIPro.Button() { Icon = FineUIPro.Icon.SystemClose, Text = this.CloseText };
            btn.Click += (s, e) => DoClose();
            this.Toolbar.Items.Insert(0, btn);
            CloseButton = btn;
            return this;
        }

        // 导出按钮
        public GridPro AddExportButton<T>() where T : EntityBase<T>
        {
            var btn = new FineUIPro.Button() { Icon = FineUIPro.Icon.PageExcel, Text = this.ExportText, EnableAjax = false, DisableControlBeforePostBack = false };
            btn.Click += (s, e) => DoExport<T>();
            this.Toolbar.Items.Insert(0, btn);
            ExportButton = btn;
            return this;
        }

        // 批量删除按钮
        public GridPro AddBatchDeleteButton<T>() where T : EntityBase<T>
        {
            var btn = new FineUIPro.Button() { Icon = FineUIPro.Icon.Delete, Text = this.DeleteText, ConfirmText = "确定删除选定记录么？" };
            btn.Click += (s, e) => DoDeleteBatch<T>();
            this.Toolbar.Items.Insert(0, btn);
            BatchDeleteButton = btn;
            return this;
        }

        // 新增按钮
        public GridPro AddNewButton()
        {
            var btn = new FineUIPro.Button() { Icon = FineUIPro.Icon.Add, Text = this.NewText };
            btn.Click += (s, e) => DoNew();
            this.Toolbar.Items.Insert(0, btn);
            NewButton = btn;
            return this;
        }

        //-------------------------------------------------
        // 分页控件
        //-------------------------------------------------
        /*
        <PageItems>
            <f:ToolbarSeparator ID="ToolbarSeparator3" runat="server" />
            <f:ToolbarText ID="ToolbarText1" runat="server" Text="每页记录数：" />
            <f:DropDownList ID="ddlGridPageSize" Width="80px" AutoPostBack="true" OnSelectedIndexChanged="ddlGridPageSize_SelectedIndexChanged" runat="server">
                <f:ListItem Text="10" Value="10" />
                <f:ListItem Text="20" Value="20" />
                <f:ListItem Text="50" Value="50" />
                <f:ListItem Text="100" Value="100" />
            </f:DropDownList>
        </PageItems>
        */
        protected FineUIPro.DropDownList _ddlGridPageSize;
        /// <summary>添加分页控件</summary>
        public GridPro AddPager()
        {
            this.PageItems.Add(new FineUIPro.ToolbarSeparator());
            this.PageItems.Add(new FineUIPro.ToolbarText() { Text = "每页记录数" });
            _ddlGridPageSize = new FineUIPro.DropDownList() { Width = 80, AutoPostBack = true };
            _ddlGridPageSize.Items.Add(new FineUIPro.ListItem("10", "10"));
            _ddlGridPageSize.Items.Add(new FineUIPro.ListItem("20", "20"));
            _ddlGridPageSize.Items.Add(new FineUIPro.ListItem("30", "30"));
            _ddlGridPageSize.Items.Add(new FineUIPro.ListItem("50", "50"));
            _ddlGridPageSize.Items.Add(new FineUIPro.ListItem("100", "100"));
            this.PageItems.Add(_ddlGridPageSize);
            return this;
        }


    }
}