using App.Utils;
using App.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Pages.Malls
{
    public class Manifest
    {

        /// <summary>获取邀请地址</summary>
        public static string GetInviteUrl(long? shopId, long? userId, string userMobile)
        {
            var inviteCode = Invite.GetInviteCode(shopId, userId, userMobile);
            var url = string.Format("~/Pages/Malls/Regist.aspx?inviteCode={0}", inviteCode.UrlEncode());
            return Asp.ResolveFullUrl(url);
        }

    }
}