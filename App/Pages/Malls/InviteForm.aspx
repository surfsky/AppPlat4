<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InviteForm.aspx.cs" Inherits="App.Pages.InviteForm" %>

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
                <f:DropDownList runat="server" ID="ddlSource" Label="来源" />
                <f:DropDownList runat="server" ID="ddlStatus" Label="状态" />


                <f:PopupBox runat="server" ID="pbShop" Label="商店" WinTitle="商店"        UrlTemplate="shops.aspx" />
	            <f:PopupBox runat="server" ID="pbInviter" Label="邀请人" WinTitle="邀请人" UrlTemplate="users.aspx" />
	            <f:PopupBox runat="server" ID="pbInvitee" Label="受邀人" WinTitle="受邀人" UrlTemplate="users.aspx" />

                <f:TextBox runat="server" ID="tbInviteeMobile" Label="受邀者手机" />
                <f:DateTimePicker runat="server" ID="dpCreate" Label="邀请时间" />
                <f:DateTimePicker runat="server" ID="dpRegist" Label="受邀人注册时间" />
                <f:TextArea runat="server" ID="tbRemark" Label="备注" />
            </Items>
        </f:Form>
    </form>
</body>
</html>
