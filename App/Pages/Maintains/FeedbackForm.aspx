<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FeedbackForm.aspx.cs"  Inherits="App.Admins.FeedbackForm" ValidateRequest="false" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="form2" runat="server" />
        <f:Form ID="form2" ShowBorder="false" ShowHeader="false" runat="server" BodyPadding="10px"   AutoScroll="true">
            <Toolbars>
                <f:Toolbar runat="server">
                    <Items>
                        <f:Button runat="server" ID="btnOperate" Text="处理" Icon="Wrench" OnClick="btnOperate_Click" />
                        <f:Button runat="server" ID="btnHistory" Text="处理历史" Icon="Time" OnClick="btnHistory_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:FormRow  runat="server">
                    <Items>
                        <f:Label runat="server" ID="lblId" Label="ID" />
                        <f:DropDownList runat="server" ID="ddlStatus" Label="状态" Required="true" Enabled="false" />
                    </Items>
                </f:FormRow>
                <f:FormRow  runat="server">
                    <Items>
                        <f:Label runat="server" ID="lblCreateDt" Label="提交时间" />
                        <f:Label runat="server" ID="lblUpdateDt" Label="修改时间" />
                    </Items>
                </f:FormRow>


                <f:Form runat="server" ID="formBase" ShowBorder="false">
                    <Items>
                        <f:FormRow runat="server">
                            <Items>
                                <f:DropDownList runat="server" ID="ddlType" Label="类型" Required="true" />
                                <f:DropDownList runat="server" ID="ddlApp"  Label="应用名" Required="true" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow runat="server">
                            <Items>
                                <f:TextBox      runat="server" ID="tbAppVersion"  Label="应用版本"  />
                                <f:TextBox      runat="server" ID="tbAppModule"  Label="应用模块"  />
                            </Items>
                        </f:FormRow>
                        <f:FormRow runat="server">
                            <Items>
                                <f:TextBox      runat="server" ID="tbUser"  Label="上报人"  />
                                <f:TextBox      runat="server" ID="tbContacts"  Label="联系方式"  />
                            </Items>
                        </f:FormRow>
                        <f:TextBox      runat="server" ID="tbTitle"  Label="标题"  ShowRedStar="true" Required="true" />
                        <f:HtmlEditor   runat="server" ID="tbContent" Label="内容" Height="400px" Editor="UMEditor" BasePath="~/res/third-party/umeditor/" ToolbarSet="Full"  />
                        <f:ImageUploader runat="server" ID="up1" Label="图片" UploadFolder="Feedbacks" OnFileUploaded="uploader_FileSelected" ImageSize="200,200"   />
                    </Items>
                </f:Form>

                <f:Form runat="server" ID="formReply" ShowBorder="false">
                    <Items>
                        <f:HtmlEditor   runat="server" ID="tbReply" Label="回复" Height="400px" Editor="UMEditor" BasePath="~/res/third-party/umeditor/" ToolbarSet="Full"  />
                        <f:ImageUploader runat="server" ID="up2" Label="回复图片" UploadFolder="Feedbacks" OnFileUploaded="uploader_FileSelected" ImageSize="200,200"   />
                    </Items>
                </f:Form>



                <f:Window runat="server" ID="winHistory" Width="800" Height="600" Hidden="true" EnableIFrame="true" EnableResize="true" Target="Top" BodyPadding="10"  />
                <f:Window runat="server" ID="winFlow" Width="600" Height="400" Hidden="true" EnableResize="true" Target="Top" BodyPadding="10" Title="请选择后继步骤"  >
                    <Toolbars>
                        <f:Toolbar runat="server" >
                            <Items>
                                <f:Button runat="server" ID="btnSubmit" Text="提交" Icon="Accept" OnClick="btnSubmit_Click"  />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:DropDownList runat="server" ID="ddlNextStep" Label="后继步骤"  EmptyText="--请选择--" AutoPostBack="true" OnSelectedIndexChanged="ddlNextStep_SelectedIndexChanged" Required="true" ShowRedStar="true" />
                        <f:PopupBox runat="server" ID="pbNextUser" Label="后继处理人" WinTitle="指定后继处理人" WinWidth="800" WinHeight="600" ShowSearcher="false" />
                        <f:DateTimePicker runat="server" ID="dtNextDt" Label="限制完成时间"  />
                        <f:TextArea runat="server" ID="tbComment" Label="意见" />
                    </Items>
                </f:Window>
            </Items>
        </f:Form>
    </form>
</body>
</html>
