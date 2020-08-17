using System;
using System.Web;
using System.Web.SessionState;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.UI;
using App.Utils;

namespace App.Components
{
    /// <summary>
    /// 友好URL模块，可省略 aspx 和 ashx 和 cshtml
    /// （2）webconfig 中进行配置：&lt;add name="FrendlyUrlModule" type="App.Components.FrendlyUrlModule" /&gt;
    /// </summary>
    public class FrendlyUrlModule : IHttpModule
    {
        public void Dispose() { }
        public void Init(HttpApplication application)
        {
            application.PostResolveRequestCache += delegate (object sender, EventArgs e)
            {
                var context = HttpContext.Current;

                // 如果是目录或者有扩展名，跳过
                if (context.Request.RawUrl.Last() == '/')
                    return;
                var ext = context.Request.RawUrl.GetFileExtension();
                if (ext.IsNotEmpty())
                    return;

                // 无扩展名的页面，尝试附加扩展名后进行解析
                // 尝试用aspx解析
                var url = new Url(context.Request.RawUrl);
                url.FileExtesion = ".aspx";
                var type = Asp.GetHandler(url.ToString());
                if (type != null)
                {
                    context.RewritePath(url.ToString());
                    return;
                }

                // 尝试用ashx解析
                url.FileExtesion = ".ashx";
                type = Asp.GetHandler(url.ToString());
                if (type != null)
                {
                    context.RewritePath(url.ToString());
                    return;
                }

                // 尝试用cshtml解析(razor page，未完成)
                url.FileExtesion = ".cshtml";
                type = Asp.GetHandler(url.ToString());
                if (type != null)
                {
                    // (1) 获取Page model
                    // (2) 将Page model 赋值给 razor page
                    // (3) 构建出网页并输出
                    //context.RewritePath(url.ToString());
                    /*
                    var services = HttpContext.RequestServices;
                    var executor = services.GetRequiredService<ViewResultExecutor>();
                    var viewEngine = services.GetRequiredService<IRazorViewEngine>();
                    var view = viewEngine.GetView(null, "~/Pages/IeAlert.cshtml", true)?.View;
                    if (view != null)
                    {
                        using (view as IDisposable)
                        {
                            await executor.ExecuteAsync(ControllerContext, view, ViewData, TempData, "text/html; charset=utf-8", 200);
                        }
                        return new EmptyResult();
                    }
                    */
                    return;
                }

                /*
                 public void ProcessRequest(HttpContext context)
                {
                    List<Person> list = new List<Person>();
                    list.Add(new Person { Name="rupeng",Age=8});
                    list.Add(new Person { Name = "qq", Age = 18 });
                    context.Items["persons"] = list;
                    context.Server.Transfer("PersonsView.aspx");
                }
                 * 
                 * */
            };
        }
    }

}
