//using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using FineUIPro;
using App.Components;
using App.Utils;
//using App.DAL;  // Power
using App.Entities;
using App.DAL;

namespace App.Controls
{
    /// <summary>
    /// Grid 增强版
    /// - 包含分页、排序、删除、批量删除、导出Excel等逻辑
    /// - 可自动生成序号、查看、删除、编辑、ID列
    /// - 可根据实体属性自动生成列
    /// - 自动删除逻辑需要类型实现 IID 接口
    /// - 自动保持选中行
    /// - 内置弹窗控件，用于展示详情、修改、新增等页面
    /// - 排序和分页: 请调用 SetSortAndPage() 方法
    /// - 导出Excel: 请实现ExportExcel事件，可调用Grid1.ExportExcel()方法。
    /// - 若不指定编辑窗口，则使用默认数据编辑窗口（DataForm.aspx）
    /// </summary>
    /// <example>
    ///    &lt;f:GridPro ID="Grid1" runat="server"
    ///            NewUrlTmpl="~UserNew.aspx"
    ///            EditUrlTmpl="UserEdit.aspx?id={0}"
    ///            ViewUrlTmpl="UserView.aspx?id={0}"
    ///            ShowNumberField="true" ShowViewField="true" ShowDeleteField="true" ShowEditField="true" ShowIDField="false"
    ///            AllowNew="true" AllowEdit="true" AllowDelete="true" AllowBatchDelete="true" AllowExport="true"
    ///            RelayoutToolbar="false"
    ///            OnDelete="Grid1_Delete"
    ///    &gl;
    ///    Grid1.InitGrid<User>(BindGrid, Toolbar1);
    ///    Grid2.SetSortAndPage(....);
    /// </example>
    /// <info>
    /// - 由于设计器不支持泛型控件，所以本控件使用InitGrid&lt;T&gl;方法实现泛型
    /// - InitGrid()方法请在PageInit方法内调用，因为要动态生成一堆控件；
    /// - SetUrls(....) 若该方法不调用，则调用默认的编辑窗口
    /// </info>
    /// <target>
    /// - 将ID名称独立出来，不写死
    /// </target>
    /// <history>
    /// 2017-02-01 初始版本
    /// 2017-11-13 简化InitGrid方法，提取SetSortAndPage方法，使用强类型指定排序字段
    /// 2018-11-12 优化InitGrid（检测权限，强类型名称字段）；支持multi,md=select 请求参数
    /// 2019-12-23 更改思路，GridPro本身就是不泛型的，泛型的是UISetting
    /// </history>
    [Param("md", "显示模式（View|New|Edit|Select）", typeof(PageMode))]
    [Param("multi", "是否允许多选", typeof(bool))]
    [Param("pv", "查看权限", typeof(Powers))]
    [Param("pn", "新建权限", typeof(Powers))]
    [Param("pe", "编辑权限", typeof(Powers))]
    [Param("pd", "删除权限", typeof(Powers))]
    public partial class GridPro: FineUIPro.Grid
    {
        //-------------------------------------------------
        // 属性和事件
        //-------------------------------------------------
        // 公共属性
        public string NewText { get; set; } = "新增";
        public string DeleteText { get; set; } = "删除";
        public string ExportText { get; set; } = "导出";
        public string CloseText { get; set; } = "关闭";
        public string SelectText { get; set; } = "选择并关闭";
        public string NewUrlTmpl { get; set; }
        public string EditUrlTmpl { get; set; }
        public string ViewUrlTmpl { get; set; }
        //
        public Window Win { get { return _window; } }
        public string WinID { get; set; } = "Window1";
        public string WinTitle { get; set; } = "编辑";
        public int WinWidth { get; set; } = 700;
        public int WinHeight { get; set; } = 600;
        public CloseAction WinCloseAction { get; set; } = CloseAction.HidePostBack;
        //
        public bool ShowNumberField { get; set; } = true;
        public bool ShowIDField { get; set; } = false;
        public bool ShowViewField { get; set; } = true;
        public bool ShowEditField { get; set; } = true;
        public bool ShowDeleteField { get; set; } = true;
        public bool AllowView { get; set; } = true;
        public bool AllowNew { get; set; } = true;
        public bool AllowEdit { get; set; } = true;
        public bool AllowDelete { get; set; } = true;
        public bool AllowBatchDelete { get; set; } = true;
        public bool AllowExport { get; set; } = false;
        public bool AllowClose { get; set; } = false;
        public bool RelayoutToolbar { get; set; } = false;
        public Toolbar Toolbar { get; set; }

        // 关键值和名称列
        public string IdField { get; set; } = "ID";
        public string NameField { get; set; } = "Name";


        // 公开事件
        /// <summary>新增按钮事件</summary>
        public event EventHandler<string> New;
        /// <summary>删除按钮事件</summary>
        public event EventHandler<List<long>> Delete;
        /// <summary>导出按钮事件</summary>
        public event EventHandler Export;
        /// <summary>关闭按钮事件</summary>
        public event EventHandler Close;
        /// <summary>窗口关闭事件</summary>
        public event EventHandler WindowClose;
        /// <summary>关闭按钮事件</summary>
        public event EventHandler Select;


        // 私有
        protected HiddenField _hidden;      // 隐藏控件，存储选中行ID列表
        protected FineUIPro.Window _window; // 弹窗控件
        protected Action _bindAction;   // 显示数据的方法。排序、分页、弹窗关闭时会调用。


        // 以下三个按钮请在 InitGrid() 方法后调用
        public Button ExportButton { get; set; }
        public Button BatchDeleteButton { get; set; }
        public Button NewButton { get; set; }
        public Button CloseButton { get; set; }

        /// <summary>列回调方法集合（列ID-返回值为字符串的方法）</summary>
        private Dictionary<string, Func<object,string>> _funcs = new Dictionary<string, Func<object,string>>();

        /// <summary>页面模式</summary>
        protected PageMode? Mode { get; set; }

        //-------------------------------------------------
        // 初始化
        //-------------------------------------------------
        // Init
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.EnableTextSelection = true;
            this.BoxFlex = 1;
            this.ShowBorder = false;
            this.ShowHeader = false;
            this.DataKeyNames = new string[] { "ID" };
            this._hidden = new HiddenField();
            this.SetMultiSelectStatus();
        }

        /// <summary>设置文本</summary>
        public GridPro SetTexts(
            string newText = "新增", 
            string deleteText = "删除", 
            string closeText = "关闭", 
            string selectText = "选择并关闭",
            string exportText = "导出"
            )
        {
            this.NewText  = newText;
            this.DeleteText = deleteText;
            this.CloseText  = closeText;
            this.SelectText  = selectText;
            this.ExportText = exportText;
            return this;
        }

        /// <summary>设置为只读</summary>
        public GridPro SetReadOnly()
        {
            this.AllowNew = false;
            this.AllowDelete = false;
            this.AllowBatchDelete = false;
            this.AllowEdit = false;
            this.ShowEditField = false;
            this.EnableCheckBoxSelect = false;
            return this;
        }

        /// <summary>设置权限</summary>
        public GridPro SetPowers(AuthAttribute auth)
        {
            auth = auth ?? GetAuth();
            this.Auth = auth;
            return SetPowers(auth.ViewPower, auth.NewPower, auth.EditPower, auth.DeletePower);
        }

        /// <summary>从实体类或查询字符串中获取鉴权对象</summary>
        private AuthAttribute GetAuth()
        {
            AuthAttribute auth = null;
            if (EntityType != null)
                auth = EntityType.GetAttribute<AuthAttribute>();
            if (auth == null)
            {
                auth = new AuthAttribute();
                auth.ViewPower = Asp.GetQuery<Powers>("pv");
                auth.NewPower = Asp.GetQuery<Powers>("pn");
                auth.EditPower = Asp.GetQuery<Powers>("pe");
                auth.DeletePower = Asp.GetQuery<Powers>("pd");
            }
            return auth;
        }

        /// <summary>设置功能权限（参数值为 bool 或 PowerType 类型）</summary>
        public GridPro SetPowers(object viewPower, object newPower, object editPower, object deletePower)
        {
            this.Mode = this.Mode ?? Common.PageMode;
            bool read = (this.Mode == PageMode.View);
            this.AllowView        = CheckPower(viewPower);
            this.AllowNew         = CheckPower(newPower) && !read;
            this.AllowEdit        = CheckPower(editPower) && !read;
            this.AllowDelete      = CheckPower(deletePower) && !read;
            this.AllowBatchDelete = CheckPower(deletePower) && !read;
            if (AllowBatchDelete)
                this.EnableCheckBoxSelect = true;
            return this;
        }

        // 检测权限
        bool CheckPower(object power)
        {
            if (power is bool)
                return Convert.ToBoolean(power);
            if (power is Powers)
                return Common.CheckPower((Powers)power);
            return true;
        }

        /// <summary>设置是否采用自动表单</summary>
        public GridPro SetAutoForm(bool useAutoForm)
        {
            UseAutoForm = useAutoForm;
            return this;
        }

        /// <summary>设置Url模板</summary>
        public GridPro SetUrls(Type type, params string[] queryKeys)
        {
            SetAutoForm(true);
            var url = Urls.GetDataFormUrl(type);
            return SetUrls(url, queryKeys);
        }

        /// <summary>设置URL模板为DataForm.aspx</summary>
        public GridPro SetAutoUrls(params string[] queryKeys)
        {
            SetAutoForm(true);
            var url = Urls.GetDataFormUrl(this.EntityType);
            this.SetUrls(url, queryKeys);
            return this;
        }

        /// <summary>设置Url模板（会自动增加mode和id参数）</summary>
        public GridPro SetUrls(string url, params string[] queryKeys)
        {
            SetAutoForm(false);
            // 附加查询参数
            if (queryKeys != null)
            {
                foreach (var key in queryKeys)
                {
                    var v = Asp.GetQueryString(key);
                    if (v.IsNotEmpty())
                        url = url.AddQueryString(key + "=" + v);
                }
            }

            // 补足mode和id参数
            var connector = url.Contains("?") ? "&" : "?";
            this.NewUrlTmpl  = url + connector + "md=new";
            this.ViewUrlTmpl = url + connector + "md=view&id={0}";
            this.EditUrlTmpl = url + connector + "md=edit&id={0}";
            return this;
        }


        /// <summary>设置Url模板</summary>
        public GridPro SetUrls(string newUrlTmpl, string viewUrlTmpl, string editUrlTmpl)
        {
            this.NewUrlTmpl = newUrlTmpl;
            this.EditUrlTmpl = editUrlTmpl;
            this.ViewUrlTmpl = viewUrlTmpl;
            return this;
        }



        /// <summary>初始化网格(及相关附属控件)，请在每次页面初始化时都调用</summary>
        /// <param name="toolbar">按钮将在该工具栏上生成。若为空则设置为网格的第一个工具栏。</param>
        public GridPro InitGrid<T>(Action dataBinding, PanelBase toolbarPanel, Expression<Func<T, object>> nameField, bool createToolbar=false)
            where T : EntityBase<T>
        {
            // 自动设置弹窗URL
            if (this.ViewUrlTmpl.IsEmpty() && this.EditUrlTmpl.IsEmpty() && this.NewUrlTmpl.IsEmpty())
                this.SetUrls(typeof(T));

            // 只读
            var url = HttpContext.Current.Request.Url;
            var mode = this.Mode;
            if (mode == PageMode.View || mode == PageMode.Select)
                SetReadOnly();

            // ID Name field
            this.NameField = nameField == null ? "" : nameField.GetName();
            SetDataKeyNames();

            // 动态生成控件
            SetDataBinding(dataBinding);
            SetToolbar(toolbarPanel, createToolbar);
            AddWindow();
            AddPager();
            AddControlColumns();
            AddToolbarButtons<T>();
            SetDeleteEvent<T>();
            SetSortPageEvent();
            SetRowEvent();
            SetLastColumnExpand();
            return this;
        }

        /// <summary>设置行事件</summary>
        public GridPro SetRowEvent()
        {
            this.RowDataBound += GridPro_RowDataBound;
            return this;
        }

        /// <summary>设置数据绑定函数</summary>
        public GridPro SetDataBinding(Action bindAction)
        {
            this._bindAction = bindAction;
            return this;
        }

        /// <summary>获取或新建工具栏</summary>
        public GridPro SetToolbar(PanelBase toolbarPanel, bool newToolbar)
        {
            toolbarPanel = toolbarPanel ?? this;
            this.Toolbar = newToolbar
                ? UI.CreateToolbar(toolbarPanel, true)
                : UI.GetToolbar(toolbarPanel)
                ;
            return this;
        }

        /// <summary>设置数据主键</summary>
        public GridPro SetDataKeyNames()
        {
            if (this.NameField.IsEmpty())
                this.DataKeyNames = new string[1] { this.IdField };
            else
                this.DataKeyNames = new string[2] { this.IdField, this.NameField };
            return this;
        }

        /// <summary>行绑定事件</summary>
        private void GridPro_RowDataBound(object sender, GridRowEventArgs e)
        {
            var data = e.DataItem; //as EntityBase;

            // 若未设置编辑窗口（使用内置的 DataFrom.aspx 进行编辑，WindowsField.Url 需要加密处理）
            foreach (var col in this.Columns)
            {
                WindowField field = col as WindowField;
                if (field != null && (field.ID == "Edit" || field.ID == "View"))
                {
                    if (field.DataIFrameUrlFormatString.Contains("DataForm.aspx"))
                    {
                        var auth = new AuthAttribute();
                        if (Page is PageBase)
                            auth = (Page as PageBase).Auth;
                        else
                        {
                            auth.ViewPower   = Asp.GetQuery<Powers>("pv");
                            auth.NewPower    = Asp.GetQuery<Powers>("pn");
                            auth.EditPower   = Asp.GetQuery<Powers>("pe");
                            auth.DeletePower = Asp.GetQuery<Powers>("pd");
                        }
                        PageMode mode = field.ColumnID.Contains("Edit") ? PageMode.Edit : PageMode.View;
                        Type type = data.GetType().GetEntityType();
                        long id = (long)data.GetValue("ID");
                        var url = Urls.GetDataFormUrl(type, id, mode, auth);
                        UI.SetGridWinCellUrl(this, field.ColumnID, url, e);
                        //field.NavigateDataIFrameUrl = url;
                    }
                }
            }

            // 自定义回调事件列
            foreach (var name in this._funcs.Keys)
            {
                var field = this.FindColumn(name) as BaseField;
                if (field != null)
                {
                    var func = _funcs[name];
                    var value = func(data);
                    e.Values[field.ColumnIndex] = value;
                }
            }

            // 缩略图列数据校验（如果不是图片，就去掉缩略图，避免对普通页面的访问）
            foreach (var col in this.Columns)
            {
                ThrumbnailField field = col as ThrumbnailField;
                if (field != null)
                {
                    var url = Reflector.GetValue(data, field.DataImageUrlField) as string;
                    if (!IO.IsImageFile(url))
                        e.Values[field.ColumnIndex] = "";
                }
            }
        }



        //-------------------------------------------------
        // 分页及排序
        //-------------------------------------------------
        /// <summary>设置排序和分页逻辑。在窗口首次创建时调用。if (!IsPostBack){ Grid1.SetSort(...);}</summary>
        /// <remarks>
        /// AllowSorting="true" SortField="Month" SortDirection="ASC" OnSort="Grid1_Sort"  
        /// AllowPaging="true" IsDatabasePaging="true" OnPageIndexChange="Grid1_PageIndexChange"
        /// </remarks>
        public GridPro SetSortPage<T>(int pageSize, Expression<Func<T, object>> sortField, bool ascend = true, bool allowSort = true, bool allowPage = true)
        {
            string field = (sortField==null) ? "ID" : sortField.GetName();
            SetSortPage(pageSize, field, ascend, allowPage, allowSort);
            return this;
        }


        /// <summary>设置排序和分页逻辑（开启分页的话必须允许排序，否则SortAndPage()方法会报错）</summary>
        /// <remarks>
        /// AllowSorting="true" SortField="Month" SortDirection="ASC" OnSort="Grid1_Sort"  
        /// AllowPaging="true" IsDatabasePaging="true" OnPageIndexChange="Grid1_PageIndexChange"
        /// </remarks>
        GridPro SetSortPage(int pageSize = 30, string sortField = "ID", bool ascend = true, bool allowPage = true, bool allowSort = true)
        {
            SetPage(pageSize, allowPage);

            // sort
            if (allowPage) allowSort = true;
            this.AllowSorting = allowSort;
            this.SortField = sortField;
            this.SortDirection = ascend ? "ASC" : "Desc";
            return this;
        }

        /// <summary>设置分页和排序（根据GridUI）</summary>
        public GridPro SetSortPage(int pageSize, bool allowPage = true, bool allowSort = true)
        {
            SetPage(pageSize, allowPage);
            SetSortByUI();
            return this;
        }


        /// <summary>设置排序（根据UI配置）</summary>
        GridPro SetSortByUI()
        {
            UISetting ui = this.GridUI;
            var attr = ui.Items.FirstOrDefault(t => t.Sort != null);
            if (attr != null)
            {
                this.SortField = attr.Name;
                this.SortDirection = attr.Sort.Value ? "ASC" : "DES";
            }
            else
            {
                this.SortField = "ID";
            }
            return this;
        }


        /// <summary>设置分页（一定要记得设置排序字段，否则程序会报错）</summary>
        public GridPro SetPage(int pageSize, bool allowPage=true)
        {
            this.AllowPaging = allowPage;
            this.IsDatabasePaging = true; // 只允许数据库分页
            this.PageSize = pageSize;
            if (this._ddlGridPageSize != null)
            {
                this._ddlGridPageSize.Hidden = !allowPage;
                this._ddlGridPageSize.SelectedValue = PageSize.ToString();
            }

            if (allowPage)
                this.AllowSorting = true;
            return this;
        }

        /// <summary>设置页面排序及分页事件</summary>
        public GridPro SetSortPageEvent()
        {
            this.Sort += (s, e) =>
            {
                this.SortField = e.SortField;
                this.SortDirection = e.SortDirection;
                ShowData();
            };
            this.PageIndexChange += (s, e) =>
            {
                this.PageIndex = e.NewPageIndex;
                ShowData();
            };
            _ddlGridPageSize.SelectedIndexChanged += (s, e) =>
            {
                this.PageSize = Convert.ToInt32(_ddlGridPageSize.SelectedValue);
                if (this.PageSize != 0)
                    ShowData();
            };
            return this;
        }



        //-------------------------------------------------
        // 网格绑定及数据查询事件
        //-------------------------------------------------
        // 显示数据（翻页、排序、关闭弹窗、删除数据后会自动调用）
        protected void ShowData()
        {
            // 保存当前选中行的ID；显示数据；恢复选中行ID；
            var ids = this.GetSelectedIds();
            if (_bindAction != null)
                _bindAction();
            this.SetSelectedIds(ids);

            // 设置选择模式时的值
            //SetSelectModeValue();
        }

        /// <summary>绑定到IQueryable</summary>
        public void Bind<T>(IQueryable<T> q)
        {
            GridHelper.Bind(this, q);
        }

        //---------------------------------------------------
        // 设置选择模式
        //---------------------------------------------------
        /// <summary>设置多选状态（根据url参数值）</summary>
        private void SetMultiSelectStatus()
        {
            // 选择模式下，默认值为false，除非显示声明是多选的;
            // 设置选中的数据在PreRender事件中实现
            bool b = (this.Mode == PageMode.Select)
                ? (Asp.GetQueryBool("multi") == true)
                : this.EnableMultiSelect
                ;
            this.EnableMultiSelect = b;
            this.EnableCheckBoxSelect = b;
        }

        /// <summary>预渲染前设置一下选择值</summary>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            SetSelectModeValue();
        }

        // 设置选择模式时的值
        private List<long> SetSelectModeValue()
        {
            // 根据参数 Value 预设选中值
            var ids = new List<long>();
            if (this.Mode == PageMode.Select)
            {
                var value = Asp.GetQueryString("value");
                if (OnSetValue != null)
                    OnSetValue(value);
                else
                {
                    try
                    {
                        ids = value.SplitLong();
                        this.SetSelectedIds(ids);
                    }
                    catch { }
                }
            }
            return ids;
        }

        /// <summary>设置选中值。可由子类继承覆盖实现。</summary>
        public event Action<string> OnSetValue;

        //-------------------------------------------------
        // 关闭
        //-------------------------------------------------
        /// <summary>关闭按钮事件：尝试触发自定义事件，否则关闭窗口并刷新主窗口）</summary>
        private void DoClose()
        {
            if (this.Close != null)
                Close(this, null);
            else
                PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
        }

        //-------------------------------------------------
        // 新增
        //-------------------------------------------------
        /// <summary>新增按钮事件：尝试触发自定义事件，，否则弹出新增窗口）</summary>
        private void DoNew()
        {
            var id = this.GetSelectedId();
            var url = string.Format(this.NewUrlTmpl, id).ToSignUrl();
            if (this.New != null)
                this.New(this, url);
            else
                ShowWindow(url, this.NewText);
        }



        //-------------------------------------------------
        // 删除
        //-------------------------------------------------
        // 行删除按钮（先尝试直接调用 OnDelete 事件，若不存在则调用默认删除方法）
        protected void SetDeleteEvent<T>() where T : EntityBase<T> // class, IID
        {
            if (ShowDeleteField && AllowDelete)
            {
                this.RowCommand += (s, e) =>
                {
                    if (e.CommandName == "Delete")
                    {
                        var id = this.GetSelectedId();
                        if (id == -1) return;
                        DeleteBatch<T>(new List<long>() { id });
                        ShowData();
                    }
                };
            }
        }

        /// <summary>设置删除事件</summary>
        public GridPro SetDeleteEvent()
        {
            this.RowCommand += (s, e) =>
            {
                if (e.CommandName == "Delete")
                {
                    var id = this.GetSelectedId();
                    if (id == -1) return;
                    if (Delete != null)
                        Delete(this, new List<long>() { id });
                    ShowData();
                }
            };
            return this;
        }

        // 批量删除
        protected void DoDeleteBatch<T>() where T : EntityBase<T> // class, IID
        {
            List<long> ids = this.GetSelectedIds();
            DeleteBatch<T>(ids);
            ShowData();
        }

        // 批量删除（先尝试直接调用 OnDelete 事件，若不存在则调用默认删除方法）
        void DeleteBatch<T>(List<long> ids) where T : EntityBase<T>// class, IID
        {
            if (ids.Count == 0) return;
            if (Delete != null)
                Delete(this, ids);
            else
            {
                // 依次删除（可处理复杂逻辑）
                var items = AppContext.Current.Set<T>().Where(t => ids.Contains(t.ID)).ToList();
                foreach (var item in items)
                    (item as EntityBase<T>).Delete();

                // 直接批量物理删除
                //ids.ForEach(id => EntityBase<T>.DeleteRes(id));
                //AppContext.Current.Set<T>().Where(t => ids.Contains(t.ID)).Delete();
            }
        }

        //-------------------------------------------------
        // 导出
        //-------------------------------------------------
        /// <summary>导出Excel。要求实现Export事件</summary>
        protected void DoExport<T>()
        {
            if (Export != null)
                Export(this, null);
        }
        /// <summary>导出Excel。</summary>
        public void ExportExcel<T>(List<T> list, string fileName="")
        {
            if (string.IsNullOrEmpty(fileName))
                fileName = string.IsNullOrEmpty(Title)
                    ? string.Format("{0}-{1:yyyyMMddHHmm}.xls", typeof(T).GetTypeInfo().Name, DateTime.Now)
                    : string.Format("{0}-{1:yyyyMMddHHmm}.xls", Title, DateTime.Now)
                    ;
            ExcelHelper.Export<T>(list, fileName);
        }
    }
}