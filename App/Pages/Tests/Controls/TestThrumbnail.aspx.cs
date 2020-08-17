using System;
using System.Web.UI;
using FineUIPro;
using App.Controls;
using App.DAL;
using App.Components;

namespace App.Tests
{
    /// <summary>
    /// </summary>
    [Auth(Powers.Admin)]
    public partial class TestThrumbnail : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
            }
        }

        protected void file_FileSelected(object sender, EventArgs e)
        {
            string imageUrl = UI.UploadFile(file, "Articles", null);
            UI.SetValue(img, imageUrl, true);
        }

        protected void btnGet_Click(object sender, EventArgs e)
        {
            var url = img.ImageUrl;
            UI.ShowAlert(url);
        }
    }
}
