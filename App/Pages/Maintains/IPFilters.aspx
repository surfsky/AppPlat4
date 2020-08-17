<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IPFilters.aspx.cs" Inherits="App.Admins.IPFilters" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>黑名单管理</title>
</head>
<body>
    <form id="form1" runat="server">
    <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
    <f:Panel ID="Panel1" runat="server" BodyPadding="0px"  ShowBorder="false" Layout="Fit" ShowHeader="false" >
        <Toolbars>
            <f:Toolbar ID="Toolbar1" runat="server">
                <Items>
                    <f:ToolbarFill runat="server" />
                    <f:SearchBox ID="ttbSearchMessage" runat="server" ShowLabel="false" EmptyText="搜索IP"   OnTriggerClick="ttbSearchMessage_TriggerClick" />
                </Items>
            </f:Toolbar>
        </Toolbars>
        <Items>
            <f:GridPro ID="Grid1" runat="server" WinHeight="300"/>
        </Items>
    </f:Panel>
 
    </form>
</body>
</html>
