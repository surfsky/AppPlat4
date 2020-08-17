using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
//using EntityFramework.Extensions;
using App.Utils;
using App.Entities;
using Z.EntityFramework.Plus;

namespace App.DAL
{
    /// <summary>部门表</summary>
    /// <remarks>子节点的判断：orgid相同，parentid=id</remarks>
    [UI("基础", "部门")]
    public class Dept : EntityBase<Dept>, ITree<Dept>, ICacheAll
    {
        [UI("组织")]          public long? OrgID { get; set; }
        [UI("名称")]          public string Name { get; set; }
        [UI("父部门")]        public long? ParentID { get; set; }
        [UI("排序")]          public int? Seq { get; set; }
        [UI("备注")]          public string Remark { get; set; }


        public virtual Dept Parent { get; set; }
        public virtual List<Dept> Children { get; set; }
        public virtual List<User> Users { get; set; }

        [NotMapped]           public bool Enabled { get; set; }           // <summary>是否可用（默认true）,在模拟树的下拉列表中使用</summary>
        [NotMapped]           public int TreeLevel { get; set; }          // <summary>菜单在树形结构中的层级（从0开始）</summary>
        [NotMapped]           public bool IsTreeLeaf { get; set; }        // <summary>是否叶子节点（默认true）</summary>


        //-----------------------------------------------------
        // 接口方法
        //-----------------------------------------------------
        public object Clone()
        {
            return new Dept
            {
                ID = this.ID,
                ParentID = this.ParentID,
                Name = this.Name,
                Remark = this.Remark,
                Seq = this.Seq,
            };
        }


        //-----------------------------------------------
        // 重载方法
        //-----------------------------------------------
        public override string ToString()
        {
            return Name;
        }

        public override void OnDeleteReference(long id)
        {
            // 删除附属表数据
            //Dept.Set.Where(t => t.ParentID == id).Update(t => new Dept { ParentID = null });
            User.Set.Where(t => t.DeptID == id).Update(t => new User { DeptID = null });
            UserDept.Set.Where(t => t.DeptID == id).Update(t => new UserDept { DeptID = null });
            ArticleDirFavorite.Set.Where(t => t.DeptID == id).Update(t => new ArticleDirFavorite { DeptID = null });

            //Set.Include(t => t.Users).Where(t => t.ID == id).ToList().ForEach(t => t.Users = null);
            //Db.SaveChanges();

            // 删除子部门
            var children = Set.Where(m => m.ParentID == id).ToList();
            foreach (var child in children)
                OnDeleteReference(child.ID);
            Set.Where(t => t.ID == id).Delete();
        }

        /*
        /// <summary>递归删除（及子部门、部门用户）</summary>
        public new static void DeleteRecursive(long id)
        {
            // 删除附属表数据
            Set.Include(t => t.Users).Where(t => t.ID == id).ToList().ForEach(t => t.Users = null);
            Db.SaveChanges();

            // 删除子部门
            var children = Set.Where(m => m.ParentID == id).ToList();
            foreach (var child in children)
                DeleteRecursive(child.ID);
            Set.Where(t => t.ID == id).Delete();
        }
        */


        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        /// <summary>所有部门（有缓存）</summary>
        public new static List<Dept> All => IO.GetCache(AllCacheName, () =>
        {
            var items = Set.OrderBy(m => m.Seq).ToList();
            return items.BuildTree();
        });


        /// <summary>获取组织相关的部门树</summary>
        public static List<Dept> GetDeptTree(long? orgId)
        {
            var items = Dept.All.Where(t => t.OrgID == orgId).ToList();
            return items.BuildTree();
        }


        //-----------------------------------------------------
        // 其它方法
        //-----------------------------------------------------
        /// <summary>获取详情（包括父节点）</summary>
        public new static Dept GetDetail(long id)
        {
            return Set.Include(t => t.Parent).Where(t => t.ID == id).FirstOrDefault();
        }

        /// <summary>递归获取区域列表（包含自身）</summary>
        public static List<Dept> GetDescendants(long? rootId)
        {
            return All.GetDescendants(rootId);
        }
    }
}