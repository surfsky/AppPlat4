<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResForm.aspx.cs"  Inherits="App.Admins.ResForm" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="form2" runat="server" />
        <f:Form ID="form2" ShowBorder="false" ShowHeader="false" runat="server" BodyPadding="10px"  AutoScroll="true">
            <Items>
                <f:TextBox runat="server" ID="tbKey" Label="ID" Readonly="true" />
                <f:TextBox runat="server" ID="tbName" Label="名称" Required="true" ShowRedStar="true" />
                <f:RadioButtonList runat="server" ID="rblType" Label="类型" Hidden="true" Readonly="true"  />
                <f:TextBox runat="server" ID="tbFile" Label="文件" Readonly="true" />
                <f:FileUpload runat="server" ID="uploader"  ShowEmptyLabel="true"
                    ButtonText="上传" ButtonOnly="true" ButtonIcon="ImageAdd" 
                    AcceptFileTypes="image/*"
                    AutoPostBack="true" OnFileSelected="uploader_FileSelected"/>
                <f:Thrumbnail runat="server" ID="img" ImageUrl="~/res/images/blank.png"  ShowEmptyLabel="true" />
                <f:CheckBox runat="server" ID="chkProtect" Label="保护" />
            </Items>
        </f:Form>
    </form>
</body>
</html>
