using App.Utils;
using System;
using System.Web;
using System.Web.SessionState;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using App.DAL;

namespace App.Components
{
    /// <summary>
    /// 图片及缩略图处理器。
    /// 图像url地址如：http://a.b.com/Pictury.png?w=200
    /// 配置：
    /// (1) 使用时需将jpg,png等图片从mime配置中删除（避免被 staticFileModule 处理）
    /// (2) webconfig 中进行配置：&lt;add name = "ThumbnailModule" type="App.Components.ImageModule" /&gt;
    /// </summary>
    public class ImageModule : IHttpModule
    {
        static List<string> _filter = new List<string>() {".png", ".jpg", ".jpeg", ".gif", ".bmp", ".tif", ".tiff"};
        public void Dispose(){}
        public void Init(HttpApplication application)
        {
            application.PostResolveRequestCache += delegate (object sender, EventArgs e)
            {
                var extension = HttpContext.Current.Request.RawUrl.GetFileExtension();
                if (_filter.Contains(extension))
                    HttpContext.Current.RemapHandler(new ImageHandler()); // 指定处理器
            };
        }
    }


    /// <summary>
    /// 图片处理器。如果启用ImageModule的话，此handle就无需在webconfig中配置了。
    /// &lt;add name="PngModule" path="*.png" verb="*" type="App.Components.ImageHandler"  /&gt;
    /// &lt;add name="JpgModule" path="*.jpg" verb="*" type="App.Components.ImageHandler"  /&gt;
    /// </summary>
    public class ImageHandler : IHttpHandler
    {
        public bool IsReusable { get { return true; } }

        // 处理请求
        public void ProcessRequest(HttpContext context)
        {
            // 原图路径校验
            var url = context.Request.Url.AbsolutePath.ResolveUrl();  // 去除 host 和 querystring
            var rawPath = Asp.MapPath(url);
            if (!File.Exists(rawPath))
                Asp.Error(404, "Not found");

            // 原图输出
            var mimeType = url.GetMimeType();
            var w = Asp.GetQueryInt("w");
            if (w == null)
            {
                Asp.WriteFile(rawPath, mimeType: mimeType);
                return;
            }

            // 缩略图参数
            var h = Asp.GetQueryInt("h");
            if (w > 1000) w = 1000;
            if (h != null && h > 1000) h = 1000;
            var key = context.Request.Url.PathAndQuery.ToLower().MD5();

            // 缩略图缓存策略
            // （1）客户端手动指派缩略图缓存模式：对客户端而言不友好，放弃
            // （2）服务器端配置缩略图缓存存储方式：采纳
            // （3）服务器端根据内存和存储状态自动调整缓存存储方式：需检测内存和硬盘，待定
            // 物理缓存方式输出缩略图
            var cacheMode = SiteConfig.Instance.CacheMode;
            if (cacheMode == CacheMode.File)
            {
                var cachePath = Asp.MapPath(string.Format("/Caches/{0}.cache", key));
                if (!File.Exists(cachePath))
                {
                    IO.PrepareDirectory(cachePath);
                    var img = Painter.Thumbnail(rawPath, w.Value, h);
                    img.Save(cachePath);
                    img.Dispose();
                }
                Asp.WriteFile(cachePath, mimeType: mimeType);
                return;
            }
            else
            {
                // 内存缓存方式输出缩略图
                var minutes = SiteConfig.Instance.MemoryCacheMinutes.Value;
                var image = IO.GetCache<Image>(key, () => Painter.Thumbnail(rawPath, w.Value, h), DateTime.Now.AddMinutes(minutes)) as Image;
                Asp.WriteImage(image);
            }
        }
    }
}
