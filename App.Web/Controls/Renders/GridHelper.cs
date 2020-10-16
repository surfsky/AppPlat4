using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Core.Objects;
using System.Data;
using System.Collections;
using System.Linq.Expressions;
using FineUIPro;
using App.Utils;


namespace App.Controls
{
    /// <summary>
    /// FineUI Grid 辅助操作类（自动生成列的代码已经迁移到 GridPro）
    /// </summary>
    public static class GridHelper
    {
        //-------------------------------------------------
        // 绑定网格
        //-------------------------------------------------
        // 排序分页后显示
        public static void Bind<T>(this Grid grid, IQueryable<T> query)
        {
            IQueryable<T> q = SortAndPage(query, grid);
            grid.DataSource = q;
            grid.DataBind();
        }


        //-----------------------------------------
        // 网格分页和排序
        //-----------------------------------------
        /// <summary>使用Grid 的排序和分页参数对查询结果进行过滤</summary>
        public static IQueryable<T> SortAndPage<T>(this IQueryable<T> query, Grid grid)
        {
            grid.RecordCount = query.Count();
            grid.PageIndex = MathHelper.Limit(grid.PageIndex, 0, grid.PageCount - 1);
            if (grid.SortField.IsEmpty())
                grid.SortField = "ID";
            return query.SortAndPage(grid.SortField, grid.SortDirection, grid.PageIndex, grid.PageSize);
        }

        /// <summary>使用Grid 的排序参数对查询结果进行排序</summary>
        public static IQueryable<T> Sort<T>(this IQueryable<T> query, Grid grid)
        {
            if (grid.SortField.IsEmpty())
                grid.SortField = "ID";
            return query.Sort(grid.SortField, grid.SortDirection);
        }

        /// <summary>排序及分页</summary>
        public static IQueryable<T> SortAndPage<T>(this IQueryable<T> query, string sortField, string sortDirection, int pageIndex, int pageSize)
        {
            #pragma warning disable 0618
            return query.Sort(sortField, sortDirection).Page(pageIndex, pageSize);
            #pragma warning restore 0618
        }

        /// <summary>排序及分页（未完成）</summary>
        public static IQueryable<T> SortAndPage<T>(this IQueryable<T> query, List<SortInfo> sorts, int pageIndex, int pageSize)
        {
            #pragma warning disable 0618
            for (int i=0; i<sorts.Count; i++)
            {
                var sort = sorts[i];
                if (i == 0)
                    query = query.Sort(sort.Field, sort.Direction);
                //else
                //    query = query.ThenSort(sort.Field, sort.Direction);
            }
            return query.Page(pageIndex, pageSize);
            #pragma warning restore 0618
        }

        /// <summary>排序参数</summary>
        public class SortInfo
        {
            public string Field;
            public bool Ascent = true;
            public string Direction => Ascent ? "ASC" : "DSC";
        }


        //-----------------------------------------
        // ID 相关
        //-----------------------------------------
        /// <summary>获取选择行主键 ID</summary>
        public static long GetSelectedId(this Grid grid, int idFieldIndex=0)
        {
            long id = -1;
            int rowIndex = grid.SelectedRowIndex;
            if (rowIndex >= 0)
                id = Convert.ToInt64(grid.DataKeys[rowIndex][idFieldIndex]);
            return id;
        }

        /// <summary>获取选择行键值数组</summary>
        public static List<string> GetSelectedValue(this Grid grid)
        {
            int rowIndex = grid.SelectedRowIndex;
            if (rowIndex >= 0)
                return grid.DataKeys[rowIndex].CastString();
            return null;
        }

        /// <summary>获取选择行主键 ID 列表</summary>
        public static List<long> GetSelectedIds(this Grid grid)
        {
            var ids = new List<long>();
            foreach (int rowIndex in grid.SelectedRowIndexArray)
                ids.Add(Convert.ToInt64(grid.DataKeys[rowIndex][0]));
            return ids;
        }

        /// <summary>获取选择行值列表</summary>
        public static List<T> GetSelectedValues<T>(this Grid grid, string keyName)
        {
            var values = new List<T>();
            var n = grid.DataKeyNames.IndexOf(t => t == keyName);
            if (n != -1)
                foreach (int rowIndex in grid.SelectedRowIndexArray)
                {
                    var v = grid.DataKeys[rowIndex][n];
                    values.Add(v.ToText().Parse<T>());
                }
            return values;
        }

        

        /// <summary>获取选择行名称列表（DataKeyNames="Id,Name")</summary>
        public static List<string> GetSelectedNames(this Grid grid)
        {
            var names = new List<string>();
            foreach (int rowIndex in grid.SelectedRowIndexArray)
            {
                if(grid.DataKeys[rowIndex].Length >= 1)
                    names.Add(grid.DataKeys[rowIndex][1].ToString());
            }
            return names;
        }

        /// <summary>设置选中的ID</summary>
        public static void SetSelectedIds(this Grid grid, List<long> ids)
        {
            var rowIds = new List<int>();
            if (grid.IsDatabasePaging)
            {
                for (int i = 0, count = Math.Min(grid.PageSize, (grid.RecordCount - grid.PageIndex * grid.PageSize)); i < count; i++)
                {
                    var id = Convert.ToInt64(grid.DataKeys[i][0]);
                    if (ids.Contains(id))
                    {
                        rowIds.Add(i);
                        ids.Remove(id);
                        if (ids.Count == 0)
                            break;
                    }
                }
            }
            else
            {
                int pageStartIndex = grid.PageIndex * grid.PageSize;
                for (int i = pageStartIndex, count = Math.Min(pageStartIndex + grid.PageSize, grid.RecordCount); i < count; i++)
                {
                    var id = Convert.ToInt64(grid.DataKeys[i][0]);
                    if (ids.Contains(id))
                    {
                        rowIds.Add(i - pageStartIndex);
                        ids.Remove(id);
                        if (ids.Count == 0)
                            break;
                    }
                }
            }
            grid.SelectedRowIndexArray = rowIds.ToArray();
        }


        /// <summary>设置选中的键值</summary>
        public static void SetSelectedKeys(this Grid grid, List<string> keys, int keyIndex)
        {
            if (keys.IsEmpty()) return;
            var rowIds = new List<int>();
            if (grid.IsDatabasePaging)
            {
                for (int i = 0, count = Math.Min(grid.PageSize, (grid.RecordCount - grid.PageIndex * grid.PageSize)); i < count; i++)
                {
                    var name = grid.DataKeys[i][keyIndex].ToString();
                    if (keys.Contains(name))
                    {
                        rowIds.Add(i);
                        keys.Remove(name);
                        if (keys.Count == 0)
                            break;
                    }
                }
            }
            else
            {
                int pageStartIndex = grid.PageIndex * grid.PageSize;
                for (int i = pageStartIndex, count = Math.Min(pageStartIndex + grid.PageSize, grid.RecordCount); i < count; i++)
                {
                    var name = grid.DataKeys[i][1].ToString();
                    if (keys.Contains(name))
                    {
                        rowIds.Add(i - pageStartIndex);
                        keys.Remove(name);
                        if (keys.Count == 0)
                            break;
                    }
                }
            }
            grid.SelectedRowIndexArray = rowIds.ToArray();
        }
    }
}