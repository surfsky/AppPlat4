using App.Utils;
using App.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using System.Diagnostics;
using App.Components;
using App.Controls;


// 若实现 IRequiresSessionState，会启用 session 读写锁
// 当会话开始的时候，ashx便得到一个读写锁, 如果这个线程一直不结束，这个锁便无法释放，
// 从而导致其他页面无法得到session锁，也就一直阻塞。
// 故以下代码实现 IReadOnlySessionState，不会阻塞
// http://blog.51cto.com/boytnt/1250084
namespace App.Pages
{
    [UI("长连接消息处理器")]
    [Auth(Ignore=true)]
    public class Comets : IHttpAsyncHandler, IReadOnlySessionState
    {
        public bool IsReusable
        {
            get { return false; }
        }

        // 开始异步请求，并挂起
        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback callback, object extraData)
        {
            var channel = context.Request.QueryString["channel"];
            var comet = new Comet(context, callback, extraData, channel);
            CometMessenger.AddClient(comet);
            return comet;
        }

        // 结束异步请求（由 CometMgr.Active() -> comet.Callback 触发）
        public void EndProcessRequest(IAsyncResult result)
        {
            var comet = result as Comet;
            comet.Send();
        }

        // 异步请求永远不会触发此方法
        public void ProcessRequest(HttpContext context)
        {
        }
    }


}