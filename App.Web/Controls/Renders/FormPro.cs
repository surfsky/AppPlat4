using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Reflection;
using System.Linq.Expressions;
using FineUIPro;
//using App.DAL;  // AppContext
using App.Utils;
using App.Entities;
using App.DAL;

namespace App.Controls
{
    /// <summary>
    /// 扩展表单控件。带保存按钮及逻辑，可根据实体类型自动生成表单。
    /// 可考虑和FormPage合并。
    /// </summary>
    public class FormPro : FineUIPro.Form
    {
        // 属性
        public EntityBase Entity { get; set; }                // 实体
        public string EntityTypeName { get; set; }            // 实体类型名称
        public long?  EntityID { get; set; }                  // 实体id
        public long?  UIID { get; set; }                      // UI 配置ID（数据来自UI配置表)
        public PageMode? Mode { get; set; }                   // 模式
        protected Type EntityType { get; set; }               // 实体类型
        protected UISetting UI { get; set; }                  // UI设置

        // UI
        public bool ShowCloseButton { get; set; } = false;    // 是否显示关闭按钮
        public bool ShowIdLabel { get; set; } = true;         // 是否显示ID标签行

        // 事件
        public event EventHandler<EntityOp> AfterSave;        // 保存后事件

        // 私有
        protected Label  lblId;                               // ID
        protected ToolbarText lblInfo;                        // Info
        protected Button btnClose;                            // 关闭按钮
        protected Button btnSave;                             // 保存后关闭按钮
        protected Button btnSaveNew;                          // 保存并新增按钮
        protected Dictionary<string, EditorInfo> map;         // 字段控件映射字典
        protected Window win;                                 // 弹窗
        protected Button btnSetting;                          // 设置按钮

        /*
        <f:Form ID = "SimpleForm1" ShowBorder="false" ShowHeader="false" runat="server" BodyPadding="10px"  Title="SimpleForm" LabelWidth="200">
            <Items>
                <f:Label runat = "server" ID ="lblId" Label="ID" Hidden="false" />
            </Items>
        </f:Form>
        */
        // Init
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.BoxFlex = 1;
            this.ShowBorder = false;
            this.ShowHeader = false;
            this.BodyPadding = "10px";
            this.LabelWidth = 200;
            this.AutoScroll = true;
        }

        /// <summary>显示提示信息</summary>
        public void ShowInfo(string format, params object[] args)
        {
            if (format.IsNotEmpty())
            {
                var info = string.Format(format, args);
                this.lblInfo.Text = info;
                App.Controls.UI.ShowHud(info);
            }
        }

        //-----------------------------------------------
        // 页面事件
        //-----------------------------------------------
        /// <summary>初始化表单</summary>
        public void Build(Type type)
        {
            Build(type, null, null, null, null);
        }
        public void Build(EntityBase o, UISetting ui = null)
        {
            Build(o.GetType(), ui, null, o.ID, o);
        }
        public void Build(UISetting ui)
        {
            Build(ui.EntityType, ui, null, null, null);
        }
        protected void Build(Type type, UISetting ui, long? uiID, long? id, EntityBase entity)
        {
            this.UI = ui;
            this.UIID = uiID ?? Asp.GetQueryLong("uiId");
            this.Entity = entity;
            this.EntityID = id ?? Asp.GetQueryLong("id");
            this.EntityType = type;
            this.EntityTypeName = type.FullName;
            BuildForm();
        }


        /// <summary>设置控件初始值</summary>
        public void SetControlValues(Dictionary<string, string> dict)
        {
            foreach (string key in dict.Keys)
            {
                var item = map.GetItem(key, true);
                if (item != null)
                {
                    var value = dict[key];
                    item.Set(value);
                    item.Editor.Enabled = false;
                }
            }
        }


        /// <summary>构建表单</summary>
        public FormPro BuildForm()
        {
            // 构建表单
            this.Mode = this.Mode ?? Common.PageMode;
            InitToolbar();
            InitIDField();

            // 若 UI 配置为空，尝试获取保存在数据库中的UI，或实体默认编辑UI
            if (this.UI == null)
            {
                var uiId = UIRender.GetSelectedMenuId(btnSetting.Menu).ParseLong();
                if (uiId != null && uiId != 0)
                {
                    var xui = XUI.Get(uiId);
                    this.UI = xui.Setting;
                    if (this.UI?.EntityType == null)
                        throw new Exception("UISetting.EntityType 为空，请检查 UI 配置信息: " + xui.Name);
                }
                if (this.UI == null)
                    this.UI = AppContext.FormUIs.FirstOrDefault(t => t.EntityType == this.EntityType);
            }

            // 创建表单
            var readOnly = (this.Mode == PageMode.View);
            var data = this.GetData();
            this.map = FormRender.BuildForm(this, this.UI, this.Mode, false, readOnly, data);

            // 根据参数值设置表单
            var dict = UIRender.ParseQueryData();
            SetControlValues(dict);

            // 只读控制
            if (readOnly)
            {
                this.btnSave.Hidden = true;
                this.btnSaveNew.Hidden = true;
                FormRender.SetFormEditable(this, false);
            }

            // 初始化
            if (!(HttpContext.Current.Handler as Page).IsPostBack)
            {
                this.lblId.Text = this.EntityID?.ToString();
                ShowInfo(Asp.GetQueryString("info"));
                this.btnSaveNew.Visible = (this.EntityID == null);
                ShowData(data);
            }

            // 若工具栏无任何编辑或按钮控件可视，则隐藏
            bool hide = true;
            foreach (var item in this.Toolbars[0].Items)
            {
                if (!(item is ToolbarText) && !(item is ToolbarSeparator) && !(item is ToolbarFill) && !item.Hidden)
                {
                    hide = false;
                    break;
                }
            }
            if (hide)
                this.Toolbars[0].Hidden = true;

            //
            return this;
        }

        /*
        <Toolbars>
            <f:Toolbar ID = "Toolbar1" runat="server">
                <Items>
                    <f:Button runat = "server" ID="btnClose" Icon="SystemClose" Text="关闭" OnClick="btnClose_Click"  />
                    <f:ToolbarSeparator runat = "server" ID="ToolbarSeparator2" />
                    <f:Button runat = "server" ID="btnSaveClose" ValidateForms="SimpleForm1" Icon="SystemSaveClose" OnClick="btnSaveClose_Click" Text="保存并关闭" />
                    <f:Button runat = "server" ID="btnSaveNew" ValidateForms="SimpleForm1" Icon="SystemSaveNew" OnClick="btnSaveNew_Click" Text="保存并新增" />
                </Items>
            </f:Toolbar>
        </Toolbars>
        */
        void InitToolbar()
        {
            // 工具栏
            if (this.Toolbars.Count == 0)
                this.Toolbars.Add(new Toolbar());
            var toolbar = this.Toolbars[0];

            // 信息标签
            lblInfo = new ToolbarText() { CssStyle = "color:red" };

            // 关闭按钮
            btnClose = new Button() { Icon = Icon.SystemClose, Text = "关闭" };
            btnClose.OnClientClick = ActiveWindow.GetHidePostBackReference();

            // 保存
            btnSave = new Button() { Icon = Icon.SystemSave, Text = "保存", ValidateForms = new string[] { this.ID } };
            btnSave.Click += (s, e) =>
            {
                try
                {
                    var item = SaveData();
                    var info = string.Format("保存成功({0:HH:mm:ss})", DateTime.Now);
                    ShowInfo(info);
                    // 新增后，转到编辑页面
                    if (this.Mode == PageMode.New)
                        PageBase.ChangePageMode(PageMode.Edit, (item as IID).ID, info);
                }
                catch (Exception ex)
                {
                    App.Controls.UI.ShowAlert("保存数据失败。" + ex.Message);
                }
            };

            // 保存并新增按钮
            btnSaveNew = new Button() { Icon = Icon.SystemSaveClose, Text = "保存并新增", ValidateForms = new string[] { this.ID } };
            btnSaveNew.Click += (s, e) =>
            {
                try
                {
                    SaveData();
                    ClearData();
                    ShowInfo("保存成功({0:HH:mm:ss})，新增中", DateTime.Now);
                }
                catch (Exception ex)
                {
                    App.Controls.UI.ShowAlert("保存数据失败。" + ex.Message);
                }
            };

            // 添加到工具栏
            toolbar.Items.Insert(0, new ToolbarSeparator());
            toolbar.Items.Insert(0, btnSaveNew);
            toolbar.Items.Insert(0, btnSave);
            if (ShowCloseButton)
                toolbar.Items.Insert(0, btnClose);
            toolbar.Items.Add(new ToolbarFill());
            if (this.UI == null)
            {
                win = App.Controls.UI.CreateWindow("配置", 800, 600);
                this.Items.Add(win);
                btnSetting = UIRender.CreateUISettingButton(this.EntityTypeName, win, XUIType.Form, this.UIID); // UI配置控件组
                toolbar.Items.Add(btnSetting);
            }
            toolbar.Items.Add(lblInfo);
        }


        //<f:Label runat = "server" ID ="lblId" Label="ID" Hidden="false" />
        void InitIDField()
        {
            lblId = new Label() { Label = "ID", Hidden= !ShowIdLabel };
            this.Items.Add(lblId);
        }

        /// <summary>添加要展示的字段（未使用）</summary>
        /// <example>form.AddField<Order>(t => t.Name).AddField<Order>(t => t.Customer).BuildForm();</example>
        public FormPro AddField<T, TValue>(Expression<Func<T, TValue>> property)
        {
            var name = property.GetName();
            var type = typeof(T);
            return AddField(type, name);
        }

        /// <summary>添加要展示的字段（未使用）</summary>
        public FormPro AddField(Type type, string name)
        {
            this.EntityType = type;
            if (UI == null)
                this.UI = new UISetting();
            var attr = type.GetUIAttribute(name);
            this.UI.Items.Add(attr);
            return this;
        }


        //-----------------------------------------------
        // 数据清空、展示、采集、保存
        //-----------------------------------------------
        // 清空数据
        void ClearData()
        {
            this.lblId.Text = "";
            foreach (var key in this.map.Keys)
                map[key].Set(null);
        }

        // 加载数据
        void ShowData(object o)
        {
            if (o != null)
                FormRender.ShowFormData(this.map, o);
        }


        // 保存数据
        object SaveData()
        {
            // 如何统一调用 EntityBase<T>.Save() 方法?
            if (this.Mode == PageMode.New)
            {
                // 新增
                EntityBase item = AppContext.Current.Set(EntityType).Create() as EntityBase;
                FormRender.CollectData(this.map, ref item);
                dynamic o = item;
                o.Save();

                // 保存后
                this.lblId.Text = item.GetValue("ID").ToString();
                if (AfterSave != null) AfterSave(item, EntityOp.New);
                return item;
            }
            else
            {
                // 更新
                var id = App.Controls.UI.GetLong(this.lblId) ?? Asp.GetQueryLong("id");
                var item = AppContext.Current.Set(EntityType).Find(id) as EntityBase;
                FormRender.CollectData(this.map, ref item);
                dynamic o = item;
                o.Save();

                // 保存后
                if (AfterSave != null)  AfterSave(item, EntityOp.Edit);
                return item;
            }
        }

        /// <summary>数据（来自数据库）</summary>
        public EntityBase GetData()
        {
            if (this.Entity != null)
                return this.Entity;

            var id = this.EntityID ?? Asp.GetQueryLong("id");
            if (id != null)
                return GetData(id.Value);
            return null;
        }

        /// <summary>获取数据，可重载该方法进行自定义获取方法</summary>
        public virtual EntityBase GetData(long id)
        {
            var type = this.EntityType;

            // 先尝试调用重载后的 GetDetail 方法
            var method = type.GetMethod("GetDetail", false);
            if (method != null)
            {
                var args = new Dictionary<string, object> { { "id", id } };
                return MethodInvoker.InvokeMethod(null, method, args) as EntityBase;
            }
            // 再尝试调用EntityBase.GetDetail 方法
            method = type.GetMethod("GetDetail", true);
            if (method != null)
            {
                var args = new Dictionary<string, object> { { "id", id } };
                return MethodInvoker.InvokeMethod(null, method, args) as EntityBase;
            }
            // 都没有则获取最简单的无关联的实体
            return AppContext.Current.Set(EntityType).Find(id) as EntityBase;
        }

    }
}