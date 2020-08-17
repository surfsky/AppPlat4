<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestPopupbox.aspx.cs" Inherits="App.Admins.TestPopupbox" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <script type="text/javascript"  src="/res/js/jquery-3.3.1.min.js"></script>
</head>
<body>

    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="form2" runat="server" />
        <f:Form ID="form2" ShowBorder="false" ShowHeader="false" runat="server" BodyPadding="10px"  AutoScroll="true">
            <Toolbars>
                <f:Toolbar runat="server" >
                    <Items>
                        <f:Button runat="server" Text="弹窗" EnablePostBack="false" OnClientClick="showConfirm()" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Label runat="server" ID="lblId" Label="ID" />
                <f:DropDownList runat="server" ID="ddlSource" Label="来源" />
                <f:DatePicker runat="server" ID="dpCreate" Label="邀请时间" />
                <f:PopupBox runat="server" ID="tbUser" Label="用户"  UrlTemplate="/pages/Base/Users.aspx"  WinWidth="1000" WinTitle="用户" />
                <f:PopupBox runat="server" ID="pbGPS" Label="位置"   UrlTemplate="/pages/common/MapTencent.aspx" WinWidth="800" WinHeight="600" WinTitle="位置" Trigger2IconUrl="~/Res/Icon/World.png"  />
                <f:PopupBox runat="server" ID="pbFiles" Label="文件"  WinWidth="1000" WinHeight="600"  Trigger2Icon="Search"/>
                <f:PopupBox runat="server" ID="pbPages" Label="网页"  WinWidth="1000" WinHeight="600"  Trigger2Icon="Search"/>
                <f:FileBox  runat="server" ID="pbImages" Label="图片" WinWidth="1000" WinHeight="600"  Root="/" Filter=".jpg .gif" ShowDownload="true" ShowInfo="false"/>
            </Items>
        </f:Form>
    </form>
    <script>
        function showConfirm() {
            F.confirm({
                message: '您有新订单要处理',
                messageIcon: 'warning',
                target: '_top',
                buttons: [
                    { id: 'ok', text: '查看订单' },
                    { id: 'cancel', text: '退出' }
                ],
                handler: function (event, buttonId) {
                    if (buttonId === 'ok') {
                        // 显示订单详情
                        var tabId = "Order-" + v.ID;
                        var tabText = "订单";
                        var url = "/Pages/OrderForm.aspx?id=" + v.ID;
                        window.addMainTab(tabId, url, tabText);
                    }
                }
            });
            /*
            // 弹窗显示
            Ext.MessageBox.show({
                msg: '您有新订单要处理。',
                title: '新订单',
                buttons: Ext.Msg.YESNO,
                buttonText:
                {
                    yes: '查看订单',
                    no: '退出'
                },
                fn: function (btnId) {
                    if (btnId === 'yes') {
                        var tabId = "Order-" + v.ID;
                        var tabText = "订单";
                        var url = "/Pages/OrderForm.aspx?id=" + v.ID;
                        window.addMainTab(tabId, url, tabText);
                    }
                }
            });
            */
        }


    </script>
</body>
</html>
