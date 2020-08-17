using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.ComponentModel;
using FineUIPro;
using App.Utils;
using App.Components;

namespace App.Controls
{
    /// <summary>
    /// 文件选择器控件
    /// </summary>
    public class FileBox : PopupBox
    {
        /// <summary>根目录</summary>
        public string Root
        {
            get { return GetState("Root", "/"); }
            set { SetState("Root", value); }
        }

        /// <summary>文件过滤器。格式如".jpg .png .gif"</summary>
        public string Filter
        {
            get { return GetState("Filter", ""); }
            set { SetState("Filter", value); }
        }

        /// <summary>是否显示下载列</summary>
        public bool ShowDownload
        {
            get { return GetState("ShowDownload", true); }
            set { SetState("ShowDownload", value); }
        }

        /// <summary>是否显示信息列</summary>
        public bool ShowInfo
        {
            get { return GetState("ShowInfo", true); }
            set { SetState("ShowInfo", value); }
        }

        //
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.EnableEdit = true;
            this.PrepareUrlTemplate += () =>
            {
                this.Value = this.Text; // 文件选择控件名称和值是一样的
                this.UrlTemplate = Urls.GetExplorerUrl(this.Root, this.Filter, this.ShowDownload, this.ShowInfo, PageMode.Select, "", false);
            };
        }

    }
}