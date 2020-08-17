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
    [UI("序列号")]
    [Auth(Powers.Sequence)]
    public partial class SequenceForm : FormPage<Sequence>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
            {
                UI.BindEnum(this.ddlType, typeof(SequenceType));
                UI.BindEnum(this.ddlLoop, typeof(LoopType));
                UI.SetVisible(this.btnGenerate, (this.Mode == PageMode.Edit || this.Mode == PageMode.View));
                ShowForm();
            }
        }

        public override void NewData()
        {
            UI.SetValue(ddlType, null);
            UI.SetValue(ddlLoop, null);
            UI.SetValue(tbFormat, "{0:yyyyMMdd}{1:00000}");
            UI.SetValue(dpLastDt, null);
            UI.SetValue(tbLastValue, 0);
            UI.SetValue(tbLastSeq, "");
        }

        public override void ShowData(Sequence item)
        {
            UI.SetValue(ddlType, item.Type);
            UI.SetValue(ddlLoop, item.Loop);
            UI.SetValue(tbFormat, item.Format);
            UI.SetValue(dpLastDt, item.LastDt);
            UI.SetValue(tbLastValue, item.LastValue);
            UI.SetValue(tbLastSeq, item.LastSeq);
        }

        public override void CollectData(ref Sequence item)
        {
            item.Type = UI.GetEnum<SequenceType>(ddlType);
            item.Loop = UI.GetEnum<LoopType>(ddlLoop);
            item.Format = UI.GetText(tbFormat);
            item.LastDt = UI.GetDate(dpLastDt);
            item.LastValue = UI.GetInt(tbLastValue);
        }

        // 生成新序列号
        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            var seq = this.Save();
            if (seq != null)
            {
                var txt = seq.Generate();
                ShowData(seq);
            }
        }
    }
}
