<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserDeptForm.aspx.cs" Inherits="App.Pages.UserDeptForm" %>

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
                <f:PopupBox runat="server" ID="pbUser" Label="用户" WinTitle="选择用户" WinWidth="1000" UrlTemplate="users.aspx" />
                <f:PopupBox runat="server" ID="pbDept" Label="部门" WinTitle="选择部门" WinWidth="1000" UrlTemplate="depts.aspx" />
                <f:TextBox runat="server" ID="tbTitle" Label="头衔" />
                <f:NumberBox runat="server" ID="tbSeq" Label="排序"  />
            </Items>
        </f:Form>
    </form>
</body>
</html>
