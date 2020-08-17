<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderItemForm.aspx.cs"  Inherits="App.Pages.OrderItemForm" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" ShowBorder="false" ShowHeader="false"  AutoScroll="true" runat="server">
            <Items>
                <f:Form ID="form2" ShowBorder="false" ShowHeader="false" runat="server" BodyPadding="10px">
                    <Items>
                        <f:DropDownList runat="server"  Label="商品"  ID="ddlProduct" Required="true" AutoPostBack="true" OnSelectedIndexChanged="ddlProduct_SelectedIndexChanged"/>
                        <f:DropDownList runat="server"  Label="规格"  ID="ddlProductSpec"  Required="true" AutoPostBack="true" OnSelectedIndexChanged="ddlProductSpec_SelectedIndexChanged" />
                        <f:TextBox runat="server"  Label="货号" ID="tbProductNo" Readonly="true" />
                        <f:NumberBox runat="server"  Label="价格" ID="tbPrice" Readonly="true" />
                        <f:NumberBox runat="server"  Label="库存" ID="tbStock" DecimalPrecision="0" Readonly="true" />
                        <f:NumberBox runat="server"  Label="数量" ID="tbAmount" DecimalPrecision="0" OnTextChanged="tbAmount_TextChanged" AutoPostBack="true" Required="true" ShowRedStar="true" />
                        <f:TextBox  runat="server" Label="明目" ID="tbTitle" Readonly="true" />
                        <f:NumberBox runat="server"  Label="金额" ID="tbMoney" DecimalPrecision="2" Readonly="true" />
                    </Items>
                </f:Form>
                <f:Panel runat="server" ID="panAsset" EnableIFrame="true"  Height="300px" ShowBorder="false" />
            </Items>
        </f:Panel>
    </form>
</body>
</html>
