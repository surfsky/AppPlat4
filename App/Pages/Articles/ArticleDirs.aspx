<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ArticleDirs.aspx.cs" Inherits="App.Admins.ArticleDirs" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Grid1" runat="server" />
        <f:GridPro ID="Grid1" runat="server" TreeExpandOnDblClick="true" WinHeight="600"/>
    </form>
</body>
</html>
