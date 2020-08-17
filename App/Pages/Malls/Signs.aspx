<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Signs.aspx.cs" Inherits="App.Admins.Signs" %>

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
                    <f:TextBox runat="server"    ID="tbUser" EmptyText="作者" Width="100" />
                    <f:DatePicker runat="server" ID="dpStart" EmptyText="开始时间" Width="100" />
                    <f:Button runat="server" ID="btnSearch" Icon="SystemSearch" Text="查找" OnClick="btnSearch_Click" Type="Submit"  />
                </Items>
            </f:Toolbar>
        </Toolbars>
        <Items>
            <f:GridPro ID="Grid1" runat="server" SortDirection="DESC" WinHeight="400">
                <Columns>
                    <f:WindowField WindowID="Window1" ToolTip="查看" HeaderText="用户" Width="100px" 
                        DataTextField="User.NickName" SortField="User.Name"
                        DataIFrameUrlFields="User.ID"  DataIFrameUrlFormatString="/pages/Base/UserForm.aspx?md=view&id={0}" 
                        />
                    <f:BoundField DataField="SignDt" SortField="SignDt" DataFormatString="{0:yyyy-MM-dd HH:mm}" Width="150px" HeaderText="签到时间" />
                    <f:BoundField DataField="Score"    SortField="Score" HeaderText="得分" Width="140px" />
                </Columns>
            </f:GridPro>
        </Items>
    </f:Panel>
    </form>
</body>
</html>
