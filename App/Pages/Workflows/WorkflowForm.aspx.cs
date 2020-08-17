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

namespace App.Pages
{
    [UI("工作流节点")]
    [Auth(Powers.Admin)]
    public partial class WorkflowForm : FormPage<Title>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
                ShowForm();
        }

        public override void NewData()
        {
            tbxName.Text = "";
            tbxRemark.Text = "";
        }

        public override void ShowData(Title item)
        {
            tbxName.Text = item.Name;
            tbxRemark.Text = item.Remark;
        }

        public override void CollectData(ref Title item)
        {
            item.Name = tbxName.Text.Trim();
            item.Remark = tbxRemark.Text.Trim();
        }
    }
}
