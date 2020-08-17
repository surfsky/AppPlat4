using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using EntityFramework.Extensions;
using System.Data.Entity;
using App.Utils;
using App.Components;
using App.Entities;
using Z.EntityFramework.Plus;

namespace App.DAL
{
    /// <summary>系统菜单</summary>
    [UI("系统", "菜单")]
    public class Menu : EntityBase<Menu>, ITree<Menu>, ICacheAll
    {
        [UI("名称")]         public string Name { get; set; }
        [UI("图片URL")]      public string ImageUrl { get; set; }
        [UI("导航URL")]      public string NavigateUrl { get; set; }
        [UI("备注")]         public string Remark { get; set; }
        [UI("顺序")]         public int?  Seq { get; set; }
        [UI("是否展开")]     public bool? IsOpen { get; set; } = false;
        [UI("是否可见")]     public bool? Visible { get; set; } = true;
        [UI("父菜单")]       public long? ParentID { get; set; }
        [UI("访问权限")]     public Powers? ViewPower { get; set; }



        public virtual Menu Parent { get; set; }
        public virtual List<Menu> Children { get; set; }

        [NotMapped] public int  TreeLevel { get; set; }    // 菜单在树形结构中的层级（从0开始）
        [NotMapped] public bool IsTreeLeaf { get; set; }   // 是否叶子节点（默认true）
        [NotMapped] public bool Enabled { get; set; }      // 是否可用（默认true）,在模拟树的下拉列表中使用
        [NotMapped] public Menu Previous { get; set; }     // 前一个
        [NotMapped] public Menu Next { get; set; }         // 后一个
        [NotMapped] public string SafeUrl { get; set; }    // 安全URL，增加了签名参数




        //-----------------------------------------------------
        // 接口方法
        //-----------------------------------------------------
        public object Clone()
        {
            return new Menu
            {
                ID = this.ID,
                Name = this.Name,
                ImageUrl = this.ImageUrl,
                NavigateUrl = this.NavigateUrl,
                SafeUrl = this.SafeUrl,
                Remark = this.Remark,
                Seq = this.Seq,
                IsOpen = this.IsOpen,
                Visible = this.Visible,
                ParentID = this.ParentID,
                ViewPower = this.ViewPower,
                TreeLevel = this.TreeLevel,
                IsTreeLeaf = this.IsTreeLeaf
            };
        }

        public override string ToString()
        {
            return $"{Name}({ID})";
        }


        //-----------------------------------------------------
        // CRUD
        //-----------------------------------------------------
        // 获取菜单详情（包括父节点和访问权限）
        public new static Menu GetDetail(long id)
        {
            return Set
                .Include(m => m.Parent)
                .Where(m => m.ID == id)
                .FirstOrDefault();
        }

        /// <summary>删除相关数据</summary>
        public override void OnDeleteReference(long id)
        {
            var children = Set.Where(m => m.ParentID == id).ToList();
            foreach (var child in children)
                OnDeleteReference(child.ID);
            Set.Where(t => t.ID == id).Delete();
        }


        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        /// <summary>所有菜单（有缓存）</summary>
        public new static List<Menu> All => IO.GetCache(AllCacheName, ()=>
        {
            var items = Set.OrderBy(m => m.Seq).ToList();
            var result = new List<Menu>();
            BuildMenuTree(items, null, 0, ref result);
            return result;
        });



        // 递归处理菜单，弄成树状的
        public static int BuildMenuTree(List<Menu> items, Menu parentItem, int level, ref List<Menu> result)
        {
            int count = 0;
            Menu pre = null;
            foreach (var menu in items.Where(m => m.Parent == parentItem))
            {
                count++;
                menu.SafeUrl = menu.NavigateUrl.ToSignUrl();
                result.Add(menu);
                menu.TreeLevel = level;
                menu.IsTreeLeaf = true;
                menu.Enabled = true;

                // 前后链表
                menu.Previous = pre;
                if (pre != null)
                    pre.Next = menu;
                pre = menu;

                // 子节点
                level++;
                int childCount = BuildMenuTree(items, menu, level, ref result);
                if (childCount != 0)
                    menu.IsTreeLeaf = false;
                level--;
            }
            return count;
        }



        //-----------------------------------------------
        // 排序相关
        //-----------------------------------------------
        /// <summary>排序前移</summary>
        public void MoveUp()
        {
            // 找到前驱节点；如果是自己不做任何操作；否则序号对调或减一
            var menu = this.Previous;
            if (menu == null)
                return;
            Swap(menu, -1);
        }

        /// <summary>排序后移</summary>
        public void MoveDown()
        {
            // 找到后继节点；如果是自己不做任何操作；否则序号对调或加一
            var menu = this.Next;
            if (menu == null)
                return;
            Swap(menu, 1);
        }

        /// <summary>排序置顶</summary>
        public void MoveTop()
        {
            // 找到最顶的节点；如果是自己不做任何操作；否则序号比最顶节点减一
            var menu = FindTop(this);
            if (menu == this)
                return;
            this.Seq = menu.Seq - 1;
            SaveMenu(this);
        }

        /// <summary>排序置底</summary>
        public void MoveBottom()
        {
            // 找到最底的节点；如果是自己不做任何操作；否则序号比对底节点加一
            var menu = FindBottom(this);
            if (menu == this)
                return;
            this.Seq = menu.Seq + 1;
            SaveMenu(this);
        }

        /// <summary>找到同级别头节点</summary>
        static Menu FindTop(Menu menu)
        {
            if (menu.Previous == null)
                return menu;
            return FindTop(menu.Previous);
        }

        /// <summary>找到同级别尾节点</summary>
        static Menu FindBottom(Menu menu)
        {
            if (menu.Next == null)
                return menu;
            return FindBottom(menu.Next);
        }

        /// <summary>与目标菜单对调，如果序号一样，加上差值</summary>
        private void Swap(Menu menu, int step)
        {
            if (menu.Seq != this.Seq)
            {
                var s = this.Seq;
                this.Seq = menu.Seq;
                menu.Seq = s;
            }
            else
                this.Seq += step;

            SaveMenu(menu);
            SaveMenu(this);
        }
        
        // 保存菜单，避免EF报错
        private static void SaveMenu(Menu menu)
        {
            var m = Menu.Get(menu.ID);
            m.Seq = menu.Seq;
            m.Save();
        }
    }
}