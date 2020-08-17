using System;
using System.Collections.Generic;
using App.Controls;
using App.DAL;
using App.Utils;
using App.Wechats;
using App.Wechats.OP;
using App.Components;

namespace App.Tests
{
    [Auth(Powers.WechatMenuEdit)]
    [UI("微信菜单管理")]
    public partial class WechatMenus : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                UI.SetVisible(btnDeleteMenu, Powers.Admin);
        }

        // 构建默认菜单
        protected void btnDefault_Click(object sender, EventArgs e)
        {
            var menu = BuildMenu();
            UI.SetText(tbMenu, menu.ToJson());
        }

        // 获取菜单
        protected void btnGetMenu_Click(object sender, EventArgs e)
        {
            var menu = WechatOP.GetMenu();
            UI.SetText(tbMenu, menu.ToJson());
        }

        // 设置菜单
        protected void btnSetMenu_Click(object sender, EventArgs e)
        {
            WechatMenu menu = UI.GetText(tbMenu).ParseJson<WechatMenu>();
            var reply = WechatOP.SetMenu(menu);
            UI.ShowAlert(reply.ToJson());
        }

        // 删除菜单
        protected void btnDeleteMenu_Click(object sender, EventArgs e)
        {
            var reply = WechatOP.DeleteMenu();
            UI.ShowAlert(reply.ToJson());
        }

        // 构建小熊默认菜单
        public static WechatMenu BuildMenu()
        {
            var url = "https://mp.weixin.qq.com/s/i8sUwxEcbUYTRHJUOjSPnA";
            var mpAppId = Wechats.WechatConfig.MPAppId;

            WechatMenu menu = new WechatMenu();
            var menu11 = WechatMenuItem.MP("首页", mpAppId, "pages/index/index", url);
            var menu12 = WechatMenuItem.MP("续保", mpAppId, "pages/insurance/extension", url);
            var menu13 = WechatMenuItem.MP("维修下单", mpAppId, "pages/repair/add", url);
            var menu14 = WechatMenuItem.MP("真伪鉴定", mpAppId, "pages/identify/add", url);
            var menu15 = WechatMenuItem.View("保修期查询", "https://checkcoverage.apple.com/cn/zh/");

            var menu21 = WechatMenuItem.View("苹果服务政策", "https://support.apple.com/zh-cn/iphone/repair/service");
            var menu22 = WechatMenuItem.View("iPhoneX", "https://www.apple.com/cn/iphone-x/");
            var menu23 = WechatMenuItem.View("iPhoneXR", "https://www.apple.com/cn/iphone-xr/");
            var menu24 = WechatMenuItem.View("iPhoneXS", "https://www.apple.com/cn/iphone-xs/");
            var menu25 = WechatMenuItem.View("故障快速查询", "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx01da4124f41c7dd5&redirect_uri=http%3a%2f%2fwww.sogservice.com.cn%2fwechat%2fuser%2ftrouble_shooting_index.aspx&response_type=code&scope=snsapi_base&state=test&connect_redirect=1#wechat_redirect");

            var menu31 = WechatMenuItem.Click("商务合作", "bussiness");
            var menu32 = WechatMenuItem.View("一德茶寮", "https://mp.weixin.qq.com/s?__biz=MzI5NTY1NzgzMg==&tempkey=MTAwNl9ldG5UdTEreUoraTlneUNrUjRnTDRXeWRxajBWTGNtMVhyUU1laV81cmM3S3k3T1lpamJKeVRLUlF0LVEtc1E2NUVXY2N2Z0tVdXFvbW9xdFBQbDNOcmhaTHVqSEktd184MVJtbjI1MC1tc3N4ZGY2WG8wRF9aaUxkNTE5NlRHbVdNUUJUdUJaZVltaUlsRkZHZkpUTXFjaFB5ejQzLUwtbFh3UmJRfn4%3D&chksm=6c5109485b26805e08dad40950b944dc7b1a3811add23094d1edc7cd4807dc22faef65396651#rd");
            var menu33 = WechatMenuItem.View("茶叶超市", "https://weidian.com/?userid=1174155720&p=iphone&wfr=BuyercopyURL&share_relation=e23e421709467613__1");

            var menu1 = WechatMenuItem.Root("自助服务", menu11, menu12, menu13, menu14, menu15);
            var menu2 = WechatMenuItem.Root("苹果资讯", menu21, menu22, menu23, menu24, menu25);
            var menu3 = WechatMenuItem.Root("俱乐部", menu31, menu32, menu33);
            menu.button = new List<WechatMenuItem>() { menu1, menu2, menu3 };
            return menu;
        }

    }
}
