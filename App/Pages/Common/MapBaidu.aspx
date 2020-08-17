<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MapBaidu.aspx.cs"  Inherits="App.Pages.MapBaidu" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>位置（百度地图）</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Form2" runat="server" />
        <f:Form ID="Form2" ShowBorder="false" ShowHeader="false"  AutoScroll="true" runat="server" BodyPadding="10" Layout="VBox">
            <Toolbars>
                <f:Toolbar runat="server" >
                    <Items>
                        <f:Button runat="server" Text="选择并关闭" ID="btnSave" Icon="SystemSaveClose" OnClick="btnSave_Click"/>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Rows>
                <f:FormRow runat="server" >
                    <Items>
                        <f:TextBox ID="tbGPS" runat="server" Label="经纬度"/>
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server" >
                    <Items>
                        <f:TextBox ID="tbAddr" runat="server" Label="详细地址"  />
                        <f:Button runat="server" Text="查找" OnClientClick="searchAddr()" EnablePostBack="false" Icon="SystemSearch" />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server" >
                    <Items>
                        <f:ContentPanel ID="panMap" runat="server" Height="400px" ShowHeader="false" >
                            <div id="mapContainer" style="width:100%; height:400px;" />
                        </f:ContentPanel>
                    </Items>
                </f:FormRow>
            </Rows>
        </f:Form>
    </form>
    </body>

    <!-- baidu 地图的这个脚本经常访问失败，不知道是不是有频率限制  -->
    <script type="text/javascript" src="https://api.map.baidu.com/api?v=2.0&ak=A60cece53af65e45e359f578c9e70d32"></script>
    <script type="text/javascript" src="/Res/js/bmap.js"></script>
    <script type="text/javascript">
        // 显示位置信息
        var map, marker;
        var tbGPS, tbAddr;
        function showGPS(point) {
            var gps = point.lng + "," + point.lat;
            tbGPS.value = gps;
        }
        function showAddr(ac) {
            var addr = ac.province + ac.city + ac.district + ac.street + ac.streetNumber;
            tbAddr.value = addr;
        }
        function searchAddr() {
            var addr = tbAddr.value;
            var city = "温州";
            new BMap.Geocoder().getPoint(addr, function (point) {
                if (point != null) {
                    showGPS(point);
                    map.centerAndZoom(point, 15);
                    marker.setPosition(point);
                }
            }, city);
        }

        F.ready(function () {
            // 解析点坐标
            tbGPS = document.getElementById("<%=tbGPS.ClientID%>-inputEl");
            tbAddr = document.getElementById("<%=tbAddr.ClientID%>-inputEl");
            var point = new BMap.Point(116.404, 39.915);
            var gps = tbGPS.value;
            var n = gps.indexOf(",");
            if (n > 0) {
                var lng = gps.substring(0, n);
                var lat = gps.substring(n+1);
                point = new BMap.Point(lng, lat);
            }

            // 百度地图
            map = new BMap.Map("mapContainer");
            map.centerAndZoom(point, 15);
            map.enableScrollWheelZoom();
            map.enableDoubleClickZoom();

            // 添加放缩按钮
            addControlGeolocation(map, function (gps, addr) {
                showGPS(gps);
                showAddr(addr);
            });

            // 地图变更后重新获取GPS位置
            marker = addMarkerSymbol(map, point);
            marker.setPosition(point);
            map.addEventListener('dragging', function () {
                var center = map.getCenter();
                marker.setPosition(center);
                showGPS(center);
            });
            map.addEventListener('dragend', function () {
                // 逆地址解析(point -> address 文本)
                var point = map.getCenter();
                new BMap.Geocoder().getLocation(point, function (result) {
                    showAddr(result.addressComponents);
                });
            });
        });
    </script>
</html>
