using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Components;
using App.Utils;
using App.DAL;
using App.Wechats;
using App.Wechats.MP;
using App.Wechats.OP;
using App.Wechats.Pay;

namespace App
{
    /// <summary>
    /// 商务逻辑类。无法明确归属的、跨实体的逻辑可放在此处
    /// </summary>
    public partial class Logic
    {
        /// <summary>微信配置</summary>
        private static void InitWechat()
        {
            Wechats.WechatConfig.OPAppId       = App.DAL.WechatConfig.Instance.OPAppID;
            Wechats.WechatConfig.OPAppSecret   = App.DAL.WechatConfig.Instance.OPAppSecret;
            Wechats.WechatConfig.OPPayUrl      = App.DAL.WechatConfig.Instance.OPPayUrl;
            Wechats.WechatConfig.OPPushKey     = App.DAL.WechatConfig.Instance.OPPushKey;
            Wechats.WechatConfig.OPPushToken   = App.DAL.WechatConfig.Instance.OPPushToken;
            Wechats.WechatConfig.OPTokenServer = App.DAL.WechatConfig.Instance.OPTokenServer;
            Wechats.WechatConfig.MPAppId       = App.DAL.WechatConfig.Instance.MPAppID;
            Wechats.WechatConfig.MPAppSecret   = App.DAL.WechatConfig.Instance.MPAppSecret;
            Wechats.WechatConfig.MPPayUrl      = App.DAL.WechatConfig.Instance.MPPayUrl;
            Wechats.WechatConfig.MPPushKey     = App.DAL.WechatConfig.Instance.MPPushKey;
            Wechats.WechatConfig.MPPushToken   = App.DAL.WechatConfig.Instance.MPPushToken;
            Wechats.WechatConfig.MPTokenServer = App.DAL.WechatConfig.Instance.MPTokenServer;
            Wechats.WechatConfig.MchId         = App.DAL.WechatConfig.Instance.MchId;
            Wechats.WechatConfig.MchKey        = App.DAL.WechatConfig.Instance.MchKey;

            // 微信接口事件
            Wechats.WechatConfig.Instance.OnLog += (name, user, request, reply) =>
            {
                var msg = (new { Request = request, Reply = reply }).ToJson();
                Logger.LogDb(name, msg, user);
            };
        }

        //---------------------------------------------
        // 微信订单预支付
        //---------------------------------------------
        /// <summary>微信预支付（金额用PayMoney字段）</summary>
        public static UnifiedOrderReply WechatPrepay(WechatAppType type, string openId, ref Order o)
        {
            var appId     = Wechats.WechatConfig.GetAppId(type);
            var appSecret = Wechats.WechatConfig.GetAppSecret(type);
            var payUrl    = Wechats.WechatConfig.GetPayUrl(type);
            //Logger.LogDb("WechatUnifiedOrder-Start", o.ExportJson(false));

            // 微信 UnifyPay
            o.FixPayMoney();
            var ip = Asp.ClientIP;
            var deviceInfo = "";
            //Logger.LogDb("WechatUnifiedOrder-Fix", o.ExportJson(false));
            var reply = WechatPay.UnifiedOrder(appId, appSecret, payUrl, o.Summary, o.PayMoney.Value, openId, o.SerialNo, ip, deviceInfo);
            o.PrepayId = reply.prepay_id;
            o.Save();
            Logger.LogDb("WechatUnifiedOrder-Ok", reply.ToJson());


            // 输出
            return reply;
        }


        //---------------------------------------------
        // 微信邀请消息
        //---------------------------------------------
        /// <summary>发送微信邀请成功消息</summary>
        public static void SendWechatInviteMessage(Invite invite, bool toInviterOrShop)
        {
            if (invite == null)  return;
            if (toInviterOrShop)
            {
                // 给邀请用户发消息
                var inviter = User.Get(invite.InviterID);
                OPENSendWechatInviteMessage(invite, inviter);
                Logger.LogDb("SendInviteMsgToUser", invite.ExportJson());
            }
            else
            {
                // 给邀请门店发消息
                //Logger.LogDb("SendInviteMsgToShop-start", invite.ExportJson(false));
                var shopOwners = User.SearchShopOwners(invite.InviteShopID);
                foreach (var owner in shopOwners)
                {
                    OPENSendWechatInviteMessage(invite, owner);
                    Logger.LogDb("SendInviteMsgToShop", owner.ExportJson());
                }
            }
        }

        /// <summary>发送微信邀请成功消息（公众号）</summary>
        /// <remarks>
        /// https://mp.weixin.qq.com/advanced/tmplmsg?action=edit&id=KAXUYy48a4rDWbyY0kgAuQkYWCsSRfm09AprzexHYhE&token=279566585&lang=zh_CN
        /// 模版ID: KAXUYy48a4rDWbyY0kgAuQkYWCsSRfm09AprzexHYhE
        /// 标题: 新增成员提醒
        /// 详细内容
        ///     {{first.DATA}}
        ///     新增成员：{{keyword1.DATA}}
        ///     加入时间：{{keyword2.DATA}}
        ///     邀请人：{{keyword3.DATA}}
        ///     {{remark.DATA}}
        /// </remarks>
        static WechatReply OPENSendWechatInviteMessage(Invite invite, User receiver)
        {
            var user = User.Get(invite.InviteeID);
            var msg = new TMessage(
                "KAXUYy48a4rDWbyY0kgAuQkYWCsSRfm09AprzexHYhE",
                "",
                first: "邀请成功",
                remark: invite.InviteShop?.Name,
                keyword1: user?.NickName,
                keyword2: user?.CreateDt?.ToString("yyyy-MM-dd HH:mm:ss"),
                keyword3: invite.Inviter?.NickName
                );
            return WechatOP.SendTMessage(receiver.WechatOPID, msg);
        }
        


        //---------------------------------------------
        // 微信订单消息
        //---------------------------------------------
        /// <summary>发送微信订单消息（公众号+小程序）</summary>
        public static int SendWechatOrderMessage(Order order)
        {
            if (order == null)
                return 0;

            // 顾客、商家、维修商用户
            var customer = User.Get(order.UserID);
            var shopOwners = User.SearchShopOwners(order.ShopID);
            var repairers = User.SearchShopOwners(order.HandleShopID);
            var status = order.Status;

            // 给顾客、接单商店、维修商店发订单状态变更消息
            OPENSendOrderMessage(order, customer);
            shopOwners.ForEach(t => OPENSendOrderMessage(order, t));
            repairers.ForEach(t => OPENSendOrderMessage(order, t));
            return 1;
        }

        /// <summary>发送微信公众号订单消息</summary> 
        /// <remarks>
        /// https://mp.weixin.qq.com/advanced/tmplmsg?action=edit&id=0TPHe0_CFT8DfkwLO75lzm8Vb1K3PREpT6SG6woB10A&token=279566585&lang=zh_CN
        /// 模版ID: 0TPHe0_CFT8DfkwLO75lzm8Vb1K3PREpT6SG6woB10A
        /// 标题：订单状态更新通知
        /// 内容
        ///     {{first.DATA}}
        ///     商家名称：{{keyword1.DATA}}
        ///     商家电话：{{keyword2.DATA}}
        ///     订单号：{{keyword3.DATA}}
        ///     状态：{{keyword4.DATA}}
        ///     总价：{{keyword5.DATA}}
        ///     {{remark.DATA}}
        /// </remarks>
        static WechatReply OPENSendOrderMessage(Order order, User receiver)
        {
            if (order == null || receiver == null || receiver.WechatOPID.IsEmpty())
                return null;

            var user = User.Get(order.UserID);
            var name = user.RealName.IsEmpty() ? user.NickName : user.RealName;
            var msg = new TMessage(
                "0TPHe0_CFT8DfkwLO75lzm8Vb1K3PREpT6SG6woB10A",
                "",
                first:    order.Summary,
                remark:   string.Format("{0}，{1:yyyy-MM-dd HH:mm:ss}", name, order.StatusDt),
                keyword1: order.Shop?.Name,
                keyword2: order.Shop?.Tel,
                keyword3: order.SerialNo,
                keyword4: order.StatusName,
                keyword5: "￥" + order.TotalMoney?.ToText()
                );
            return WechatOP.SendTMessage(receiver.WechatOPID, msg);
        }

        /// <summary>发送微信小程序订单消息</summary>
        /// <remarks>
        /// https://mp.weixin.qq.com/wxopen/tmplmsg?action=self_detail&tmpl_id=ByW6hs7EpOnmD23XGpHrZwzGdCOu3VOcHOrVFAItU7w&token=812622028&lang=zh_CN
        /// 模板ID: ByW6hs7EpOnmD23XGpHrZwzGdCOu3VOcHOrVFAItU7w
        /// 标题: 订单状态通知
        /// 内容
        ///     订单内容{{keyword1.DATA}}
        ///     订单状态{{keyword2.DATA}}
        ///     下单时间{{keyword3.DATA}}
        ///     订单金额{{keyword4.DATA}}
        ///     备注    {{keyword5.DATA}}
        ///     通知时间{{keyword6.DATA}}
        /// </remarks>
        static WechatReply MPSendOrderMessage(Order order, User receiver)
        {
            if (order == null || receiver == null || receiver.WechatMPID.IsEmpty())
                return null;

            var user = User.Get(order.UserID);
            var msg = new TMessage(
                "ByW6hs7EpOnmD23XGpHrZwzGdCOu3VOcHOrVFAItU7w",
                string.Format("pages/order/detail?orderId={0}", order.ID),
                first: "",
                remark:  "",
                keyword1: order.Summary,
                keyword2: order.StatusName,
                keyword3: order.CreateDt.ToText("yyyy-MM-dd HH:mm"),
                keyword4: "￥" + order.TotalMoney.ToText(),
                keyword5: user?.NickName,
                keyword6: order.StatusDt.ToText("yyyy-MM-dd HH:mm")
                );
            // 获取formId
            var openId = receiver.WechatMPID;
            var formId = WechatMPForm.GetForm(openId: openId)?.FormID;
            return WechatMP.SendTMessage(openId, msg, formId);
        }
    }
}
