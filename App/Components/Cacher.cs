using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Text.RegularExpressions;
using App.Controls;
using App.DAL;
using App.Core;
using App.Components;
using System.IO;

namespace App
{
    /// <summary>
    /// 文件相关辅助方法
    /// </summary>
    public class Cacher
    {
        /// <summary>清除文件缓存</summary>
        /// <param name="allOrExpired">是否删除所有缓存文件；还是只删除过期文件</param>
        public static int ClearFileCaches(bool allOrExpired)
        {
            var folder = GetCacheFolder(); // Asp.MapPath("/caches/");
            var files = Directory.EnumerateFiles(folder);
            var n = 0;
            foreach (string file in files)
            {
                try
                {
                    if (allOrExpired || IsFileCacheExpired(file))
                    {
                        File.Delete(file);
                        n++;
                    }
                }
                catch { }
            }
            return n;
        }

        /// <summary>获取缓存目录</summary>
        public static string GetCacheFolder()
        {
            return string.Format(@"{0}caches\", HttpRuntime.AppDomainAppPath);
        }

        /// <summary>缓存文件是否过期</summary>
        static bool IsFileCacheExpired(string file)
        {
            var ext = file.GetFileExtension();
            var minutes = Configs.Site.FileCacheMinutes ?? 10;

            FileInfo fi = new FileInfo(file);
            if (!fi.Exists)
                return false;
            return DateTime.Now > fi.CreationTime.AddMinutes(minutes);
        }


    }
}
