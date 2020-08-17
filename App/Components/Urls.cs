using App.Controls;
using App.Core;
using App.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Components
{
    /// <summary>本系统提供的基础服务页面（类似装配单）</summary>
    public static class Urls
    {
        public static string FolderBase         => "/Pages/Base";
        public static string FolderCommon       => "/Pages/Common";
        public static string FolderConfigs      => "/Pages/Configs";
        public static string FolderMaintains    => "/Pages/Maintains";
        public static string FolderDevs         => "/Pages/Devs";

        // Base
        public static string BaseAreas          => $"{FolderBase}/Areas.aspx";
        public static string BaseArticles       => $"{FolderBase}/Articles.aspx";
        public static string BaseDepts          => $"{FolderBase}/Depts.aspx";
        public static string BaseTitles         => $"{FolderBase}/Titles.aspx";
        public static string BaseUsers          => $"{FolderBase}/Users.aspx";
        public static string BaseUserDepts      => $"{FolderBase}/UserDepts.aspx";

        // configs
        public static string ConfigAlis         => $"{FolderConfigs}/ConfigAlis.aspx";
        public static string ConfigArticles     => $"{FolderConfigs}/ConfigArticles.aspx";
        public static string ConfigDings        => $"{FolderConfigs}/ConfigDings.aspx";
        public static string ConfigSites        => $"{FolderConfigs}/ConfigSites.aspx";
        public static string ConfigWechats      => $"{FolderConfigs}/ConfigWechats.aspx";
        public static string ConfigMenus        => $"{FolderConfigs}/Menus.aspx";
        public static string ConfigRoles        => $"{FolderConfigs}/Roles.aspx";
        public static string ConfigSequences    => $"{FolderConfigs}/Sequences.aspx";
        public static string ConfigThemes       => $"{FolderConfigs}/Themes.aspx";
        public static string ConfigWorkflows    => $"{FolderConfigs}/Workflows.aspx";

        // devs
        public static string DevAPI             => $"{FolderDevs}/API.aspx";
        public static string DevData            => $"{FolderDevs}/Data.aspx";

        // maintains
        public static string MaintainDashboard  => $"{FolderMaintains}/Dashboard.aspx";
        public static string MaintainFeedbacks  => $"{FolderMaintains}/Feedbacks.aspx";
        public static string MaintainIPFilters  => $"{FolderMaintains}/IPFilters.aspx";
        public static string MaintainOnlines    => $"{FolderMaintains}/Onlines.aspx";
        public static string MaintainLogs       => $"{FolderMaintains}/Logs.aspx";


        //-----------------------------------------
        // URL
        //-----------------------------------------
        /// <summary>获取二维码图片地址</summary>
        public static string GetQrCodeUrl(string text)
        {
            text = HttpUtility.UrlEncode(text);
            return string.Format("/HttpApi/Common/QrCode?text={0}", text);
        }

        /// <summary>获取邀请地址</summary>
        public static string GetInviteUrl(long? shopId, long? userId, string userMobile)
        {
            var inviteCode = Invite.GetInviteCode(shopId, userId, userMobile);
            var url = string.Format("~/Pages/Malls/Regist.aspx?inviteCode={0}", inviteCode.UrlEncode());
            return Asp.ResolveFullUrl(url);
        }

        /// <summary>获取缩略图URL</summary>
        public static string GetThrumbnailUrl(string imageUrl, int w)
        {
            return string.Format("{0}?w={1}", imageUrl, w);
        }

        /// <summary>获取下载URL</summary>
        public static string GetDownUrl(string url, string name)
        {
            return string.Format("/down.ashx?url={0}&name={1}", url.UrlEncode(), name.UrlEncode()).ToSignUrl();
        }

        /// <summary>获取下载URL</summary>
        public static string GetDownUrl(long resId, string name)
        {
            return string.Format("/down.ashx?rid={0}&name={1}", resId, name.UrlEncode()).ToSignUrl();
        }

        /// <summary>获取资源列表页面</summary>
        public static string GetResesUrl(PageMode mode, string key, string cate, bool onlyImage)
        {
            return String.Format("/Pages/Base/Reses.aspx?md={0}&key={1}&cate={2}&imageOnly={3}", mode, key, "Articles", onlyImage).ToSignUrl();
        }

        /// <summary>获取文章列表页面</summary>
        public static string GetArticlesUrl(PageMode mode, long? replyId, bool search)
        {
            return String.Format("/Pages/Base/Articles.aspx?md={0}&replyId={1}&search={2}", mode, replyId, search).ToSignUrl();
        }



        /// <summary>获取大文件上传页面地址</summary>
        /// <param name="folder">保存目录，如 Videos</param>
        /// <param name="title">文件名，如 About.mp4。若为空，则随机生成。</param>
        /// <param name="filter">过滤器，如 video/*</param>
        public static string GetHugeUpUrl(string folder, string title, string key, string filter = "*/*")
        {
            return string.Format("/Pages/Common/HugeUp.aspx?folder={0}&title={1}&filter={2}&key={3}",
                folder.UrlEncode(),
                title.UrlEncode(),
                filter.UrlEncode(),
                key.UrlEncode()
                ).ToSignUrl();
        }


        /*
        [Param("key", "资源键值")]
        [Param("cate", "附件目录")]
        [Param("imageOnly", "只允许上传图片", false)]
        [Param("imageWidth", "图片宽度限制", false)]
        [Param("imageHeight", "图片高度限制", false)]
        [Param("selectName", "选择器参数", false)]
        [Param("md", "模式", false)]
        */
        /// <summary>获取图片列表URL(reses.aspx)</summary>
        /// <param name="key">资源键，如 User-12345</param>
        /// <param name="cate">图片上传时的存储路径，如 Users</param>
        public static string GetImagesUrl(string key, string cate, PageMode? mode = null, int? imageWidth = null)
        {
            var url = string.Format("/Pages/Base/Reses.aspx?key={0}&cate={1}&imageOnly=true", key, cate);
            if (mode != null) url = url.AddQueryString($"md={mode}");
            if (imageWidth != null) url = url.AddQueryString($"imageWidth={imageWidth}");
            return url;
        }

        /// <summary>获取图片列表URL(reses.aspx)</summary>
        /// <param name="key">资源键，如 User-12345</param>
        /// <param name="cate">图片上传时的存储路径，如 Users</param>
        public static string GetFilesUrl(string key, string cate, PageMode? mode = null)
        {
            var url = string.Format("/Pages/Base/Reses.aspx?key={0}&cate={1}&imageOnly=false", key, cate);
            if (mode != null) url = url.AddQueryString($"md={mode}");
            return url;
        }

        //------------------------------------------------------
        // 自动化UI相关页面
        //------------------------------------------------------
        /// <summary>获取数据列表URL(datas.aspx)</summary>
        /// <param name="query">查询字符串（如a=x&b=x）</param>
        public static string GetDatasUrl(Type type, string query = "", PageMode? mode = null, AuthAttribute auth = null)
        {
            var url = string.Format("/Pages/Devs/Datas.aspx?tp={0}", type.FullName);
            //if (query.IsNotEmpty()) url = url.AddQueryString(string.Format("q={0}", query.UrlEncode()));
            if (query.IsNotEmpty()) url = url.AddQueryString(query);
            if (mode != null) url = url.AddQueryString($"md={mode}");
            if (auth != null) url = url.AddQueryString(string.Format("pv={0}&pn={1}&pe={2}&pd={3}", auth.ViewPower, auth.NewPower, auth.EditPower, auth.DeletePower));
            return url.ToSignUrl();
        }

        /// <summary>获取数据表单URL</summary>
        public static string GetDataFormUrl(Type type, long? id = null, PageMode? mode = null, AuthAttribute auth = null)
        {
            var url = string.Format("/Pages/Devs/DataForm.aspx?tp={0}", type.FullName);
            if (id != null) url = url.AddQueryString($"id={id}");
            if (mode != null) url = url.AddQueryString($"md={mode}");
            if (auth != null) url = url.AddQueryString(string.Format("pv={0}&pn={1}&pe={2}&pd={3}", auth.ViewPower, auth.NewPower, auth.EditPower, auth.DeletePower));
            return url.ToSignUrl();
        }

        /// <summary>获取数据模型URL</summary>
        public static string GetDataModelUrl(Type type, AuthAttribute auth = null)
        {
            var url = string.Format("/Pages/Devs/DataModel.ashx?tp={0}", type.FullName);
            if (auth != null) url = url.AddQueryString(string.Format("pv={0}&pn={1}&pe={2}&pd={3}", auth.ViewPower, auth.NewPower, auth.EditPower, auth.DeletePower));
            return url.ToSignUrl();
        }

        /// <summary>获取UI设置URL</summary>
        public static string GetUISettingsUrl(Type type)
        {
            return $"/Pages/Devs/UIs.aspx?tp={type.FullName}";
        }

        /// <summary>获取UI设置URL</summary>
        public static string GetUISettingUrl(long settingId)
        {
            return $"/Pages/Devs/UIForm.aspx?id={settingId}";
        }

        /// <summary>获取页面信息URL</summary>
        /// <param name="url">url 必须无任何查询参数，以.aspx 或 .ashx 结尾</param>
        public static string GetPageInfoUrl(string url)
        {
            return string.Format("{0}$", url);
        }


    }
}