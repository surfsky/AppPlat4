<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Regist.aspx.cs" Inherits="App.Pages.Regist" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <script src="/HttpApi/User/js"></script>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0, user-scalable=no" />
    <style>
        .bg {background-color:lightgrey;}
    </style>
</head>
<body class="bg">
    <f:PageManager runat="server" />
    <form id="form1" runat="server">
        <f:Window ID="Window1" runat="server" Title="注册（绑定手机）" IsModal="false" EnableClose="false"
            WindowPosition="GoldenSection" Width="500px">
            <Items>
                <f:Form ID="Form3" runat="server" ShowBorder="true" BodyPadding="10px" ShowHeader="false" Title="表单">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TextBox runat="server" ID="tbMobile" Label="手机" Required="true" ShowRedStar="true" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextBox runat="server" ID="tbSmsCode" Label="短信验证码" Required="true" ShowRedStar="true" />
                                <f:Button runat="server" ID="btnGetSmsCode" Text="获取验证码" Margin="0 0 0 10" OnClick="btnGetSmsCode_Click"/>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextBox runat="server" ID="tbPassword" Label="登录密码" Required="true" ShowRedStar="true" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextBox runat="server" ID="tbInviteCode" Label="邀请码" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextBox runat="server" ID="tbInfo"  Label="邀请信息" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:Button runat="server" ID="btnRegist" Text="注册" Icon="UserAdd" OnClick="btnRegist_Click" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                    <Items>
                    </Items>
                </f:Form>
            </Items>
        </f:Window>
    </form>
</body>
</html>
