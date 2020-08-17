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
    [UI("区域")]
    [Auth(Powers.AreaView, Powers.AreaNew, Powers.AreaEdit, Powers.AreaDelete)]
    [Param("parentId", "父节点ID")]
    public partial class AreaForm : FormPage<Area>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
            {
                UI.BindEnum(this.ddlType, typeof(AreaType));
                ShowForm();
            }
        }


        // 绑定下拉列表
        private void BindAreas(long? selectId = null, long? disableId = null)
        {
            UI.BindTree(ddlParent, Common.LoginUser.GetAllowedAreas(), t => t.ID, t => t.Name, "--根区域--", selectId, disableId);
        }

        //-----------------------------------------------
        // 数据清空、展示、采集、保存
        //-----------------------------------------------
        // 清空数据
        public override void NewData()
        {
            UI.SetValue(lblId, "-1");
            UI.SetValue(tbName, "");
            UI.SetValue(tbSeq, "0");
            UI.SetValue(tbRemark, "");

            var parentId = Asp.GetQueryLong("parentid");
            BindAreas(parentId, parentId);
        }

        // 加载数据
        public override void ShowData(Area item)
        {
            UI.SetValue(lblId, item.ID);
            UI.SetValue(tbName, item.Name);
            UI.SetValue(tbSeq, item.Seq);
            UI.SetValue(tbRemark, item.Remark);
            UI.SetValue(ddlType, item.Type);
            UI.SetValue(tbFullName, item.FullName);
            BindAreas(item.ParentID, item.ID);
        }

        // 采集数据
        public override void CollectData(ref Area item)
        {
            item.Type = UI.GetEnum<AreaType>(this.ddlType);
            item.Name = UI.GetText(this.tbName);
            item.Seq = UI.GetInt(this.tbSeq, 0);
            item.ParentID = UI.GetLong(this.ddlParent);
            item.Remark = UI.GetText(this.tbRemark);
        }

        // 保存完毕后刷新部门数据
        public override void SaveData(Area item)
        {
            item.Save();
            item.SetFullName();
            UI.SetValue(tbFullName, item.FullName);
        }

    }
}
