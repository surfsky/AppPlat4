using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using FineUIPro;
using App.Utils;


namespace App.Controls
{

    /// <summary>
    /// 检索面板渲染器
    /// </summary>
    public class SearchRender
    {
        /// <summary>UI配置信息</summary>
        public UISetting UI { get; set; }

        /// <summary>参数控件对照字典</summary>
        Dictionary<string, EditorInfo> _map;


        //--------------------------------------------
        // 构造
        //--------------------------------------------
        /// <summary>构造函数</summary>
        public SearchRender(UISetting setting)
        {
            this.UI = setting;
        }

        //--------------------------------------------
        // 渲染
        //--------------------------------------------
        /// <summary>渲染</summary>
        public void Render(ControlBaseCollection container, Dictionary<string, string> dict)
        {
            _map = BuildSearcher(container, this.UI);
            SetControlValues(dict);
        }

        /// <summary>设置控件初始值</summary>
        public void SetControlValues(Dictionary<string, string> dict)
        {
            var map = this._map;
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



        //--------------------------------------------
        // 获取数据
        //--------------------------------------------
        /// <summary>获取结果</summary>
        public dynamic GetResult()
        {
            var args = GetParameters();
            var dict = UIRender.ParseQueryData();
            foreach(var key in dict.Keys)
                args[key] = dict[key];

            return MethodInvoker.InvokeMethod(null, this.UI.Method, args);
        }


        // 从控件中解析方法参数值
        private Dictionary<string, object> GetParameters()
        {
            var args = new Dictionary<string, object>();
            foreach (var key in _map.Keys)
            {
                var editor = _map[key];
                var value = editor.Editor.GetValue(editor.Property);
                args.Add(key, value.ToText());
            }
            return args;
        }


        //--------------------------------------------
        // 方案2
        //--------------------------------------------
        // 根据对象类型，获取默认编辑控件名称（工具栏上bool类型统一用下拉框）
        public static EditorType GetDefaultEditor(Type type, bool readOnly)
        {
            var editor = FormRender.GetDefaultEditorType(type, readOnly);
            if (editor == EditorType.BoolGroup)
                editor = EditorType.Bool;
            return editor;
        }

        /// <summary>动态生成搜索工具栏（返回“字段名-控件”字典）</summary>
        public static Dictionary<string, EditorInfo> BuildSearcher(ControlBaseCollection container, UISetting ui, bool readOnly = false)
        {
            var map = new Dictionary<string, EditorInfo>();
            foreach (var attr in ui.Items)
            {
                // 根据 UIAttribute 创建控件
                if (attr.Title.IsEmpty())
                    attr.Title = attr.Name;
                var edt = attr.Editor;
                if (edt == EditorType.Auto)
                {
                    edt = GetDefaultEditor(attr.ValueType ?? attr.Type, readOnly);
                }
                var info = FormRender.CreateEditor(ui, attr, edt, null, null);
                var ctrl = info.Editor;
                ctrl.Hidden = (attr.Mode == PageMode.None);

                // 搜索栏控件更为精简，需要隐藏 Label，填写 EmptyText
                if (ctrl is RealTextField )
                {
                    ctrl.Width = 120;
                    ctrl.SetValue("ShowLabel", false);  //ctrl.ShowLabel = false;
                    (ctrl as RealTextField).EmptyText = attr.Title;
                }
                else if (ctrl is DropDownList)
                {
                    ctrl.Width = 120;
                    ctrl.SetValue("ShowLabel", false);
                    (ctrl as DropDownList).EmptyText = attr.Title;
                }

                // 添加到表单并记录到字典
                container.Add(ctrl);
                map.Add(attr.Name, info);
            }
            return map;
        }
    }
}