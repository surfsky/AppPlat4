<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Article.aspx.cs"  Inherits="App.Admins.Article" ValidateRequest="false" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
</head>
<body>
    <form id="form1" runat="server">
        <h1><asp:Literal runat="server" ID="lblTitle" /></h1>
        <p>
            <small><asp:Literal runat="server" ID="lblAuthor" /></small>
            <small><asp:Literal runat="server" ID="lblPostDt" /></small>
            <img src="/res/images/view.png" />
            <small><asp:Literal runat="server" ID="lblVisitCnt" /></small>
            <img src="/res/images/approval.png" />
            <small><asp:Literal runat="server" ID="lblApproval" /></small>
        </p>
        <div>
            <asp:Literal runat="server" ID="lblContent" />
        </div>

        <h3><img src="/Res/images/block.png" width="3" />&nbsp;附件</h3>
        <asp:Repeater runat="server" ID="rptFiles" >
            <ItemTemplate>
                <img src="/Res/images/clip.png" width="14" />&nbsp;
                <a href="<%#Eval("Url") %>" target="_blank"><%#Eval("FileName") %></a>
                <br />
            </ItemTemplate>
        </asp:Repeater>

        <h3><img src="/Res/images/block.png" width="3" />&nbsp;回帖</h3>
        <asp:Repeater runat="server" ID="rptReply" >
            <ItemTemplate>
                <small> <%#Eval("AuthorName") %> </small>
                <div>
                    <img src="/Res/images/reply.png" width="14" />&nbsp;
                    <%#Eval("Title") %> 
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </form>
</body>
</html>
