using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.DAL;
using System.Configuration;
using System.Drawing;
//using EntityFramework.Extensions;
using App.Utils;
using App.Components;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entities;

namespace App.DAL
{
    [UI("文档", "文档库配置")]
    public class ArticleConfig : EntityBase<ArticleConfig>
    {
        public Size SizeWatermark = new Size(64, 64);

        [UI("文档", "热点关键字")]        public string  Keywords         { get; set; } = "5G,校园,政企,战狼";
        [UI("防护", "保护文档")]          public bool?   Protect          { get; set; }
        [UI("防护", "水印图片")]          public string  WatermarkPic     { get; set; }
        [UI("防护", "Office水印器")]      public string  OfficeMarker     { get; set; }
        [UI("防护", "Office转图器")]      public string  OfficeImager     { get; set; }


        [UI("防护", "水印图片"), NotMapped]
        public Image WatermarkImage
        {
            get
            {
                return IO.GetDict<Image>("WatermarkImage", () =>
                {
                    try
                    {
                        var url = WatermarkPic;
                        if (url.IsEmpty())
                            return null;
                        var imgLogo = Painter.LoadImage(Asp.MapPath(url));  // Image.FromFile(Asp.MapPath(url));
                        imgLogo = Painter.Thumbnail(imgLogo, 20);
                        return imgLogo;
                    }
                    catch
                    {
                        return null;
                    }
                });
            }
        }

        /// <summary>批量初始化数据</summary>
        public override void Init()
        {
            var item = ArticleConfig.Instance;
            if (item.OfficeMarker.IsEmpty())   item.OfficeMarker = "/bin/OfficeMarker/OfficeMarker.exe";
            if (item.OfficeImager.IsEmpty())   item.OfficeImager = "/bin/OfficeImager/OfficeImager.exe";
            item.Save();
        }

        // 表单UI
        public override UISetting FormUI()
        {
            var ui = new UISetting<ArticleConfig>(true);
            ui.SetEditorImage(t => t.WatermarkPic, SizeWatermark);
            return ui.BuildGroups();
        }

    }
}