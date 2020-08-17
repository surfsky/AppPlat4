using App.Controls.ECharts;
using App.DAL;
using App.Entities;
using App.Plugins;
using FineUIPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using App.Utils;
using App.Controls;

namespace App.Base
{
    //
    // News
    //
    public class News : IWidget
    {
        public string Title { get; set; } = "新闻";
        public void Render(DateTime startDt, DateTime endDt, string clientId)
        {
            // 注意模板不要换行。客户端脚本识别不了
            var template = @"<li><a href='#' onclick=""window.top.addMainTab('Article-{id}', '/pages/articles/Article.aspx?id={id}', '新闻') "">{title}</a><div class='createDt' style='color: #D6D5D5; font-size: 10px; float: right;'>{date}</div></li>";
            var items = DAL.Article.SearchKnowledges("", null, ArticleSortType.Date, 0, 10);
            var sb = new StringBuilder();
            foreach (var item in items)
            {
                var li = template
                    .Replace("{id}", item.ID.ToString())
                    .Replace("{title}", item.Title.ToString())
                    .Replace("{date}", item.CreateDt?.ToString("yyyy-MM-dd"))
                    ;
                sb.Append(li);
            }
            UI.Render(clientId, sb.ToString(), true);
        }
    }

    //
    // Base
    //
    public class UserChart : IWidget
    {
        public string Title { get; set; } = "用户数";
        public void Render(DateTime startDt, DateTime endDt, string clientId)
        {
            var data = DAL.User.StatDayAmount(startDt, endDt, t => t.InUsed != false);
            EChart.BuildLineChart(data, Title, "日期", "数量").Render(clientId);
        }
    }

    public class RoleChart : IWidget
    {
        public string Title { get; set; } = "角色";
        public void Render(DateTime startDt, DateTime endDt, string clientId)
        {
            var roles = Role.All;
            var data = new List<StatItem>();
            foreach (var role in roles)
                data.Add(new StatItem(role.Name, "", DAL.User.SearchByRole(role.ID).Count()));

            EChart.BuildPieChart(data, "角色").Render(clientId);
        }
    }

    // 
    // Artitle
    // 
    public class ArticleChart : IWidget
    {
        public string Title { get; set; } = "文章数"; 
        public void Render(DateTime startDt, DateTime endDt, string clientId)
        {
            var data = DAL.Article.StatDayAmount(startDt, endDt, t => t.Type == ArticleType.Knowledge);
            EChart.BuildLineChart(data, Title, "日期", "数量").Render(clientId);
        }
    }

    public class ArticleVisitChart : IWidget
    {
        public string Title { get; set; } = "访问数";
        public void Render(DateTime startDt, DateTime endDt, string clientId)
        {
            var data = ArticleVisit.StatDayNew(startDt, endDt, t => t.Article.Type == ArticleType.Knowledge);
            EChart.BuildLineChart(data, Title, "日期", "数量").Render(clientId);
        }
    }


    //
    // Mall
    //
    public class OrderChart : IWidget
    {
        public string Title { get; set; } = "订单数";
        public void Render(DateTime startDt, DateTime endDt, string clientId)
        {
            var shopId = Common.LoginUser.HasPower(Powers.Admin) ? null : Common.LoginUser.ShopID;
            var data = DAL.Order.Stat(shopId, startDt, endDt);
            EChart.BuildLineChart(data, Title, "日期", "数量").Render(clientId);
        }
    }

    public class SignChart : IWidget
    {
        public string Title { get; set; } = "签到数";
        public void Render(DateTime startDt, DateTime endDt, string clientId)
        {
            var shopId = Common.LoginUser.HasPower(Powers.Admin) ? null : Common.LoginUser.ShopID;
            var data = DAL.UserSign.Stat(shopId, startDt, endDt);
            EChart.BuildLineChart(data, "签到数", "日期", "数量").Render(clientId);
        }
    }
}
