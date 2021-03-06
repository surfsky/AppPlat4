﻿//using EntityFramework.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using App.Utils;
using App.Entities;

namespace App.DAL
{
    /// <summary>
    /// GDP 数据（报表示例，详见/Report目录）
    /// </summary>
    [UI("报表", "GDP 报表")]
    public class RptGDP : EntityBase<RptGDP>
    {
        [UI("季度")]            public string Quarter { get; set; }

        // 生产总值
        [UI("生产总值", "绝对值", "{0:0.00}")] public double? GDPAbs { get; set; }
        [UI("生产总值", "增速%", "{0:0.00}")] public double? GDPInc { get; set; }

        // 第一产业
        [UI("第一产业", "绝对值", "{0:0.00}")] public double? FirstIndustryAbs { get; set; }
        [UI("第一产业", "增速%", "{0:0.00}")] public double? FirstIndustryInc { get; set; }
        [UI("第一产业", "占比", "{0:0.00}")] public double? FirstIndustryPct { get; set; }

        // 第二产业
        [UI("第二产业", "绝对值", "{0:0.00}")] public double? SecondIndustryAbs { get; set; }
        [UI("第二产业", "增速%", "{0:0.00}")] public double? SecondIndustryInc { get; set; }
        [UI("第二产业", "占比", "{0:0.00}")] public double? SecondIndustryPct { get; set; }

        // 第三产业
        [UI("第三产业", "绝对值", "{0:0.00}")] public double? ThirdIndustryAbs { get; set; }
        [UI("第三产业", "增速%",  "{0:0.00}")] public double? ThirdIndustryInc { get; set; }
        [UI("第三产业", "占比", "{0:0.00}")] public double? ThirdIndustryPct { get; set; }

        // 工业
        [UI("工业", "绝对值", "{0:0.00}")] public double? IndustryAbs { get; set; }
        [UI("工业", "增速%", "{0:0.00}")] public double? IndustryInc { get; set; }

        // 全国/浙江
        [UI("", "全国增速", "{0:0.00}")] public double? CountryInc { get; set; }
        [UI("", "浙江增速", "{0:0.00}")] public double? ZheJiangInc { get; set; }

        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        /// <summary>按季度查询</summary>
        /// <param name="from">2017Q1</param>
        /// <param name="to">2017Q3</param>
        /// <returns></returns>
        public static IQueryable<RptGDP> SearchByQuarter(string from = "", string to = "")
        {
            IQueryable<RptGDP> q = Set;
            if (!string.IsNullOrEmpty(from)) q = q.Where(t => t.Quarter.CompareTo(from) >= 0);
            if (!string.IsNullOrEmpty(to))   q = q.Where(t => t.Quarter.CompareTo(to) <= 0);
            return q;
        }
    }
}