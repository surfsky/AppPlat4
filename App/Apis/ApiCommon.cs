using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Net;
using System.ComponentModel;
using System.Text;
using App.Utils;
using App.HttpApi;
using App.Components;
using App.DAL;
using System.IO;
using System.Web.Security;

namespace App.Apis
{
    /// <summary>
    /// 短信消息类别
    /// </summary>
    public enum SmsType : int
    {
        [UI("注册")]         Regist = 0,
        [UI("登陆验证")]     Verify = 1,
        [UI("修改密码")]     ChangePassword = 2,
        [UI("更改用户信息")] ChangeInfo = 3
    }

    /// <summary>
    /// App 类型
    /// </summary>
    public enum AppType : int
    {
        [UI("Web")]          Web = 0,
        [UI("MobileWeb")]    MobileWeb = 1,

        [UI("iOS")]          iOS = 10,
        [UI("Android")]      Android = 11,
        [UI("Windows")]      Windows = 12,
        [UI("Mac")]          Mac = 13,
        [UI("Linux")]        Linux = 14,

        [UI("微信小程序")]   WechatMP = 20,
        [UI("支付宝小程序")] AlipayMP = 21,
        [UI("钉钉小程序")]   DingTalkMP = 22
    }


    [Scope("Base")]
    [Description("通用接口")]
    public class ApiCommon
    {
        [HttpApi("注销并跳到登陆页面")]
        public static void Logout()
        {
            Common.Logout();
            FormsAuthentication.RedirectToLoginPage();
        }

        //--------------------------------------------
        // 图片
        //--------------------------------------------
        [HttpApi("生成验证码图片")]
        public static Image VerifyImage()
        {
            var o = VerifyPainter.Draw(100, 40);
            HttpContext.Current.Session[Common.SESSION_VERIFYCODE] = o.Code;
            return o.Image;
        }

        [HttpApi("生成缩略图", Type=ResponseType.Image)]
        [HttpParam("u", "url。图像地址，支持...~/ 等路径表达式，请先用UrlEncode处理，且路径短于256个字符。")]
        [HttpParam("w", "width")]
        [HttpParam("h", "height")]
        public static Image Thumbnail(string u, int w, int? h=null)
        {
            // 尝试从缓存文件中获取文件
            var cacheCode = string.Format("{0}-{1}-{2}", u, w, h).MD5();
            string cacheFile = string.Format("/Caches/{0}.cache", cacheCode);
            string path = Asp.MapPath(cacheFile);
            if (File.Exists(path))
                return Painter.LoadImage(path);

            // 获取原始文件
            Image img = HttpHelper.GetServerOrNetworkImage(u);

            // 创建缩略图
            IO.PrepareDirectory(path);
            if (w > 1000) w = 1000;
            if (h!= null && h > 1000) h = 1000;
            img = Painter.Thumbnail(img, w, h.Value);
            img.Save(path);
            return img;
        }


        //--------------------------------------------
        // 配置信息
        //--------------------------------------------
        [HttpApi("网站信息", CacheSeconds=60*60)]
        public static APIResult GetSiteInfo()
        {
            var site = SiteConfig.Instance;
            return new
            {
                site.Name,
                site.AbbrName,
                site.Owner,
                site.Copyright,
                site.ICP,
                site.LoginBg,
                site.Logo,
                site.LogoDark,
            }.ToResult();
        }



        //--------------------------------------------
        // 短信
        //--------------------------------------------
        [HttpApi("发送短信验证码", AuthToken = true)]
        [HttpParam("mobile", "手机号码")]
        [HttpParam("type", "短信消息类别")]
        [HttpParam("appType", "App类型")]
        public static APIResult SendSms(string mobile, SmsType type, AppType appType)
        {
            try
            {
                var code = new VerifyCode();
                code.Code = StringHelper.BuildRandomText("0123456789", 6);
                code.CreateDt = DateTime.Now;
                code.ExpireDt = code.CreateDt.Value.AddMinutes(30);
                code.Source = appType.GetTitle();
                code.Mobile = mobile;
                code.Save();

                switch (type)
                {
                    case SmsType.Regist:         AliSmsMessenger.SendSmsRegist(mobile, code.Code); break;
                    case SmsType.Verify:         AliSmsMessenger.SendSmsVerify(mobile, code.Code); break;
                    case SmsType.ChangePassword: AliSmsMessenger.SendSmsChangePassword(mobile, code.Code); break;
                    case SmsType.ChangeInfo:     AliSmsMessenger.SendSmsChangeInfo(mobile, code.Code); break;
                }
                return new APIResult(true, "短信发送成功");
            }
            catch (Exception e)
            {
                Logger.LogDb(appType.GetTitle(), e.Message, "Sms", LogLevel.Error);
                return new APIResult(false, e.Message);
            }
        }

        //--------------------------------------------
        // 辅助
        //--------------------------------------------
        [HttpApi("生成GUID")]
        public static string GUID()
        {
            return Guid.NewGuid().ToString("N");
        }

        [HttpApi("获取枚举信息")]
        [HttpParam("enumType", "枚举类型。如App.DAL.ArticleType")]
        public static APIResult GetEnumInfos(string enumType)
        {
            var type = Reflector.GetType(enumType);
            if (type != null && type.IsEnum)
                return type.GetEnumInfos().ToResult();
            return new APIResult(false);
        }

        [HttpApi("生成二维码图片", Type = ResponseType.Image, CacheSeconds = 600, Example = "/HttpApi/Common/QRCode?text=x&icon=/res/images/defaultuser.png")]
        [HttpParam("text", "文本")]
        [HttpParam("iconUrl", "图标地址，可放置在二维码中间，支持...~/ 等路径表达式")]
        public static Image QrCode(string text, string iconUrl = "")
        {
            throw new NotImplementedException();
            //return Drawer.CreateQrCodeImage(text, iconUrl);
        }

        /*
        [HttpApi("回显", CacheSeconds = 20)]
        public static APIResult Echo(string text)
        {
            return text.ToResult();
        }

        [HttpApi("回显", Type = ResponseType.HTML)]
        public static string Html(string text)
        {
            return string.Format("<h1>{0}</h1>", text);
        }
        */

        //----------------------------------------------
        // 测试
        //----------------------------------------------
        [HttpApi("测试-上传图片", false, AuthVerbs = "POST")]
        [HttpParam("image1", "base64字符串")]
        public static APIResult TestUpload(string image1, string image2 = "", string image3 = "")
        {
            return Uploader.UploadBase64Images("Tests", image1, image2, image3).ToResult(ExportMode.Detail, "上传成功");
        }

        [HttpApi.HttpApi("测试-生成base64图片", Type = ResponseType.ImageBase64)]
        public static Image TestImage()
        {
            return VerifyPainter.Draw(100, 40).Image;
        }
    }
}