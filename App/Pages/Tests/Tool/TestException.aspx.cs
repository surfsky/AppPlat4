using System;
using System.Web.UI;
using FineUIPro;
using App.Controls;
using App.DAL;
using App.Components;
using App.HttpApi;

namespace App.Tests
{
    /// <summary>
    /// </summary>
    [Auth(Powers.Admin)]
    public partial class TestException : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            throw new HttpApiException(500, "触发异常，请在后台日志查看");
        }
    }
}
