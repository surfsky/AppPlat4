using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Data;
using System.ComponentModel;
using App.Components;
using App.DAL;
using App.Wechats;
using App.HttpApi;
using App.Utils;

namespace App.Apis
{
    [Scope("Base")]
    [Description("用户相关数据接口")]
    public class ApiUser
    {
        //----------------------------------------------------
        // 用户 & 部门 & 角色
        //----------------------------------------------------
        /*
        [HttpApi("获取门店工程师列表")]
        public static APIResult GetShopEngineers(long shopId)
        {
            var roleId = RoleEntity.Get("工程师").ID;
            var users = User
                .SearchRole(roleId)
                .Where(u => u.ShopID == shopId)
                .ToList()
                .Cast(t => t.Export(ExportMode.Simple))
                ;
            return users.ToResult();
        }

        [HttpApi("获取门店顾客列表", true)]
        public static APIResult GetShopCustomers(long shopId, string key="", int pageIndex = 0, int pageSize = 10)
        {
            var q = User
                .SearchRole(Role.Employees)
                .Where(u => u.ShopID == shopId)
                ;
            if (key.IsNotEmpty())   q = q.Where(t => t.NickName.Contains(key) || t.Mobile.Contains(key));
            q = q.SortPage(t => t.NickName, true, pageIndex, pageSize);

            var users = q.ToList().Cast(t => t.Export(ExportMode.Simple));
            return users.ToResult();
        }
        */


        //------------------------------------------------
        // 用户信息
        //------------------------------------------------
        [HttpApi("获取用户信息")]
        public static APIResult GetUserInfo(long? userId=null)
        {
            return Common.TryGetUser(userId, Powers.UserView).ToResult();
        }

        [HttpApi("获取用户信息", true)]
        public static APIResult HasPower(Powers power)
        {
            return Common.LoginUser.HasPower(power).ToResult();
        }


        //----------------------------------------------------
        // 注册
        //----------------------------------------------------
        [HttpApi("用户注册（手机号码、短信验证码、初始密码、邀请码）")]
        public static APIResult Regist(string mobile, string password, string smsCode, string inviteCode="")
        {
            // 校验验证码
            var user = App.DAL.User.Get(mobile: mobile);
            CheckVerifySMS(mobile, smsCode);
            CheckMobileExist(null, mobile);

            // 创建客户
            if (password.IsEmpty()) password = SiteConfig.Instance.DefaultPassword;
            user = User.CreateCustomer(null, mobile, password);

            // 添加邀请
            if (!inviteCode.IsEmpty())
            {
                Invite.Add(InviteSource.Web, inviteCode, user.ID, user.Mobile, InviteStatus.Regist);
                Logic.AwardInviteUser(user.ID);
            }

            return new APIResult(true, "注册成功，请登录。");
        }

        //----------------------------------------------------
        // 登录和注销
        //----------------------------------------------------
        // 注销
        [HttpApi("注销")]
        public static APIResult Logout()
        {
            Common.Logout();
            return new APIResult(true, "成功注销");
        }


        // Web 客户端登陆
        [HttpApi("Web 客户端端登陆（含验证码）", AuthTraffic=1)]
        public static APIResult Login(string userName, string password, string verifyImage)
        {
            var user = User.Get(name: userName);
            CheckVerifyImage(verifyImage);
            CheckUserExist(user);

            // 校验账户和密码
            if (PasswordHelper.Compare(user.Password, password))
            {
                Common.LoginSuccess(user, SiteConfig.Instance.CookieHours ?? 24);
                Logger.LogDb("LoginOk", user.NickName, userName);
                LoginLog.Regist(user.ID, "Web 客户端端登陆（含验证码）");
                return new APIResult(true, "登录成功");
            }
            else
            {
                Logger.LogDb("LoginFail", user.NickName, userName, LogLevel.Warn);
                throw new Exception("账户或密码错误");
            }
        }



        [HttpApi("用手机加密码登录")]
        public static APIResult LoginByMobile(string mobile, string password, string verifyCode, string OS)
        {
            var user = User.GetDetail(mobile: mobile);
            CheckVerifyImage(verifyCode);
            CheckUserExist(user);
            CheckPassword(user, password);
            if (!PasswordHelper.Compare(user.Password, password))
            {
                Logger.LogDb("LoginByMobileFail", user.NickName, mobile, LogLevel.Warn);
                throw new Exception("账户或密码错误");
            }
            //
            Common.LoginSuccess(user, SiteConfig.Instance.CookieHours ?? 24);
            Logger.LogDb("LoginByMobileOk", user.NickName, mobile, LogLevel.Info);
            LoginLog.Regist(user.ID, "用手机加密码登录");
            return new APIResult(true, "登录成功");
        }

        [HttpApi("用手机加短信登录")]
        public static APIResult LoginByMsgCode(string mobile, string msgCode)
        {
            var user = User.GetDetail(mobile: mobile);
            CheckVerifySMS(mobile, msgCode);
            CheckUserExist(user);
            Common.LoginSuccess(user, SiteConfig.Instance.CookieHours ?? 24);
            Logger.LogDb("LoginByMobileOk", user.NickName, mobile);
            LoginLog.Regist(user.ID, "用手机加短信登录");
            return new APIResult(true, "登录成功");
        }




        //----------------------------------------------------
        // 修改用户信息
        //----------------------------------------------------
        [HttpApi("修改用户信息", AuthLogin = true)]
        public static APIResult EditUser(string name, string nickName, string email, string realName, string idCard, string birthday, string openId, string photo)
        {
            var user = Common.LoginUser;
            CheckUserExist(user);

            // 基础信息
            user.NickName = nickName;
            user.Email = email;
            user.RealName = realName;
            user.IDCard = idCard;
            user.Birthday = birthday.ParseDate();

            // 图片处理
            if (!string.IsNullOrEmpty(photo))
            {
                var image = Painter.ParseImage(photo);
                if (image == null)
                    return new APIResult(false, "图片格式有误");

                var path = Uploader.GetUploadPath("Users");
                image.Save(HttpContext.Current.Server.MapPath(path));
                user.Avatar = path;
            }
            user.Save();
            return new APIResult(true, "修改成功");
        }


        [HttpApi("修改头像", AuthLogin =true)]
        public static APIResult EditPhoto(long userId, string photo)
        {
            var user = User.Get(id: userId);
            CheckUserExist(user);
            user.Avatar = photo;
            user.Save();
            return new APIResult(true, "修改成功");
        }

        [HttpApi("修改手机", AuthLogin = true)]
        public static APIResult EditMobile(string newMobile, string msgCode, string password)
        {
            var user = Common.LoginUser;
            CheckVerifySMS(newMobile, msgCode);
            CheckUserExist(user);
            CheckMobileExist(user, newMobile);
            CheckPassword(user, password);

            user.Mobile = newMobile;
            user.Save();
            return new APIResult(true, "修改成功");
        }


        [HttpApi("设置手机（绑定）", AuthLogin = true)]
        public static APIResult SetMobile(string mobile, string msgCode, string userName = "", string password = "", long? inviteShopId = null, string inviteUserMobile = "")
        {
            var user = Common.LoginUser;
            // 190701 接口复用，如果msgCode==Site，跳过短信认证
            if (msgCode != SiteConfig.Instance.Name)
                CheckVerifySMS(mobile, msgCode);
            CheckUserExist(user);
            CheckMobileExist(user, mobile);

            // 绑定手机加积分（若原手机为空）
            if (user.Mobile.IsEmpty())
                UserScore.Add(ScoreType.BindMobile, user.ID, 20);

            // 记录用户的手机、姓名、密码
            user = User.Get(id: user.ID);
            user.Mobile = mobile;
            user.RealName = userName;
            if (password.IsNotEmpty()) user.Password = PasswordHelper.CreateDbPassword(password);
            user.Save();

            // 尝试新增注册邀请记录
            var inviteUserId = User.Get(mobile: inviteUserMobile)?.ID;
            var invite = Logic.InviteAndAward(user, inviteUserId, inviteShopId);
            return new APIResult(true, "设置成功", invite?.Export(ExportMode.Detail));
        }




        //----------------------------------------------------
        // 输入检测（会抛出异常）
        //----------------------------------------------------
        /// <summary>校验验证图片（失败会抛出异常）</summary>
        private static void CheckVerifyImage(string verifyCode)
        {
            if (!Common.CheckVerifyImage(verifyCode))
                throw new Exception("验证码错误");
        }

        /// <summary>检测短信验证码是否正确（失败会抛出异常）</summary>
        private static void CheckVerifySMS(string mobile, string smsCode)
        {
            var sts = Common.CheckVerifySMS(mobile, smsCode);
            if (sts != VerifyCodeStatus.Ok)
                throw new Exception("验证码" + sts.GetTitle());
        }

        /// <summary>检测用户是否存在（失败会抛出异常）</summary>
        private static void CheckUserExist(User user)
        {
            if (user == null || user.InUsed != true)
                throw new Exception("用户不存在");
        }

        /// <summary>校验密码（失败会抛出异常）</summary>
        private static void CheckPassword(User user, string password)
        {
            if (!PasswordHelper.Compare(user.Password, password))
                throw new Exception("密码错误");
        }

        /// <summary>检测手机是否被他人使用</summary>
        private static void CheckMobileExist(User user, string mobile)
        {
            var mobileUser = User.Get(mobile: mobile);
            if (mobileUser == null)
                return;

            // 如果另外一个用户处于不在用状态，清空其手机和openid
            if (mobileUser.InUsed == false)
            {
                mobileUser.Mobile = "";
                mobileUser.WechatMPID = "";
                mobileUser.Save();
                return;
            }

            // 如果该手机号给另外一个在用用户使用了
            if (user?.ID != mobileUser.ID)
            {
                //throw new Exception("号码已使用");
                // 若该用户微信OpenId也有了，则提示错误
                if (mobileUser.WechatMPID.IsNotEmpty())
                    throw new Exception("号码已使用");
                else
                {
                    // 若用户是通过网页方式注册的（微信OpenId为空）
                    // 尝试合并用户（先尝试物理删除网页注册的用户，如果不成功则逻辑删除）
                    try
                    {
                        mobileUser.Delete();
                    }
                    catch
                    {
                        mobileUser.InUsed = false;
                        mobileUser.Save();
                    }
                }
            }
        }
    }
}