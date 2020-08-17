using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
//using EntityFramework.Extensions;
using App.Utils;
using App.Components;
using Newtonsoft.Json;
using App.Entities;
using Z.EntityFramework.Plus;

namespace App.DAL
{
    /// <summary>
    /// 文章类别
    /// </summary>
    [UI("文档", "文章目录")]
    public class ArticleDir : EntityBase<ArticleDir>, ITree<ArticleDir>, IDeleteLogic, ICacheAll, ICloneable
    {
        [UI("名称")]          public string Name { get; set; }
        [UI("备注")]          public string Remark { get; set; }
        [UI("排序")]          public int? Seq { get; set; }
        [UI("父ID")]          public long? ParentID { get; set; }
        [UI("全称")]          public string FullName { get; set; }
        [UI("图标")]          public string Icon { get; set; } = SiteConfig.Instance.DefaultDirImage;
        [UI("在用")]          public bool? InUsed { get; set; } = true;


        [NotMapped]           public bool Enabled { get; set; }           // 是否可用（默认true）,在模拟树的下拉列表中使用
        [NotMapped]           public int TreeLevel { get; set; }          // 菜单在树形结构中的层级（从0开始）
        [NotMapped]           public bool IsTreeLeaf { get; set; }        // 是否叶子节点（默认true）


        [JsonIgnore]
        public virtual ArticleDir Parent { get; set; }
        public virtual List<ArticleDir> Children { get; set; }

        //-----------------------------------------------
        // 扩展属性
        //-----------------------------------------------
        /// <summary>缓存</summary>
        public new static List<ArticleDir> All => IO.GetCache(AllCacheName, () =>
        {
            var items = ValidSet.OrderBy(d => d.Seq).ToList();
            return items.BuildTree();
        });


        //-----------------------------------------------
        // 接口方法
        //-----------------------------------------------
        // 克隆（避免树节点Enabled状态被覆盖）
        public object Clone()
        {
            var item = new ArticleDir
            {
                ID = ID,
                ParentID = ParentID,
                Name = Name,
                Remark = Remark,
                Seq = Seq,
                TreeLevel = TreeLevel,
                Enabled = Enabled,
                IsTreeLeaf = IsTreeLeaf,
                CreateDt = CreateDt,
                UpdateDt = UpdateDt,
                Icon = Icon
            };
            return item;
        }

        // 导出
        public override object Export(ExportMode type = ExportMode.Normal)
        {
            return new
            {
                ID,
                ParentID,
                Name,
                Icon,
                Enabled,
                Children = Children.Cast(t => t.Export(type))
            };
        }

        // 保存前处理
        public override void BeforeSave(EntityOp op)
        {
            this.FullName = string.Format("{0}/{1}", Parent?.FullName, Name);
        }

        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        /// <summary>递归获取子节点（及自身）的id列表（可参考Dept.GetRecursive进行修改）</summary>
        public static List<long> GetDescendantIds(List<long> ids)
        {
            return All.GetDescendants(ids).Cast(t => t.ID);
        }


        //-----------------------------------------------------
        // 其它方法
        //-----------------------------------------------------
        /// <summary>获取详情（包括父节点）</summary>
        public new static ArticleDir GetDetail(long id)
        {
            return Set.Include(t => t.Parent).Where(t => t.ID == id).FirstOrDefault();
        }

        /// <summary>递归删除</summary>
        public override void OnDeleteReference(long id)
        {
            var children = Set.Where(m => m.ParentID == id).ToList();
            foreach (var child in children)
                OnDeleteReference(child.ID);
            Set.Where(t => t.ID == id).Delete();
        }

        /// <summary>查找子目录</summary>
        public static ArticleDir SearchChild(ArticleDir root, long id)
        {
            if (root == null || root.ID == id)
                return root;
            foreach (var child in root.Children)
            {
                var item = SearchChild(child, id);
                if (item != null)
                    return item;
            }
            return null;
        }



        /// <summary>检测文章目录是否可用（将设置目录的 Enabled 属性）</summary>
        /// <param name="dir">文章目录（包括子目录）</param>
        /// <param name="ids">允许访问的目录ID</param>
        public static void CheckArticleDir(ArticleDir dir, List<long> ids)
        {
            if (dir == null || ids == null)
                return;
            SetArticleDirEnable(dir, ids.Contains(dir.ID));
            foreach (var sub in dir.Children)
                CheckArticleDir(sub, ids);
        }

        /// <summary>设置目录可用性。如果目录可用，溯源到根目录都可用.（父节点会反复设置true，可优化）</summary>
        static void SetArticleDirEnable(ArticleDir dir, bool enabled)
        {
            if (dir == null)
                return;
            dir.Enabled = enabled;
            if (enabled && dir.Parent != null)
                SetArticleDirEnable(dir.Parent, true);
        }

        /// <summary>
        /// 获取父节点和自身的id
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static List<long> GetParentIds(ArticleDir dir)
        {
            List<long> dirIds = new List<long>();
            dirIds.Add(dir.ID);
            while (dir.Parent != null)
            {
                dirIds.Add(dir.ParentID.Value);
                dir = dir.Parent;
            }

            return dirIds;
        }
    }
}