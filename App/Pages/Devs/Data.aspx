<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Data.aspx.cs" Inherits="App.Pages.Data" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>数据模型管理</title>
</head>
<body>
    <form id="form1" runat="server">
    <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
    <f:Panel ID="Panel1" runat="server" BodyPadding="0px"  ShowBorder="false" Layout="Fit" ShowHeader="false" >
        <Toolbars>
            <f:Toolbar runat="server">
                <Items>
                    <f:Button runat="server" ID="btnDict" Icon="Database" Text="数据字典"  EnablePostBack="false" 
                        OnClientClick="window.top.addMainTab('DbSchema', '/Pages/Devs/DbSchema.ashx', '数据字典') "
                        />
                    <f:Button runat="server" ID="btnModel" Icon="Brick" Text="数据模型"  EnablePostBack="false" 
                        OnClientClick="window.top.addMainTab('DataModel', '/Pages/Devs/DataModel.ashx', '数据模型') "
                        />
                    <f:ToolbarFill runat="server" />
                    <f:TextBox runat="server" ID="tbName" EmptyText="类名" />
                    <f:Button runat="server" ID="btnSearch" Icon="SystemSearch" Text="查找" Type="Submit" OnClick="btnSearch_Click" />
                </Items>
            </f:Toolbar>
        </Toolbars>
        <Items>
            <f:Grid ID="Grid1" runat="server" ShowBorder="false" ShowHeader="false" 
                AllowSorting="true" OnSort="Grid1_Sort"  OnPreRowDataBound="Grid1_PreRowDataBound" >
                <Columns>
                    <f:RowNumberField />
                    <f:WindowField WindowID="win" Width="50" DataIFrameUrlFields="FullName" DataIFrameUrlFormatString="DataModel.ashx?tp={0}" Title="结构" HeaderText="结构" Icon="Brick" ColumnID="Model" />
                    <f:WindowField WindowID="win" Width="50" DataIFrameUrlFields="FullName" DataIFrameUrlFormatString="Datas.aspx?tp={0}"  Title="数据"  HeaderText="数据" Icon="Database" ColumnID="Data" />
                    <f:WindowField WindowID="win" Width="50" DataIFrameUrlFields="FullName" DataIFrameUrlFormatString="UIs.aspx?tp={0}"  Title="UI"  HeaderText="UI" Icon="PageWhiteGear" ColumnID="UI" />
                    <f:BoundField DataField="Group" ColumnID="Group" HeaderText="分组" SortField="Group" />
                    <f:BoundField DataField="Name" Width="200" HeaderText="名称" SortField="Name" />
                    <f:BoundField DataField="FullName" Width="400" HeaderText="全称" SortField="FullName" />
                    <f:BoundField DataField="Description" ColumnID="Description" HeaderText="说明" ExpandUnusedSpace="true" />
                </Columns>
            </f:Grid>
        </Items>
    </f:Panel>
    <f:Window runat="server" ID="win" Width="1000" Height="800" Title="查看" EnableIFrame="true" Hidden="true"  EnableResize="true" EnableMaximize="true" />
    </form>
</body>
</html>
