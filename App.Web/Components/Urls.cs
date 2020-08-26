using App.Controls;
using App.Utils;
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
        // Folder
        public static string FolderBase         => "/Pages/Base";
        public static string FolderArticle      => "/Pages/Articles";
        public static string FolderCommon       => "/Pages/Common";
        public static string FolderConfigs      => "/Pages/Configs";
        public static string FolderMaintains    => "/Pages/Maintains";
        public static string FolderDevs         => "/Pages/Devs";

        // Root
        public static string Login              => $"/Login.html";
        public static string Logout             => $"/Logout.ashx";
        public static string Backend            => $"/Pages/Index.aspx";

        // Base
        public static string Areas              => $"{FolderBase}/Areas.aspx";
        public static string Depts              => $"{FolderBase}/Depts.aspx";
        public static string Titles             => $"{FolderBase}/Titles.aspx";

        // Article
        public static string Articles           => $"{FolderArticle}/Articles.aspx";
        public static string ArticleDirs        => $"{FolderArticle}/ArticleDirs.aspx";
        public static string ArticleConfig      => $"{FolderArticle}/ArticleConfigForm.aspx";

        // configs
        public static string ConfigRoles        => $"{FolderConfigs}/Roles.aspx";
        //public static string ConfigAlis         => $"{FolderConfigs}/ConfigAlis.aspx";
        //public static string ConfigDings        => $"{FolderConfigs}/ConfigDings.aspx";
        //public static string ConfigSites        => $"{FolderConfigs}/ConfigSites.aspx";
        //public static string ConfigWechats      => $"{FolderConfigs}/ConfigWechats.aspx";
        //public static string ConfigMenus        => $"{FolderConfigs}/Menus.aspx";
        //public static string ConfigSequences    => $"{FolderConfigs}/Sequences.aspx";
        //public static string ConfigThemes       => $"{FolderConfigs}/Themes.aspx";

        // maintains
        public static string Dashboard          => $"{FolderMaintains}/Dashboard.aspx";
        public static string Feedbacks          => $"{FolderMaintains}/Feedbacks.aspx";
        public static string IPFilters          => $"{FolderMaintains}/IPFilters.aspx";
        public static string Onlines            => $"{FolderMaintains}/Onlines.aspx";
        public static string Logs               => $"{FolderMaintains}/Logs.aspx";


        //-----------------------------------------
        // URL
        //-----------------------------------------
        /// <summary>获取二维码图片地址</summary>
        public static string GetUsersUrl(Role role, Powers? power, bool showSearch=false)
        {
            return string.Format("/Pages/Base/Users.aspx?role={0}&power={1}&search={2}", role?.ID, (long?)power, showSearch);
        }

        /// <summary>获取二维码图片地址</summary>
        public static string GetQrCodeUrl(string text)
        {
            text = HttpUtility.UrlEncode(text);
            return string.Format("/HttpApi/Common/QrCode?text={0}", text);
        }

        /// <summary>获取缩略图URL</summary>
        public static string GetThrumbnailUrl(string imageUrl, int w)
        {
            return string.Format("{0}?w={1}", imageUrl, w);
        }

        /// <summary>获取加密下载URL（直接指定下载地址）</summary>
        /// <param name="name">文件名</param>
        /// <param name="watermark">水印文字，若为空，则尝试自动根据用户名创建</param>
        public static string GetDownUrl(string url, string name = "")
        {
            var watermark = Downloader.GetWatermarkText();
            return string.Format("/down.ashx?url={0}&name={1}&watermark={2}", url.UrlEncode(), name.UrlEncode(), watermark.UrlEncode()).ToSignUrl();
        }

        /// <summary>获取加密下载URL</summary>
        /// <param name="watermark">水印文字，若为空，则尝试自动根据用户名创建</param>
        public static string GetDownUrl(long resId)
        {
            var watermark = Downloader.GetWatermarkText();
            return string.Format("/down.ashx?rid={0}&watermark={1}", resId, watermark.UrlEncode()).ToSignUrl();
        }

        /// <summary>获取资源列表页面</summary>
        public static string GetResesUrl(PageMode mode, string key, string cate, bool onlyImage)
        {
            return String.Format("/Pages/Common/Reses.aspx?md={0}&key={1}&cate={2}&imageOnly={3}", mode, key, "Articles", onlyImage).ToSignUrl();
        }

        /// <summary>获取文章列表页面</summary>
        public static string GetArticlesUrl(PageMode mode, long? replyId, bool search)
        {
            return String.Format("/Pages/Articles/Articles.aspx?md={0}&replyId={1}&search={2}", mode, replyId, search).ToSignUrl();
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
        /// <summary>获取图片附件列表URL</summary>
        /// <param name="key">资源键，如 User-12345</param>
        /// <param name="cate">图片上传时的存储路径，如 Users</param>
        public static string GetImagesUrl(string key, string cate, PageMode? mode = null, int? imageWidth = null)
        {
            var url = string.Format("/Pages/Common/Reses.aspx?key={0}&cate={1}&imageOnly=true", key, cate);
            if (mode != null) url = url.AddQueryString($"md={mode}");
            if (imageWidth != null) url = url.AddQueryString($"imageWidth={imageWidth}");
            return url;
        }

        /// <summary>获取文件附件列表URL</summary>
        /// <param name="key">资源键，如 User-12345</param>
        /// <param name="cate">图片上传时的存储路径，如 Users</param>
        public static string GetFilesUrl(string key, string cate, PageMode? mode = null)
        {
            var url = string.Format("/Pages/Common/Reses.aspx?key={0}&cate={1}&imageOnly=false", key, cate);
            if (mode != null) url = url.AddQueryString($"md={mode}");
            return url;
        }


        /// <summary>获取资源管理器页面地址</summary>
        /// <param name="root">根目录</param>
        /// <param name="filter">文件过滤器，格式如：.jpg .png</param>
        /// <param name="showDownload">是否显示下载列</param>
        /// <param name="showInfo">是否显示文件信息</param>
        /// <param name="folder">当前目录</param>
        /// <param name="mode">显示模式</param>
        public static string GetExplorerUrl(string root, string filter, bool showDownload, bool showInfo, PageMode? mode, string folder, bool sign)
        {
            var url = string.Format("/Pages/Common/Explorer.aspx?root={0}&filter={1}&showDownload={2}&showInfo={3}&md={4}&folder={5}",
                    root.UrlEncode(),
                    filter.UrlEncode(),
                    showDownload,
                    showInfo,
                    mode,
                    folder.UrlEncode()
                    );
            return sign ? url.ToSignUrl() : url;
        }

        /// <summary>获取操作历史页面地址</summary>
        public static string GetHistoriesUrl(string uniId, long flowId)
        {
            return string.Format("/Pages/Workflows/Histories.aspx?key={0}&md=view&search=false&flowType={1}", uniId, flowId);
        }

        /// <summary>获取工作流配置地址</summary>
        public static string GetWorkflowsUrl(WFType flowType)
        {
            return string.Format("/Pages/Workflows/Workflows.aspx?md=view&type={0}&search=false", flowType);
        }



        //------------------------------------------------------
        // 自动化UI相关页面
        //------------------------------------------------------
        /// <summary>获取数据列表URL(datas.aspx)</summary>
        /// <param name="query">查询字符串（如a=x&b=x）</param>
        public static string GetDatasUrl(Type type, string query = "", PageMode? mode = null, AuthAttribute auth = null)
        {
            if (type == null) return "";
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