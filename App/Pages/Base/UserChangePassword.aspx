<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserChangePassword.aspx.cs" Inherits="App.Admins.Profiles" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>用户更改自己的密码</title>
</head>
<body>
    <form id="form1" runat="server">
    <f:PageManager ID="PageManager1" AutoSizePanelID="form2" runat="server" />
    <f:Form ID="form2" runat="server" LabelWidth="100px" BodyPadding="10px"
        LabelAlign="Left"  ShowBorder="false" ShowHeader="false" AutoScroll="true"
        >
        <Toolbars>
            <f:Toolbar runat="server">
                <Items>
                    <f:Button ID="btnSave" runat="server" Icon="SystemSave" OnClick="btnSave_OnClick"
                        ValidateForms="SimpleForm1" ValidateTarget="Top" Text="修改"
                        />
                </Items>
            </f:Toolbar>
        </Toolbars>

        <Items>
            <f:TextBox ID="tbOldPassword" TextMode="Password" runat="server" Label="当前密码" Required="true" ShowRedStar="true" />
            <f:TextBox ID="tbNewPassword" TextMode="Password" runat="server" Label="新密码" Required="true" MinLength="5" ShowRedStar="true" />
            <f:TextBox ID="tbConfirmPassword" runat="server" Label="确认新密码" Required="true"  ShowRedStar="true" TextMode="Password" CompareControl="tbPassword" CompareOperator="Equal" CompareMessage="密码不一致" />
        </Items>
    </f:Form>
    </form>
</body>
</html>
