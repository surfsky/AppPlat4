using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using System.Linq;
using System.Data.Entity;
//using EntityFramework.Extensions;
using App.DAL;
using App.Utils;
using App.Components;
using App.Controls;

namespace App.Admins
{
    [UI("文章推送")]
    public partial class ArticlePush : PageBase
    {
        //--------------------------------------------------
        // Init
        //--------------------------------------------------
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                UI.Bind(ddlTitle, DAL.Title.Set.ToList(), t => t.ID, t => t.Name, "--全部职务--", null);
                UI.Bind(ddlRole, Role.All, nameof(Role.ID), nameof(Role.Name), "--全部角色--", null);
            }
        }



        // 推送
        protected void btnPush_Click(object sender, EventArgs e)
        {
            
        }
    }
}
