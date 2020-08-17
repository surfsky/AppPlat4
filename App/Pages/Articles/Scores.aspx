<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Scores.aspx.cs" Inherits="App.Admins.Scores" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <style>
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" BodyPadding="0px"  
            ShowBorder="false" Layout="VBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start"
            ShowHeader="false" Title="积分管理">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />                 
                        <f:TextBox runat="server" ID="tbName" EmptyText="用户名" Width="100" />
                        <f:PopupBox runat="server" ID="pbDept" UrlTemplate="~/pages/Base/depts.aspx" Multiselect="false" EmptyText="部门" Width="200" />
                        <f:DatePicker runat="server" ID="dpStart" EmptyText="开始时间" Width="100" />
                        <f:DatePicker runat="server" ID="dpEnd" EmptyText="结束时间" Width="100" />
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

