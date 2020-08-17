using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
//using EntityFramework.Extensions;
using System.Text.RegularExpressions;
using FineUIPro;
using App.DAL;
using App.Utils;
using App.Components;
using App.Controls;

namespace App.Pages
{
    [Auth(AuthLogin=true, AuthSign = true)]
    [UI("文件选择窗口", Remark="管理员可附加检阅页面安全情况")]
    [Param("root",   "根目录（如/Pages/）", false)]
    [Param("folder", "当前目录（如/Pages/Malls/）", false)]
    [Param("filter", "文件过滤器（如 *.aspx,*.html）", false)]
    [Param("value", "当前选择文件（如/a/b.aspx）", false)]
    [Param("showInfo", "是否显示文件信息", false)]
    [Param("showDownload", "是否允许文件下载", false)]
    [Param("showUpload", "是否允许上传文件", false)]
    public partial class Explorer : PageBase
    {
        // 参数
        string _root = Asp.GetQueryString("root");
        string _folder = Asp.GetQueryString("folder");
        string _filter = Asp.GetQueryString("filter")?.ToLower();
        bool _showDownload = Asp.GetQueryBool("showDownload") ?? true;
        bool _showInfo = Asp.GetQueryBool("showInfo") ?? true;
        bool _showUpload = Asp.GetQueryBool("showUpload") ?? true;
        string _uploadFolder = "/Files";  // 仅此目录允许上传文件（图像文件）

        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            var url = Request.RawUrl;
            _root = Asp.GetQueryString("root");
            _folder = Asp.GetQueryString("folder");
            if (_root.IsEmpty())
                _root = "/";
            if (_folder.IsEmpty())
                _folder = _root;

            // 普通列
            this.Grid1.SetPowers(false, false, false, false)
                .AddColumn<WebFile>(t => t.Type, 50, " ")
                .AddThrumbnailColumn<WebFile>(t => t.Url, 40, "图片")
                .AddColumn<WebFile>(t => t.Name, 350, "名称")
                .AddColumn<WebFile>(t => t.Extension, 120, "扩展名")
                .AddColumn<WebFile>(t => t.Size, 100, "大小")
                //.AddButtonColumn(FineUIPro.Icon.PackageDown, (values)=> {Asp.WriteFile(values[2].MapPath()), "Download")  // 没有任何效果，估计和FineUI的机制有关
                ;
            // 管理员列
            if (Common.CheckPower(Powers.Admin))
                this.Grid1
                    .AddColumn<WebFile>(t => t.Folder, 200, "目录")
                    .AddLinkColumn<WebFile>(t => t.Url, t => t.Name, "/Down.ashx?url={0}", 40, "下载")
                    .AddWindowColumn<WebFile>(t => t.Url, null, "{0}", 40, "信息", "", FineUIPro.Icon.Information)
                    .AddCheckColumn<WebFile>(t => t.Auth.IsSafe, 80, "是否安全")
                    .AddCheckColumn<WebFile>(t => t.Auth.AuthLogin, 70, "登陆校验")
                    .AddCheckColumn<WebFile>(t => t.Auth.AuthSign, 70, "签名校验")
                    .AddCheckColumn<WebFile>(t => t.Auth.Ignore, 70, "忽略校验")
                    .AddColumn<WebFile>(t => t.Auth.ViewPower, 140, "访问权限")
                    .AddColumn<WebFile>(t => t.Description, 140, "说明")
                    ;
            Grid1.InitGrid<WebFile>(BindGrid, this.Panel1, t => t.Name);
            this.Grid1.DataKeyNames = new string[]{ "ID", "Name", "Url", "Type" };
            this.Grid1.RowDataBound += Grid1_RowDataBound;
            this.Grid1.Select += Grid1_Select;
            this.Grid1.OnSetValue += Grid1_OnSetValue;
            if (!IsPostBack)
            {
                bool isAdmin = Common.LoginUser.HasPower(Powers.Admin);
                UI.SetGridColumnVisible(Grid1, "Link-Url", _showDownload);
                UI.SetGridColumnVisible(Grid1, "Win-Url", _showInfo);
                UI.SetVisible(isAdmin, this.chkUnsafe);
                this.uploader.Hidden = !_folder.StartsWith(_uploadFolder);  // 仅Files目录允许用户上传文件
                this.Grid1.SetSortPage<WebFile>(SiteConfig.Instance.PageSize, t => t.Type, true);
                BindGrid();
            }
        }

        // 设置当前选择的值
        private void Grid1_OnSetValue(string value)
        {
            if (value.IsNotEmpty())
            {
                var name = new Url(value).FileName;
                GridHelper.SetSelectedKeys(this.Grid1, name.AsList(), 1);
            }
        }

        // 绑定网格
        private void BindGrid()
        {
            List<WebFile> files;
            bool onlyUnsafe = UI.GetBool(this.chkUnsafe);
            if (onlyUnsafe)
            {
                files = WebFile.GetUnsafeFiles();
            }
            else
            {
                // 根据选中的文件寻找匹配的目录
                var value = Asp.GetQueryString("value");
                if (value.IsNotEmpty())
                {
                    var folder = new Url(value).FileFolder;
                    if (folder.StartsWith(_root))
                        _folder = folder;
                }
                this.lblFolder.Text = _folder;

                // 显示指定目录数据
                var exts = _filter.SplitString();
                files = WebFile.GetFiles(_root, _folder, exts);
            }

            IQueryable<WebFile> q = files.AsQueryable();
            var name = UI.GetText(tbName);
            if (name.IsNotEmpty())
                q = q.Where(t => t.Name.Contains(name, true));
            Grid1.Bind(q);
        }

        // 图片上传
        protected void uploader_FileSelected(object sender, EventArgs e)
        {
            string imageUrl = UI.UploadFile(uploader, _folder, SiteConfig.Instance.SizeBigImage);
            UI.ShowHud($"图片已经上传：{imageUrl}");
            this.BindGrid();
        }

        // 查找
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }


        // 选择事件
        private void Grid1_Select(object sender, EventArgs e)
        {
            // 过滤文件夹
            var types = this.Grid1.GetSelectedValues<WebFileType>("Type");
            if (types.Contains(WebFileType.Folder))
                return;

            // 返回文件名称和地址
            var txt = this.Grid1.GetSelectedValues<string>("Name").ToSeparatedString();
            var urls = this.Grid1.GetSelectedValues<string>("Url").ToSeparatedString();
            var script = string.Format("{0}{1}",
                ActiveWindow.GetWriteBackValueReference(urls, urls),
                ActiveWindow.GetHideReference()
                );
            PageContext.RegisterStartupScript(script);
        }


        // 行数据绑定事件（完全定制单元格内容）
        private void Grid1_RowDataBound(object sender, GridRowEventArgs e)
        {
            var data = e.DataItem as WebFile;

            // 图标列、名称列、下载文件列、下载按钮列（windows测试无效）、页面信息列
            UI.SetGridCellText(Grid1, "Type", GetIconText(data), e);
            UI.SetGridCellText(Grid1, "Name", GetNameText(data), e);
            UI.SetGridCellText(Grid1, "Link-Url", GetDownText(data), e);

            if (data.Type == WebFileType.Folder)
            {
                UI.SetGridCellText(Grid1, "Button-Download", "", e);
                UI.SetGridCellText(Grid1, "Win-Url", "", e);
                // 如果是目录的话，隐藏所有checkbox列（权限列。不知道为什么，没有效果）
                /*
                foreach (var col in Grid1.Columns)
                {
                    var field = col as FineUIPro.CheckBoxField;
                    if (field != null)
                        e.Values[field.ColumnIndex] = "";
                }
                */
            }
            else
            {
                UI.SetGridWinCellUrl(Grid1, "Win-Url", Urls.GetPageInfoUrl(data.Url), e);
            }
        }


        //
        // 获取各列的文本
        //
        private static string GetIconText(WebFile data)
        {
            var iconUrl = data.Type == WebFileType.Folder ? "/res/icon/folder.png" : "/res/icon/page.png";
            return $"<img src='{iconUrl}' />";
        }

        private string GetNameText(WebFile data)
        {
            if (data.Type == WebFileType.File)
                return $"<a href='{data.Url}' target='_blank'>{data.Name}</a>";
            else
            {
                var url = Urls.GetExplorerUrl(_root, _filter, _showDownload, _showInfo, this.Mode, data.Url, true);
                return $"<a href='{url}'>{data.Name}</a>";
            }
        }

        private static string GetDownText(WebFile data)
        {
            var downUrl = Urls.GetDownUrl(data.Url, data.Name);
            var downIcon = string.Format("<img src='{0}' />", UI.GetIconUrl(FineUIPro.Icon.PackageDown));
            return (data.Type == WebFileType.Folder) ? "" : $"<a href='{downUrl}' target='_blank'>{downIcon}</a>";
        }
    }
}
