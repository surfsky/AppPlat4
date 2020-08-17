<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="App.Pages.Users" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" BodyPadding="0px"  
            ShowBorder="false" Layout="VBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start"
            ShowHeader="false" Title="用户管理">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
		                <f:PopupBox runat="server" ID="pbShop" EmptyText="商店" WinTitle="商店" UrlTemplate="shops.aspx" />
                        <f:DropDownList runat="server" ID="ddlRole" EmptyText="角色" Width="100" Hidden="true" />
                        <f:DropDownList runat="server" ID="ddlStatus" EmptyText="用户状态" Width="100" />
                        <f:TextBox runat="server" ID="tbName" EmptyText="用户名/昵称" Width="100" />
                        <f:TextBox runat="server" ID="tbMobile" EmptyText="手机号" Width="100" />
                        <f:Button runat="server" ID="btnSearch" Text="查找" OnClick="btnSearch_Click"  Type="Submit" Icon="SystemSearch" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:GridPro ID="Grid1" runat="server" AllowExport="true" WinHeight="700" WinWidth="800" OnExport="Grid1_Export" >
                    <Columns>
                        <f:WindowField ColumnID="changePasswordField" TextAlign="Center" Icon="Key" ToolTip="修改密码"
                            WindowID="Window1" Title="修改密码" DataIFrameUrlFields="ID" DataIFrameUrlFormatString="/pages/Base/UserPassword.aspx?id={0}"
                            Width="30px"  />
                        <f:ThrumbnailField DataImageUrlField="Avatar" HeaderText="照片" ImageWidth="30" />
                        <f:BoundField DataField="Name" SortField="Name" Width="100px" HeaderText="用户名" />
                        <f:BoundField DataField="NickName" SortField="NickName" Width="100px" HeaderText="昵称" />
                        <f:CheckBoxField DataField="InUsed" SortField="InUsed" HeaderText="启用" RenderAsStaticField="true"  Width="50px" />
                        <f:WindowField TextAlign="Center" Width="60px" Text="设备" HeaderText="设备" Title="设备" 
                            WindowID="Window1" DataIFrameUrlFields="ID" DataIFrameUrlFormatString="UserAssets.aspx?userId={0}&search=false"
                            />
                        <f:WindowField TextAlign="Center" Width="60px" Text="财务" HeaderText="财务" Title="财务" 
                            WindowID="Window1" DataIFrameUrlFields="ID" DataIFrameUrlFormatString="UserFinances.aspx?userId={0}&md=view&search=false"
                            />
                        <f:WindowField TextAlign="Center" Width="60px" Text="积分" HeaderText="积分" Title="积分"  DataTextField="FinanceScore"
                            WindowID="Window1" DataIFrameUrlFields="ID" DataIFrameUrlFormatString="UserScores.aspx?userId={0}&search=false"
                            />
                        <f:BoundField DataField="RoleNames" Width="200px" HeaderText="角色" Hidden="false" />
                        <f:BoundField DataField="Gender" SortField="Gender" Width="50px" HeaderText="性别" />
                        <f:BoundField DataField="Mobile" SortField="Mobile" Width="150px" HeaderText="移动电话" />
                        <f:BoundField DataField="Shop.AbbrName"  Width="100px" HeaderText="商店"  />
                        <f:BoundField DataField="CreateDt"  Width="100px" HeaderText="注册时间" SortField="CreateDt" DataFormatString="{0:yyyy-MM-dd}"  />
                        <f:BoundField DataField="Inviter.NickName"  Width="100px" HeaderText="邀请人"  />
                        <f:BoundField DataField="Area.FullName"  Width="100px" HeaderText="区域"  ExpandUnusedSpace="true" />
                    </Columns>
                </f:GridPro>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
