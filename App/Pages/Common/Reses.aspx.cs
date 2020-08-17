using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
//using EntityFramework.Extensions;
using App.DAL;
using App.Utils;
using System.Drawing;
using System.Drawing.Imaging;
using App.Components;
using App.Controls;
using App.Entities;

namespace App.Admins
{
    [UI("资源管理")]
    [Auth(AuthLogin = true)]
    [Param("key", "资源键值")]
    [Param("cate", "附件目录")]
    [Param("imageOnly", "只允许上传图片", false)]
    [Param("imageWidth", "图片宽度限制", false)]
    [Param("imageHeight", "图片高度限制", false)]
    [Param("mode", "模式", false)]
    [Param("selectName", "选择器参数", false)]
    public partial class Reses : PageBase
    {
        /*
        <f:HyperLinkField HeaderText="图片" Width="50px" DataTextField="Url" DataTextFormatString="<img src='{0}?w=50'/>" DataNavigateUrlFields="Url" HtmlEncode="false" UrlEncode="false" />
        <f:BoundField HeaderText="类型" DataField="Type" SortField="Type" Width="100px"  />
        <f:BoundField HeaderText="大小" DataField="FileSize" SortField="FileSize" Width="100px"  />
        <f:BoundField HeaderText="时间" DataField="CreateDt" SortField="CreateDt" Width="200px" Hidden="false" />
         */
        // Init
        protected void Page_Load(object sender, EventArgs e)
        {
            this.uploader.AcceptFileTypes = (Asp.GetQueryBool("imageOnly") == true) ? "image/*" : "*/*";
            var key = Asp.GetQueryString("key");
            var cate = Asp.GetQueryString("cate");
            var newUrl = string.Format("ResForm.aspx?md=new&key={0}&cate={1}", key, cate);
            this.Grid1.SetPowers(true, false, true, true)
                .SetUrls(
                    newUrl,
                    "ResForm.aspx?md=view&id={0}",
                    "ResForm.aspx?md=edit&id={0}")
                .AddThrumbnailColumn<Res>(t=>t.Url, 40, " ")
                .AddLinkColumn<Res>(t => t.Url, t => t.FileName, "/Down.ashx?url={0}", 400, "名称")
                .AddColumn<Res>(t => t.FileSizeText, 100, "大小")
                .AddColumn<Res>(t => t.CreateDt, 100, "时间", "{0:yyyy-MM-dd}")
                .AddColumn<Res>(t => t.Type, 100, "类型")
                .AddCheckColumn<Res>(t => t.Protect, 80, "保护")
                .InitGrid<Res>(BindGrid, this.Panel1, t=>t.FileName)
                ;
            this.Grid1.RowDataBound += Grid1_RowDataBound;
            if (!IsPostBack)
            {
                UI.SetVisible(this.uploader, (this.Mode == PageMode.Edit));
                UI.SetVisible(this.btnSelect, (this.Mode == PageMode.Select));
                this.Grid1.SetSortPage<Res>(SiteConfig.Instance.PageSize, t => t.ID, true);
                BindGrid();

                // 打开大文件上传对话框
                var url = Urls.GetHugeUpUrl(cate, "", key);
                Grid1.Win.CloseAction = CloseAction.HideRefresh;
                var script = Grid1.Win.GetShowReference(url, "大文件上传", 500, 200);
                this.btnBigUp.OnClientClick += script;
            }
        }

        // 行绑定事件
        private void Grid1_RowDataBound(object sender, GridRowEventArgs e)
        {
            var data = e.DataItem as Res;
            var url = Urls.GetDownUrl(data.ID);
            var tag = string.Format("<a href='{0}' target='_blank'>{1}</a>", url, data.FileName);
            UI.SetGridCellText(Grid1, "Link-Url", tag, e);

        }

        // 绑定网格
        private void BindGrid()
        {
            string key = Request.QueryString["key"];
            IQueryable<Res> q = Res.Search(key);
            Grid1.Bind(q);
        }

        // 查找
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        // 删除事件
        protected void Grid1_Delete(object sender, List<long> ids)
        {
            Res.DeleteBatch(ids);
        }

        // 图片上传
        protected void uploader_FileSelected(object sender, EventArgs e)
        {
            // 保存目录和键
            string cate = Request.QueryString["cate"];
            string key = Request.QueryString["key"];

            // 上传
            if (uploader.HasFile)
            {
                // 图片文件自动压缩
                Size? size = null;
                var ext = uploader.FileName.GetFileExtension();
                if (IO.IsImageFile(uploader.FileName))
                {
                    var imageWidth = Asp.GetQueryInt("imageWidth") ?? 500;
                    var imageHeight = Asp.GetQueryInt("imageHeight") ?? 500;
                    size = new Size(imageWidth, imageHeight);
                }

                // 上传并记录
                var exts = SiteConfig.Instance.UpFileTypes.SplitString();
                if (!exts.Contains(ext))
                    UI.ShowAlert("不允许上传该类型文件");
                else
                {
                    var url = UI.UploadFile(uploader, cate, size);
                    Res.Add(ResType.File, key, url, uploader.FileName);
                    BindGrid();
                }
            }
        }

        // 选择并关闭
        protected void btnSelect_Click(object sender, EventArgs e)
        {
            var ids = this.Grid1.GetSelectedIds().ToSeparatedString();
            string script = string.Format("parent.__doPostBack('','Select_{0}_{1}');", Asp.GetQueryString("selectName"), ids);
            PageContext.RegisterStartupScript(script);
        }
    }
}
