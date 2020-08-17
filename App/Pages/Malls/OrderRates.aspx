<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderRates.aspx.cs" Inherits="App.Pages.OrderRates" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" BodyPadding="0px" ShowBorder="false" Layout="Fit" ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:ToolbarFill runat="server" />
                        <f:TextBox runat="server" ID="tbTitle" EmptyText="内容" Width="100" />
                        <f:Button runat="server" ID="btnSearch" Text="查找" OnClick="btnSearch_Click"  Type="Submit" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:GridPro ID="Grid1" runat="server"/>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
