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
using System.Collections;
using App.Components;
using App.Controls;
using System.Reflection;
using App.Utils;

namespace App.Pages
{
    [UI("API 接口清单")]
    [Auth(Powers.API)]
    public partial class API : PageBase
    {
        //------------------------------------------
        // init
        //------------------------------------------
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindGrid();
        }

        private void BindGrid()
        {
            /*
            ArrayList arr = new ArrayList
            {
                  new { Name = "Common",  URL = "/HttpApi/Common/api", JS="/HttpApi/Common/js",  Description = "通用接口" }
                 ,new { Name = "Base",    URL = "/HttpApi/Base/api",   JS="/HttpApi/Base/js",  Description = "基础模块接口" }
                 ,new { Name = "User",    URL = "/HttpApi/User/api",   JS="/HttpApi/User/js",  Description = "用户相关接口" }
                 //,new { Name = "Mall",    URL = "/HttpApi/Mall/api",   JS="/HttpApi/Mall/js",  Description = "商城相关接口" }
                 //,new { Name = "Wechat",  URL = "/HttpApi/Wechat/api", JS="/HttpApi/Wechat/js",  Description = "微信相关接口" }
                 ,new { Name = "Ding",       URL = "/HttpApi/Ding/api", JS="/HttpApi/Ding/js",  Description = "钉钉相关接口" }
                 ,new { Name = "Knowledge",  URL = "/HttpApi/Knowledge/api", JS="/HttpApi/Knowledge/js",  Description = "知识库相关接口" }
            };
            */

            /*
            // 自动枚举以"App.Components.Api"开头的类
            ArrayList arr = new ArrayList();
            var types = Assembly.GetAssembly(typeof(API)).GetTypes();
            foreach (var type in types)
            {
                // 接口文件列表
                var name = type.FullName;
                var prefix = "App.Apis.Api";
                if (type.FullName.StartsWith(prefix) && !type.Name.Contains("<>c"))  // <>c是partial类
                {
                    // 跳过商城接口
                    var attr = type.GetAttribute<ScopeAttribute>();
                    if (attr?.Scope == "Mall")
                        continue;

                    //
                    var api = name.Substring(prefix.Length);
                    var desc = type.GetTitle();
                    arr.Add(new { Name = api, URL = $"/HttpApi/{api}/api", JS = $"/HttpApi/{api}/js", Description = desc });
                }
            }
            */
            var prefix = "App.Apis.Api";
            ArrayList arr = new ArrayList();
            foreach (var p in Global.Plugins)
            {
                foreach (var type in p.Apis)
                {
                    var name = type.FullName.TrimStart(prefix);
                    var desc = type.GetTitle();
                    arr.Add(new { Name = name, URL = $"/HttpApi/{name}/api", JS = $"/HttpApi/{name}/js", Description = desc });
                }
            }

            // 绑定
            Grid1.DataSource = arr;
            Grid1.DataBind();
        }
    }
}
