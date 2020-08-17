<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Enterprise.aspx.cs" Inherits="App.Dings.Enterprise" %>

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
                        <f:Button ID="btnGetTokon" runat="server"  Text="获取Token" Icon="PlayGreen" OnClick="btnGetTokon_Click"/>
                        <f:Button ID="btnGetDepts" runat="server"  Text="获取部门列表" Icon="PlayGreen" OnClick="btnGetDepts_Click"/>
                        <f:Button ID="btnSetDeptsFromDing" runat="server"  Text="同步钉钉部门" Icon="PlayGreen" OnClick="btnSetDeptsFromDing_Click" ConfirmText="确定同步部门吗？老的部门及相关的数据都将清空。"/>
                    </Items>
                </f:Toolbar>
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
                <f:FormRow runat="server">
                    <Items>
                        <f:TextBox runat="server" ID="tbCode" Label="小程序登录Code" EmptyText="开发者工具dd.login()会获取code，拷贝过来，且不要用掉" />
                        <f:Button ID="btnGetUserInfo" runat="server"  Text="获取用户详细信息" Icon="PlayGreen" OnClick="btnGetUserInfo_Click"/>
                    </Items>
                </f:FormRow>
            </Items>
        </f:Form>
    </form>
</body>
</html>