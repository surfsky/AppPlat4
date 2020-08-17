# App.Plat ����

2019-10
CJH

## HttpModule ����

``` xml
<system.webServer>
<staticContent>
    <remove fileExtension=".doc" />
    <remove fileExtension=".docx" />
    <remove fileExtension=".xls" />
    <remove fileExtension=".xlsx" />
    <remove fileExtension=".ppt" />
    <remove fileExtension=".pptx" />
    <remove fileExtension=".pdf" />
    <remove fileExtension=".jpg" />
    <remove fileExtension=".png" />
    <remove fileExtension=".gif" />
</staticContent>
<modules>
    <!-- ͼƬ������ͼ����Module������ʱ��Ҫɾ����Ӧ�� MimeType��jpg,jpeg,png,gif�� -->
    <add name="ImageModule" type="App.Components.ImageModule" />

    <!-- Office�ļ�����Module������ʱ��Ҫɾ����Ӧ�� MimeType: doc,docx,xls,xlsx,ppt,pptx,pdf -->
    <add name="OfficeModule" type="App.Components.OfficeModule" />

    <!-- ��Щ�����������⣬Module RemapHandler ���޷���ȡ Session��Ҫ�������� -->
    <remove name="Session" />
    <add name="Session" type="System.Web.SessionState.SessionStateModule" />
    <add name="HttpApiModule" type="App.HttpApi.HttpApiModule" />
</modules>
</system.webServer>
```

## �������������⼰�������

���⣺���ز���httpapi�ӿڻ���������ok�ģ������𵽷������Ϲ��ڻ���ʼ���޷�������������������޷�ˢ��
��������Թ��������������ս����������web.config/web������������������Ϣ��������iis�Զ����㻺���߼������ˣ�

```xml
<!-- ���⻺�汻�Զ����ա�������ڻ���ʼ���޷������� -->
<caching>
    <cache disableMemoryCollection = "true"
        disableExpiration = "false"
        privateBytesLimit = "20971520"
        percentagePhysicalMemoryUsedLimit = "60"
        privateBytesPollTime = "00:01:00"
        />
</caching>
```




