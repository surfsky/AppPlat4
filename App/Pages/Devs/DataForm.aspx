<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataForm.aspx.cs" Inherits="App.Pages.DataForm" ValidateRequest="false"  %>


<!DOCTYPE html>
<html>
<head runat="server">
    <title>数据编辑</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="form2" runat="server" />
        <f:FormPro ID="form2" runat="server" AutoScroll="true" />
    </form>
</body>
</html>
