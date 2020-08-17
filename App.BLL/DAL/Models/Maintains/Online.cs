using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using App.Utils;
using App.Entities;


namespace App.DAL
{
    /// <summary>
    /// 用户最后活动时间记录表，一个用户一条记录（用户扩展表）。
    /// TODO: 可以合并到 User 表
    /// </summary>
    [UI("系统", "在线用户表")]
    public class Online : EntityBase<Online>
    {
        [UI("用户")]               public long? UserID { get; set; }
        [UI("最后访问的IP地址")]   public string IP { get; set; }

        public virtual User User { get; set; }

        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        /// <summary>获取在线用户数</summary>
        public static int GetOnlines(int minutes = 15)
        {
            return SearchOnlines(null, null, minutes).Count();
        }

        /// <summary>搜索在线用户（指定时间内有操作的用户）</summary>
        public static IQueryable<Online> SearchOnlines(long? userId, string name, int minutes=15)
        {
            var lastDt = DateTime.Now.AddMinutes(-minutes);
            IQueryable<Online> q = Set.Include(o => o.User);
            if (userId != null)                 q = q.Where(o => o.UserID == userId);
            if (!String.IsNullOrEmpty(name))    q = q.Where(o => o.User.Name.Contains(name));
            q = q.Where(o => o.UpdateDt > lastDt);
            return q;
        }

        /// <summary>获取详情</summary>
        public static Online GetDetail(long? userId, string name)
        {
            return userId != null 
                ? Set.Include(t => t.User).Where(o => o.User.ID == userId).FirstOrDefault()
                : Set.Include(t => t.User).Where(t => t.User.Name == name).FirstOrDefault()
                ;
        }

        /// <summary>插入或更新用户的登录记录</summary>
        public static DateTime Regist(long? userId, string username, string ip, DateTime dt)
        {
            var online = Online.GetDetail(userId, username);
            online = online ?? new Online();
            online.UserID = userId;
            online.IP = ip;
            online.UpdateDt = dt;
            online.Save();
            return dt;
        }

        /// <summary>更新用户的最后活动记录</summary>
        public static void Update(string username, string ip, DateTime dt)
        {
            var online = Online.GetDetail(null, username);
            if (online != null)
            {
                online.IP = ip;
                online.Save(false);
            }
        }
    }
}