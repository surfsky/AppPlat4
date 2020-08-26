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
    /// <summary>
    /// 健身邀请状态
    /// </summary>
    public enum FitInviteStatus
    {
        [UI("已创建")] New,
        [UI("已接受")] Accept
    }

    /// <summary>
    /// 邀请锻炼表
    /// （1）用户可邀请其它用户一起锻炼。系统发送推送消息。
    /// （2）受邀者点击消息，并点击接受按钮，接受邀请。
    /// </summary>
    [UI("健身", "邀请")]
    public class FitInvite : EntityBase<FitInvite>
    {
        [UI("邀请人")]       public long? InviterID { get; set; }
        [UI("受邀人")]       public long? InviteeID { get; set; }
        [UI("接受邀请时间")] public DateTime? AcceptDt { get; set; }
        [UI("状态")]         public FitInviteStatus? Status { get; set; }


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

                // inviter
                this.InviterID,
                InviterName = this.Inviter?.NickName,
                InviterPhoto = this.Inviter?.Avatar,
                InviterMobile = this.Inviter?.Mobile,

                // invitee
                this.InviteeID,
                InviteeName = this.Invitee?.NickName,
                InviteePhoto = this.Invitee?.Avatar,
                InviteeMobile = this.Invitee?.Mobile,

                // 时间和状态
                this.CreateDt,
                this.AcceptDt,
                this.Status,
                StatusName = this.Status?.GetTitle(),
            };
        }


        /// <summary>获取详情</summary>
        public new static FitInvite GetDetail(long id)
        {
            return IncludeSet.Where(t => t.ID == id).FirstOrDefault();
        }

        // 查询
        public static IQueryable<FitInvite> Search(
            long? inviterID = null, string inviterName = null, string inviterMobile = null,
            long? inviteeId = null, string inviteeName = null, string inviteeMobile = null,
            DateTime? createStartDt = null, DateTime? createEndDt = null,
            FitInviteStatus? status = null
            )
        {
            var q = IncludeSet;

            //
            if (inviterID != null)              q = q.Where(t => t.InviterID == inviterID);
            if (inviterName.IsNotEmpty())       q = q.Where(t => t.Inviter.NickName.Contains(inviterName));
            if (inviterMobile.IsNotEmpty())     q = q.Where(t => t.Inviter.Mobile == inviterMobile);

            //
            if (inviteeId != null)              q = q.Where(t => t.InviteeID == inviteeId);
            if (inviteeName.IsNotEmpty())       q = q.Where(t => t.Invitee.NickName.Contains(inviteeName));
            if (inviteeMobile.IsNotEmpty())     q = q.Where(t => t.Invitee.Mobile == inviteeMobile);

            //
            if (createStartDt != null)          q = q.Where(t => t.CreateDt >= createStartDt);
            if (createEndDt != null)            q = q.Where(t => t.CreateDt <= createEndDt);
            if (status != null)                 q = q.Where(t => t.Status == status);
            return q;
        }


        //-----------------------------------------------
        // 邀请及注册
        //-----------------------------------------------
        /// <summary>新增邀请记录（用手机号或用户ID）</summary>
        /// <returns>若成功返回新建的邀请记录；若已存在邀请则返回空</returns>
        public static FitInvite Add(long? inviterId, long? inviteeId)
        {
            if (inviterId == null || inviteeId == null)
                return null;

            // 受邀者记录已经存在则直接返回空
            var inviter = User.Get(inviterId);
            var invitee = User.Get(inviteeId);

            // 新增
            var item = new FitInvite();
            item.Status = FitInviteStatus.New;
            item.CreateDt = DateTime.Now;
            item.InviterID = inviterId;
            item.InviteeID = inviteeId;
            item.Save();

            //
            return item;
        }


        /// <summary>状态变更</summary>
        public FitInvite ChangeStatus(FitInviteStatus status)
        {
            var invitee = User.Get(this.InviteeID);
            this.AddHistory(invitee.ID, invitee.NickName, invitee.Mobile, status.GetTitle(), (int)status);
            this.Status = status;
            if (status == FitInviteStatus.Accept)
                this.AcceptDt = DateTime.Now;
            return this.Save();
        }
    }
}