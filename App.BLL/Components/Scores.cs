using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using App.Utils;
using App.DAL;
using System.Data.Entity;

namespace App.Components
{

    /// <summary>
    /// 积分
    /// </summary>
    public class Score
    {
        [UI("积分")] public double Num { get; set; }

        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        /// <summary>计算积分</summary>
        [Searcher]
        [Param("startDt", "开始日期")]
        [Param("endDt", "结束日期")]
        public static double GetNum(long userId, DateTime? startDt = null, DateTime? endDt = null)
        {
            double num = 0;
            //登录积分
            var lrList = LoginLog.Search(userId: userId, startDt: startDt, endDt: endDt).Count();
            num = num + lrList;

            //阅读非视频积分
            var asrList = ArticleStudy.Search(userId: userId, startDt: startDt, endDt: endDt, type: ArticleType.Knowledge);
            var aas = asrList.Where(c => c.Article != null).GroupBy(c => c.Article).ToList();
            foreach (var aa in aas)
            {
                List<ArticleStudy> temp = aa.ToList();
                int time = temp.Sum(x => x.Interval);
                if (time >= 30)
                {
                    if (temp[0].Article.IsRequir == true)
                    {
                        num = num + 2;
                    }
                    else
                    {
                        num = num + 1;
                    }
                }
                if (time >= 120)
                {
                    int n = time / 120;
                    if (n > 5)
                    {
                        n = 5;
                    }
                    num = num + n;

                }
            }
            //有效评论积分
            var arList = Article.Search(type: ArticleType.Reply, startDt: startDt, endDt: endDt, status:ArticleStatus.Publish);
            arList = arList.Where(c => c.IsValid == true && c.AuthorID == userId);
            var ads = arList.GroupBy(t => new
            {
                Day = DbFunctions.TruncateTime(t.CreateDt).Value
            });
            foreach (var ad in ads)
            {
                var temp = ad.ToList();
                double jf = temp.Count() * 0.5;
                if(jf >= 2)
                {
                    jf = 2;
                }
                num = num + jf;
            }

            return num;
        }
    }
}