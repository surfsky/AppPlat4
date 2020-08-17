<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Adverts.aspx.cs" Inherits="App.Pages.Adverts" %>
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
                    <f:TextBox ID="tbTitle" runat="server" EmptyText="标题" />
                    <f:DropDownList ID="ddlType" runat="server" EmptyText="类别" />
                    <f:DropDownList ID="ddlStatus" runat="server" EmptyText="状态" />
                    <f:Button ID="btnSearch" runat="server" Text="查找" OnClick="btnSearch_Click"  Type="Submit" />
                </Items>
            </f:Toolbar>
        </Toolbars>
        <Items>
            <f:GridPro ID="Grid1" runat="server" WinHeight="650"/>
        </Items>
    </f:Panel>
 
    </form>
</body>
</html>
