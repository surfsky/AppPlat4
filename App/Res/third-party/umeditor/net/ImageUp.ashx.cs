using App.Controls;
using App.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.Components;

namespace App.Controls.UmEditors
{
    [UI("UMEditor 图像上传处理器")]
    [Auth(Ignore=true)]
    public class ImageUp : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            // 上传配置
            var folder = "/Files/Editors/";                                     // 保存路径
            int size = 10;                                                      // 文件大小限制，单位MB
            string[] filetype = { ".gif", ".png", ".jpg", ".jpeg", ".bmp" };    // 文件允许格式

            // 上传图片
            var up = new App.UMUploader();
            var info = up.upFile(context, folder, filetype, size); //获取上传状态

            // 回传脚本
            string callback = context.Request["callback"];
            string editorId = context.Request["editorid"];
            var json = BuildJson(info);
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            context.Response.ContentType = "text/html";
            if (callback != null)
                context.Response.Write(String.Format("<script>{0}(JSON.parse(\"{1}\"));</script>", callback, json));
            else
                context.Response.Write(json);
        }

        // 可以用ToJson替代，或者直接输出
        private string BuildJson(Hashtable info)
        {
            List<string> fields = new List<string>();
            string[] keys = new string[] { "originalName", "name", "url", "size", "state", "type" };
            for (int i = 0; i < keys.Length; i++)
            {
                fields.Add(String.Format("\"{0}\": \"{1}\"", keys[i], info[keys[i]]));
            }
            return "{" + String.Join(",", fields) + "}";
        }
    }
}