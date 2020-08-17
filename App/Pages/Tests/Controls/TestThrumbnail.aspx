<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestThrumbnail.aspx.cs" Inherits="App.Tests.TestThrumbnail" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" runat="server" AutoSizePanelID="form2" />
        <f:Form ID="form2" IsFluid="true" BodyPadding="10px" runat="server" EnableCollapse="false"
            ShowBorder="true" Title="表单" ShowHeader="true">
            <Items>
                <f:Thrumbnail runat="server" ID="img" />
                <f:FileUpload runat="server" ID="file" AcceptFileTypes="image/*" OnFileSelected="file_FileSelected"  AutoPostBack="true" ButtonOnly="true"  />
                <f:Button runat="server" ID="btnGet" Text="获取图片地址" OnClick="btnGet_Click" />
            </Items>
        </f:Form>
    </form>
</body>
</html>