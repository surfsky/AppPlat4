<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShopForm.aspx.cs"  Inherits="App.Pages.ShopForm" ValidateRequest="false" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="form2" runat="server" />
        <f:Form ID="form2" ShowBorder="false" ShowHeader="false"  AutoScroll="true" runat="server" BodyPadding="10">
            <Items>
                <f:Panel runat="server" ShowBorder="false" ShowHeader="false"  AutoScroll="true" Layout="HBox">
                    <Items>
                        <f:Panel runat="server"  ShowBorder="false" ShowHeader="false" BoxFlex="5" >
                            <Items>
                                <f:DropDownList ID="ddlCity" runat="server" Label="区域" Required="true" ShowRedStar="true" />
                                <f:TextBox ID="tbName" runat="server" Label="名称" Required="true" ShowRedStar="true" />
                                <f:TextBox ID="tbAbbrName" runat="server" Label="简写名称" Required="true" ShowRedStar="true" MaxLength="10" EmptyText="最多10个字符" />
                                <f:TextBox ID="tbTel" runat="server" Label="服务电话"/>
                                <f:PopupBox runat="server" ID="pbGPS" Label="经纬度"  UrlTemplate="~/Pages/Common/MapTencent.aspx" WinWidth="800" WinHeight="600" WinTitle="位置"  Trigger2IconUrl="~/Res/Icon/World.png"  />
                                <f:TextBox ID="tbAddr" runat="server" Label="详细地址"  />
                            </Items>
                        </f:Panel>
                        <f:Panel runat="server" ShowHeader="false" ShowBorder="false" BoxFlex="5">
                            <Items>
                                <f:Thrumbnail ID="imgPhoto" CssClass="photo" ImageUrl="~/res/images/blank.png" ShowEmptyLabel="true" runat="server" />
                                <f:FileUpload runat="server" ID="uploader" ShowRedStar="false" ShowEmptyLabel="true"
                                    ButtonText="上传图像" ButtonOnly="true" Required="false" ButtonIcon="ImageAdd" AcceptFileTypes="image/*"
                                    AutoPostBack="true" OnFileSelected="uploader_FileSelected"/>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Panel>
                <f:HtmlEditor runat="server" Label="说明" ID="tbDescription" Height="400px" Editor="UMEditor" BasePath="~/res/third-party/umeditor/" ToolbarSet="Full"  />
                <f:Image ID="imgMPInvite" runat="server"  Label="微信小程序邀请二维码" ImageWidth="240"  Hidden="true" />
                <f:Image ID="imgWebInvite" runat="server"  Label="微信公众号邀请二维码" ImageWidth="240"  />
            </Items>
        </f:Form>
    </form>
    </body>

</html>
