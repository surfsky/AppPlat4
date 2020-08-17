<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Welcome.aspx.cs" Inherits="App.Admins.Welcome" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <script type="text/javascript" src="/res/js/jquery.min.js" ></script>
    <script type="text/javascript" src="/res/js/echarts.min.js" ></script>
    <style type="text/css">
        .main td.x-table-layout-cell {
            padding: 5px;
        }

        .main td.f-layout-table-cell {
            padding: 5px;
        }

        li{
            /*list-style: url('/res/images/arrow.png')*/
        }
        li a{
            text-decoration: none;
            color: gray;
        }
        li a:visited {
            color: gray;
        }
        li a:hover {
            color: #157FCC;
        }
        .createDt {
            color: #D6D5D5;
            font-size: 10px;
            float: right;
        }
        .x-panel-header-default {
            background-image: none;
            background-color: #FBFBFB;
        }
        .x-panel-header-title-default {
            color: #4c9dd8;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server"  />
    <f:Panel ID="Panel1" runat="server" ShowBorder="True" ShowHeader="false" Layout="VBox" Title="自动平均分布" BodyPadding="5">
        <Items>
            <f:Panel ID="Panel5" BoxFlex="1" runat="server" ShowBorder="false" ShowHeader="false" Layout="HBox" BoxConfigChildMargin="0 5 0 0" Margin="0 0 5 0">
                <Items>
                    <f:Panel ID="pan1" BoxFlex="1" runat="server" ShowBorder="false" ShowHeader="true" Title="报表" BodyPadding="5px"  Layout="Card"  >
                        <Items>
                            <f:Label runat="server" ID="widget1" CssStyle="width: 300px;height:200px;" />
                        </Items>
                    </f:Panel>
                    <f:Panel ID="pan2" BoxFlex="1" runat="server" ShowBorder="false" ShowHeader="true" Title="报表" BodyPadding="5px"  Layout="Card"  >
                        <Items>
                            <f:Label runat="server" ID="widget2" CssStyle="width: 300px;height:200px;" />
                        </Items>
                    </f:Panel>
                </Items>
            </f:Panel>

            <f:Panel ID="Panel11" BoxFlex="1" runat="server" ShowBorder="false" ShowHeader="false" Layout="HBox" BoxConfigChildMargin="0 5 0 0">
                <Items>
                    <f:Panel ID="pan3" BoxFlex="1" runat="server" ShowBorder="false" ShowHeader="true" BodyPadding="5px" Title="报表" Layout="Card" >
                        <Items>
                            <f:Label runat="server" ID="widget3" CssStyle="width: 300px;height:200px;" />
                        </Items>
                    </f:Panel>
                    <f:Panel ID="pan4" BoxFlex="1" runat="server" ShowBorder="false" ShowHeader="true" BodyPadding="5px" Title="报表" Layout="Card" >
                        <Items>
                            <f:Label runat="server" ID="widget4" CssStyle="width: 300px;height:200px;" />
                        </Items>
                    </f:Panel>
                </Items>
            </f:Panel>
        </Items>
    </f:Panel>

    </form>
</body>
</html>