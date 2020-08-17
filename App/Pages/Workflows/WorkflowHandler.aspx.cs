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
using App.Entities;

namespace App.Pages
{
    [UI("操作历史表单")]
    [Auth(AuthLogin = true)]
    [Param("key", "新增态必须有这个参数", false)]
    [Param("status", "当前状态")]
    [Param("nextStatus", "想选择的状态")]
    public partial class WorkflowHandler : FormPage<History>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Auth.ViewPower   = Asp.GetQuery<Powers>("pv") ?? Powers.OrderView;
            this.Auth.NewPower    = Asp.GetQuery<Powers>("pn") ?? Powers.OrderNew;
            this.Auth.EditPower   = Asp.GetQuery<Powers>("pe") ?? Powers.OrderEdit;
            this.Auth.DeletePower = Asp.GetQuery<Powers>("pd") ?? Powers.OrderDelete;

            InitForm(this.form2, this.Panel1);
            if (!IsPostBack)
            {
                pbUser.UrlTemplate = Urls.GetUsersUrl(null, null, false);
                ShowForm();
            }
        }

        /// <summary>根据健值显示合适的下拉框</summary>
        /// <param name="key">键值</param>
        /// <param name="status">当前状态（此处没用到，Order里面有了。先保留吧，多当前状态工作流有用。）</param>
        /// <param name="nextStatus">后继想选择的状态</param>
        private void BindDDLStatus(string key)
        {
            var status = Asp.GetQueryLong("status");
            var nextStatus = Asp.GetQueryLong("nextStatus");

            //
            var order = GetOrder(key);
            var steps = order?.GetNextSteps(Common.LoginUser);
            if (steps == null || steps.Count == 0)
            {
                this.btnSave.Hidden = true;
                this.btnSaveNew.Hidden = true;
            }

            // 显示后继步骤，设置要选择的步骤
            UI.Bind(ddlStatus, steps, t => t.Status, t => t.StatusName, "--请选择--", nextStatus);
            if (nextStatus != null)
                this.ddlStatus.Enabled = false;
        }

        private static Order GetOrder(string key)
        {
            int n = key.LastIndexOf('-');
            var type = key.Substring(0, n);
            var id = key.Substring(n + 1).ParseInt();
            var order = Order.Get(id);
            return order;
        }


        //-----------------------------------------------
        // 数据清空、展示、采集、保存
        //-----------------------------------------------
        // 清空数据
        public override void NewData()
        {
            // 状态下拉框
            var key = Asp.GetQueryString("key");
            if (key.IsEmpty())
            {
                Asp.Fail("缺少 key 参数");
                return;
            }
            BindDDLStatus(key);
            UI.SetValue(this.lblId, "-1");
            UI.SetValue(this.tbKey, key);
            UI.SetValue(this.dpCreateDt, DateTime.Now);
            UI.SetValue(this.pbUser, Common.LoginUser, t=>t.ID, t=>t.NickName);
            UI.SetValue(this.tbUserName, "");
            UI.SetValue(this.tbUserMobile, "");
        }


        // 加载数据
        public override void ShowData(History item)
        {
            var user = DAL.User.Get(item.UserId);
            BindDDLStatus(item.Key);
            UI.SetValue(this.ddlStatus, item.StatusId);
            UI.SetValue(this.lblId, item.ID);
            UI.SetValue(this.tbKey, item.Key);
            UI.SetValue(this.dpCreateDt, item.CreateDt);
            UI.SetValue(this.pbUser, user, t => t.ID, t => t.NickName);
            UI.SetValue(this.tbUserName, item.UserName);
            UI.SetValue(this.tbUserMobile, item.UserMobile);

            this.panDetail.IFrameUrl = Urls.GetResesUrl(this.Mode, item.UniID, "Histories", true);
        }

        // 采集数据
        public override void CollectData(ref History item)
        {
            item.StatusId = UI.GetInt(this.ddlStatus);
            item.Status = UI.GetText(this.ddlStatus);
            item.Key = UI.GetText(this.tbKey);
            item.CreateDt = UI.GetDate(this.dpCreateDt);
            item.UserId = UI.GetLong(this.pbUser);
            item.UserName = UI.GetText(this.tbUserName);
            item.UserMobile = UI.GetText(this.tbUserMobile);
        }

        // 修改订单状态（未完成）
        public override void SaveData(History item)
        {
            item.Save();
            var key = Asp.GetQueryString("key");
            var order = GetOrder(key);
            order.Status = item.StatusId;
            order.StatusName = item.Status;
            order.Save();
            //order.ChangeStatus(item.StatusId);  
        }
    }
}
