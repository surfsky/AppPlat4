using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Utils;
using App.DAL;

namespace App.Components
{
    /// <summary>
    /// Office 辅助操作类（调用控制台程序处理office文件）
    /// </summary>
    public static class OfficeHelper
    {
        public static string _officeImager => ArticleConfig.Instance.OfficeImager.MapPath();  // Office 转图工具
        public static string _officeMarker => ArticleConfig.Instance.OfficeMarker.MapPath();  // Office 水印工具


        /// <summary>Office转化为图片</summary>
        /// <param name="sourceFile">源文件</param>
        /// <param name="targetFolder">目标目录</param>
        public static List<string> MakeOfficeImages(string sourceFile, string targetFolder)
        {
            var urls = new List<string>();
            if (_officeImager.IsEmpty())
                return urls;
            if (!File.Exists(_officeImager))
            {
                Logger.LogDb("Office-MissImager", "未找到Office转图程序：" + _officeImager);
                return urls;
            }

            Process p = new Process();
            p.StartInfo.FileName = _officeImager;
            p.StartInfo.Arguments = $"\"{sourceFile}\" \"{targetFolder}\" ";
            p.Start();
            p.WaitForExit();
            var n = p.ExitCode;
            if (n == 0)
                Logger.LogDb("Office-ImagerOK", new { sourceFile, targetFolder }.ToJson());
            else
                Logger.LogDb("Office-ImagerFail", new { sourceFile, targetFolder }.ToJson());

            // 遍历目录，获取所有图片地址
            var files = Directory.GetFiles(targetFolder, "*.png");
            return files.Cast(t => t.ToVirtualPath());
        }

        /// <summary>Office 转化为PDF </summary>
        public static void MarkOfficePdf(string sourceFile, string targetFile)
        {
            if (sourceFile.GetFileExtension() == ".pdf")
                File.Copy(sourceFile, targetFile);
            else
                MakeOfficeMarker(sourceFile, targetFile, ".pdf", "");
        }

        /// <summary>Office 文件打水印</summary>
        /// <param name="sourceFile">源文件</param>
        /// <param name="targetFile">目标文件</param>
        /// <param name="ext">目标文件扩展名</param>
        /// <param name="text">水印文本</param>
        public static void MakeOfficeMarker(string sourceFile, string targetFile, string ext, string text)
        {
            if (_officeMarker.IsEmpty())
                return;
            if (!File.Exists(_officeMarker))
            {
                Logger.LogDb("Office-MissMarker", "未找到Office水印程序：" + _officeMarker);
                return;
            }

            Process p = new Process();
            p.StartInfo.FileName = _officeMarker;
            p.StartInfo.Arguments = $"\"{sourceFile}\" \"{targetFile}\" \"{ext}\" \"{text}\"";
            p.Start();
            p.WaitForExit();
            var n = p.ExitCode;
            if (n == 0)
                Logger.LogDb("Office-WatermarkOK", new { sourceFile, targetFile, text }.ToJson());
            else
                Logger.LogDb("Office-WatermarkFail", new { sourceFile, targetFile, text }.ToJson());
        }
    }
}
