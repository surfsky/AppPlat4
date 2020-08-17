<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserFinanceForm.aspx.cs" Inherits="App.Pages.UserFinanceForm" %>

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
                <f:PopupBox runat="server" ID="pbUser" Label="经手人" WinTitle="选择用户" UrlTemplate="users.aspx" />
                <f:DropDownList runat="server" ID="ddlType" Label="类型" />
                <f:Label runat="server" ID="lblCreateDt" Label="创建时间" />
                <f:NumberBox runat="server" ID="tbMoney" Label="金额" DecimalPrecision="2" EmptyText="正值表示收入，负值表示支出" />
                <f:TextBox runat="server" ID="tbOrderId" Label="订单号" />
            </Items>
        </f:Form>
    </form>
</body>
</html>
