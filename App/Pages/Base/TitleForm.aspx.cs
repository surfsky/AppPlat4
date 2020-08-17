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

namespace App.Admins
{
    [UI("职务")]
    [Auth(Powers.TitleView, Powers.TitleNew, Powers.TitleEdit, Powers.TitleDelete)]
    public partial class TitleForm : FormPage<Title>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
                ShowForm();
        }

        public override void NewData()
        {
            UI.SetText(tbName, "");
            UI.SetText(tbRemark, "");
        }

        public override void ShowData(Title item)
        {
            UI.SetText(tbName, item.Name);
            UI.SetText(tbRemark, item.Remark);
        }

        public override void CollectData(ref Title item)
        {
            item.Name = UI.GetText(tbName);
            item.Remark = UI.GetText(tbRemark);
        }
    }
}
