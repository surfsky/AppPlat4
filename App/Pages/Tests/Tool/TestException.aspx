<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestException.aspx.cs" Inherits="App.Tests.TestException" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" runat="server" />
        <f:Form ID="form2" IsFluid="true" BodyPadding="10px" runat="server" EnableCollapse="false"
            ShowBorder="true" Title="表单" ShowHeader="true">
            <Items>
                <f:Button ID="btnSubmit" runat="server" ValidateForms="form2" Text="提交表单" OnClick="btnSubmit_Click" />
                <f:HyperLink runat="server" NavigateUrl="~/Pages/Maintains/Logs.aspx" Text="查看日志" />
            </Items>
        </f:Form>
    </form>

</body>
</html>