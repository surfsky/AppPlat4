using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
using App.Controls;
using App.DAL;
using App.Utils;
using App.Components;
using App.Entities;

namespace App.Admins
{
    [UI("资源")]
    [Auth(AuthLogin=true)]
    [Param("cate", "目录", false)]
    [Param("key", "键值", false)]
    public partial class ResForm : FormPage<Res>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
            {
                this.img.Hidden = true;
                UI.BindEnum(this.rblType, typeof(ResType));
                ShowForm();
            }
        }

        public override void NewData()
        {
            UI.SetValue(tbKey, Asp.GetQueryString("key"));
            UI.SetValue<ResType>(rblType, null);
            UI.SetValue(tbName, "");
            UI.SetValue(tbFile, "");
            UI.SetValue(chkProtect, ArticleConfig.Instance.Protect);
        }

        public override void ShowData(Res item)
        {
            UI.SetValue(tbKey, item.Key);
            UI.SetValue(rblType, item.Type);
            UI.SetValue(tbName, item.FileName);
            UI.SetValue(tbFile, item.Content);
            UI.SetValue(chkProtect, item.Protect);
            if (item.Type == ResType.Image)
            {
                UI.SetValue(img, item.Content);
                this.img.Hidden = false;
            }
        }

        public override void CollectData(ref Res item)
        {
            item.Key = UI.GetText(tbKey);
            item.FileName = UI.GetText(tbName);
            item.Type = UI.GetEnum<ResType>(rblType);
            item.Content = UI.GetText(tbFile);
            item.Protect = UI.GetBool(chkProtect);
        }

        // 保存后更新窗体（BLL层做了一些处理）
        public override void SaveData(Res item)
        {
            item.Save();
            ShowData(item);
        }

        // 上传
        protected void uploader_FileSelected(object sender, EventArgs e)
        {
            // 保存目录和键
            string cate = Asp.GetQueryString("cate");
            string key = Asp.GetQueryString("key");

            string imageUrl = UI.UploadFile(uploader, cate, SiteConfig.Instance.SizeBigImage);
            UI.SetValue(this.img, imageUrl, true);
            UI.SetValue(this.tbFile, Asp.ResolveUrl(imageUrl));
            if (this.Mode == PageMode.Edit)
            {
                var data = this.GetData();
                data.Content = UI.GetUrl(this.img);
                data.Save();
            }

        }
    }
}
