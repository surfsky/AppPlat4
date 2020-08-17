<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RolePowers.aspx.cs" Inherits="App.Admins.RolePowers" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title></title>
    <style>
        ul.powers {
            margin: 0;
            padding: 0;
        }

        ul.powers li {
            margin: 5px 15px 5px 0;
            display: inline-block;
            min-width: 150px;
        }

        ul.powers li input {
            vertical-align: middle;
        }

        ul.powers li label {
            margin-left: 5px;
        }

        /* 自动换行，放置权限列表过长 */
        .f-grid-row .f-grid-cell-inner {
            white-space: normal;
        }
        .red {
            color: #ff0000!important;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="RegionPanel1" runat="server"></f:PageManager>
        <f:RegionPanel ID="RegionPanel1" ShowBorder="false" runat="server">
            <Regions>

                <f:Region ID="Region1" ShowBorder="false" ShowHeader="false" Width="200px" Position="Left" Layout="Fit" BodyPadding="0px" runat="server" Split="true">
                    <Toolbars>
                        <f:Toolbar  runat="server">
                            <Items>
                                <f:Button runat="server" ID="btnRole" Text="设置角色" Icon="PageGear" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:Grid ID="Grid1" runat="server" ShowBorder="true" ShowHeader="false" EnableCheckBoxSelect="false" DataKeyNames="ID" 
                            AllowSorting="false" SortField="Name" SortDirection="ASC" 
                            AllowPaging="false" EnableMultiSelect="false" 
                            OnRowClick="Grid1_RowClick" EnableRowClickEvent="true">
                            <Columns>
                                <f:BoundField DataField="ID" SortField="ID" HeaderText="ID" Width="80" />
                                <f:BoundField DataField="Name" SortField="Name" ExpandUnusedSpace="true" HeaderText="名称" />
                            </Columns>
                        </f:Grid>
                    </Items>
                </f:Region>


                <f:Region ID="Region2" ShowBorder="false" ShowHeader="false" Position="Center" Layout="Fit" BodyPadding="0px" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:Button ID="btnSelectAll" EnablePostBack="false" runat="server" Text="全选" Icon="Accept" OnClientClick="setCheck(true)"  />
                                <f:Button ID="btnUnSelectAll" EnablePostBack="false" runat="server" Text="全不选" Icon="Cross"  OnClientClick="setCheck(false)" />
                                <f:Button ID="btnGroupUpdate" Icon="SystemSave" runat="server" Text="保存" OnClick="btnGroupUpdate_Click"  />
                                <f:ToolbarFill runat="server" />
                                <f:Label ID="lblInfo" runat="server" Text="."  CssStyle="color:red" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:Grid ID="Grid2" runat="server" ShowBorder="true" ShowHeader="false" DataKeyNames="Id,Name" 
                            AllowSorting="false"  AllowPaging="false" OnRowDataBound="Grid2_RowDataBound" SortField="GroupName" SortDirection="DESC" 
                            EnableMultiSelect="false" EnableCheckBoxSelect="false" CheckBoxSelectOnly="true"  ShowSelectedCell="true"
                            >
                            <Columns>
                                <f:RowNumberField />
                                <f:TemplateField HeaderText="全选" Width="80px" >
                                    <ItemTemplate>
                                        <input type="checkbox" class="powerGroup" />
                                    </ItemTemplate>
                                </f:TemplateField>
                                <f:CheckBoxField ColumnID="CheckBoxField1" Width="100px" RenderAsStaticField="false" HeaderText="复选框列" Hidden="true" />
                                <f:BoundField DataField="Group" SortField="Group" HeaderText="分组名称" Width="120px" />
                                <f:TemplateField ExpandUnusedSpace="true" ColumnID="Powers" HeaderText="权限列表" >
                                    <ItemTemplate>
                                        <asp:CheckBoxList ID="ddlPowers" CssClass="powers" RepeatLayout="Flow" RepeatDirection="Horizontal" runat="server" />
                                    </ItemTemplate>
                                </f:TemplateField>
                            </Columns>
                        </f:Grid>
                    </Items>
                </f:Region>
            </Regions>
        </f:RegionPanel>

        <f:Window ID="Window1" CloseAction="HidePostBack" runat="server" IsModal="true" Hidden="true" Target="Top" EnableResize="true" EnableMaximize="true" 
            EnableIFrame="true" IFrameUrl="about:blank" Width="700px" Height="450px"
            OnClose="Window1_Close" />
    </form>


    <script>
        // FineUI准备就绪
        F.ready(function () {
            var grid2ID = '<%= Grid2.ClientID %>';
            //F(grid2ID).addListener('viewready', function () {createQtip();});  // 无此事件，先屏蔽

            /*
            // 权限行全选或反选
            $('.f-grid-checkbox').click(function () {
                var checked = this.checked;
                var row = this.parents(".f-grid-row")[0];
                row.children("input[type='checkbox'").prop("checked", checked);
            });
            */

            // 权限行全选或反选
            $('.powerGroup').click(function () {
                var checked = this.checked;                   // this 是 dom 对象
                var row = $(this).parents(".f-grid-row")[0];  // row 是 dom 对象
                $(row).find("input[type='checkbox']").prop("checked", checked);
            });
        });

        // 设置QTip
        function createQtip() {
            $('.powers li span').each(function (el) {
                var qtip = el.getAttribute('data-qtip');
                el.select('input,label').set({ 'f:qtip': qtip });
            });
        }

        // 全选或全不选
        function setCheck(checked) {
            $("input[type='checkbox']").prop("checked", checked);
        }
    </script>
</body>
</html>
