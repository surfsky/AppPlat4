<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserScores.aspx.cs" Inherits="App.Pages.UserScores" %>

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
                    <f:Button runat="server" ID="btnExchange" Text="积分兑换" OnClick="btnExchange_Click" Icon="Basket" />
                    <f:ToolbarFill runat="server" />
                    <f:TextBox runat="server"    ID="tbUser" EmptyText="用户" Width="100" />
                    <f:DatePicker runat="server" ID="dpCreate" Label="创建时间" />
                    <f:Button runat="server" ID="btnSearch" Icon="SystemSearch" Text="查找" OnClick="btnSearch_Click" Type="Submit"  />
                </Items>
            </f:Toolbar>
        </Toolbars>
        <Items>
            <f:GridPro ID="Grid1" runat="server"  WinHeight="400"/>
        </Items>
    </f:Panel>
    </form>
</body>
</html>
