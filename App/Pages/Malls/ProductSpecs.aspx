<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductSpecs.aspx.cs"  Inherits="App.Pages.ProductSpecs" ValidateRequest="false" %>

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
                <f:GridPro ID="Grid1" runat="server" WinWidth="800" WinHeight="500"
                    NewText="新增规格" DeleteText="删除规格"
                    OnDelete="Grid1_Delete"
                    >
                    <Columns>
                        <f:ThrumbnailField runat="server"  DataImageUrlField="CoverImage" HeaderText="图片" />
                        <f:BoundField runat="server"  DataField="Spec1" HeaderText="规格1" ColumnID="Spec1" />
                        <f:BoundField runat="server"  DataField="Spec2" HeaderText="规格2" ColumnID="Spec2" />
                        <f:BoundField runat="server"  DataField="Spec3" HeaderText="规格3" ColumnID="Spec3" />
                        <f:BoundField runat="server"  DataField="Price" HeaderText="价格" />
                        <f:BoundField runat="server"  DataField="Stock" HeaderText="库存" />
                        <f:BoundField runat="server"  DataField="Data" HeaderText="数量" />
                        <f:BoundField runat="server"  DataField="InsuranceDays" HeaderText="质保天数" />
                    </Columns>
                </f:GridPro>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
