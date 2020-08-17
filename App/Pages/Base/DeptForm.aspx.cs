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
    [UI("部门")]
    [Auth(Powers.DeptView, Powers.DeptNew, Powers.DeptEdit, Powers.DeptDelete)]
    [Param("parentId", "父部门ID")]
    public partial class DeptForm : FormPage<Dept>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
                ShowForm();
        }


        //-----------------------------------------------
        // 数据清空、展示、采集、保存
        //-----------------------------------------------
        // 清空数据
        public override void NewData()
        {
            var parentId = Asp.GetQueryLong("parentid");
            UI.SetValue(this.lblId, "-1");
            UI.SetValue(this.tbName, "");
            UI.SetValue(this.tbSeq, 0);
            UI.SetValue(this.tbRemark, "");
            BindDepts(parentId, parentId);
        }

        // 加载数据
        public override void ShowData(Dept item)
        {
            UI.SetValue(this.lblId, item.ID);
            UI.SetValue(this.tbName, item.Name);
            UI.SetValue(this.tbSeq, item.Seq);
            UI.SetValue(this.tbRemark, item.Remark);
            //UI.SetValue(tbDingDeptId, item.DingDeptID);
            BindDepts(item.ParentID, item.ID);
        }

        // 采集数据
        public override void CollectData(ref Dept item)
        {
            item.Name = UI.GetText(this.tbName);
            item.Seq = UI.GetInt(this.tbSeq);
            item.ParentID = UI.GetLong(this.ddlParent);
            item.Remark = UI.GetText(this.tbRemark);
            //item.DingDeptID = UI.GetLong(this.tbDingDeptId);
        }

        // 绑定下拉列表（启用模拟树功能和不可选择项功能）
        private void BindDepts(long? selectId=null, long? disableId=null)
        {
            UI.BindTree(ddlParent, Dept.All, t => t.ID, t => t.Name, "--根部门--", selectId, disableId);
        }
    }
}
