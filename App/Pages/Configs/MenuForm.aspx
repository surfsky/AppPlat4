<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MenuForm.aspx.cs" Inherits="App.Admins.MenuForm" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="form2" runat="server" />
        <f:Form ID="form2" ShowBorder="false" ShowHeader="false" runat="server" BodyPadding="10px" AutoScroll="true">
            <Items>
                <f:FormRow runat="server">
                    <Items>
                        <f:TextBox ID="tbName" runat="server" Label="菜单名称" Required="true" ShowRedStar="true" />
                        <f:DropDownList ID="ddlParentMenu" Label="上级菜单" Required="false" ShowRedStar="false" runat="server" />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:NumberBox ID="tbSeq" Label="排序" Required="true" ShowRedStar="true" runat="server" />
                        <f:DropDownList ID="ddlViewPower" runat="server" Label="浏览权限" EnableEdit="false" />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:CheckBox ID="chkVisible" runat="server" Label="是否可见" Checked="true" />
                        <f:CheckBox ID="chkOpen" runat="server" Label="是否展开" Checked="false" />
                    </Items>
                </f:FormRow>
                <f:FileBox runat="server" ID="fbUrl" Label="链接"  Filter=".aspx .ashx .html"  Root="/"/>
                <f:PopupBox runat="server" ID="pbParam" Label="参数"  OnTrigger2Click="pbParam_Trigger2Click" EnableEdit="true" />
                <f:FileBox runat="server" ID="fbIcon" Label="图标" Filter=".jpg .png .gif" Root="/res/icon/" />

                <f:RadioButtonList ID="iconList" ColumnNumber="8" ShowEmptyLabel="true" runat="server" />
                <f:TextArea ID="tbRemark" runat="server" Label="备注" Height="100" />
            </Items>
        </f:Form>
    </form>

    <script type="text/javascript">
        F.ready(function () {
            var iconList = F('<%= iconList.ClientID %>');
            var tbxIcon = F('<%= fbIcon.ClientID %>');

            iconList.on('change', function (field) {
                tbxIcon.setValue(iconList.getValue());
            });

            tbxIcon.on('change', function (field, newValue, oldValue) {
                iconList.setValue(newValue);
            });
        });
    </script>
</body>
</html>
