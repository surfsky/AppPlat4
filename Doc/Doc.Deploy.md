# App.Plat 部署

2019-10
CJH

## HttpModule 部署

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
    <!-- 图片及缩略图处理Module，部署时需要删除对应的 MimeType：jpg,jpeg,png,gif等 -->
    <add name="ImageModule" type="App.Components.ImageModule" />

    <!-- Office文件保护Module，部署时需要删除对应的 MimeType: doc,docx,xls,xlsx,ppt,pptx,pdf -->
    <add name="OfficeModule" type="App.Components.OfficeModule" />

    <!-- 有些服务器有问题，Module RemapHandler 后无法获取 Session，要加这两行 -->
    <remove name="Session" />
    <add name="Session" type="System.Web.SessionState.SessionStateModule" />
    <add name="HttpApiModule" type="App.HttpApi.HttpApiModule" />
</modules>
</system.webServer>
```

## 部署遇到的问题及解决方案

问题：本地测试httpapi接口缓存能力是ok的，但部署到服务器上过期缓存始终无法被清理掉，导致数据无法刷新
解决：尝试过无数方案，最终解决方案，在web.config/web节中增加如下配置信息，估计是iis自动计算缓存逻辑出错了：

```xml
<!-- 避免缓存被自动回收、避免过期缓存始终无法被回收 -->
<caching>
    <cache disableMemoryCollection = "true"
        disableExpiration = "false"
        privateBytesLimit = "20971520"
        percentagePhysicalMemoryUsedLimit = "60"
        privateBytesPollTime = "00:01:00"
        />
</caching>
```




