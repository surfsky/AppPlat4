<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderPay.aspx.cs"  Inherits="App.Pages.OrderPay" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="form2" runat="server" />
        <f:Form ID="form2" ShowBorder="false" ShowHeader="false"  AutoScroll="true" runat="server"  BodyPadding="10px">
            <Toolbars >
                <f:Toolbar runat="server" >
                    <Items>
                        <f:Button runat="server" ID="btnOk" Text="提交" Icon="SystemSave" OnClick="btnOk_Click" ValidateForms="form2" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:FormRow runat="server">
                    <Items>
                        <f:TextBox runat="server" ID="tbID" Label="订单编号"  Readonly="true" Hidden="false" />
                        <f:TextBox runat="server" ID="tbGUID" Label="订单流水号" Required="true" ShowRedStar="true" />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:TextBox runat="server" ID="tbSummary" Label="概述" />
                        <f:Label runat="server" ID="lblStatus" Label="状态"/>
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:DropDownList runat="server" ID="ddlPayMode" Label="支付方式" />
                        <f:Label runat="server" ID="lblPayDt" Label="支付时间"  />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:NumberBox runat="server" ID="tbTotalMoney" Label="总费用" Enabled="false"/>
                        <f:NumberBox runat="server" ID="tbPayMoney" Label="实付费用" Required="true" ShowRedStar="true"/>
                    </Items>
                </f:FormRow>
            </Items>
        </f:Form>
    </form>
</body>
</html>
