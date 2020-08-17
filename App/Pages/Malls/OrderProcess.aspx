<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderProcess.aspx.cs" Inherits="App.Pages.OrderProcess" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="form2" runat="server" />
        <f:Form ID="form2" ShowBorder="false" ShowHeader="false" runat="server" BodyPadding="10px" AutoScroll="true" >
            <Toolbars >
                <f:Toolbar runat="server" >
                    <Items>
                        <f:Button runat="server" ID="btnOk" Text="提交" Icon="SystemSave" OnClick="btnOk_Click" ValidateForms="form2" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Label runat="server" ID="lblOrder" Label="订单" />
                <f:FormRow runat="server">
                    <Items>
                        <f:Label runat="server" ID="lblAction" Label="操作" />
                        <f:Label runat="server" ID="lblStatus" Label="下步状态" />
                    </Items>
                </f:FormRow>
                <f:TextArea runat="server" ID="tbRemark" Label="备注" />
                <f:FormRow runat="server" >
                    <Items>
                        <f:FileUpload runat="server" ID="uploader1" ShowRedStar="false" Label="图像"
                            ButtonText="上传图像" ButtonOnly="true" Required="false" ButtonIcon="ImageAdd" AcceptFileTypes="image/*"
                            AutoPostBack="true" OnFileSelected="uploader_FileSelected"  />
                        <f:FileUpload runat="server" ID="uploader2" ShowRedStar="false" ShowEmptyLabel="true"
                            ButtonText="上传图像" ButtonOnly="true" Required="false" ButtonIcon="ImageAdd" AcceptFileTypes="image/*"
                            AutoPostBack="true" OnFileSelected="uploader_FileSelected"  />
                        <f:FileUpload runat="server" ID="uploader3" ShowRedStar="false" ShowEmptyLabel="true"
                            ButtonText="上传图像" ButtonOnly="true" Required="false" ButtonIcon="ImageAdd" AcceptFileTypes="image/*"
                            AutoPostBack="true" OnFileSelected="uploader_FileSelected"  />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:Thrumbnail runat="server" ID="img1" Width="100" ShowEmptyLabel="true" />
                        <f:Thrumbnail runat="server" ID="img2" Width="100" ShowEmptyLabel="false" ShowLabel="false"/>
                        <f:Thrumbnail runat="server" ID="img3" Width="100" ShowEmptyLabel="false" ShowLabel="false"/>
                    </Items>
                </f:FormRow>
            </Items>
        </f:Form>
    </form>
</body>
</html>
