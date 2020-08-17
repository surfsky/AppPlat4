using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.IO;
using System.Data.Entity;
using System.Web.Routing;
using System.Diagnostics;
using System.Threading;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.ModelConfiguration;
using App.DAL;
using App.Utils;
using App.HttpApi;
using App.Components;
using App.Entities;
using App.Plugins;

namespace App
{
    public class Global : HttpApplication
    {
        public static DateTime StartDt;
        public static List<ISitePlugin> Plugins;
        public static Version Version => typeof(Global).Assembly.GetName().Version;

        //----------------------------------------
        // IIS 应用程序池开启关闭事件
        //----------------------------------------
        protected void Application_End(object sender, EventArgs e)
        {
            Logger.LogDb("ApplicationEnd");
        }

        // 网站启动（若长时间未访问网站会释放进程池并关闭，等待下次请求时才开启，在本方法内无法保证后台线程运行的可靠性）
        protected void Application_Start(object sender, EventArgs e)
        {
            StartDt = DateTime.Now;
            InitCore();
            InitPlugins();
            InitDatabase();
            InitCaches();
            InitHttpApi();
            InitIO();
            InitSchedule();
            Logger.LogDb("InitOk");
        }

        /// <summary>初始化 App.Utils 配置</summary>
        private static void InitCore()
        {
            UtilConfig.Instance.MachineId = IO.GetAppSetting<int>("MachineID");
            UtilConfig.Instance.OnLog += (type, info, level) => Logger.Log(type, info, LogLevel.Info);   // 设置记录日志的方法
        }

        /// <summary>初始化插件</summary>
        static void InitPlugins()
        {
            Plugins = new List<ISitePlugin>();
            var types = Reflector.GetTypes(typeof(ISitePlugin));
            foreach( var type in types)
                Plugins.Add(Reflector.Create(type) as ISitePlugin);

            IO.Debug(Plugins.Count.ToString());
        }

        /// <summary>初始化数据库</summary>
        private static void InitDatabase()
        {
            // 设置获取数据库上下文的方法
            EntityConfig.Instance.OnGetDb += () => AppContext.Current;

            // 动态添加实体类
            AppContext.Current.OnBuild += (c) =>
            {
                c.Add<Power>(new EntityTypeConfiguration<Power>());
            };
            Database.SetInitializer(new AppDatabaseInitializer());
            DbInterception.Add(new EntityLogger());
            Logger.LogDb("ApplicationStart");

            // 初始化插件
            foreach (var p in Plugins)
                p.Init(AppContext.Current);
        }

        /// <summary>初始化缓存</summary>
        static void InitCaches()
        {
            // caches
            var o = XUI.All;
            var o1 = AppContext.EntityTypes;
            var o2 = AppContext.GridUIs;
            var o3 = AppContext.FormUIs;
            var o4 = AppContext.SearchUIs;
        }

        /// <summary>初始化IO环境</summary>
        private static void InitIO()
        {
            IO.PrepareDirectory(FileCacher.CacheFolder);
        }

        /// <summary>初始化调度</summary>
        /// <remarks>简单、不耗时的定时调度，如：清理文件缓存。
        /// 网站无请求时随时会给杀掉。要求稳定的调度请用App.Scheduler组件，如订单处理逻辑。
        /// </remarks>
        private static void InitSchedule()
        {
            var minutes = 10;
            var t = new System.Timers.Timer(1000*60*minutes);
            t.Elapsed += (s, te) =>
            {
                var n = FileCacher.ClearFileCaches(false);
                if (n > 0)
                    Logger.LogDb("RemoveCacheFiles", n.ToString());
            };
            t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            t.Enabled = true;
        }

        /// <summary>初始化 HttpApi</summary>
        private void InitHttpApi()
        {
            var httpApi = HttpApiConfig.Instance;
            httpApi.OnVisit += (ctx, method, attr, input) =>
            {
                if (attr.Log)
                    Logger.LogDb("HttpApi", input.ToJson(), "", LogLevel.Debug);
            };
            httpApi.OnAuth += (ctx, method, attr, token) =>
            {
                if (attr.AuthToken)
                {
                    var app = DAL.OpenApp.CheckToken(token);
                    if (app == null)
                        throw new HttpApiException(401, "授权 Token 错误，请咨询管理员");
                }
            };
            httpApi.OnException += (method, ex) =>
            {
                if (ex is HttpApiException)
                {
                    var err = ex as HttpApiException;
                    var info = string.Format("code={0}, message={1}, detail={2}", err.Code, err.Message, Asp.BuildRequestHtml(ex));
                    Logger.LogDb("HttpApiException", info, "", LogLevel.Error);
                }
                else
                    Logger.LogWebRequest("HttpApiException", ex);
            };
        }




        //----------------------------------------
        // 页面请求生命周期
        //----------------------------------------
        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }
        
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            UserRolePrincipal user = App.Utils.AuthHelper.LoadPrincipalFromCookie() as UserRolePrincipal;

            // 自动续签验票（待测试，频繁刷新cookie是否会带来性能及网络问题）
            /*
            if (user != null && user.Identity != null && user.Identity.Name.IsNotEmpty())
            {
                var expiration = DateTime.Now.AddHours(1);
                AuthHelper.Login(user.Identity.Name, user.Roles, expiration);
            }
            */
        }

        protected virtual void Application_EndRequest()
        {
            AppContext.Release();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Logger.LogWebException("ApplicationError");
        }

    }
}