using App.Utils;
//using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using App.Entities;

namespace App.DAL
{
    /// <summary>
    /// 路由控制
    /// </summary>
    public enum RouteType
    {
        [UI("协议")] Protocol,
        [UI("主机")] Host,
        [UI("路径")] Path
    }

    /// <summary>路由</summary>
    [UI("系统", "路由规则设置")]
    public class Route : EntityBase<Route>, ICacheAll, IDeleteLogic
    {
        [UI("类别")]       public RouteType? Type { get; set; }
        [UI("匹配路径")]   public string From { get; set; }
        [UI("目标路径")]   public string To { get; set; }
        [UI("是否生效")]   public bool?  InUsed { get; set; }
        [UI("备注")]       public string Remark { get; set; }

        /// <summary>路由列表</summary>
        public new static List<Route> All => IO.GetCache(AllCacheName, () =>
        {
            return Set.Where(t => t.InUsed == true).Where(t => t.From != "" && t.To != "").ToList();
        });


        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        // 查询
        public static IQueryable<DAL.Route> Search(RouteType? type = null)
        {
            IQueryable<Route> q = All.AsQueryable();
            if (type.IsNotEmpty())  q = q.Where(t => t.Type == type);
            return q;
        }

    }
}