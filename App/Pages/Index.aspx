<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="App.Index" %>
<%@ OutputCache Location=None %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>首页</title>
    <script type="text/javascript"  src="/Res/js/common.js"></script>
    <style>
/* 用 Chrome 开发人员工具调整测试 */
/* 滚动条 */
/*定义滚动条高宽及背景 高宽分别对应横竖滚动条的尺寸*/
::-webkit-scrollbar {
    width: 10px;
    height: 10px;
}

/*定义滚动条轨道 内阴影+圆角*/
::-webkit-scrollbar-track {
    /*-webkit-box-shadow: inset 0 0 6px rgba(0,0,0,0.3);*/
    border-radius: 0px;
    background-color: transparent;
}

/*定义滑块 内阴影+圆角*/
::-webkit-scrollbar-thumb {
    border-radius: 10px; /*滚动条的圆角*/
    /*-webkit-box-shadow: inset 0 0 6px rgba(0,0,0,.3);*/
    background-color: rgba(255, 255, 255, 0.30); /*滚动条的背景颜色*/
}

/* 顶部工具栏 */
#logo img {
    width: 20px;
}
#Toolbar1 {
    background-color: #005999 !important;
    background-image: url("/res/images/barbg.jpg");
    background-repeat: no-repeat;
}
#Toolbar1 .f-toolbar-item{
    background: transparent;
    border: none !important;
    font-size: 12px;
    color: white;
}
#Toolbar1 .f-toolbar-item.f-state-hover{
    background: #1981cc !important;    /*#005a99*/
    color: white !important;
}
#lblVersion {
    top: 13px !important;
}


/* 中间分隔条 */
.f-region-split {
    background: #005a99;
}
.f-region-split .f-icon {
    visibility: hidden;
}
.f-region-split-icon.f-state-default {
    display: none;
}


/* 面板 */
.f-panel-header {
    background:#005a99 !important;
    height: 30px !important;
    padding-top: 0px !important;
}
.f-panel-title {
    color: white !important;
}
.f-panel-title-text {
    color: #047bce;
    padding-top: 5px !important;
}

/* 右侧TabStrip样式 */
.f-panel-header.f-state-hover {
    background: #024573!important;
    border-left-color:  #024573!important;
    border-right-color:  #024573!important;
}
.f-tabstrip-header-clip {
    height: 30px !important;
}
a.f-panel-header {
    padding-top: 2px !important;
}

/* 右侧主区域 */
#regionPanel_regionMain {
    border: 0px;
}

a.f-tab-header.f-state-active {
    background-color: white !important;
    color: #005a99;
}

/* 左侧面板标题 */
#regionPanel_regionLeft {
    border: none !important;
}
.f-widget-header.f-tool.f-tool-toggle {
    background-color: #005a99;
    height: 30px;
    border-bottom-color: #0071b0;
}
.f-widget-header.f-tool.f-tool-toggle.f-state-hover {
    background-color: #024573;
}
a.f-tool.f-tool-icon-only.f-cmp.f-tool-toggle.f-widget.f-state-hover {
    background-color: #024573!important;
}
.f-widget-header.f-region-cover {
    background-color: #005a99;
}
.f-region-cover-text {
    color: #0b81c8;
}

/* menu bar bg */
.f-panel-header.f-widget-header.f-noselect {
    border-bottom-color: #0064af;
}

/* menu bar toggle icon */
i.f-tool-icon.f-icon.f-iconfont.f-iconfont-toggle {
    color: #0b81c8;
}
i.f-icon.f-iconfont {
    color: #0b81c8;
}

/* menu tree bg */
.f-tree-bodyct {
    background-color: #005a99;
    color: white !important;
}

/* menu tree icon */
i.f-tree-cell-icon.f-tree-expander.f-icon.f-iconfont {
    color: white;
}

/* menu tree node */
a.f-tree-cell-text {
    color: white;
}
.f-tree-node.f-state-hover {
    background: #0072b3;
    color:white !important;
}
.f-tree-node.f-state-hover a.f-tree-cell-text{
    background: #0072b3;
    color:white !important;
}

/* menu tree node selected */
.f-tree-node.f-tree-node-selected {
    background: #0072b3;
    color: white!important;
}
.f-tree-node.f-tree-node-selected a.f-tree-cell-text {
    color: white;
}





    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="regionPanel" runat="server" />
        <f:RegionPanel ID="regionPanel" ShowBorder="false" runat="server">
            <Regions>

                <f:Region ID="regionTop" ShowBorder="false" ShowHeader="false" Position="Top"  Layout="Fit" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" Position="Bottom" runat="server" ClientIDMode="Static" CssClass="topbar">
                            <Items>
                                <f:ToolbarText runat="server" Text=" " />
                                <f:Image runat="server" ID="logo" ImageUrl="~/res/logowhite.png" Width="20px" Height="20px" />
                                <f:ToolbarText runat="server" ID="txtTitle" Text="SiteTitle"  CssStyle="font-size:20px; color:white;" />
                                <f:ToolbarText runat="server" ID="lblVersion" />
                                <f:ToolbarFill runat="server" />
                                <f:ToolbarText runat="server" ID="txtUser" Text="欢迎" />
                                <f:ToolbarText runat="server" ID="lblInfo" />
                                <f:Button ID="btnRefresh" runat="server" Icon="Reload" Text="刷新" ToolTip="刷新主区域内容" EnablePostBack="false"/>
				                <f:Button ID="btnCloseAll" runat="server" Icon="Cross" Text="全部关闭" ToolTip="关闭所有标签页面" EnablePostBack="false"  OnClientClick="window.removeAllTab();" Hidden="false"/>
                                <f:Button ID="btnFullScreen" runat="server" Icon="ArrowOut" Text="全屏" ToolTip="全屏（不支持IE)" EnablePostBack="false"/>
                                <f:Button ID="btnHelp" runat="server" Icon="Help" Text="帮助" EnablePostBack="false" />
                                <f:Button ID="btnExit" runat="server" Icon="DoorOut" Text="退出" OnClientClick="location.href='/logout.ashx'"/>
                                <f:ContentPanel  runat="server" Hidden="true">
                                    <audio id="audio" src="/res/neworder.mp3"  hidden="hidden" />
                                </f:ContentPanel>
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                </f:Region>


                <f:Region ID="regionLeft" Split="true" EnableCollapse="true" Width="200px"
                    ShowHeader="true" Title="系统菜单" Layout="Fit" Position="Left" runat="server" >
                    <Items>
                        <f:Tree runat="server" ShowBorder="false" ShowHeader="false" EnableArrows="true"  ID="treeMenu" />
                    </Items>
                </f:Region>

                <f:Region ID="regionMain" ShowHeader="false" Layout="Fit" Position="Center" runat="server">
                    <Items>
                        <f:TabStrip ID="mainTabStrip" EnableTabCloseMenu="true" ShowBorder="false" runat="server">
                            <Tabs>
                                <f:Tab ID="Tab1" Title="首页" EnableIFrame="true" IFrameUrl="Welcome.aspx"  Icon="House" runat="server" EnableClose="false" />
                            </Tabs>
                        </f:TabStrip>
                    </Items>
                </f:Region>
            </Regions>
        </f:RegionPanel>
    </form>

    <script>

    // F相关的方法请查源码 xxx.js 文件
    F.ready(function () {
        var treeMenu = F('<%= treeMenu.ClientID %>');
        var mainTabStrip = F('<%= mainTabStrip.ClientID %>');
        var btnRefresh = F('<%= btnRefresh.ClientID %>');
        var btnFullScreen = F('<%= btnFullScreen.ClientID %>');

        // 刷新当前标签
        btnRefresh.on('click', function () {
            refleshActiveTab();
        });

        // 全屏按钮
        btnFullScreen.on('click', function () {
            switchFullScreen();
            var root = window.location.origin;
            var btn = $("#btnFullScreen .f-btn-text");
            var img = $("#btnFullScreen img");
            var txt = btn.text();
            if (txt == "全屏")
            {
                img.attr("src", "/res/icon/arrow_in.png");
                btn.text("恢复");
            }
            else
            {
                img.attr("src", "/res/icon/arrow_out.png");
                btn.text("全屏");
            }
        });

        // 初始化主框架中的树和选项卡互动，及地址栏的更新逻辑
        F.initTreeTabStrip(treeMenu, mainTabStrip, null, true, false, false);

        //----------------------------------------------
        // 请查看 F.js 手册及 http://js.fineui.com/
        //----------------------------------------------
        // 刷新活动标签内容
        window.refleshActiveTab = function () {
            var activeTab = mainTabStrip.getActiveTab();
            if (activeTab.iframe) {
                var iframeWnd = activeTab.getIFrameWindow();
                iframeWnd.location.reload();
            }
        }

        // 刷新标签内容
        window.refleshTab = function (id) {
            var tab = getTab(id); // mainTabStrip.getTab(id);
            if (tab != 'undifined') {
                mainTabStrip.setActiveTab(tab);
                if (tab.iframe) {
                    var iframeWnd = tab.getIFrameWindow();
                    iframeWnd.location.reload();
                }
            }
        }

        // 遍历选项卡
        window.getTab = function (id) {
            $.each(mainTabStrip.items, function (index, item) {
                var itemId = item.id;
                if (itemId == id)
                    return item;
            });
            return null;
        }

        // 删除活动标签
        window.removeActiveTab = function () {
            var activeTab = mainTabStrip.getActiveTab();
            mainTabStrip.removeTab(activeTab.id);
        };

        // 删除所有标签
        window.removeAllTab = function () {
            mainTabStrip.hideAllTabs();
        };

        // 添加标签(id是标签的唯一性标志，若该标签已经打开，再次操作时会跳到已经打开的tab上)
        window.addMainTab = function (id, url, text, icon, refreshWhenExist) {
            F.addMainTab(mainTabStrip, id, url, text, icon, null, refreshWhenExist);
            mainTabStrip.setActiveTab(id);
        };

        // 添加标签页
        // id： 选项卡ID
        // iframeUrl: 选项卡IFrame地址 
        // title： 选项卡标题
        // icon： 选项卡图标
        // createToolbar： 创建选项卡前的回调函数（接受tabOptions参数）
        // refreshWhenExist： 添加选项卡时，如果选项卡已经存在，是否刷新内部IFrame
        // iconFont： 选项卡图标字体
        window.addExampleTab = function (tabOptions) {
            if (typeof (tabOptions) === 'string') {
                tabOptions = {
                    id: arguments[0],
                    iframeUrl: arguments[1],
                    title: arguments[2],
                    icon: arguments[3],
                    createToolbar: arguments[4],
                    refreshWhenExist: arguments[5],
                    iconFont: arguments[6]
                };
            }
            F.addMainTab(F(mainTabStripClientID), tabOptions);
            mainTabStrip.setActiveTab(id);
        }

        // 启动长连接
        //longConnect();
    });

    //-------------------------------------------
    // 长连接
    //-------------------------------------------
    function longConnect() {
        var lblInfo = $('#<%=lblInfo.ClientID%>');
        var audio = document.getElementById("audio");  // $('#audio')[0];
        $.ajax({
            url: "/Comets.ashx",
            timeout: 1000*60*5
        })
        .done(function (data) {
            console.info(data);
            if (data == "" || data.Type == "undefined" || data.Value == "undefined")
                return;
            var t = data.Type;
            var v = data.Value;

            // 在线类消息
            if (t == "Online")
                lblInfo.html("在线 " + v);

            // 订单类消息
            else if (t == "Order") {
                //lblInfo.html("新订单 ");
                audio.play();
                window.refleshTab("Orders");  // 刷新订单列表页面
                F.confirm({
                    message: '您有新订单要处理',
                    messageIcon: 'warning',
                    target: '_top',
                    buttons: [
                        { id: 'ok', text: '查看订单'},
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
            }

            // 新闻类消息
            else if (t == "News") {
                lblInfo.html("新消息");
                lblInfo.click(function () {
                    var tabId = "Article-" + v.ID;
                    var tabText = "消息";
                    var url = "/Pages/Articles/Article.aspx?id=" + v.ID;
                    window.addMainTab(tabId, url, tabText);
                });
            }
            else {
                lblInfo.html(t + " " + v);
            }
        })
        .always(function (data) {
            console.info(data.statusText);
            longConnect();
        });
    }

    </script>
</body>
</html>
