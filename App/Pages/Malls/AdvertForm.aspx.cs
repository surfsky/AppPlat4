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

namespace App.Pages
{
    /// <summary>
    /// 未完成，需实现 key 的选择弹窗
    /// </summary>
    [UI("广告")]
    [Auth(Powers.AdvertView, Powers.AdvertNew, Powers.AdvertEdit, Powers.AdvertDelete)]
    public partial class AdvertForm : FormPage<Advert>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.Form2, this.Form2);
            if (!IsPostBack)
            {
                UI.BindEnum(ddlLocation, typeof(AdvertPlace));
                UI.BindEnum(ddlStatus, typeof(AdvertStatus));
                UI.SetText(uploader, SiteConfig.Instance.SizeAppBanner);
                ShowForm();
            }
        }

        public override void NewData()
        {
            UI.SetValue<Shop>(pbShop, null, null, null);
            UI.SetValue<Product>(pbProduct, null, null, null);
            UI.SetValue<Article>(pbArticle, null, null, null);
            UI.SetValue(this.tbContent, "");
            UI.SetValue(this.ddlStatus, null);
            UI.SetValue(this.ddlLocation, -1);
            UI.SetValue(this.tbTitle, "");
            UI.SetValue(this.tbSeq, "");
            UI.SetValue(this.img, SiteConfig.Instance.DefaultAppBannerImage);
            UI.SetValue(this.dpCreate, DateTime.Now);
            UI.SetValue(this.dpStart, DateTime.Now.TrimDay());
            UI.SetValue(this.dpEnd, DateTime.Now.TrimDay().AddYears(10));
        }

        public override void ShowData(Advert item)
        {
            UI.SetValue(this.ddlLocation, item.Place);
            UI.SetValue<Shop>(pbShop, item.Shop, t=>t.ID, t=>t.AbbrName);
            UI.SetValue<Product>(pbProduct, item.Product, t=>t.ID, t=>t.Name);
            UI.SetValue<Article>(pbArticle, item.Article, t => t.ID, t => t.Title);
            UI.SetValue(this.tbContent, item.Body);
            UI.SetValue(this.ddlStatus, item.Status);
            UI.SetValue(this.img, item.CoverImage);
            UI.SetValue(this.tbTitle, item.Title);
            UI.SetValue(this.tbSeq, item.Seq.ToString());
            UI.SetValue(this.dpCreate, item.CreateDt);
            UI.SetValue(this.dpStart, item.StartDt);
            UI.SetValue(this.dpEnd, item.EndDt);
        }

        public override void CollectData(ref Advert item)
        {
            item.Place = UI.GetEnum<AdvertPlace>(ddlLocation);
            item.Title = UI.GetText(this.tbTitle);
            item.Body = UI.GetText(this.tbContent);
            item.ShopID = UI.GetLong(this.pbShop);
            item.ProductID = UI.GetLong(this.pbProduct);
            item.ArticleID = UI.GetLong(this.pbArticle);
            item.Seq = UI.GetInt(this.tbSeq, 0).Value;
            item.Status = UI.GetEnum<AdvertStatus>(this.ddlStatus);
            item.CoverImage = UI.GetUrl(this.img);
            item.StartDt = UI.GetDate(this.dpStart);
            item.EndDt = UI.GetDate(this.dpEnd);
        }

        // 图片上传
        protected void uploader_FileSelected(object sender, EventArgs e)
        {
            string imageUrl = UI.UploadFile(uploader, "Adverts", SiteConfig.Instance.SizeAppBanner);
            UI.SetValue(this.img, imageUrl, true);
            if (this.Mode == PageMode.Edit)
            {
                var data = this.GetData();
                data.CoverImage = imageUrl;
                data.Save();
            }
        }

    }
}
