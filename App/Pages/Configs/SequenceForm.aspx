<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SequenceForm.aspx.cs"  Inherits="App.Admins.SequenceForm" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="form2" runat="server" />
        <f:Form ID="form2" ShowBorder="false" ShowHeader="false" runat="server" BodyPadding="10px"  AutoScroll="true">
            <Toolbars>
                <f:Toolbar runat="server">
                    <Items>
                        <f:Button runat="server" ID="btnGenerate" Text="生成新序列号" Icon="Build" OnClick="btnGenerate_Click" /> 
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:DropDownList runat="server" ID="ddlType" Label="类别" Required="true" ShowRedStar="true" />
                <f:DropDownList runat="server" ID="ddlLoop" Label="循环方式" Required="true" ShowRedStar="true" />
                <f:TextBox runat="server" ID="tbFormat" Label="格式" Required="true" ShowRedStar="true" />
                <f:DateTimePicker runat="server" ID="dpLastDt" Label="最后时间"  />
                <f:NumberBox runat="server" ID="tbLastValue" Label="最后值" DecimalPrecision="0" />
                <f:TextBox runat="server" ID="tbLastSeq" Label="最后序列值" Readonly="true" />
            </Items>
        </f:Form>
    </form>
</body>
</html>
