C#
------------------------------------------

nameof() 表达式实现强类型
    var name = nameof(ApiCommon.QrCode));


注意
	切勿使用 Order.ToJson()扩展方法，会无限膨胀


Route
    // 注册路由
    RegisterRoutes(RouteTable.Routes);
    void RegisterRoutes(RouteCollection routes)
    {
        //routes.MapPageRoute("Default", "", "~/Default.aspx");
        //routes.MapPageRoute("WebForm1", "{floder}/{webform}", "~/{floder}/{webform}.aspx");
        //routes.MapPageRoute("WebForm2", "{floder}/{webform}/{parameter}", "~/{floder}/{webform}.aspx");
        //routes.MapPageRoute("HttpApi", "HttpApi/{Handler}/{Method}", "~/HttpApi.{Handler}.axd/{Method}");
        //routes.MapPageRoute("HttpApi", "HttpApi/{Handler}/{Method}", "~/HttpApi/Handler.ashx");
    }

动态设置TypeDescriptor
    // 动态设置 TypeDescriptor
    var oType = o.GetType();
    var ui = new UIAttribute() { Field = oType.GetProperty("Title"), Title = "标题", Name = "Title" };
    var descriptors = TypeDescriptor.GetProperties(o);
    PropertyDescriptor descriptor = descriptors["Title"];
    if (descriptor != null)
    {
        // 用反射来设置 AttributeArray 保护属性
        //descriptor.AttributeArray = new Attribute[] { ui };
        var attrs = ReflectionHelper.GetPropertyValue(descriptor, "AttributeArray") as Attribute[];
        var list = attrs.ToList();
        list.Add(ui);
        attrs = list.ToArray();
        ReflectionHelper.SetPropertyValue(descriptor, "AttributeArray", attrs);
    }


在aspx中只输出接口数据
    context.Response.Write(string.Format("Chunk {0} ok", index));
    context.Response.Flush();
    context.Response.End();
    // 不输出页面的其它内容。经测试，只能输出一个 chunk 4 ok，前面的都不见了，不知道怎么处理。
    // 使用Response.End 方法时，会因为内部呼叫Thread.Abort() 而引发ThreadAbortException 的例外，停止网页的执行，略过Response.End 以下的代码
    // 直接触发HTTP 管线（HTTP Pipelines）的执行链结里的最后一个事件，也就是HttpApplication.EndRequest 事件。 然后将缓冲输出的资料传送到用户端。
    // Flush的内容至少要有256字节。也就是只有编译产生了至少256字节的数据，才能在执行Response.Flush()以后将信息发到客户端并显示。
    // 所以在Page中进行输出控制是很困难的，还是直接用 HttpHandler 吧
    经过测试，该方法不稳定，还是老老实实的写成 httpapi 或者 handler
    
Default.aspx
    if (!IsPostBack)
    {
        if (User.Identity.IsAuthenticated)
            Response.Redirect(FormsAuthentication.DefaultUrl);
    }


-----------------------------------------------
JS
---------------------------------------------
json
    var s = JSON.stringify(result);
    alert(s);

jquery
    <script src="https://cdn.staticfile.org/jquery/3.2.1/jquery.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $("#btnSubmit").click(function () {
               var formData = new FormData(document.getElementById("form"));
                $.ajax({
                    url: '/HttpApi/Ding/Up' ,
                    type: 'POST',
                    data: formData,
                    async: false,
                    cache: false,
                    contentType: false,
                    processData: false,
                    success: function (result) {
                        var s = JSON.stringify(result);
                        alert(s);
                    }
                });
            });
        });
    </script>

Razor
static void Main(string[] args)
{
    string template = "Hello @Model.Name! Welcome to Razor!";
    string result = Razor.Parse(template, new { Name = "World" });
    Console.WriteLine(result);
    Console.Read();
}
