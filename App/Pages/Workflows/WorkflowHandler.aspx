<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkflowHandler.aspx.cs" Inherits="App.Pages.WorkflowHandler" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" ShowBorder="false" ShowHeader="false"  AutoScroll="true" runat="server">
            <Items>
                <f:Form ID="form2" ShowBorder="false" ShowHeader="false" runat="server" BodyPadding="10px"  Title="SimpleForm">
                    <Items>
                        <f:FormRow runat="server">
                            <Items>
                                <f:Label runat="server" ID="lblId" Label="ID"  Enabled="false" />
                                <f:TextBox runat="server" ID="tbKey" Label="Key"  Hidden="true"/>
                            </Items>
                        </f:FormRow>
                        <f:FormRow runat="server">
                            <Items>
                                <f:DropDownList runat="server" ID="ddlStatus" Label="状态" />
                                <f:DateTimePicker runat="server" ID="dpCreateDt" Label="时间" Enabled="false"  />
                            </Items>
                        </f:FormRow>
                        <f:FormRow runat="server">
                            <Items>
                                <f:TextBox runat="server" ID="tbUserName" Label="经手人"  />
                                <f:TextBox runat="server" ID="tbUserMobile" Label="经手人手机" />
                            </Items>
                        </f:FormRow>
                        <f:PopupBox runat="server" ID="pbUser" Label="经手人" WinTitle="选择用户" Hidden="true" ShowSearcher="false" />
                    </Items>
                </f:Form>
                <f:Panel runat="server" ID="panDetail" ShowBorder="false" Height="300px"  EnableIFrame="true"   />
            </Items>
        </f:Panel>
    </form>
</body>
</html>
