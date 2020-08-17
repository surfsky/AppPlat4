<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WechatWEB.aspx.cs" Inherits="App.Tests.TestWechatWEB" %>

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
                <f:TextBox runat="server" ID="tbCode" Label="公众号登录Code" EmptyText="...获取code，拷贝过来，且不要用掉" />
                <f:TextBox runat="server" ID="tbOpenId" Label="OpenID" />
                <f:TextBox runat="server" ID="tbUnionId" Label="UnionID" />
                <f:Image runat="server" ID="photo" Label="头像" />
                <f:Button ID="btnGetSession" runat="server"  Text="GetUser" Icon="PlayGreen"  OnClick="btnGetSession_Click"   />

                <f:Label runat="server" ShowLabel="false" ShowEmptyLabel="true" />
                <f:TextBox runat="server" ID="tbToken" Label="公众号AccessToken" />
                <f:Button ID="btnGetToken" runat="server"  Text="获取Token" Icon="PlayGreen" OnClick="btnGetToken_Click" />
                <f:Button ID="btnRefreshToken" runat="server"  Text="刷新Token" Icon="PlayGreen" OnClick="btnRefreshToken_Click"/>

                <f:Label runat="server" ShowLabel="false" ShowEmptyLabel="true" />
                <f:TextBox runat="server" ID="tbTemplateId" Label="消息模板ID" EmptyText="请去公众号>功能>模板消息中开通获取" />
                <f:TextBox runat="server" ID="tbPage" Label="page" Text="/pages/order/list" />
                <f:TextBox runat="server" ID="tbFirst" Label="first" Text="first" />
                <f:TextBox runat="server" ID="tbRemark" Label="remark" Text="remark" />
                <f:TextBox runat="server" ID="tbKeyword1" Label="keyword1" Text="keyword1" />
                <f:TextBox runat="server" ID="tbKeyword2" Label="keyword2" Text="keyword2" />
                <f:TextBox runat="server" ID="tbKeyword3" Label="keyword3" Text="keyword3" />
                <f:TextBox runat="server" ID="tbKeyword4" Label="keyword4" Text="keyword4" />
                <f:TextBox runat="server" ID="tbKeyword5" Label="keyword5" Text="keyword5" />
                <f:Button  runat="server" ID="btnSendMsg" Text="发送消息" Icon="PlayGreen" OnClick="btnSendMsg_Click" />

                <f:Label runat="server" ShowLabel="false" ShowEmptyLabel="true" />
                <f:TextBox runat="server" ID="tbPath" Label="路径" Text="/pages/index/index?inviteShopId=99" />
                <f:Image runat="server" ID="imgQrCode" />
                <f:Button ID="btnGetQrCode" runat="server"  Text="获取二维码" Icon="PlayGreen" OnClick="btnGetQrCode_Click"/>
            </Items>
        </f:Form>
    </form>
</body>
</html>