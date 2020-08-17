<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Datas.aspx.cs" Inherits="App.Pages.Datas" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>数据列表</title>
</head>
<body>
    <form id="form1" runat="server">
    <f:PageManager ID="PageManager1" AutoSizePanelID="Grid1" runat="server" />
    <f:GridPro ID="Grid1" runat="server" WinWidth="1000" WinHeight="600" EnableTextSelection="true"/>
    </form>
</body>
</html>
