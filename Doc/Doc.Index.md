# AppPlat 开发指南

- Author: surfsky@sina,com
- For: Developer


# 项目架构

- 核心项目
    - App               网站
    - App.BLL           业务类库
    - App.Tests         业务类库测试
    - App.Monitor       控制台监控程序
- 引用模块
    - App.Utils         UI无关通用类库
    - App.HttpApi       数据接口框架
    - App.Scheduler     调度框架


# 项目目标及举措

- 安全：
	* 页面、菜单、接口都有鉴权
	* 日志系统
	* 黑名单系统
	* 页面权限及参数管理页面
    * ...

- 性能：
	* 大规模使用缓存
	* 及物理缓存

- 可配置化
	- 文章配置
		* Configs.Site
		* Configs.Wechat
		* Configs.Ding
		* 且配置界面都是自动生成的
	- 数据库路由：RouteModule
	- 内容管理：ContentModule

- 精简代码：
	- DAL层代码简化：
		* EntityBase & App.Data
		* 写实体类极其简单，且访问非常舒服
	- UI层简化：
		* List-Form 结构，如Articles.aspx & ArticleForm.aspx 可实现增删改查所有逻辑
		* GridPro & FormBase ： 将共性抽取出来，程序员只需要关注实体的读写即可，无需关注页面元素变更逻辑
		* UI 类：精简所有UI操作，都是一条语句摆平
		* 自动数据编辑网页：Datas.aspx 
	- HttpModule 层简化
		* 缩略图：ImageModules.cs，访问图片的缩略图只需要加 w=100的参数即可
	- 工具层简化
		* App.Utils：众多的扩展方法，统统一个方法，且串联表示
	- 数据接口层简化
		* 采用HttpApi: 包含角色、权限、缓存、异常、鉴权等逻辑，代码极度简洁
- 文档：
	- 网页即文档
	- DataModules.aspx
	- DataModel.aspx
    - DbSchema.aspx

- 监控
- 热更新系统
	热补丁
	热插件


# 编码规范

- 编码规则参考《c# 简明规范》


# 页面代码安全性要求

- 所有页面都必须继承 PageBase 或 FormPage
- 所有页面都必加上 AuthAttribute
    - 校验访问模式：[Auth(Power.UserView, Power.UserNew, Power.UserEdit)]
    - 校验登陆状态：[Auth(AuthLogin=true)]
    - 校验URL签名： [Auth(AuthSign=true)]
- 查看后台管理页面 Explorer.aspx，检测每个页面的安全校验能力


# 数据访问层编码

## 本框架采用EntityFramework，并用 EntityBase 做了封装

``` csharp
    public class User : EntityBase<User>{...}

    // New and save
    var u = new User();
    u.Name = "Kevin";
    u.Save();

    // Get and modify
    var user = User.Get(5);
    var user = User.Get(t => t.UserID == 5);
    user.Age = 20;
    user.Save();

    // Search and Bind
    var users = User.Search(t => t.Name.Contains("Kevin")).ToList();
    var data = users.SortAndPage(t => t.Name, true, 0, 50);
    DataGrid1.DataSource = data;
    DataGrid1.DataBind();

    // transaction
    using (var transaction = AppContext.Current.Database.BeginTransaction())
    {
        try
        {
            var orderItem = new OrderItem(....);
            var order = new Order(....);
            orderItem.Save();
            order.Save();
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
        }
    }
    ```

EF 中的可空类型

	- 如果属性类型是数值类型
		public int  Count {get; set;}    // 会生成不可为空的数据字段
		public int?  Count {get; set;}   // 会生成可为空的数据字段
	- 前者会造成很多异常，故在本系统的模型设计中：
		- 数值类型全部定义为可空类型
		- 是否填写值在程序中进行控制，避免影响数据库操作


数据结构的设计

	通用的表设计
	独立功能的页面及控制（通用的界面会导致很多问题）

逻辑删除

	- 实体类实现 IDeleteLogic
	- 查询语句中加上 Where(t => t.InUsed != false)

物理删除
``` csharp
    /// <summary>物理删除用户（并删除关联部门、角色、职位等数据）</summary>
    /// <remarks>随着实际项目功能扩展，还可能需要删除很多表。用户表非常重要，建议使用逻辑删除。</remarks>
    public static void DeleteUsersPhysical(List<int> ids)
    {
        Set.Where(t => ids.Contains(t.ID)).Where(t => t.Name != "admin").ToList()
            .ForEach(t => { t.Titles = null; t.Dept = null; t.Roles = null; });
        Db.SaveChanges();
        Set.Where(t => ids.Contains(t.ID)).Where(t => t.Name != "admin").Delete();
    }
```

事务 Transaction

``` csharp
    // 用 Transaction 控制异常回滚
    using (var transaction = AppContext.Current.Database.BeginTransaction())
    {
        try
        {
            var order = Order.Create(type, userId, ShopId);
            order.AddItem(productSpecId, n);
            transaction.Commit();
            return order;
        }
        catch
        {
            transaction.Rollback();
            return null;
        }
    }
```

多状态查询

``` csharp
    public static IQueryable<Order> Search(
        OrderStatus[] status, string userName="", 
        DateTime? startDt=null, DateTime? endDt=null, 
        int? userId=null, string userMobile="", string serialNo=""
        )
    {
        IQueryable<Order> q = Set.Include(t => t.FirstProduct).Include(t => t.User);
        if (!String.IsNullOrEmpty(userName))   q = q.Where(t => t.User.Name.Contains(userName));
        if (!String.IsNullOrEmpty(userMobile)) q = q.Where(t => t.User.Mobile.Contains(userMobile));
        if (!String.IsNullOrEmpty(serialNo))   q = q.Where(t => t.SerialNo.Contains(serialNo));
        if (status != null)                    q = q.Where(t => status.Contains(t.Status.Value));
        if (startDt != null)                   q = q.Where(t => t.CreateDt >= startDt);
        if (endDt != null)                     q = q.Where(t => t.CreateDt <= endDt);
        if (userId != null && userId != -1)    q = q.Where(t => t.UserID == userId);

        return q;
    }
```




