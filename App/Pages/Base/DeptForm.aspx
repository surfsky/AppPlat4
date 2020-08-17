<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeptForm.aspx.cs" Inherits="App.Admins.DeptForm" %>

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
                <f:DropDownList ID="ddlParent" Label="上级部门" ShowRedStar="true" runat="server" />
                <f:NumberBox ID="tbSeq" Label="排序" Required="true" ShowRedStar="true" runat="server" />
                <f:TextArea ID="tbRemark" runat="server" Label="备注" />
                <f:NumberBox ID="tbDingDeptId" runat="server" Label="钉钉部门ID"  />
            </Items>
        </f:Form>
    </form>
</body>
</html>
