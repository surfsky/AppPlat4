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
    [UI("订单细项")]
    [Auth(Powers.OrderView, Powers.OrderEdit, Powers.OrderEdit, Powers.OrderEdit)]
    [Param("userId", "用户ID", false)]
    [Param("orderId", "订单ID", false)]
    [Param("productType", "产品类别", false)]
    public partial class OrderItemForm : FormPage<OrderItem>
    {
        /// <summary>所有产品</summary>
        public static List<Product> AllProducts
        {
            get { return Product.Search(onShelf: true).Sort(t => t.Name).ToList(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.Panel1);
            if (!IsPostBack)
            {
                UI.Bind(ddlProduct, AllProducts, t => t.ID, t => t.Name);
                ShowForm();
            }
        }

        // 新增
        public override void NewData()
        {
            var userId = Asp.GetQueryLong("userId");
            var orderId = Asp.GetQueryLong("orderId");
            if (userId == null && orderId == null)
            {
                Asp.Fail("缺少 userId, orderId 参数");
                return;
            }

            // 限制产品类别
            var productType = Order.Get(orderId.Value).Type; // Asp.GetQueryEnumValue<ProductType>("productType");
            if (productType != null)
            {
                UI.Bind(ddlProduct, AllProducts.Where(t => t.Type == productType), t => t.ID, t => t.Name);
                UI.SetValue(ddlProduct, productType);
                //this.ddlProduct.Readonly = true;
                ddlProduct_SelectedIndexChanged(null, null);
            }

            //
            UI.SetValue(ddlProductSpec, null);
            UI.SetValue(this.tbTitle, "");
            UI.SetValue(this.tbPrice, 0.0);
            UI.SetValue(this.tbAmount, 1);
            UI.SetValue(this.tbMoney, 0.0);
            UI.SetValue(this.tbProductNo, "");
            this.panAsset.Hidden = true;
        }

        // 显示
        public override void ShowData(OrderItem item)
        {
            UI.SetValue(ddlProduct, item.ProductID);
            UI.SetValue(ddlProductSpec, item.ProductSpecID);
            UI.SetValue(this.tbTitle, item.Title);
            UI.SetValue(this.tbPrice, item.Price);
            UI.SetValue(this.tbAmount, item.Amount);
            UI.SetValue(this.tbMoney, item.Money);
            UI.SetValue(this.tbProductNo, item.ProductSpecCode);


            // 触发
            ddlProduct_SelectedIndexChanged(null, null);
            ddlProductSpec_SelectedIndexChanged(null, null);

            // 显示资产面板
            var order = Order.Get(item.OrderID);
            this.panAsset.Hidden = false;
            this.panAsset.IFrameUrl = string.Format("OrderItemAssets.aspx?search=false&orderItemId={0}&userId={1}", item.ID, order.UserID);
        }

        // 采集数据
        public override void CollectData(ref OrderItem item)
        {
            if (this.Mode == PageMode.New)
                item.OrderID = Asp.GetQueryLong("orderId").Value;

            item.ProductID = UI.GetLong(this.ddlProduct).Value;          // 必选
            item.ProductSpecID = UI.GetLong(this.ddlProductSpec).Value;  // 必选
            item.ProductType = Product.Get(item.ProductID).Type;
            item.Title = UI.GetText(this.tbTitle);
            item.Price = UI.GetDouble(this.tbPrice, 0);
            item.Amount = UI.GetInt(this.tbAmount, 1);
            item.Money = UI.GetDouble(this.tbMoney, 0);
            item.ProductSpecCode = UI.GetText(this.tbProductNo);
        }


        // 保存
        public override void SaveData(OrderItem item)
        {
            if (item.ProductID == null || item.ProductSpecID == null)
                throw new Exception("必须选择产品");

            var isNew = (this.Mode == PageMode.New);
            item.Save(isNew);
            // TODO: 要想办法通知 OrderForm 页面刷新
        }


        //---------------------------------------------
        // 页面逻辑控制
        //---------------------------------------------
        protected void ddlProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = UI.GetLong(this.ddlProduct);
            if (id != null)
            {
                var product = Product.Get(id.Value);
                var productSpecs = ProductSpec.Search(id);
                var statusType = product.Type.GetTitle();
                UI.Bind(ddlProductSpec, productSpecs, t => t.ID, t => t.Name);
                Calc();
            }
        }


        protected void ddlProductSpec_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calc();
        }

        protected void tbAmount_TextChanged(object sender, EventArgs e)
        {
            Calc();
        }

        // 自动计算出费用
        private void Calc()
        {
            var product = this.ddlProduct.SelectedText;
            var id = UI.GetLong(this.ddlProductSpec);
            if (id != null)
            {
                // 显示商品规格
                var item = ProductSpec.Get(id.Value);
                UI.SetValue(tbPrice, item.Price);
                UI.SetValue(tbStock, item.Stock);
                UI.SetValue(tbProductNo, item.Code);

                // 判断库存
                var amount = UI.GetInt(tbAmount, 1);
                if (amount > item.Stock)
                    this.tbAmount.MarkInvalid("库存不足");
                else
                {
                    // 计算金额
                    this.tbTitle.Text = string.Format("{0}（{1}） x{2}", product, item?.Name, amount);
                    this.tbMoney.Text = (item.Price * amount).ToText();
                }
            }
        }

    }
}
