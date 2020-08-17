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
using App.Components;

namespace App.Admins
{
    [UI("文章详情")]
    [Auth(AuthLogin = true)]
    [Param("id", "文章ID")]
    public partial class Article : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Asp.GetQueryLong("id");
                if (id == null)
                    return;
                var item = DAL.Article.GetDetail(id, Common.LoginUser?.ID);
                if (item == null)
                {
                    Asp.Fail("无此文章");
                    return;
                }

                //
                this.lblTitle.Text = item.Title;
                this.lblAuthor.Text = item.AuthorName;
                this.lblPostDt.Text = item.CreateDt?.ToString("yyyy-MM-dd");
                this.lblVisitCnt.Text = item.VisitCnt.ToText();
                this.lblApproval.Text = item.ApprovalCnt.ToText();
                this.lblContent.Text = item.Body;

                // 图片附件
                this.rptFiles.DataSource = item.Reses;
                this.rptFiles.DataBind();

                // 评论
                this.rptReply.DataSource = item.GetReplies();
                this.rptReply.DataBind();
            }
        }
    }
}
