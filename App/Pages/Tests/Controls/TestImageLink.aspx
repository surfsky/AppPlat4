<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestImageLink.aspx.cs" Inherits="App.Tests.TestImageLink" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" runat="server" AutoSizePanelID="Panel1" />
        <f:Panel ID="Panel1" runat="server" BodyPadding="0px"  ShowBorder="false" ShowHeader="false" >
            <Items>
                <f:Form runat="server" ID="form2" Title="ImageLink" BodyPadding="10px" Height="300px" ShowBorder="true" ShowHeader="true">
                    <Items>
                        <f:HyperLink runat="server" ID="lnkImage" Target="_blank" Text="<img src='/res/wechat.jpg?w=100' />" EncodeText="false" NavigateUrl="/res/wechat.jpg" />
                        <f:Image ID="img" ImageUrl="~/res/images/blank.png" ShowEmptyLabel="true" runat="server" ImageHeight="160"/>
                    </Items>
                </f:Form>
                <f:Grid runat="server" ID="Grid1" Title="ImageLinkField" ShowHeader="true" >
                    <Columns>
                        <f:ThrumbnailField HeaderText="Image" DataImageUrlField="Url"  Width="80px"  />
                        <f:ImageField HeaderText="Image" DataImageUrlField="Url" DataImageUrlFormatString="{0}?w=50" Width="80px" ImageWidth="50" />
                        <f:HyperLinkField HeaderText="ImageLink" Width="80px" DataTextField="Url" DataTextFormatString="<img src='{0}?w=50'/>" DataNavigateUrlFields="Url" HtmlEncode="false" UrlEncode="false" />
                        <f:BoundField HeaderText="大小" DataField="FileSize" SortField="FileSize" Width="100px"  />
                        <f:BoundField HeaderText="时间" DataField="CreateDt" SortField="CreateDt" Width="200px"  />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
    <script>
        F.ready(function () {
            // 图像点击事件
            var imgId = '#<%= img.ClientID %>';
            var img = $(imgId).find('img');
            var url = img.attr('data-tag');

            // 直接包裹一层a 标签
            img.wrap("<a href=\"" + url + "\" target=_blank></a>");

            /*
            // 用tab方式展示
            F(imgId).el.on('click', function (event) {
                var root = getRoot(window);
                root.addMainTab(imgId, url, "Image");
            });
            */
        });
    </script>
</body>
</html>