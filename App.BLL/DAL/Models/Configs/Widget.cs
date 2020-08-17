using App.Utils;
//using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using App.Entities;

namespace App.DAL
{
    [UI("配置", "小插件")]
    public class Widget : EntityBase<Widget>, ICacheAll
    {
        [UI("标题")]                public string Title { get; set; }
        [UI("插件类型")]            public string TypeName { get; set; }
        [UI("位置")]                public int? Seq { get; set; }
        [UI("宽度")]                public float? Width { get; set; }
        [UI("高度")]                public float? Height { get; set; }
        [UI("开始日期（如-30）")]   public double? StartDay { get; set; }
        [UI("结束日期（如0）")]     public double? EndDay { get; set; }
     }
}