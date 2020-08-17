<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserScoreExchange.aspx.cs" Inherits="App.Pages.UserScoreExchange" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="form2" runat="server" />
        <f:Form ID="form2" ShowBorder="false" ShowHeader="false" runat="server" BodyPadding="10px"  AutoScroll="true">
            <Toolbars>
                <f:Toolbar runat="server" >
                    <Items>
                        <f:Button runat="server" ID="btnExchange" Text="兑换" Icon="Basket" ConfirmText="确定兑换?" OnClick="btnExchange_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:PopupBox runat="server" ID="pbUser" Label="经手人" WinTitle="选择用户" UrlTemplate="users.aspx" Enabled="false" />
                <f:Label runat="server" ID="lblScore" Label="拥有积分" />
                <f:NumberBox runat="server" ID="tbScore" Label="兑换积分" DecimalPrecision="0" />
                <f:TextBox runat="server" ID="tbRemark" Label="用途" />
            </Items>
        </f:Form>
    </form>
</body>
</html>
