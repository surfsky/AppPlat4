using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FineUIPro;
using App.Components;
using App.Utils;
//using App.DAL;  // AppContext
using App.Entities;
using App.DAL;

namespace App.Controls
{
    /// <summary>
    /// 根据配置自动创建列及检索工具栏
    /// 注：此类比较独立，但出于使用上的方便，和GridPro合并
    /// </summary>
    public partial class GridPro
    {
        public Type EntityType { get; set; }
        public UISetting GridUI { get; set; }
        public UISetting SearchUI { get; set; }
        public AuthAttribute Auth { get; set; }
        public bool UseAutoForm { get; set; }
        protected SearchRender _search;
        protected Button _btnSetting;


        public GridPro SetUI(Type type, PageMode? mode = null)
        {
            return SetUI(type, null, mode);
        }
        public GridPro SetUI(UISetting ui, PageMode? mode = null)
        {
            return SetUI(ui.EntityType, ui, mode);
        }

        /// <summary>设置关联实体类及UI</summary>
        /// <param name="ui">网格UI配置。若为空，则尝试从实体类中解析。</param>
        /// <param name="searchUI">检索工具栏UI配置。若为空，则尝试从实体类中解析Search方法。</param>
        /// <param name="auth">页面授权</param>
        GridPro SetUI(Type type=null, UISetting ui = null, PageMode? mode=null)
        {
            this.EntityType = type ?? GridUI?.EntityType;
            this.GridUI = ui;
            this.Mode = mode ?? Common.PageMode;
            this.SetPowers(null);
            return this;
        }

        /// <summary>设置检索工具栏</summary>
        public GridPro SetSearcher(UISetting searchUI)
        {
            this.SearchUI = searchUI;
            return this;
        }

        /// <summary>初始化网格（弹窗、分页器、工具栏、搜索栏、控制列、自动列）</summary>
        public void Build(PanelBase toolbarPanel=null, bool newToolbar=false)
        {
            // 构建网格
            this
                .SetDataKeyNames()
                .SetToolbar(toolbarPanel, newToolbar)
                .AddWindow()
                .AddPager()
                .AddNewButton()
                .AddSelectButton()
                .AddControlColumns()
                .SetDeleteEvent()
                .SetSortPageEvent()
                .SetDataBinding(BindGrid)
                .SetRowEvent()
                ;

            // 创建搜索控件组
            AddExportButton();
            AddToolbarFill();
            AddSearcher();
            AddSettingButton();

            // 创建列
            if (GridUI == null)
                GridUI = GetGridUISetting();
            this.AddColumns(this.GridUI);
            this.SetLastColumnExpand();

            // 各种事件
            this.Delete += (s, ids) =>
            {
                foreach (long id in ids)
                {
                    var item = AppContext.Current.Set(EntityType).Find(id) as EntityBase;
                    item.Delete();
                    //var entry = AppContext.Current.Entry(item);
                    //entry.State = EntityState.Deleted;
                }
                AppContext.Current.SaveChanges();

            };
            this.PreRowDataBound += (s,e)=>
            {
                dynamic data = e.DataItem;
                long id = (long)Reflector.GetValue(data, "ID");  // data.ID
                if (UseAutoForm)
                {
                    var dataField = this.FindColumn("Edit") as FineUIPro.WindowField;
                    if (dataField != null)
                        dataField.DataIFrameUrlFormatString = Urls.GetDataFormUrl(EntityType, id, PageMode.Edit, this.Auth);
                }
            };
            UI.SetVisibleByQuery("search", this.Toolbar);
        }

        


        //---------------------------------------------------
        // 搜索工具栏
        //---------------------------------------------------
        /// <summary>添加工具栏填充件</summary>
        public GridPro AddToolbarFill()
        {
            this.Toolbar.Items.Add(new ToolbarFill());
            return this;
        }

        /// <summary>添加工具栏UI设置按钮</summary>
        public GridPro AddSettingButton()
        {
            if (this.GridUI == null)
            {
                var uiId = Asp.GetQueryLong("uiId");
                var win = UI.CreateWindow("配置", 800, 600);
                _btnSetting = UIRender.CreateUISettingButton(EntityType.FullName, win, XUIType.List, uiId);
                this.Toolbar.Items.Add(win);
                this.Toolbar.Items.Add(_btnSetting);
            }
            return this;
        }

        /// <summary>添加工具栏查找相关控件组</summary>
        public GridPro AddSearcher()
        {
            UISetting ui = this.SearchUI;
            if (ui == null)
                ui = AppContext.SearchUIs.FirstOrDefault(t => t.EntityType == EntityType);
            if (ui != null)
            {
                var dict = UIRender.ParseQueryData();
                _search = new SearchRender(ui);
                _search.Render(this.Toolbar.Items, dict);
                var btn = new Button() { Text = "查找", Icon = Icon.SystemSearch, Type = ButtonType.Submit };
                btn.Click += (s, e) => BindGrid();
                this.Toolbar.Items.Add(btn);
            }
            return this;
        }


        /// <summary>添加工具栏导出按钮</summary>
        private GridPro AddExportButton()
        {
            var btnExport = new Button() { Icon = Icon.PageExcel, Text = "导出", EnablePostBack = false };
            var mb1 = new MenuButton() { Text = "查询结果", EnableAjax = false, DisableControlBeforePostBack = false };
            var mb2 = new MenuButton() { Text = "选中数据", EnableAjax = false, DisableControlBeforePostBack = false };
            mb1.Click += (s, e) =>
            {
                var d = GetData(false, true);
                var fileName = string.Format("{0}_{1:yyyyMMddHHmm}.xls", EntityType.Name, DateTime.Now);
                ExportExcel(d, fileName);
            };
            mb2.Click += (s, e) =>
            {
                var d = GetData(false, true);
                var ids = GridHelper.GetSelectedIds(this);
                d = d.Search(t => ids.Contains((long)t.GetValue("ID")));
                var fileName = string.Format("{0}_{1:yyyyMMddHHmm}.xls", EntityType.Name, DateTime.Now);
                ExportExcel(d, fileName);
            };
            btnExport.Menu.Items.Add(mb1);
            btnExport.Menu.Items.Add(mb2);
            this.Toolbar.Items.Add(btnExport);
            return this;
        }

        /// <summary>绑定和显示数据（AutoUI）</summary>
        public void BindGrid()
        {
            var d = GetData(true, false);
            this.DataSource = d;
            this.DataBind();
        }

        // 获取网格UI配置
        private UISetting GetGridUISetting()
        {
            // UI 配置
            UISetting ui = null;
            var uiId = UIRender.GetSelectedMenuId(_btnSetting.Menu).ParseLong();
            if (uiId != null && uiId != 0)
                ui = XUI.Get(uiId.Value).Setting;
            if (ui == null)
                ui = AppContext.GridUIs.FirstOrDefault(t => t.EntityType == EntityType);
            return ui;
        }


        /// <summary>获取查询结果</summary>
        /// <param name="currentPage">仅本页数据</param>
        private List<object> GetData(bool currentPage, bool useExport)
        {
            // 数据
            dynamic q = null;
            if (_search != null)
                q = _search.GetResult();
            else
            {
                q = AppContext.GetQuery(EntityType);
                // 根据url参数进行过滤（未完成，或难以实现）
                var dict = UIRender.ParseQueryData();
            }

            // 分页
            q = (currentPage) ? GridHelper.SortAndPage(q, this) : GridHelper.Sort(q, this);
            var data = System.Linq.Enumerable.ToList(q) as IList;

            // 导出
            return useExport
                ? data.Cast(t => (t as IExport)?.Export(ExportMode.Normal))
                : data.Cast(t => (t as object))
                ;
        }
    }
}