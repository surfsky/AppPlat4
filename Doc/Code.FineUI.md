----------------------------------------------
FineUI
----------------------------------------------
FineUI
	FineUI ��Ϣ��ʾ
		// ��ʾ��ʾ����
		function showNotify(value) {
			// �����⣬��ǩʱ��ʾʱ����ʾ�����ƺ�Ĭ�ϵ������߼���ͻ��
			//$("#f_ajax_loading").text('����ɹ�').show();
			//setTimeout("$('#f_ajax_loading').text('���ڼ���...').hide()", 3000);

			// С��ʾ���ڣ����������رհ�ť��̫���ˣ����á������ο���
			// new Ext.ux.Notification({
			//    autoHide: true,
			//    hideDelay: 2000
			//}).showMessage('������ʾ', '<h1>' + value + '</h1>', true);
		}
		PageContext.RegisterStartupScript("showNotify('�ѱ���');");
	FineUI��ť��windows�С�����
		<f:Button ID="btnClose"  Icon="SystemSaveClose" OnClick="btnClose_Click" runat="server" Text="�ر�" Hidden="true" />
		<f:WindowField  WindowID="Window1" DataTextField="Coach.User.Name" DataIFrameUrlFields="Coach.User.ID" DataIFrameUrlFormatString="CoachForm.aspx?userid={0}&mode=view" Width="100px" HeaderText="����" />
		<f:WindowField  WindowID="Window1" Text="ѧԱ" DataIFrameUrlFields="ID" DataIFrameUrlFormatString="ClassEnrolls.aspx?classId={0}&mode=view" Width="100px" HeaderText="ѧԱ" />
		<f:BoundField DataField="Type" SortField="Type" HeaderText="���" Width="100" ColumnID="Type" />
		<f:WindowField WindowID="Window1" ToolTip="�鿴" HeaderText="�̵�" Width="100px" 
			DataTextField="Shop.Name" SortField="Shop.Name"
			DataIFrameUrlFields="Shop.ID"  DataIFrameUrlFormatString="ShopForm.aspx?mode=view&id={0}" 
			/>
		<f:WindowField WindowID="Window1" ToolTip="�鿴" HeaderText="������" Width="100px" 
			DataTextField="Inviter.NickName" SortField="Inviter.Name"
			DataIFrameUrlFields="Inviter.ID"  DataIFrameUrlFormatString="UserForm.aspx?mode=view&id={0}" 
			/>
		<f:TemplateField HeaderText="�Ա�">
			<ItemTemplate>
				<%# GetGender(Eval("Gender")) %>
			</ItemTemplate>
		</f:TemplateField>
		<f:BoundField DataField="Title" Width="100px" HeaderText="ְ��" ColumnID="Titles" Hidden="true"  />
		<f:BoundField DataField="Email" SortField="Email" Width="150px" HeaderText="����" Hidden="true" />
		<f:BoundField DataField="Phone" SortField="Phone" Width="150px" HeaderText="�칫�绰" Hidden="true" />
		<f:BoundField DataField="IdentityCard" SortField="IdentityCard" Width="150px" HeaderText="���֤" Hidden="true" />
		<f:BoundField DataField="Birthday" SortField="Birthday" Width="150px" HeaderText="����" Hidden="true" />
		<f:BoundField DataField="TakeOfficeDt" SortField="TakeOfficeDt" Width="150px" HeaderText="��ְ����" Hidden="true" />
		<f:BoundField DataField="LastLoginDt" SortField="LastLoginDt" Width="150px" HeaderText="����¼����" Hidden="true" />
		<f:BoundField DataField="Remark" ExpandUnusedSpace="true" HeaderText="��ע" Hidden="true" />
		<f:HyperLinkField HeaderText="ͼƬ" Width="150px" DataTextField="CoverImage" DataTextFormatString="<img src='{0}?w=150'/>" DataNavigateUrlFields="CoverImage" HtmlEncode="false" UrlEncode="false" />
	grid �Զ��嵥Ԫ����ʾ
        /// <summary>����Ⱦÿһ�к���ã���ʱ Values ���Ա�����ÿһ����Ⱦ��� HTML Ƭ��</summary>
        protected void Grid1_RowDataBound(object sender, GridRowEventArgs e)
        {
            // e.DataItem  -> System.Data.DataRowView �����Զ�����
            // e.RowIndex -> ��ǰ����ţ��� 0 ��ʼ��
            // e.Values -> ��ǰ��ÿһ����Ⱦ��� HTML Ƭ��
            Type item = e.DataItem as Type;
            var groupIndex = Grid1.FindColumn("Group").ColumnIndex;
            var descIndex = Grid1.FindColumn("Description").ColumnIndex;
            e.Values[groupIndex] = item.GetUIGroup();
            e.Values[descIndex] = item.GetDescription();
        }
	client
		PageContext.RegisterStartupScript(...);
		Alert.ShowInTop("������ Window1 �Ĺر��¼���");
		TriggerBox1.OnClientTriggerClick
	windows
		ActiveWindow.GetWriteBackValueReference(TextBox1.Text, TextBox1.Text + " - �ڶ���ֵ")
		ActiveWindow.GetHideReference()
		ActiveWindow.GetHidePostBackReference()
		Window1.GetSaveStateReference(TriggerBox1.ClientID, HiddenField1.ClientID)
		Window1.GetShowReference("./triggerbox_iframe_iframe.aspx");
		Alert.ShowInTop(HttpUtility.HtmlEncode(editorContent));
		JsHelper.Enquote(content)
	form
		btnReset.OnClientClick = SimpleForm1.GetResetReference();
		tbxUserName.MarkInvalid(String.Format("{0} �Ǳ����֣�������ѡ��", tbxUserName.Text));
	����ص�
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
		    <f:Button runat="server" ID="btnSelect" OnClick="btnSelect_Click" Text="ѡ�񲢹ر�" Icon="ShapesManySelect" />
            UI.SetVisible(this.btnSelect, (this.Mode == PageMode.Select));
	Grid
		���������򣬲������ҳ�ͻᱨ��
			<f:GridPro ID="Grid1" runat="server" SortDirection="DESC" WinHeight="400"
			ShowNumberField="true"  ShowEditField="true" ShowDeleteField="true" ShowViewField="true"
			OnDelete="Grid1_Delete" AutoCreateFields="false" WinWidth="800" 
			NewText="�������" DeleteText="ɾ�����"
			>
	Form����
		<Rows>
            <f:FormRow>
                <Items>
                    <f:Label ID="Label2" Label="������" Text="����ʯ��" CssClass="highlight" runat="server" />
                    <f:Label ID="Label3" Label="�绰" Text="0551-1234567" runat="server" />
                </Items>
            </f:FormRow>
		</Rows>


�����ļ�
    ����ֱ�Ӷ�Response������в����������ڵ���������ʱҪ����AJAX�������磺
``` xml
<f:Button ID="Button1" EnableAjax="false" DisableControlBeforePostBack="false"
    runat="server" Text="����ΪExcel�ļ�" OnClick="Button1_Click">
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
		maxTabMessage: '���ȹر�һЩѡ����������� 10 ������'
		});
	F.alert('��δʵ��');

��ǩ����
	parent.removeActiveTab();
	JsObjectBuilder joBuilder = new JsObjectBuilder();
    joBuilder.AddProperty("id", "grid_newtab_addnew");
    joBuilder.AddProperty("title", "������Ա");
    joBuilder.AddProperty("iframeUrl", ResolveUrl("~/grid/grid_newtab_window.aspx"));
    joBuilder.AddProperty("refreshWhenExist", true);
    joBuilder.AddProperty("iconFont", "plus");
    btnNew.OnClientClick = String.Format("parent.addExampleTab({0});", joBuilder);

�����ͻ��˿ؼ�
	var itemNames = [], itemCount = 0;
	$.each(F.ui, function(name, item) {
		itemNames.push(name);
		itemCount++;
	});
	console.log(itemNames.join(','));
	console.log('�ؼ�������' + itemCount);

��ʱͳ��
	<head id="Head1" runat="server">
		<title></title>
		<link href="../res/css/main.css" rel="stylesheet" type="text/css" />
		<script>
			var _startTime = new Date();
		</script>
	</head>
	��ҳ�����ײ���
	</script>
		F.ready(function () {
			F.notify('��ʱ��' + ((new Date() - _startTime) / 1000).toFixed(2) + ' ��');
		});
	</script>

ͳ��ajax��ʱ
    function onbtnLookUpHisRevClick() {
        _startAjaxTime = new Date();
    }
    F.ajaxReady(function () {
        F.notify('AJAX��ʱ��' + ((new Date() - _startAjaxTime) / 1000).toFixed(2) + ' ��');
    });


Grid

        /// <summary>
        /// ����ͨGrid������ʾֻ�������ã����������Զ������С�
        /// �����������Զ����ɵģ�����ÿ��ҳ�����ʱ�����á�
        /// Ҫ��ʵ�������ķ�ҳ������ɾ�����༭���߼�������GridPro�ؼ���
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
        // �����
        //--------------------------------------------------
        /// <summary>��Ӱ���</summary>
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

            // ���ñ��⡢������ID
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


        /// <summary>�����������У�δ���ã�</summary>
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



        /// <summary>Ԥ�����������ϸ��ڵ㣬��ֹĳЩ�ڵ㡣�����Կ������ø��ƣ����ٽӿ���Ŀ��</summary>
        static List<T> PrepareTree<T>(List<T> source, string title = "--���ڵ�--", long? disableId = null) 
            where T : ITree, new()
        {
            bool addRootNode = title.IsNotEmpty(); // !string.IsNullOrEmpty(title);
            List<T> result = new List<T>();

            // ��Ӹ��ڵ�
            if (addRootNode)
            {
                T root = new T();
                root.Name = title;
                root.ID = -1;
                root.TreeLevel = 0;
                root.Enabled = true;
                result.Add(root);
            }

            // ���������б�
            foreach (T item in source)
            {
                T newT = (T)item.Clone();
                result.Add(newT);

                // �и��ڵ�Ļ������нڵ��TreeLevel��һ
                if (addRootNode)
                    newT.TreeLevel++;
            }

            // ��ǰ�ڵ㼰�ӽڵ㶼����ѡ��
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