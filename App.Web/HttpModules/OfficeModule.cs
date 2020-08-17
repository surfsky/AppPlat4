using App.Utils;
using System;
using System.Web;
using System.Web.SessionState;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using App.DAL;

namespace App.Components
{
    /*
    HttpApplication 生命周期

    01.对请求进行验证，将检查浏览器发送的信息，并确定其是否包含潜在恶意标记。有关更多信息，请参见 ValidateRequest 和脚本侵入概述。
    02.如果已在 Web.config 文件的 UrlMappingsSection 节中配置了任何 URL，则执行 URL 映射。
    03.引发 BeginRequest 事件。
    04.引发 AuthenticateRequest 事件。
    05.引发 PostAuthenticateRequest 事件。
    06.引发 AuthorizeRequest 事件。
    07.引发 PostAuthorizeRequest 事件。
    08.引发 ResolveRequestCache 事件。
    09.引发 PostResolveRequestCache 事件。<--- 可在此进行 HttpContext.Current.RemapHandler
    10.根据所请求文件扩展名，选择实现 IHttpHandler 的类，对请求进行处理。
    11.引发 PostMapRequestHandler 事件。
    12.引发 AcquireRequestState 事件。    <--- 可获取Session信息
    13.引发 PostAcquireRequestState 事件。
    14.引发 PreRequestHandlerExecute 事件。
    15.为该请求调用合适的 IHttpHandler 类的 ProcessRequest 方法（或异步版 BeginProcessRequest）。例如，如果该请求针对某页，则当前的页实例将处理该请求。
    16.引发 PostRequestHandlerExecute 事件。
    17.引发 ReleaseRequestState 事件。
    18.引发 PostReleaseRequestState 事件。
    19.如果定义了 Filter 属性，则执行响应筛选。
    20.引发 UpdateRequestCache 事件。
    21.引发 PostUpdateRequestCache 事件。
    22.引发 EndRequest 事件。
    */
    /// <summary>
    /// Office 文件保护模块（可以打上查看用户信息水印）
    /// 配置：
    /// (1) 使用时需将doc,docx,xls,xlsx,ppt,pptx,pdf等从mime配置中删除（避免被 staticFileModule 处理）
    /// (2) webconfig 中进行配置：&lt;add name="OfficeModule" type="App.Components.OfficeModule" /&gt;
    /// </summary>
    public class OfficeModule : IHttpModule
    {
        public void Dispose() { }
        public void Init(HttpApplication application)
        {
            // 注意必须这么写。OfficeHandler 用到了 Session
            // 用其它事件或方法都无法正确获取 Session 对象
            application.PostResolveRequestCache += delegate (object sender, EventArgs e)
            {
                var ext = HttpContext.Current.Request.RawUrl.GetFileExtension();
                if (Downloader.IsOfficeFile(ext))
                    HttpContext.Current.RemapHandler(new OfficeHandler()); // 指定处理器
            };
        }
    }

    /// <summary>
    /// Office文件保护处理器。
    /// <add name="OfficeHandler" path="*.doc;*.docx;*.xls;*.xlsx;*.ppt;*.pptx;" verb="*" type="App.Components.OfficeHandler"  />
    /// </summary>
    public class OfficeHandler : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable { get { return true; } }
        public void ProcessRequest(HttpContext context)
        {
            var url = context.Request.RawUrl;
            var protect = Asp.GetQueryBool("protect");
            var watermark = Asp.GetQueryString("watermark");
            Downloader.Down(url, "", protect, watermark);
        }
    }
}
