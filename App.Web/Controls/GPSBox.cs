using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.ComponentModel;
using App.Utils;
using FineUIPro;

namespace App.Controls
{
    //<f:PopupBox runat="server" ID="pbGPS" Label="经纬度"  UrlTemplate="MapTencent.aspx" WinWidth="800" WinHeight="600" WinTitle="位置"  Trigger2IconUrl="~/Res/Icon/World.png"  />

    public enum GPSType
    {
        Baidu,
        Tencent
    }

    /// <summary>
    /// 地图定位选择器
    /// </summary>
    public class GPSBox : PopupBox
    {
        const string TencentMapUrl = "/Pages/Common/MapTencent.aspx";
        const string BaiduMapUrl   = "/Pages/Common/MapBaidu.aspx";

        /// <summary>是否显示下载列</summary>
        public GPSType Type
        {
            get { return GetState("Type", GPSType.Tencent); }
            set { SetState("Type", value); }
        }

        // 初始化设置
        protected override void OnInit(EventArgs e)
        {
            this.EnableEdit = true;
            this.Trigger2IconUrl = "~/Res/Icon/World.png";
            this.WinTitle = "位置";
            this.WinWidth = 800;
            this.WinHeight = 600;
            this.UrlTemplate = GetMapUrl();
            base.OnInit(e);
        }

        // 获取地图地址
        string GetMapUrl()
        {
            switch (Type)
            {
                case GPSType.Tencent: return TencentMapUrl;
                case GPSType.Baidu:   return BaiduMapUrl;
                default:              return TencentMapUrl;
            }
        }
    }
}