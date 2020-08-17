<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestECharts.aspx.cs" Inherits="App.Controls.ECharts.TestECharts" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <!-- 引入 echarts.js -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/echarts/4.2.1/echarts.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    </form>

    <!-- ECharts -->
    <div id="div1" style="width: 40%;height:400px; display:inline-block;"></div>
    <div id="div2" style="width: 40%;height:400px; display:inline-block;"></div>
    <script type="text/javascript">
        $(function () {
            $.ajax({
                type: "post",
                url: "/HttpApi/App.Controls.EChart.TestECharts/GetLineData",
                data: {},
                dataType: "json",
                success: function (option) {
                    var myChart = echarts.init(document.getElementById('div1'));
                    myChart.setOption(option);
                }
            });
            $.ajax({
                type: "post",
                url: "/HttpApi/App.Controls.EChart.TestECharts/GetPieData",
                data: {},
                dataType: "json",
                success: function (option) {
                    var myChart = echarts.init(document.getElementById('div2'));
                    myChart.setOption(option);
                }
            });

        })

    </script>
</body>
</html>
