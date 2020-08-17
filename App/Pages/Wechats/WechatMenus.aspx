<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WechatMenus.aspx.cs" Inherits="App.Tests.WechatMenus" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>微信相关功能</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" runat="server" AutoSizePanelID="form2" />
        <f:Form ID="form2" IsFluid="true" BodyPadding="10px" runat="server" EnableCollapse="false" Title="微信公众号菜单"
            ShowBorder="true"  ShowHeader="false" AutoScroll="true">
            <Toolbars>
                <f:Toolbar runat="server">
                    <Items>
                        <f:Button runat="server" ID="btnDefault" Text="默认配置" Icon="DatabaseGear" OnClick="btnDefault_Click" />
                        <f:Button runat="server" ID="btnGetMenu" Text="获取菜单" Icon="DatabaseGear"  OnClick="btnGetMenu_Click"/>
                        <f:Button runat="server" ID="btnSetMenu" Text="设置菜单" Icon="DatabaseGear"  OnClick="btnSetMenu_Click" />
                        <f:Button runat="server" ID="btnDeleteMenu" Text="删除菜单" Icon="DatabaseGear"  OnClick="btnDeleteMenu_Click" />
                        <f:HyperLink runat="server" NavigateUrl="https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1421141013" Text="官方文档" Target="_blank" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:TextArea runat="server" ID="tbMenu"  ShowLabel="false"  Height="600px" />
            </Items>
        </f:Form>
    </form>
</body>
</html>