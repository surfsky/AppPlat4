<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UIs.aspx.cs" Inherits="App.Pages.UIs" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>UI配置管理</title>
</head>
<body>
    <form id="form1" runat="server">
    <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
    <f:Panel ID="Panel1" runat="server" BodyPadding="0px"  ShowBorder="false" Layout="Fit" ShowHeader="false" >
        <Toolbars>
            <f:Toolbar ID="Toolbar1" runat="server">
                <Items>
                    <f:ToolbarFill runat="server" />
                    <f:DropDownList runat="server" ID="ddlEntityType"  EmptyText="实体类"  />
                    <f:Button runat="server" ID="btnSearch" Icon="SystemSearch" Text="查找" OnClick="btnSearch_Click" />
                </Items>
            </f:Toolbar>
        </Toolbars>
        <Items>
            <f:GridPro ID="Grid1" runat="server" WinHeight="700"/>
        </Items>
    </f:Panel>
 
    </form>
</body>
</html>
