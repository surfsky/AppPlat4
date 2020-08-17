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
        //---------------------------------------------
        // 邀请
        //---------------------------------------------
        /// <summary>添加邀请并直接奖励</summary>
        public static Invite InviteAndAward(User user, long? inviteUserId, long? inviteShopId,
            InviteSource source = InviteSource.Wechat, InviteStatus status = InviteStatus.Regist)
        {
            Invite.Add(source, inviteShopId, inviteUserId, user.ID, user.Mobile, status);
            AwardInviteShop(user.ID);
            return Logic.AwardInviteUser(user.ID);
        }

        /// <summary>奖励真正的邀请人</summary>
        /// <param name="inviteeId">受邀者ID</param>
        /// <returns>有效的用户邀请记录</returns>
        public static Invite AwardInviteUser(long? inviteeId)
        {
            var invite = Invite.GetUserInvite(inviteeId, "");
            if (invite != null && invite.Status == InviteStatus.Regist)
            {
                if (invite.InviterAwarded != true && invite.InviterID != null)
                {
                    UserScore.Add(ScoreType.Invite, invite.InviterID, 20, invite.UniID);
                    invite.InviterAwarded = true;
                    invite.Save();

                    Logger.LogDb("AwardInviteUser", invite.ExportJson());
                    SendWechatInviteMessage(invite, true);
                }
                return invite;
            }
            return null;
        }

        /// <summary>奖励真正的邀请商店</summary>
        public static Invite AwardInviteShop(long? inviteeId)
        {
            var invite = Invite.GetShopInvite(inviteeId, "");
            if (invite != null && invite.Status == InviteStatus.Regist)
            {
                if (invite.InviteShopAwarded != true && invite.InviteShopID != null)
                {
                    invite.InviteShopAwarded = true;
                    invite.Save();
                    Logger.LogDb("AwardInviteShop", invite.ExportJson());
                    SendWechatInviteMessage(invite, false);
                }
                return invite;
            }
            return null;
        }

    }
}
