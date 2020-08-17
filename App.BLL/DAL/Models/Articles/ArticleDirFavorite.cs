using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using EntityFramework.Extensions;
using System.Data.Entity;
using System.ComponentModel;
using App.Utils;
using App.Entities;
using Z.EntityFramework.Plus;

namespace App.DAL
{
    /// <summary>关注来源类型</summary>
    public enum FavoriteType : int
    {
        [UI("系统")]      System = 0,
        [UI("部门")]      Dept = 1,
        [UI("用户")]      User = 2,
    }

    /// <summary>用户关注的文章目录</summary>
    [UI("文档", "用户关注的文章目录")]
    public class ArticleDirFavorite : EntityBase<ArticleDirFavorite>
    {
        [UI("类型")]        public FavoriteType? Type { get; set; }
        [UI("类型")]        public string TypeName => Type.GetTitle();
        [UI("用户")]        public long? UserID { get; set; }
        [UI("部门")]        public long? DeptID { get; set; }
        [UI("知识库目录")]  public long? ArticleDirID { get; set; }
        [UI("排序")]        public int? Seq { get; set; }
        [UI("备注")]        public string Remark { get; set; }

        // 关联属性
        [UI("用户")] public virtual User User { get; set; }
        [UI("部门")] public virtual Dept Dept { get; set; }
        [UI("目录")] public virtual ArticleDir ArticleDir { get; set; }


        //----------------------------------------------------
        // 基础操作
        //----------------------------------------------------
        /// <summary>获取详细对象</summary>
        public override object Export(ExportMode type = ExportMode.Normal)
        {
            return new
            {
                ID,
                Type,
                Seq,
                ArticleDirID,
                ArticleDirName = this.ArticleDir?.Name,
                ArticleDirIcon = ArticleDir.Icon

                //this.TypeName,
                //this.UserID,
                //this.CreateDt,
                //this.Remark,
                //this.User?.NickName,
                //this.Dept?.Name,
            };
        }

        /// <summary>获取详细对象</summary>
        public new static ArticleDirFavorite GetDetail(long id)
        {
            return Set.Include(t => t.User).Include(t => t.Dept).Include(t => t.ArticleDir)
                .FirstOrDefault(t => t.ID == id);
        }


        /// <summary>获取用户关注的模块</summary>
        public static List<ArticleDirFavorite> GetUserFavorites(long userId)
        {
            var items = Search(userId: userId);
            if (items.Count() == 0)
            {
                var user = User.Get(userId);
                if (user != null)
                {
                    items = Search(type: FavoriteType.Dept, deptId: user.DeptID);
                    if (items.Count() == 0)
                        items = Search(type: FavoriteType.System);
                }
            }
            return items.Sort(t => t.Seq, true).ToList();
        }

        /// <summary>获取用户关注的模块</summary>
        public static List<ArticleDirFavorite> GetDeptFavorites(long deptId)
        {
            var items = Search(deptId: deptId);
            return items.Sort(t => t.Seq, true).ToList();
        }

        /// <summary>获取系统关注的模块</summary>
        public static List<ArticleDirFavorite> GetSystemFavorites()
        {
            return Search(type: FavoriteType.System).Sort(t => t.Seq, true).ToList();
        }

        /// <summary>清除用户关注目录</summary>
        public static void ClearUserFavorites(long userId)
        {
            ArticleDirFavorite.Set.Where(t => t.UserID == userId).Delete();
        }

        /// <summary>查询</summary>
        public static IQueryable<ArticleDirFavorite> Search(
            long? userId = null, string userName = null, long? deptId = null,
            FavoriteType? type = null,
            DateTime? startDt = null,
            DateTime? endDt = null
            )
        {
            IQueryable<ArticleDirFavorite> q = Set.Include(t => t.User).Include(t => t.Dept).Include(t => t.ArticleDir);
            if (userName.IsNotEmpty())     q = q.Where(t => t.User.NickName.Contains(userName));
            if (userId != null)            q = q.Where(t => t.UserID == userId);
            if (deptId != null)            q = q.Where(t => t.DeptID == deptId);
            if (type != null)              q = q.Where(t => t.Type == type);
            if (startDt != null)           q = q.Where(t => t.CreateDt >= startDt);
            if (endDt != null)             q = q.Where(t => t.CreateDt <= endDt);
            return q;
        }

        /// <summary>新增记录（会抛出异常）</summary>
        public static ArticleDirFavorite Add(FavoriteType type, long? userId, long? deptId, long articleDirId, string remark="")
        {
            var user = User.Get(userId);
            if (user != null)
            {
                // 新增记录
                var item = new ArticleDirFavorite();
                item.Type = type;
                item.UserID = userId;
                item.DeptID = deptId;
                item.ArticleDirID = articleDirId;
                item.Remark = remark;
                item.CreateDt = DateTime.Now;
                item.Save();
                return item;
            }
            return null;
        }

    }
}