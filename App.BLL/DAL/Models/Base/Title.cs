using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using EntityFramework.Extensions;
using App.Utils;
using App.Entities;


namespace App.DAL
{
    /// <summary>
    /// 职称（头衔）表
    /// </summary>
    [UI("基础", "职称头衔")]
    public class Title : EntityBase<Title>, ICacheAll
    {
        [UI("职称（头衔）")]  public string Name { get; set; }
        [UI("备注")]          public string Remark { get; set; }
        public virtual List<User> Users { get; set; }


        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        // 查找
        public static IQueryable<Title> Search(string name)
        {
            IQueryable<Title> q = All.AsQueryable();
            if (name.IsNotEmpty())
                q = q.Where(t => t.Name.Contains(name));
            return q;
        }

        // 根据id列表获取
        public static List<Title> GetTitles(List<long> titleIds)
        {
            return Set.Where(t => titleIds.Contains(t.ID)).ToList();
        }

        /// <summary>删除相关数据</summary>
        public override void OnDeleteReference(long id)
        {
            this.Users = null;
        }
    }
}