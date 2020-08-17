<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderItemAssets.aspx.cs" Inherits="App.Pages.OrderItemAssets" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
    <f:Panel ID="Panel1" runat="server" BodyPadding="0px" ShowBorder="false" ShowHeader="false" Layout="Fit">
        <Toolbars>
            <f:Toolbar ID="Toolbar1" runat="server">
                <Items>
                    <f:ToolbarFill runat="server" />
                    <f:TextBox runat="server"    ID="tbUser" EmptyText="用户" Width="100" />
                    <f:TextBox runat="server"    ID="tbSerialNo" EmptyText="串号" Width="100" />
                    <f:Button runat="server" ID="btnSearch" Icon="SystemSearch" Text="查找" OnClick="btnSearch_Click" Type="Submit"  />
                </Items>
            </f:Toolbar>
        </Toolbars>
        <Items>
            <f:GridPro ID="Grid1" runat="server"  WinHeight="400" NewText="新增设备" DeleteText="删除设备">
                <Columns>
                    <f:BoundField HeaderText="时间"  DataField="CreateDt" SortField="CreateDt" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                    <f:BoundField HeaderText="名称" DataField="Asset.Name" SortField="Name" />
                    <f:BoundField HeaderText="串号"  DataField="Asset.SerialNo" SortField="SerialNo" />
                    <f:BoundField HeaderText="质保开始"  DataField="Asset.InsuranceStartDt" DataFormatString="{0:yyyy-MM-dd}" />
                    <f:BoundField HeaderText="质保结束"  DataField="Asset.InsuranceEndDt" DataFormatString="{0:yyyy-MM-dd}" />
                    <f:WindowField HeaderText="客户"  DataTextField="User.NickName" DataIFrameUrlFields="UserID" WindowID="Window1" DataIFrameUrlFormatString="UserForm.aspx?id={0}&md=view&search=false" Width="100px" />
                </Columns>
            </f:GridPro>
        </Items>
    </f:Panel>
    </form>
</body>
</html>
