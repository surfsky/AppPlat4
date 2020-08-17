<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HistoryView.aspx.cs" Inherits="App.Pages.HistoryView" %>

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
                        <f:Label runat="server" ID="lblId" Label="ID" />
                        <f:FormRow runat="server">
                            <Items>
                                <f:TextBox runat="server" ID="lblStatus" Label="状态"  Enabled="false"/>
                                <f:TextBox runat="server" ID="lblCreateDt" Label="时间" Enabled="false"/>
                            </Items>
                        </f:FormRow>
                        <f:FormRow runat="server">
                            <Items>
                                <f:TextBox runat="server" ID="lblUserName" Label="经手人"  Enabled="false"/>
                                <f:TextBox runat="server" ID="lblUserMobile" Label="经手人手机" Enabled="false"/>
                            </Items>
                        </f:FormRow>
                    </Items>
                </f:Form>
                <f:Panel runat="server" ID="panDetail" ShowBorder="false" Height="300px"  EnableIFrame="true"   />
            </Items>
        </f:Panel>
    </form>
</body>
</html>
