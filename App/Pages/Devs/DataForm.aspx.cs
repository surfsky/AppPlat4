using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Data.Entity;
using FineUIPro;
using App.DAL;
using App;
using System.Reflection;
using System.Collections;
using App.Utils;
using App.Components;
using App.Controls;

namespace App.Pages
{
    [UI("通用数据编辑窗口")]
    [Auth(AuthLogin =true)]
    [Param("tp", "实体类名")]
    [Param("id",   "ID")]
    [Param("uiid", "UI 配置 ID")]
    [Param("md",   "访问模式", typeof(PageMode))]
    [Param("pv",   "访问权限", typeof(Powers))]
    [Param("pn",   "新建权限", typeof(Powers))]
    [Param("pe",   "编辑权限", typeof(Powers))]
    [Param("pd",   "删除权限", typeof(Powers))]
    public partial class DataForm : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 解析类型名
            var typeName = Asp.GetQueryString("tp");
            var type = Reflector.GetType(typeName);
            if (type == null)
            {
                Asp.Fail("未找到指定的类");
                return;
            }

            // 权限控制
            var auth = type.GetAttribute<AuthAttribute>();
            if (auth != null)
            {
                this.Auth.ViewPower   = auth.ViewPower;
                this.Auth.NewPower    = auth.NewPower;
                this.Auth.EditPower   = auth.EditPower;
                this.Auth.DeletePower = auth.DeletePower;
            }
            else
            {
                this.Auth.ViewPower   = Asp.GetQuery<Powers>("pv");
                this.Auth.NewPower    = Asp.GetQuery<Powers>("pn");
                this.Auth.EditPower   = Asp.GetQuery<Powers>("pe");
                this.Auth.DeletePower = Asp.GetQuery<Powers>("pd");
            }
            if (!CheckAuth())
                return;

            // 创建表单
            this.form2.Mode = this.Mode;
            this.form2.Build(type);
        }
    }
}
