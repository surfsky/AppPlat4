<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WechatMP.aspx.cs" Inherits="App.Tests.TestWechatMP" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>测试微信相关功能</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" runat="server" />
        <f:Form ID="form2" IsFluid="true" BodyPadding="10px" runat="server" EnableCollapse="false"
            ShowBorder="true"  ShowHeader="false" AutoScroll="true">
            <Toolbars>
                <f:Toolbar runat="server">
                    <Items>
                        <f:ToolbarText runat="server"  Text="ErrorCode:"/>
                        <f:ToolbarText runat="server" ID="lblErrCode" />
                        <f:ToolbarText runat="server"  Text="ErrorInfo:"/>
                        <f:ToolbarText runat="server" ID="lblErrInfo"/>
                        <f:ToolbarFill runat="server" />
                        <f:ToolbarText runat="server" ID="lblInfo" CssStyle="color:red" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:TextBox runat="server" ID="tbCode" Label="小程序登录Code" EmptyText="微信开发者工具wx.login()会获取code，拷贝过来，且不要用掉" />
                <f:TextBox runat="server" ID="tbOpenId" Label="用户小程序ID" />
                <f:TextBox runat="server" ID="tbUnionId" Label="用户UnionID" />
                <f:TextBox runat="server" ID="tbSession" Label="获取的SessionKey" />
                <f:Image runat="server" ID="photo" Label="头像" />
                <f:Button ID="btnGetSession" runat="server"  Text="JsCode2Session" Icon="PlayGreen"  OnClick="btnGetSession_Click"   />

                <f:Label runat="server" ShowLabel="false" ShowEmptyLabel="true" />
                <f:TextBox runat="server" ID="tbToken" Label="小程序AccessToken" />
                <f:Button ID="btnGetToken" runat="server"  Text="获取Token" Icon="PlayGreen" OnClick="btnGetToken_Click" />
                <f:Button ID="btnRefreshToken" runat="server"  Text="刷新Token" Icon="PlayGreen" OnClick="btnRefreshToken_Click"/>

                <f:Label runat="server" ShowLabel="false" ShowEmptyLabel="true" />
                <f:TextArea runat="server" ID="tbUserInfo" Label="wx.getUserInfo() reply" />
                <f:Button ID="btnGetUserInfo" runat="server"  Text="获取用户详细信息" Icon="PlayGreen" OnClick="btnGetUserInfo_Click"/>

                <f:Label runat="server" ShowLabel="false" ShowEmptyLabel="true" />
                <f:TextBox runat="server" ID="tbTemplateId" Label="消息模板ID" />
                <f:TextBox runat="server" ID="tbFormId" Label="formId" />
                <f:TextBox runat="server" ID="tbPage" Label="page" Text="/pages/order/list" />
                <f:TextBox runat="server" ID="tbKeyword1" Label="keyword1" Text="keyword1" />
                <f:TextBox runat="server" ID="tbKeyword2" Label="keyword2" Text="keyword2" />
                <f:TextBox runat="server" ID="tbKeyword3" Label="keyword3" Text="keyword3" />
                <f:TextBox runat="server" ID="tbKeyword4" Label="keyword4" Text="keyword4" />
                <f:TextBox runat="server" ID="tbKeyword5" Label="keyword5" Text="keyword5" />
                <f:Button  runat="server" ID="btnSendMsg" Text="发送消息" Icon="PlayGreen" OnClick="btnSendMsg_Click" />


                <f:Label runat="server" ShowLabel="false" ShowEmptyLabel="true" />
                <f:PopupBox runat="server" ID="pbUser" Label="用户" WinTitle="用户"  UrlTemplate="~/pages/base/users.aspx" />
                <f:PopupBox runat="server" ID="pbOrder" Label="订单" WinTitle="订单" UrlTemplate="~/pages/malls/orders.aspx" />
                <f:Button  runat="server" ID="btnSendMsg2" Text="发送联合消息" Icon="PlayGreen" OnClick="btnSendMsg2_Click"/>


                <f:Label runat="server" ShowLabel="false" ShowEmptyLabel="true" />
                <f:TextBox runat="server" ID="tbPath" Label="路径" Text="/pages/index/index?inviteShopId=99" />
                <f:NumberBox runat="server" ID="tbSize" Label="宽度" DecimalPrecision="0" Text="280" />
                <f:Image runat="server" ID="imgQrCode" />
                <f:Button ID="btnGetQrCode" runat="server"  Text="获取二维码" Icon="PlayGreen" OnClick="btnGetQrCode_Click"/>
            </Items>
        </f:Form>
    </form>
</body>
</html>