<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserForm.aspx.cs" Inherits="App.Pages.UserForm" %>

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
        <f:Form ID="form2" ShowBorder="false" ShowHeader="false"  AutoScroll="true" runat="server"  BodyPadding="10px">
            <Items>
                <f:Panel runat="server" Layout="HBox" ShowHeader="false" ShowBorder="false">
                    <Items>
                        <f:Panel runat="server" ShowHeader="false" ShowBorder="false" BoxFlex="5" MarginRight="5px" >
                            <Items>
                                <f:TextBox ID="tbName" runat="server" Label="用户名" Required="true" ShowRedStar="true" EmptyText="建议用手机号码，避免重复。用户名一旦设置将不允许修改。" />
                                <f:TextBox ID="tbNickName" runat="server" Label="昵称" Required="true" ShowRedStar="true" EmptyText="可填写用户姓名" />
                                <f:TextBox ID="tbRealName" runat="server" Label="真实姓名" />
                                <f:TextBox ID="tbMobile" runat="server" Label="手机号" ShowRedStar="true" />
                                <f:TextBox ID="tbTitle" runat="server" Label="头衔" />
		                        <f:PopupBox runat="server" ID="pbShop" Label="归属商店" WinTitle="归属商店" UrlTemplate="shops.aspx" />
	                            <f:PopupBox runat="server" ID="pbInviter" Label="邀请人" WinTitle="邀请人"  UrlTemplate="users.aspx" />
                                <f:DropDownList runat="server" ID="ddlArea" Label="区域" />
                                <f:CheckBox ID="cbEnabled" runat="server" Label="是否启用" />
                                <f:Label ID="lblRegistDt" runat="server" Label="注册日期" />
                            </Items>
                        </f:Panel>
                        <f:Panel runat="server" ShowBorder="false" ShowHeader="false"  BoxFlex="5">
                            <Items>
                                <f:Thrumbnail ID="imgPhoto" CssClass="photo" ImageUrl="~/res/images/blank.png" ShowEmptyLabel="true" runat="server"/>
                                <f:FileUpload runat="server" ID="uploader" ShowRedStar="false" ShowEmptyLabel="true"
                                    ButtonText="上传图像" ButtonOnly="true" Required="false" ButtonIcon="ImageAdd" AcceptFileTypes="image/*"
                                    AutoPostBack="true" OnFileSelected="uploader_FileSelected"/>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Panel>

                <f:CheckBoxList runat="server" ID="cblRole" Label="拥有角色" />

                <f:Form runat="server" ShowHeader="true" EnableCollapse="true" Collapsed="true" Title="更多..." ShowBorder="false" BodyPadding="10" >
                    <Items>
                        <f:FormRow runat="server">
                            <Items>
                                <f:RadioButtonList ID="ddlGender" Label="性别" Required="true" ShowRedStar="true" runat="server">
                                    <f:RadioItem Text="男" Value="男" Selected="true" />
                                    <f:RadioItem Text="女" Value="女" />
                                </f:RadioButtonList>
                                <f:TextBox ID="tbIdCard" runat="server" Label="身份证" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow runat="server">
                            <Items>
                                <f:TextBox ID="tbPhone" runat="server" Label="工作电话" />
                                <f:DropDownList runat="server" ID="ddlDept" Label="归属部门" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow runat="server">
                            <Items>
                                <f:TextBox ID="tbEmail" runat="server" Label="邮箱" Required="false" ShowRedStar="false" RegexPattern="EMAIL" />
                                <f:TextBox ID="tbQQ" runat="server" Label="QQ号" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow runat="server">
                            <Items>
                                <f:TextBox ID="tbWechat" runat="server" Label="微信号" />
                                <f:DatePicker ID="dpBirthday" runat="server" Label="生日" />
                            </Items>
                        </f:FormRow>
                        <f:CheckBoxList runat="server" ID="cblTitle" Label="拥有职务" />
                        <f:TextBox ID="tbSpecialty" runat="server" Label="特长" />
                        <f:TextArea ID="tbRemark" runat="server" Label="备注" Height="80" />
                        <f:Image ID="imgMPInvite" runat="server"  Label="微信小程序邀请二维码" ImageHeight="120" Hidden="true" />
                        <f:Image ID="imgWebInvite" runat="server"  Label="微信公众号邀请二维码" ImageHeight="120" />
                    </Items>
                </f:Form>


                <f:Panel runat="server" ID="panAdmin" ShowHeader="true" EnableCollapse="true" Collapsed="true" Title="管理员面板" ShowBorder="false" BodyPadding="10" Layout="VBox" >
                    <Items>
                        <f:TextBox ID="tbWechatUnionId" runat="server" Label="微信UnionId" />
                        <f:TextBox ID="tbWechatWebId" runat="server" Label="微信公众号OpenID" />
                        <f:TextBox ID="tbWechatWebSessionKey" runat="server" Label="微信公众号SessionKey"  />
                        <f:TextBox ID="tbWechatMPId" runat="server" Label="微信小程序OpenID" />
                        <f:TextBox ID="tbWechatMPSessionKey" runat="server" Label="微信小程序SessionKey"  />
                        <f:TextBox ID="tbLastGPS" runat="server" Label="最后地理位置"  />
                    </Items>
                </f:Panel>
            </Items>
        </f:Form>


        <f:Window ID="Window1" Title="编辑" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Top" IsModal="True" Width="550px"
            Height="350px"
            />
    </form>
</body>
</html>
