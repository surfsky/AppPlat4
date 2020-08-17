using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using EntityFramework.Extensions;
using System.Data.Entity;
using App.Utils;
using App.Entities;
using Z.EntityFramework.Plus;

namespace App.DAL
{
    /// <summary>
    /// 区域类型
    /// </summary>
    public enum AreaType
    {
        [UI("国家")]         Country = 0,
        [UI("省（直辖市）")] Province = 1,
        [UI("地级市")]       City = 2,
        [UI("区县")]         County = 3,
        [UI("街道")]         Street = 4,
    }

    /// <summary>
    /// 区域表（全国）
    /// </summary>
    [UI("基础", "区域")]
    public class Area : EntityBase<Area>, ITree<Area>, ICacheAll
    {
        [UI("类别")]                   public AreaType? Type { get; set; }
        [UI("类别")]                   public string TypeName { get { return this.Type.GetTitle(); } }

        [UI("名称")]                   public string Name { get; set; }
        [UI("编码")]                   public string Code { get; set; }
        [UI("顺序")]                   public int? Seq { get; set; }
        [UI("备注")]                   public string Remark { get; set; }
        [UI("全称")]                   public string FullName { get; set; }        // 全名，如：浙江温州鹿城区。后台编辑时自动生成

        [UI("父ID")]                   public long? ParentID { get; set; }
        [UI("父区域")]                 public virtual Area Parent { get; set; }
        [UI("子区域集合")]             public virtual List<Area> Children { get; set; }

        // ITree
        [UI(Mode=PageMode.None), NotMapped]  public int  TreeLevel { get; set; }
        [UI(Mode=PageMode.None), NotMapped]  public bool IsTreeLeaf { get; set; }
        [UI(Mode=PageMode.None), NotMapped]  public bool Enabled { get; set; }


        //-----------------------------------------------------
        // 接口方法
        //-----------------------------------------------------
        public override string ToString()
        {
            return $"{Name}({TypeName})";
        }
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

        //
        public override UISetting GridUI()
        {
            var ui = new UISetting<Area>(true);
            ui.SetColumnTree(t => t.Name, t=> t.ID);
            ui.SetColumn(t => t.TreeLevel, null, ColumnType.None);
            ui.SetColumn(t => t.IsTreeLeaf, null, ColumnType.None);
            ui.SetColumn(t => t.Enabled, null, ColumnType.None);
            return ui;
        }

        //-----------------------------------------------
        // 树相关
        //-----------------------------------------------
        /// <summary>缓存</summary>
        public new static List<Area> All => IO.GetCache(AllCacheName, () =>
        {
            var items = Set.OrderBy(m => m.Seq).ToList();
            return items.BuildTree();
        });

        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        [Param("name", "名称")]
        [Param("code", "编码")]
        public static IQueryable<Area> Search(string name, string code)
        {
            IQueryable<Area> q = All.AsQueryable();
            if (name.IsNotEmpty())   q = q.Where(t => t.Name.Contains(name));
            if (code.IsNotEmpty())   q = q.Where(t => t.Code.Contains(code));
            return q;
        }

        /// <summary>递归获取区域列表（包含自身）</summary>
        public static List<Area> GetDescendants(long? rootId)
        {
            return All.GetDescendants(rootId);
        }

        /// <summary>计算全名</summary>
        public static string CalcFullName(long? id)
        {
            if (id == null)
                return "";
            var area = Get(id);
            if (area == null)
                return "";
            return string.Format("{0}{1}", CalcFullName(area.ParentID), area.Name);
        }

        /// <summary>设置全名</summary>
        public void SetFullName()
        {
            this.FullName = CalcFullName(this.ID);
            this.Save();
        }

        //-----------------------------------------------------
        // 重载方法
        //-----------------------------------------------------
        /// <summary>获取详情（包括父节点）</summary>
        public new static Area GetDetail(long id)
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
    }
}