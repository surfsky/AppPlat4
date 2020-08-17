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
using App.Components;
using App.Controls;

namespace App.Pages
{
    [UI("用户资产管理")]
    [Auth(Powers.AssetView, Powers.AssetNew, Powers.AssetEdit, Powers.AssetDelete)]
    public partial class UserAssets : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var userId = Asp.GetQueryLong("userId") ?? Common.LoginUser.ID;
            var newUrl = string.Format("UserAssetForm.aspx?md=new&userId={0}", userId);
            this.Grid1
                .SetPowers(this.Auth)
                .SetUrls(
                    newUrl,
                    "UserAssetForm.aspx?md=view&id={0}",
                    "UserAssetForm.aspx?md=edit&id={0}")
                .InitGrid<DAL.UserAsset>(BindGrid, Panel1, t => t.Name)
                ;
            if (!IsPostBack)
            {
                this.Grid1.SetSortPage<UserAsset>(SiteConfig.Instance.PageSize, t => t.CreateDt, false);
                BindGrid();
                UI.SetVisibleByQuery("search", this.tbUser, this.dpCreate, this.btnSearch);
            }
        }

        //-------------------------------------------------
        // Grid
        //-------------------------------------------------
        private void BindGrid()
        {
            var userId = Asp.GetQueryLong("userId");
            var user = UI.GetText(tbUser);
            var serialNo = UI.GetText(tbSerialNo);
            var createDt = UI.GetDate(this.dpCreate);
            IQueryable<UserAsset> q = UserAsset.Search(
                userId: userId,
                userName:user, 
                startDt: createDt,
                serialNo: serialNo
                );
            Grid1.Bind(q);
        }

        //-------------------------------------------------
        // 工具栏
        //-------------------------------------------------
        // 查找
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}
