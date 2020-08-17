<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserPassword.aspx.cs"  Inherits="App.Admins.UserPassword" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>管理员设置用户密码</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" ShowBorder="false" ShowHeader="false"  AutoScroll="true" runat="server" BodyPadding="10px">

            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:Button ID="btnClose" Icon="SystemClose" EnablePostBack="false" runat="server" Text="关闭" />
                        <f:Button ID="btnSaveClose" ValidateForms="SimpleForm1" Icon="SystemSaveClose"
                            OnClick="btnSaveClose_Click" runat="server" Text="保存后关闭" />
                        <f:Button ID="btnSet" Icon="PasteWord" runat="server" Text="设为默认密码" OnClick="btnSet_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>

            <Items>
                <f:Label ID="lblUserName" Label="用户名" runat="server" />
                <f:Label ID="lblNickName" Label="昵称" runat="server" />
                <f:TextBox ID="tbPassword" runat="server" Label="密码" Required="true" ShowRedStar="true" TextMode="Password" />
                <f:TextBox ID="tbConfirmPassword" runat="server" Label="确认密码" Required="true"  ShowRedStar="true" TextMode="Password" CompareControl="tbPassword" CompareOperator="Equal" CompareMessage="密码不一致" />
            </Items>
        </f:Panel>
    </form>
</body>
</html>
