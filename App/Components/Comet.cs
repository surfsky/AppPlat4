using App.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace App.Components
{
    /// <summary>
    /// 长连接消息类别
    /// </summary>
    public enum CometMessageType : int
    {
        HeartBeat = 0,
        Online = 1,
        Order = 2,
        News = 3
    }

    /// <summary>
    /// 长连接消息
    /// </summary>
    public class CometMessage
    {
        public CometMessageType Type { get; set; }
        public Object Value { get; set; }
        public string Channel { get; set; }
        public bool Sended { get; set; } = false;

        public CometMessage(CometMessageType type, object value, string channel = "")
        {
            this.Type = type;
            this.Value = value;
            this.Channel = channel;
        }

        public void Send()
        {
            var json = this.ToJson();
            var response = Asp.Response;
            if (response.IsClientConnected)
            {
                response.ContentType = "text/json";
                response.Write(json);
            }
        }
    }


    /// <summary>
    /// 长连接对象（与Comets.ashx 结合使用）
    /// </summary>
    public class Comet : IAsyncResult
    {
        // 实现IAsyncResult接口
        public object AsyncState { get; private set; }
        public WaitHandle AsyncWaitHandle { get; private set; }
        public bool CompletedSynchronously { get; private set; }
        public bool IsCompleted { get; private set; }

        // 原样保存异步页面请求信息
        public AsyncCallback Callback { get; private set; }
        public HttpContext Context { get; private set; }
        public object ExtraData { get; private set; }

        // 自定义数据，用于传递给客户端
        public CometMessage Message { get; set; }
        public string Channel { get; set; }
        
        /// <summary>构造方法（原样保存 IHttpAsyncHandler.BeginProcessRequest 的几个参数）</summary>
        public Comet(HttpContext context, AsyncCallback callback, object extraData, string channel)
        {
            this.Context = context;
            this.Callback = callback;
            this.ExtraData = extraData;
            this.Channel = channel;
        }

        /// <summary>是否连接</summary>
        public bool IsConnected
        {
            get
            {
                if (Context == null)
                    return false;
                else
                    return Context.Response.IsClientConnected;
            }
        }

        /// <summary>发送消息</summary>
        public void Send()
        {
            if (this.IsConnected)
            {
                var json = new { Type = Message.Type.GetTitle(), Message.Value, Message.Channel }.ToJson();
                var response = this.Context.Response;
                Message.Sended = true;
                response.ContentType = "text/json";
                response.Write(json);
                response.Flush();
            }
        }
    }



    /// <summary>
    /// 长连接信使（管理器，与Comets.ashx 结合使用）
    /// 发送消息给客户端，请调用: CometMessenger.Send() 方法
    /// </summary>
    public class CometMessenger
    {
        //--------------------------------------------
        // 客户端管理
        //--------------------------------------------
        public static List<Comet> Clients = new List<Comet>();


        /// <summary>搜索客户端</summary>
        public static List<Comet> Search(string channel)
        {
            var q = Clients.AsQueryable();
            //if (!userName.IsNullOrEmpty()) q = q.Where(t => t.UserName == userName);
            if (!channel.IsEmpty()) q = q.Where(t => t.Channel == channel);
            return q.ToList();
        }

        /// <summary>新增客户端</summary>
        public static void AddClient(Comet client)
        {
            CheckClients();
            Clients.Add(client);
        }

        /// <summary>检测并移除无效客户端</summary>
        public static void CheckClients()
        {
            var clients = new List<Comet>();
            foreach (var client in Clients)
            {
                if (client.IsConnected)
                    clients.Add(client);
            }
            Clients = clients;
        }


        //--------------------------------------------
        // 发送消息给客户端
        //--------------------------------------------
        /// <summary>通知客户端</summary>
        public static void Send(CometMessageType type, object value, string channel="")
        {
            Send(new CometMessage(type, value, channel));
        }

        /// <summary>激活并通知客户端</summary>
        public static void Send(CometMessage msg)
        {
            var comets = CometMessenger.Search(msg.Channel);
            comets.ForEach(comet =>
            {
                comet.Message = msg;
                if (IsNeedSend(comet) && comet.IsConnected && comet.Callback != null)
                {
                    try
                    {
                        //comet.Context.Response.EndFlush(comet);
                        comet.Callback(comet);  // 通知异步页面端（来调用我自己的方法）会阻塞？
                    }
                    catch (Exception ex)
                    {
                        Logger.LogDb("SendCometMessageFail", ex.StackTrace, msg.Channel, DAL.LogLevel.Error);
                    }
                }
            });
        }

        /// <summary>根据消息类型、数据组合判断是否需要发送给当前客户</summary>
        /// <remarks>建议写成事件，把这个类封装到类库中去</remarks>
        private static bool IsNeedSend(Comet comet)
        {
            var o = comet.Message.Value;
            if (o is DAL.Order)
            {
                var order = o as DAL.Order;
                var userName = comet.Context.User.Identity.Name;

                // 判断用户是否具有查看此订单的权限，再决定是否推送
                if (userName == "admin")
                    return true;
                var user = DAL.User.Get(name: userName);
                if (user != null && user.HasPower(DAL.Power.OrderEdit))
                {
                    if (user.ShopID == order.ShopID)
                        return true;
                }
            }
            return false;
        }

    }
}