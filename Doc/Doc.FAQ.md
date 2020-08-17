# 阿里短信接口发不出短信

Q 阿里短信接口发不出短信

A
	https://help.aliyun.com/knowledge_detail/57717.html?spm=5176.12438469.0.0.5bc71cbeNDmAlH
	isv.BUSINESS_LIMIT_CONTROL
	默认流控：短信验证码 ：使用同一个签名，对同一个手机号码发送短信验证码，支持1条/分钟，5条/小时 ，累计10条/天。
	设置：短信服务》国内消息设置》发送频率设置》一个自然日内短信发送条数不超过40（最大值）

Q 如何合并属性输出 HttpApi 数据

A 如获取文章，我们想把当前用户是否点赞的数据也传输到客户端
    如何传输点赞数据，方案有：
    （1）让客户端再调用 GetArticleApproval 接口
    （2）把点赞数据放到 APIResult.Extra 中去
    （3）把点赞数据组合到 APIResult.Data 里面去，多加一层封装，如 {Article={...}, Approval=true}
    （3）把点赞数据组合到 APIResult.Data 里面去，作为属性。
    方案(4)当然对客户端最为友好，但C#是强类型，实现比较麻烦，解决方案有：
    （1）用反射的方法，动态增加属性
    （2）用JObject方法，动态增加属性
    （3）用FreeDictionary，和 JObject 差不多
    注：dynamic 只能调用类中已经存在的方法和属性，无法动态增加方法和属性。
    实例代码
``` csharp
    [HttpApi("获取文章详情", true, CacheSeconds = 60)]
    public static APIResult GetArticle(long articleId)
    {
        // 文章数据及点赞数据
        var item = Article.GetDetail(articleId, Common.LoginUser?.ID).Export(ExportType.Detail);
        var approval = Article.HasApproval(articleId, Common.LoginUser.ID);

        // 采用JObject的方式合并输出数据
        var jsonsetting = HttpApiConfig.Instance.JsonSetting;
        var o = item.AsJObject(jsonsetting).AddProperty("Approval", approval);
        return item.ToResult();
    }
```

# Windows Search

<https://docs.microsoft.com/zh-cn/windows/win32/search/-search-3x-wds-overview>

Windows 全文检索方案
- WDS(Windows Desktop Search), 已过时，作为 Windows XP 和 Windows Server 2003 插件存在。
- Indexing Service 已过时，仅用于WinXP。
- Windows Search: 推荐


Windows Search is composed of three components:
- Windows Search Service (WSS)
- Development platform: https://docs.microsoft.com/zh-cn/windows/win32/search/-search-3x-wds-development-ovr
- User interface

属性
https://docs.microsoft.com/zh-cn/windows/win32/search/-search-3x-wds-propertymappings

示例
https://github.com/Microsoft/Windows-classic-samples/tree/master/Samples/Win7Samples/winui/WindowsSearch
https://docs.microsoft.com/zh-cn/windows/win32/search/-search-samples-ovw


Q: 文件锁定问题
A:
    (1)
    var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);  // 共享打开
    using (var writer = new StreamWriter(fs))
    {
        ...
        writer.Flush();
        writer.Close();
        fs.Close();
    }
    (2)
    var img = Image.FromFile(filepath);
    var bmp = new Bitmap(img);
    img.Dispose();
