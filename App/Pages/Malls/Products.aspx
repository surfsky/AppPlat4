<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Products.aspx.cs" Inherits="App.Pages.Products" %>
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
                    <f:Button ID="btnClose" ValidateForms="SimpleForm1" Icon="SystemSaveClose" OnClick="btnClose_Click" runat="server" Text="关闭" Hidden="true" />
                    <f:ToolbarFill runat="server" />
                    <f:TextBox ID="tbName" runat="server" EmptyText="名称" />
                    <f:DropDownList ID="ddlType" runat="server" EmptyText="商品类别" />
                    <f:DropDownList ID="ddlOnShelf" runat="server" EmptyText="是否上架" />
                    <f:Button ID="btnSearch" runat="server" Text="查找" OnClick="btnSearch_Click"  Type="Submit" />
                </Items>
            </f:Toolbar>
        </Toolbars>
        <Items>
            <f:GridPro ID="Grid1" runat="server" WinHeight="650" WinWidth="800" DeleteText="下架" OnDelete="Grid1_Delete"/>
        </Items>
    </f:Panel>
 
    </form>
</body>
</html>
