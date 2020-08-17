using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Utils;
using App.DAL;
using App.Entities;
using App.Scheduler;

namespace App.Plugins
{
    /// <summary>网站小部件</summary>
    public interface IWidget
    {
        string Title { get; }
        void Render(DateTime startDt, DateTime endDt, string clientId);
    }

    /// <summary>网站插件类型</summary>
    public enum SitePluginType
    {
        Base,
        CMS,
        OA,
        MALL,
        Others,
    }

    /// <summary>网站插件（模块）接口</summary>
    public interface ISitePlugin
    {
        /// <summary>名称</summary>
        string Name { get; }

        /// <summary>类型</summary>
        SitePluginType Type { get; }

        /// <summary>版本</summary>
        string Version { get; }

        /// <summary>依赖的插件</summary>
        List<ISitePlugin> Dependencies { get; }


        //-------------------------------------
        // 页面、接口、图表、任务
        //-------------------------------------
        /// <summary>页面</summary>
        List<string> Pages { get; }

        /// <summary>API接口类</summary>
        List<Type> Apis { get; }

        /// <summary>小插件</summary>
        List<IWidget> Widgets { get; }

        /// <summary>调度任务</summary>
        List<IJobRunner> Jobs { get; }  // 考虑重构，增加默认配置

        //-------------------------------------
        // 数据
        //-------------------------------------
        /// <summary>实体类</summary>
        List<Type> Entities { get; }

        /// <summary>角色</summary>
        List<Role> Roles { get;}

        /// <summary>权限</summary>
        List<Power> Powers { get;}

        /// <summary>初始化数据</summary>
        void Init(DbContext context);

        /// <summary>修复数据</summary>
        int Fix(DbContext context);
    }
}
