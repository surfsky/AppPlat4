<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ArticlePush.aspx.cs" Inherits="App.Admins.ArticlePush" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>文章推送</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager2" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" ShowBorder="false" ShowHeader="false" AutoScroll="true" runat="server" BodyPadding="10px">
            <%--            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                    </Items>
                </f:Toolbar>
            </Toolbars>--%>
            <Items>
                <f:PopupBox runat="server" ID="pbDept" Label="部门" UrlTemplate="~/pages/Base/depts.aspx" Multiselect="false" EmptyText="部门" Width="300" />
                <f:DropDownList runat="server" ID="ddlTitle" Label="职称" EmptyText="职称" Width="300" />
                <f:DropDownList runat="server" ID="ddlRole" Label="角色" EmptyText="角色" Width="300" />
                <f:Button runat="server" ID="btnPush" Text="推送" OnClick="btnPush_Click" Type="Submit" Icon="Add" />
            </Items>
        </f:Panel>
    </form>
    <script>

        //F.ready(function () {

        //    if (!window) return;

        //    function resetWindowHeight() {

        //        // 清空左上角定位
        //        window.top = undefined;
        //        window.left = undefined;

        //        // 改变高度
        //        window.resizeTo(200,200);

        //    }

        //    // 页面加载完毕后，首先对窗体进行高度设置
        //    resetWindowHeight();

        //    // 页面大小改变事件
        //    F.windowResize(function () {

        //        resetWindowHeight();

        //    });

        //});
    </script>
</body>
</html>
