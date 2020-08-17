<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestDateTime.aspx.cs" Inherits="App.Tests.TestDateTime" %>

<!-- 
http://pro.fineui.com/#/third-party/my97/my97.aspx
-->
<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <script src="/res/third-party/my97/WdatePicker.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" runat="server" />
        <f:Form ID="form2" IsFluid="true" BodyPadding="10px" runat="server" EnableCollapse="false"
            ShowBorder="true" Title="表单" ShowHeader="true">
            <Items>
                <f:DatePicker runat="server" Required="true" Label="日期一" EmptyText="请选择日期一" ID="dp" ShowRedStar="true" />
                <f:TriggerBox ID="tbBox" Required="true" ShowRedStar="true" Label="日期和时间" EmptyText="请选择日期和时间" TriggerIcon="Date" runat="server" />
                <f:Button ID="btnSubmit" runat="server" ValidateForms="SimpleForm1" Text="提交表单" OnClick="btnSubmit_Click" />
            </Items>
        </f:Form>
        <f:Label ID="labResult" ShowLabel="false" EncodeText="false" runat="server" />
    </form>

    <script type="text/javascript">
        var pickerId = '<%= tbBox.ClientID %>';
        F.ready(function () {
            var picker = F(pickerId);
            picker.onTriggerClick = function () {
                WdatePicker({
                    el: pickerId + '-inputEl',
                    dateFmt: 'yyyy-MM-dd HH:mm:ss',
                    onpicked: function () {
                        picker.validate();
                    }
                });
            };
        });
    </script>
</body>
</html>