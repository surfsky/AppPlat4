using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.Components;
using System.Reflection;
using App.Entities;
using App.Utils;
//using App.DAL;    // AppContext
using FineUIPro;
using App.DAL;

namespace App.Controls
{
    /// <summary>
    /// 表单窗口。实现实体的查看、编辑、新增逻辑。
    /// 输入参数：
    ///     id                      实体ID
    ///     mode = view/new/edit    查看/新建/编辑
    ///     create = true/false     若不存在时是否创建
    ///     showBtnClose            默认为false
    /// </summary>
    /// <example>
    /// protected void Page_Load(object sender, EventArgs e)
    /// {
    ///     this.InitForm(this.SimpleForm1, "CorePowerView", "CorePowerNew", "CorePowerEdit");
    ///     if (!IsPostBack)
    ///         ShowForm();
    /// }
    /// 重载三个虚方法：NewData(), ShowData(), CollectData()
    /// </example>
    public class FormPage<T> : PageBase, IDataForm<T>
        where T : EntityBase<T>
    {
        //---------------------------------------------
        // 成员
        //---------------------------------------------
        // 页面控件
        public Button btnClose;
        public Button btnSave;
        public Button btnSaveNew;
        public ToolbarText lblInfo;
        public FormBase frm;
        public Toolbar Toolbar { get; private set; }

        // 几个按钮的显示控制
        public bool ShowBtnClose { get; set; } = false;
        public bool ShowBtnSave { get; set; } = true;
        public bool ShowBtnSaveNew { get; set; } = true;



        /// <summary>显示提示信息</summary>
        public void ShowInfo(string format, params object[] args)
        {
            if (format.IsNotEmpty())
            {
                var info = string.Format(format, args);
                this.lblInfo.Text = info;
                UI.ShowHud(info);
            }
        }

        /// <summary>更改页面模式（修改url并刷新）</summary>
        public virtual void ChangeMode(PageMode mode, long id, string info="")
        {
            ChangePageMode(mode, id, info);
            /*
            // 想用Post方式刷新页面
            this.Mode = mode;
            if (this.Request.QueryString["id"].IsNullOrEmpty())
                this.Request.QueryString["id"] = id.ToString();  // 改不了，会报错
            this.ShowForm();
            */
        }

        //---------------------------------------------
        // IDataForm 接口方法，请在子类中重载实现逻辑
        //---------------------------------------------
        /// <summary>新建数据时调用，可重载该方法清空表单</summary>
        public virtual void NewData() { }

        /// <summary>编辑数据时调用，可重载该方法显示数据</summary>
        public virtual void ShowData(T item) { }

        /// <summary>采集表单数据供保存时调用，可重载该方法从表单获取数据</summary>
        public virtual void CollectData(ref T item) { }


        /// <summary>数据</summary>
        public T GetData()
        {
            var id = Asp.GetQueryLong("id");
            if (id != null)
                return GetData(id.Value);
            return null;
        }

        //---------------------------------------------
        // 其它虚方法，可在子类中重载
        //---------------------------------------------
        /// <summary>获取数据，可重载该方法进行自定义获取方法</summary>
        public virtual T GetData(long id)
        {
            // 尝试调用重载后的 GetDetail() 方法；如果没有重载的话，调用最简单的。
            var type = typeof(T);
            var method = type.GetMethod("GetDetail");
            if (method?.DeclaringType == type)
            {
                var args = new Dictionary<string, object> { {"id", id } };
                return MethodInvoker.InvokeMethod(null, method, args) as T;
            }
            else
                return AppContext.Current.Set<T>().Find(id);
        }

        

        /// <summary>保存数据前预处理。若为false则不进行后继存储操作。</summary>
        public virtual bool CheckData(T item)
        {
            return true;
        }

        /// <summary>新增或修改实体数据</summary>
        public virtual void SaveData(T item)
        {
            item.Save();
        }

        /// <summary>保存（含新增或修改逻辑）。允许抛出异常阻止保存操作。</summary>
        public T Save()
        {
            T item = (this.Mode == PageMode.New)
                ? AppContext.Current.Set<T>().Create()
                : GetData(Asp.GetQueryLong("id").Value)
                ;
            CollectData(ref item);
            if (CheckData(item))
            {
                SaveData(item);
                return item;
            }
            return null;
        }

        /// <summary>保存数据</summary>
        private void DoSave(bool addNew)
        {
            try
            {
                T item = Save();
                if (item == null)
                {
                    ShowInfo("保存失败");
                    return;
                }
                if (addNew)
                {
                    // 新增后，转到新建页面
                    ShowInfo("保存成功({0:HH:mm:ss})，新增中", DateTime.Now);
                    NewData();
                }
                else
                {
                    var info = string.Format("保存成功({0:HH:mm:ss})", DateTime.Now);
                    ShowInfo(info);
                    // 新增后，转到编辑或查看页面
                    if (this.Mode == PageMode.New)
                    {
                        if (Common.CheckPower(this.Auth?.EditPower))
                            this.ChangeMode(PageMode.Edit, item.ID, info);
                        else if (Common.CheckPower(this.Auth?.ViewPower))
                            this.ChangeMode(PageMode.View, item.ID, info);
                        else
                            UI.HideWindow(CloseAction.HideRefresh);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowInfo("保存失败");
                Alert.Show(ex.Message);
                return;
            }
        }

        //---------------------------------------------
        // 公有方法
        //---------------------------------------------
        /// <summary>初始化表单。访问权限验证；生成工具栏按钮；请在OnInit事件中调用。</summary>
        /// <param name="viewPower">PowerType? | boolean </param>
        /// <param name="newPower">PowerType? | boolean </param>
        /// <param name="editPower">PowerType? | boolean </param>
        public void InitForm(FormBase form, PanelBase toolbarPanel, 
            object viewPower = null, object newPower = null, object editPower=null, 
            bool relayoutToolbar = true
            )
        {
            // 检测页面访问权限
            viewPower = viewPower ?? this.Auth.ViewPower;
            newPower  = newPower  ?? this.Auth.NewPower;
            editPower = editPower ?? this.Auth.EditPower;
            switch (this.Mode)
            {
                case PageMode.View: Common.CheckPagePower(viewPower); break;
                case PageMode.New:  Common.CheckPagePower(newPower); break;
                case PageMode.Edit: Common.CheckPagePower(editPower); break;
            }

            // 工具栏
            this.frm = form;
            this.Toolbar = (toolbarPanel == null) ? UI.GetToolbar(form) : UI.GetToolbar(toolbarPanel);

            // 工具栏控件
            InitToolbar(this.Toolbar, relayoutToolbar);
            this.ShowBtnClose = Asp.GetQueryBool("showBtnClose") ?? false;
            ShowInfo(Asp.GetQueryString("info"));
        }




        // 初始化工具栏控件
        // <f:Button runat="server" ID = "btnClose" Icon="SystemClose" EnablePostBack="false" Text="关闭" />
        // <f:Button runat="server" ID = "btnSaveClose" ValidateForms="SimpleForm1" Icon="SystemSaveClose" OnClick="btnSaveClose_Click" Text="保存后关闭" />
        // <f:Button runat="server" ID = "btnSaveNew" ValidateForms="SimpleForm1" Icon="SystemSaveNew" OnClick="btnSaveNew_Click" Text="保存并新增" />
        // <f:ToolbarText runat="server" />
        private void InitToolbar(Toolbar toolbar, bool relayoutToolbar)
        {
            // 信息标签
            lblInfo = new ToolbarText() { CssStyle = "color:red" };

            // 关闭按钮
            btnClose = new Button() { Icon = Icon.SystemClose, Text = "关闭", EnablePostBack = false };
            btnClose.OnClientClick = ActiveWindow.GetHidePostBackReference();

            // 保存并关闭按钮
            btnSave = new Button() { Icon = Icon.SystemSave, Text = "保存" };
            btnSave.ValidateForms = new string[] { this.frm.ID };
            btnSave.Click += (s, e) => DoSave(false);

            // 保存并新增按钮
            btnSaveNew = new Button() { Icon = Icon.SystemSaveNew, Text = "保存并新增" };
            btnSaveNew.ValidateForms = new string[] { this.frm.ID };
            btnSaveNew.Click += (s, e) => DoSave(true);

            // 添加到工具栏上
            if (relayoutToolbar)
                toolbar.Items.Insert(0, new ToolbarFill());
            toolbar.Items.Insert(0, btnSaveNew);
            toolbar.Items.Insert(0, btnSave);
            toolbar.Items.Insert(0, btnClose);
            toolbar.Items.Add(lblInfo);
        }




        /// <summary>
        /// 显示表单，请在页面首次初始化代码中调用
        /// </summary>
        public void ShowForm()
        {
            // 新建
            var mode = this.Mode;
            if (mode == PageMode.New)
            {
                NewData();
                ShowButtons(true, true, true);
                return;
            }

            // 尝试获取实体
            var id = Asp.GetQueryLong("id");
            if (id == null)
                return;
            T item = GetData(id.Value);
            if (item == null)
            {
                Alert.Show("参数错误！", String.Empty, ActiveWindow.GetHideReference());
                return;
            }

            // 显示表单数据
            ShowData(item);

            // 查看或编辑
            if (mode == PageMode.View)
            {
                ShowButtons(false, false, false);
                FormRender.SetFormEditable(this.frm, false);
            }
            else if (mode == PageMode.Edit)
            {
                ShowButtons(true, true, false);
            }
        }

        // 显示按钮
        void ShowButtons(bool close, bool save, bool saveNew)
        {
            this.btnClose.Hidden = !(close && ShowBtnClose);
            this.btnSave.Hidden = !(save && ShowBtnSave);
            this.btnSaveNew.Hidden = !(saveNew && ShowBtnSaveNew);

            // 是否隐藏工具栏
            bool show = false;
            foreach (var item in this.Toolbar.Items)
                if (item is Button)
                    show = show | (!item.Hidden);
            this.Toolbar.Hidden = !show;
        }


    }
}