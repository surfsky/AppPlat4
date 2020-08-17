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

namespace App.Admins
{
    [Auth(Powers.InviteView, Powers.InviteNew, Powers.InviteEdit, Powers.InviteDelete)]
    public partial class Invites : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Grid1
                .SetPowers(this.Auth)
                .SetUrls("InviteForm.aspx")
                .InitGrid<DAL.Invite>(BindGrid, Panel1, t => t.InviteeMobile)
                ;
            if (!IsPostBack)
            {
                MallHelper.LimitShop(this.pbInviteShop);

                this.Grid1.SetSortPage<Invite>(SiteConfig.Instance.PageSize, t => t.CreateDt, false);
                BindGrid();
            }
        }

        //-------------------------------------------------
        // Grid
        //-------------------------------------------------
        private void BindGrid()
        {
            var inviteShopId = UI.GetLong(pbInviteShop);
            var inviterName = UI.GetText(tbInviterName);
            var inviterMobile = UI.GetText(tbInviterMobile);
            var inviteeName = UI.GetText(tbInviteeName);
            var inviteeMobile = UI.GetText(tbInviteeMobile);
            var startDt = UI.GetDate(this.dpStart);
            IQueryable<Invite> q = Invite.Search(
                inviteShopId: inviteShopId,
                inviterName: inviterName,
                inviterMobile: inviterMobile,
                inviteeName: inviteeName,
                inviteeMobile: inviteeMobile,
                createStartDt: startDt
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
