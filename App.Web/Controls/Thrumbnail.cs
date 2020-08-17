using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Drawing;
using App.Utils;
using FineUIPro;

namespace App.Controls
{
    /// <summary>
    /// 缩略图控件（点击后可显示原图）
    /// </summary>
    public class Thrumbnail : FineUIPro.HyperLink
    {
        /// <summary>图片地址</summary>
        public string ImageUrl
        {
            get { return GetState("ImageUrl", ""); }
            set
            {
                if (ImageUrl != value)
                {
                    SetState("ImageUrl", value);
                    ShowImage();
                }
            }
        }

        /// <summary>图片宽度</summary>
        public int ImageWidth
        {
            get { return GetState("ImageWidth", 200); }
            set
            {
                if (ImageWidth != value)
                {
                    SetState("ImageWidth", value);
                    ShowImage();
                }
            }
        }


        // 初始化
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.EncodeText = false;
            this.Target = "_blank";
            if (this.ImageUrl.IsNotEmpty())
                ShowImage();
        }

        // 显示图片
        void ShowImage()
        {
            this.NavigateUrl = ImageUrl;
            this.Text = string.Format("<img src='{0}?w={1}' style='max-width:{1}px;display:block'/>", ImageUrl, ImageWidth);
        }


        //--------------------------------------------
        // 辅助方法
        //--------------------------------------------
        // ViewState 辅助方法 ( 注：Control.ViewState 是 protected 的，无法写成扩展方法抽取出来，只能继承使用）
        protected void SetState(string name, object value)
        {
            ViewState[name] = value;
        }
        protected string GetState(string name, string defaultValue)
        {
            return ViewState[name] == null ? defaultValue : (string)ViewState[name];
        }
        protected T GetState<T>(string name, T defaultValue) where T : struct
        {
            return ViewState[name] == null ? defaultValue : (T)ViewState[name];
        }

    }
}