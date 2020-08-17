# AppPlat ����ָ��

- Author: surfsky@sina,com
- For: Developer


# ��Ŀ�ܹ�

- ������Ŀ
    - App               ��վ
    - App.BLL           ҵ�����
    - App.Tests         ҵ��������
    - App.Monitor       ����̨��س���
- ����ģ��
    - App.Utils         UI�޹�ͨ�����
    - App.HttpApi       ���ݽӿڿ��
    - App.Scheduler     ���ȿ��


# ��ĿĿ�꼰�ٴ�

- ��ȫ��
	* ҳ�桢�˵����ӿڶ��м�Ȩ
	* ��־ϵͳ
	* ������ϵͳ
	* ҳ��Ȩ�޼���������ҳ��
    * ...

- ���ܣ�
	* ���ģʹ�û���
	* ��������

- �����û�
	- ��������
		* Configs.Site
		* Configs.Wechat
		* Configs.Ding
		* �����ý��涼���Զ����ɵ�
	- ���ݿ�·�ɣ�RouteModule
	- ���ݹ���ContentModule

- ������룺
	- DAL�����򻯣�
		* EntityBase & App.Data
		* дʵ���༫��򵥣��ҷ��ʷǳ����
	- UI��򻯣�
		* List-Form �ṹ����Articles.aspx & ArticleForm.aspx ��ʵ����ɾ�Ĳ������߼�
		* GridPro & FormBase �� �����Գ�ȡ����������Աֻ��Ҫ��עʵ��Ķ�д���ɣ������עҳ��Ԫ�ر���߼�
		* UI �ࣺ��������UI����������һ������ƽ
		* �Զ����ݱ༭��ҳ��Datas.aspx 
	- HttpModule ���
		* ����ͼ��ImageModules.cs������ͼƬ������ͼֻ��Ҫ�� w=100�Ĳ�������
	- ���߲��
		* App.Utils���ڶ����չ������ͳͳһ���������Ҵ�����ʾ
	- ���ݽӿڲ��
		* ����HttpApi: ������ɫ��Ȩ�ޡ����桢�쳣����Ȩ���߼������뼫�ȼ��
- �ĵ���
	- ��ҳ���ĵ�
	- DataModules.aspx
	- DataModel.aspx
    - DbSchema.aspx

- ���
- �ȸ���ϵͳ
	�Ȳ���
	�Ȳ��


# ����淶

- �������ο���c# �����淶��


# ҳ����밲ȫ��Ҫ��

- ����ҳ�涼����̳� PageBase �� FormPage
- ����ҳ�涼�ؼ��� AuthAttribute
    - У�����ģʽ��[Auth(Power.UserView, Power.UserNew, Power.UserEdit)]
    - У���½״̬��[Auth(AuthLogin=true)]
    - У��URLǩ���� [Auth(AuthSign=true)]
- �鿴��̨����ҳ�� Explorer.aspx�����ÿ��ҳ��İ�ȫУ������


# ���ݷ��ʲ����

## ����ܲ���EntityFramework������ EntityBase ���˷�װ

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

EF �еĿɿ�����

	- ���������������ֵ����
		public int  Count {get; set;}    // �����ɲ���Ϊ�յ������ֶ�
		public int?  Count {get; set;}   // �����ɿ�Ϊ�յ������ֶ�
	- ǰ�߻���ɺܶ��쳣�����ڱ�ϵͳ��ģ������У�
		- ��ֵ����ȫ������Ϊ�ɿ�����
		- �Ƿ���дֵ�ڳ����н��п��ƣ�����Ӱ�����ݿ����


���ݽṹ�����

	ͨ�õı����
	�������ܵ�ҳ�漰���ƣ�ͨ�õĽ���ᵼ�ºܶ����⣩

�߼�ɾ��

	- ʵ����ʵ�� IDeleteLogic
	- ��ѯ����м��� Where(t => t.InUsed != false)

����ɾ��
``` csharp
    /// <summary>����ɾ���û�����ɾ���������š���ɫ��ְλ�����ݣ�</summary>
    /// <remarks>����ʵ����Ŀ������չ����������Ҫɾ���ܶ���û���ǳ���Ҫ������ʹ���߼�ɾ����</remarks>
    public static void DeleteUsersPhysical(List<int> ids)
    {
        Set.Where(t => ids.Contains(t.ID)).Where(t => t.Name != "admin").ToList()
            .ForEach(t => { t.Titles = null; t.Dept = null; t.Roles = null; });
        Db.SaveChanges();
        Set.Where(t => ids.Contains(t.ID)).Where(t => t.Name != "admin").Delete();
    }
```

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

��״̬��ѯ

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




