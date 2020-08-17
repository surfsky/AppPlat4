<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserAssetForm.aspx.cs" Inherits="App.Pages.UserAssetForm" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="form2" runat="server" />
        <f:Form ID="form2" ShowBorder="false" ShowHeader="false" runat="server" BodyPadding="10px" AutoScroll="true">
            <Items>
                <f:Label runat="server" ID="lblId" Label="ID" />
                <f:PopupBox runat="server" ID="pbUser" Label="用户" WinTitle="选择用户" UrlTemplate="users.aspx" Readonly="true" />
                <f:Label runat="server" ID="lblCreateDt" Label="创建时间" />
                <f:TextBox runat="server" ID="tbName" Label="名称" ShowRedStar="true" />
                <f:TextBox runat="server" ID="tbSerialNo" Label="串号" ShowRedStar="true" />
                <f:DateTimePicker runat="server" ID="dpInsuranceStart" Label="保修开始日期" />
                <f:DateTimePicker runat="server" ID="dpInsuranceEnd" Label="保修结束日期" />
            </Items>
        </f:Form>
    </form>
</body>
</html>
