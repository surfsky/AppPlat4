<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogForm.aspx.cs" Inherits="App.Admins.LogForm" ValidateRequest="false"  %>


<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="form2" runat="server" />
        <f:Form ID="form2" ShowBorder="false" ShowHeader="false" runat="server" BodyPadding="10px"  AutoScroll="true">
            <Toolbars>
                <f:Toolbar runat="server">
                    <Items>
                        <f:Button runat="server"  ID="btnSwitch" Text="切换内容展示方式" Icon="ArrowSwitch" OnClick="btnSwitch_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:FormRow runat="server">
                    <Items>
                        <f:Label runat="server" ID="lblId" Label="ID" />
                        <f:TextBox ID="tbLevel" runat="server" Label="级别" />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:TextBox ID="tbOperator" runat="server" Label="操作者"  />
                        <f:TextBox ID="tbLogDt" runat="server" Label="时间"  />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:TextBox ID="tbFrom" runat="server" Label="数据来源" />
                        <f:TextBox ID="tbMethod" runat="server" Label="请求方式" />
                    </Items>
                </f:FormRow>
                <f:TextBox ID="tbSummary" runat="server" Label="概述" />
                <f:TextBox ID="tbURL" runat="server" Label="请求 URL" />
                <f:TextBox ID="tbReferrer" runat="server" Label="前网页URL" />
                <f:FormRow runat="server">
                    <Items>
                        <f:TextBox ID="tbIP" runat="server" Label="来源 IP" />
                        <f:Button runat="server" ID="btnBan" Text="加入黑名单" Icon="Delete" OnClick="btnBan_Click" />
                    </Items>
                </f:FormRow>
                <f:TextArea runat="server" Label="内容" ID="tbMsg" Height="400px"  />
                <f:HtmlEditor runat="server" Label="内容(HTML)" ID="tbMsgHtml" Height="400px" Editor="UMEditor" BasePath="~/res/third-party/umeditor/" ToolbarSet="Full"  Hidden="true" />
            </Items>
        </f:Form>
        <f:Window ID="win" Width="700px" Height="500px" Icon="TagBlue" Title="窗体一" Hidden="true" EnableIFrame="true"
            EnableMaximize="true" EnableCollapse="false" runat="server" EnableResize="true"  CloseAction="Hide"
            IsModal="true" AutoScroll="true" BodyPadding="10px"  />
    </form>
</body>
</html>
