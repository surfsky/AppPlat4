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

namespace App.Pages
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
                UI.Bind(cblRole, Role.All, t => t.ID, t => t.Name, null);
                UI.BindTree(ddlDept, DAL.Dept.All, t => t.ID, t => t.Name, "--选择部门--", null, null);
                UI.BindTree(ddlArea, Area.All, t => t.ID, t => t.Name);
                UI.Bind(cblTitle, DAL.Title.Set, t => t.ID, t => t.Name, null);
                UI.SetText(uploader, SiteConfig.Instance.SizeBigImage);
                ShowForm();

                // 仅管理员可修改角色、归属部门、邀请者信息
                UI.SetEnable(cblRole, Powers.Admin);
                UI.SetEnable(pbShop, Powers.Admin);
                UI.SetEnable(pbInviter, Powers.Admin);
                UI.SetEnable(ddlArea, Powers.Admin);
                UI.SetVisible(panAdmin, Powers.Admin);
            }
        }



        //----------------------------------------------------
        // 重载方法
        //----------------------------------------------------
        // 新建
        public override void NewData()
        {
            UI.SetValue(tbName, "");
            UI.SetValue(tbRealName, "");
            UI.SetValue(tbNickName, "");
            UI.SetValue(tbEmail, "");
            UI.SetValue(tbMobile, "");
            UI.SetValue(tbPhone, "");
            UI.SetValue(tbQQ, "");
            UI.SetValue(tbWechat, "");
            UI.SetValue(tbRemark, "");
            UI.SetValue(cbEnabled, true);
            UI.SetValue(imgPhoto, SiteConfig.Instance.DefaultUserImage);
            UI.SetValue(lblRegistDt, "");
            UI.SetValue(tbIdCard, "");
            UI.SetValue(dpBirthday, "");
            UI.SetValue(tbSpecialty, "");
            UI.SetValue(pbShop, "");
            UI.SetValue(pbInviter, "");
            UI.SetValue(ddlDept, null);
            UI.SetValue(ddlArea, null);
            UI.SetValues(cblTitle, new List<long>());
            UI.SetValues(cblRole, new List<long>());
            UI.SetValue(tbTitle, "");
            UI.SetValue(tbWechatWebId, "");
            UI.SetValue(tbWechatMPId, "");
            UI.SetValue(tbWechatUnionId, "");
            UI.SetValue(tbWechatMPSessionKey, "");
            UI.SetValue(tbWechatWebSessionKey, "");
            UI.SetValue(tbLastGPS, "");
            tbName.Readonly = false;
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
            UI.SetEditable(tbName, Powers.Admin);  // 仅管理员可修改用户名

            // 实体数据
            UI.SetValue(tbName, item.Name);
            UI.SetValue(tbRealName, item.RealName);
            UI.SetValue(tbNickName, item.NickName);
            UI.SetValue(tbEmail, item.Email);
            UI.SetValue(tbMobile, item.Mobile);
            UI.SetValue(tbPhone, item.Tel);
            UI.SetValue(tbQQ, item.QQ);
            UI.SetValue(tbWechat, item.Wechat);
            UI.SetValue(tbRemark, item.Remark);
            UI.SetValue(cbEnabled, item.InUsed);
            UI.SetValue(ddlGender, item.Gender);
            UI.SetValue(imgPhoto,  item.Avatar, false, SiteConfig.Instance.DefaultUserImage);
            UI.SetValue(lblRegistDt, item.CreateDt);
            UI.SetValue(tbIdCard, item.IDCard);
            UI.SetValue(dpBirthday, item.Birthday);
            UI.SetValue(tbSpecialty, item.Specialty);
            UI.SetValue(tbTitle, item.Title);
            UI.SetValue(tbWechatWebId, item.WechatOPID);
            UI.SetValue(tbWechatMPId, item.WechatMPID);
            UI.SetValue(tbWechatUnionId, item.WechatUnionID);
            UI.SetValue(tbWechatMPSessionKey, item.WechatMPSessionKey);
            UI.SetValue(tbWechatWebSessionKey, item.WechatOPSessionKey);
            UI.SetValue(tbLastGPS, item.LastGPS);
            UI.SetValue(ddlDept, item.DeptID);
            UI.SetValue(ddlArea, item.AreaID);
            UI.SetValue(this.pbShop, item.Shop, t => t.ID, t => t.AbbrName);
            UI.SetValue(this.pbInviter, item.Inviter, t => t.ID, t => t.NickName);
            UI.SetValues(cblTitle, item.Titles?.Cast(t => t.ID));
            UI.SetValues(cblRole, item.RoleIds?.CastString());

            // 邀请
            var mpPage     = string.Format("/pages/index/index?inviteUserId={0}", item.ID).UrlEncode();
            var openPage    = string.Format("?inviteUserId={0}", item.ID).UrlEncode();
            var mpQrCode   = string.Format("/HttpApi/Wechat/MPQrCode?page={0}&width={1}",  mpPage, 280);
            var openQrCode  = string.Format("/HttpApi/Wechat/OPENQrCode?page={0}&width={1}", openPage, 280);
            this.imgMPInvite.ImageUrl = mpQrCode;
            this.imgWebInvite.ImageUrl = openQrCode;
        }

        // 采集表单数据
        public override void CollectData(ref User item)
       {
            // 用户名检测
            var name = UI.GetText(tbName);
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
            var mobile = UI.GetText(tbMobile);
            if (mobile.IsNotEmpty() && mobile.Length <= 8)
                throw new Exception("电话或手机的长度应大于8个字符");
            if (item.Mobile != mobile)
            {
                var user = DAL.User.Get(mobile: mobile);
                if (user != null)
                    throw new Exception("手机号已被注册使用，请更换手机号。");
            }
            item.Mobile = mobile;


            // 如果是新用户，设置个默认密码
            if (Mode == PageMode.New)
            {
                item.Password = PasswordHelper.CreateDbPassword(SiteConfig.Instance.DefaultPassword);
            }

            //
            item.RealName = UI.GetText(tbRealName);
            item.NickName = UI.GetText(tbNickName);
            item.Gender = UI.GetText(ddlGender);
            item.Email = UI.GetText(tbEmail);
            item.Tel = UI.GetText(tbPhone);
            item.QQ = UI.GetText(tbQQ);
            item.Wechat = UI.GetText(tbWechat);
            item.Remark = UI.GetText(tbRemark);
            item.InUsed = UI.GetBool(cbEnabled);
            item.Avatar = UI.GetUrl(imgPhoto);
            item.IDCard = UI.GetText(this.tbIdCard);
            item.Birthday = UI.GetDate(this.dpBirthday);
            item.Specialty = UI.GetText(this.tbSpecialty);
            item.WechatOPID = UI.GetText(tbWechatWebId);
            item.WechatMPID = UI.GetText(tbWechatMPId);
            item.WechatUnionID = UI.GetText(tbWechatUnionId);
            item.Title = UI.GetText(tbTitle);


            // 部门、角色、职务
            item.ShopID = UI.GetLong(this.pbShop);
            item.InviterID = UI.GetLong(this.pbInviter);
            item.DeptID = UI.GetLong(this.ddlDept);
            item.AreaID = UI.GetLong(this.ddlArea);
            item.RoleIds = UI.GetLongs(this.cblRole);
            item.Titles = DAL.Title.GetTitles(UI.GetLongs(this.cblTitle));

            // 补足区域ID
            item.FixArea();
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
