<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Histories.aspx.cs" Inherits="App.Pages.Histories" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
    <f:Panel ID="Panel1" runat="server" BodyPadding="0px" ShowBorder="false" ShowHeader="false" Layout="Fit">
        <Toolbars>
            <f:Toolbar ID="Toolbar1" runat="server">
                <Items>
                    <f:ToolbarFill runat="server" />
                    <f:TextBox runat="server"    ID="tbUser" EmptyText="用户" Width="100" />
                    <f:TextBox runat="server"    ID="tbMobile" EmptyText="手机" Width="100" />
                    <f:DatePicker runat="server" ID="dpCreate" EmptyText="开始时间" />
                    <f:Button runat="server" ID="btnSearch" Icon="SystemSearch" Text="查找" OnClick="btnSearch_Click" Type="Submit"  />
                    <f:Button runat="server" ID="btnFlow" Icon="ChartLine" Text="流程" OnClick="btnFlow_Click" />
                </Items>
            </f:Toolbar>
        </Toolbars>
        <Items>
            <f:GridPro ID="Grid1" runat="server"  WinHeight="400" />
        </Items>
    </f:Panel>
    </form>
</body>
</html>
