using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
using App.Controls;
using App.DAL;
using App.Utils;
using System.Drawing;
using App.Components;

namespace App.Admins
{
    [UI("文章")]
    [Auth(Powers.ArticleView, Powers.ArticleNew, Powers.ArticleEdit, Powers.ArticleDelete)]
    [Param("replyId", "回复文章ID")]
    [Param("type", "文章类型", typeof(ArticleType))]
    [Param("typeGroup", "文章分组", typeof(string), Remark = "Page|Site|FAQ|Doc")]
    public partial class ArticleForm : FormPage<DAL.Article>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.imgUploader.ImageSize = SiteConfig.Instance.SizeBigImage;
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
            {
                var typeGroup = Asp.GetQueryString("typeGroup");
                var motherPages = DAL.Article.Search(type: ArticleType.MotherPage).Sort(t => t.Title).ToList();
                UI.BindEnum(ddlType, typeof(ArticleType), "--类型--", typeGroup);
                UI.BindEnum(ddlStatus, typeof(ArticleStatus), "--状态--");
                UI.BindTree(ddlDir, ArticleDir.All, t=> t.ID, t=> t.Name, "--目录--", null, null);
                UI.Bind(ddlMother, motherPages, t => t.ID, t => t.Title, "--母版--");
                UI.BindBool(ddlIsRequir, "是", "否", "--请选择--", false);
                UI.BindBool(ddlIsValid, "是", "否", "--请选择--", false);
                ShowForm();
            }
        }

        // 类别变更
        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SwitchPanel(null);
        }

        // 切换文档类型，界面调整
        private void SwitchPanel(DAL.Article article)
        {
            // 文章类型
            var type = Asp.GetQuery<ArticleType>("type");
            if (type != null)
            {
                UI.SetValue(ddlType, type);
                UI.SetEnable(false, ddlType);
            }
            type = UI.GetEnum<ArticleType>(ddlType);
            this.panPage.Hidden = (type != ArticleType.Page);
            this.panRes.Hidden = (Mode == PageMode.New);
            this.panReply.Hidden = (Mode == PageMode.New || article == null);
            this.ddlIsRequir.Hidden = (type != ArticleType.Knowledge);

            // 回帖
            bool isReply = Asp.GetQueryLong("replyId") != null || article?.Type == ArticleType.Reply;
            if (isReply)
            {
                UI.SetValue(ddlType, ArticleType.Reply);
                UI.SetVisible(false, this.imgUploader, tbVisitCnt,
                    this.ddlType, this.tbKeywords, this.ddlDir, this.tbWeight,
                    this.panReply, this.tbContent);
                this.ddlIsValid.Hidden = false;
            }

            // FAQ 界面控制
            var typeGroup = Asp.GetQueryString("typeGroup");
            if (typeGroup == "FAQ" || type.GetEnumInfo().Group == "FAQ")
            {
                this.panArticle.Hidden = true;
                this.lblReplyId.Hidden = true;
            }
        }

        //---------------------------------------------
        // CRU
        //---------------------------------------------
        public override void NewData()
        {
            UI.SetValue(imgUploader, SiteConfig.Instance.DefaultArticleImage);
            UI.SetValue(tbTitle, "");
            UI.SetValue(tbAuthor, Common.LoginUser.NickName);
            UI.SetValue(tbContent, "");
            UI.SetValue(lblCreateDt, "");
            UI.SetValue(tbVisitCnt, "0");
            UI.SetValue(ddlType, ArticleType.Knowledge);
            UI.SetValue(ddlMother, null);
            UI.SetValue(tbMotherSlot, "{{Content}}");
            UI.SetValue(tbRoutePath, "");
            UI.SetValue(tbCacheSeconds, 0);
            UI.SetValue(ddlDir, null);
            UI.SetValue(tbKeywords, "");
            UI.SetValue(tbWeight, 0);
            UI.SetValue(lblReplyId, Asp.GetQueryLong("replyId"));
            UI.SetValue(ddlStatus, ArticleStatus.Publish);
            UI.SetValue(dtExpire, null);
            UI.SetValue(ddlIsRequir, false);
            UI.SetValue(ddlIsValid, false);

            SwitchPanel(null);
        }

        public override void ShowData(DAL.Article item)
        {
            panRes.IFrameUrl = Urls.GetResesUrl(this.Mode, item.UniID, "Articles", false);
            panReply.IFrameUrl = Urls.GetArticlesUrl(this.Mode, item.ID, false);

            UI.SetValue(imgUploader, item.CoverImage);
            UI.SetValue(tbTitle, item.Title);
            UI.SetValue(tbAuthor, item.AuthorName);
            UI.SetValue(tbContent, item.Body);
            UI.SetValue(lblCreateDt, item.CreateDt);
            UI.SetValue(tbVisitCnt, item.VisitCnt);
            UI.SetValue(ddlType, item.Type);
            UI.SetValue(ddlMother, item.MotherID);
            UI.SetValue(tbMotherSlot, item.MotherSlot);
            UI.SetValue(tbRoutePath, item.RoutePath);
            UI.SetValue(tbCacheSeconds, item.CacheSeconds);
            UI.SetValue(ddlDir, item.DirID);
            UI.SetValue(tbKeywords, item.Keywords);
            UI.SetValue(tbWeight, item.Weight);
            UI.SetValue(lblReplyId, item.ReplyID);
            UI.SetValue(ddlStatus, item.Status);
            UI.SetValue(dtExpire, item.ExpireDt);
            UI.SetValue(ddlIsRequir, item.IsRequir);
            UI.SetValue(ddlIsValid, item.IsValid);

            SwitchPanel(item);
            UI.SetEnable(false, ddlType);
        }

        public override void CollectData(ref DAL.Article item)
        {
            item.CoverImage = UI.GetUrl(imgUploader);
            item.Title = UI.GetText(tbTitle);
            item.AuthorName = UI.GetText(tbAuthor);
            item.Body = UI.GetText(tbContent);
            item.VisitCnt = UI.GetInt(tbVisitCnt, 0);
            item.Type = UI.GetEnum<ArticleType>(ddlType);
            item.MotherID = UI.GetLong(ddlMother);
            item.MotherSlot = UI.GetText(tbMotherSlot);
            item.RoutePath = UI.GetText(tbRoutePath);
            item.CacheSeconds = UI.GetInt(tbCacheSeconds, 0);
            item.DirID = UI.GetLong(ddlDir);
            item.Keywords = UI.GetText(tbKeywords);
            item.Weight = UI.GetInt(tbWeight);
            item.ReplyID = UI.GetLong(lblReplyId);
            item.Status = UI.GetEnum<ArticleStatus>(ddlStatus);
            item.ExpireDt = UI.GetDate(dtExpire);
            item.IsRequir = UI.GetBool(ddlIsRequir);
            item.IsValid = UI.GetBool(ddlIsValid);

            item.SetSummary();
            if (item.AuthorID == null)
                item.AuthorID = Common.LoginUser.ID;
        }

        // 保存数据（后刷新一下缓存）
        public override void SaveData(DAL.Article item)
        {
            item.Save();
            ContentHandler.Reload();
        }

        // 图片上传成功
        protected void ImgUploader_FileUploaded(object sender, EventArgs e)
        {
            if (this.Mode == PageMode.Edit)
            {
                var data = this.GetData();
                data.CoverImage = UI.GetUrl(this.imgUploader);
                data.Save();
            }
        }
    }
}
