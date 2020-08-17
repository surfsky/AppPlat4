using App.Core;
using App.Spire;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

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
        public static void Down(string url, bool? protect, string attachName)
        {
            // 路径校验
            url = url.TrimQuery();
            var path = Asp.MapPath(url);
            var ext = path.GetFileExtension();
            if (!File.Exists(path))
            {
                Asp.WriteError(404, "Not found");
                return;
            }

            // 是否保护文件逻辑（如果设置了参数，以参数为准；如果未设置参数，以系统为准；若系统也未设置，不保护）
            protect = protect ?? (Configs.Article.Protect ?? false);

            // 不保护，直接输出
            if (!protect.Value)
            {
                Asp.WriteFile(path, attachName);
                return;
            }

            // 输出保护文件
            if (IsOfficeFile(ext))
                DownOfficeFile(path, attachName);
            else if (IsImageFile(ext))
                DownImageFile(path, attachName);
            else
                Asp.WriteFile(path, attachName);
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
        public static void DownImageFile(string path, string attachName)
        {
            var ext = path.GetFileExtension();
            var logo = Configs.Site.Logo;
            if (logo.IsEmpty())
                Asp.Write(path);

            // 缓存带水印文件
            var key = path.ToVirtualPath().MD5();
            var cachePath = Asp.MapPath(string.Format("/Caches/{0}{1}", key, ext));
            if (!File.Exists(cachePath) || Asp.GetQueryBool("refresh") == true)
            {
                var img = Image.FromFile(path);
                var imgLogo = Image.FromFile(Asp.MapPath(logo));
                imgLogo = Painter.CreateThumbnail(imgLogo, 20);
                if (img.Width < imgLogo.Width * 4)
                {
                    // 原图片太小就不打水印了
                    img.Save(cachePath);
                }
                else
                {
                    // 在右下角打水印
                    var padding = 10;
                    var point = new Point(img.Width - imgLogo.Width - padding, img.Height - imgLogo.Height - padding);  // 右下角
                    var img2 = Painter.MergeImage(img, imgLogo, 0.5f, point);
                    img2.Save(cachePath);
                    img2.Dispose();
                }
                img.Dispose();
                imgLogo.Dispose();
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
        public static void DownOfficeFile(string path, string attachName)
        {
            // 除了excel以外的所有文件都输出为pdf
            var ext = path.GetFileExtension();
            if (!ext.Contains(".xls"))
                ext = ".pdf";
            var mimeType = ext.GetMimeType();
            if (attachName.IsNotEmpty() && attachName.GetFileExtension().IsNotEmpty())
                attachName = attachName.TrimExtension() + ext;

            // 计算水印缓存文件名
            var user = Common.LoginUser;
            var key = $"{path.ToVirtualPath()}-{user.Name}".MD5();
            var cachePath = Asp.MapPath(string.Format("/Caches/{0}{1}", key, ext));

            // 生成带水印缓存文件
            if (!File.Exists(cachePath) || Asp.GetQueryBool("refresh") == true)
            {
                var watermarker = DrawHelper.GetWatermarker(path);
                if (watermarker != null)
                {
                    var userName = user.RealName.IsEmpty() ? user.NickName : user.RealName;
                    var text = string.Format("{0}-{1:yyyyMMdd}", userName, DateTime.Now);

                    //
                    try
                    {
                        if (watermarker is WordWatermarker && ext == ".pdf")
                            watermarker.Config.TextColor = Color.FromArgb(255, 0, 0, 0);
                        watermarker.DoWatermark(path, cachePath, true, text, "");
                    }
                    catch (Exception ex)
                    {
                        cachePath = path;
                        Logger.LogWebException("Office-Fail", ex);
                    }
                }
                Logger.LogDb("Office-BuildCache", new { path, cachePath, user?.NickName }.ToJson());
            }

            // 输出到客户端
            Logger.LogDb("Office-WriteCache", new { path, cachePath, user?.NickName }.ToJson());
            Asp.WriteFile(cachePath, attachName, mimeType);
        }
    }

}