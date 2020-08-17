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
using System.Text.RegularExpressions;
using FineUIPro;
//using App.DAL;  // Power
using App.Utils;
using App.DAL;

namespace App.Controls
{
    /// <summary>
    /// 网格操作相关（可考虑迁移到 GridHelper 去）
    /// </summary>
    public static partial class UI
    {
        //------------------------------------------------------
        // 网格相关
        //------------------------------------------------------
        /// <summary>设置网格单元格文本（在RowBindEvent 时间中使用）</summary>
        public static void SetGridCellText(this Grid grid, string columnId, string text, GridRowEventArgs e)
        {
            //var grid = e.Row.Grid;
            var column = grid.FindColumn(columnId) as GridColumn;
            if (column != null)
                e.Values[column.ColumnIndex] = text;
        }

        /// <summary>设置网格 Window 列的 Url 值（在RowBindEvent 事件中使用）</summary>
        public static void SetGridWinCellUrl(this Grid grid, string columnId, string url, GridRowEventArgs e)
        {
            // <a href="javascript:;" onclick="javascript:F(&#39;Panel1_Grid1_Window1&#39;).show(&#39;/Pages/Base/%2fres%2fabout.mp4&#39;,&#39;信息&#39;);" data-qtip="信息"><img class="f-grid-cell-icon" src="/res/icon/information.png"/></a>
            url = Asp.ResolveUrl(url);
            var column = grid.FindColumn(columnId) as GridColumn;
            var text = e.Values[column.ColumnIndex].ToString();
            text = text.ReplaceRegex(@"show\(.*,", (m) => $"show('{url}',");
            e.Values[column.ColumnIndex] = text;
        }

        // 根据权限设置网格列状态
        // Common.SetGridColumnByPower(Grid1,  "editField", "CorDeptEdit");
        public static void SetGridColumnByPower(this Grid grid, string columnID, Powers power)
        {
            if (!Common.CheckPower(power))
            {
                BaseField field = grid.FindColumn(columnID) as BaseField;
                field.Hidden = true;     // 整个列都隐藏
                //field.ToolTip = Common.CHECK_POWER_FAIL_ACTION_MESSAGE;
                //field.Enabled = false; // 按钮不能点
                //field.Visible = false; // BUG: 加上该语句页面就无法显示
            }
        }

        /// <summary>设置网格列标题</summary>
        public static void SetGridColumnVisible(this Grid grid, string columnID, bool visible)
        {
            var field = grid.FindColumn(columnID) as GridColumn;
            if (field != null)
                field.Hidden = !visible;
        }

        /// <summary> 给网格批量操作按钮增加客户端确认框</summary>
        public static void SetGridBatchActionConfirm(
            this Grid grid, FineUIPro.Button btn,
            string actionName = "删除",
            string confirmTemplate = "确定要{0}选中的<span><script>{1}</script></span>项记录吗？",
            string noSelectionMessage = "请至少应该选择一条记录！"
            )
        {
            btn.OnClientClick = grid.GetNoSelectionAlertInParentReference(noSelectionMessage);
            btn.ConfirmText = String.Format(confirmTemplate, actionName, grid.GetSelectedCountReference());
            btn.ConfirmTarget = FineUIPro.Target.Top;
        }
        public static void SetGridBatchActionConfirm(
            this Grid grid, FineUIPro.MenuButton btn,
            string actionName = "删除",
            string confirmTemplate = "确定要{0}选中的<span><script>{1}</script></span>项记录吗？",
            string noSelectionMessage = "请至少应该选择一条记录！"
            )
        {
            btn.OnClientClick = grid.GetNoSelectionAlertInParentReference(noSelectionMessage);
            btn.ConfirmText = String.Format(confirmTemplate, actionName, grid.GetSelectedCountReference());
            btn.ConfirmTarget = FineUIPro.Target.Top;
        }

    }
}