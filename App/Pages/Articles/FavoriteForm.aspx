<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FavoriteForm.aspx.cs" Inherits="App.Pages.FavoriteForm" %>

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
                <f:Label runat="server" ID="lblId" Label="ID" />
                <f:DropDownList runat="server" ID="ddlDir" Label="知识库目录" />
                <f:DropDownList runat="server" ID="ddlType" Label="来源" OnSelectedIndexChanged="ddlType_SelectedIndexChanged" AutoPostBack="true" />
                <f:PopupBox runat="server" ID="pbUser" Label="用户" WinTitle="选择用户" UrlTemplate="~/pages/Base/users.aspx" />
                <f:PopupBox runat="server" ID="pbDept" Label="部门" WinTitle="选择部门" UrlTemplate="~/pages/Base/depts.aspx" />
                <f:NumberBox runat="server" ID="tbSeq" Label="排序" />

                <f:Label runat="server" ID="lblCreateDt" Label="创建时间" />
                <f:TextBox runat="server" ID="tbRemark" Label="备注" />
            </Items>
        </f:Form>
    </form>
</body>
</html>
