<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="App.Admins.Users" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <style>
        .f-grid-cell.hide a{display: none;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" BodyPadding="0px"  
            ShowBorder="false" Layout="VBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start"
            ShowHeader="false" Title="用户管理">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <f:PopupBox runat="server" ID="pbDept" UrlTemplate="Depts.aspx" Multiselect="false" EmptyText="部门" Width="200" />
                        <f:DropDownList runat="server" ID="ddlTitle" EmptyText="职称" Width="100" />
                        <f:DropDownList runat="server" ID="ddlRole" EmptyText="角色" Width="100" />
                        <f:DropDownList runat="server" ID="ddlStatus" EmptyText="用户状态" Width="100" />
                        <f:TextBox runat="server" ID="tbName" EmptyText="用户名" Width="100" />
                        <f:TextBox runat="server" ID="tbMobile" EmptyText="手机号" Width="100" />
                        <f:Button runat="server" ID="btnSearch" Text="查找" OnClick="btnSearch_Click"  Type="Submit" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:GridPro ID="Grid1" runat="server" WinHeight="700" OnRowDataBound="Grid1_RowDataBound" />
            </Items>
        </f:Panel>
    </form>
</body>
</html>
