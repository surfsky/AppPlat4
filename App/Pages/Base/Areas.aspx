<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Areas.aspx.cs" Inherits="App.Admins.Areas" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Grid1" runat="server" />
        <f:GridPro ID="Grid1" runat="server" WinHeight="500" WinCloseAction="HidePostBack"/>
    </form>
</body>
</html>
