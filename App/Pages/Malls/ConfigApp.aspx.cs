using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using EntityFramework.Extensions;
using Newtonsoft.Json.Linq;
using FineUIPro;
using App.Utils;
using App.DAL;
using App.Components;
using App.Controls;

namespace App.Pages
{
    /// <summary>
    /// app客户端配置
    /// </summary>
    [Auth(Powers.Client)]
    public partial class ConfigApp : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ShowData();
                UI.SetText(uploaderBg, SiteConfig.Instance.SizeAppBg);
            }
        }

        // 显示
        void ShowData()
        {
        }

        // App登录背景图片
        protected void uploaderBg_FileSelected(object sender, EventArgs e)
        {
            string url = UI.UploadFile(uploaderBg, "App", SiteConfig.Instance.SizeAppBg);
            //Configs.Site.WatermarkPic = url;
            //SiteConfig.Save();
            UI.SetValue(this.imgBg, url, true);
        }


        // 保存
        protected void btnSave_Click(object sender, EventArgs e)
        {
            //SiteConfig.Save();
            this.lblInfo.Text = "保存成功";
        }
    }
}
