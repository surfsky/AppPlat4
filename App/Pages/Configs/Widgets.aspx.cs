using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
//using EntityFramework.Extensions;
using App.DAL;
using App.Components;
using App.Controls;
using App.Utils;

namespace App.Pages
{
    [UI("配置-小插件管理")]
    [Auth(Powers.Admin)]
    public partial class Widgets : MixPage<Widget>
    {
        protected override UISetting GetGridUI()
        {
            var ui = new UISetting<Widget>(true);
            return ui;
        }

        protected override UISetting GetFormUI()
        {
            // 插件字典
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var s in Global.Plugins)
                foreach (var w in s.Widgets)
                    dict.Add(w.GetType().FullName, $"{s.Name}-{w.Title}");

            var widgets = Global.Plugins;
            var ui = new UISetting<Widget>(true);
            ui.SetEditorWinList(t => t.TypeName, dict);
            return ui;
        }
    }
}
