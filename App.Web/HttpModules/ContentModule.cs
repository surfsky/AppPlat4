using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using App.Utils;
using App.DAL;

namespace App.Components
{
    /*
    <system.webServer>
      <modules>
        <add name="ContentModule" type="App.Components.ContentModule" />
      </modules>
    /<system.webServer>
     * 
     */
    /// <summary>
    /// 内容管理模块。用户可使用 /Content/Home 的方式访问存储在数据库中的文档 Article
    /// </summary>
    public class ContentModule : IHttpModule
    {
        public void Dispose() { }
        public void Init(HttpApplication application)
        {
            application.PostResolveRequestCache += delegate (object sender, EventArgs e)
            {
                var content = ContentHandler.TryGetContent(HttpContext.Current.Request.Url);
                if (content != null)
                    HttpContext.Current.RemapHandler(new ContentHandler());
            };
        }
    }

    /// <summary>
    /// 内容管理处理器
    /// </summary>
    public class ContentHandler : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable { get { return false; } }

        // 处理请求
        public void ProcessRequest(HttpContext context)
        {
            var content = TryGetContent(context.Request.Url);
            if (content == null)
                Asp.Error(404, "Not found");
            else
            {
                var cacheKey = string.Format("content-{0}", context.Request.Url);
                var expired = DateTime.Now.AddSeconds(content.CacheSeconds.Value);
                var text = IO.GetCache<string>(cacheKey, () =>{
                    return BuildContentText(content.Body, content);
                    },
                    expired);
                Asp.WriteHtml(text);
            }
        }


        //-------------------------------------------------
        // 内容管理
        //-------------------------------------------------
        /// <summary>尝试根据 Url 获取内容对象（以后可加入正则解析）</summary>
        public static Article TryGetContent(Uri url)
        {
            var path = url.AbsolutePath.ToLower();
            foreach (var content in Contents)
            {
                var route = content.RoutePath?.ToLower();
                if (!route.IsEmpty() && path.StartsWith(route))
                    return content;
            }
            return null;
        }

        /// <summary>内容缓存列表</summary>
        static List<Article> Contents
        {
            get
            {
                return Asp.GetApplicationData<List<Article>>("AppContents", () =>
                {
                    return DAL.Article.Set.Where(t => t.RoutePath != null).Where(t => t.RoutePath != "").ToList();
                });
            }
        }

        /// <summary>重置内容缓存（清空）</summary>
        public static void Reload()
        {
            Asp.ClearApplicationData("AppContents");
        }


        /// <summary>构建内容文本</summary>
        public static string BuildContentText(string rawText, Article content, int deep = 0)
        {
            // 嵌套太深或者没有母模板，都跳出递归循环
            if (deep >= 5 || content.MotherID == null || content.MotherSlot.IsEmpty())
                return rawText;

            // 找到母版>填充插槽>递归
            var mother = Article.GetDetail(content.MotherID.Value, Common.LoginUser?.ID);
            rawText = mother.Body.Replace(content.MotherSlot, rawText);
            deep++;
            return BuildContentText(rawText, mother, deep);
        }
    }
}
