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

namespace App.Admins
{
    [UI("弹窗测试")]
    [Auth(Powers.Admin)]
    public partial class TestPopupbox : FormPage<Invite>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            this.pbFiles.TextChanged += pbFiles_TextChanged;
            if (!IsPostBack)
            {
                UI.BindEnum(ddlSource, typeof(InviteSource));
                ShowForm();
                pbPages.UrlTemplate = Urls.GetExplorerUrl("/Pages/Base", ".aspx .ashx", false, false, null, "", false);
                pbFiles.UrlTemplate = Urls.GetExplorerUrl("/res/", ".jpg .png .gif", false, false, null, "", false);
            }
        }

        // 文件选择完毕后触发（没有及时触发，需完善）
        private void pbFiles_TextChanged(object sender, EventArgs e)
        {
            var file = pbFiles.Text;
            var type = Asp.GetHandler(file);
            if (type != null)
            {
                UI.ShowAlert(type.Name);
            }
        }

        //-----------------------------------------------
        // 数据清空、展示、采集、保存
        //-----------------------------------------------
        // 清空数据
        public override void NewData()
        {
            UI.SetValue(this.lblId, "-1");
            UI.SetValue(this.ddlSource, null);
            UI.SetValue(this.dpCreate, DateTime.Now);
            UI.SetValue(tbUser, Common.LoginUser, t => t.ID, t => t.NickName);
        }

        // 加载数据
        public override void ShowData(Invite item)
        {
            UI.SetValue(this.lblId, item.ID);
            UI.SetValue(this.ddlSource, item.Source);
            UI.SetValue(this.dpCreate, item.CreateDt);
            UI.SetValue(tbUser, item.Inviter, t => t.ID, t => t.NickName);
        }

        // 采集数据
        public override void CollectData(ref Invite item)
        {
            item.Source = UI.GetEnum<InviteSource>(this.ddlSource);
            item.CreateDt = UI.GetDate(this.dpCreate);
            item.InviteeID = UI.GetLong(tbUser);
            var gps = UI.GetText(pbGPS);
        }
    }
}
