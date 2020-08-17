using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using DingTalk.Api.Response;
using DingTalk.Api;
using DingTalk.Api.Request;
using App.HttpApi;
using App.Utils;
using App.DAL;
using App.Components;

namespace App.Apis
{
    [Scope("Base")]
    [Description("钉钉接口")]
    public class ApiDing
    {
        [HttpApi("文件上传", Remark="参考 https://ding-doc.dingtalk.com/doc#/dev/frd69q", PostFile=true, AuthLogin=true)]
        [HttpParam("folder", "存储目录，如 Articles")]
        public APIResult Up(string folder)
        {
            var urls = new List<string>();
            var files = Asp.Request.Files;
            for (int i=0; i<files.Count; i++)
            {
                var url = Uploader.UploadFile(files[i], folder, "");
                urls.Add(url);
            }
            return new APIResult(true, "上传成功", urls);
        }

        //---------------------------------------------------
        // 数据接口
        //---------------------------------------------------
        /*
        [HttpApi("获取钉钉用户ID")]
        public APIResult GetUserId(string authCode)
        {
            var user = DingHelper.GetUser(authCode);
            if (user.Errcode != 0)
                return new APIResult(false, "失败", user.Errmsg);
            return new APIResult(true, "成功", user.Userid);
        }
        */


        [HttpApi("钉钉用户登录（企业内部应用）", AuthTraffic=1)]
        [HttpParam("authCode", "钉钉授权码")]
        [HttpParam("authBackend", "是否校验后台登录权限")]
        public APIResult LoginEnterprise(string authCode, bool authBackend=false)
        {
            // 根据 authCode 获取钉钉 userId
            if (authCode.IsEmpty())
                return new APIResult(false, "authCode不能为空");
            var u = DingHelper.GetUser(authCode);
            if (u.Errcode != 0)
                return new APIResult(false, u.Errmsg);
            User user = DingHelper.GetSiteUser(u.Userid);
            if (authBackend && !user.HasPower(Powers.Backend))
                return new APIResult(false, "无后台登陆权限");
            Common.LoginSuccess(user, 999);
            var o = user.ToResult();
            Logger.LogDb("DingLoginOk", o.ToJson());
            LoginLog.Regist(user.ID, "钉钉用户登录（企业内部应用）");
            return o;

            /*
            string[] RoleId = { };
            string[] RoleCode = { };
            string[] RolePower = { };
            OapiDepartmentGetResponse oapiDepartment = null;
            if (user == null)//用户不在平台上的时候
            {
                if (deptId.IsEmpty())//不是手动选择的时候
                {
                    if (dingUser.Department.Count > 1)//部门不是只有一个的时候
                    {
                        List<OapiDepartmentGetResponse> oapiDepartments = new List<OapiDepartmentGetResponse>();
                        foreach (var item in dingUser.Department)
                        {
                            string parentId = "1";
                            List<long> parentIds = DingHelper.GetParentDeptIds(item.ToString()).ParentIds;
                            parentId = parentIds[0].ToString();
                            if (parentIds.Count > 2)
                                parentId = parentIds[parentIds.Count - 2].ToString();//部门的所有上级父部门路径 倒数第二个
                            else if (parentIds.Count > 1)
                                parentId = parentIds[1].ToString();//部门的所有上级父部门路径 倒数第二个 
                            OapiDepartmentGetResponse deptResponse = oapiDepartments.Where(s => s.Id == parentId.ParseLong()).FirstOrDefault();
                            if (deptResponse == null)
                            {
                                var school = DingHelper.GetDepartmentView(parentId);
                                string schoolId = OrganizeApp.ExistenceSchoolName(school);
                                if (oapiDepartments.Where(s => s.Id == schoolId.ParseLong()).Count() <= 0)
                                    oapiDepartments.Add(school);
                            }
                        }
                        return new { Result = true, Info = "请选择所在单位", CreateDt = DateTime.Now, Data = new { CheckState = PersonCheckEnum.ChooseOapiDept, OapiUserId = dingUser.Userid, OapiDepartments = oapiDepartments } };//返回钉钉部门集合
                    }
                    else
                    {
                    // 只有一个部门
                        List<long> parentIds = DingHelper.GetParentDeptIds(dingUser.Department[0].ToString()).ParentIds;
                        string parentId = parentIds[0].ToString();
                        if (parentIds.Count > 2)
                            parentId = parentIds[parentIds.Count - 2].ToString();//部门的所有上级父部门路径 倒数第二个
                        else if (parentIds.Count > 1)
                            parentId = parentIds[1].ToString();//部门的所有上级父部门路径 倒数第二个 
                        oapiDepartment = DingHelper.GetDepartmentView(parentId);
                    }
                }
                else
                {
                    oapiDepartment = DingHelper.GetDepartmentView(deptId);
                }
            }


            if (dingMobile.IsEmpty())
            {
                CheckState = PersonCheckEnum.NotPhone;
            }
            else if (user == null)
            {
                user = new User();
                user.Id = Guid.NewGuid().ToString();
                user.CheckState = 0;
                user.Account = dingMobile;
                user.RealName = dingUser.Name;
                user.OapiUserId = dingUser.Userid;
                user.NickName = dingUser.Name;
                user.Gender = true;
                user.InUsed = true;
                user.EnabledMark = true;
                user.MobilePhone = dingUser.Mobile;
                user.IsAdministrator = false;
                user.CreatorTime = DateTime.Now;
                user.Types = "1";
                user.OapiDeptId = oapiDepartment.Id.ToString();
                OrganizeModel organizeModel = OrganizeApp.GetList(s => s.EnabledMark == true).Where(s => !s.DingDingDepartmentIds.IsNullOrEmpty()).Where(s => s.DingDingDepartmentIds.Split(',').ToList().Where(x => !x.IsNullOrEmpty()).ToList().Contains(user.OapiDeptId)).FirstOrDefault();
                if (organizeModel == null)//根据钉钉部门Id，找不到对应部门，直接全名称比对
                    organizeModel = OrganizeApp.GetList(s => s.EnabledMark == true).Where(s => s.FullName == oapiDepartment.Name).FirstOrDefault();
                user.OrganizeId = organizeModel?.Id;
                if (!user.OrganizeId.IsNullOrEmpty())
                {
                    var roles = RoleApp.GetList(s => s.OrganizeId == user.OrganizeId && s.EnabledMark == true).ToList();
                    string powerValue = "";
                    if (organizeModel.OrganizeCategoryCode == CoreEntity.OrganizeCategory003 || organizeModel.OrganizeCategoryCode == CoreEntity.OrganizeCategory004)
                        powerValue = (Convert.ToInt32(PowerType.SecurityPersonnel)).ToString();
                    else if (organizeModel.OrganizeCategoryCode == CoreEntity.OrganizeCategory002)
                        powerValue = (Convert.ToInt32(PowerType.CountyEducationPersonnel)).ToString();
                    else if (organizeModel.OrganizeCategoryCode == CoreEntity.OrganizeCategory001)
                        powerValue = (Convert.ToInt32(PowerType.EducationPersonnel)).ToString();

                    roles = roles.Where(s => !s.Powers.IsNullOrEmpty()).Where(s => s.Powers.Split(',').ToList().Where(x => !x.IsNullOrEmpty()).ToList().Contains(powerValue)).ToList();
                    if (roles != null && roles.Count() > 0)
                    {
                        roles = roles.OrderBy(s => s.Powers?.Split(',').ToList().Count()).ToList();
                        string[] roleId = { roles[0].Id };
                        //UserModel mobileUser = UserApp.Get(s => s.Account == responeseMobile && s.InUsed == true);
                        //if (mobileUser.EnabledMark != null && mobileUser.EnabledMark == false)
                        //    return FailResult($"对不起，当前手机号在平台上已被关闭，请联系管理员");
                        UserApp.SubmitForm(user, new UserLogOn() { UserPassword = "123456" }, roleId, "");

                        var userModelId = UserApp.Find(m => m.MobilePhone == dingMobile).Id;
                        user = UserApp.Get(userModelId);
                        UserAuthApp.InsertToSuccess(user: user, type: FromType.Ding);
                    }
                    else
                    {
                        UserAuth userAuth = new UserAuth();
                        userAuth.Name = user.RealName;
                        userAuth.Mobile = user.MobilePhone;
                        userAuth.OrganizeId = oapiDepartment.Id.ToString();
                        userAuth.OrganizeName = oapiDepartment.Name;
                        userAuth.DingTalkOpenId = dingUser.Userid;
                        userAuth.CreateDt = DateTime.Now;
                        userAuth.CreatorTime = DateTime.Now;
                        userAuth.Id = Guid.NewGuid().ToString();
                        userAuth.FromType = FromType.Ding;
                        userAuth.Remark = "相关学校未配置角色";
                        UserAuthApp.Insert(userAuth);
                        return FailResult($"号码{dingUser.Mobile}未匹配相应[角色]配置，请联系{organizeModel.FullName}管理员");
                    }
                }
                else
                {
                    UserAuth userAuth = new UserAuth();
                    userAuth.Name = user.RealName;
                    userAuth.Mobile = user.MobilePhone;
                    userAuth.OrganizeId = oapiDepartment.Id.ToString();
                    userAuth.OrganizeName = oapiDepartment.Name;
                    userAuth.DingTalkOpenId = dingUser.Userid;
                    userAuth.CreateDt = DateTime.Now;
                    userAuth.CreatorTime = DateTime.Now;
                    userAuth.Id = Guid.NewGuid().ToString();
                    userAuth.FromType = FromType.Ding;
                    userAuth.Remark = "匹配不到学校";
                    UserAuthApp.Insert(userAuth);
                    return FailResult($"对不起，您所在的学校/组织尚未接入平安校园系统,请联系教育局相关人员");
                }
            }
            else
            {
                user.OapiUserId = dingUser.Userid;
                user.CheckState = 0;
                UserApp.Update(user);
            }

            RoleId = user.RoleUsers.Select(l => l.RoleId).ToArray();
            var UserInfo = new OapiUserInfo()
            {
                OrganizeCategoryCode = user.Organize?.OrganizeCategory?.EnCode,
                OrganizeCategoryName = user.Organize?.OrganizeCategory?.Name,
                OrganizeId = user.OrganizeId,
                AccountId = user.Id,
                CheckState = user.CheckState,
                CheckStateName = CoreEntity.GetPersonCheckState(user.CheckState),
                OrganizeName = user.Organize?.FullName,
                RoleId = RoleId,
                Account = user.Account,
                PersonName = user.Account,
                MemberView = dingUser.MemberView,
                Name = dingUser.Name,
                Active = dingUser.Active,
                Avatar = dingUser.Avatar,
                Body = dingUser.Body,
                Department = dingUser.Department,
                DingId = dingUser.DingId,
                Email = dingUser.Email,
                Errcode = dingUser.Errcode,
                ErrCode = dingUser.ErrCode,
                Errmsg = dingUser.Errmsg,
                ErrMsg = dingUser.ErrMsg,
                Extattr = dingUser.Extattr,
                HiredDate = dingUser.HiredDate,
                InviteMobile = dingUser.InviteMobile,
                IsAdmin = dingUser.IsAdmin,
                IsBoss = dingUser.IsBoss,
                IsCustomizedPortal = dingUser.IsCustomizedPortal,
                IsHide = dingUser.IsHide,
                IsLeaderInDepts = dingUser.IsLeaderInDepts,
                IsLimited = dingUser.IsLimited,
                IsSenior = dingUser.IsSenior,
                Jobnumber = dingUser.Jobnumber,
                ManagerUserId = dingUser.ManagerUserId,
                Mobile = dingUser.Mobile,
                MobileHash = dingUser.MobileHash,
                Nickname = dingUser.Nickname,
                OpenId = dingUser.OpenId,
                OrderInDepts = dingUser.OrderInDepts,
                OrgEmail = dingUser.OrgEmail,
                Position = dingUser.Position,
                Remark = dingUser.Remark,
                Roles = dingUser.Roles,
                StateCode = dingUser.StateCode,
                SubErrCode = dingUser.SubErrCode,
                SubErrMsg = dingUser.SubErrMsg,
                Tel = dingUser.Tel,
                Unionid = dingUser.Unionid,
                Userid = dingUser.Userid,
                WorkPlace = dingUser.WorkPlace,
            };
            var powers = new List<int>();
            var roleIds = user.RoleUsers.Select(s => s.RoleId).ToList();
            RolePower = RoleApp.GetList(s => roleIds.Contains(s.Id)).Select(l => l.Powers).ToArray();
            foreach (var item in RolePower)
            {
                var array = item.ToStringArray().ToIntList();
                foreach (var subItem in array)
                {
                    if (!powers.Contains(subItem))
                    {
                        powers.Add(subItem);
                    }
                }
            }
            var powertypes = typeof(PowerType).ToList();
            UserInfo.AppRights = powertypes
                .Where(m => powers.Contains(m.ID))
                .Select(m => new AppRightInt()
                {
                    Id = m.ID,
                    Name = m.Name,
                    Info = m.Info,
                    Value = m.Value.ToString()
                }).ToList();
            UserInfo.RightValues = UserInfo.AppRights.Select(l => l.Value).ToArray();
            return SucessResult(UserInfo);
            */
        }

    }
}