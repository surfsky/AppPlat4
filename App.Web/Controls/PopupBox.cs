using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.ComponentModel;
using App.Components;
//using App.DAL;    // AppContext
using App.Utils;
using FineUIPro;
using App.DAL;

namespace App.Controls
{
    /// <summary>
    /// 弹出框的值
    /// </summary>
    public class PopupValue
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }



    /// <summary>
    /// 可弹出弹窗，并显示返回值的文本控件。
    /// 弹窗的关闭按钮事件中请这么写：
    /// PageContext.RegisterStartupScript(ActiveWindow.GetWriteBackValueReference(txt) + ActiveWindow.GetHideReference());
    /// </summary>
    public class PopupBox : TwinTriggerBox
    {
        /// <summary>是否多选（默认值为false）</summary>
        public bool Multiselect
        {
            get { return GetState("Multiselect", false); }
            set { SetState("Multiselect", value); }
        }
        /// <summary>是否显示搜索工具栏（默认值为true）</summary>
        public bool ShowSearcher
        {
            get { return GetState("ShowSearcher", true); }
            set { SetState("ShowSearcher", value); }
        }
        /// <summary>弹窗 Url 模板</summary>
        public string UrlTemplate
        {
            get { return GetState("UrlTemplate", ""); }
            set { SetState("UrlTemplate", value); }
        }
        /// <summary>弹窗标题</summary>
        public string WinTitle
        {
            get { return GetState("WinTitle", "请选择");}
            set { SetState("WinTitle", value); this.Window.Title = value; }
        }
        /// <summary>弹窗宽度</summary>
        public int WinWidth
        {
            get { return GetState("WinWidth", 800); }
            set { SetState("WinWidth", value); this.Window.Width = value; }
        }
        /// <summary>弹窗高度</summary>
        public int WinHeight
        {
            get { return GetState("WinHeight", 600); }
            set { SetState("WinHeight", value); this.Window.Height = value; }
        }


        // 值类型和文本属性
        /// <summary>数据类型。如 Article</summary>
        public Type EntityType { get; set; }
        /// <summary>值属性名称。如 ID</summary>
        public string ValueField { get; set; }
        /// <summary>文本属性名称。如 Name</summary>
        public string TextField { get; set; }


        /// <summary>控件值（保存在Hidden里面）</summary>
        public string Value
        {
            get {return _hidden.Text;}
            set 
            { 
                _hidden.Text = value;
                SetTextByEntity(value.ParseLong());
            }
        }

        /// <summary>选择项（包含文本和值）</summary>
        public PopupValue Item
        {
            get { return GetState<PopupValue>("Item", new PopupValue()); }
            set { SetState("Item", value); }
        }

        /// <summary>弹窗地址</summary>
        public string Url
        {
            get
            {
                if (PrepareUrlTemplate != null)
                    PrepareUrlTemplate();
                if (this.UrlTemplate.IsEmpty())
                    return "";

                var url = string.Format(UrlTemplate, this.Value);
                var u = new Url(url);
                if (u["md"] == null)
                    u["md"] = "select";
                u["sender"] = this.UniqueID;
                u["multi"] = this.Multiselect.ToString();
                u["search"] = this.ShowSearcher.ToString();
                u["value"] = this.Value.UrlEncode();
                return u.ToString();
            }
        }


        /// <summary>准备URL模板事件</summary>
        public event Action PrepareUrlTemplate;


        //
        // 弹窗
        //
        Window _window;
        public Window Window
        {
            get {return BuildWindow();}
        }
        protected Window BuildWindow()
        {
            if (_window == null)
            {
                _window = new FineUIPro.Window()
                {
                    ID = string.Format("Window"),
                    IsModal = true,
                    Hidden = true,
                    Target = FineUIPro.Target.Top,
                    EnableResize = true,
                    EnableMaximize = true,
                    EnableClose = true,
                    EnableIFrame = true,
                    IFrameUrl = "about:blank",
                    CloseAction = CloseAction.Hide
                };
                this.Controls.Add(this._window);
            }
            return _window;
        }

        //
        // 隐藏域（放置值）
        //
        HiddenField _hidden;
        protected HiddenField BuildHiddenField()
        {
            if (_hidden == null)
            {
                _hidden = new FineUIPro.HiddenField()
                {
                    ID = string.Format("Hidden"),
                    Hidden = true,
                };
                this.Controls.Add(_hidden);
            }
            return _hidden;
        }

        //--------------------------------------------
        // 初始化
        //--------------------------------------------
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
        }

        protected override void OnInit(EventArgs e)
        {
            if (this.Trigger1IconUrl.IsEmpty())
                this.Trigger1Icon = TriggerIcon.Clear;
            if (this.Trigger2IconUrl.IsEmpty())
                this.Trigger2Icon = TriggerIcon.List;
            base.OnInit(e);
            BuildWindow();
            BuildHiddenField();
            if (EmptyText.IsEmpty())
                EmptyText = "请选择";
            //EnableEdit = false;
            this.ShowTrigger1 = this.Text.IsNotEmpty();

            // 清空
            this.Trigger1Click += (o, a) =>
            {
                this.Text = "";
                this.Value = "";
                this.ShowTrigger1 = false;
            };

            // 弹窗
            this.Trigger2Click += (o, a) =>
            {
                if (this.Readonly)
                    return;
                string url = this.Url.ToSignUrl();  // Users.aspx?md=select&search=false&sender=Panel1$SimpleForm1$tbUser
                if (url.IsNotEmpty())
                {
                    // 弹窗、关闭窗口后回写数据
                    System.Diagnostics.Trace.WriteLine(url);
                    var eventName = nameof(this.TextChanged);
                    var saveStateScript = this.Window.GetSaveStateReference(this.ClientID, this._hidden.ClientID);
                    var showWinScript = this.Window.GetShowReference(url, this.WinTitle, this.WinWidth, this.WinHeight);
                    var postScript = this.GetPostBackEventReference(eventName);
                    var winHideExeScript = this.Window.GetHideExecuteScriptReference(postScript);
                    var winHidePostScript = this.Window.GetHidePostBackReference(eventName);

                    // Todo: 回写数据后触发事件（始终无效）
                    //F('form2_pbFiles_Window').hideExecuteScript('__doPostBack(\'form2$pbFiles\',\'TextChanged\');');
                    //F('form2_pbFiles_Window').f_property_save_state_control_client_ids =['form2_pbFiles', 'form2_pbFiles_Hidden'];
                    //F('form2_pbFiles_Window').show('/Pages/Common/Explorer.aspx?root=%2fres%2f&md=select&sender=form2$pbFiles&mutli=False&search=True&nonceStr=72382f341b&createDt=1572496629&sign=D0D4E05851444AD20A19ECD9F31AF6D9', '请选择', 800, 600);
                    var events = Reflector.GetEventSubscribers(this, eventName);
                    //var script = saveStateScript + winHidePostScript + showWinScript;
                    var script = saveStateScript + winHideExeScript + showWinScript;

                    PageContext.RegisterStartupScript(script);
                    this.ShowTrigger1 = true;
                }
            };
        }


        //--------------------------------------------
        // 存取数据
        //--------------------------------------------
        /// <summary>获取选中值</summary>
        public List<long> GetSelectValues()
        {
            return _hidden.Text.SplitLong();
        }

        /// <summary>设置选中值（若有异常会忽略）</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">数据源</param>
        /// <param name="idExpression">ID表达式</param>
        /// <param name="nameExpression">名称表达式</param>
        public void SetSelectedValue<T>(List<T> data, Expression<Func<T, object>> idExpression, Expression<Func<T, object>> nameExpression)
        {
            this.EntityType = typeof(T);
            this.ValueField = idExpression.Name;
            this.TextField = nameExpression.Name;

            var idFunc = idExpression.Compile();
            var nameFunc = nameExpression.Compile();
            var txt = new StringBuilder();
            var ids = new StringBuilder();
            foreach (var item in data)
            {
                try
                {
                    var id = idFunc.Invoke(item);
                    var name = nameFunc.Invoke(item);
                    //txt.AppendFormat("{0}({1}), ", name, id);
                    txt.AppendFormat("{0}, ", name);
                    ids.AppendFormat("{0}, ", id);
                }
                catch { continue; }
            }
            this.Text = txt.ToString().TrimEnd(',', ' ');
            this._hidden.Text = ids.ToString().TrimEnd(',', ' ');
            this.ShowTrigger1 = this.Text.IsNotEmpty();
        }

        /// <summary>设置文本（从实体获取）（id->hidden, textfield->text)</summary>
        public void SetTextByEntity(long? id)
        {
            if (id != null && this.EntityType != null && this.TextField != null)
            {
                var entity = AppContext.GetEntity(EntityType, id.Value);
                if (entity != null)
                    this.Text = entity.GetValue(this.TextField).ToText();
            }
        }


        //--------------------------------------------
        // 辅助方法
        //--------------------------------------------
        // ViewState 辅助方法 ( 注：Control.ViewState 是 protected 的，无法写成扩展方法抽取出来，只能继承使用）
        // 可以用 FState 改造
        protected void SetState(string name, object value)
        {
            ViewState[name] = value;
        }
        protected string GetState(string name, string defaultValue)
        {
            return ViewState[name] == null ? defaultValue : (string)ViewState[name];
        }
        protected T GetState<T>(string name, T defaultValue)
        {
            return ViewState[name] == null ? defaultValue : ViewState[name].To<T>();
        }

    }
}