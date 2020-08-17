<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignForm.aspx.cs" Inherits="App.Admins.SignForm" %>

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
                <f:PopupBox runat="server" ID="pbUser" Label="经手人" WinTitle="选择用户" UrlTemplate="users.aspx" />
                <f:DateTimePicker runat="server" ID="dpSign" Label="签到时间" />
                <f:NumberBox ID="tbScore" Label="得分" runat="server" />
            </Items>
        </f:Form>
    </form>
</body>
</html>
