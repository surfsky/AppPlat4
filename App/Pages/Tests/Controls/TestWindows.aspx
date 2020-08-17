<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestWindows.aspx.cs" Inherits="App.Tests.TestWindows" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <script type="text/javascript"  src="/res/js/jquery-3.3.1.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" ShowBorder="false" ShowHeader="false"  AutoScroll="true" runat="server">
            <Toolbars>
                <f:Toolbar runat="server" >
                    <Items>
                        <f:Button runat="server" ID="btnWin" Text="弹窗" OnClick="btnWin_Click" />
                        <f:Label runat="server" ID="lblInfo" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Panel>
        <f:Window ID="win" Width="650px" Height="300px" Icon="TagBlue" Title="窗体一" Hidden="true" EnableIFrame="true"
            EnableMaximize="true" EnableCollapse="false" runat="server" EnableResize="true"  CloseAction="HidePostBack"
            IsModal="true" AutoScroll="true" BodyPadding="10px"  />
    </form>
</body>
</html>
