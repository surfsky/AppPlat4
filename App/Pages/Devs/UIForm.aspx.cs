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
using App.Entities;

namespace App.Pages
{
    [UI("UI配置")]
    [Auth(Powers.Admin)]
    [Param("tp", "实体类型")]
    public partial class UIForm : FormPage<XUI>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var type = Asp.GetQueryString("tp");
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
            {
                UI.Bind(ddlEntityType, AppContext.EntityTypes, t => t.FullName, t => t.FullName);
                UI.BindEnum(ddlType, typeof(XUIType));
                this.lnkUrl.NavigateUrl = Urls.GetDataModelUrl(typeof(UIAttribute), new AuthAttribute(Powers.Admin));
                ShowForm();

                // 参数值
                if (type.IsNotEmpty())
                {
                    UI.SetValue(ddlEntityType, type);
                    UI.SetEnable(false, ddlEntityType);
                }
            }
        }

        public override void NewData()
        {
            UI.SetValue(this.tbName, "");
            UI.SetValue(this.ddlType, XUIType.Form);
            UI.SetValue(this.ddlEntityType, "");
            UI.SetValue(this.tbSetting, "");
            UI.SetValue(this.tbError, "");
            BuildSetting();
        }

        public override void ShowData(XUI item)
        {
            UI.SetValue(this.tbName, item.Name);
            UI.SetValue(this.ddlType, item.Type);
            UI.SetValue(this.ddlEntityType, item.EntityTypeName);
            UI.SetValue(this.tbSetting, item.SettingText);
            UI.SetValue(this.tbError, item.Error);
            BuildSetting();
        }

        public override void CollectData(ref XUI item)
        {
            item.Name = UI.GetText(tbName);
            item.Type = UI.GetEnum<XUIType>(ddlType);
            item.EntityTypeName = UI.GetText(ddlEntityType);
            item.SettingText =  UI.GetText(tbSetting);
            item.Error = "";

            // 尝试解析和优化
            try
            {
                item.Setting = item.SettingText.ParseJson<UISetting>();  // 尝试解析
                item.SettingText = item.Setting.ToJson();                // 尝试优化
            }
            catch (Exception ex)
            {
                item.Error = ex.Message;
            }
            UI.SetText(tbSetting, item.SettingText);
            UI.SetText(tbError, item.Error);
        }

        // 生成配置
        protected void ddlEntityType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuildSetting();
        }
        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            BuildSetting();
        }
        private void BuildSetting()
        {
            if (tbSetting.Text.IsEmpty())
            {
                var typeName = UI.GetText(ddlEntityType);
                var type = Reflector.GetType(typeName);
                if (type != null)
                {
                    this.tbSetting.Text = new UISetting(type).ToJson();
                    this.tbName.Text = string.Format("{0}-{1}", type.GetTitle(), UI.GetEnum<XUIType>(ddlType).GetTitle());
                }
            }
        }
    }
}
