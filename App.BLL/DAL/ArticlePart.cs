using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using App.Utils;

namespace App.DAL
{
    /// <summary>
    /// 文章内容块（解析html供客户端使用时用到）
    /// </summary>
    public class ArticlePart
    {
        public string Type { get; set; }
        public string Content { get; set; }
        public ArticlePart(string type, string content)
        {
            this.Type = type;
            this.Content = content;
        }
        public override string ToString()
        {
            return this.Content;
        }

        /// <summary>解析标签</summary>
        public static List<ArticlePart> ToParts(string text)
        {
            List<ArticlePart> items = new List<ArticlePart>();
            if (text.IsEmpty()) return items;

            // 预处理所有结对标签，弄成平面文档
            text = text.Replace("<br/>", "\r").Replace("<br>", "\r");                       // 换行符：改为回车  
            text = Regex.Replace(text, @"<!-[^>]*->", "");                                  // 注释：去掉
            text = Regex.Replace(text, @"<\/[^>]*>", "\r");                                 // 结对标签尾：改为回车
            text = Regex.Replace(text, @"<[^>]*>", new MatchEvaluator(DoReplace));          // 除了img标签外，去除所有单标签头
            text = Regex.Replace(text, @"[\r\n]{2,}", "\r", RegexOptions.IgnoreCase);       // 合并多个回车换行
            text = Regex.Replace(text, @"[\t\v ]{2,}", " ", RegexOptions.IgnoreCase);       // 合并多个空格
            text = text.Trim();

            // 剩下的就是单标签和回车符
            var parts = text.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                // 尝试解析图像标签
                string pattern = @"<img\s*src=['""](?<src>[^'""]*)['""].*>";
                Regex r = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                var m = r.Match(part);

                if (m.Success)
                    items.Add(new ArticlePart("img", m.Result("${src}")));
                else
                    items.Add(new ArticlePart("text", part.RemoveHtml()));
            }

            return items;
        }

        // 除了img标签以外，所有的标签都清空
        public static string DoReplace(Match m)
        {
            var txt = m.Value.ToLower();
            if (txt.StartsWith("<img"))
                return txt;
            return "";
        }
    }
}
