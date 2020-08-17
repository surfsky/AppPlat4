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
    [UI("商品")]
    [Auth(Powers.ProductView, Powers.ProductNew, Powers.ProductEdit, Powers.ProductDelete)]
    public partial class ProductForm : FormPage<Product>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 产品概述信息
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
            {
                var allShops = Shop.Search().Sort(t => t.Name).ToList();
                UI.Bind(ddlShop, allShops, t => t.ID, t => t.AbbrName);
                UI.BindBool(rblOnShelf, "上架", "下架");
                UI.BindEnum(rblType, typeof(ProductType), null);
                UI.SetText(uploader, SiteConfig.Instance.SizeBigImage);
                ShowForm();
            }
        }

        //---------------------------------------------
        //
        //---------------------------------------------
        public override void NewData()
        {
            UI.SetValue(tbID, "-1");
            UI.SetValue(tbName, "");
            UI.SetValue(tbBarCode, "");
            UI.SetValue(rblOnShelf, true);
            UI.SetValue<ProductType>(rblType, null);
            UI.SetValue(tbSaleCnt, 0);
            UI.SetValue(tbCreateDt, DateTime.Now);
            UI.SetValue(tbDescription, "");
            UI.SetValue(imgPhoto, SiteConfig.Instance.DefaultProductImage);
            UI.SetValue(tbPositiveCnt, 0);
            UI.SetValue(tbProtocol, "");
            UI.SetValue(ddlShop, null);

            UI.SetValue(tbPrice, 0.0d);
            UI.SetValue(tbRawPrice, 0.0d);
            UI.SetValue(lblDiscount, "");
            UI.SetValue(tbStock, 999);

            this.panDetail.Hidden = true;
        }

        public override void ShowData(Product item)
        {
            this.ddlShop.Enabled = Common.LoginUser.HasPower(Powers.Admin);

            UI.SetValue(tbID, item.ID.ToString());
            UI.SetValue(tbName, item.Name);
            UI.SetValue(tbBarCode, item.BarCode);
            UI.SetValue(rblOnShelf, item.OnShelf);
            UI.SetValue(rblType, item.Type);
            UI.SetValue(tbSaleCnt, item.SaleCnt);
            UI.SetValue(tbCreateDt,item.CreateDt);
            UI.SetValue(tbDescription, item.Description);
            UI.SetValue(imgPhoto, item.CoverImage);
            UI.SetValue(tbPositiveCnt, item.PositiveCnt);
            UI.SetValue(tbSpecName1, item.SpecName1);
            UI.SetValue(tbSpecName2, item.SpecName2);
            UI.SetValue(tbSpecName3, item.SpecName3);
            UI.SetValue(tbProtocol, item.Protocol);
            UI.SetValue(ddlShop, item.ShopID);

            UI.SetValue(tbPrice, item.Price);
            UI.SetValue(tbRawPrice, item.RawPrice);
            UI.SetValue(lblDiscount, item.Discount);
            UI.SetValue(tbStock, item.Stock);

            this.panDetail.IFrameUrl = string.Format("ProductSpecs.aspx?productId={0}", item.ID);
            this.panDetail.Hidden = false;
        }

        public override void CollectData(ref Product item)
        {
            item.Name = UI.GetText(tbName);
            item.BarCode = UI.GetText(tbBarCode);
            item.OnShelf = UI.GetBool(rblOnShelf, true);
            item.Type = UI.GetEnum<ProductType>(rblType);
            item.SaleCnt = UI.GetInt(tbSaleCnt, 0);
            item.CreateDt = UI.GetDate(tbCreateDt);
            item.Description = UI.GetText(tbDescription);
            item.CoverImage = UI.GetUrl(imgPhoto);
            item.PositiveCnt = UI.GetInt(tbPositiveCnt, 0);
            item.SpecName1 = UI.GetText(tbSpecName1);
            item.SpecName2 = UI.GetText(tbSpecName2);
            item.SpecName3 = UI.GetText(tbSpecName3);
            item.Protocol = UI.GetText(tbProtocol);
            item.ShopID = UI.GetLong(ddlShop);

            item.Price = UI.GetDouble(tbPrice, 0);
            item.RawPrice = UI.GetDouble(tbRawPrice, 0);
            item.Stock = UI.GetInt(tbStock, 0);
        }

        // 保存后显示折扣
        public override void SaveData(Product item)
        {
            item.Save();
            UI.SetValue(this.lblDiscount, item.Discount);
            this.panDetail.RefreshIFrame();
        }

        // 图片上传
        protected void uploader_FileSelected(object sender, EventArgs e)
        {
            string imageUrl = UI.UploadFile(uploader, "Products", SiteConfig.Instance.SizeBigImage);
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
