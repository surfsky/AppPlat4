using App.Utils;
using App.HttpApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using App.DAL;
using App.Wechats;
using App.Components;

namespace App.Apis
{
    /// <summary>
    /// 商城接口-流程相关
    /// </summary>
    public partial class ApiMall
    {
        //--------------------------------------------
        // 保险订单
        //--------------------------------------------
        [HttpApi("续保订单-创建", true)]
        public static APIResult InsuranceOrder_Create(long assetId, long productSpecId, long? shopId)
        {
            var asset = UserAsset.Get(assetId);
            if (asset == null)
                throw new Exception("无法找到该资产");

            var order = SimplyOrderCreate(productSpecId, shopId, ProductType.Insurance, InsuranceStatus.Create);
            var item = order.Items[0];
            if (asset != null && item != null)
            {
                OrderItemAsset oia = new OrderItemAsset();
                oia.OrderItemID = item.ID;
                oia.UserID = order.UserID;
                oia.AssetID = asset.ID;
                oia.CreateDt = DateTime.Now;
                oia.Save();
            }
            return Order.GetDetail(order.ID).ToResult();
        }

        /// <summary>移到/WeiXin/Pay.ashx 统一处理</summary>
        [HttpApi("续保订单-支付", true, Status = ApiStatus.Obsolete)]
        public static APIResult InsuranceOrder_Pay(long orderId)
        {
            Logger.LogDb("InsuranceOrderPay", orderId.ToString());
            var order = SimplyOrderChangeStatus(orderId, InsuranceStatus.UserPay, InsuranceStatus.Finish);
            var item = order?.Items[0];
            var spec = item.ProductSpec;
            var asset = item.GetAssets()[0];
            asset?.AddInsurance(spec);
            return Order.GetDetail(order.ID).ToResult();
        }


        //--------------------------------------------
        // 验真订单
        //--------------------------------------------
        [HttpApi("验真订单-创建", true)]
        public static APIResult CheckOrder_Create(long productSpecId, long? shopId)
        {
            return SimplyOrderCreate(productSpecId, shopId, ProductType.Check, CheckStatus.Create).ToResult(ExportMode.Detail, "创建成功");
        }

        [HttpApi("验真订单-支付", true)]
        public static APIResult CheckOrder_Pay(long orderId)
        {
            Logger.LogDb("CheckOrderPay", orderId.ToText());
            return SimplyOrderChangeStatus(orderId, (int)CheckStatus.UserPay).ToResult(ExportMode.Detail, "支付成功");
        }

        [HttpApi("验真订单-商家检测且完成订单", true)]
        public static APIResult CheckOrder_BizCheck(long orderId)
        {
            return SimplyOrderChangeStatus(orderId, (int)CheckStatus.BizCheck, (int)CheckStatus.Finish).ToResult(ExportMode.Detail, "检测完成");
        }



        
        //--------------------------------------------
        // 维修订单
        //--------------------------------------------
        [HttpApi("维修定单-创建", true, Remark ="客户自己先评估，后面商家会重新评估")]
        public static APIResult RepairOrder_Create(string device, string mobile, string remark, long productSpecId, DateTime? apptDt, long? shopId, string user="")
        {
            var o = SimplyOrderCreate(productSpecId, shopId, ProductType.Repair, RepairStatus.Create);
            o.ApptDevice = device;
            o.ApptMobile = mobile;
            o.ApptRemark = remark;
            o.ApptDt = apptDt;
            o.ApptUser = user;
            o.Save();

            return Order.GetDetail(o.ID).ToResult();
        }

        [HttpApi("维修定单-商家接单", true)]
        public static APIResult RepairOrder_BizAccept(long orderId, long? shopId=null, string remark = "")
        {
            var o = MallHelper.TryGetOrder(orderId);
            if (shopId != null)
            {
                o.ShopID = shopId;
                o.Save();
            }
            return MallHelper.TryChangeOrderStatus(orderId, RepairStatus.BizAccept, remark).ToResult();
        }



        [HttpApi("维修定单-商家维修中", true)]
        public static APIResult RepairOrder_BizRepair(long orderId, string remark = "", string image1 = "", string image2 = "", string image3 = "")
        {
            return MallHelper.TryChangeOrderStatus(orderId, RepairStatus.BizRepair, remark, image1, image2, image3).ToResult();
        }

        [HttpApi("维修定单-商家维修完毕", true)]
        public static APIResult RepairOrder_BizRepairOk(long orderId, string remark = "", string image1 = "", string image2 = "", string image3 = "")
        {
            return MallHelper.TryChangeOrderStatus(orderId, RepairStatus.BizRepairOk, remark, image1, image2, image3).ToResult();
        }

        [HttpApi("维修定单-商家已寄送维修", true)]
        public static APIResult RepairOrder_BizSend(long orderId, string remark = "", string image1 = "", string image2 = "", string image3 = "", 
            long? handleShopId = null)
        {
            var order = MallHelper.TryChangeOrderStatus(orderId, RepairStatus.BizSend, remark, image1, image2, image3);
            if (handleShopId != null)
                order.HandleShopID = handleShopId;
            order.Save();
            return order.ToResult();
        }


        [HttpApi("维修定单-寄送维修中", true)]
        public static APIResult RepairOrder_SendRepair(long orderId, string remark = "", string image1 = "", string image2 = "", string image3 = "")
        {
            var order = MallHelper.TryChangeOrderStatus(orderId, RepairStatus.SendRepair, remark, image1, image2, image3);
            return order.ToResult();
        }


        [HttpApi("维修定单-寄送维修完毕已寄回", true)]
        public static APIResult RepairOrder_SendRepairOk(long orderId, string remark = "", string image1 = "", string image2 = "", string image3 = "")
        {
            var order = MallHelper.TryChangeOrderStatus(orderId, RepairStatus.SendRepairOK, remark, image1, image2, image3);
            order.Save();
            return order.ToResult();
        }

        [HttpApi("维修定单-商家已取寄修件", true)]
        public static APIResult RepairOrder_BizReceive(long orderId, string remark = "", string image1 = "", string image2 = "", string image3 = "")
        {
            return MallHelper.TryChangeOrderStatus(orderId, RepairStatus.BizReceive, remark, image1, image2, image3).ToResult();
        }


        [HttpApi("维修定单-用户拿机", true)]
        public static APIResult RepairOrder_UserReceive(long orderId, string remark = "", string image1 = "", string image2 = "", string image3 = "")
        {
            var order = MallHelper.TryChangeOrderStatus(orderId, RepairStatus.UserReceive, remark, image1, image2, image3);
            order.Finish(); // 自动完成订单
            return MallHelper.TryChangeOrderStatus(orderId, RepairStatus.Finish).ToResult();
        }



        //--------------------------------------------
        // 订单流程
        //--------------------------------------------
        /// <summary>
        /// 微信预支付。预封装微信支付信息，供微信支付用。
        /// 微信支付成功后会回调 /WeiXin/Pay.ashx
        /// 各个分散的 Orderxxx_Pay() 接口都可以不用调用了
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpApi("订单微信预支付", true)]
        public static APIResult OrderPrepay(long orderId)
        {
            var order = MallHelper.TryGetOrder(orderId);
            var user = Common.LoginUser;
            //Logger.LogDb("WechatPrepay-Start", order.ExportJson(false));


            // 若用户具有测试员角色，将订单金额改为0.01元，并标注测试字样
            if (user.HasPower(Powers.Test))
            {
                order.PayMoney = 0.01;
                if (!order.Summary.Contains("（测试）"))
                    order.Summary = order.Summary + "（测试）";
                order.Save();
            }
            //Logger.LogDb("WechatPrepay-Ready", order.ExportJson(false));

            // 调用微信预支付接口
            var reply = Logic.WechatPrepay(WechatAppType.MP, user.WechatMPID, ref order);
            var result = new { Data = order.Export(ExportMode.Detail), Extra = reply };
            Logger.LogDb("WechatPrepay-Ok", result.ToJson(), user.NickName);
            return result.ToResult();
        }


        [HttpApi("订单取消", true)]
        public static APIResult OrderCancel(long orderId, long? userId)
        {
            var order = MallHelper.TryGetOrder(orderId);
            order = order.Cancel(Common.LoginUser.ID);
            return order.ToResult();
        }


        [HttpApi("订单评价", true)]
        public static APIResult OrderRate(long orderId, RateType rate, string comment)
        {
            var order = MallHelper.TryGetOrder(orderId);
            order = order.AddRate(rate, comment);
            return order.ToResult();
        }

        [HttpApi("获取评价", true)]
        public static APIResult GetRates(long? shopId, long? orderId)
        {
            if (shopId == null && orderId == null)
                return new APIResult(false, "请输入商店ID或订单ID");

            var rates = DAL.OrderRate.Search(shopId, orderId).ToList().Cast(t => t.Export(ExportMode.Detail));
            return rates.ToResult();
        }

        //--------------------------------------------
        // 订单项流程
        //--------------------------------------------
        [HttpApi("获取所有操作步骤", true)]
        public static APIResult WFGetFlow(WFType type)
        {
            var flow = Workflow.GetFlow(type).Export();
            return flow.ToResult();
        }

        [HttpApi("获取后继操作步骤", true)]
        public static APIResult WFGetNextSteps(long orderId)
        {
            var order = MallHelper.TryGetOrder(orderId);
            var items = order.GetNextSteps(Common.LoginUser).Cast(t => t.Export());
            return items.ToResult();
        }


        //--------------------------------------------
        // 公共方法
        //--------------------------------------------
        /// <summary>简单订单（只有一个订单子项）创建</summary>
        /// <param name="productSpecId">产品规格</param>
        /// <param name="shopId">店铺</param>
        /// <param name="productType">产品类别</param>
        /// <param name="statuses">要添加的订单行状态</param>
        private static Order SimplyOrderCreate<T>(long productSpecId, long? shopId, ProductType productType, params T[] statuses) where T : struct
        {
            var userId = Common.LoginUser.ID;
            var spec = ProductSpec.GetDetail(productSpecId);
            if (spec == null || spec.Product?.Type != productType)
                throw new Exception("找不到此产品");

            // 创建订单
            var o = Order.Create(productType, userId, shopId);
            var oi = o.AddItem(productSpecId, 1);
            foreach (var s in statuses)
                o.ChangeStatusEnum(s, userId);
            o = Order.GetDetail(o.ID).UpdateStat();
            Logger.LogDb("OrderCreate", o.ExportJson(ExportMode.Detail), Common.LoginUser.NickName);

            // 通知
            Messenger.SendToComet(CometMessageType.Order, o, "");
            return o;
        }

        /// <summary>修改简单订单（只有一个订单子项）状态。若状态不允许会抛出异常</summary>
        private static Order SimplyOrderChangeStatus<T>(long orderId, params T[] statuses) where T : struct
        {
            var userId = Common.LoginUser.ID;
            var order = Order.GetDetail(orderId);
            if (order == null)
                throw new Exception("未找到订单");
            if (order.UserID != userId && Common.CheckPower(Powers.OrderEdit))
                throw new Exception("您无权修改他人订单");

            // 取消
            foreach (var s in statuses)
                order.ChangeStatusEnum(s, userId);
            order = Order.GetDetail(orderId);
            return order;
        }


    }
}