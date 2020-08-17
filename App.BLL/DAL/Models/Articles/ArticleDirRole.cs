using App.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using App.Entities;

namespace App.DAL
{
    [UI("文档", "角色可访问的文章目录")]
    public class ArticleDirRole : EntityBase<ArticleDirRole>, ICacheAll
    {
        [UI("角色")]     public long  RoleID { get; set; }
        [UI("文章目录")] public long  ArticleDirID { get; set; }

        //public string RoleName => Role.Name;
        //public string ArticleDirName => ArticleDir.FullName;

        public virtual Role Role { get; set; }
        public virtual ArticleDir ArticleDir { get; set; }
    }
}