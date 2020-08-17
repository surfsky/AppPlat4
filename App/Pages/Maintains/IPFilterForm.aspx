<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IPFilterForm.aspx.cs"  Inherits="App.Admins.IPFilterForm" %>

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
                <f:TextBox ID="tbIP" runat="server" Label="IP" Required="true" ShowRedStar="true" />
                <f:DateTimePicker runat="server" ID="dtStart" Label="封禁时间" />
                <f:DateTimePicker runat="server" ID="dtEnd" Label="解禁时间" />
                <f:FormRow runat="server">
                    <Items>
                        <f:TextBox ID="tbAddr" runat="server" Label="地理位置" />
                        <f:Button ID="btnSearch" runat="server" Icon="SystemSearch" Text="查找"  OnClick="btnSearch_Click" />
                    </Items>
                </f:FormRow>
                <f:TextBox ID="tbRemark" runat="server" Label="备注" />
            </Items>
        </f:Form>
    </form>
</body>
</html>
