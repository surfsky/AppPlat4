using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using App.Components;
using App.Utils;
using Newtonsoft.Json;
using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using App.DAL;

namespace App.Components
{
    /// <summary>
    /// 钉钉辅助方法
    /// </summary>
    public partial class DingHelper
    {
        /// <summary>获取本系统用户（根据钉钉用户ID），若不存则新建</summary>
        public static User GetSiteUser(string dingUserId)
        {
            // 根据钉钉 userId 获取钉钉用户的详细信息（主要是手机）
            // 去本地数据库根据手机查找用户
            var dingUser = DingHelper.GetUserInfo(dingUserId);
            var user = User.Search(inUsed: true, mobile: dingUser.Mobile).FirstOrDefault();
            Logger.LogDb("Ding", new { dingUser, siteUser = user?.Export() }.ToJson());

            // 如果数据库不存该用户，创建该用户
            if (user == null)
            {
                user = new User();
                user.DingUserID = dingUserId;
                user.DingMPID = dingUser.OpenId;
                user.DingUnionID = dingUser.Unionid;
                user.Name = dingUser.Mobile;
                user.RealName = dingUser.Name;
                user.NickName = dingUser.Nickname.IsEmpty() ? dingUser.Name : dingUser.Nickname;
                user.Avatar = dingUser.Avatar;
                user.InUsed = true;
                user.Mobile = dingUser.Mobile;
                user.CreateDt = DateTime.Now;
                user.HireDt = dingUser.HiredDate.ParseDate();
                user.Remark = dingUser.Remark;
                user.Title = dingUser.Position;
                user.Tel = dingUser.Tel;
                user.Source = "Ding";
            }
            if (user.DeptID == null)
                user.DeptID = dingUser.Department?.FirstOrDefault();
            user.Save();


            // 返回本系统用户信息
            return user;
        }

        /// <summary>获取token</summary>
        public static OapiGettokenResponse GetAccessToken()
        {
            try
            {
                var ding = AliDingConfig.Instance;
                var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/gettoken");
                var req = new OapiGettokenRequest();
                req.Appkey = ding.MPAppKey;
                req.Appsecret = ding.MPAppSecret;
                req.SetHttpMethod("GET");
                var rsp = client.Execute(req);
                return rsp;
            }
            catch
            {
                return null;
            }
        }

        //--------------------------------------------------------
        // 用户及部门
        //--------------------------------------------------------
        /// <summary>获取用户ID</summary>
        public static OapiUserGetuserinfoResponse GetUser(string authCode)
        {
            var accessToken = DingHelper.GetAccessToken();
            var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/user/getuserinfo");
            var req = new OapiUserGetuserinfoRequest();
            req.Code = authCode;
            req.SetHttpMethod("GET");
            return client.Execute(req, accessToken.AccessToken);
        }

        /// <summary>用户详情</summary>
        /// <param name="userId">钉钉UserId</param>
        public static OapiUserGetResponse GetUserInfo(string userId)
        {
            var accessToken = GetAccessToken();
            var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/user/get");
            var request = new OapiUserGetRequest();
            request.Userid = userId;
            request.SetHttpMethod("GET");
            return client.Execute(request, accessToken.AccessToken);
        }

        /// <summary>查询部门的所有上级父部门路径</summary>
        /// <param name="deptId">希望查询的部门的id，包含查询的部门本身</param>
        public static OapiDepartmentListParentDeptsByDeptResponse GetParentDeptIds(string deptId)
        {
            var accessToken = GetAccessToken();
            var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/department/list_parent_depts_by_dept");
            var request = new OapiDepartmentListParentDeptsByDeptRequest();
            request.Id = (deptId);
            request.SetHttpMethod("GET");
            return client.Execute(request, accessToken.AccessToken);
        }

        /// <summary>获取部门详情.https://ding-doc.dingtalk.com/doc#/serverapi2/dubakq/5bf960de</summary>
        public static OapiDepartmentGetResponse GetDepartment(string id)
        {
            var accessToken = GetAccessToken();
            var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/department/get");
            var request = new OapiDepartmentGetRequest();
            request.Id = id;
            request.SetHttpMethod("GET");
            return client.Execute(request, accessToken.AccessToken);
        }

        /// <summary>获取子部门清单.https://ding-doc.dingtalk.com/doc#/serverapi2/dubakq</summary>
        /// <param name="parentId">父部门id（如果不传，默认部门为根部门，根部门ID为1）</param>
        /// <param name="fetchChild">是否递归部门的全部子部门</param>
        /// <returns></returns>
        public static OapiDepartmentListResponse GetDepartments(string parentId, bool fetchChild = true)
        {
            var accessToken = GetAccessToken();
            var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/department/list");
            var request = new OapiDepartmentListRequest();
            request.Id = parentId;
            request.FetchChild = fetchChild;
            request.SetHttpMethod("GET");
            return client.Execute(request, accessToken.AccessToken);
        }


        /// <summary>创建用户(https://ding-doc.dingtalk.com/doc#/serverapi2/ege851/b6a05ccd)</summary>
        /// <remarks>更复杂的参数请查看源文档</remarks>
        /// <param name="userId">员工在当前企业内的唯一标识（如工号），如果不传，服务器将自动生成一个userid。</param>
        public static OapiUserCreateResponse CreateUser(string userId, string name, string mobile, long deptId)
        {
            var accessToken = GetAccessToken();
            var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/user/create");
            var request = new OapiUserCreateRequest();
            request.Userid = userId;
            request.Mobile = mobile;
            request.Name = name;
            request.Department = deptId.AsList().ToJson();
            return client.Execute(request, accessToken.AccessToken);
        }


        /// <summary>修改用户信息</summary>
        public static OapiUserUpdateResponse UpdateUser(string userId, string name, string mobile, long deptId)
        {
            var accessToken = GetAccessToken();
            var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/user/update");
            var request = new OapiUserUpdateRequest();
            request.Userid = userId;
            request.Name = name;
            request.Mobile = mobile;
            request.Department = deptId.AsList();
            return client.Execute(request, accessToken.AccessToken);
        }



    }
}