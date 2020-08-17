<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Invites.aspx.cs" Inherits="App.Admins.Invites" %>

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
		            <f:PopupBox runat="server" ID="pbInviteShop" EmptyText="邀请商店" WinTitle="商店" UrlTemplate="stops.aspx" />
                    <f:TextBox runat="server"    ID="tbInviterName" EmptyText="邀请人昵称" Width="100" />
                    <f:TextBox runat="server"    ID="tbInviterMobile" EmptyText="邀请人手机" Width="100" />
                    <f:TextBox runat="server"    ID="tbInviteeName" EmptyText="受邀人昵称" Width="100" />
                    <f:TextBox runat="server"    ID="tbInviteeMobile" EmptyText="受邀人手机" Width="100" />
                    <f:DatePicker runat="server" ID="dpStart" EmptyText="开始时间" Width="100" />
                    <f:Button runat="server" ID="btnSearch" Icon="SystemSearch" Text="查找" OnClick="btnSearch_Click" Type="Submit"  />
                </Items>
            </f:Toolbar>
        </Toolbars>
        <Items>
            <f:GridPro ID="Grid1" runat="server" WinHeight="400" >
                <Columns>
                    <f:BoundField DataField="SourceName"    SortField="Source" HeaderText="来源" Width="140px" />
                    <f:BoundField DataField="StatusName"    SortField="Status" HeaderText="状态" Width="140px" />
                    <f:WindowField WindowID="Window1" ToolTip="查看" HeaderText="邀请商店" Width="100px" 
                        DataTextField="InviteShop.AbbrName" 
                        DataIFrameUrlFields="InviteShopID"  DataIFrameUrlFormatString="ShopForm.aspx?md=view&id={0}" 
                        />
                    <f:WindowField WindowID="Window1" ToolTip="查看" HeaderText="邀请人" Width="100px" 
                        DataTextField="Inviter.NickName" SortField="Inviter.Name"
                        DataIFrameUrlFields="InviterID"  DataIFrameUrlFormatString="UserForm.aspx?md=view&id={0}" 
                        />
                    <f:BoundField DataField="Inviter.Mobile"    HeaderText="邀请人手机" Width="140px" />
                    <f:WindowField WindowID="Window1" ToolTip="查看" HeaderText="受邀人" Width="100px" 
                        DataTextField="Invitee.NickName" SortField="Invitee.Name"
                        DataIFrameUrlFields="Invitee.ID"  DataIFrameUrlFormatString="UserForm.aspx?md=view&id={0}" 
                        />
                    <f:BoundField DataField="InviteeMobile"    SortField="InviteeMobile" HeaderText="受邀者手机" Width="140px"/>
                    <f:BoundField DataField="InviteShopAwarded" SortField="InviteShopAwarded" HeaderText="邀请商店已奖励" Width="140px" />
                    <f:BoundField DataField="InviterAwarded"     SortField  ="InviterAwarded" HeaderText="邀请人已奖励" Width="140px" />
                    <f:BoundField DataField="CreateDt" SortField="CreateDt" DataFormatString="{0:yyyy-MM-dd HH:mm}" Width="150px" HeaderText="创建日期" />
                    <f:BoundField DataField="RegistDt"    SortField="RegistDt" DataFormatString="{0:yyyy-MM-dd HH:mm}" HeaderText="用户注册日期" Width="140px" />
                    <f:BoundField DataField="Remark"    SortField="Remark" HeaderText="备注" Width="140px" ExpandUnusedSpace="true" />
                </Columns>
            </f:GridPro>
        </Items>
    </f:Panel>
    </form>
</body>
</html>
