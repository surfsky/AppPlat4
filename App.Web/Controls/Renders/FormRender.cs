using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using FineUIPro;
using App.Entities;
using App.Components;
using App.Utils;
using App.DAL;
//using App.DAL;  // AppContext

namespace App.Controls
{
    /// <summary>
    /// 表单辅助方法（自动生成表单）
    /// </summary>
    public class FormRender
    {
        //-----------------------------------------------
        // 构建表单
        //-----------------------------------------------
        /// <summary>动态生成表单（返回“字段名-控件”字典）</summary>
        public static Dictionary<string, EditorInfo> BuildForm(FormBase form, UISetting ui, PageMode? mode, bool showIdField = false, bool readOnly=false, EntityBase data=null)
        {
            var map = new Dictionary<string, EditorInfo>();
            if (ui.Groups == null)
                ui.BuildGroups();
            foreach (var key in ui.Groups.Keys)
            {
                var container = form.Items;
                // 如果有分组，用 Panel 容纳
                if (key.IsNotEmpty())
                {
                    var panel = new Panel() { Title = key, EnableCollapse=true, BodyPadding="2px", ShowBorder=false, Icon= FineUIPro.Icon.BulletBlue };
                    form.Items.Add(panel);
                    container = panel.Items;
                }
                // 遍历组内属性
                var attrs = ui.Groups[key];
                foreach (var attr in attrs)
                {
                    // 显示可读写、非虚拟、非过滤的字段
                    if (attr.Editor == EditorType.None)
                        continue;
                    if (attr.Field == null)
                        attr.Field = ui.EntityType.GetProperty(attr.Name);
                    if (attr.Field == null)
                        continue;

                    // 可视性判断
                    //var v = mode.ToString().Parse<PageMode>();
                    mode = mode ?? PageMode.View;
                    if (!attr.Mode.HasFlag(mode))
                        continue;

                    // 自动列过滤处理
                    if (attr.Editor == EditorType.Auto)
                    {
                        if (attr.Field.Name == "ID" && !showIdField)
                            continue;
                        if (!attr.Field.CanWrite || !attr.Field.CanRead)
                            continue;
                        if (attr.Field.SetMethod.IsPrivate)
                            continue;
                        if (null != attr.Field.GetAttribute<System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute>())
                            continue;
                        // 跳过复杂类型属性
                        var realType = attr.Type.GetRealType();
                        if (realType != typeof(string) && realType.IsClass)
                            continue;
                        // 跳过集合对象（以后要改为Grid）
                        if (realType.IsCollection())
                            continue;
                    }


                    // 根据 UIAttribute 创建控件
                    //（此处用edit变量，避免view和edit模式切换变更编辑器类型），或者拷贝一个
                    var edt = attr.Editor;
                    if (edt == EditorType.Auto)
                        edt = GetDefaultEditorType(attr.ValueType ?? attr.Type, readOnly);
                    var info = CreateEditor(ui, attr, edt, form, data);
                    container.Add(info.Editor);
                    map.Add(attr.Name, info);
                }
            }
            return map;
        }


        
        // 根据对象类型，获取默认编辑控件名称
        public static EditorType GetDefaultEditorType(Type type, bool readOnly)
        {
            if (type == typeof(bool?)) 
                return EditorType.Bool;

            type = type.GetRealType();
            if (readOnly)
                return EditorType.Text;
            else
            {
                if (type == typeof(String))             return EditorType.Text;
                if (type == typeof(Int16))              return EditorType.Number;
                if (type == typeof(Int32))              return EditorType.Number;
                if (type == typeof(Int64))              return EditorType.Number;
                if (type == typeof(Double))             return EditorType.Number;
                if (type == typeof(Single))             return EditorType.Number;
                if (type == typeof(Decimal))            return EditorType.Number;
                if (type == typeof(DateTime))           return EditorType.DateTime;
                if (type == typeof(bool))               return EditorType.BoolGroup;
                if (type.IsEnum)                        return EditorType.Enum;
                if (type.IsCollection())                return EditorType.WinGrid;
                if (type.IsClass)                       return EditorType.WinGrid;
                return EditorType.Text;
            }
        }


        // 创建绑定列
        /*
        <f:Label runat = "server" ID ="lblId" Label="ID" Hidden="false" />
        <f:TextBox runat = "server" ID="tbxYear"            Label="年度（YYYY）" Required="true" ShowRedStar="true" />
        <f:NumberBox runat = "server" ID="tbxCityGDP"         Label=" 全市生产总值        " DecimalPrecision="2" />
        */
        public static EditorInfo CreateEditor(UISetting ui, UIAttribute attr, EditorType edt, FormBase form, EntityBase data)
        {
            switch (edt)
            {
                case EditorType.Label:            return CreateLabel(attr, edt);
                case EditorType.Text:             return CreateTextBox(attr, edt);
                case EditorType.TextArea:         return CreateTextArea(attr, edt);
                case EditorType.Number:           return CreateNumberBox(attr, edt);
                case EditorType.Date:             return CreateDatePicker(attr, edt);
                case EditorType.Time:             return CreateTimePicker(attr, edt);
                case EditorType.DateTime:         return CreateDateTimePicker(attr, edt);

                //
                case EditorType.Markdown:         return CreateTextArea(attr, edt); 
                case EditorType.Html:             return CreateHtmlEditor(attr, edt);
                case EditorType.Image:            return CreateImageUploader(attr, ui, data, edt);
                case EditorType.Bool:             return CreateBoolDropDownList(attr, edt);
                case EditorType.BoolGroup:        return CreateBoolCheckBox(attr, edt);
                case EditorType.Enum:             return CreateEnumDropDownList(attr, edt);
                case EditorType.EnumGroup:        return CreateEnumCheckBoxList(attr, edt);
                case EditorType.Grid:             return CreateGrid(attr, data, edt);
                case EditorType.Panel:            return CreatePanel(attr, data, edt);
                case EditorType.Win:              return CreateWin(attr, data, edt);
                case EditorType.WinGrid:          return CreateWinGrid(attr, data, edt);
                case EditorType.WinList:          return CreateWinList(attr, edt);
                case EditorType.WinTree:          return CreateWinTree(attr, edt);

                //
                case EditorType.Images:           return CreateImages(attr, data, edt);
                case EditorType.Files:            return CreateFiles(attr, data, edt);
                case EditorType.GPS:              return CreateWinGPS(attr, data, edt);

                //
                default:                          return CreateTextBox(attr, edt);
            }
        }

        private static EditorInfo CreateDateTimePicker(UIAttribute attr, EditorType type)
        {
            return new EditorInfo<DateTimePicker>(type, t => t.SelectedDate, attr,
                new DateTimePicker() { 
                    ID = attr.Name, 
                    Label = attr.Title, 
                    Required = attr.Required, 
                    ShowRedStar = attr.Required, 
                    Readonly = attr.ReadOnly 
                    /*, SelectedDate = DateTime.Now*/
                });
        }

        private static EditorInfo CreateTimePicker(UIAttribute attr, EditorType type)
        {
            return new EditorInfo<TimePicker>(type, t => t.SelectedDate, attr,
                new TimePicker() { 
                    ID = attr.Name, 
                    Label = attr.Title, 
                    Required = attr.Required, 
                    ShowRedStar = attr.Required, 
                    Readonly = attr.ReadOnly 
                    /*, SelectedDate = DateTime.Now*/
                });
        }

        private static EditorInfo CreateDatePicker(UIAttribute attr, EditorType type)
        {
            return new EditorInfo<DatePicker>(type, t => t.SelectedDate, attr,
                new DatePicker() { 
                    ID = attr.Name, 
                    Label = attr.Title, 
                    Required = attr.Required, 
                    ShowRedStar = attr.Required, 
                    Readonly = attr.ReadOnly
                    /*, SelectedDate = DateTime.Today*/
                });
        }

        private static EditorInfo CreateNumberBox(UIAttribute attr, EditorType type)
        {
            return new EditorInfo<NumberBox>(type, t => t.Text, attr,
                new NumberBox() { 
                    ID = attr.Name, 
                    Label = attr.Title, 
                    Required = attr.Required, 
                    ShowRedStar = attr.Required, 
                    Readonly = attr.ReadOnly, 
                    DecimalPrecision = attr.Precision 
                });
        }

        private static EditorInfo CreateLabel(UIAttribute attr, EditorType type)
        {
            return new EditorInfo<Label>(type, t => t.Text, attr,
                new Label() { 
                    ID = attr.Name, 
                    Label = attr.Title, 
                    Hidden = false 
                });
        }

        /// <summary>文本区域。参数 Name, Height</summary>
        private static EditorInfo CreateTextArea(UIAttribute attr, EditorType type)
        {
            return new EditorInfo<TextArea>(type, t => t.Text, attr,
                new TextArea() { 
                    ID = attr.Name, 
                    Label = attr.Title, 
                    Required = attr.Required, 
                    ShowRedStar = attr.Required, 
                    Height = attr.Height < 0 ? 100 : attr.Height,
                    AutoGrowHeight = true, 
                    Readonly = attr.ReadOnly 
                });
        }

        /// <summary>创建HTML编辑控件。参数Name</summary>
        private static EditorInfo CreateHtmlEditor(UIAttribute attr, EditorType type)
        {
            return new EditorInfo<HtmlEditor>(type, t => t.Text, attr,
                new HtmlEditor() { 
                    ID = attr.Name, 
                    Label = attr.Title, 
                    ShowRedStar = attr.Required, 
                    Height = 500, 
                    Editor = Editor.UMEditor, 
                    BasePath = "~/res/third-party/umeditor/", 
                    ToolbarSet = EditorToolbarSet.Full, 
                    Readonly = attr.ReadOnly 
                });
        }

        /// <summary>创建图片上传控件。参数Name、Tag（ImageSize）</summary>
        private static EditorInfo CreateImageUploader(UIAttribute attr, UISetting ui, object data, EditorType type)
        {
            var ctrl = new ImageUploader()
            {
                ID = attr.Name,
                Label = attr.Title,
                UploadFolder = ui.EntityType.Name + "s",
                ImageSize = attr.Tag.ParseJson<Size?>(true),
                Readonly = attr.ReadOnly,
            };
            ctrl.FileUploaded += (s, e) =>
            {
                var mode = Common.PageMode;
                if (mode == PageMode.Edit)
                {
                    var val = UI.GetUrl(ctrl);
                    Reflector.SetValue(data, attr.Name, val);
                    MethodInvoker.InvokeMethod(data, "Save", null); // data.Save();
                }
            };
            return new EditorInfo<ImageUploader>(type, t => t.ImageUrl, attr, ctrl);
        }

        /// <summary>创建文本框控件。参数Name</summary>
        private static EditorInfo CreateTextBox(UIAttribute attr, EditorType type)
        {
            return new EditorInfo<TextBox>(type, t => t.Text, attr,
                new TextBox() { 
                    ID = attr.Name, 
                    Label = attr.Title, 
                    Required = attr.Required, 
                    ShowRedStar = attr.Required, 
                    Readonly = attr.ReadOnly 
                });
        }


        /// <summary>创建弹窗控件。参数 ValueType=ArticleType, TextField=Name, Query=cate={0}</summary>
        private static EditorInfo CreateWin(UIAttribute attr, EntityBase data, EditorType type)
        {
            var val = data?.GetValue(attr.Name);
            var url = string.Format(attr.UrlTemplate, val).AddModeQuery(PageMode.Select);
            var ctrl = new PopupBox()
            {
                ID = attr.Name,
                Label = attr.Title,
                Required = attr.Required,
                ShowRedStar = attr.Required,
                UrlTemplate = url,
                Readonly = attr.ReadOnly,
                EntityType = attr.ValueType,
                TextField = attr.TextField
            };
            return new EditorInfo<PopupBox>(type, t => t.Value, attr, ctrl);
        }


        /// <summary>创建弹窗控件。参数 ValueType=ArticleType, TextField=Name, Query=cate={0}</summary>
        private static EditorInfo CreateWinGrid(UIAttribute attr, EntityBase data, EditorType type)
        {
            var mode = PageMode.Select;
            var val = data?.GetValue(attr.Name);
            //
            var query = attr.QueryString.IsNotEmpty() ? string.Format(attr.QueryString, val) : "";  // QueryString
            var url = Urls.GetDatasUrl(attr.ValueType, query, mode, null);
            var ctrl = new PopupBox()
            {
                ID = attr.Name,
                Label = attr.Title,
                Required = attr.Required,
                ShowRedStar = attr.Required,
                UrlTemplate = url,
                Readonly = attr.ReadOnly,
                EntityType = attr.ValueType,
                TextField = attr.TextField
            };
            return new EditorInfo<PopupBox>(type, t => t.Value, attr, ctrl);
        }

        /// <summary>创建自动网格控件。参数ValueField, ValueType, QueryString（</summary>
        private static EditorInfo CreateGrid(UIAttribute attr, EntityBase data, EditorType type)
        {
            var mode = attr.UrlMode ?? Common.PageMode;
            var val = data?.GetValue(attr.ValueField);
            var query = attr.QueryString.IsNotEmpty() ? string.Format(attr.QueryString, val) : ""; // QueryString
            var url = Urls.GetDatasUrl(attr.ValueType, query, mode, null);
            var ctrl = new Panel()
            {
                ID = attr.Name,
                Title = attr.Title,
                ShowHeader = true,
                ShowBorder = false,
                BodyPadding = "0",
                EnableIFrame = true,
                IFrameUrl = url,
                Height = 400,
            };
            return new EditorInfo<Panel>(type, null, attr, ctrl);
        }


        /// <summary>创建面板控件。参数UrlTemplate, Name,  Height</summary>
        private static EditorInfo CreatePanel(UIAttribute attr, EntityBase data, EditorType type)
        {
            var key = data?.GetValue(attr.Name);
            var url = string.Format(attr.UrlTemplate, key);
            var ctrl = new Panel()
            {
                ID = attr.Name,
                Title = attr.Title,
                ShowHeader = true,
                ShowBorder = false,
                BodyPadding = "0",
                EnableIFrame = true,
                IFrameUrl = url,
                Height = attr.Height,
            };
            return new EditorInfo<Panel>(type, null, attr, ctrl);
        }


        /// <summary>创建图片列表控件。参数Name, Tag{cate:x, imageWidth:x}</summary>
        private static EditorInfo CreateImages(UIAttribute attr, EntityBase data, EditorType type)
        {
            var mode = attr.UrlMode ?? Common.PageMode;
            var dict = attr.Tag.ParseJObject();
            var cate = dict["cate"].ToText();
            var imageWidth = dict["imageWidth"].ToText().ParseInt();
            var key = data?.GetValue(attr.Name) as string;
            var url = Urls.GetImagesUrl(key, cate, mode, imageWidth);
            var ctrl = new Panel()
            {
                ID = attr.Name,
                Title = attr.Title,
                ShowHeader = true,
                ShowBorder = false,
                BodyPadding = "0",
                EnableIFrame = true,
                IFrameUrl = url,
                Height = 400,
            };
            return new EditorInfo<Panel>(type, null, attr, ctrl);
        }

        /// <summary>创建文件列表控件。参数Name, Tag{cate:x}</summary>
        private static EditorInfo CreateFiles(UIAttribute attr, EntityBase data, EditorType type)
        {
            var mode = attr.UrlMode ?? Common.PageMode;
            var dict = attr.Tag.ParseJObject();
            var cate = dict["cate"].ToText();
            var key = data?.GetValue(attr.Name) as string;
            var url = Urls.GetFilesUrl(key, cate, mode);
            var ctrl = new Panel()
            {
                ID = attr.Name,
                Title = attr.Title,
                ShowHeader = true,
                ShowBorder = false,
                BodyPadding = "0",
                EnableIFrame = true,
                IFrameUrl = url,
                Height = 400,
            };
            return new EditorInfo<Panel>(type, null, attr, ctrl);
        }


        /// <summary>创建GPS控件。参数 Name=addr</summary>
        private static EditorInfo CreateWinGPS(UIAttribute attr, EntityBase data, EditorType type)
        {
            var val = data?.GetValue(attr.Name);
            var ctrl = new GPSBox()
            {
                ID = attr.Name,
                Label = attr.Title,
                Required = attr.Required,
                ShowRedStar = attr.Required,
                Readonly = attr.ReadOnly,
                Text = val.ToText()
            };
            return new EditorInfo<GPSBox>(type, t => t.Text, attr, ctrl);
        }


        /// <summary>创建弹出下拉框。参数Name、ValueType、TextField 或 Values</summary>
        static EditorInfo CreateWinList(UIAttribute attr, EditorType type)
        {
            var ctrl = new DropDownList() { ID = attr.Name, Label = attr.Title, Required = attr.Required, ShowRedStar = attr.Required, Readonly = attr.ReadOnly };
            if (attr.ValueType != null)
            {
                var data = AppContext.GetEntities(attr.ValueType);
                UI.Bind(ctrl, data, attr.Name, attr.TextField);
            }
            else if (attr.Values != null)
            {
                foreach (var key in attr.Values.Keys)
                    ctrl.Items.Add(attr.Values[key].ToText(), key);
            }
            return new EditorInfo<DropDownList>(type, t => t.SelectedValue, attr, ctrl);
        }

        /// <summary>创建弹出树。参数 Name、ValueType, TextField, ValueField</summary>
        static EditorInfo CreateWinTree(UIAttribute attr, EditorType type)
        {
            var ctrl = new DropDownList() { ID = attr.Name, Label = attr.Title, Required = attr.Required, ShowRedStar = attr.Required, Readonly = attr.ReadOnly };
            if (attr.ValueType != null)
            {
                dynamic data = AppContext.GetQuery(attr.ValueType);
                var list = Enumerable.ToList(data);
                UI.BindTree(ctrl, list, attr.ValueField, attr.TextField);
            }
            return new EditorInfo<DropDownList>(type, t => t.SelectedValue, attr, ctrl);
        }

        /// <summary>创建枚举下拉框。参数 Type</summary>
        static EditorInfo CreateEnumDropDownList(UIAttribute attr, EditorType type)
        {
            var ctrl = new DropDownList() { ID = attr.Name, Label = attr.Title, Required=attr.Required, ShowRedStar = attr.Required, Readonly = attr.ReadOnly };
            if (attr.Type != null)
                UI.BindEnum(ctrl, attr.Type);
            return new EditorInfo<DropDownList>(type, t => t.SelectedValue, attr, ctrl);
        }


        /// <summary>创建枚举选择框列表。参数 Type</summary>
        static EditorInfo CreateEnumCheckBoxList(UIAttribute attr, EditorType type)
        {
            var ctrl = new CheckBoxList() { ID = attr.Name, Label = attr.Title, Required = attr.Required, ShowRedStar = attr.Required, Readonly = attr.ReadOnly };
            if (attr.Type != null)
                UI.BindEnum(ctrl, attr.Type);
            return new EditorInfo<CheckBoxList>(type, t => t.SelectedValueArray, attr, ctrl);
        }


        /// <summary>创建布尔下拉框</summary>
        static EditorInfo CreateBoolDropDownList(UIAttribute attr, EditorType type)
        {
            var ctrl = new DropDownList() { ID = attr.Name, Label = attr.Title, Required=attr.Required, ShowRedStar = attr.Required, Readonly = attr.ReadOnly };
            UI.BindBool(ctrl, "是", "否");
            return new EditorInfo<DropDownList>(type, t => t.SelectedValue, attr, ctrl);
        }

        /// <summary>创建布尔复选框</summary>
        static EditorInfo CreateBoolCheckBox(UIAttribute attr, EditorType type)
        {
            var ctrl = new FineUIPro.CheckBox() { ID = attr.Name, Label = attr.Title, ShowRedStar = attr.Required, Readonly = attr.ReadOnly };
            return new EditorInfo<CheckBox>(type, t => t.Checked, attr, ctrl);
        }

        //-----------------------------------------------
        // 显示数据
        //-----------------------------------------------
        /// <summary>显示表单</summary>
        /// <param name="map">属性名-控件字典</param>
        /// <param name="o">对象</param>
        public static void ShowFormData(Dictionary<string, EditorInfo> map, object o)
        {
            foreach (string key in map.Keys)
            {
                var editor = map[key];
                var value = o.GetValue(key);
                editor.Set(value);
            }
        }


        //-----------------------------------------------
        // 采集表单数据
        //-----------------------------------------------
        /// <summary>采集表单数据</summary>
        /// <param name="map">属性名-控件字典</param>
        /// <param name="o">数据对象</param>
        public static void CollectData(Dictionary<string, EditorInfo> map, ref EntityBase o)
        {
            foreach (var key in map.Keys)
            {
                var info = map[key];
                if (info.Property != null)
                {
                    object value = info.Get();
                    o.SetValue(key, value.ToText());
                }
            }
        }



        //-----------------------------------------------
        // 辅助方法
        //-----------------------------------------------
        /// <summary>设置表单为只读</summary>
        public static void SetFormEditable(FormBase form, bool editable = true)
        {
            ProcessItems(form.Items, t =>
            {
                if (t is FileUpload)        
                    t.Hidden = !editable;
                else 
                    t.Readonly = !editable;
            });
        }

        // 遍历处理表单的所有控件
        public static void ProcessItems(ControlBaseCollection items, Action<Field> process)
        {
            foreach (var item in items)
            {
                if (item is PanelBase)
                    ProcessItems((item as PanelBase).Items, process);
                else if (item is FormRow)
                    ProcessItems((item as FormRow).Items, process);
                else if (item is Field)
                    process(item as Field);
            }
        }

    }
}