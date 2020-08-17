using System;
using System.Web;
using System.Web.Security;
using FineUIPro;
using System.Text;
using System.Linq;
using App;
using App.DAL;
using App.Utils;
using App.Components;
using App.Controls;

namespace App
{
    [UI("默认登陆页面")]
    [Auth(Ignore =true)]
    public partial class Default : PageBase
    {
        protected System.Web.UI.HtmlControls.HtmlForm form1;
        protected System.Web.UI.WebControls.Label lblTitle;
        protected System.Web.UI.WebControls.Label lblDomain;
        protected System.Web.UI.WebControls.Label lblICPNumber;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.lblTitle.Text = SiteConfig.Instance.Name;
                this.lblDomain.Text = SiteConfig.Instance.Domain;
                this.lblICPNumber.Text = SiteConfig.Instance.ICP;
                
                if (User.Identity.IsAuthenticated)
                    Response.Redirect(FormsAuthentication.DefaultUrl);
            }
        }
    }
}
