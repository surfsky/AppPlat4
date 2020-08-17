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

namespace App.Pages
{
    [UI("关注")]
    [Auth(Powers.FavoriteView, Powers.FavoriteNew, Powers.FavoriteEdit, Powers.FavoriteDelete)]
    [Param("userId", "用户ID", false)]
    public partial class FavoriteForm : FormPage<ArticleDirFavorite>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
            {
                UI.BindEnum(ddlType, typeof(FavoriteType));
                UI.BindTree(ddlDir, ArticleDir.All, t=> t.ID, t=> t.Name);
                ShowForm();
            }
        }


        //-----------------------------------------------
        // 数据清空、展示、采集、保存
        //-----------------------------------------------
        // 清空数据
        public override void NewData()
        {
            // userId
            DAL.User user = null;
            var userId = Asp.GetQueryLong("userId");
            if (userId != null)
                user = DAL.User.Get(userId);

            //
            UI.SetValue(this.lblId, "-1");
            UI.SetValue(this.ddlType, FavoriteType.System);
            UI.SetValue(this.pbUser, user, t => t.ID, t => t.NickName);
            UI.SetValue(this.pbDept, "");
            UI.SetValue(this.ddlDir, "");

            UI.SetValue(this.tbSeq, 0);
            UI.SetValue(this.lblCreateDt, "");
            UI.SetValue(this.tbRemark, "");
            SwitchType();
        }

        // 加载数据
        public override void ShowData(ArticleDirFavorite item)
        {
            UI.SetValue(this.lblId, item.ID);
            UI.SetValue(this.ddlType, item.Type);
            UI.SetValue(this.pbUser, item.User, t => t.ID, t => t.NickName);
            UI.SetValue(this.pbDept, item.Dept, t => t.ID, t => t.Name);
            UI.SetValue(this.ddlDir, item.ArticleDirID);

            UI.SetValue(this.tbSeq, item.Seq);
            UI.SetValue(this.lblCreateDt, item.CreateDt);
            UI.SetValue(this.tbRemark, item.Remark);
            SwitchType();
        }

        // 采集数据
        public override void CollectData(ref ArticleDirFavorite item)
        {
            item.Type = UI.GetEnum<FavoriteType>(this.ddlType);
            item.UserID = UI.GetLong(this.pbUser);
            item.DeptID = UI.GetLong(this.pbDept);
            item.ArticleDirID = UI.GetLong(this.ddlDir);
            item.Seq = UI.GetInt(this.tbSeq);
            item.Remark = UI.GetText(this.tbRemark);
        }

        // 类型变更
        protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SwitchType();
        }

        private void SwitchType()
        {
            var type = UI.GetEnum<FavoriteType>(ddlType);
            if (type == FavoriteType.System)
            {
                this.pbDept.Hidden = true;
                this.pbUser.Hidden = true;
            }
            else if (type == FavoriteType.Dept)
            {
                this.pbDept.Hidden = false;
                this.pbUser.Hidden = true;
            }
            else if (type == FavoriteType.User)
            {
                this.pbDept.Hidden = true;
                this.pbUser.Hidden = false;
            }
        }
    }
}
