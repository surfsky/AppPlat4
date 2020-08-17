<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserAssets.aspx.cs" Inherits="App.Pages.UserAssets" %>

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
                    <f:DatePicker runat="server" ID="dpCreate" EmptyText="创建时间" />
                    <f:Button runat="server" ID="btnSearch" Icon="SystemSearch" Text="查找" OnClick="btnSearch_Click" Type="Submit"  />
                </Items>
            </f:Toolbar>
        </Toolbars>
        <Items>
            <f:GridPro ID="Grid1" runat="server"  WinHeight="400" NewText="新增设备" DeleteText="删除设备">
                <Columns>
                    <f:BoundField DataField="Name" SortField="Name" HeaderText="名称" />
                    <f:BoundField DataField="SerialNo" SortField="SerialNo" HeaderText="串号" />
                    <f:BoundField DataField="CreateDt" SortField="CreateDt" HeaderText="创建时间"  DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                    <f:BoundField DataField="InsuranceStartDt" SortField="InsuranceStartDt" HeaderText="质保开始"  DataFormatString="{0:yyyy-MM-dd}" />
                    <f:BoundField DataField="InsuranceEndDt" SortField="InsuranceEndDt" HeaderText="质保结束"  DataFormatString="{0:yyyy-MM-dd}" />
                    <f:WindowField DataTextField="User.NickName" DataIFrameUrlFields="UserID" WindowID="Window1" DataIFrameUrlFormatString="UserForm.aspx?id={0}&md=view&search=false" Width="100px" HeaderText="客户" />
                </Columns>
            </f:GridPro>
        </Items>
    </f:Panel>
    </form>
</body>
</html>
