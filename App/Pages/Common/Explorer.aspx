<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Explorer.aspx.cs" Inherits="App.Pages.Explorer" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
    <f:Panel ID="Panel1" runat="server" BodyPadding="0px"  ShowBorder="false" Layout="Fit" ShowHeader="false" >
        <Toolbars>
            <f:Toolbar ID="Toolbar1" runat="server">
                <Items>
                    <f:Label runat="server" ID="lblFolder" />
                    <f:ToolbarFill runat="server" />
                    <f:FileUpload runat="server" ID="uploader" Hidden="true"
                        ButtonText="上传图像" ButtonOnly="true" ButtonIcon="ImageAdd" AcceptFileTypes="image/*"
                        AutoPostBack="true" OnFileSelected="uploader_FileSelected"/>
                    <f:CheckBox runat="server" ID="chkUnsafe"  Text="所有不安全文件" />
                    <f:TextBox runat="server" ID="tbName" EmptyText="名称" />
                    <f:Button runat="server" ID="btnSearch" Icon="SystemSearch" Text="查找" OnClick="btnSearch_Click" Type="Submit" />
                </Items>
            </f:Toolbar>
        </Toolbars>
        <Items>
            <f:GridPro ID="Grid1" runat="server" WinWidth="1000" WinHeight="800"/>
        </Items>
    </f:Panel>
 
    </form>
</body>
</html>
