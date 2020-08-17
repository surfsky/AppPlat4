using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using App.Controls;
using App.Core;
using App.DAL;

namespace App.Components
{
    /// <summary>网页文件类型</summary>
    public enum WebFileType
    {
        Folder,
        File
    }

    /// <summary>
    /// 文章文件
    /// </summary>
    public class WebFile : EntityBase<WebFile>
    {
        public override long ID { get; set; } = SnowflakeID.Instance.NewID();  // 自动分配一个ID，避免表格处理时出错
        public WebFileType Type { get; set; }
        public string Name { get; set; }
        public string Folder { get; set; }
        public string Extension { get; set; }
        public string Url { get; set; }
        public string PhysicalPath { get; set; }
        public AuthAttribute Auth { get; set; }
        public string Size { get; set; }
        public FileInfo Info { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return this.Url;
        }

        /// <summary>获取文件列表</summary>
        /// <param name="root">根目录</param>
        /// <param name="folder">检索目录</param>
        /// <param name="exts">扩展名列表，格式如 .jpg .png</param>
        public static List<WebFile> GetFiles(string root, string folder, List<string> exts)
        {
            // 参数校对
            var files = new List<WebFile>();
            if (root.IsEmpty()) return files;
            if (folder.IsEmpty()) folder = root;
            var rootPath = Asp.MapPath(root).TrimEnd('\\');
            var folderPath = Asp.MapPath(folder).TrimEnd('\\');

            // 父目录
            DirectoryInfo di = new DirectoryInfo(folderPath);
            if (rootPath != folderPath)
            {
                var item = new WebFile();
                item.Type = WebFileType.Folder;
                item.Name = "..";
                item.PhysicalPath = GetParentFolder(folderPath);
                item.Url = item.PhysicalPath.ToVirtualPath();
                item.Auth = new AuthAttribute(true) { Ignore = true };
                files.Add(item);
            }

            // 子目录
            foreach (var d in di.GetDirectories())
            {
                var item = new WebFile();
                item.Type = WebFileType.Folder;
                item.Name = d.Name;
                item.PhysicalPath = d.FullName;
                item.Url = d.FullName.ToVirtualPath();
                item.Auth = new AuthAttribute(true) { Ignore = true };
                files.Add(item);
            }

            // 子文件
            foreach (var fi in di.GetFiles())
            {
                if (exts.IsEmpty() || exts.Contains(fi.Extension.ToLower()))
                {
                    var item = new WebFile();
                    item.Type = WebFileType.File;
                    item.Info = fi;
                    item.Name = fi.Name;
                    item.Folder = folder;
                    item.Extension = fi.Extension;
                    item.PhysicalPath = fi.FullName;
                    item.Url = Asp.ToVirtualPath(fi.FullName);
                    item.Auth = GetAuth(item.Url, out string desc);
                    item.Size = fi.Length.ToSizeText();
                    item.Description = desc;
                    files.Add(item);
                }
            }
            return files.AsQueryable().Sort(t => t.Type).Sort(t => t.Name).ToList();
        }


        /// <summary>递归获取所有文件列表</summary>
        /// <param name="folder">根目录</param>
        /// <param name="exts">扩展名列表，格式如 .jpg .png</param>
        public static void GetAllFiles(string folder, List<string> exts, List<WebFile> files)
        {
            if (folder.IsEmpty()) return;
            var folderPath = Asp.MapPath(folder).TrimEnd('\\');

            // 子文件
            DirectoryInfo di = new DirectoryInfo(folderPath);
            foreach (var fi in di.GetFiles())
            {
                var ext = fi.Extension.ToLower();
                if (exts.IsEmpty() || exts.Contains(ext))
                {
                    var item = new WebFile();
                    item.Type = WebFileType.File;
                    item.Info = fi;
                    item.Name = fi.Name;
                    item.Folder = folder;
                    item.Extension = ext;
                    item.PhysicalPath = fi.FullName;
                    item.Url = Asp.ToVirtualPath(fi.FullName);
                    item.Auth = GetAuth(item.Url, out string desc);
                    item.Size = fi.Length.ToSizeText();
                    item.Description = desc;
                    files.Add(item);
                }
            }
            // 子目录
            foreach (var d in di.GetDirectories())
                GetAllFiles(Asp.ToVirtualPath(d.FullName), exts, files);
        }

        /// <summary>获取不安全的文件列表</summary>
        public static List<WebFile> GetUnsafeFiles()
        {
            var files = new List<WebFile>();
            var exts = new List<string> { ".cs", ".aspx", ".ashx" };
            WebFile.GetAllFiles("/", exts, files);
            return files.Where(t => !t.Auth.IsSafe).ToList();
        }


        /// <summary>获取父目录</summary>
        private static string GetParentFolder(string folder)
        {
            return new DirectoryInfo(folder).Parent.FullName;
        }

        /// <summary>获取页面对应的访问权限（aspx ashx），其它页面忽略权限检查</summary>
        private static AuthAttribute GetAuth(string virtualPath, out string desc)
        {
            desc = "";
            var ext = virtualPath.GetFileExtension();
            var exts = new string[] { ".aspx", ".ashx" };
            if (exts.Contains(ext))
            {
                var type = Asp.GetHandler(virtualPath);
                if (type != null)
                {
                    desc = type.GetTitle();
                    var attr = type.GetAttribute<AuthAttribute>() ?? new AuthAttribute(false);
                    attr.CheckSafe();
                    return attr;
                }
            }
            if (ext == ".cs")
                return new AuthAttribute(false);
            return new AuthAttribute(true);
        }

    }

}