<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="App.Pages.Index" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
    <f:Panel ID="Panel1" runat="server" BodyPadding="0px"  ShowBorder="false" Layout="Fit" ShowHeader="false" >
        <Items>
            <f:Grid ID="Grid1" IsFluid="true" ShowBorder="true" ShowHeader="false" Title="表格" runat="server" EnableCollapse="false"
                DataKeyNames="Name">
                <Columns>
                    <f:RowNumberField />
                    <f:BoundField runat="server" DataField="Name" HeaderText="页面" Width="400px" />
                    <f:HyperLinkField runat="server" HeaderText="查看" Text="查看" DataNavigateUrlFields="Name" DataNavigateUrlFormatString="{0}" Target="_blank" ExpandUnusedSpace="true" />
                </Columns>
            </f:Grid>
        </Items>
    </f:Panel>
 
    </form>
</body>
</html>
