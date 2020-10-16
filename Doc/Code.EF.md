# FAQ

- EF ��λ�ȡʵ�����Ӧ�ı�����


# �����߼�

Has������

	- HasOptional��ǰ�߰�������һ��ʵ������Ϊnull
	- HasRequired��ǰ��(A)��������(B)һ����Ϊnull��ʵ��
	- HasMany��ǰ�߰�������ʵ���ļ���

With������

	- WithOptional������(B)���԰���ǰ��(A)һ��ʵ������null
	- WithRequired�����߰���ǰ��һ����Ϊnull��ʵ��
	- WithMany�����߰���ǰ��ʵ���ļ���


��Ƭ�����й���ĳ���ˣ�ĳ������0��N����Ƭ

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

������Destination��ס����Lodging��һ�Զ�Ĺ�ϵ��

    - һ��������һ������ס�޵ĵط�
    - һ��ס�޵ĵط�ֻ����һ�����㣬���߲������κξ���
    - ����.HasMany(d => d.Lodgings).WithRequired(l => l.Destination).Map(l => l.MapKey("DestinationId"));
    - ס��.HasRequired(d => d.Destination).WithMany(l => l.Lodgings).Map(l => l.MapKey("DestinationId"));


# һ�Զ��ϵ

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

    /// <summary>���Ԫ�ص�������</summary>
    private void Add<T>(T collection, object obj)
    {
        var methodInfo = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(m => m.Name.Equals("Add"));
        if (methodInfo == null)
            throw new Exception($"�����л�����xml����ʧ�ܣ�Ŀ��{typeof(T).FullName}�Ǽ�������");

        var instance = Expression.Constant(collection);
        var param = Expression.Constant(obj);
        var addExpression = Expression.Call(instance, methodInfo, param);
        var add = Expression.Lambda<Action>(addExpression).Compile();
        add.Invoke();
    }

ExportExpression
    /// <summary>����������ʽ����ѡ������</summary>
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

��������
    /// <summary>�������ݣ���ϲ����ݣ�</summary>
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


# ͳ��
    /// <summary>����ͳ��</summary>
    public static List<StatItem> StatDayNew<T>(DateTime startDt, DateTime? endDt) where T : EntityBase
    {
        IQueryable<T> q = AppContext.GetEntitySet(typeof(T)) as IQueryable<T>;
        q = q.Where(t => t.CreateDt >= startDt);
        if (endDt != null)
            q = q.Where(t => t.CreateDt <= endDt);

        // ÿ�յ���������
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

    /// <summary>�մ���ͳ��</summary>
    public static List<StatItem> StatDayAmount<T>(DateTime startDt, DateTime? endDt) where T : EntityBase
    {
        IQueryable<T> q = AppContext.GetEntitySet(typeof(T)) as IQueryable<T>;
        var n = q.Where(t => t.CreateDt < startDt).Count();    // ��ʼ����
        var items = StatDayNew<T>(startDt, endDt);             // ÿ����������

        // ���� = ֮ǰ�� + ��������
        return items
            .Each2((item, preItem) => item.Value = item.Value + (preItem?.Value ?? 0))
            .Each(t => t.Value += n)
            ;
    }

# UserTitles �߼�
    model
        public virtual List<Title> Titles { get; set; }
        // ���Ĺ�����ϵ
        // User <--> Title��Ч��ͬ����һ�ű� TitleUsers(TitleID, UserID)
        modelBuilder.Entity<Title>()
            .HasMany(t => t.Users)
            .WithMany(u => u.Titles)
            .Map(x => x.ToTable("TitleUsers").MapLeftKey("TitleID").MapRightKey("UserID"))
            ;
        /// <summary>�û�ӵ�е�ͷ���ַ������ö��Ÿ����� </summary>
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
        //.AddColumn<User>(t => t.Titles, 100, "ְ��")  //
        // ����ͷ��
        //string titles = user.Titles.Select(t => t.Name).ToSeparatedString();
        //UI.SetGridCellText(Grid1, "Titles", titles, e);
    form
        UI.Bind(cblTitle, DAL.Title.Set, t => t.ID, t => t.Name, null);
        UI.SetValues(this.cblTitle, item.Titles?.Cast(t => t.ID));
        item.Titles = DAL.Title.GetTitles(UI.GetLongs(this.cblTitle).ToList());


EntityBase
        /// <summary>�����ֵ�</summary>
        private Dictionary<string, object> ExportDict(ExportType type = ExportType.Normal)
        {
            var o = new Dictionary<string, object>();
            foreach (PropertyInfo pi in this.GetType().GetProperties())
            {
                // ���ƺ�ֵ
                var name = pi.Name;
                var value = pi.GetValue(this);

                // ���� UI ��ע�ж��Ƿ���Ҫ����
                bool needExport = false;
                var ui = pi.GetUIAttribute();
                if (ui == null)
                    needExport = true;
                else
                {
                    if (ui.Export.HasFlag(type))
                        needExport = true;
                }

                // �ݹ鵼�����ֶ�ֵ
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

���� Transaction

``` csharp
    // �� Transaction �����쳣�ع�
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