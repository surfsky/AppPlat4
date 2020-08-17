using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using App.Utils;
using App.DAL;  // ArticleConfig

namespace App.Components
{
    /// <summary>
    /// 文件下载（含保护逻辑）
    /// </summary>
    public class Downloader
    {
        static List<string> _officeExt = new List<string>() { ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf" };
        static List<string> _imageExt = new List<string>() { ".jpg", ".png", ".gif", ".jpeg", ".bmp", ".tif", ".tiff" };

        /// <summary>下载文件（并保护）</summary>
        public static void Down(string url, string attachName, bool? protect, string watermark)
        {
            // 校验文件是否存在
            url = url.TrimQuery();
            var path = Asp.MapPath(url);
            if (!File.Exists(path))
            {
                Asp.Error(404, "Not found");
                return;
            }

            // 判断是否保护文件：全局保护参数》当前参数》默认不保护
            if (ArticleConfig.Instance.Protect != null)
                protect = ArticleConfig.Instance.Protect;
            else if (protect == null)
                protect = false;

            // 不保护，直接输出
            if (!protect.Value)
            {
                Asp.WriteFile(path, attachName);
                return;
            }

            // 扩展名
            var ext = path.GetFileExtension();
            if (ext.IsEmpty())
                ext = attachName.GetFileExtension();

            // 输出保护文件
            if (IsOfficeFile(ext))
                DownOfficeFile(path, attachName, watermark);
            else if (IsImageFile(ext))
                DownImageFile(path, attachName, ArticleConfig.Instance.WatermarkImage);
            else
                Asp.WriteFile(path, attachName);
        }

        /// <summary>获取水印文字</summary>
        public static string GetWatermarkText()
        {
            var user = Common.LoginUser;
            if (user == null)
                return "";
            var userName = user.RealName.IsEmpty() ? user.NickName : user.RealName;
            var watermark = string.Format("{0} {1:yyyyMMdd}", userName, DateTime.Now);
            return watermark;
        }



        //--------------------------------------------
        // 下载图片文件
        //--------------------------------------------
        /// <summary>是图片文件</summary>
        public static bool IsImageFile(string ext)
        {
            return _imageExt.Contains(ext);
        }

        /// <summary>下载图片文件（并加以水印保护）</summary>
        public static void DownImageFile(string path, string attachName, Image logo)
        {
            if (logo == null)
            {
                Asp.WriteFile(path, attachName);
                return;
            }

            // 缓存带水印文件
            var ext = path.GetFileExtension();
            var key = path.ToVirtualPath().MD5();
            var cachePath = Asp.MapPath(string.Format("/Caches/{0}{1}", key, ext));
            if (!File.Exists(cachePath) || Asp.GetQueryBool("refresh") == true)
            {
                var img = Painter.LoadImage(path);   //Image.FromFile(path);
                if (img.Width < logo.Width * 4)
                {
                    // 原图片太小就不打水印了
                    img.Save(cachePath);
                }
                else
                {
                    // 在右下角打水印
                    var padding = 10;
                    var point = new Point(img.Width - logo.Width - padding, img.Height - logo.Height - padding);  // 右下角
                    var img2 = Painter.Merge(img, logo, 0.5f, point);
                    img2.Save(cachePath);
                    img2.Dispose();
                }
                img.Dispose();
                logo.Dispose();
            }

            // 输出缓存文件
            Asp.WriteFile(cachePath, attachName);
        }


        //--------------------------------------------
        // 下载 Office 文件
        //--------------------------------------------
        /// <summary>是Office文件</summary>
        public static bool IsOfficeFile(string ext)
        {
            return _officeExt.Contains(ext);
        }

        /// <summary>下载Office文件（并加以水印保护）</summary>
        /// <param name="path">文件的物理路径</param>
        /// <param name="path">文件名称</param>
        /// <param name="path">水印文本</param>
        public static void DownOfficeFile(string path, string attachName, string watermark)
        {
            if (watermark.IsEmpty())
                watermark = GetWatermarkText();

            try
            {
                // 除了excel以外的所有文件都输出为pdf
                var ext = path.GetFileExtension();
                if (!ext.Contains(".xls"))
                    ext = ".pdf";
                var mimeType = ext.GetMimeType();
                if (attachName.IsNotEmpty() && attachName.GetFileExtension().IsNotEmpty())
                    attachName = attachName.TrimExtension() + ext;

                // 计算水印缓存文件名
                // 生成带水印缓存文件
                var key = $"{path.ToVirtualPath()}-{watermark}".ToLower().MD5();
                var cachePath = Asp.MapPath(string.Format("/Caches/{0}{1}", key, ext));
                if (!File.Exists(cachePath) || Asp.GetQueryBool("refresh") == true)
                    OfficeHelper.MakeOfficeMarker(path, cachePath, ext, watermark);

                // 生成图片文件和pdf文件备用
                /*
                var cacheFolder = Asp.MapPath(string.Format("/Caches/{0}/", key));
                if (!Directory.Exists(cacheFolder) || Asp.GetQueryBool("refresh") == true)
                {
                    OfficeHelper.MakeOfficeImages(path, cacheFolder);
                    OfficeHelper.MarkOfficePdf(path, cacheFolder + "1.pdf");
                }
                */

                // 输出到客户端
                Logger.LogDb("Office-Write", new { path, cachePath, watermark }.ToJson());
                Asp.WriteFile(cachePath, attachName, mimeType);
            }
            catch (Exception ex)
            {
                //cachePath = path;
                Logger.LogWebException("Office-Fail", ex);
                throw ex;
            }

        }

    }

}