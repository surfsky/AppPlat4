using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using FineUIPro;
using App.Utils;
//using App.DAL;
using App.Entities;

namespace App.Components
{
    //-----------------------------------------
    // 处理器
    //-----------------------------------------
    /// <summary>
    /// 类型处理器接口
    /// </summary>
    public interface ITypeHandler
    {
         Type Type { get; }
         string FormUrl { get; }
         string GridUrl { get; }
         string Id { get; }
         string Name { get; }
    }

    /// <summary>
    /// 类型处理器：将实体类和编辑窗口匹配起来（实验中）
    /// 或者直接丢到数据库去管理
    /// </summary>
    public class TypeHandler<T> : ITypeHandler
    {
        public Type Type { get; set; }
        public string FormUrl { get; set; }
        public string GridUrl { get; set; }
        public string Id { get { return IdField.GetName();}}
        public string Name { get { return NameField.GetName(); } }
        protected Expression<Func<T, object>> IdField { get; set; }
        protected Expression<Func<T, object>> NameField { get; set; }

        public TypeHandler(string formUrl, string gridUrl, Expression<Func<T, object>> id, Expression<Func<T, object>> name)
        {
            this.Type = typeof(T);
            this.FormUrl = formUrl;
            this.GridUrl = gridUrl;
            this.IdField = id;
            this.NameField = name;
        }

        public TypeHandler(string pagePrefix, Expression<Func<T, object>> id, Expression<Func<T, object>> name)
        {
            this.Type = typeof(T);
            this.FormUrl = string.Format("{0}Form.aspx", pagePrefix);
            this.GridUrl = string.Format("{0}s.aspx", pagePrefix);
            this.IdField = id;
            this.NameField = name;
        }

        static string GetHandler(string name, bool formOrGrid)
        {
            return formOrGrid
                ? string.Format("~/Pages/{0}Form.aspx", name)
                : string.Format("~/Pages/{0}s.aspx", name)
                ;
        }

        /*
        /// <summary>类型对应的编辑器</summary>
        public static List<ITypeHandler> Handlers = new List<ITypeHandler>()
        {
            new TypeHandler<User>("~/Pages/User",               t => t.ID, t=>t.NickName),
            new TypeHandler<Dept>("~/Pages/Dept",               t => t.ID, t=>t.Name),
            new TypeHandler<Title>("~/Pages/Title",             t => t.ID, t=>t.Name),
            new TypeHandler<Advert>("~/Pages/Advert",           t => t.ID, t=>t.Title),
            new TypeHandler<History>("~/Pages/History",         t => t.ID, t=>t.Status),
            new TypeHandler<Invite>("~/Pages/Invite",           t => t.ID, t=>t.Inviter),
            new TypeHandler<Order>("~/Pages/Order",             t => t.ID, t=>t.SerialNo),
            new TypeHandler<OrderItem>("~/Pages/OrderItem",     t => t.ID, t=>t.Title),
            new TypeHandler<Product>("~/Pages/Product",         t => t.ID, t=>t.Name),
            new TypeHandler<ProductSpec>("~/Pages/ProductSpec", t => t.ID, t=>t.Name),
            new TypeHandler<UserSign>("~/Pages/UserSign",       t => t.ID, t=>t.User),
            new TypeHandler<Shop>("~/Pages/Shop",               t => t.ID, t=>t.Name),
            new TypeHandler<UserAsset>("~/Pages/UserAsset",     t => t.ID, t=>t.Name),
        };
        */

        /// <summary>创建表单弹窗列</summary>
        private static BaseField CreateFormColumn(UIAttribute ui, string windowId, ITypeHandler handler)
        {
            var url = handler.FormUrl + "?id={0}";
            var member = handler.Name;
            var id = handler.Id;
            var field = ui.Name;
            var col = new FineUIPro.WindowField()
            {
                Title = ui.Title,       // 弹窗的标题
                HeaderText = ui.Title,  // 列的标题
                SortField = ui.Name,
                Width = ui.ColumnWidth,
                DataIFrameUrlFormatString = url,
                WindowID = windowId,
                DataTextField = field + "." + member,
                DataIFrameUrlFields = field + "." + id
            };
            return col;
        }

    }

}