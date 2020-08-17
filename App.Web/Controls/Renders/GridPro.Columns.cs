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
using App.Utils;
using FineUIPro;

namespace App.Controls
{
    /// <summary>
    /// 各种列
    /// </summary>
    public partial class GridPro: FineUIPro.Grid
    {
        const int ICONWIDTH = 38;

        //-------------------------------------------------
        // 控制列
        //-------------------------------------------------
        /// <summary>设置控制列</summary>
        public GridPro SetControlColumns(bool numberField, bool viewField, bool editField, bool deleteField, bool idField)
        {
            this.ShowNumberField = numberField;
            this.ShowViewField = viewField;
            this.ShowEditField = editField;
            this.ShowDeleteField = deleteField;
            this.ShowIDField = idField;
            return this;
        }

        /*
        <Columns>
            <f:ImageField DataImageUrlField="Photo" HeaderText="照片" Hidden="false" ImageHeight="30" ImageWidth="30" MinWidth="30" />
            <f:RowNumberField Width = "30px" EnablePagingNumber="true" />
            <f:WindowField ColumnID = "Edit" TextAlign="Center" Icon="Info" ToolTip="查看" Width="30px" 
                WindowID="Window1" Title="查看" DataIFrameUrlFields="ID" DataIFrameUrlFormatString="UserView.aspx?id={0}"
                />
            <f:WindowField ColumnID = "Edit" TextAlign="Center" Icon="Pencil" ToolTip="编辑" Width="30px" 
                WindowID="Window1" Title="编辑" DataIFrameUrlFields="ID" DataIFrameUrlFormatString="UserEdit.aspx?id={0}"
                />
            <f:LinkButtonField ColumnID = "Delete" TextAlign="Center" Icon="Delete" ToolTip="删除" Width="30px" 
                ConfirmText="确定删除此记录？" ConfirmTarget="Top" CommandName="Delete" 
                />
            <f:BoundField DataField = "ID" SortField="ID" Width="30px" HeaderText="ID" Hidden="true" />
        </Columns>
        */
        /// <summary>添加控制列（id、删除、查看等）</summary>
        public GridPro AddControlColumns()
        {
            if (ShowIDField)
                this.Columns.Insert(0, new FineUIPro.BoundField()
                {
                    DataField = "ID",
                    SortField = "ID",
                    HeaderText = "ID",
                    Width = 50
                });
            if (ShowDeleteField && AllowDelete)
                this.Columns.Insert(0, new FineUIPro.LinkButtonField()
                {
                    ColumnID = "Delete",
                    TextAlign = FineUIPro.TextAlign.Center,
                    Icon = FineUIPro.Icon.Delete,
                    ToolTip = this.DeleteText,
                    ConfirmText = "确定删除此记录?",
                    ConfirmTarget = FineUIPro.Target.Top,
                    CommandName = "Delete",
                    Width = ICONWIDTH
                });
            if (ShowEditField && AllowEdit)
                this.Columns.Insert(0, new FineUIPro.WindowField()
                {
                    ColumnID = "Edit",
                    TextAlign = FineUIPro.TextAlign.Center,
                    Icon = FineUIPro.Icon.Pencil,
                    ToolTip = "编辑",
                    Width = ICONWIDTH,
                    Title = "编辑",
                    WindowID = this.WinID,
                    DataIFrameUrlFields = "ID",
                    DataIFrameUrlFormatString = this.EditUrlTmpl
                });

            // 如果显示了编辑列，查看列就不显示了
            if (ShowEditField && AllowEdit)
                this.ShowViewField = false;
            if (ShowViewField)
                this.Columns.Insert(0, new FineUIPro.WindowField()
                {
                    ColumnID = "View",
                    TextAlign = FineUIPro.TextAlign.Center,
                    Icon = FineUIPro.Icon.Information,
                    ToolTip = "查看",
                    Width = ICONWIDTH,
                    Title = "查看",
                    WindowID = this.WinID,
                    DataIFrameUrlFields = "ID",
                    DataIFrameUrlFormatString = this.ViewUrlTmpl
                });
            if (ShowNumberField)
                this.Columns.Insert(0, new FineUIPro.RowNumberField()
                {
                    Width = 30,
                    EnablePagingNumber = true
                });
            return this;
        }


        /// <summary>设置最后的列展开占满剩余空间</summary>
        public GridPro SetLastColumnExpand()
        {
            this.Columns.Last().ExpandUnusedSpace = true;
            return this;
        }


        //-------------------------------------------------
        // 手工列
        //-------------------------------------------------
        /// <summary>修正列ColumnID、标题、排序等信息</summary>
        private static void FixColumn(GridColumn field, string fieldName, string fieldDesc, bool canSort)
        {
            // ColumnID 
            if (field.ColumnID.IsEmpty())
                field.ColumnID = fieldName.Replace('.', '_');

            // 修复标题
            if (field.HeaderText.IsEmpty())
                field.HeaderText = fieldDesc;

            // 设置排序（组合字段和只读字段不允许排序）
            if (field.SortField.IsEmpty())
                if (!fieldName.Contains(".") && canSort)
                    field.SortField = fieldName;
        }


        /// <summary>添加绑定列</summary>
        public GridPro AddColumn<T>(
            Expression<Func<T, object>> textField, int width, string title = "",
            string formatString = "{0}",
            Expression<Func<T, object>> sortField = null,
            bool? isTree = null
            )
        {
            //return this.AddColumn(textField, width, title, formatString, sortField, treeField);
            var prop = textField.GetProperty();
            var name = textField.GetName();
            var desc = prop.GetTitle();
            var canSort = prop.CanWrite;
            var sort = sortField.GetName();

            var field = new FineUIPro.BoundField()
            {
                DataField = name,
                Width = width,
                DataFormatString = formatString,
                HeaderText = title,
            };

            // 设置标题、排序、列ID
            if (sort.IsNotEmpty())
                field.SortField = sort;
            if (isTree == true)
            {
                // 模拟树列，无法实现点击展开能力
                field.DataSimulateTreeLevelField = "TreeLevel";
                //  企业版 FineUI 才能使用
                //this.EnableTree = true;
                //this.DataParentIDField = ...;
                //this.ExpandAllTreeNodes = true;
            }
            FixColumn(field, name, desc, canSort);
            this.Columns.Add(field);
            return this;
        }

        /// <summary>添加枚举列</summary>
        public GridPro AddEnumColumn<T>(Expression<Func<T, object>> enumField, int width, string title = "")
        {
            var prop = enumField.GetProperty();
            var name = enumField.GetName();
            if (title.IsEmpty())
                title = prop.GetTitle();
            var canSort = prop.CanWrite;
            var field = CreateEnumColumn(new UIAttribute() { Name = name, Title = title, ColumnWidth = width });
            this.Columns.Add(field);
            return this;
        }



        /// <summary>添加布尔类型列</summary>
        public GridPro AddBoolColumn<T>(Expression<Func<T, object>> boolField, int width, string title = "", string trueText = "是", string falseText = "否")
        {
            var prop = boolField.GetProperty();
            var name = boolField.GetName();
            var desc = prop.GetTitle();
            var canSort = prop.CanWrite;
            var sort = name;

            var field = new FineUIPro.BoundField()
            {
                DataField = name,
                Width = width,
                HeaderText = title,
                SortField = sort,
                ColumnID = "Bool-" + name,
            };

            // 设置标题、排序、列ID
            FixColumn(field, name, desc, canSort);
            this.Columns.Add(field);
            var func = new Func<object, string>(t => {
                var p = t.GetValue(name);
                if ((bool?)p == true) return trueText;
                else return falseText;
            });
            this._funcs.Add(field.ColumnID, func);
            return this;
        }



        /// <summary>添加选择列</summary>
        public GridPro AddCheckColumn<T>(Expression<Func<T, object>> dataField, int width, string title = "")
        {
            var name = dataField.GetName();
            var prop = dataField.GetProperty();
            var desc = prop.GetTitle();
            var canSort = prop.CanWrite;
            var field = new CheckBoxField()
            {
                HeaderText = title,
                DataField = name,
                Width = width,
                RenderAsStaticField = true,
                ColumnID = "Check-" + name,
            };
            FixColumn(field, name, desc, canSort);
            this.Columns.Add(field);
            return this;
        }


        /// <summary>添加窗口列</summary>
        public GridPro AddWindowColumn<T>(
            Expression<Func<T, object>> idField, Expression<Func<T, object>> textField,
            string urlTmpl, int width, string title = "", string text = "", Icon? icon = null,
            string columnId = ""
            )
        {
            var idName = idField.GetName();
            var textName = textField.GetName();
            var prop = idField.GetProperty();
            var desc = prop.GetTitle();
            var canSort = prop.CanWrite;
            if (title.IsEmpty())
                title = prop.GetTitle().TrimEnd("ID");   //
            if (columnId.IsEmpty())
                columnId = idName;

            var field = new FineUIPro.WindowField()
            {
                HeaderText = title,
                Title = title,
                Width = width,
                TextAlign = FineUIPro.TextAlign.Center,
                WindowID = this.WinID,
                DataIFrameUrlFields = idName,
                DataIFrameUrlFormatString = urlTmpl,
                ToolTip = title,
                ColumnID = "Win-" + columnId
            };

            // 文本、绑定文本、图标
            if (text.IsNotEmpty())
                field.Text = text;
            else
                field.DataTextField = textName;
            if (icon != null)
                field.Icon = icon.Value;

            FixColumn(field, idName, desc, canSort);
            this.Columns.Add(field);
            return this;
        }

        /// <summary>添加超链接列</summary>
        public GridPro AddLinkColumn<T>(Expression<Func<T, object>> idField, Expression<Func<T, object>> textField,
            string urlTmpl, int width,
            string title = "", string text = "", Icon? icon=null, string target = "_blank")
        {
            var textName = textField.GetName();
            var idName = idField.GetName();
            var prop = idField.GetProperty();
            var desc = prop.GetTitle();
            var canSort = prop.CanWrite;
            var field = new HyperLinkField()
            {
                ColumnID = "Link-" + idName,
                HeaderText = title,
                Width = width,
                DataNavigateUrlFields = new string[] { idName },
                DataNavigateUrlFormatString = urlTmpl,
                UrlEncode = true,
                Target = target
            };
            if (icon != null)
                text = string.Format(@"<img src='{0}'/>", UI.GetIconUrl(icon.Value));
            if (text.IsNotEmpty())
                field.Text = text;
            else
                field.DataTextField = textName;
            FixColumn(field, idName, desc, canSort);
            this.Columns.Add(field);
            return this;
        }

        /// <summary>添加缩略图列</summary>
        public GridPro AddThrumbnailColumn<T>(Expression<Func<T, object>> urlField, int width, string title = "")
        {
            var name = urlField.GetName();
            var prop = urlField.GetProperty();
            var desc = prop.GetTitle();
            var canSort = prop.CanWrite;
            var field = new ThrumbnailField()
            {
                ColumnID = "Thrumbnail-" + name,
                HeaderText = title,
                Width = width,
                DataImageUrlField = name,
                ImageWidth = width- 10
            };
            FixColumn(field, name, desc, canSort);
            this.Columns.Add(field);
            return this;
        }


        /// <summary>添加图像列</summary>
        public GridPro AddImageColumn<T>(Expression<Func<T, object>> urlField, int width = 40, string title = "", string urlTmpl = "{0}", int? imageWidth = null, int? imageHeight = null, Expression<Func<T, object>> tooltipField = null)
        {
            var name = urlField.GetName();
            var prop = urlField.GetProperty();
            var desc = prop.GetTitle();
            var canSort = prop.CanWrite;
            var field = new FineUIPro.ImageField()
            {
                ColumnID = "Image-" + name,
                HeaderText = title,
                Width = width,
                DataImageUrlField = name,
                DataImageUrlFormatString = urlTmpl
            };
            if (tooltipField != null)   field.DataToolTipField = tooltipField.GetName();
            if (imageWidth != null)     field.ImageWidth = imageWidth.Value;
            if (imageHeight != null)    field.ImageHeight = imageHeight.Value;
            FixColumn(field, name, desc, canSort);
            this.Columns.Add(field);
            return this;
        }

       

        /// <summary>添加按钮列</summary>
        /// <param name="action">点击后处理方法。参数是当前行的值数组。</param>
        public GridPro AddButtonColumn(Icon icon, Action<List<string>> action, string commandName, string confirmText = "", string tooltip = "", int width = ICONWIDTH, string title = "")
        {
            var field = new LinkButtonField()
            {
                TextAlign = FineUIPro.TextAlign.Center,
                Icon = icon,
                ToolTip = tooltip,
                ConfirmText = confirmText,
                ConfirmTarget = FineUIPro.Target.Top,
                Width = width,
                CommandName = commandName,
                ColumnID = "Button-" + commandName
            };
            this.Columns.Add(field);

            // 点击事件
            if (action != null)
            {
                this.RowCommand += (s, e) =>
                {
                    if (e.CommandName == commandName)
                    {
                        var values = this.GetSelectedValue();
                        action(values);
                    }
                };
            }
            return this;
        }


        /// <summary>添加回调方法列（此类型列无法参与排序）</summary>
        /// <param name="func">内容显示方法。如t => t.Sex.GetDescription()</param>
        public GridPro AddFuncColumn<T>(Func<object, string> func, Expression<Func<T, object>> sortField, int width, string title)
        {
            var name = sortField.GetName();
            var field = new FineUIPro.BoundField()
            {
                ColumnID = "Func-" + name,
                HeaderText = title,
                DataField = name,
                Width = width,
                SortField = name
            };
            if (sortField != null)
                field.SortField = name;
            this.Columns.Add(field);
            this._funcs.Add(field.ColumnID, func);
            return this;
        }


    }
}