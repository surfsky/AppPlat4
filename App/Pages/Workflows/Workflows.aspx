<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Workflows.aspx.cs" Inherits="App.Pages.Workflows" %>

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
                    <f:DropDownList ID="ddlType" runat="server" ForceSelection="true" EmptyText="工作流类别" />
                    <f:Button runat="server" ID="btnSearch" Text="查找" Type="Submit" OnClick="btnSearch_Click" Icon="SystemSearch" />
                </Items>
            </f:Toolbar>
        </Toolbars>
        <Items>
            <f:GridPro ID="Grid1" runat="server"/>
        </Items>
    </f:Panel>
 
    </form>
</body>
</html>
