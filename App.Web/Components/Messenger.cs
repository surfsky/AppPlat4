using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.DAL;

namespace App.Components
{
    /// <summary>
    /// 消息统一派送类
    /// </summary>
    public class Messenger
    {
        /// <summary>通知 Web 客户端（必须不阻断、不报错，不影响业务）</summary>
        public static void SendToComet(CometMessageType type, object value, string channel = "")
        {
            try
            {
                //CometMessenger.Send(type, value, channel);
            }
            catch (Exception ex)
            {
                Logger.LogWebRequest("WebComet", ex, Common.LoginUser?.NickName);
            }
        }

    }
}