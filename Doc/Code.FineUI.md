----------------------------------------------
FineUI
----------------------------------------------
FineUI
	FineUI 信息提示
		// 显示提示窗口
		function showNotify(value) {
			// 有问题，标签时显示时不显示，估计和默认的显隐逻辑冲突了
			//$("#f_ajax_loading").text('保存成功').show();
			//setTimeout("$('#f_ajax_loading').text('正在加载...').hide()", 3000);

			// 小提示窗口，带标题栏关闭按钮。太丑了，不用。（供参考）
			// new Ext.ux.Notification({
			//    autoHide: true,
			//    hideDelay: 2000
			//}).showMessage('操作提示', '<h1>' + value + '</h1>', true);
		}
		PageContext.RegisterStartupScript("showNotify('已保存');");
	FineUI按钮、windows列、绑定列
		<f:Button ID="btnClose"  Icon="SystemSaveClose" OnClick="btnClose_Click" runat="server" Text="关闭" Hidden="true" />
		<f:WindowField  WindowID="Window1" DataTextField="Coach.User.Name" DataIFrameUrlFields="Coach.User.ID" DataIFrameUrlFormatString="CoachForm.aspx?userid={0}&mode=view" Width="100px" HeaderText="教练" />
		<f:WindowField  WindowID="Window1" Text="学员" DataIFrameUrlFields="ID" DataIFrameUrlFormatString="ClassEnrolls.aspx?classId={0}&mode=view" Width="100px" HeaderText="学员" />
		<f:BoundField DataField="Type" SortField="Type" HeaderText="类别" Width="100" ColumnID="Type" />
		<f:WindowField WindowID="Window1" ToolTip="查看" HeaderText="商店" Width="100px" 
			DataTextField="Shop.Name" SortField="Shop.Name"
			DataIFrameUrlFields="Shop.ID"  DataIFrameUrlFormatString="ShopForm.aspx?mode=view&id={0}" 
			/>
		<f:WindowField WindowID="Window1" ToolTip="查看" HeaderText="邀请人" Width="100px" 
			DataTextField="Inviter.NickName" SortField="Inviter.Name"
			DataIFrameUrlFields="Inviter.ID"  DataIFrameUrlFormatString="UserForm.aspx?mode=view&id={0}" 
			/>
		<f:TemplateField HeaderText="性别">
			<ItemTemplate>
				<%# GetGender(Eval("Gender")) %>
			</ItemTemplate>
		</f:TemplateField>
		<f:BoundField DataField="Title" Width="100px" HeaderText="职务" ColumnID="Titles" Hidden="true"  />
		<f:BoundField DataField="Email" SortField="Email" Width="150px" HeaderText="邮箱" Hidden="true" />
		<f:BoundField DataField="Phone" SortField="Phone" Width="150px" HeaderText="办公电话" Hidden="true" />
		<f:BoundField DataField="IdentityCard" SortField="IdentityCard" Width="150px" HeaderText="身份证" Hidden="true" />
		<f:BoundField DataField="Birthday" SortField="Birthday" Width="150px" HeaderText="生日" Hidden="true" />
		<f:BoundField DataField="TakeOfficeDt" SortField="TakeOfficeDt" Width="150px" HeaderText="就职日期" Hidden="true" />
		<f:BoundField DataField="LastLoginDt" SortField="LastLoginDt" Width="150px" HeaderText="最后登录日期" Hidden="true" />
		<f:BoundField DataField="Remark" ExpandUnusedSpace="true" HeaderText="备注" Hidden="true" />
		<f:HyperLinkField HeaderText="图片" Width="150px" DataTextField="CoverImage" DataTextFormatString="<img src='{0}?w=150'/>" DataNavigateUrlFields="CoverImage" HtmlEncode="false" UrlEncode="false" />
	grid 自定义单元格显示
        /// <summary>在渲染每一行后调用，此时 Values 属性保存了每一项渲染后的 HTML 片段</summary>
        protected void Grid1_RowDataBound(object sender, GridRowEventArgs e)
        {
            // e.DataItem  -> System.Data.DataRowView 或者自定义类
            // e.RowIndex -> 当前行序号（从 0 开始）
            // e.Values -> 当前行每一列渲染后的 HTML 片段
            Type item = e.DataItem as Type;
            var groupIndex = Grid1.FindColumn("Group").ColumnIndex;
            var descIndex = Grid1.FindColumn("Description").ColumnIndex;
            e.Values[groupIndex] = item.GetUIGroup();
            e.Values[descIndex] = item.GetDescription();
        }
	client
		PageContext.RegisterStartupScript(...);
		Alert.ShowInTop("触发了 Window1 的关闭事件！");
		TriggerBox1.OnClientTriggerClick
	windows
		ActiveWindow.GetWriteBackValueReference(TextBox1.Text, TextBox1.Text + " - 第二个值")
		ActiveWindow.GetHideReference()
		ActiveWindow.GetHidePostBackReference()
		Window1.GetSaveStateReference(TriggerBox1.ClientID, HiddenField1.ClientID)
		Window1.GetShowReference("./triggerbox_iframe_iframe.aspx");
		Alert.ShowInTop(HttpUtility.HtmlEncode(editorContent));
		JsHelper.Enquote(content)
	form
		btnReset.OnClientClick = SimpleForm1.GetResetReference();
		tbxUserName.MarkInvalid(String.Format("{0} 是保留字，请另外选择！", tbxUserName.Text));
	窗体回调
		main.aspx
			if (Page.IsPostback)
			{
				var inviters = Request.Params["__EVENTARGUMENT"];
				if (inviters.StartsWith("Select_inviter_"))
				{
					var inviterIds = inviters.Substring("Select_inviter_".Length);
					this.lblInfo.Text = string.Format("Select inviter: {0}", inviterIds);
				}
			}
			protected void btnSelectInviter_Click(object sender, EventArgs e)
			{
				Window1.IFrameUrl = "~/Admins/Users.aspx?mode=select&selectName=inviter";
				PageContext.RegisterStartupScript(Window1.GetShowReference());
			}
		win.aspx
			PageContext.RegisterStartupScript("parent.__doPostBack('','Test3WindowClose');");
		    <f:Button runat="server" ID="btnSelect" OnClick="btnSelect_Click" Text="选择并关闭" Icon="ShapesManySelect" />
            UI.SetVisible(this.btnSelect, (this.Mode == PageMode.Select));
	Grid
		不允许排序，不允许分页就会报错
			<f:GridPro ID="Grid1" runat="server" SortDirection="DESC" WinHeight="400"
			ShowNumberField="true"  ShowEditField="true" ShowDeleteField="true" ShowViewField="true"
			OnDelete="Grid1_Delete" AutoCreateFields="false" WinWidth="800" 
			NewText="新增规格" DeleteText="删除规格"
			>
	Form布局
		<Rows>
            <f:FormRow>
                <Items>
                    <f:Label ID="Label2" Label="申请人" Text="三生石上" CssClass="highlight" runat="server" />
                    <f:Label ID="Label3" Label="电话" Text="0551-1234567" runat="server" />
                </Items>
            </f:FormRow>
		</Rows>


下载文件
    这里直接对Response对象进行操作，所以在导出和下载时要禁用AJAX。代码如：
``` xml
<f:Button ID="Button1" EnableAjax="false" DisableControlBeforePostBack="false"
    runat="server" Text="导出为Excel文件" OnClick="Button1_Click">
</f:Button>
```


----------------------------------------------
FineUI js
----------------------------------------------
js
	F.ready(function () {
	F.cookie('Theme_Pro', themeName, {expires: 100});
	var themeName = F.cookie('Theme_Pro');
	F.initTreeTabStrip(treeMenu, mainTabStrip, {
		maxTabCount: 10,
		maxTabMessage: '请先关闭一些选项卡（最多允许打开 10 个）！'
		});
	F.alert('尚未实现');

标签控制
	parent.removeActiveTab();
	JsObjectBuilder joBuilder = new JsObjectBuilder();
    joBuilder.AddProperty("id", "grid_newtab_addnew");
    joBuilder.AddProperty("title", "新增人员");
    joBuilder.AddProperty("iframeUrl", ResolveUrl("~/grid/grid_newtab_window.aspx"));
    joBuilder.AddProperty("refreshWhenExist", true);
    joBuilder.AddProperty("iconFont", "plus");
    btnNew.OnClientClick = String.Format("parent.addExampleTab({0});", joBuilder);

遍历客户端控件
	var itemNames = [], itemCount = 0;
	$.each(F.ui, function(name, item) {
		itemNames.push(name);
		itemCount++;
	});
	console.log(itemNames.join(','));
	console.log('控件总数：' + itemCount);

耗时统计
	<head id="Head1" runat="server">
		<title></title>
		<link href="../res/css/main.css" rel="stylesheet" type="text/css" />
		<script>
			var _startTime = new Date();
		</script>
	</head>
	在页面的最底部：
	</script>
		F.ready(function () {
			F.notify('耗时：' + ((new Date() - _startTime) / 1000).toFixed(2) + ' 秒');
		});
	</script>

统计ajax耗时
    function onbtnLookUpHisRevClick() {
        _startAjaxTime = new Date();
    }
    F.ajaxReady(function () {
        F.notify('AJAX耗时：' + ((new Date() - _startAjaxTime) / 1000).toFixed(2) + ' 秒');
    });


Grid

        /// <summary>
        /// 供普通Grid快速显示只读数据用，本方法会自动生成列。
        /// 由于其列是自动生成的，请在每次页面访问时都调用。
        /// 要想实现完整的分页、排序、删除、编辑等逻辑，请用GridPro控件。
        /// </summary>
        public static void BindGrid(this Grid grid, DataTable dt)
        {
            grid.Columns.Clear();
            foreach (DataColumn col in dt.Columns)
            {
                grid.Columns.Add(CreateBoundColumn(col.ColumnName, 100, col.ColumnName, ""));
            }
            grid.DataSource = dt;
            grid.DataBind();
        }
        public static void BindGrid(this Grid grid, UISetting ui, object data, string windowId)
        {
            grid.Columns.Clear();
            AddColumns(grid, ui, windowId);
            grid.DataSource = data;
            grid.DataBind();
        }
                    public static void BindGrid(this Grid grid, Type type, object data, string windowId)
        {
            BindGrid(grid, new UISetting(type), data, windowId);
        }


        //--------------------------------------------------
        // 添加列
        //--------------------------------------------------
        /// <summary>添加绑定列</summary>
        public static Grid AddColumn<T>(this Grid grid, Expression<Func<T, object>> textField, int width, string title = "",
            string formatString = "{0}",
            Expression<Func<T, object>> sortField = null,
            Expression<Func<T, object>> treeField = null
            )
        {
            var prop = textField.GetPropertyInfo();
            var name = textField.GetExpressionName();
            var desc = prop.GetTitle();
            var canSort = prop.CanWrite;
            var sort = sortField.GetExpressionName();
            var tree = treeField.GetExpressionName();

            var field = new FineUIPro.BoundField()
            {
                DataField = name,
                Width = width,
                DataFormatString = formatString,
                HeaderText = title,
            };

            // 设置标题、排序、列ID
            if (sort.IsNotEmpty())
                field.SortField = sort;
            if (tree.IsNotEmpty())
                field.DataSimulateTreeLevelField = tree;
            FixColumn(field, name, desc, canSort);
            grid.Columns.Add(field);
            return grid;
        }
        public static Grid  AddColumn(this Grid grid, string dataField, int width, string title, string formatString="{0}")
        {
            var column =  new FineUIPro.BoundField()
            {
                DataField = dataField,
                SortField = dataField,
                HeaderText = title,
                DataFormatString = formatString,
                Width = width
            };
            grid.Columns.Add(column);
            return grid;
        }


        /// <summary>创建表单弹窗列（未启用）</summary>
        private static BaseField CreateFormColumn(UIAttribute ui, string windowId, ITypeHandler handler)
        {
            var url = handler.FormUrl + "?id={0}";
            var member = handler.Name;
            var id = handler.Id;
            var field = ui.Name;
            var col = new FineUIPro.WindowField()
            {
                SortField = ui.Name,
                HeaderText = ui.Title,
                Width = ui.ColumnWidth,
                DataIFrameUrlFormatString = url,
                WindowID = windowId,
                DataTextField = field + "." + member,
                DataIFrameUrlFields = field + "." + id
            };
            SetFieldSort(ui, col);
            return col;
        }



        /// <summary>预处理树。加上根节点，禁止某些节点。（试试看，不用复制，减少接口数目）</summary>
        static List<T> PrepareTree<T>(List<T> source, string title = "--根节点--", long? disableId = null) 
            where T : ITree, new()
        {
            bool addRootNode = title.IsNotEmpty(); // !string.IsNullOrEmpty(title);
            List<T> result = new List<T>();

            // 添加根节点
            if (addRootNode)
            {
                T root = new T();
                root.Name = title;
                root.ID = -1;
                root.TreeLevel = 0;
                root.Enabled = true;
                result.Add(root);
            }

            // 拷贝到新列表
            foreach (T item in source)
            {
                T newT = (T)item.Clone();
                result.Add(newT);

                // 有根节点的话，所有节点的TreeLevel加一
                if (addRootNode)
                    newT.TreeLevel++;
            }

            // 当前节点及子节点都不可选择
            if (disableId != null)
            {
                bool startChildNode = false;
                int startTreeLevel = 0;
                foreach (T node in result)
                {
                    if (node.ID == disableId.Value)
                    {
                        startTreeLevel = node.TreeLevel;
                        node.Enabled = false;
                        startChildNode = true;
                    }
                    else
                    {
                        if (startChildNode)
                        {
                            if (node.TreeLevel > startTreeLevel)
                                node.Enabled = false;
                            else
                                startChildNode = false;
                        }
                    }
                }
            }
            return result;
        }