# ������Žӿڷ���������

Q ������Žӿڷ���������

A
	https://help.aliyun.com/knowledge_detail/57717.html?spm=5176.12438469.0.0.5bc71cbeNDmAlH
	isv.BUSINESS_LIMIT_CONTROL
	Ĭ�����أ�������֤�� ��ʹ��ͬһ��ǩ������ͬһ���ֻ����뷢�Ͷ�����֤�룬֧��1��/���ӣ�5��/Сʱ ���ۼ�10��/�졣
	���ã����ŷ��񡷹�����Ϣ���á�����Ƶ�����á�һ����Ȼ���ڶ��ŷ�������������40�����ֵ��

Q ��κϲ�������� HttpApi ����

A ���ȡ���£�������ѵ�ǰ�û��Ƿ���޵�����Ҳ���䵽�ͻ���
    ��δ���������ݣ������У�
    ��1���ÿͻ����ٵ��� GetArticleApproval �ӿ�
    ��2���ѵ������ݷŵ� APIResult.Extra ��ȥ
    ��3���ѵ���������ϵ� APIResult.Data ����ȥ�����һ���װ���� {Article={...}, Approval=true}
    ��3���ѵ���������ϵ� APIResult.Data ����ȥ����Ϊ���ԡ�
    ����(4)��Ȼ�Կͻ�����Ϊ�Ѻã���C#��ǿ���ͣ�ʵ�ֱȽ��鷳����������У�
    ��1���÷���ķ�������̬��������
    ��2����JObject��������̬��������
    ��3����FreeDictionary���� JObject ���
    ע��dynamic ֻ�ܵ��������Ѿ����ڵķ��������ԣ��޷���̬���ӷ��������ԡ�
    ʵ������
``` csharp
    [HttpApi("��ȡ��������", true, CacheSeconds = 60)]
    public static APIResult GetArticle(long articleId)
    {
        // �������ݼ���������
        var item = Article.GetDetail(articleId, Common.LoginUser?.ID).Export(ExportType.Detail);
        var approval = Article.HasApproval(articleId, Common.LoginUser.ID);

        // ����JObject�ķ�ʽ�ϲ��������
        var jsonsetting = HttpApiConfig.Instance.JsonSetting;
        var o = item.AsJObject(jsonsetting).AddProperty("Approval", approval);
        return item.ToResult();
    }
```

# Windows Search

<https://docs.microsoft.com/zh-cn/windows/win32/search/-search-3x-wds-overview>

Windows ȫ�ļ�������
- WDS(Windows Desktop Search), �ѹ�ʱ����Ϊ Windows XP �� Windows Server 2003 ������ڡ�
- Indexing Service �ѹ�ʱ��������WinXP��
- Windows Search: �Ƽ�


Windows Search is composed of three components:
- Windows Search Service (WSS)
- Development platform: https://docs.microsoft.com/zh-cn/windows/win32/search/-search-3x-wds-development-ovr
- User interface

����
https://docs.microsoft.com/zh-cn/windows/win32/search/-search-3x-wds-propertymappings

ʾ��
https://github.com/Microsoft/Windows-classic-samples/tree/master/Samples/Win7Samples/winui/WindowsSearch
https://docs.microsoft.com/zh-cn/windows/win32/search/-search-samples-ovw


Q: �ļ���������
A:
    (1)
    var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);  // �����
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
