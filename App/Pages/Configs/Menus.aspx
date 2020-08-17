<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Menus.aspx.cs" Inherits="App.Admins.Menus" %>

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
                <f:Toolbar runat="server" ID="toolbar">
                    <Items>
                        <f:CheckBox runat="server" ID="chkError" Text="仅错误" AutoPostBack="true" OnCheckedChanged="chkError_CheckedChanged" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:GridPro ID="Grid1" runat="server" WinHeight="600" WinWidth="1000"
                    OnRowCommand="Grid1_RowCommand"
                    >
                    <Columns>
                        <f:LinkButtonField CommandName="MoveTop" Width="22px" TextAlign="Center" Icon="BulletArrowTop" ToolTip="上移到顶部" />
                        <f:LinkButtonField CommandName="MoveUp" Width="22px" TextAlign="Center" Icon="BulletArrowUp" ToolTip="上移一位"/>
                        <f:LinkButtonField CommandName="MoveDown" Width="22px" TextAlign="Center" Icon="BulletArrowDown" ToolTip="下移一位"/>
                        <f:LinkButtonField CommandName="MoveBottom" Width="22px" TextAlign="Center" Icon="BulletArrowBottom" ToolTip="下移到底部" />
                    </Columns>
                </f:GridPro>

            </Items>
        </f:Panel>
    </form>
</body>
</html>
