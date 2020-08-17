using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using EntityFramework.Extensions;
using System.Data.Entity;
using System.ComponentModel;
using App.Utils;
using App.Components;
using App.Wechats.OP;
using App.Entities;

namespace App.DAL
{
    /// <summary>邀请来源</summary>
    public enum InviteSource : int
    {
        [UI("地推")]  Offline = 0,
        [UI("网站")]  Web = 1,
        [UI("微信")]  Wechat = 2,
        [UI("App")]   App  = 3
    }

    /// <summary>邀请状态</summary>
    public enum InviteStatus : int
    {
        [UI("无效邀请")]  Invalid = -1,
        [UI("新邀请")]    New = 0,
        [UI("已注册")]    Regist = 2
    }

    /// <summary>
    /// 邀请信息表
    /// (0) 商店和员工都可以邀请，线下结算
    /// (1）该表可用于推广营销登记用，用于推广结算
    ///（2）推荐时先查找该用户手机是否已经注册；再查找本表看该手机是否已经被别人推荐了；实在没有才新增一条记录。
    /// 该表做邀请日志用，最终邀请数据以用户表字段 ShopID 和 InviterID 为准
    /// 一个用户可能存在多个邀请记录，inviteStatus=Invalid 是无效记录
    /// </summary>
    [UI("商城", "邀请")]
    public class Invite : EntityBase<Invite>
    {
        // 邀请基础信息
        [UI("来源")]                           public InviteSource? Source { get; set; }
        [UI("状态")]                           public InviteStatus? Status { get; set; }
        [UI("新用户注册日期")]                 public DateTime? RegistDt { get; set; }
        [UI("备注")]                           public string Remark {get; set;}

        // 邀请商店
        [UI("邀请商店")]                       public long? InviteShopID { get; set; }

        // 邀请人
        [UI("邀请人")]                         public long? InviterID { get; set; }

        // 受邀者
        [UI("受邀者账户")]                     public long? InviteeID { get; set; }
        [UI("受邀者手机")]                     public string InviteeMobile { get; set; }
        
        // 是否已奖励
        [UI("已奖励商店")]                     public bool? InviteShopAwarded { get; set; } = false;
        [UI("已奖励个人")]                     public bool? InviterAwarded { get; set; } = false;

        // 名称
        [UI("来源")] public string SourceName { get { return this.Source.GetTitle(); } }
        [UI("状态")] public string StatusName { get { return this.Status.GetTitle(); } }

        // 导航属性
        public virtual Shop InviteShop { get; set; }
        public virtual User Inviter { get; set; }
        public virtual User Invitee { get; set; }


        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        /// <summary>获取导出对象</summary>
        public override object Export(ExportMode type=ExportMode.Normal)
        {
            return new
            {
                this.ID,

                // inviteShop
                this.InviteShopID,
                InviteShopName = this.InviteShop?.Name,
                InviteShopAbbrName = this.InviteShop?.AbbrName,

                // inviter
                this.InviterID,
                InviterName = this.Inviter?.NickName,
                InviterPhoto = this.Inviter?.Avatar,
                InviterMobile = this.Inviter?.Mobile,

                // invitee
                this.InviteeID,
                this.InviteeMobile,
                InviteeName = this.Invitee?.NickName,
                InviteePhoto = this.Invitee?.Avatar,

                //
                this.CreateDt,
                this.RegistDt,
                this.Source,
                SourceName = this.Source?.GetTitle(),
                this.Status,
                StatusName = this.Status?.GetTitle(),

                // 奖励状态
                this.InviteShopAwarded,
                this.InviterAwarded
            };
        }


        /// <summary>获取详情</summary>
        public new static Invite GetDetail(long id)
        {
            return Set.Include(t => t.Inviter).Include(t => t.Invitee).Include(t => t.InviteShop)
                .Where(t => t.ID == id).FirstOrDefault();
        }


        /// <summary>查找用户邀请记录（根据受邀者手机或账号ID）</summary>
        public static Invite GetUserInvite(long? inviteeId, string inviteeMobile)
        {
            if (inviteeId != null)           return Search(inviteeId: inviteeId).FirstOrDefault(t => t.InviterID != null);
            if (inviteeMobile.IsNotEmpty())  return Search(inviteeMobile: inviteeMobile).FirstOrDefault(t => t.InviterID != null);
            return null;
        }

        /// <summary>查找商店邀请记录（根据受邀者手机或账号ID）</summary>
        public static Invite GetShopInvite(long? inviteeId, string inviteeMobile)
        {
            if (inviteeId != null)           return Search(inviteeId: inviteeId).FirstOrDefault(t => t.InviteShopID != null);
            if (inviteeMobile.IsNotEmpty())  return Search(inviteeMobile: inviteeMobile).FirstOrDefault(t => t.InviteShopID != null);
            return null;
        }

        // 查询
        public static IQueryable<Invite> Search(
            long? inviteShopId = null,
            long? inviterID = null, string inviterName = null, string inviterMobile = null,
            long? inviteeId = null, string inviteeName = null, string inviteeMobile = null,
            DateTime? createStartDt = null, DateTime? createEndDt = null,
            InviteSource? source = null, InviteStatus? status = null
            )
        {
            IQueryable<Invite> q = Set.Include(t => t.Inviter).Include(t => t.Invitee).Include(t => t.InviteShop);
            if (inviteShopId != null)          q = q.Where(t => t.InviteShopID == inviteShopId);

            //
            if (inviterID != null)              q = q.Where(t => t.InviterID == inviterID);
            if (inviterName.IsNotEmpty())       q = q.Where(t => t.Inviter.NickName.Contains(inviterName));
            if (inviterMobile.IsNotEmpty())     q = q.Where(t => t.Inviter.Mobile == inviterMobile);

            //
            if (inviteeId != null)              q = q.Where(t => t.InviteeID == inviteeId);
            if (inviteeName.IsNotEmpty())       q = q.Where(t => t.Invitee.NickName.Contains(inviteeName));
            if (inviteeMobile.IsNotEmpty())     q = q.Where(t => t.InviteeMobile == inviteeMobile);

            //
            if (createStartDt != null)          q = q.Where(t => t.CreateDt >= createStartDt);
            if (createEndDt != null)            q = q.Where(t => t.CreateDt <= createEndDt);
            if (source != null)                 q = q.Where(t => t.Source == source);
            if (status != null)                 q = q.Where(t => t.Status == status);
            return q;
        }


        //-----------------------------------------------
        // 邀请及注册
        //-----------------------------------------------
        /// <summary>新增邀请记录（用手机号或用户ID）</summary>
        /// <returns>若成功返回新建的邀请记录；若已存在邀请则返回空</returns>
        public static Invite Add(
            InviteSource source,
            long? inviteShopId, long? inviteUserId, 
            long? inviteeId, string inviteeMobile,
            InviteStatus status = InviteStatus.New)
        {
            //Logger.LogDb("AddInviteBegin", new { inviteShopId, inviteUserId, inviteeId, inviteeMobile }.ToJson());
            if (inviteUserId == null && inviteShopId == null)
                return null;

            // 补足用户ID信息
            if (inviteeId == null  && inviteeMobile.IsNotEmpty())
                inviteeId = User.Get(mobile: inviteeMobile)?.ID;

            // 受邀者记录已经存在则直接返回空
            var user = User.Get(inviteeId);
            var userInvite  = GetUserInvite(inviteeId, inviteeMobile);
            var shopInvite = GetShopInvite(inviteeId, inviteeMobile);
            //Logger.LogDb("GetInvite", new { user = user?.Export(false), userInvite = userInvite?.Export(false), ShopInvite = ShopInvite?.Export(false) }.ToJson());
            if (inviteUserId != null)
            {
                if (user?.InviterID != null) return null;
                if (userInvite != null)      return null;
            }
            if (inviteShopId != null)
            {
                if (user?.ShopID != null)   return null;
                if (shopInvite != null)     return null;
            }

            // 新增
            var item = new Invite();
            item.Source = source;
            item.Status = status;
            item.CreateDt = DateTime.Now;
            item.InviteShopID = inviteShopId;
            item.InviterID = inviteUserId;
            item.InviteeID = inviteeId;
            item.InviteeMobile = inviteeMobile;
            if (status == InviteStatus.Regist)
                item.ChangeStatus(status);
            item.Save();
            //Logger.LogDb("NewInvite", item.ExportJson(false));

            // 设置用户归属商店和邀请人信息
            if (user != null)
            {
                if (user.ShopID == null)
                {
                    user.ShopID = inviteShopId;
                    TrySetUserTagByShop(user.WechatOPID, inviteShopId);
                }
                if (user.InviterID == null)
                    user.InviterID = item.InviterID;
                user.FixArea();
                user.Save();
                //Logger.LogDb("SetUserShopAndInviter", user.ExportJson(false));
            }

            //
            return item;
        }

        /// <summary>设置微信公众号用户标签（根据门店名称）</summary>
        public static void TrySetUserTagByShop(string openId, long? inviteShopId)
        {
            var shop = Shop.Get(inviteShopId);
            if (shop != null && openId.IsNotEmpty())
            {
                Tag tag = WechatOP.TrySetUserTag(openId, shop.AbbrName);
                if (tag != null)
                {
                    var o = new { openId = openId, tag = tag.name };
                    Logger.LogDb("WechatOpen-SetTag", o.ToString(), openId, LogLevel.Debug);
                }
            }
        }



        /// <summary>状态变更</summary>
        public Invite ChangeStatus(InviteStatus status)
        {
            var invitee = User.Get(this.InviteeID);
            this.AddHistory(invitee.ID, invitee.NickName, invitee.Mobile, status.GetTitle(), (int)status);
            this.Status = status;
            if (status == InviteStatus.Regist)
                this.RegistDt = DateTime.Now;
            return this.Save();
        }


        //-----------------------------------------------
        // 邀请及注册（邀请码方式）
        //-----------------------------------------------
        /// <summary>新增邀请记录（用邀请码）</summary>
        /// <returns>若成功返回新建的邀请记录；若已存在邀请则返回空</returns>
        public static Invite Add(InviteSource source, string inviteCode, long? inviteeId, string inviteeMobile, InviteStatus status= InviteStatus.New)
        {
            QrCodeData code = QrCodeData.Parse(inviteCode);
            if (code == null)
                return null;

            var dict = code.Key.ParseDict();
            var inviteShopId = dict["shopId"].ParseInt();
            var inviteUserId  = dict["userId"].ParseInt();
            return Add(source, inviteShopId, inviteUserId, inviteeId, inviteeMobile, status);
        }

        /// <summary>获取邀请码</summary>
        public static string GetInviteCode(long? shopId, long? userId, string userMobile)
        {
            User user = (userId == null) ? null : User.Get(id: userId, mobile: userMobile);
            Shop shop = (shopId == null) ? null : Shop.Get(shopId);
            var title = string.Format("{0} {1} 邀请你加入", shop.Name, user.NickName);
            var key   = string.Format("userId={0}&shopId={1}", user?.ID, shop?.ID);
            return new QrCodeData(QrCodeType.Invite, key, title, null).ToString();
        }
    }
}