using App.Utils;
using App.HttpApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using App.DAL;

namespace App.Apis
{
    [Scope("Mall")]
    [Description("商城相关接口")]
    [HttpApi.History("2019-04-01", "CJH", "Update")]
    public partial class ApiMall
    {
        //----------------------------------------------
        // 资产
        //----------------------------------------------
        [HttpApi("获取用户资产列表（含维保信息）", true)]
        public static APIResult GetAssets(long? userId, int pageIndex = 0, int pageSize = 10)
        {
            var user = Common.TryGetUser(userId, Powers.AssetView);
            var items = UserAsset.Search(userId: user.ID)
                .SortPage(t => t.CreateDt, false, pageIndex, pageSize)
                .ToList()
                .Cast(t => t.Export())
                ;
            return items.ToResult();
        }

        [HttpApi("获取资产详细信息（含维保信息）", true)]
        public static APIResult GetAsset(long assetId)
        {
            return UserAsset.GetDetail(assetId).ToResult();
        }

        [HttpApi("新增资产", true)]
        public static APIResult AddAsset(long? userId, string name, string serialNo, string remark, long? shopId, long? orderItemId, DateTime? insuranceStartDt)
        {
            var user = Common.TryGetUser(userId, Powers.AssetNew);
            var item = UserAsset.Add(user.ID, name, serialNo, remark, shopId, orderItemId, insuranceStartDt);
            return item.ToResult();
        }

        [HttpApi("编辑资产", true)]
        public static APIResult EditAsset(long assetId, long? userId, string name, string serialNo, string remark, long? shopId, DateTime? insuranceStartDt, DateTime? insuranceEndDt)
        {
            var user = Common.TryGetUser(userId, Powers.AssetEdit);
            var item = UserAsset.Get(assetId);
            var op = Common.LoginUser;
            item.AddHistory(op.ID, op.NickName, op.Mobile, "老数据", null, item.ExportJson());
            item.UserID = user.ID;
            item.Name = name;
            item.SerialNo = serialNo;
            item.Remark = remark;
            item.ShopID = shopId;
            item.InsuranceStartDt = insuranceStartDt;
            item.InsuranceEndDt = insuranceEndDt;
            item.Save();

            return item.ToResult();
        }

        //----------------------------------------------
        // 产品相关
        //----------------------------------------------
        [HttpApi("获取商品列表", false, 0)]
        public static APIResult GetProducts(ProductType? type, int pageIndex = 0, int pageSize = 10)
        {
            IQueryable<Product> q = Product.Search(type: type, onShelf: true);
            int count = q.Count();

            // 如果商品不存在，尝试建一个
            if (count == 0 && type != null)
            {
                var p = Product.CreateProducts(type, SiteConfig.Instance.DefaultProductImage);
                if (p.Count != 0)
                    return GetProducts(type, pageIndex, pageSize);
            }

            var items = q.Sort(t => t.CreateDt, false).Page(pageIndex, pageSize).ToList().Cast(t => t.Export());
            return new APIResult(true, "获取成功", items, new DataPager(count, pageIndex, pageSize));
        }

        [HttpApi("获取商品", false, 60)]
        public static APIResult GetProduct(long productId)
        {
            return Product.GetDetail(productId).ToResult(ExportMode.Detail, "获取成功", "无此商品");
        }

        [HttpApi("获取商品协议", false, 0, Type= ResponseType.HTML)]
        public static string GetProductProtocol(long productId)
        {
            var p = Product.GetDetail(productId);
            return p?.Protocol;
        }


        [HttpApi("获取商品规格", false, 0)]
        public static APIResult GetProductSpec(long productSpecId)
        {
            var p = ProductSpec.GetDetail(productSpecId);
            return p.ToResult(ExportMode.Detail, "获取成功", "无此商品规格");
        }

        //----------------------------------------------
        // 订单相关
        //----------------------------------------------
        [HttpApi("获取订单列表", true)]
        public static APIResult GetOrders(long? userId, long? shopId, int? status, int pageIndex = 0, int pageSize = 10)
        {
            if (userId == null)
            {
                var user = Common.TryGetUser(userId, Powers.OrderView);
                userId = user.ID;
            }
            IQueryable<Order> q = Order.Search(userId: userId, shopId: shopId, status: status).Sort(t => t.CreateDt, false);
            int count = q.Count();
            q = q.Page(pageIndex, pageSize);
            var items = q.ToList().Cast(t => t.Export());
            return new APIResult(true, "获取成功", items, new DataPager(count, pageIndex, pageSize));
        }

        [HttpApi("获取订单项列表", true)]
        public static APIResult GetOrder(long? orderId = null, string serialNo = "")
        {
            return Order.GetDetail(orderId, serialNo).ToResult(ExportMode.Detail, "获取成功", "无此订单");
        }

        [HttpApi("获取订单项详情", true)]
        public static APIResult GetOrderItem(long orderItemId)
        {
            return OrderItem.Get(orderItemId).ToResult(ExportMode.Detail, "获取成功", "无此订单项");
        }


    }
}