<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ArticleVisits.aspx.cs" Inherits="App.Admins.ArticleVisits" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" BodyPadding="0px" ShowBorder="false" Layout="Fit" ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:Button runat="server" ID="btnExport" Text="导出" Icon="PageExcel" OnClick="btnExport_Click" EnableAjax="false" DisableControlBeforePostBack="false" />
                        <f:ToolbarFill runat="server" />
                        <f:TextBox runat="server" ID="tbUser" EmptyText="用户" Width="150" />
                        <f:TextBox runat="server" ID="tbArticle" EmptyText="文章" Width="150" />
                        <f:PopupBox runat="server" ID="pbArticleDir" EmptyText="文章目录" WinTitle="文章目录" Width="150" />
                        <f:PopupBox runat="server" ID="pbDept" EmptyText="部门" WinTitle="部门" />
                        <f:DatePicker runat="server" ID="dpStart" EmptyText="开始时间" Width="100" />
                        <f:DropDownList runat="server" Label="应知应会" ID="ddlIsRequir" AutoPostBack="true" />
                        <f:Button runat="server" ID="btnSearch" Text="查找" OnClick="btnSearch_Click" Icon="SystemSearch" Type="Submit" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:GridPro ID="Grid1" runat="server" WinHeight="800" WinWidth="1000" />
            </Items>
        </f:Panel>

    </form>
</body>
</html>
