# FAQ

- EF 如何获取实体类对应的表名？


# 包含逻辑

Has方法：

	- HasOptional：前者包含后者一个实例或者为null
	- HasRequired：前者(A)包含后者(B)一个不为null的实例
	- HasMany：前者包含后者实例的集合

With方法：

	- WithOptional：后者(B)可以包含前者(A)一个实例或者null
	- WithRequired：后者包含前者一个不为null的实例
	- WithMany：后者包含前者实例的集合


照片必须有归属某个人，某个人有0到N张照片

    ``` csharp
    public class Person
    {
	    public virtual List<Photo> Photos {get; set;}
    }
    pubic class Photo
    {
	    public int PersonID {get; set;}
	    public Person Person {get;set;}
    }
    Photo.HasRequired(p => p.Person).WithOptional(p => p.Photos);
    Person.HasMany(p=> p.Photos).WithRequired(p => p.Person);
    ```

景点类Destination和住宿类Lodging是一对多的关系，

    - 一个景点有一个或多个住宿的地方
    - 一个住宿的地方只属于一个景点，或者不属于任何景点
    - 景点.HasMany(d => d.Lodgings).WithRequired(l => l.Destination).Map(l => l.MapKey("DestinationId"));
    - 住宿.HasRequired(d => d.Destination).WithMany(l => l.Lodgings).Map(l => l.MapKey("DestinationId"));


# 一对多关系

    class User
    {
        public virutal ICollection<Role> Roles {get;set;}
    }
    /*
    // User <-> Role
    modelBuilder.Entity<User>()
        .HasMany(r => r.Roles)
        .Map(x => x.ToTable("UserRoles")
        .MapLeftKey("UserID")
        .MapRightKey("RoleID")
        );
    */




# Expression

    /// <summary>添加元素到集合中</summary>
    private void Add<T>(T collection, object obj)
    {
        var methodInfo = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(m => m.Name.Equals("Add"));
        if (methodInfo == null)
            throw new Exception($"反序列化集合xml内容失败，目标{typeof(T).FullName}非集合类型");

        var instance = Expression.Constant(collection);
        var param = Expression.Constant(obj);
        var addExpression = Expression.Call(instance, methodInfo, param);
        var add = Expression.Lambda<Action>(addExpression).Compile();
        add.Invoke();
    }

ExportExpression
    /// <summary>导出对象表达式（备选方案）</summary>
    /// <example>
    /// var items = User.Search(....).Select(User.GetExportExpression(false)).ToList();
    /// </example>
    public static Expression<Func<T, object>> GetExportExpression(bool detail = true)
    {
        return null;
    }
	public new static Expression<Func<Order, object>> GetExportFields()
    {
        return o => new
        {
            o.ID,
            o.SerialNo,
            o.Sts,
            o.Summary,
            o.ShopID,
            ShopName = o.Shop.Name,
            o.CreateDt,
            o.PayMoney,
            o.Histories,
            o.Items
        };
    }

导出数据
    /// <summary>导出数据（会合并数据）</summary>
    public Dictionary<string, object> ExportData(ExportType type = ExportType.Normal)
    {
        object simpleObj = (type.HasFlag(ExportType.Simple)) ? Export(ExportType.Simple) : null;
        object normalObj = (type.HasFlag(ExportType.Normal)) ? Export(ExportType.Normal) : null;
        object detailObj = (type.HasFlag(ExportType.Detail)) ? Export(ExportType.Detail) : null;
        return ReflectionHelper.CombineObject(simpleObj, normalObj, detailObj);
    }

# DbSet

    // AppContext.DbSet<User>
    var ps = typeof(AppContext).GetProperties();
    foreach (var p in ps.OrderBy(m => m.Name))
    {
        var type = p.PropertyType;
        if (type.IsGenericType && type.Name.Contains("DbSet"))
        {
            var entityType = type.GetGenericDataType();
            if (entityType == t)
                return AppContext.Current.GetPropertyValue(p.Name);
        }
    }
    return null;


# 统计
    /// <summary>日增统计</summary>
    public static List<StatItem> StatDayNew<T>(DateTime startDt, DateTime? endDt) where T : EntityBase
    {
        IQueryable<T> q = AppContext.GetEntitySet(typeof(T)) as IQueryable<T>;
        q = q.Where(t => t.CreateDt >= startDt);
        if (endDt != null)
            q = q.Where(t => t.CreateDt <= endDt);

        // 每日的增量数据
        var items = q
            .GroupBy(t => new
            {
                Day = DbFunctions.TruncateTime(t.CreateDt).Value
            })
            .Select(t => new
            {
                Day = t.Key.Day,
                Cnt = t.Count()
            })
            .OrderBy(t => new { t.Day })
            .ToList()
            .Select(t => new StatItem("", t.Day.ToString("MMdd"), t.Cnt))
            .ToList()
            ;
        ;
        return items;
    }

    /// <summary>日存量统计</summary>
    public static List<StatItem> StatDayAmount<T>(DateTime startDt, DateTime? endDt) where T : EntityBase
    {
        IQueryable<T> q = AppContext.GetEntitySet(typeof(T)) as IQueryable<T>;
        var n = q.Where(t => t.CreateDt < startDt).Count();    // 初始数据
        var items = StatDayNew<T>(startDt, endDt);             // 每日新增数据

        // 存量 = 之前量 + 今日增量
        return items
            .Each2((item, preItem) => item.Value = item.Value + (preItem?.Value ?? 0))
            .Each(t => t.Value += n)
            ;
    }

# UserTitles 逻辑
    model
        public virtual List<Title> Titles { get; set; }
        // 表间的关联关系
        // User <--> Title（效果同建立一张表 TitleUsers(TitleID, UserID)
        modelBuilder.Entity<Title>()
            .HasMany(t => t.Users)
            .WithMany(u => u.Titles)
            .Map(x => x.ToTable("TitleUsers").MapLeftKey("TitleID").MapRightKey("UserID"))
            ;
        /// <summary>用户拥有的头衔字符串（用逗号隔开） </summary>
        public string TitleNames
        {
            get
            {
                string titles = "";
                foreach (var item in this.Titles)
                    titles += item.Name + ",";
                titles = titles.TrimEnd(',');
                return titles;
            }
        }
    grid
        //.AddColumn<User>(t => t.Titles, 100, "职务")  //
        // 设置头衔
        //string titles = user.Titles.Select(t => t.Name).ToSeparatedString();
        //UI.SetGridCellText(Grid1, "Titles", titles, e);
    form
        UI.Bind(cblTitle, DAL.Title.Set, t => t.ID, t => t.Name, null);
        UI.SetValues(this.cblTitle, item.Titles?.Cast(t => t.ID));
        item.Titles = DAL.Title.GetTitles(UI.GetLongs(this.cblTitle).ToList());


EntityBase
        /// <summary>导出字典</summary>
        private Dictionary<string, object> ExportDict(ExportType type = ExportType.Normal)
        {
            var o = new Dictionary<string, object>();
            foreach (PropertyInfo pi in this.GetType().GetProperties())
            {
                // 名称和值
                var name = pi.Name;
                var value = pi.GetValue(this);

                // 根据 UI 标注判断是否需要导出
                bool needExport = false;
                var ui = pi.GetUIAttribute();
                if (ui == null)
                    needExport = true;
                else
                {
                    if (ui.Export.HasFlag(type))
                        needExport = true;
                }

                // 递归导出该字段值
                if (needExport && value != null)
                {
                    if (value is IExport)
                        value = (value as IExport).Export(type);
                    else if (value is IList)
                    {
                        var list = value as IList;
                        value = list.Cast(t => (t is IExport) ? (t as IExport).Export(type) : t);
                    }
                    o.Add(name, value);
                }
            }
            return o;
        }

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