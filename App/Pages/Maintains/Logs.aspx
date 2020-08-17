<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logs.aspx.cs" Inherits="App.Admins.Logs" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
    <f:Panel ID="Panel1" runat="server" BodyPadding="0px" ShowBorder="false" ShowHeader="false" Layout="Fit">
        <Toolbars>
            <f:Toolbar ID="Toolbar1" runat="server">
                <Items>
                    <f:Button ID="btnDelete" Icon="Delete" runat="server" Text="删除一个月前的记录" OnClick="btnDelete_Click" />
                    <f:Button ID="btnLogConfig" Icon="Wrench" runat="server" Text="配置" OnClick="btnLogConfig_Click" />
                </Items>
            </f:Toolbar>
            <f:Toolbar runat="server">
                <Items>
                    <f:DropDownList ID="ddlSearchLevel" runat="server" Width="120px" />
                    <f:DropDownList ID="ddlSearchRange" runat="server"  Width="120px"  EnableEdit="true"
                        EmptyText="-- 时间范围 --" AutoSelectFirstItem="false" ForceSelection="true"
                        >
                        <f:ListItem Text="今天" Value="TODAY"/>
                        <f:ListItem Text="最近一周" Value="LASTWEEK"  Selected="true" />
                        <f:ListItem Text="最近一个月" Value="LASTMONTH" />
                        <f:ListItem Text="最近一年" Value="LASTYEAR" />
                    </f:DropDownList>
                    <f:TextBox runat="server" ID="tbFrom" EmptyText="来源" Width="100px" />
                    <f:TextBox runat="server" ID="tbOperator" EmptyText="操作人" Width="100px" />
                    <f:TextBox runat="server" ID="tbIP" EmptyText="IP" Width="100px" />
                    <f:TextBox runat="server" ID="tbMessage" EmptyText="信息" Width="100px" />
                    <f:Button runat="server" ID="btnSearch" Icon="SystemSearch" Text="查找" OnClick="btnSearch_Click" Type="Submit"  />
                </Items>
            </f:Toolbar>
        </Toolbars>
        <Items>
            <f:GridPro ID="Grid1" runat="server" WinWidth="800" WinHeight="1000" 
                SortDirection="DESC" ShowNumberField="true"  ShowViewField="true"  ShowIDField="true"
                AllowNew="false" AllowDelete="false" AllowBatchDelete="false"
                OnPreRowDataBound="Grid1_PreRowDataBound" 
                />
        </Items>
    </f:Panel>
    </form>
</body>
</html>
