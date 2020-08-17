using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using EntityFramework.Extensions;
using System.Data.Entity;
using System.ComponentModel;
using App.Utils;
using System.Collections;
using App.Entities;

namespace App.DAL
{
    /// <summary>
    /// 用户管辖的部门
    /// </summary>
    [UI("基础", "用户管辖的部门")]
    public class UserDept : EntityBase<UserDept>
    {
        [UI("用户")]     public long? UserID { get; set; }
        [UI("部门")]     public long? DeptID { get; set; }
        [UI("头衔")]     public string Title { get; set; }
        [UI("排序")]     public int?  Seq { get; set; }

        public virtual User User { get; set; }
        public virtual Dept Dept { get; set; }

        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        /// <summary>导出</summary>
        public override object Export(ExportMode type = ExportMode.Normal)
        {
            return new
            {
                this.ID,
                this.UserID,
                this.User?.NickName,
                this.DeptID,
                this.Dept?.Name,
                this.Title,
                this.Seq
            };
        }

        /// <summary>获取详情</summary>
        public new static UserDept GetDetail(long id)
        {
            return Set.Include(t => t.User).Include(t => t.Dept).Where(t => t.ID == id).FirstOrDefault();
        }

        /// <summary>检索</summary>
        public static IQueryable<UserDept> Search(
            long? userId = null,
            string userName = null,
            long? deptId = null,
            string deptName = null
            )
        {
            IQueryable<UserDept> q = Set.Include(t => t.User).Include(t => t.Dept);
            if (userId.IsNotEmpty())          q = q.Where(t => t.UserID == userId);
            if (userName.IsNotEmpty())        q = q.Where(t => t.User.NickName.Contains(userName));
            if (deptId.IsNotEmpty())          q = q.Where(t => t.DeptID == deptId);
            if (deptName.IsNotEmpty())        q = q.Where(t => t.Dept.Name.Contains(deptName));
            return q;
        }

    }
}