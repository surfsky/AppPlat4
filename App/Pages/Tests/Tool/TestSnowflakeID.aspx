<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestSnowflakeID.aspx.cs" Inherits="App.Tests.TestSnowflakeID" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" runat="server" />
        <f:Form ID="form2" IsFluid="true" BodyPadding="10px" runat="server" EnableCollapse="false"
            ShowBorder="true" Title="SnowflakeID 生成测试" ShowHeader="true">
            <Toolbars>
                <f:Toolbar runat="server">
                    <Items>
                        <f:Button ID="btnSubmit" runat="server" ValidateForms="form2" Text="生成SnowflakeID" OnClick="btnSubmit_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:TextArea runat="server" ID="tbIDS" Height="600" />
            </Items>
        </f:Form>
    </form>
</body>
</html>