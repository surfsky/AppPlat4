using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FineUIPro;
using App.Components;  // Urls, AuthAttribute
using App.Utils;
using App.Entities;
using App.DAL;

namespace App.Controls
{
    /// <summary>
    /// 渲染器基类
    /// </summary>
    public static class UIRender
    {
        /// <summary>给 URL 加上（或设置）PageMode 参数</summary>
        public static string AddModeQuery(this string url, PageMode mode)
        {
            return url.AddQueryString(string.Format("md={0}", mode));
        }
        /// <summary>给 URL 加上（或设置）Id 参数</summary>
        public static string AddIdQuery(this string url, string idName="id")
        {
            var q = $"{idName}={{0}}";
            return url.AddQueryString(q);
        }

        /// <summary>解析url参数（如果有q参数则先尝试解析q参数，否则的话解析完整querystring）</summary>
        public static FreeDictionary<string, string> ParseQueryData()
        {
            // 解析所有QueryString
            //var dict = new FreeDictionary<string, string>();
            //var q = Asp.GetQueryString("q");
            //if (q.IsNotEmpty())
            //    dict = q.ParseDict();
            //else
            var dict = Asp.QueryString.ParseDict();

            // 删除掉系统内置参数
            dict.Remove("md");    // page mode
            dict.Remove("ns");    // nonce string
            dict.Remove("ts");    // time stamp
            dict.Remove("sn");    // sign
            dict.Remove("pow");   // power
            dict.Remove("pv");    // power view
            dict.Remove("pn");    // power new
            dict.Remove("pe");    // power edit
            dict.Remove("pd");    // power delete
            dict.Remove("tp");    // type
            if (dict.Count == 0)
                return dict;

            // 仅保留页面所需参数
            var type = Asp.GetHandler(Asp.Url);
            if (type != null)
            {
                var ps = type?.GetAttributes<ParamAttribute>();
                var dict2 = new FreeDictionary<string, string>();
                foreach (var p in ps)
                {
                    var v = dict[p.Name];
                    if (v != null)
                        dict2.Add(p.Name, v);
                }
                return dict2;
            }
            return dict;
        }


        /// <summary>创建UI设置按钮（点击后跳到UI设置窗口）</summary>
        public static Button CreateUISettingButton(string typeName, Window win, XUIType type, long? uiId)
        {
            var entityType = Reflector.GetType(typeName);
            var btnSetting = new Button() { Icon = FineUIPro.Icon.Cog, ToolTip = "UI 配置", EnablePostBack = false };

            // 菜单项1
            var mi1 = new MenuButton() { Text = "UI 设置", Icon = FineUIPro.Icon.PageWhiteGear };
            mi1.Click += (sender, arg) =>
            {
                var url = Urls.GetUISettingsUrl(entityType);
                UI.ShowWindow(win, url, "UI 配置");
            };
            btnSetting.Menu.Items.Add(mi1);

            // 菜单项2
            var mi2 = new MenuButton() { Text = "本页地址", Icon = FineUIPro.Icon.Page };
            mi2.Click += (sender, arg) =>
            {
                var url = new Url(Asp.Request.RawUrl.ResolveUrl());
                url.Remove("pv", "pn", "pe", "pd", "ns", "ts", "sn");
                url["uiId"] = (uiId == null) ? GetSelectedMenuId(btnSetting.Menu) : uiId.ToString();
                var auth = entityType.GetAttribute<AuthAttribute>();
                if (auth != null)
                {
                    url["pv"] = auth.ViewPower?.ToString();
                    url["pn"] = auth.NewPower?.ToString();
                    url["pe"] = auth.EditPower?.ToString();
                    url["pd"] = auth.DeletePower?.ToString();
                }
                UI.ShowAlert(url.ToString());
            };
            btnSetting.Menu.Items.Add(mi2);

            // 菜单项3
            var mi3 = new MenuButton() { Text = "刷新", Icon = FineUIPro.Icon.PageRefresh };
            mi3.Click += (sender, arg) =>
            {
                var url = Asp.Request.RawUrl.ResolveUrl();
                Asp.Response.Redirect(url);
            };
            btnSetting.Menu.Items.Add(mi3);
            btnSetting.Menu.Items.Add(new MenuSeparator());

            // 可选UI设置
            CreateMenuCheckBoxes(btnSetting, type, entityType, uiId);
            return btnSetting;
        }

        /// <summary>创建菜单单选框列表</summary>
        private static void CreateMenuCheckBoxes(Button btn, XUIType type, Type entityType, long? uiId)
        {
            var checks = new List<MenuCheckBox>();
            MenuCheckBox chk;

            // from xui
            var settings = XUI.Search(type, entityType.FullName).ToList();
            for (int i = 0; i < settings.Count; i++)
            {
                var setting = settings[i];
                chk = new MenuCheckBox() { ID = setting.ID.ToString(), Text = setting.Name, GroupName = "UISettings", AutoPostBack = true };
                chk.Checked = (uiId == setting.ID);
                chk.CheckedChanged += menu_CheckedChanged;
                checks.Add(chk);
            }

            // default
            chk = new MenuCheckBox() { ID = "0", Text = "Default", GroupName = "UISettings", AutoPostBack = true };
            chk.Checked = (uiId == 0);
            chk.CheckedChanged += menu_CheckedChanged;
            checks.Add(chk);

            // 若无uiId参数，默认选中第一个选项
            if (uiId == null)
                checks[0].Checked = true;

            // 添加到菜单
            foreach (var ck in checks)
                btn.Menu.Items.Add(ck);
        }

        // 菜单选择项变更刷新页面
        private static void menu_CheckedChanged(object sender, CheckedEventArgs e)
        {
            var cb = sender as MenuCheckBox;
            if (cb.Checked)
            {
                var url = new Url(Asp.Request.RawUrl);
                url["uiId"] = cb.ID;
                Asp.Response.Redirect(url.ToString());
            }
        }

        /// <summary>清除所有选中菜单</summary>
        public static string SetSelectedMenu(FineUIPro.Menu menu, string id)
        {
            foreach (var menuItem in menu.Items)
            {
                if (menuItem is MenuCheckBox chk)
                {
                    if (chk.ID == id)
                        chk.Checked = true;
                    else
                        chk.Checked = false;
                }
            }
            return "";
        }


        /// <summary>获取选中的菜单ID（返回 UIID 或空字符串）</summary>
        public static string GetSelectedMenuId(FineUIPro.Menu menu)
        {
            foreach (var menuItem in menu.Items)
            {
                if (menuItem is MenuCheckBox chk)
                {
                    if (chk.Checked)
                        return chk.ID;
                }
            }
            return "";
        }



    }
}