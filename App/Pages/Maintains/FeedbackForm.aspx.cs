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
using App.Components;
using App.Utils;

namespace App.Admins
{
    [UI("反馈")]
    [Auth(Powers.FeedBackView, Powers.FeedBackNew, Powers.FeedBackEdit, Powers.FeedBackDelete)]
    public partial class FeedbackForm : FormPage<Feedback>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(form2, this.form2, null, null, null, false);
            if (!IsPostBack)
            {
                UI.BindEnum(ddlType, typeof(FeedType));
                UI.BindEnum(ddlApp, typeof(FeedApp));
                UI.BindEnum(ddlStatus, typeof(FeedbackStatus));
                ShowForm();
            }
        }

        // 新建
        public override void NewData()
        {
            UI.SetValue(lblId, "-1");
            UI.SetValue(lblCreateDt, DateTime.Now);
            UI.SetValue(lblUpdateDt, "");
            UI.SetValue(ddlType, FeedType.Bug);
            UI.SetValue(ddlStatus, FeedbackStatus.Create);
            UI.SetValue(ddlApp, FeedApp.Web);
            UI.SetValue(tbAppVersion, Global.Version);
            UI.SetValue(tbAppModule, "");
            UI.SetValue(tbUser, Common.LoginUser.NickName);
            UI.SetValue(tbContacts, Common.LoginUser.Mobile);
            UI.SetValue(tbTitle, "");
            UI.SetValue(tbContent, "");
            UI.SetValue(tbReply, "");

            // 新建状态
            UI.SetValue(ddlStatus, FeedbackStatus.Create);
            SwitchUI(null);
        }

        // 显示
        public override void ShowData(Feedback item)
        {
            UI.SetValue(lblId, item.ID);
            UI.SetValue(lblCreateDt, item.CreateDt);
            UI.SetValue(lblUpdateDt, item.UpdateDt);
            UI.SetValue(ddlType, item.Type);
            UI.SetValue(ddlStatus, item.Status);
            UI.SetValue(ddlApp, item.App);
            UI.SetValue(tbAppVersion, item.AppVersion);
            UI.SetValue(tbAppModule, item.AppModule);
            UI.SetValue(tbUser, item.User);
            UI.SetValue(tbContacts, item.Contacts);
            UI.SetValue(tbTitle, item.Title);
            UI.SetValue(tbContent, item.Content);
            UI.SetValue(tbReply, item.Reply);
            UI.SetValue(up1, item.Image1);
            UI.SetValue(up2, item.Image2);
            UI.SetValue(ddlStatus, item.Status);

            // 显示当前状态和后继操作步骤
            var steps = item.GetNextSteps(Common.LoginUser);
            UI.Bind(ddlNextStep, steps, t => t.Status, t => t.Action);
            if (steps.Count == 0)
            {
                this.ddlNextStep.Hidden = true;
                this.btnOperate.Hidden = true;
            }

            SwitchUI(item);
        }

        // 控件可用性控制
        private void SwitchUI(Feedback item)
        {
            if (item == null)
            {
                UI.SetVisible(false, this.ddlStatus, btnOperate, btnHistory, formReply);
            }
            else
            {
                UI.SetEnable(false, this.formBase);
                this.btnSave.Visible = false; //.Hidden = true;
                this.formReply.Hidden = (item.Status == FeedbackStatus.Create || item.Status == FeedbackStatus.Dispatched);
                this.formReply.Enabled = (item.Status == FeedbackStatus.Handling);
            }
        }

        // 采集
        public override void CollectData(ref Feedback item)
        {
            if (this.Mode == PageMode.New)
                item.UserID = Common.LoginUser.ID;

            item.Type = UI.GetEnum<FeedType>(ddlType);
            item.Status = UI.GetEnum<FeedbackStatus>(ddlStatus);
            item.App = UI.GetEnum<FeedApp>(ddlApp);
            item.AppVersion = UI.GetText(tbAppVersion);
            item.AppModule = UI.GetText(tbAppModule);
            item.User = UI.GetText(tbUser);
            item.Contacts = UI.GetText(tbContacts);
            item.Title = UI.GetText(tbTitle);
            item.Content = UI.GetText(tbContent);
            item.Reply  = UI.GetText(tbReply);
            item.Image1 = UI.GetUrl(up1);
            item.Image2 = UI.GetUrl(up2);
        }


        // 图片上传
        protected void uploader_FileSelected(object sender, EventArgs e)
        {
            if (this.Mode == PageMode.Edit)
            {
                var data = this.GetData();
                data.Image1 = UI.GetUrl(this.up1);
                data.Image2 = UI.GetUrl(this.up2);
                data.Save();
            }
        }

        //----------------------------------------------
        // 流程处理
        //----------------------------------------------
        /// <summary>处理</summary>
        protected void btnOperate_Click(object sender, EventArgs e)
        {
            this.winFlow.Hidden = false;
        }

        // 显示处理历史
        protected void btnHistory_Click(object sender, EventArgs e)
        {
            var item = this.GetData();
            if (item != null)
            {
                string url = Urls.GetHistoriesUrl(item.UniID, (int)item.Type);
                UI.ShowWindow(this.winHistory, url, "处理历史", 800, 600, CloseAction.Hide);
            }
        }


        /// <summary>选择后继操作步骤后，列出可处理该任务的人员</summary>
        protected void ddlNextStep_SelectedIndexChanged(object sender, EventArgs e)
        {
            var nextStatus = UI.GetEnum<FeedbackStatus>(ddlNextStep);
            if (nextStatus != null)
            {
                var nextStep = Feedback.Flow.GetStep((int)nextStatus.Value);
                var power = nextStep.Power;
                pbNextUser.UrlTemplate = Urls.GetUsersUrl(null, power);
                if (nextStep.Type == WFStepType.End)
                {
                    pbNextUser.Hidden = true;
                    dtNextDt.Hidden = true;
                }
            }
        }

        /// <summary>提交处理流程</summary>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // 后继操作
            var nextStatus = UI.GetEnum<FeedbackStatus>(ddlNextStep);
            if (nextStatus == null)
            {
                UI.ShowAlert("请选择下步操作");
                return;
            }

            // 指定后继处理人（可选，未启用）
            DAL.User nextUser = null;
            var nextUserId = UI.GetLong(pbNextUser);
            if (nextUserId != null)
                nextUser = DAL.User.Get(nextUserId);
            var nextDt = UI.GetDate(dtNextDt);

            // 提交变更
            var remark = UI.GetText(tbComment);
            var item = this.GetData();
            item.Save();
            item.ChangeStatus(nextStatus.Value, Common.LoginUser, remark);
            item.SetNextProcessor(nextUser, nextDt);
            this.winFlow.Hidden = true;
            this.ShowData(item);
        }
    }
}
