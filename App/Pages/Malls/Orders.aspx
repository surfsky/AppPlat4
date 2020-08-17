<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Orders.aspx.cs" Inherits="App.Pages.Orders" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" BodyPadding="0px" ShowBorder="false" Layout="Fit" ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:Button ID="btnClose" ValidateForms="SimpleForm1" Icon="SystemSaveClose" OnClick="btnClose_Click" runat="server" Text="关闭" Hidden="true" />
                        <f:DropDownList ID="ddlType" runat="server" EmptyText="类别" Width="80" />
                        <f:DropDownList ID="ddlSts" runat="server" EmptyText="状态" Width="80" />
		                <f:PopupBox runat="server" ID="pbShop" EmptyText="商店" WinTitle="商店" UrlTemplate="Shops.aspx" />
                        <f:TextBox runat="server" ID="tbSerialNo" EmptyText="订单号" Width="100" />
                        <f:TextBox runat="server" ID="tbName" EmptyText="用户" Width="100"/>
                        <f:TextBox runat="server" ID="tbMobile" EmptyText="手机号" Width="100" />
                        <f:DatePicker ID="dpStart" runat="server" EmptyText="开始日期" Width="100" />
                        <f:DatePicker ID="dpEnd" runat="server" EmptyText="结束日期" Width="100" Hidden="true" />
                        <f:Button runat="server" ID="btnSearch" Text="查找" OnClick="btnSearch_Click"  Type="Submit" Icon="SystemSearch" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:GridPro ID="Grid1" runat="server" WinWidth="900" WinHeight="800"  AllowExport="true"  DeleteText="作废" OnExport="Grid1_Export">
                    <Columns>
                        <f:BoundField DataField="CreateDt" SortField="CreateDt" HeaderText="创建日期" Width="140" />
                        <f:BoundField DataField="SerialNo" SortField="SerialNo" Width="140px" HeaderText="订单流水号" />
                        <f:WindowField  DataTextField="StatusName" DataIFrameUrlFields="UniID" WindowID="Window1" DataIFrameUrlFormatString="/Pages/Workflows/Histories.aspx?key={0}&md=view&search=false" Width="100px" HeaderText="状态" Title="状态" />
                        <f:BoundField DataField="TypeName"  Width="60px" HeaderText="类别" />
                        <f:BoundField DataField="Summary" SortField="Summary" Width="200px" HeaderText="概述"/>
                        <f:BoundField DataField="TotalMoney" SortField="TotalMoney" Width="60px" HeaderText="费用" />
                        <f:BoundField DataField="Shop.AbbrName"  Width="100px" HeaderText="创建商店"/>
                        <f:BoundField DataField="HandleShop.AbbrName"  Width="100px" HeaderText="受理商店"/>

                        <f:WindowField DataTextField="User.NickName" DataIFrameUrlFields="UserID" WindowID="Window1" DataIFrameUrlFormatString="UserForm.aspx?id={0}&md=view" Width="100px" HeaderText="客户" />
                        <f:WindowField Text="评分" DataTextField="LastRate" SortField="LastRate" DataIFrameUrlFields="ID" WindowID="Window1" DataIFrameUrlFormatString="OrderRates.aspx?orderId={0}&md=view&search=false" Width="100px" HeaderText="评分" />
                        <f:BoundField DataField="Remark"  Width="200px" HeaderText="备注" ExpandUnusedSpace="true"/>
                    </Columns>
                </f:GridPro>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
