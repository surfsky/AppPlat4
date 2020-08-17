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
    /// 编辑器信息
    /// </summary>
    public class EditorInfo
    {
        /// <summary>编辑器类型</summary>
        public EditorType Type { get; set; }

        /// <summary>编辑器值的名称</summary>
        public string Property { get; set; } = "Text";

        /// <summary>编辑器控件</summary>
        public BoxComponent Editor { get; set; }

        /// <summary>UI 配置</summary>
        public UIAttribute UI { get; set; }

        // 读写数据
        public object Get()
        {
            if (this.Property.IsEmpty())
                return null;
            return this.Editor.GetValue(this.Property);
        }
        public void Set(object value)
        {
            // 如果是枚举值，统一转化为数字后，再给控件赋值
            var type = UI.Type.GetRealType();
            if (type.IsEnum())
            {
                var o = value.ToText().Parse(type, true);
                value = (int)(o ?? 0);
            }

            // 赋值
            if (this.Property.IsNotEmpty())
                this.Editor.SetValue(this.Property, value);
        }
    }

    public class EditorInfo<T> : EditorInfo where T : BoxComponent
    {
        public EditorInfo(EditorType type, Expression<Func<T, object>> property, UIAttribute ui, T editor)
        {
            var pi = property.GetProperty();
            this.Type = type;
            this.Property = pi?.Name;
            this.Editor = editor;
            this.UI = ui;
        }
    }
}