using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using App.Utils;
using App.Controls;
using App.Controls.ECharts;
using App.DAL;
using FineUIPro;
using App.Components;
using App.Entities;
using App.Base;
using App.Plugins;

namespace App.Admins
{
    [UI("欢迎页面")]
    [Auth(AuthLogin =true)]
    public partial class Welcome : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // 渲染小插件
                var pans   = new List<FineUIPro.Panel> { pan1, pan2, pan3, pan4 };
                var labels = new List<FineUIPro.Label> { widget1, widget2, widget3, widget4 };
                var i = 0;
                foreach (var widget in Widget.All.OrderBy(t => t.Seq))
                {
                    var today = DateTime.Today;
                    var startDt = today.AddDays(widget.StartDay ?? -60);
                    var endDt = today.AddDays(widget.EndDay ?? 0);
                    try
                    {
                        var w = Reflector.Create(widget.TypeName, "") as IWidget;
                        pans[i].Title = widget.Title;
                        w.Render(startDt, endDt, labels[i++].ClientID);
                        if (i >= labels.Count)
                            break;
                    }
                    catch
                    {
                        Logger.LogDb("Widget-Fail", widget.TypeName);
                    }
                }
            }
        }


    }
}
