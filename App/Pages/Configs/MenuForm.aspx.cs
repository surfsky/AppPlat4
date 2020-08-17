using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
using App.DAL;
using App.Controls;
using App.Utils;
using App.Components;

namespace App.Admins
{
    [UI("菜单")]
    [Auth(Powers.Menu)]
    public partial class MenuForm : FormPage<DAL.Menu>
    {
        //----------------------------------------------------
        // Init
        //----------------------------------------------------
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
            {
                ShowForm();
                this.pbParam.Window.IsModal = false;
            }
        }

        // 新建数据
        public override void NewData()
        {
            UI.SetText(tbName, "");
            UI.SetText(tbSeq, "0");
            UI.SetText(tbRemark, "");
            UI.SetText(fbUrl, "");
            chkOpen.Checked = false;

            // 图标列表, 上级菜单，权限下拉框
            var parentId = Asp.GetQueryLong("parentId");
            BindMenus(parentId, null);
            BindPowers(null);
            BindIcons("/res/icon/page.png");
        }

        // 展示数据
        public override void ShowData(DAL.Menu item)
        {
            UI.SetText(tbName, item.Name);
            UI.SetText(tbSeq, item.Seq.ToString());
            UI.SetText(tbRemark, item.Remark);
            UI.SetText(fbUrl, item.NavigateUrl);
            UI.SetText(fbIcon, item.ImageUrl);
            UI.SetValue(chkOpen, item.IsOpen);
            UI.SetValue(chkVisible, item.Visible);

            // 图标列表, 上级菜单，权限下拉框
            BindMenus(item.ParentID, item.ID);
            BindPowers((int?)item.ViewPower);
            BindIcons(item.ImageUrl);
        }

        // 收集数据
        public override void CollectData(ref DAL.Menu item)
        {
            item.Name = UI.GetText(tbName);
            item.Seq = UI.GetInt(tbSeq);
            item.NavigateUrl = UI.GetText(fbUrl);
            item.ImageUrl = UI.GetText(fbIcon);
            item.Remark = UI.GetText(tbRemark);
            item.IsOpen = UI.GetBool(chkOpen);
            item.Visible = UI.GetBool(chkVisible);
            item.ParentID = UI.GetLong(ddlParentMenu);

            // 自动设置菜单的 ViewPower
            if (UI.GetEnum<Powers>(ddlViewPower) == null) 
            {
                var type = Asp.GetHandler(item.NavigateUrl);
                if (type != null)
                {
                    var auth = type.GetAttribute<AuthAttribute>();
                    UI.SetValue(ddlViewPower, auth?.ViewPower);
                }
            }
            item.ViewPower = UI.GetEnum<Powers>(ddlViewPower);
        }

        //
        public override void SaveData(DAL.Menu item)
        {
            item.NavigateUrl = item.NavigateUrl.AddQueryString(UI.GetText(this.pbParam));
            item.Save();
            Common.RefreshLoginUser();
        }

        //----------------------------------------------------
        // 辅助方法
        //----------------------------------------------------
        List<string> _icons = new List<string>()
        {
            "/res/icon/page.png",
            "/res/icon/folder.png",
            "/res/icon/tag_yellow.png",
            "/res/icon/tag_red.png",
            "/res/icon/tag_purple.png",
            "/res/icon/tag_orange.png",
            "/res/icon/tag_blue.png",
            "/res/icon/tag_green.png",
        };

        // 菜单图标列表
        public void BindIcons(string selectImageUrl="")
        {
            FineUIPro.RadioButtonList iconList = this.iconList;
            iconList.Items.Clear();
            foreach (string icon in _icons)
            {
                string text = String.Format("<img style=\"vertical-align:bottom;\" src=\"{0}\" />&nbsp;", ResolveUrl(icon));
                iconList.Items.Add(new RadioItem(text, icon));
            }
            if (selectImageUrl.IsNotEmpty())
            {
                iconList.SelectedValue = selectImageUrl;
                UI.SetText(fbIcon, selectImageUrl);
            }
        }

        // 绑定到下拉列表（启用模拟树功能和不可选择项功能）
        private void BindMenus(long? selectedId = null, long? disableId = null)
        {
            UI.BindTree(ddlParentMenu, DAL.Menu.All, t => t.ID, t => t.Name, "--根目录--", selectedId, disableId);
        }

        // 显示权限下拉框(根据当前用户拥有的权限来设置)
        private void BindPowers(long? selectedId = null)
        {
            var items = new List<EnumInfo>();
            foreach (var item in Common.LoginUser.Powers)
                items.Add(item.GetEnumInfo());

            items = items.AsQueryable().Sort(t => t.FullName).ToList();
            UI.Bind(ddlViewPower, items, t => t.ID, t => t.FullName, "--请选择权限--", selectedId);

        }

        // 显示参数配置窗口
        protected void pbParam_Trigger2Click(object sender, EventArgs e)
        {
            var url = fbUrl.Text.TrimQuery();
            var paramUrl = url + "$";
            UI.ShowWindow(pbParam.Window, paramUrl, "参数设置", 1000, 700);
        }
    }
}
