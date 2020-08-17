using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Diagnostics;
using System.Reflection;
using App.Utils;
using System.Collections;
using System.Linq.Expressions;
using App.Entities;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace App.DAL
{
    /// <summary>
    /// 数据模型类型信息
    /// </summary>
    public class EntityType
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Group { get; set; }
        public string Description { get; set; }
        public Type Type { get; set; }
    }

    /// <summary>
    /// EntityFramework 数据库上下文
    /// </summary>
    public class AppContext : DbContext
    {
        //------------------------------------------------------
        // 实体集合
        //------------------------------------------------------
        // 开放平台
        public DbSet<OpenApp>         Apps { get; set; }

        // XUI
        public DbSet<XUI>             UIs { get; set; }
        public DbSet<XState>         Configs { get; set; }

        // 基础库表
        public DbSet<Area>            Areas { get; set; }
        public DbSet<Dept>            Depts { get; set; }
        public DbSet<User>            Users { get; set; }
        public DbSet<RolePower>       RolePowers { get; set; }
        public DbSet<Role>            Roles { get; set; }
        public DbSet<Title>           Titles { get; set; }
        public DbSet<UserDept>        UserDepts { get; set; }
        public DbSet<UserArea>        UserAreas { get; set; }
        public DbSet<Org>             Orgs { get; set; }
        public DbSet<Res>             Res { get; set; }
        public DbSet<History>         Histories { get; set; }

        // 配置
        public DbSet<Menu>            Menus { get; set; }
        public DbSet<Route>           Routes { get; set; }
        public DbSet<Sequence>        Sequences { get; set; }
        public DbSet<SiteConfig>      SiteConfigs { get; set; }
        public DbSet<Widget>          Widgets{ get; set; }

        // 运维
        public DbSet<Online>          Onlines { get; set; }
        public DbSet<Log>             Logs { get; set; }
        public DbSet<LogConfig>       LogConfigs { get; set; }
        public DbSet<VerifyCode>      VerifyCodes { get; set; }
        public DbSet<IPFilter>        IPFilters { get; set; }
        public DbSet<Feedback>        Feedbacks { get; set; }
        public DbSet<Message>         Messages { get; set; }
        public DbSet<LoginLog>     LoginRecords { get; set; }

        // 文档
        public DbSet<Article>         Articles { get; set; }
        public DbSet<ArticleDir>      ArticleDirs { get; set; }
        public DbSet<ArticleVisit>    ArticleVisits { get; set; }
        public DbSet<ArticleDirRole>  ArticleDirRoles { get; set; }
        public DbSet<ArticleDirFavorite> ArticleDirFavorites { get; set; }
        public DbSet<ArticleConfig>   ArticleConfigs { get; set; }
        public DbSet<ArticleStudy> ArticleStudyRecords { get; set; }


        // 用户附属表
        public DbSet<Invite>          UserInvites { get; set; }
        public DbSet<UserSign>        UserSigns { get; set; }
        public DbSet<UserFinance>     UserFinances { get; set; }
        public DbSet<UserScore>       UserScores { get; set; }

        // 商店相关
        public DbSet<Advert>          Adverts { get; set; }
        public DbSet<Shop>            Shops { get; set; }
        public DbSet<Product>         Products { get; set; }
        public DbSet<ProductSpec>     ProductSpecs { get; set; }
        public DbSet<Order>           Orders { get; set; }
        public DbSet<OrderItem>       OrderItems { get; set; }
        public DbSet<OrderRate>       OrderRates { get; set; }
        public DbSet<OrderItemAsset>  OrderItemAssets { get; set; }

        // 报表相关（供参考）
        public DbSet<RptGDP>          RptGDPs { get; set; }

        // 微信相关
        public DbSet<WechatConfig>    WechatConfigs { get; set; }
        public DbSet<WechatMPForm>    WechatMPForms { get; set; }

        // Ali
        public DbSet<AliSmsConfig>    AliSmsConfigs { get; set; }
        public DbSet<AliDingConfig>   AliDingConfigs { get; set; }


        // 工作流
        //public DbSet<WFStep>          Steps { get; set; }



        //------------------------------------------------------
        // 事件
        //------------------------------------------------------
        /// <summary>模型创建事件（可在此扩展模型，如增加DbSet）</summary>
        /// <example>
        /// public class Customer { }
        /// AppContext.Current.OnBuild += (c) =>
        /// {
        ///     c.Add<Customer>(new System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Customer>());
        /// };
        /// </example>
        public event Action<ConfigurationRegistrar> OnBuild;



        //------------------------------------------------------
        // 数据库上下文
        //------------------------------------------------------
        /// <summary>
        /// 数据库上下文。
        /// （1）若为 Web 环境，则使用 HttpContext 来保存 AppContext 实例，针对每个请求创建一个。
        /// （2）否则动态创建一个，循环使用。(在线程环境下可能会有问题，最好即开即关，需要测试）
        /// </summary>
        private static AppContext _context;
        public static AppContext Current
        {
            get
            {
                if (Asp.IsWeb)
                    return Asp.GetContextData("__DbContext", () => new AppContext()) as AppContext;
                else
                {
                    if (_context == null)
                        _context = new AppContext();
                    return _context;
                }
            }
        }

        /// <summary>
        /// 释放数据库上下文
        /// </summary>
        public static void Release()
        {
            if (Asp.IsWeb)
                HttpContext.Current.Items["__DbContext"] = null;
            else
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }


        //------------------------------------------------------
        // 成员方法
        //------------------------------------------------------
        // 构造函数 (别合并这两个方法，会出错)
        public AppContext() : this("db") {}
        public AppContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            this.Database.Log = (t) => Debug.WriteLine(t);  // 打印数据库操作SQL到console
        }


        // 创建数据库时进行设置
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 模型变更会自动去修改数据库
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppContext, AppMigrationConfiguration>());  // 自动更新数据库
            //Database.SetInitializer<AppContext>(new AppDatabaseInitializer()); // 模型一变更就重建数据库
            //Database.SetInitializer<AppContext<Label>>(null);

            // 表间的关联关系
            // User <--> Title（效果同建立一张表 TitleUsers(TitleID, UserID)
            modelBuilder.Entity<Title>()
                .HasMany(t => t.Users)
                .WithMany(u => u.Titles)
                .Map(x => x.ToTable("TitleUsers").MapLeftKey("TitleID").MapRightKey("UserID"))
                ;

            // 触发创建模型事件
            if (this.OnBuild != null)
                this.OnBuild(modelBuilder.Configurations);
        }


        //-----------------------------------------
        // 数据模型
        //-----------------------------------------
        /// <summary>获取实体查询对象(如 User.Set 或 All)</summary>
        public static object GetQuery(Type t)
        {
            if (t.IsInterface(typeof(ICacheAll)))
                return GetCaches(t);
            else
                return GetEntitySet(t);
        }

        /// <summary>获取实体数据集(如 User.Set)</summary>
        public static object GetEntitySet(Type t)
        {
            var property = t.GetProperty("Set", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            return property?.GetValue(null);
        }

        /// <summary>获取实体数据集缓存(如 User.All)</summary>
        public static object GetCaches(Type t)
        {
            var property = t.GetProperty("All", BindingFlags.Public | BindingFlags.Static);
            property = property ?? t.GetProperty("All", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var list = property?.GetValue(null);
            return System.Linq.Queryable.AsQueryable(list as IEnumerable);
        }

        /// <summary>获取实体数据列表</summary>
        public static IList GetEntities(Type t)
        {
            dynamic q = GetEntitySet(t);
            return System.Linq.Enumerable.ToList(q) as IList;
        }

        /// <summary>获取实体数据</summary>
        public static EntityBase GetEntity(Type t, long id)
        {
            dynamic q = GetQuery(t);
            if (q is DbSet ds)
                return ds.Find(id) as EntityBase;
            else
            {
                foreach (var item in q)
                    if (item.ID == id)
                        return item as EntityBase;
                return null;
            }
        }

        /// <summary>获取实体数据</summary>
        public static T GetEntity<T>(long id) where T : EntityBase
        {
            return AppContext.Current.Set<T>().Find(id);
        }

        /// <summary>获取实体类型列表</summary>
        public static List<EntityType> EntityTypes => IO.GetCache("EntityTypes", () =>
        {
            var types = new List<EntityType>();
            var ps = typeof(AppContext).GetProperties();
            foreach (var p in ps.OrderBy(t => t.Name))
            {
                var type = p.PropertyType;
                if (type.IsGenericType && type.Name.Contains("DbSet"))
                {
                    var entityType = type.GetGenericDataType();
                    types.Add(new EntityType()
                    {
                        Name = entityType.Name,
                        FullName = entityType.FullName,
                        Group = entityType.GetUIGroup(),
                        Description = entityType.GetTitle(),
                        Type = entityType
                    });
                }
            }
            return types;
        });


        //-----------------------------------------
        // UI 缓存
        //-----------------------------------------
        /// <summary>网格UI缓存</summary>
        public static List<UISetting> GridUIs => IO.GetCache("GridUIs", () =>
        {
            var settings = new List<UISetting>();
            foreach (var item in EntityTypes)
            {
                var setting = (Activator.CreateInstance(item.Type) as EntityBase).GridUI();
                settings.Add(setting);
            }
            return settings;
        });

        /// <summary>表单UI缓存</summary>
        public static List<UISetting> FormUIs => IO.GetCache("FormUIs", () =>
        {
            var settings = new List<UISetting>();
            foreach (var item in EntityTypes)
            {
                var setting = (Activator.CreateInstance(item.Type) as EntityBase).FormUI();
                settings.Add(setting);
            }
            return settings;
        });


        /// <summary>检索UI缓存</summary>
        public static List<UISetting> SearchUIs => IO.GetCache("SearchUIs", () =>
        {
            var settings = new List<UISetting>();
            foreach (var item in EntityTypes)
            {
                var m = GetSearchMethod(item.Type);
                if (m != null)
                    settings.Add(new UISetting(m));
            }
            return settings;
        });

        /// <summary>找到实体检索方法（具有[SearcherAttribute]，若没有则尝试找名称为"Search"的方法）</summary>
        static MethodInfo GetSearchMethod(Type type)
        {
            foreach (var m in type.GetMethods())
            {
                if (m.GetAttribute<SearcherAttribute>() != null)
                    return m;
            }
            return type.GetMethods("Search", false).FirstOrDefault();
        }


    }



}