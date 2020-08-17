<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Shops.aspx.cs" Inherits="App.Pages.Shops" %>
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
                    <f:DropDownList runat="server" ID="ddlArea" EmptyText="区域" />
                    <f:TextBox ID="tbName" runat="server" EmptyText="名称" />
                    <f:Button ID="btnSearch" runat="server" Text="查找" Icon="SystemSearch" OnClick="btnSearch_Click"  Type="Submit"  />
                </Items>
            </f:Toolbar>
        </Toolbars>
        <Items>
            <f:GridPro ID="Grid1" runat="server" WinWidth="800" WinHeight="600" />
        </Items>
    </f:Panel>
 
    </form>
</body>
</html>
