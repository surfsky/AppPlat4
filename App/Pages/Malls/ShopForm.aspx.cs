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
    [UI("商店")]
    [Auth(Powers.ShopView, Powers.ShopEdit, Powers.ShopEdit, Powers.ShopDelete)]
    public partial class ShopForm : FormPage<Shop>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
            {
                UI.BindTree(ddlCity, Area.All, t => t.ID, t => t.Name);
                UI.SetText(uploader, SiteConfig.Instance.SizeBigImage);
                ShowForm();
            }
        }

        public override void NewData()
        {
            this.ddlCity.SelectedIndex = -1;
            this.tbName.Text = "";
            this.tbAbbrName.Text = "";
            this.tbTel.Text = "";
            this.tbAddr.Text = "";
            this.pbGPS.Text = "";
            this.imgPhoto.ImageUrl = SiteConfig.Instance.DefaultShopImage;
            this.tbDescription.Text = "";
        }

        public override void ShowData(Shop item)
        {
            this.ddlCity.SelectedValue = item.AreaID.ToText();
            this.tbName.Text = item.Name;
            this.tbAbbrName.Text = item.AbbrName;
            this.tbTel.Text = item.Tel;
            this.tbAddr.Text = item.Addr;
            this.pbGPS.Text = item.GPS;
            this.imgPhoto.ImageUrl = item.CoverImage;
            this.tbDescription.Text = item.Description;

            // 邀请
            var mpPage = string.Format("/pages/index/index?inviteShopId={0}", item.ID).UrlEncode();
            var openPage = string.Format("?inviteShopId={0}", item.ID).UrlEncode();
            var mpQrCode = string.Format("/HttpApi/Wechat/MPQrCode?page={0}&width={1}", mpPage, 280);
            var openQrCode = string.Format("/HttpApi/Wechat/OPENQrCode?page={0}&width={1}", openPage, 280);
            this.imgMPInvite.ImageUrl = mpQrCode;
            this.imgWebInvite.ImageUrl = openQrCode;
        }

        public override void CollectData(ref Shop item)
        {
            item.AreaID      = UI.GetLong(ddlCity);
            item.Name        = UI.GetText(tbName);
            item.AbbrName    = UI.GetText(tbAbbrName);
            item.Tel         = UI.GetText(tbTel);
            item.Addr        = UI.GetText(tbAddr);
            item.GPS         = UI.GetText(pbGPS);
            item.CoverImage  = UI.GetUrl(imgPhoto);
            item.Description = UI.GetText(tbDescription);
        }

        // 图片上传
        protected void uploader_FileSelected(object sender, EventArgs e)
        {
            string imageUrl = UI.UploadFile(uploader, "Shops", SiteConfig.Instance.SizeBigImage);
            UI.SetValue(this.imgPhoto, imageUrl, true);
            if (this.Mode == PageMode.Edit)
            {
                var data = this.GetData();
                data.CoverImage = UI.GetUrl(this.imgPhoto);
                data.Save();
            }
        }

    }
}
