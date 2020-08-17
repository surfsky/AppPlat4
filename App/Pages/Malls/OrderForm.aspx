<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderForm.aspx.cs"  Inherits="App.Pages.OrderForm" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="form2" runat="server" />
        <f:Form ID="form2" ShowBorder="false" ShowHeader="false"  AutoScroll="true" runat="server"  BodyPadding="10px">
            <Toolbars>
                <f:Toolbar runat="server">
                    <Items>
                        <f:DropDownList runat="server" ID="ddlNextStep"  EmptyText="下一步" />
                        <f:Button runat="server" ID="btnOperate" Text="处理" Icon="Wrench" OnClick="btnOperate_Click" />
                        <f:Button runat="server" ID="btnHistory" Text="处理历史" Icon="Time" OnClick="btnHistory_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:FormRow runat="server">
                    <Items>
                        <f:TextBox runat="server" ID="tbID" Label="订单编号"  Readonly="true" Hidden="false" />
                        <f:TextBox runat="server" ID="tbGUID" Label="订单流水号" Required="true" ShowRedStar="true" />
                    </Items>
                </f:FormRow>
                <f:TextBox runat="server" ID="tbSummary" Label="概述" />
                <f:FormRow runat="server">
                    <Items>
                        <f:DropDownList runat="server" ID="ddlStatus" Label="状态"  Required="true" />
                        <f:DropDownList runat="server" ID="ddlType" Label="类别" />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:PopupBox runat="server" ID="pbUser" Label="用户" WinTitle="用户"     UrlTemplate="users.aspx" />
                        <f:PopupBox runat="server" ID="pbShop" Label="受理商店" WinTitle="商店" UrlTemplate="shops.aspx" />
                    </Items>
                </f:FormRow>
                <f:FormRow runat="server">
                    <Items>
                        <f:Label runat="server" ID="lblCreate" Label="创建时间" />
                        <f:Label runat="server" ID="lblExpire" Label="过期时间" />
                    </Items>
                </f:FormRow>
                <f:TextBox runat="server" ID="tbRemark" Label="备注" />

                <f:Form runat="server" ID="panAppt" ShowHeader="true" ShowBorder="false" Title="维修信息"  EnableCollapse="true" Icon="Wrench" >
                    <Items>
                        <f:FormRow runat="server" ID="rowAppt">
                            <Items>
                                <f:Label runat="server" ID="lblApptDevice" Label="维修设备" />
                                <f:Label runat="server" ID="lblApptUser" Label="维修人" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow runat="server" ID="FormRow1">
                            <Items>
                                <f:Label runat="server" ID="lblApptMobile" Label="联系方式" />
                                <f:Label runat="server" ID="lblApptDt" Label="预约时间" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow runat="server" ID="FormRow2">
                            <Items>
                                <f:Label runat="server" ID="lblApptRemark" Label="预约备注" />
                                <f:PopupBox runat="server" ID="pbHandleShop" Label="送修商店" WinTitle="商店" UrlTemplate="shops.aspx" />
                            </Items>
                        </f:FormRow>
                    </Items>
                </f:Form>

                <f:Form runat="server" ID="Form3" ShowHeader="true" ShowBorder="false" Title="支付信息"  EnableCollapse="true" Icon="Money" >
                    <Items>
                        <f:FormRow runat="server">
                            <Items>
                                <f:DropDownList runat="server" ID="ddlPayMode" Label="支付方式" />
                                <f:Label runat="server" ID="lblPayDt" Label="支付时间"  />
                            </Items>
                        </f:FormRow>
                        <f:FormRow runat="server">
                            <Items>
                                <f:NumberBox runat="server" ID="tbTotalMoney" Label="总费用" Enabled="false"/>
                                <f:NumberBox runat="server" ID="tbPayMoney" Label="实付费用" Required="true" ShowRedStar="true"/>
                            </Items>
                        </f:FormRow>
                    </Items>
                </f:Form>

                <f:Panel runat="server" ID="panDetail" ShowBorder="false" Height="300px"  EnableIFrame="true"   />
                <f:Window runat="server" ID="win" EnableIFrame="true" Width="800" Height="600" Hidden="true" EnableResize="true" Target="Top"  />
            </Items>
        </f:Form>
    </form>
</body>
</html>
