<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Feedbacks.aspx.cs" Inherits="App.Admins.Feedbacks" %>

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
                    <f:DropDownList runat="server" ID="ddlType" EmptyText="类型" Width="100" />
                    <f:DropDownList runat="server" ID="ddlStatus" EmptyText="状态"  Width="100"/>
                    <f:DropDownList runat="server" ID="ddlApp" EmptyText="应用"  Width="100"/>
                    <f:TextBox runat="server" ID="tbAppVersion" EmptyText="版本号"  Width="100"/>
                    <f:TextBox runat="server" ID="tbKeyword" EmptyText="关键字"  Width="100"/>
                    <f:Button runat="server" ID="btnSearch" Text="查找" OnClick="btnSearch_Click"  Type="Submit" Icon="SystemSearch" />
                    <f:Button runat="server" ID="btnFlow" Text="流程" EnablePostBack="false" Icon="ChartLine" />
                </Items>
            </f:Toolbar>
        </Toolbars>
        <Items>
            <f:GridPro ID="Grid1" runat="server" WinHeight="700" WinWidth="800"/>
        </Items>
    </f:Panel>
 
    </form>
</body>
</html>
