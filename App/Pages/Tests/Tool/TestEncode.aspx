<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestEncode.aspx.cs" Inherits="App.Tests.TestEncode" ValidateRequest="false" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" runat="server" />
        <f:Form ID="form2" IsFluid="true" BodyPadding="10px" runat="server" EnableCollapse="false"
            ShowBorder="true" Title="编码转换" ShowHeader="true">
            <Items>
                <f:TextArea runat="server" ID="tbText" Label="文本" Height="200px" ShowLabel="false" EmptyText="文本"  />

                <f:FormRow runat="server">
                    <Items>
                        <f:Button runat="server" ID="btnHtmlEncode"  Text="HtmlEncode" Icon="SystemSearch" OnClick="btnHtmlEncode_Click"/>
                        <f:Button runat="server" ID="btnHtmlDecode"  Text="HtmlDecode" Icon="SystemSearch"  OnClick="btnHtmlDecode_Click"  />
                        <f:Button runat="server" ID="btnUrlEncode"  Text="UrlEncode" Icon="SystemSearch"  OnClick="btnUrlEncode_Click" />
                        <f:Button runat="server" ID="btnUrlDecode"  Text="UrlDecode" Icon="SystemSearch"  OnClick="btnUrlDecode_Click" />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:Button runat="server" ID="btnRemoveBlank"  Text="RemoveBlank" Icon="SystemSearch"  OnClick="btnRemoveBlank_Click"/>
                        <f:Button runat="server" ID="btnRemoveBlankTranslator"  Text="RemoveBlankTranslator" Icon="SystemSearch"  OnClick="btnRemoveBlankTranslator_Click"  />
                        <f:Button runat="server" ID="btnSlim"  Text="Slim" Icon="SystemSearch" OnClick="btnSlim_Click" />
                        <f:Button runat="server" ID="btnSummary"  Text="Summary" Icon="SystemSearch"  OnClick="btnSummary_Click"  />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:Button runat="server" ID="btnRemoveTag"  Text="RemoveTag" Icon="SystemSearch" OnClick="btnRemoveTag_Click" />
                        <f:Button runat="server" ID="btnRemoveHtml"  Text="RemoveHtml" Icon="SystemSearch"   OnClick="btnRemoveHtml_Click" />
                        <f:Button runat="server" ID="btnAddQuote"  Text="AddQuote" Icon="SystemSearch"  OnClick="btnAddQuote_Click" />
                        <f:Button runat="server" ID="btnRemoveQuote"  Text="RemoveQuote" Icon="SystemSearch"  OnClick="btnRemoveQuote_Click" />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:Button runat="server" ID="btnJson"  Text="ToJson" Icon="SystemSearch"  OnClick="btnJson_Click" />
                        <f:Button runat="server" ID="btnXml"  Text="ToXml" Icon="SystemSearch"  OnClick="btnXml_Click" />
                    </Items>
                </f:FormRow>
                <f:TextArea runat="server" ID="tbResult" Label="结果" Height="200px" ShowLabel="false" EmptyText="结果" MarginTop="10px" />
            </Items>
        </f:Form>
    </form>
</body>
</html>