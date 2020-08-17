using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.DAL;
using System.Web.Security;
using App.Controls;
using App.Utils;
//using Aspose.Words;
using System.IO;
using App.Components;
using App.Entities;

namespace App.Handlers
{
    /// <summary>
    /// 文件下载处理器（可下载网站中的任意文件，要求URL签名）
    /// </summary>
    [Auth(AuthLogin=false, AuthSign=true)]
    [Param("rid", "资源ID（可选）")]
    [Param("url", "文件路径（可选）")]
    [Param("name", "附件文件名（可选）")]
    public class Down : HandlerBase
    {
        public override void Process(HttpContext context)
        {
            var rid = Asp.GetQueryLong("rid");
            if (rid != null)
                DownRes(rid);
            else
                DownUrl();
        }

        /// <summary>下载资源文件（需要rid，watermark）</summary>
        private static void DownRes(long? rid)
        {
            var watermark = Asp.GetQueryString("watermark");
            var res = Res.Get(rid);
            if (res == null)
                Asp.Error(404, "无此资源");
            else
            {
                var url = res.Url;
                var name = res.FileName;
                var protect = res.Protect;
                Downloader.Down(url, name, protect, watermark);
            }
        }

        private static void DownUrl()
        {
            var url = Asp.GetQueryString("url");
            var name = Asp.GetQueryString("name");
            var protect = Asp.GetQueryBool("protect");
            var watermark = Asp.GetQueryString("watermark");

            /*
            // 如果有url参数，则要求检测url签名
            if (url.IsNotEmpty())
            {
                if (!url.CheckSignedUrl())
                {
                    Common.ShowFail("Auth sign fail");
                    return;
                }
            }
            */

            // 如果是rid参数方式，无需检测url签名
            Downloader.Down(url, name, protect, watermark);
        }

    }
}