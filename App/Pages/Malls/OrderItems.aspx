<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderItems.aspx.cs" Inherits="App.Pages.OrderItems" %>

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
                        <f:ToolbarFill runat="server" />
                        <f:TextBox runat="server" ID="tbTitle" EmptyText="明目" Width="100" />
                        <f:Button runat="server" ID="btnSearch" Text="查找" OnClick="btnSearch_Click"  Type="Submit" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:GridPro ID="Grid1" runat="server" WinWidth="800"  WinHeight="700" NewText="新增商品" DeleteText="删除商品">
                    <Columns>
                        <f:WindowField HeaderText="商品" Title="商品" WindowID="Window1" DataTextField="Product.Name" DataIFrameUrlFields="Product.ID" DataIFrameUrlFormatString="ProductForm.aspx?id={0}&md=view" Width="100px"  />
                        <f:WindowField HeaderText="规格" Title="规格" WindowID="Window1" DataTextField="ProductSpecName" DataIFrameUrlFields="ProductSpecID" DataIFrameUrlFormatString="ProductSpecForm.aspx?id={0}&md=view" Width="100px"  />
                        <f:WindowField HeaderText="设备" Title="设备" WindowID="Window1" Text="设备"  DataIFrameUrlFields="ID" DataIFrameUrlFormatString="OrderItemAssets.aspx?orderItemId={0}&search=false" Width="100px" />
                        <f:BoundField DataField="Title" SortField="Title" Width="100px" HeaderText="明目" />
                        <f:BoundField DataField="Price" SortField="Price" Width="100px" HeaderText="单价" />
                        <f:BoundField DataField="Cnt" SortField="Cnt" Width="100px" HeaderText="数量" />
                        <f:BoundField DataField="Money" SortField="Money" Width="100px" HeaderText="金额" />
                        <f:BoundField DataField="ProductSpecCode" SortField="ProductSpecCode" Width="100px" HeaderText="货号" ExpandUnusedSpace="true"/>
                    </Columns>
                </f:GridPro>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
