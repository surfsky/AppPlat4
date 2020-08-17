<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductSpecForm.aspx.cs"  Inherits="App.Pages.ProductSpecForm" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="form2" runat="server" />
        <f:Form ID="form2" ShowBorder="false" ShowHeader="false"  AutoScroll="true" runat="server" Layout="HBox" BodyPadding="10px">
            <Items>
                <f:Panel runat="server" ShowBorder="false" ShowHeader="false" BoxFlex="5">
                    <Items>
                        <f:TextBox ID="tbSpec1" runat="server" Label="规格1" />
                        <f:TextBox ID="tbSpec2" runat="server" Label="规格2" />
                        <f:TextBox ID="tbSpec3" runat="server" Label="规格3" />
                        <f:NumberBox ID="tbPrice" runat="server" Label="价格（元）"  />
                        <f:NumberBox ID="tbRawPrice" runat="server" Label="原价（元）" DecimalPrecision="2" />
                        <f:Label ID="lblDiscount" runat="server" Label="折扣" />
                        <f:NumberBox ID="tbStock" runat="server" Label="库存"/>
                        <f:NumberBox ID="tbData" runat="server" Label="数量（个数、天数等）"/>
                        <f:NumberBox ID="tbInsuranceDays" runat="server" Label="质保天数"/>
                        <f:NumberBox ID="tbSeq" runat="server" Label="顺序"/>
                    </Items>
                </f:Panel>
                <f:Panel runat="server" ShowBorder="false" ShowHeader="false" BoxFlex="5">
                    <Items>
                        <f:Thrumbnail ID="imgPhoto" CssClass="photo" ImageUrl="~/res/images/blank.png" ShowEmptyLabel="true" runat="server" />
                        <f:FileUpload runat="server" ID="uploader" ShowRedStar="false" ShowEmptyLabel="true"
                            ButtonText="上传图像" ButtonOnly="true" Required="false" ButtonIcon="ImageAdd" AcceptFileTypes="image/*"
                            AutoPostBack="true" OnFileSelected="uploader_FileSelected"/>
                    </Items>
                </f:Panel>
            </Items>
        </f:Form>
    </form>
</body>
</html>
