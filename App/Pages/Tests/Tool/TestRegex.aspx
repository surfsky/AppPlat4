<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestRegex.aspx.cs" Inherits="App.Tests.TestRegex" ValidateRequest="false" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" runat="server" />
        <f:Form ID="form2" IsFluid="true" BodyPadding="10px" runat="server" EnableCollapse="false"
            ShowBorder="true" Title="正则表达式" ShowHeader="true">
            <Items>
                <f:TextArea runat="server" ID="tbText" Label="文本" Height="200px"  />

                <f:FormRow runat="server">
                    <Items>
                        <f:TextBox runat="server" ID="tbRegex" Label="正则表达式"   />
                        <f:Button runat="server" ID="btnMatch"  Text="寻找匹配" Icon="SystemSearch" OnClick="btnMatch_Click"  />
                    </Items>
                </f:FormRow>
                <f:TextArea runat="server" ID="tbMatchResult" Label="匹配结果" Height="200px"  />

                <f:FormRow runat="server">
                    <Items>
                        <f:TextBox runat="server" ID="tbReplace" Label="替换表达式"  />
                        <f:Button runat="server" ID="btnReplace"  Text="替换" Icon="TextReplace" OnClick="btnReplace_Click" />
                    </Items>
                </f:FormRow>
                <f:TextArea runat="server" ID="tbReplaceResult" Label="替换结果" Height="200px"  />
            </Items>
        </f:Form>
    </form>
</body>
</html>