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

namespace App.Tests
{
    [UI("视频上传")]
    [Auth(AuthLogin =true)]
    public partial class TestChunkUp : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }

        // 上传
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            win.IFrameUrl = Urls.GetHugeUpUrl("Videos", "About.mp4", "", "video/*");
            win.Hidden = false;
        }

        
    }
}
