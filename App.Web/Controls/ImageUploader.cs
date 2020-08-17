using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Drawing;
using System.ComponentModel;
using App.Utils;
using FineUIPro;

namespace App.Controls
{
    /// <summary>
    /// 图片展示及上传控件（包括一个缩略图和一个文件上传控件）
    /// </summary>
    public class ImageUploader : FineUIPro.Panel
    {
        protected Thrumbnail Thrumbnail { get; set; }
        protected FileUpload Upload { get; set; }

        /// <summary>是否只读</summary>
        public bool Readonly
        {
            get { return this.Upload.Hidden; }
            set { this.Upload.Hidden = value; }
        }

        /// <summary>标签</summary>
        public string Label
        {
            get { return Thrumbnail.Label; }
            set { Thrumbnail.Label = value; }
        }

        /// <summary>图片地址</summary>
        public string ImageUrl
        {
            get { return Thrumbnail.ImageUrl; }
            set { Thrumbnail.ImageUrl = value; }
        }

        /// <summary>图片上传保存目录</summary>
        public string UploadFolder
        {
            get { return GetState("UploadFolder", "");}
            set { SetState("UploadFolder", value); }
        }

        /// <summary>缩略图宽度</summary>
        public int ThrumbnailWidth
        {
            get { return GetState("ImageWidth", 200); }
            set { SetState("ImageWidth", value); }
        }

        [TypeConverter(typeof(SizeConverter))]
        /// <summary>上传图片大小限制（文本格式如x,y）</summary>
        public Size? ImageSize
        {
            get { return GetState("ImageSize", new Size(0,0));  }
            set { SetState("ImageSize", value); SetButtonText(); }
        }


        /// <summary>图片文件上传结束事件</summary>
        public event EventHandler FileUploaded;


        //--------------------------------------------
        // 构造
        //--------------------------------------------
        /// <summary>构造函数</summary>
        public ImageUploader()
        {
            Thrumbnail = new Thrumbnail();
            Upload = new FileUpload();
            this.Items.Add(Thrumbnail);
            this.Items.Add(Upload);
        }

        //<f:Panel runat = "server" ShowBorder="false" ShowHeader="false" >
        //    <Items>
        //        <f:Image ID = "imgPhoto" CssClass="photo" ImageUrl="~/res/images/blank.png" ShowEmptyLabel="true" runat="server"/>
        //        <f:FileUpload runat = "server" ID="uploader" ShowRedStar="false" ShowEmptyLabel="true"
        //            ButtonText="上传图像" ButtonOnly="true" Required="false" ButtonIcon="ImageAdd" AcceptFileTypes="image/*"
        //            AutoPostBack="true" OnFileSelected="uploader_FileSelected"/>
        //    </Items>
        //</f:Panel>
        // 初始化
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ShowBorder = false;
            ShowHeader = false;
            Upload.ShowEmptyLabel = true;
            Upload.ButtonOnly = true;
            Upload.ButtonIcon = FineUIPro.Icon.ImageAdd;
            Upload.AcceptFileTypes = "image/*";
            Upload.AutoPostBack = true;
            Upload.ButtonText = "上传图像";
            SetButtonText();
            Upload.FileSelected += (s, e2) =>
            {
                if (Upload.HasFile)
                {
                    string imageUrl = UI.UploadFile(Upload, UploadFolder, ImageSize);
                    UI.SetValue(Thrumbnail, imageUrl, true);
                    if (FileUploaded != null)
                        FileUploaded(this, e2);
                }
            };
        }

        private void SetButtonText()
        {
            if (ImageSize != null && ImageSize.Value.Width != 0)
                Upload.ButtonText = $"上传图像({ImageSize.Value.Width}x{ImageSize.Value.Height})";
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