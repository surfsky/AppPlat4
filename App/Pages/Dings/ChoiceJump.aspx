<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChoiceJump.aspx.cs" Inherits="App.Admins.ChoiceJump" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <style type="text/css">
        .bg {
            background-image: url(~/Res/images/CJbg.jpg) !important;
            width: 100%;
            height: 100%;
            border-width: 0 !important;
            background-color: transparent !important;
            background-position:center center ;
        }
        .bgbtn1 {
            background-image: url(~/Res/images/CJbtn01.png) !important;
            width: 100%;
            height: 100%;
            border-width: 0 !important;
            background-color: transparent !important;
        }

        .bgbtn2 {
            background-image: url(~/Res/images/CJbtn02.png) !important;
            width: 100%;
            height: 100%;
            border-width: 0 !important;
            background-color: transparent !important;
        }
    </style>
</head>
<body id="mybody" runat="server"  class="bg">
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" BodyPadding="0px" ShowBorder="false" ShowHeader="false" Layout="HBox"
            BoxConfigAlign="Center" BoxConfigPosition="Center">
            <Items>
                <f:Panel ID="Panel32" Title="面板1" Width="350px"
                    Height="350px" runat="server" BodyPadding="0px" ShowBorder="false" ShowHeader="false">
                    <Items>
                        <f:Button ID="Button1" Text="" CssClass="bgbtn1" runat="server" EnablePostBack="false" OnClientClick="window.goToIndex('/htmls/index.html')" />
                    </Items>
                </f:Panel>
                <f:Panel ID="Panel33" Title="面板2" Width="350px" Height="350px"
                    runat="server" BodyPadding="0px" ShowBorder="false" ShowHeader="false">
                    <Items>
                        <f:Button ID="Button2" Text="" CssClass="bgbtn2" runat="server" EnablePostBack="false" OnClientClick="window.goToIndex('/Pages/Index.aspx')" />
                    </Items>
                </f:Panel>
            </Items>
        </f:Panel>
    </form>
    <script>
        window.goToIndex = function (url) {
            window.location.href = url;
        };
    </script>
</body>
</html>

