using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
using App.DAL;
using App.Utils;
using App.Controls;
using App.Components;

namespace App.Admins
{
    [UI("文章目录")]
    [Auth(Powers.ArticleDirView, Powers.ArticleDirEdit, Powers.ArticleDirEdit, Powers.ArticleDirEdit)]
    [Param("parentId", "父节点ID")]
    public partial class ArticleDirForm : FormPage<ArticleDir>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
            {
                ShowForm();
                UI.SetText(uploader, SiteConfig.Instance.SizeDirImage);
            }
        }


        //-----------------------------------------------
        // 数据清空、展示、采集、保存
        //-----------------------------------------------
        // 清空数据
        public override void NewData()
        {
            UI.SetValue(this.lblId, "-1");
            UI.SetValue(this.tbName, "");
            UI.SetValue(this.tbSeq, 0);
            UI.SetValue(this.tbRemark, "");
            var parentId = Asp.GetQueryLong("parentid");
            BindDirs(parentId, parentId);
        }

        // 加载数据
        public override void ShowData(ArticleDir item)
        {
            UI.SetValue(this.lblId, item.ID);
            UI.SetValue(this.tbName, item.Name);
            UI.SetValue(this.tbSeq, item.Seq);
            UI.SetValue(this.tbRemark, item.Remark);
            UI.SetValue(this.img, item.Icon);
            BindDirs(item.ParentID, item.ID);
        }

        // 采集数据
        public override void CollectData(ref ArticleDir item)
        {
            item.Name = UI.GetText(this.tbName);
            item.Seq = UI.GetInt(this.tbSeq);
            item.Remark = UI.GetText(this.tbRemark);
            item.ParentID = UI.GetLong(ddlParent);
            item.Icon = UI.GetUrl(img);
        }

        // 绑定下拉列表（启用模拟树功能和不可选择项功能）
        private void BindDirs(long? selectId=null, long? disableId=null)
        {
            UI.BindTree(ddlParent, ArticleDir.All, t => t.ID, t => t.Name, "--根--", selectId, disableId);
        }

        // 图片上传
        protected void uploader_FileSelected(object sender, EventArgs e)
        {
            string imageUrl = UI.UploadFile(uploader, "ArticleDir", SiteConfig.Instance.SizeDirImage);
            UI.SetValue(this.img, imageUrl, true);
            if (this.Mode == PageMode.Edit)
            {
                var data = this.GetData();
                data.Icon = UI.GetUrl(this.img);
                data.Save();
            }
        }
    }
}
