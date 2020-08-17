<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MapTencent.aspx.cs"  Inherits="App.Pages.MapTencent" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>位置（腾讯地图）</title>
    <style>
        #mapContainer {
            min-width: 700px;
            min-height: 300px;
        }
    </style>
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
                        <f:TextBox ID="tbAddr" runat="server" Label="详细地址" Width="400"  />
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

    <!-- 腾讯地图  -->
    <script charset="utf-8" src="https://map.qq.com/api/js?v=2.exp&key=BZABZ-W2EHU-WELVJ-4ODPE-4KW5V-VRBQW&libraries=convertor"></script>
    <script type="text/javascript" src="https://3gimg.qq.com/lightmap/components/geolocation/geolocation.min.js"></script>
    <script>
        var map, marker = null;
        var tbGPS, tbAddr;

        F.ready(function () {
            tbGPS = document.getElementById("<%=tbGPS.ClientID%>-inputEl");
            tbAddr = document.getElementById("<%=tbAddr.ClientID%>-inputEl");

            // 位置
            var center = new qq.maps.LatLng(28.01469, 120.65523);
            var gps = tbGPS.value;
            var n = gps.indexOf(",");
            if (n > 0)
                center = toLatlng(gps);

            // 地图
            map = new qq.maps.Map(document.getElementById('mapContainer'), {
                center: center,
                zoom: 13
            });

            // 中心点图标
            var middleControl = document.createElement("div");
            middleControl.innerHTML = "<img src='/res/images/center.gif' />";
            middleControl.index = 1;
            map.controls[qq.maps.ControlPosition.CENTER].push(middleControl);

            // 右下角图标
            var locateControl = document.createElement("div");
            locateControl.innerHTML = "<img src='/res/images/locator.png' />";
            locateControl.index = 1;
            locateControl.style.paddingRight = "10px";
            locateControl.style.paddingBottom = "10px";
            locateControl.onclick = setCurrentLocation2;
            map.controls[qq.maps.ControlPosition.BOTTOM_RIGHT].push(locateControl);

            // 地图中心更改事件(marker 始终居中, 显示GPS及文本地址)
            qq.maps.event.addListener(map, 'center_changed', function () {
                var point = map.getCenter();
                tbGPS.value = toGPS(point);
                searchGPS();
            });
        });

        
        // 转化为经度纬度
        function toGPS(latlng) {
            var lat = latlng.lat.toString();
            var lng = latlng.lng.toString();
            return lng + ", " + lat;
        }

        // 转化为纬度经度
        function toLatlng(gps) {
            var array = gps.split(",", 2);
            var lng = parseFloat(array[0]);
            var lat = parseFloat(array[1]);
            return new qq.maps.LatLng(lat, lng);
        }


        // 设置当前城市中心点（根据IP)
        function setCurrentCity() {
            var citylocation = new qq.maps.CityService({
                complete: function (result) {
                    map.setCenter(result.detail.latLng);
                }
            });
            citylocation.searchLocalCity();
        }


        // 设置当前坐标（浏览器定位）
        function setCurrentLocation() {
            if (navigator.geolocation) {
                // safari 要求 https
                // chrome 必须翻墙才能进入回调函数
                navigator.geolocation.getCurrentPosition(function (position) {
                    var lat = position.coords.latitude;
                    var lng = position.coords.longitude;
                    var latlng = new qq.maps.LatLng(lat, lng);
                    map.setCenter(latlng);
                    tbGPS.value = toGPS(latlng);

                    // 调用转换接口。type的可选值为 1:GPS经纬度，2:搜狗经纬度，3:百度经纬度，4:mapbar经纬度，5:google经纬度，6:搜狗墨卡托
                    qq.maps.convertor.translate(latlng, 1, function (res) {
                        var point = res[0];
                        map.setCenter(point);
                        tbGPS.value = toGPS(point);
                    });
                });
            }
        }

        // 设置当前坐标（用腾讯定位服务）
        // https://lbs.qq.com/tool/component-geolocation.html
        function setCurrentLocation2() {
            var geolocation = new qq.maps.Geolocation("BZABZ-W2EHU-WELVJ-4ODPE-4KW5V-VRBQW", "bearmanager");
            geolocation.getLocation(
                function (position) {
                    var latlng = new qq.maps.LatLng(position.lat, position.lng);
                    map.setCenter(latlng);
                },
                function () {
                    console.info("定位失败");
                },
                { timeout: 8000 }
            );
        }

        // 查找指定的GPS坐标（并返回文本地址）
        function searchGPS() {
            // 获取经纬度数值
            var gps = tbGPS.value;
            var latLng = toLatlng(gps);

            // 获取位置文本（调用 Geocoder 需要企业账户并购买配额）
            var geocoder = new qq.maps.Geocoder({
                complete: function (result) {
                    tbAddr.value = result.detail.address;
                }
            });
            geocoder.getAddress(latLng);
        }

        // 查找指定的文本地址（并返回GPS）
        function searchAddr() {
            var addr = tbAddr.value;
            var geocoder = new qq.maps.Geocoder({
                complete: function (result) {
                    map.setCenter(result.detail.location);
                }
            });
            geocoder.getLocation(addr);
        }

        //实时路况图层
        function showTraffic() {
            var layer = new qq.maps.TrafficLayer();
            layer.setMap(map);
        }
    </script>
</html>
