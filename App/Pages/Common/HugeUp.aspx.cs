using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using App.Components;
using App.Controls;
using App.Utils;
using App.DAL;
using App.HttpApi;
using App.Entities;

namespace App
{
    /// <summary>
    /// 大文件上传（成功）
    /// 
    /// 客户端                                                       服务器端
    /// --------                                                     --------
    /// 文件协商请求 DoRequest（fileName, fileSize)                  判断文件类型和大小是否合法，合法的话返回临时文件名
    /// 切割文件，循环发送DoChunk请求(name,index)                    保存分块文件到临时目录（接收是无序的），成功后应答
    /// 若区块发送完毕，发送DoMerge请求(folder,name,key,total)       根据文件名合并文件并保存到指定目录，若key存在并记录到数据库中去
    /// 上传成功后显示文件信息，跳到回调URL（如果有设置）
    /// 
    /// 文件               说明
    /// ----               ---------------
    /// HugeUp.aspx        提供html展现
    /// HugeUp.aspx.cs     以 HttpApi 方式提供数据接口（/HttpApi/App.HugeUp/DoXXX）
    /// </summary>
    [UI("大文件上传页面")]
    [Auth(AuthSign = true, AuthLogin = true)]
    [Param("folder", "保存目录", required: true)]
    [Param("title", "文件名")]
    [Param("filter", "文件过滤器")]
    [Param("key", "资源键")]
    [Param("callback", "回调URL")]
    public partial class HugeUp : PageBase
    {
        [HttpApi("协商上传文件", PostFile = true)]
        [HttpParam("ext", "扩展名")]
        public static APIResult DoRequest(string fileName, long fileSize)
        {
            var ext = fileName.GetFileExtension();
            var exts = SiteConfig.Instance.UpFileTypes.Split();
            if (!exts.Contains(ext))
                return new APIResult(false, "不允许该类文件上传");
            if (fileSize >= SiteConfig.Instance.UpFileSize * 1024 * 1024)
                return new APIResult(false, "文件大小不得超过 50 M");
            var id = string.Format("{0}{1}", SnowflakeID.Instance.NewID(), ext);
            return new APIResult(true, id);
        }

        [HttpApi("保存切片文件", PostFile = true)]
        [HttpParam("id", "操作编号")]
        [HttpParam("seq", "分块编号")]
        public static APIResult DoChunk(string id, string seq)
        {
            var context = HttpContext.Current;
            if (context.Request.Files.Count == 0)
                return new APIResult(false, "未找到文件");

            var file = context.Request.Files[0];
            var fileName = string.Format("{0}-{1}", id, seq);
            Uploader.UploadFile(file, "Chunks", fileName, false);
            return new APIResult(true, string.Format("Chunk {0} ok", seq));
        }

        [HttpApi("合并切片文件并保存到指定目录")]
        [HttpParam("id", "编号")]
        [HttpParam("total", "分块总数")]
        [HttpParam("folder", "文件保存目录")]
        [HttpParam("title", "文件标题")]
        [HttpParam("key", "资源键值")]
        public static APIResult DoMerge(string id, int total, string folder, string key = "", string title = "")
        {
            // 需合并的文件（在临时目录中根据文件名找）
            var path = Asp.MapPath("/Files/Chunks/");
            var files = new List<string>();
            for (int i = 0; i < total; i++)
                files.Add(string.Format("{0}{1}-{2}", path, id, i));

            // 合并文件
            var url = string.Format("/Files/{0}/{1}", folder, id);
            var filePath = Asp.MapPath(url);
            IO.MergeFiles(files, filePath);
            SaveRes(id, key, title, url);

            var fi = new FileInfo(filePath);
            var o = new { Url = url, Size = fi.Length.ToSizeText() };
            return new APIResult(true, "上传成功", o);
        }

        [HttpApi("直接保存上传文件", PostFile = true)]
        public static APIResult DoSave(string id, string folder, string key = "", string title = "")
        {
            var context = HttpContext.Current;
            if (context.Request.Files.Count == 0)
                return new APIResult(false, "未找到文件");

            var file = context.Request.Files[0];
            var url = Uploader.UploadFile(file, folder, id, true);
            SaveRes(id, key, title, url);

            var fi = new FileInfo(Asp.MapPath(url));
            var o = new { Url = url, Size = fi.Length.ToSizeText() };
            return new APIResult(true, "上传成功", o);
        }


        // 记录到数据库中
        private static void SaveRes(string id, string key, string title, string url)
        {
            if (key.IsNotEmpty())
            {
                if (title.IsEmpty())
                    title = id;
                Res.Add(ResType.File, key, url, title);
            }
        }


    }
}