using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using App.Controls;
using App.Utils;
using App.Components;
using App.DAL;

namespace App.Admins
{
    [UI("样式设置")]
    [Auth(DAL.Powers.Admin)]
    public partial class Themes : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}