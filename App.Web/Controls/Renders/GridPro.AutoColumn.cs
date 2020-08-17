using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Linq.Expressions;
//using EntityFramework.Extensions;
using FineUIPro;
using App.Utils;
using App.Components;
//using App.Components;  // Urls

namespace App.Controls
{
    /// <summary>
    /// 自动列
    /// </summary>
    public partial class GridPro: FineUIPro.Grid
    {
        //-------------------------------------------------
        // 自动生成绑定列
        //-------------------------------------------------
        /// <summary>根据类型，自动添加网格列</summary>
        public GridPro AddColumns(Type type)
        {
            return AddColumns(new UISetting(type));
        }

        /// <summary>根据UISetting，自动添加网格列</summary>
        public GridPro AddColumns(UISetting ui)
        {
            var mode = this.Mode;
            FineUIPro.GroupField group = null;
            foreach (var attr in ui.Items)
            {
                // 跳过字段
                if (attr.Column == ColumnType.None)
                    continue;
                if (mode != null && !attr.Mode.HasFlag(mode))
                    continue;

                // 根据 UIAttribute 创建绑定列
                var field = CreateColumn(attr, this.WinID);
                if (field == null)
                    continue;
                if (attr.Group.IsEmpty())
                    this.Columns.Add(field);
                else
                {
                    // 分组展示
                    if (group != null && group.HeaderText == attr.Group)
                        group.Columns.Add(field);
                    else
                    {
                        group = new FineUIPro.GroupField() { HeaderText = attr.Group };
                        group.Columns.Add(field);
                        this.Columns.Add(group);
                    }
                }
            }
            return this;
        }

        // 根据对象类型，获取默认编辑控件名称
        public static ColumnType GetDefaultColumnType(Type type)
        {
            if (type == null)
                return ColumnType.Text;
            type = type.GetRealType();
            if (type == typeof(bool))       return ColumnType.Bool;
            if (type == typeof(DateTime))   return ColumnType.DateTime;
            if (type.IsEnum)                return ColumnType.Enum;
            return ColumnType.Text;
        }

        // 根据对象类型，获取默认的格式化字符串
        static string GetDefaultColumnFormat(Type type, ColumnType columnType)
        {
            if (type == null)
                return "{0}";
            type = type.GetRealType();

            // 先根据列类型判断
            if (columnType == ColumnType.Date)     return "{0:yyyy-MM-dd}";
            if (columnType == ColumnType.DateTime) return "{0:yyyy-MM-dd HH:mm}";
            if (columnType == ColumnType.Time)     return "{0:HH:mm:ss}";

            // 再根据类型判断
            if (type == typeof(Double))     return "{0:0.00}";
            if (type == typeof(Single))     return "{0:0.00}";
            if (type == typeof(Decimal))    return "{0:0.00}";
            if (type == typeof(DateTime))   return "{0:yyyy-MM-dd HH:mm}";
            return "{0}";
        }

        // 根据对象类型，获取默认列宽
        public static int GetDefaultColumnWidth(Type type, ColumnType columnType)
        {
            if (type == null)
                return 120;
            type = type.GetRealType();

            // 先根据列类型判断
            if (columnType == ColumnType.Image)    return 30;
            if (columnType == ColumnType.Date)     return 100;
            if (columnType == ColumnType.DateTime) return 140;

            // 再根据类型判断
            if (type.IsNumber())                return 70;
            if (type.IsEnum)                    return 100;
            if (type == typeof(String))         return 200;
            if (type == typeof(DateTime))       return 140;
            if (type == typeof(bool))           return 80;
            return 120;
        }


        // <f:ImageField DataImageUrlField="Photo" HeaderText="照片" Hidden="false" ImageHeight="30" ImageWidth="30" MinWidth="30" />
        // <f:BoundField DataField = "ID" SortField="ID" Width="30px" HeaderText="ID" Hidden="true" />
        // 创建绑定列
        FineUIPro.BaseField CreateColumn(UIAttribute attr, string windowId)
        {
            // 补足数据
            if (attr.Type == null)
                attr.Type = typeof(string);
            if (attr.Name.IsEmpty())
                attr.Name = attr.Field?.Name;
            if (attr.Type.IsEmpty())
                attr.Type = attr.Field?.PropertyType;
            if (attr.Title.IsEmpty())
                attr.Title = attr.Field?.GetTitle();

            // 补足格式
            if (attr.Column == ColumnType.Auto)
                attr.Column = GetDefaultColumnType(attr.Type);
            if (attr.Format.IsEmpty())
                attr.Format = GetDefaultColumnFormat(attr.Type, attr.Column);
            if (attr.ColumnWidth <= 0)
                attr.ColumnWidth = GetDefaultColumnWidth(attr.Type, attr.Column);

            // 略过列表属性（以后可以改为 GridWin)
            if (!attr.Type.IsType(typeof(string)) && attr.Type.IsInterface(typeof(IEnumerable)))
                return null;

            // 略过复杂类属性（以后可以改为 FormWin）
            if (!attr.Type.IsType(typeof(string)) && attr.Type.IsClass)
                return null;

            // 创建列
            if (attr.Column == ColumnType.Enum)         return CreateEnumColumn(attr);
            if (attr.Column == ColumnType.Image)        return CreateImageColumn(attr);
            if (attr.Column == ColumnType.Bool)         return CreateCheckColumn(attr);
            if (attr.Column == ColumnType.Win)          return CreateWinColumn(attr, windowId);
            if (attr.Column == ColumnType.WinForm)      return CreateWinFormColumn(attr, windowId);
            if (attr.Column == ColumnType.WinGrid)      return CreateWinGridColumn(attr, windowId);
            return CreateBoundColumn(attr);
        }

        /// <summary>创建弹窗列。使用属性：Name, Title, Text, TextField, UrlTemplate</summary>
        private static BaseField CreateWinColumn(UIAttribute ui, string windowId)
        {
            var field = new FineUIPro.WindowField()
            {
                Title = ui.Title,       // 弹窗的标题
                HeaderText = ui.Title,  // 列的标题
                Text = ui.Text,
                DataTextField = ui.TextField,
                DataIFrameUrlFields = ui.Name,
                DataIFrameUrlFormatString = ui.UrlTemplate,
                Width = ui.ColumnWidth,
                WindowID = windowId,
                ID = "Win-" + ui.Name,
            };
            SetFieldSort(ui, field);
            return field;
        }

        /// <summary>创建自动表单弹窗列。使用属性：ValueType</summary>
        private static BaseField CreateWinFormColumn(UIAttribute ui, string windowId)
        {
            var url = Urls.GetDataFormUrl(ui.ValueType, -1, PageMode.View, null);
            url = url.SetQueryString("id", "{0}");
            var field = new FineUIPro.WindowField()
            {
                Title = ui.Title,       // 弹窗的标题
                HeaderText = ui.Title,  // 列的标题
                Text = ui.Text,
                DataTextField = ui.TextField,
                DataIFrameUrlFields = ui.Name,
                DataIFrameUrlFormatString = url,
                Width = ui.ColumnWidth,
                WindowID = windowId,
            };
            SetFieldSort(ui, field);
            return field;
        }

        /// <summary>创建网格弹窗列</summary>
        private static BaseField CreateWinGridColumn(UIAttribute ui, string windowId)
        {
            var field = new FineUIPro.WindowField()
            {
                Title = ui.Title,       // 弹窗的标题
                HeaderText = ui.Title,  // 列的标题
                Width = ui.ColumnWidth,
                DataIFrameUrlFormatString = ui.UrlTemplate,
                WindowID = windowId,
                Text = ui.Title,
                DataIFrameUrlFields = ui.Name,
                ID = "Grid-" + ui.Name,
            };
            SetFieldSort(ui, field);
            // win.Width = ui.WinSize.Width;
            // win.Height = ui.WinSize.Height;
            return field;
        }



        /// <summary>添加枚举列</summary>
        private BaseField CreateEnumColumn(UIAttribute ui)
        {
            var field = new FineUIPro.BoundField()
            {
                DataField = ui.Name,
                Width = ui.ColumnWidth,
                HeaderText = ui.Title,
                ColumnID = "Enum-" + ui.Name,
            };
            SetFieldSort(ui, field);

            // 设置标题、排序、列ID
            FixColumn(field, ui.Name, ui.Title, true);
            var func = new Func<object, string>(t =>
            {
                var p = t.GetValue(ui.Name);
                return p.GetTitle();
            });
            this._funcs.Add(field.ColumnID, func);
            return field;
        }

        /*
        /// <summary>创建网格弹窗列（未启用）</summary>
        private static BaseField CreateGridColumn(UIAttribute ui, string windowId, ITypeHandler handler)
        {
            string url = handler.GridUrl;
            var field = new FineUIPro.WindowField() 
            {
                Title = ui.Title,       // 弹窗的标题
                HeaderText = ui.Title,  // 列的标题
                Width = ui.ColumnWidth, 
                DataIFrameUrlFormatString = url, 
                WindowID = windowId, 
                Text = ui.Name, 
            };
            SetFieldSort(ui, field);
            return field;
        }
        */

        /// <summary>创建图像列</summary>
        static BaseField CreateImageColumn(UIAttribute ui)
        {
            return new ThrumbnailField()
            {
                ColumnID = "Image-" + ui.Name,
                HeaderText = ui.Title,
                Width = ui.ColumnWidth,
                DataImageUrlField = ui.Name,
                ImageWidth = ui.ColumnWidth - 10
            };
        }

        /// <summary>创建普通绑定列</summary>
        static BoundField CreateBoundColumn(UIAttribute ui)
        {
            var field = new FineUIPro.BoundField()
            {
                DataField = ui.Name,
                HeaderText = ui.Title,
                DataFormatString = ui.Format,
                Width = ui.ColumnWidth,
                DataSimulateTreeLevelField = ui.Tree ? "TreeLevel" : "",
            };
            SetFieldSort(ui, field);
            return field;
        }

        /// <summary>创建布尔值绑定列</summary>
        static CheckBoxField CreateCheckColumn(UIAttribute ui)
        {
            var field = new FineUIPro.CheckBoxField()
            {
                DataField = ui.Name,
                HeaderText = ui.Title,
                Width = ui.ColumnWidth
            };
            SetFieldSort(ui, field);
            return field;
        }

        // 设置列的排序
        private static void SetFieldSort(UIAttribute ui, GridColumn field)
        {
            if (!ui.Name.Contains("."))
                field.SortField = ui.Name;
        }
    }
}