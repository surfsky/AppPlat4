using System;
using System.Data.Entity;
using System.Linq;
using FineUIPro;
using App.DAL;
using App.Utils;
using App.Controls;
using System.Collections;
using System.Collections.Generic;
using App;
using System.Web;
using App.Components;

namespace App.Admins
{
    [UI("用户")]
    [Auth(Powers.UserView, Powers.UserNew, Powers.UserEdit, Powers.UserDelete)]
    public partial class UserForm : FormPage<User>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitForm(this.form2, this.form2);
            if (!IsPostBack)
            {
                //UI.BindTree(ddlDept, DAL.Dept.All, t => t.ID, t => t.Name, "--选择部门--", null, null);
                UI.Bind(cblTitle, DAL.Title.Set, t => t.ID, t => t.Name, null);
                UI.Bind(cblRole, Role.All, t => t.ID, t => t.Name, null);
                UI.SetText(uploader, SiteConfig.Instance.SizeBigImage);
                ShowForm();
                UI.SetEnable(cblRole, Powers.Admin);
            }
        }


        //----------------------------------------------------
        // 重载方法
        //----------------------------------------------------
        public override User GetData(long id)
        {
            return DAL.User.GetDetail(id, inUsed:null);
        }

        // 新建
        public override void NewData()
        {
            this.tbName.Text = "";
            this.tbRealName.Text = "";
            this.tbNickName.Text = "";
            this.tbEmail.Text = "";
            this.tbMobile.Text = "";
            this.tbPhone.Text = "";
            this.tbQQ.Text = "";
            this.tbWechat.Text = "";
            this.tbRemark.Text = "";
            this.cbEnabled.Checked = true;
            this.tbName.Readonly = false;
            this.imgPhoto.ImageUrl = SiteConfig.Instance.DefaultUserImage;
            //this.ddlDept.SelectedValue = "";
            UI.SetValue(pbDept, null);
            this.lblRegistDt.Text = "";
            this.tbIdCard.Text = "";
            this.dpBirthday.Text = "";
            this.tbSpecialty.Text = "";
            this.cblTitle.SelectedIndexArray = new int[]{};
            UI.SetText(tbKeywords, "");
        }

        // 编辑
        public override void ShowData(User item)
        {
            // 编辑权限控制
            if (item.Name == "admin" && AuthHelper.GetLoginUserName() != "admin")
            {
                Alert.Show("你无权查看和编辑超级管理员！", String.Empty, ActiveWindow.GetHideReference());
                return;
            }
            if (this.Mode == PageMode.View)
                this.uploader.Hidden = true;
            UI.SetEditable(tbName, Powers.Admin);

            // 实体数据
            UI.SetText(this.tbName, item.Name);
            UI.SetText(this.tbRealName, item.RealName);
            UI.SetText(this.tbNickName, item.NickName);
            UI.SetText(this.tbEmail, item.Email);
            UI.SetText(this.tbMobile, item.Mobile);
            UI.SetText(this.tbPhone, item.Tel);
            UI.SetText(this.tbQQ, item.QQ);
            UI.SetText(this.tbWechat, item.Wechat);
            UI.SetText(this.tbRemark, item.Remark);
            UI.SetValue(this.cbEnabled, item.InUsed);
            UI.SetValue(this.ddlGender, item.Gender);
            UI.SetValue(this.imgPhoto, item.Avatar, false, SiteConfig.Instance.DefaultUserImage);
            UI.SetText(this.lblRegistDt, item.CreateDt.ToString());
            UI.SetText(this.tbIdCard, item.IDCard);
            UI.SetValue(this.dpBirthday, item.Birthday);
            UI.SetText(this.tbSpecialty, item.Specialty);
            UI.SetText(this.tbKeywords, item.Keywords);
            //UI.SetValue(this.ddlDept, item.DeptID);

            // 职务、角色、管辖部门
            UI.SetValues(this.cblTitle, item.Titles?.Cast(t => t.ID));
            UI.SetValues(this.cblRole, item.RoleIds?.CastLong());
            UI.SetValues(this.pbManageDepts, item.GetManageDepts(), t=> t.ID, t=> t.Name);
            UI.SetValue(this.pbDept, item.Dept, t => t.ID, t => t.Name);
        }

        // 采集表单数据
        public override void CollectData(ref User item)
        {
            // 用户名检测
            var name = tbName.Text.Trim();
            if (name.Length <= 3)
                throw new Exception("用户名长度应大于3个字符");
            if (item.Name != name)
            {
                var user = DAL.User.Get(name: name);
                if (user != null)
                    throw new Exception("该用户名已被使用");
            }
            item.Name = name;

            // 手机号码校验
            var mobile = tbMobile.Text.Trim();
            if (mobile.Length <= 8)
                throw new Exception("电话或手机的长度应大于8个字符");
            if (item.Mobile != mobile)
            {
                var user = DAL.User.Get(mobile: mobile);
                if (user != null)
                    throw new Exception("手机号已被注册使用，请更换手机号。");
            }
            item.Mobile = mobile;

            //
            item.RealName = tbRealName.Text.Trim();
            item.NickName = tbNickName.Text.Trim();
            item.Gender = ddlGender.SelectedValue;
            item.Email = tbEmail.Text.Trim();
            item.Mobile = tbMobile.Text.Trim();
            item.Tel = tbPhone.Text.Trim();
            item.QQ = tbQQ.Text.Trim();
            item.Wechat = tbWechat.Text.Trim();
            item.Remark = tbRemark.Text.Trim();
            item.InUsed = cbEnabled.Checked;
            item.Avatar = imgPhoto.ImageUrl;
            item.CreateDt = DateTime.Now;
            item.IDCard = this.tbIdCard.Text;
            item.Birthday = this.dpBirthday.SelectedDate;
            item.Specialty = this.tbSpecialty.Text;
            item.Keywords = UI.GetText(tbKeywords);
            //item.DeptID = UI.GetLong(this.ddlDept);
            item.NamePinYin = item.RealName.ToPinYin();
            item.NamePinYinCap = item.RealName.ToPinYinCap();

            // 如果是新用户设置默认密码
            if (Mode == PageMode.New)
                item.Password = PasswordHelper.CreateDbPassword(SiteConfig.Instance.DefaultPassword);

            // 部门、角色、职务
            item.DeptID = UI.GetLong(this.pbDept);
            item.RoleIds = UI.GetLongs(this.cblRole);
            item.Titles = DAL.Title.GetTitles(UI.GetLongs(this.cblTitle));
            item.SetManagerDepts(UI.GetLongs(pbManageDepts));
        }

        /// <summary>保存后立即生效</summary>
        public override void SaveData(User item)
        {
            item.Save();
            if (item.ID == Common.LoginUser.ID)
                Common.RefreshLoginUser();
        }


        // 图片上传
        protected void uploader_FileSelected(object sender, EventArgs e)
        {
            string imageUrl = UI.UploadFile(uploader, "Users", SiteConfig.Instance.SizeBigImage);
            UI.SetValue(this.imgPhoto, imageUrl, true);
            if (this.Mode == PageMode.Edit)
            {
                var data = this.GetData();
                data.Avatar = UI.GetUrl(this.imgPhoto);
                data.Save();
            }
        }

    }
}
