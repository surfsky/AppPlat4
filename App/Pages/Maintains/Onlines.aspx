<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Onlines.aspx.cs" Inherits="App.Admins.Onlines" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" BodyPadding="0px" ShowBorder="false" ShowHeader="false" Title="日志管理" Layout="Fit">
            <Toolbars>
                <f:Toolbar runat="server" ID="Toolbar1">
                    <Items>
                        <f:SearchBox ID="ttbSearchMessage" runat="server" ShowLabel="false" EmptyText="搜索用户名" OnTriggerClick="ttbSearchMessage_TriggerClick" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:GridPro ID="Grid1" runat="server" WinHeight="600"  WinWidth="1000"  />
            </Items>
        </f:Panel>
    </form>
</body>
</html>
