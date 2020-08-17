<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Articles.aspx.cs" Inherits="App.Admins.Articles" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
    <f:Panel ID="Panel1" runat="server" BodyPadding="0px"  ShowBorder="false" Layout="Fit" ShowHeader="false" >
        <Toolbars>
            <f:Toolbar ID="Toolbar1" runat="server">
                <Items>
                    <f:ToolbarFill runat="server" />
                    <f:DropDownList runat="server" ID="ddlType" EmptyText="类别"  />
                    <f:PopupBox runat="server" ID="pbDir" EmptyText="文章目录" WinTitle="文章目录" UrlTemplate="ArticleDirs.aspx" />
                    <f:TextBox runat="server" ID="tbKeywords" EmptyText="关键字" Width="200" />
                    <f:TextBox runat="server" ID="tbTitle" EmptyText="标题" Width="100" Hidden="true" />
                    <f:TextBox runat="server" ID="tbAuthor" EmptyText="作者" Width="100" Hidden="true" />
                    <f:DatePicker runat="server" ID="dpStart" EmptyText="开始时间" Width="100" Hidden="true" />
                    <f:DatePicker runat="server" ID="dpEnd" EmptyText="结束时间" Width="100"  Hidden="true" />

                    <f:Button runat="server" ID="btnSearch" Text="查找" OnClick="btnSearch_Click"  Icon="SystemSearch" Type="Submit" />
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
