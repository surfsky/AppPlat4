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
    /// 记录每日登陆
    /// </summary>
    [UI("系统", "每日登陆表")]
    public class LoginLog : EntityBase<LoginLog>
    {
        [UI("用户")] public long? UserID { get; set; }
        [UI("登陆方式")] public string Where { get; set; }

        public virtual User User { get; set; }

        //-----------------------------------------------
        // 构造函数
        //-----------------------------------------------
        public LoginLog() { }
        /// <summary>创建新文章</summary>
        public LoginLog(long? userId, string Where = "")
        {
            this.UserID = userId;
            this.Where = Where;
        }
        /// <summary>插入或更新用户的登录记录</summary>
        public static void Regist(long? userId, string where = "")
        {
            string startDtStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").Substring(0, 11);
            string endDtStr = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss").Substring(0, 11);

            var v = Search(userId: userId, startDt: Convert.ToDateTime(startDtStr), endDt: Convert.ToDateTime(endDtStr)).Count();

            if (v <= 0)
            {
                LoginLog item = new LoginLog(userId, where);
                item.Save();
            }
        }
        /// <summary>每日登陆记录</summary>
        [Searcher]
        [Param("userId", "用户ID")]
        [Param("startDt", "开始日期")]
        [Param("endDt", "结束日期")]
        [Param("user", "用户")]
        public static IQueryable<DAL.LoginLog> Search(long? userId = null,
            string user = "", DateTime? startDt = null, DateTime? endDt = null)
        {
            IQueryable<LoginLog> q = Set;
            if (userId != null) q = q.Where(t => t.UserID == userId);
            if (startDt != null) q = q.Where(t => t.CreateDt >= startDt);
            if (endDt != null) q = q.Where(t => t.CreateDt <= endDt);
            if (user.IsNotEmpty()) q = q.Where(t => t.User.NickName.Contains(user));

            return q;
        }
    }
}