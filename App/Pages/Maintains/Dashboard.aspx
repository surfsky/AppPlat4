<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="App.Admins.Dashboard" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <asp:Literal runat="server" ID="ltCss" />
</head>
<body>
    <form id="form1" runat="server">
    <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
    <f:Panel ID="Panel1" runat="server" ShowBorder="True" ShowHeader="false" Layout="VBox" BodyPadding="5">
        <Toolbars>
            <f:Toolbar ID="Toolbar1" runat="server">
                <Items>
                    <f:Button runat="server" ID="btnClearCache" Icon="CupDelete" Text="清除缓存" OnClick="btnClearCache_Click" />
                    <f:Button runat="server" ID="btnReboot" Icon="ControlPowerBlue" Text="重启网站" OnClick="btnReboot_Click" ConfirmText="确定重启网站吗？重启后将无任何提示，请手动刷新页面。" />
                </Items>
            </f:Toolbar>
        </Toolbars>
        <Items>
            <f:Panel ID="Panel5" BoxFlex="1" runat="server" ShowBorder="false" ShowHeader="false" Layout="HBox" BoxConfigChildMargin="0 5 0 0" Margin="0 0 5 0">
                <Items>
                    <f:Panel ID="Panel2" BoxFlex="1" runat="server" ShowBorder="false" ShowHeader="true" Title="运行状态" BodyPadding="5px"  Layout="Card"  AutoScroll="true"  >
                        <Items>
                            <f:Label runat="server" ID="lbl1" CssStyle="width: 300px;height:200px;" EncodeText="false" />
                        </Items>
                    </f:Panel>
                    <f:Panel ID="Panel9" BoxFlex="1" runat="server" ShowBorder="false" ShowHeader="true" Title="程序集" BodyPadding="5px"  Layout="Card"  AutoScroll="true"  >
                        <Items>
                            <f:Label runat="server" ID="lbl2" CssStyle="width: 300px;height:200px;" EncodeText="false" />
                        </Items>
                    </f:Panel>
                </Items>
            </f:Panel>

            <f:Panel ID="Panel11" BoxFlex="1" runat="server" ShowBorder="false" ShowHeader="false" Layout="HBox" BoxConfigChildMargin="0 5 0 0">
                <Items>
                    <f:Panel ID="Panel12" BoxFlex="1" runat="server" ShowBorder="false" ShowHeader="true" BodyPadding="5px" Title="参数信息" Layout="Card" AutoScroll="true" >
                        <Items>
                            <f:Label runat="server" ID="lbl3" CssStyle="width: 300px;height:200px;" EncodeText="false" />
                        </Items>
                    </f:Panel>
                    <f:Panel ID="Panel13" BoxFlex="1" runat="server" ShowBorder="false" ShowHeader="true" BodyPadding="5px" Title="硬件信息" Layout="Card" AutoScroll="true" >
                        <Items>
                            <f:Label runat="server" ID="lbl4" CssStyle="width: 300px;height:200px;" EncodeText="false" />
                        </Items>
                    </f:Panel>
                </Items>
            </f:Panel>
        </Items>
    </f:Panel>

    </form>
    <script type="text/javascript">
        $(function () {
            $.get('Dashboard.aspx/GetComputerInfo', 
                function (result) {
                    var s = JSON.stringify(result);
                    $("#<%=lbl4.ClientID%>").html(s);
                }
            );
        });

    </script>

</body>
</html>
