<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AreaForm.aspx.cs" Inherits="App.Admins.AreaForm" %>

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
                <f:DropDownList ID="ddlType" Label="类别" runat="server" />
                <f:TextBox ID="tbFullName" runat="server" Label="全称" Enabled="false" />
                <f:NumberBox ID="tbSeq" Label="排序" Required="true" ShowRedStar="true" runat="server" />
                <f:DropDownList ID="ddlParent" Label="上级" ShowRedStar="true" runat="server" />
                <f:TextArea ID="tbRemark" runat="server" Label="备注" />
            </Items>
        </f:Form>
    </form>
</body>
</html>
