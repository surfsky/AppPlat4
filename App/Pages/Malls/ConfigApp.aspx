<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfigApp.aspx.cs" Inherits="App.Pages.ConfigApp" %>

<!DOCTYPE html>
<html>
<head runat="server">
</head>
<body>
    <form id="form1" runat="server">
    <f:PageManager ID="PageManager1" AutoSizePanelID="form2" runat="server" AjaxAspnetControls="repeater" />
    <f:Form ID="form2" runat="server" ShowBorder="false" ShowHeader="false" BodyPadding="10" AutoScroll="true">
        <Toolbars>
            <f:Toolbar runat="server">
                <Items>
                    <f:Button runat="server" ID="btnSave" Text="保存" Icon="SystemSave" OnClick="btnSave_Click" />
                    <f:ToolbarFill runat="server" />
                    <f:Label runat="server" ID="lblInfo" CssStyle="color:red!important"/>
                </Items>
            </f:Toolbar>
        </Toolbars>
        <Items>
            <f:FileUpload runat="server" ID="uploaderBg" ShowRedStar="false" Label="登录背景"
                ButtonText="上传图像" ButtonOnly="true" Required="false" ButtonIcon="ImageAdd" AcceptFileTypes="image/*"
                AutoPostBack="true" OnFileSelected="uploaderBg_FileSelected"/>
            <f:Image ID="imgBg" runat="server" ImageHeight="160" ShowEmptyLabel="true" />
        </Items>
    </f:Form>
    </form>
</body>
</html>
