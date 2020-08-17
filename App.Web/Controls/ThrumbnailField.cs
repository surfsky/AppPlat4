using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Text;
using System.Web.UI;
using App.Utils;
using FineUIPro;

namespace App.Controls
{
    /// <summary>
    /// 缩略图列（点击后可显示原图）
    /// </summary>
    public class ThrumbnailField : FineUIPro.HyperLinkField
    {
        /// <summary>图片地址列名</summary>
        public string DataImageUrlField
        {
            get { return GetState("DataImageUrlField", ""); }
            set
            {
                if (DataImageUrlField != value)
                    SetState("DataImageUrlField", value);
            }
        }

        /// <summary>图片宽度</summary>
        public int ImageWidth
        {
            get { return GetState("ImageWidth", 30); }
            set
            {
                if (ImageWidth != value)
                    SetState("ImageWidth", value);
            }
        }

        // 初始化
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            var name = DataImageUrlField;
            var textFormat = string.Format("<img src='{{0}}?w={0}' width='{0}'/>", ImageWidth);
            this.DataTextField = name;
            this.DataTextFormatString = textFormat;
            this.DataNavigateUrlFields = new string[] { name };
            this.DataNavigateUrlFormatString = "{0}";
            this.Target = "_blank";
            this.HtmlEncode = false;
            this.UrlEncode = false;
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