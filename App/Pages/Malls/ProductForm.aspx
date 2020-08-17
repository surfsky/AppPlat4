<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProductForm.aspx.cs"  Inherits="App.Pages.ProductForm" ValidateRequest="false" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <style>
        .photo {
            height: 150px;
            line-height: 150px;
            overflow: hidden;
        }
        .photo img {
            height: 150px;
            vertical-align: middle;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="form2" runat="server" />
        <f:Form ID="form2" ShowBorder="false" ShowHeader="false"  AutoScroll="true" runat="server" BodyPadding="10px" >
            <Items>
                <f:Panel  runat="server" ShowBorder="false" ShowHeader="false" Layout="HBox">
                    <Items>
                        <f:Panel  runat="server" ShowBorder="false" ShowHeader="false"  BoxFlex="5">
                            <Items>
                                <f:TextBox ID="tbName" runat="server" Label="名称" ShowRedStar="true" />
                                <f:TextBox ID="tbBarCode" runat="server" Label="条码" />
                                <f:DropDownList ID="ddlShop" runat="server" Label="商店" />
                                <f:DateTimePicker ID="tbCreateDt" runat="server" Label="创建日期" Readonly="true"/>
                                <f:RadioButtonList ID="rblOnShelf" runat="server" Label="是否上架" ShowRedStar="true" />
                            </Items>
                        </f:Panel>
                        <f:Panel  runat="server" ShowBorder="false" ShowHeader="false" BoxFlex="5">
                            <Items>
			                    <f:Thrumbnail ID="imgPhoto" CssClass="photo" ImageUrl="~/res/images/blank.png" ShowEmptyLabel="true" runat="server" />
                                <f:FileUpload runat="server" ID="uploader" ShowRedStar="false" ShowEmptyLabel="true"
                                    ButtonText="上传图像" ButtonOnly="true" Required="false" ButtonIcon="ImageAdd" AcceptFileTypes="image/*"
                                    AutoPostBack="true" OnFileSelected="uploader_FileSelected"/>
                                <f:TextBox ID="tbID" runat="server" Label="ID" Readonly="true" Hidden="true" />
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Panel>


                <f:RadioButtonList id="rblType" runat="server" Label="类型" ShowRedStar="true"  />
                <f:FormRow runat="server">
                    <Items>
                        <f:NumberBox ID="tbPrice" runat="server" Label="价格（元）"  DecimalPrecision="2" EmptyText="以规格数据为准"  />
                        <f:NumberBox ID="tbRawPrice" runat="server" Label="原价（元）" DecimalPrecision="2" EmptyText="以规格数据为准"  />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:NumberBox ID="tbStock" runat="server" Label="库存" EmptyText="以规格数据为准" />
                        <f:Label ID="lblDiscount" runat="server" Label="折扣" />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:NumberBox ID="tbSaleCnt" runat="server" Label="已售数" />
                        <f:NumberBox ID="tbPositiveCnt" runat="server" Label="好评数" />
                    </Items>
                </f:FormRow>
                <f:TextArea ID="tbDescription" runat="server" Label="描述"  />
                <f:HtmlEditor runat="server" Label="协议" ID="tbProtocol" Height="400px" Editor="UMEditor" BasePath="~/res/third-party/umeditor/" ToolbarSet="Full"  />
                <f:FormRow runat="server">
                    <Items>
                        <f:TextBox ID="tbSpecName1" runat="server" Label="规格标题"  />
                        <f:TextBox ID="tbSpecName2" runat="server" Label="规格标题2" ShowLabel="false"  />
                        <f:TextBox ID="tbSpecName3" runat="server" Label="规格标题3" ShowLabel="false"  />
                        <f:Label runat="server" Text="." ShowLabel="false" />
                    </Items>
                </f:FormRow>
                <f:Panel runat="server" ID="panDetail" ShowBorder="false" Height="300px"  EnableIFrame="true"  />
            </Items>
        </f:Form>
    </form>
</body>
</html>
