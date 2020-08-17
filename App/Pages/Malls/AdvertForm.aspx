<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdvertForm.aspx.cs"  Inherits="App.Pages.AdvertForm" ValidateRequest="false" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Form2" runat="server" />
        <f:Form ID="Form2" ShowBorder="false" ShowHeader="false" runat="server" BodyPadding="10px"  AutoScroll="true">
            <Toolbars>
                <f:Toolbar runat="server" ID="toolbar" />
            </Toolbars>
            <Items>
                <f:Thrumbnail ID="img" ImageUrl="~/res/images/blank.png"  runat="server" Label="图片"/>
                <f:FileUpload runat="server" ID="uploader" ShowRedStar="false" ShowEmptyLabel="true"
                    ButtonText="上传图像" ButtonOnly="true" Required="false" ButtonIcon="ImageAdd" AcceptFileTypes="image/*"
                    AutoPostBack="true" OnFileSelected="uploader_FileSelected"/>
                <f:FormRow runat="server">
                    <Items>
                        <f:DropDownList ID="ddlLocation" runat="server" Label="位置" Required="true" ShowRedStar="true" />
                        <f:DropDownList ID="ddlStatus" runat="server" Label="状态" />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:NumberBox ID="tbSeq" runat="server" Label="顺序" />
                        <f:DateTimePicker ID="dpCreate" runat="server" Label="创建时间" Readonly="true" />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:DateTimePicker ID="dpStart" runat="server" Label="开始时间" />
                        <f:DateTimePicker ID="dpEnd" runat="server" Label="结束时间" />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:PopupBox runat="server" ID="pbProduct" Label="关联商品" WinTitle="商品" UrlTemplate="products.aspx" />
                        <f:PopupBox runat="server" ID="pbArticle" Label="关联文章" WinTitle="商品" UrlTemplate="/pages/articles/articles.aspx" />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:TextBox ID="tbTitle" runat="server" Label="标题" Required="true" ShowRedStar="true" />
                        <f:PopupBox runat="server" ID="pbShop" Label="归属商店" WinTitle="商店" UrlTemplate="Shops.aspx" />
                    </Items>
                </f:FormRow>
                <f:HtmlEditor runat="server" Label="内容" ID="tbContent" Height="400px" Editor="UMEditor" BasePath="~/res/third-party/umeditor/" ToolbarSet="Full"  />
            </Items>
        </f:Form>
    </form>
    </body>
</html>
