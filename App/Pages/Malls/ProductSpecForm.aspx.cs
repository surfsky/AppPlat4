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
    [UI("商品规格")]
    [Auth(Powers.ProductView, Powers.ProductEdit, Powers.ProductEdit, Powers.ProductEdit)]
    [Param("productId", "商品ID")]
    [Param("id", "ID")]
    public partial class ProductSpecForm : FormPage<ProductSpec>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
            {
                // 设置产品相关信息
                Product product = null;
                var productId = Asp.GetQueryLong("productId");
                var specId = Asp.GetQueryLong("id");
                if (productId != null)
                    product = Product.Get(productId.Value);
                else if (specId != null)
                    product = ProductSpec.GetDetail(specId.Value)?.Product;
                if (product != null)
                {
                    this.tbSpec1.Label = product.SpecName1.IsEmpty() ? "规格1" : product.SpecName1;
                    this.tbSpec2.Label = product.SpecName2.IsEmpty() ? "规格2" : product.SpecName2;
                    this.tbSpec3.Label = product.SpecName3.IsEmpty() ? "规格3" : product.SpecName3;
                }

                //
                UI.SetText(uploader, SiteConfig.Instance.SizeBigImage);
                ShowForm();
            }
        }

        public override void NewData()
        {
            UI.SetValue(tbSpec1, "");
            UI.SetValue(tbSpec2, "");
            UI.SetValue(tbSpec3, "");
            UI.SetValue(imgPhoto, SiteConfig.Instance.DefaultProductImage);
            UI.SetValue(tbPrice, 0.0d);
            UI.SetValue(tbRawPrice, 0.0d);
            UI.SetValue(lblDiscount, "");
            UI.SetValue(tbStock, 999);
            UI.SetValue(tbData, 1);
            UI.SetValue(tbSeq, 0);
            UI.SetValue(tbInsuranceDays, 365);
        }

        public override void ShowData(ProductSpec item)
        {
            UI.SetValue(tbSpec1, item.Spec1);
            UI.SetValue(tbSpec2, item.Spec2);
            UI.SetValue(tbSpec3, item.Spec3);
            UI.SetValue(imgPhoto, item.CoverImage);
            UI.SetValue(tbPrice, item.Price);
            UI.SetValue(tbRawPrice, item.RawPrice);
            UI.SetValue(lblDiscount, item.Discount);
            UI.SetValue(tbStock, item.Stock);
            UI.SetValue(tbData, item.Data);
            UI.SetValue(tbSeq, item.Seq);
            UI.SetValue(tbInsuranceDays, item.InsuranceDays);
        }

        public override void CollectData(ref ProductSpec item)
        {
            if (this.Mode == PageMode.New)
                item.ProductID = Asp.GetQueryLong("productId").Value;
            item.Spec1 = UI.GetText(tbSpec1);
            item.Spec2 = UI.GetText(tbSpec2);
            item.Spec3 = UI.GetText(tbSpec3);
            item.CoverImage = UI.GetUrl(imgPhoto);
            item.Price = UI.GetDouble(tbPrice, 0);
            item.RawPrice = UI.GetDouble(tbRawPrice, 0);
            item.Stock = UI.GetInt(tbStock, null);
            item.Data = UI.GetInt(tbData, 1);
            item.Seq = UI.GetInt(tbSeq, 0);
            item.InsuranceDays = UI.GetInt(tbInsuranceDays, null);
        }

        // 保存后显示折扣
        public override void SaveData(ProductSpec item)
        {
            item.Save();
            UI.SetValue(this.lblDiscount, item.Discount);
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
