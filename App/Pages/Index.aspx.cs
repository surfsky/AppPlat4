using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using Newtonsoft.Json.Linq;
using FineUIPro;
using System.Linq;
using System.Data.Entity;
using App.DAL;
using App.Utils;
using App.Components;
using App.Controls;

namespace App
{
    [UI("后台主窗口")]
    [Auth(Powers.Backend, AuthLogin=true)]
    public partial class Index : PageBase
    {
        //--------------------------------------------------
        // Init
        //--------------------------------------------------
        // 加载
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitHelp();
                this.treeMenu.Nodes.Clear();
                if (Common.LoginUser != null)
                    BuildTree(Common.LoginUser.Menus, null, treeMenu.Nodes);

                // title
                var version = Global.Version;
                this.logo.ImageUrl = SiteConfig.Instance.Logo;
                this.txtTitle.Text = SiteConfig.Instance.Name;
                this.Title = SiteConfig.Instance.Name;
                this.lblVersion.Text = string.Format("{0}.{1}", version.Major, version.Minor);
                this.txtUser.Text = string.Format("<span class='label'>欢迎 </span><span>{0}</span>", AuthHelper.GetLoginUserName());
                this.lblInfo.Text = string.Format("在线 {0}", Online.GetOnlines());
            }
        }

        //--------------------------------------------------
        // 初始化页面
        //--------------------------------------------------
        // 工具栏上的帮助菜单
        private void InitHelp()
        {
            if (SiteConfig.Instance.HelpList.IsEmpty())
                return;
            JArray ja = JArray.Parse(SiteConfig.Instance.HelpList);
            foreach (JObject jo in ja)
            {
                MenuButton menuItem = new MenuButton();
                menuItem.EnablePostBack = false;
                menuItem.Text = jo.Value<string>("Text");
                menuItem.Icon = IconHelper.String2Icon(jo.Value<string>("Icon"), true);
                menuItem.OnClientClick = String.Format("addMainTab('{0}','{1}','{2}')", jo.Value<string>("ID"), ResolveUrl(jo.Value<string>("URL")), jo.Value<string>("Text"));
                btnHelp.Menu.Items.Add(menuItem);
            }
        }


        /// <summary>递归生成菜单树</summary>
        void BuildTree(List<DAL.Menu> menus, DAL.Menu parentMenu, FineUIPro.TreeNodeCollection nodes)
        {
            foreach (var menu in menus.Where(m => m.Parent == parentMenu).Where(t => t.Visible==true))
            {
                FineUIPro.TreeNode node = new FineUIPro.TreeNode();
                nodes.Add(node);
                node.Text = menu.Name;
                node.IconUrl = menu.ImageUrl;
                node.Expanded = (menu.IsOpen==true) && !menu.IsTreeLeaf;
                if (menu.SafeUrl.IsNotEmpty())
                    node.NavigateUrl = ResolveUrl(menu.SafeUrl);

                if (menu.IsTreeLeaf)
                    node.Leaf = true;
                else
                    BuildTree(menus, menu, node.Nodes);
            }
        }

    }
}
