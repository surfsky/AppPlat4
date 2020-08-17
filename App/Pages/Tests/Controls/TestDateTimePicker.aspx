<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestDateTimePicker.aspx.cs" Inherits="App.Tests.TestDateTimePicker" %>

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
                <f:DatePicker runat="server" Required="true" Label="日期一" EmptyText="请选择日期一" ID="dp" ShowRedStar="true" />
                <f:DateTimePicker runat="server" Required="true" Label="日期和时间" EmptyText="请选择" ID="dtp" ShowRedStar="true" />
                <f:DateTimePicker runat="server" Required="true" Label="日期和时间" EmptyText="请选择" ID="dtp2" ShowRedStar="true" />
                <f:Button ID="btnSubmit" runat="server" ValidateForms="form2" Text="提交表单" OnClick="btnSubmit_Click" />
            </Items>
        </f:Form>
        <f:Label ID="labResult" ShowLabel="false" EncodeText="false" runat="server" />
    </form>
</body>
</html>