using App.Components;
using App.Entities;
using App.Plugins;
using App.Scheduler;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Apis;
using App.DAL;

namespace App.Base
{
    /// <summary>
    /// 基础模块（插件）
    /// </summary>
    public class BasePlugin : ISitePlugin
    {
        public string Name => "基础";
        public SitePluginType Type =>  SitePluginType.Base;
        public string Version => "1.0";
        public List<ISitePlugin> Dependencies => null;


        //-------------------------------------
        // 页面及接口
        //-------------------------------------
        public List<string> Pages => new List<string>
        {
            "/Pages/Base/Users.aspx" ,
            "/Pages/Base/UserForm.aspx"
        };

        public List<Type> Apis => new List<Type>
        {
            typeof(ApiKnowledge),
            typeof(ApiCommon),
            typeof(ApiMall),
            typeof(ApiOpen),
            typeof(ApiUser),
            typeof(ApiWechat),
            typeof(ApiDing),
        };

        public List<IWidget> Widgets => new List<IWidget>()
        {
            new News(),
            new UserChart(),
            new RoleChart(),
            new ArticleChart(),
            new ArticleVisitChart(),
            new OrderChart(),
            new SignChart(),
        };

        public List<IJobRunner> Jobs => null;


        //-------------------------------------
        // 数据
        //-------------------------------------
        public List<Type> Entities => new List<Type>
        {
            typeof(App.DAL.User),
            typeof(App.DAL.Article),
        };

        public List<Role> Roles => new List<Role>
        {
            Role.Get("系统管理员"),
            Role.Get("文档管理员"),
            Role.Get("部门管理员"),
            Role.Get("目录管理员"),
            Role.Get("政企"),
            Role.Get("公众"),
        };

        public List<Power> Powers => new List<Power>
        {
        };

        public void Init(DbContext context)
        {
            new SiteConfig().Init();
            new ArticleConfig().Init();
            var roles = Roles;
            new Power().Init();
        }

        public int Fix(DbContext context)
        {
            return 0;
        }
    }
}
