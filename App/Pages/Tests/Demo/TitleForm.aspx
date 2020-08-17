<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TitleForm.aspx.cs"  Inherits="App.Tests.TitleForm" %>

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
                <f:TextBox ID="tbName" runat="server" Label="名称" Required="true" ShowRedStar="true" />
                <f:TextArea ID="tbRemark" runat="server" Label="备注" />
            </Items>
        </f:Form>
    </form>
</body>
</html>
