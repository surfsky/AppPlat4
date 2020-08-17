<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderItemAssetForm.aspx.cs" Inherits="App.Pages.OrderItemAssetForm" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="form2" runat="server" />
        <f:Form ID="form2" ShowBorder="false" ShowHeader="false" runat="server" BodyPadding="10px"  AutoScroll="true">
            <Items>
                <f:Label runat="server" ID="lblId" Label="ID" />
                <f:Label runat="server" ID="lblCreateDt" Label="创建时间" />
                <f:Label runat="server" ID="lblUser" Label="用户" />
                <f:Label runat="server" ID="lblOrderItemID" Label="订单项ID" />
                <f:PopupBox runat="server" ID="pbAsset" Label="资产" WinTitle="资产" UrlTemplate="userAssets.aspx"  />
            </Items>
        </f:Form>
    </form>
</body>
</html>
