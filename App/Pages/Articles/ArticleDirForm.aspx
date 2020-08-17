<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ArticleDirForm.aspx.cs" Inherits="App.Admins.ArticleDirForm" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="form2" runat="server" />
        <f:Form ID="form2" ShowBorder="false" ShowHeader="false" runat="server" BodyPadding="10px" AutoScroll="true">
            <Items>
                <f:Label runat="server" ID="lblId" Label="ID" />
                <f:TextBox ID="tbName" runat="server" Label="名称" Required="true" ShowRedStar="true" />
                <f:NumberBox ID="tbSeq" Label="排序" Required="true" ShowRedStar="true" runat="server" />
                <f:DropDownList ID="ddlParent" Label="上级" ShowRedStar="true" runat="server" />
                <f:TextArea ID="tbRemark" runat="server" Label="备注" />
                <f:Image ID="img" CssClass="photo" ImageUrl="~/res/images/defaultDir.png" Label="图标" runat="server"/>
                <f:FileUpload runat="server" ID="uploader" ShowRedStar="false" ShowEmptyLabel="true"
                    ButtonText="上传图像" ButtonOnly="true" Required="false" ButtonIcon="ImageAdd" AcceptFileTypes="image/*"
                    AutoPostBack="true" OnFileSelected="uploader_FileSelected"/>
            </Items>
        </f:Form>
    </form>
</body>
</html>
