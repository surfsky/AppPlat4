using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Text.RegularExpressions;
using App.DAL;  // SiteConfig
using App.Utils;
using System.IO;

namespace App
{
    /// <summary>
    /// 文件缓存
    /// </summary>
    public class FileCacher
    {
        /// <summary>缓存虚拟路径</summary>
        public static string CachePath = "/Caches/";

        /// <summary>缓存物理目录</summary>
        public static string CacheFolder => string.Format(@"{0}caches\", Asp.HostFolder);  // 用 MapPath() 会报错 Server 为空

        /// <summary>清除文件缓存</summary>
        /// <param name="allOrExpired">是否删除所有缓存文件；还是只删除过期文件</param>
        public static int ClearFileCaches(bool allOrExpired)
        {
            var folder = CacheFolder;
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

        /// <summary>缓存文件是否过期</summary>
        static bool IsFileCacheExpired(string file)
        {
            var ext = file.GetFileExtension();
            var minutes = SiteConfig.Instance.FileCacheMinutes ?? 10;

            FileInfo fi = new FileInfo(file);
            if (!fi.Exists)
                return false;
            return DateTime.Now > fi.CreationTime.AddMinutes(minutes);
        }


    }
}
