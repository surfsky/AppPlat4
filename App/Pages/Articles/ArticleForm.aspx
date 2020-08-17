<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ArticleForm.aspx.cs" Inherits="App.Admins.ArticleForm" ValidateRequest="false" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="pageManager1" AutoSizePanelID="form2" runat="server" />
        <f:Form runat="server" ID="form2" ShowBorder="false" ShowHeader="false" AutoScroll="true" BodyPadding="10px">
            <Items>
                <f:TextBox runat="server" Label="标题" ID="tbTitle" Required="true" ShowRedStar="true" />
                <f:FormRow runat="server">
                    <Items>
                        <f:DropDownList runat="server" Label="类别" ID="ddlType" EmptyText="类别" Required="true" EnableEdit="false" ForceSelection="true" ShowRedStar="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged" AutoPostBack="true" />
                        <f:TextBox runat="server" Label="作者" ID="tbAuthor" />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:Label runat="server" Label="发布日期" ID="lblCreateDt" />
                        <f:Label runat="server" Label="回帖文章ID" ID="lblReplyId" />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:DropDownList runat="server" Label="状态" ID="ddlStatus" EmptyText="状态" ForceSelection="true" ShowRedStar="true" />
                        <f:DateTimePicker runat="server" Label="过期日期" ID="dtExpire" />
                    </Items>
                </f:FormRow>
                <f:Form runat="server" ID="panArticle" ShowBorder="false" BodyPadding="0" ShowHeader="false">
                    <Items>
                        <f:FormRow runat="server">
                            <Items>
                                <f:DropDownList runat="server" Label="目录" ID="ddlDir" EmptyText="目录" ForceSelection="true" />
                                <f:TextBox runat="server" Label="关键字" ID="tbKeywords" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow runat="server">
                            <Items>
                                <f:NumberBox runat="server" Label="权重" ID="tbWeight" Text="0" />
                                <f:NumberBox runat="server" Label="访问次数" ID="tbVisitCnt" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow runat="server">
                            <Items>
                                <f:DropDownList runat="server" Label="有效评论" ID="ddlIsValid" AutoPostBack="true" Hidden="true" />
                                <f:DropDownList runat="server" Label="应知应会" ID="ddlIsRequir" AutoPostBack="true" />
                            </Items>
                        </f:FormRow>
                        <f:HtmlEditor runat="server" Label="内容" ID="tbContent" Height="600px" Editor="UMEditor" BasePath="~/res/third-party/umeditor/" ToolbarSet="Full" />
                        <f:ImageUploader runat="server" ID="imgUploader" Label="封面图" UploadFolder="Articles" OnFileUploaded="ImgUploader_FileUploaded" ImageSize="200,200" />
                    </Items>
                </f:Form>
                <f:Panel runat="server" ID="panPage" ShowBorder="false" BodyPadding="0" ShowHeader="false">
                    <Items>
                        <f:DropDownList runat="server" Label="母版页" ID="ddlMother" EmptyText="母版页面" />
                        <f:TextBox runat="server" Label="母版插槽" ID="tbMotherSlot" EmptyText="" />
                        <f:TextBox runat="server" Label="路由路径" ID="tbRoutePath" EmptyText="" />
                        <f:NumberBox runat="server" Label="缓存秒数" ID="tbCacheSeconds" Text="0" />
                    </Items>
                </f:Panel>
                <f:Panel runat="server" ID="panRes" Title="附件" ShowHeader="true" Height="400" ShowBorder="false" BodyPadding="0" IFrameUrl="" Hidden="true" EnableIFrame="true" />
                <f:Panel runat="server" ID="panReply" Title="回帖" ShowHeader="true" Height="400" ShowBorder="false" BodyPadding="0" IFrameUrl="" Hidden="true" EnableIFrame="true" />
            </Items>
        </f:Form>
    </form>
</body>
</html>
