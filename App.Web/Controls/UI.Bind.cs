using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Linq.Expressions;
using FineUIPro;
using App.Utils;
using App.Entities;

namespace App.Controls
{
    /// <summary>
    /// UI 操作 (FineUI) 辅助类
    /// </summary>
    public partial class UI
    {
        //------------------------------------------------------
        // Bind DropDownList
        //------------------------------------------------------
        // 绑定到下拉列表（启用模拟树功能和不可选择项功能）
        public static void BindTree<T>(
            DropDownList ddl, List<T> data,
            Expression<Func<T, object>> valueField,
            Expression<Func<T, object>> textField,
            string title = "--请选择--", long? selectedId = null, long? disableId = null)
            where T : class, ITree<T>
        {
            BindTree(ddl, data, valueField.GetName(), textField.GetName(), title, selectedId, disableId);
        }

        // 绑定到下拉列表（启用模拟树功能和不可选择项功能）
        public static void BindTree<T>(
            DropDownList ddl, List<T> data,
            string valueField, string textField,
            string title = "--请选择--", long? selectedId = null, long? disableId = null)
            where T : class, ITree<T>
        {
            if (data == null)
                return;
            data = data.CloneTree();
            var tree = DisableTreeItem(data, disableId);
            ddl.EmptyText = title;
            ddl.AutoSelectFirstItem = false;
            ddl.EnableEdit = true;
            ddl.ForceSelection = true;
            ddl.EnableSimulateTree = true;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataSimulateTreeLevelField = nameof(ITree.TreeLevel);  // "TreeLevel";
            ddl.DataEnableSelectField = nameof(ITree.Enabled);         // "Enabled";
            ddl.DataSource = tree;
            ddl.DataBind();

            if (selectedId != null)
                ddl.SelectedValue = selectedId.ToString();
        }


        /// <summary>设置树不可选节点</summary>
        static List<T> DisableTreeItem<T>(List<T> source, long? disableId)
            where T : ITree
        {
            if (disableId == null)
                return source;

            // 不可选择设定
            bool startChildNode = false;
            int startTreeLevel = 0;
            foreach (T node in source)
            {
                node.Enabled = true;

                // 指定节点不可选
                if (node.ID == disableId.Value)
                {
                    node.Enabled = false;
                    startTreeLevel = node.TreeLevel;
                    startChildNode = true;
                    continue;
                }
                // 子节点不可选（数据已经预先整理好。子节点数据跟在父节点后面，且TreeLevel比父节点大）
                if (startChildNode)
                {
                    if (node.TreeLevel > startTreeLevel)
                        node.Enabled = false;
                    else
                        startChildNode = false;
                }
            }
            return source;
        }

        /// <summary>设置树不可选节点（未测试）</summary>
        public static void DisableTreeNode(this DropDownList ddl, long? disableId)
        {
            if (disableId == null)
                return;

            // 不可选择设定
            bool startChildNode = false;
            int startTreeLevel = 0;
            foreach (var item in ddl.Items)
            {
                // 指定节点不可选
                var id = item.Value.ParseLong();
                if (id == disableId.Value)
                {
                    item.EnableSelect = false;
                    startTreeLevel = item.SimulateTreeLevel;
                    startChildNode = true;
                    continue;
                }
                // 子节点不可选（数据已经预先整理好。子节点数据跟在父节点后面，且TreeLevel比父节点大）
                if (startChildNode)
                {
                    if (item.SimulateTreeLevel > startTreeLevel)
                        item.EnableSelect = false;
                    else
                        startChildNode = false;
                }
            }
        }

        /// <summary>绑定下拉框</summary>
        public static void Bind<T, TText, TValue>(
            DropDownList ddl, IEnumerable<T> data,
            Expression<Func<T, TValue>> valueField,
            Expression<Func<T, TText>> textField,
            string title = "--请选择--", long? selectedId = null)
        {
            Bind(ddl, data, valueField.GetName(), textField.GetName(), title, selectedId);
        }

        /// <summary>绑定下拉框</summary>
        public static void Bind(
            DropDownList ddl, object data,
            string valueField,
            string textField,
            string title = "--请选择--", long? selectedId = null)
        {
            ddl.EmptyText = title;
            ddl.AutoSelectFirstItem = false;
            ddl.EnableEdit = true;
            ddl.ForceSelection = true;
            ddl.DataSource = data;
            ddl.DataValueField = valueField;
            ddl.DataTextField = textField;
            ddl.DataBind();
            if (selectedId != null)
                ddl.SelectedValue = selectedId.ToString();
        }


        /// <summary>绑定下拉框（多选）</summary>
        public static void BindMulti<T, TText, TValue>(
            DropDownList ddl, IEnumerable data,
            Expression<Func<T, TValue>> valueField,
            Expression<Func<T, TText>> textField,
            string title = "--请选择--", string[] selectedValues = null)
        {
            BindMulti(ddl, data, valueField.GetName(), textField.GetName(), title, selectedValues);
        }
        /// <summary>绑定下拉框（多选）</summary>
        public static void BindMulti(
            DropDownList ddl, IEnumerable data,
            string valueField,
            string textField,
            string title = "--请选择--", string[] selectedValues = null)
        {
            ddl.EmptyText = title;
            ddl.AutoSelectFirstItem = false;
            ddl.EnableEdit = true;
            ddl.ForceSelection = true;
            ddl.EnableMultiSelect = true;
            ddl.DataSource = data;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();
            if (selectedValues != null)
                ddl.SelectedValueArray = selectedValues;
        }

        // 绑定下拉框（bool类型）
        public static void BindBool(DropDownList ddl, string trueText, string falseText, string title = "--请选择--", bool? value = null)
        {
            ddl.EmptyText = title;
            ddl.AutoSelectFirstItem = false;
            ddl.EnableEdit = true;
            ddl.ForceSelection = true;
            ddl.Items.Add(new ListItem(trueText, true.ToString()));    // True
            ddl.Items.Add(new ListItem(falseText, false.ToString()));  // False
            if (value != null)
                ddl.SelectedValue = value.ToText();
        }


       
        // 绑定下拉框（枚举类型）
        public static void BindEnum(DropDownList ddl, Type enumType, string title = "--请选择--", string enumGroup = "", long? selectedId = null)
        {
            enumType = enumType.GetRealType();
            var items = enumType.GetEnumInfos();
            if (enumGroup.IsEmpty())
            {
                Bind(ddl, items, t => t.ID, t => t.Title, title, selectedId);
            }
            else
            {
                enumGroup = enumGroup?.ToLower();
                ddl.Items.Clear();
                foreach (var item in items)
                    if (item.Group?.ToLower() == enumGroup)
                        ddl.Items.Add(item.Title, item.ID.ToString());

                ddl.EmptyText = title;
                ddl.AutoSelectFirstItem = false;
                ddl.ForceSelection = true;
                ddl.EnableEdit = true;
                if (ddl.Items.Count == 1)
                {
                    ddl.EnableEdit = false;
                    ddl.SelectedIndex = 0;
                }
            }
        }

        //------------------------------------------------------
        // Bind CheckButtonList
        //------------------------------------------------------
        /// <summary>绑定到复选框列表</summary>
        public static void Bind<T, TText, TValue>(
            CheckBoxList cbl, IEnumerable<T> data,
            Expression<Func<T, TValue>> valueField,
            Expression<Func<T, TText>> textField,
            long? selectedId = null)
        {
            Bind(cbl, data, valueField.GetName(), textField.GetName(), selectedId);
        }
        /// <summary>绑定到复选框列表</summary>
        public static void Bind(
            CheckBoxList cbl, IEnumerable data,
            string valueField,
            string textField,
            long? selectedId = null)
        {
            cbl.DataSource = data;
            cbl.DataTextField = textField;
            cbl.DataValueField = valueField;
            cbl.DataBind();
            if (selectedId != null)
                cbl.SelectedValueArray = new string[] { selectedId.ToString() };
        }

        /// <summary>绑定到单选框列表</summary>
        public static void BindEnum(CheckBoxList cbl, Type enumType, int? selectedId = null)
        {
            enumType = enumType.GetRealType();
            var items = enumType.GetEnumInfos();
            Bind(cbl, items, t => t.ID, t => t.Title, selectedId);
        }




        //------------------------------------------------------
        // Bind RadioButtonList
        //------------------------------------------------------
        /// <summary>绑定到单选框列表（默认设置第一个值选中）</summary>
        public static void Bind<T, TText, TValue>(
            RadioButtonList rbl, IEnumerable<T> data,
            Expression<Func<T, TValue>> valueField,
            Expression<Func<T, TText>> textField,
            long? selectedId = null)
        {
            Bind(rbl, data, valueField.GetName(), textField.GetName(), selectedId);
        }
        /// <summary>绑定到单选框列表（默认设置第一个值选中）</summary>
        public static void Bind(
            RadioButtonList rbl, IEnumerable data,
            string valueField,
            string textField,
            long? selectedId = null)
        {
            rbl.DataSource = data;
            rbl.DataTextField = textField;
            rbl.DataValueField = valueField;
            rbl.DataBind();
            if (selectedId != null)
                rbl.SelectedValue = selectedId.ToString();
            else
                rbl.SelectedIndex = 0;
        }

        /// <summary>绑定到单选框列表（枚举）</summary>
        public static void BindEnum(RadioButtonList rbl, Type enumType, int? selectedId = null)
        {
            enumType = enumType.GetRealType();
            var items = enumType.GetEnumInfos();
            Bind(rbl, items, t => t.ID, t => t.Title, selectedId);
        }

        /// <summary>绑定到单选框列表（布尔类型）</summary>
        public static void BindBool(RadioButtonList rbl, string trueText, string falseText, bool? value = null)
        {
            rbl.Items.Clear();
            rbl.Items.Add(new RadioItem(trueText, "true"));
            rbl.Items.Add(new RadioItem(falseText, "false"));
            if (value != null)
                rbl.SelectedValue = value.ToString();
        }


    }
}