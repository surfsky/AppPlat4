
-------------------------------------
UI �Զ���ؼ�
-------------------------------------
PopupBox
	Users
	    <f:PopupBox runat="server" ID="pbUser" Label="�û�" WinTitle="�û�" UrlTemplate="users.aspx?multiply=false&search=true" />
        UI.SetValue(this.pbUser, Common.LoginUser, t => t.ID, t => t.AbbrName);
        item.UserID = UI.GetValue(pbUser);
	Shops
		<f:PopupBox runat="server" ID="pbShop" Label="�̵�" WinTitle="�̵�" UrlTemplate="Shops.aspx?multiply=false&search=true" />
        UI.SetValue(this.pbShop, item.Shop, t => t.ID, t => t.AbbrName);
        item.ShopID = UI.GetValue(pbShop);
		Common.LimitShop(this.pbShop);


FormPro
    <f:FormPro ID="form2" runat="server" ShowCloseButton="true" LabelWidth="80" />
    protected void Page_Load(object sender, EventArgs e)
    {
        this.form2.EntityID = Asp.GetQueryInt("id").Value;
        this.form2.Mode = PageMode.View;
        this.form2.InitForm(typeof(App.DAL.Log));
    }

GridPro
    Grid1.AddImageColumn<Shop>("ͼƬ", 30, 30, "/HttpApi/Common/Thumbnail?u={0}&w=30", t=> t.CoverImage)



EChart
{
  "xAxis": {
    "name": "����",
    "type": "category",
    "boundaryGap": false,
    "data": [
      "1108",
      "1114",
      "1118",
      "1119",
      "1120"
    ],
    "axisLabel": {}
  },
  "yAxis": {
    "name": "����",
    "type": "value",
    "boundaryGap": false,
    "data": [],
    "axisLabel": {}
  },
  "visualMap": [],
  "title": {
    "text": "�����շ���",
    "x": "center"
  },
  "tooltip": {
    "axisPointer": {
      "type": "cross",
      "crossStyle": {
        "color": "#999"
      }
    },
    "trigger": "axis"
  },
  "toolbox": {
    "feature": {
      "dataView": {
        "show": true,
        "readOnly": false
      },
      "restore": {
        "show": true
      },
      "saveAsImage": {
        "show": true
      }
    },
    "show": true
  },
  "legend": {
    "top": 50,
    "data": [
      ""
    ]
  },
  "series": [
    {
      "data": [
        "1",
        "1",
        "1",
        "1",
        "1"
      ],
      "name": "",
      "type": "line",
      "symbol": "circle",
      "showSymbol": true,
      "showAllSymbol": false,
      "symbolSize": 2
    }
  ]
}

