<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UIForm.aspx.cs"  Inherits="App.Pages.UIForm" ValidateRequest="false" %>

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
                        <f:HyperLink runat="server" ID="lnkUrl" Text="配置参考" Target="_blank" />
                        <f:Button runat="server" ID="btnGenerate" Text="生成配置" Icon="Build" OnClick="btnGenerate_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:TextBox runat="server" ID="tbName" Label="名称" />
                <f:DropDownList runat="server" ID="ddlType" Label="类型" Required="true" ShowRedStar="true" />
                <f:DropDownList runat="server" ID="ddlEntityType" Label="实体类" Required="true" ShowRedStar="true"  OnSelectedIndexChanged="ddlEntityType_SelectedIndexChanged" AutoPostBack="true" />
                <f:TextArea runat="server" ID="tbError" Label="错误信息" Height="80" AutoGrowHeight="true" Readonly="true" />
                <f:TextArea runat="server" ID="tbSetting" Label="设置" Height="500" AutoGrowHeight="true" />
            </Items>
        </f:Form>
    </form>
</body>
</html>
