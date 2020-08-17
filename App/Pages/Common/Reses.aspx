<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Reses.aspx.cs" Inherits="App.Admins.Reses" %>

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
                    <f:FileUpload runat="server" ID="uploader" 
                        ButtonText="上传" ButtonOnly="true" ButtonIcon="PageAdd" 
                        AcceptFileTypes="*/*"
                        AutoPostBack="true" OnFileSelected="uploader_FileSelected"/>
                    <f:Button runat="server" ID="btnBigUp" Text="大文件上传" Icon="PageAdd" />
                    <f:Button runat="server" ID="btnSelect" OnClick="btnSelect_Click" Text="选择并关闭" Icon="ShapesManySelect" />
                </Items>
            </f:Toolbar>
        </Toolbars>
        <Items>
            <f:GridPro ID="Grid1" runat="server" WinHeight="500" OnDelete="Grid1_Delete" AllowNew="false"/>
        </Items>
    </f:Panel>
 
    </form>
</body>
</html>
