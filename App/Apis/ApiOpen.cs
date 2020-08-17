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

namespace App.Apis
{
    //---------------------------------------------
    // Token 机制
    // 1. 获取动态token
    // 2. 访问需要健权的接口，带上token参数
    // 3. 服务器端统一在 global 里面做token校验
    //---------------------------------------------
    [Scope("Base")]
    [Description("开放平台接口")]
    public class ApiOpen
    {
        //--------------------------------------------------
        // 方式1：appKey+appSecret->Token
        //--------------------------------------------------
        [HttpApi("GetToken")]
        public APIResult GetToken(string appKey, string appSecret)
        {
            var token = DAL.OpenApp.CreateToken(appKey, appSecret, 60 * 2);
            if (token.IsEmpty())
                return new APIResult(false, "获取失败", token);
            else
                return new APIResult(true, "获取成功", token);
        }


        //--------------------------------------------------
        // 方式2：appKey->Code->Token
        //--------------------------------------------------
        [HttpApi("GetToken")]
        public APIResult GetCode(string appKey)
        {
            var code = appKey.DesEncrypt("12345678");
            return new APIResult(true, "创建成功", code);
        }
        [HttpApi("GetToken")]
        public APIResult GetTokenByCode(string code, string appSecret)
        {
            var appKey = code.DesDecrypt("12345678");
            var token = DAL.OpenApp.CreateToken(appKey, appSecret, 60 * 2);
            return new APIResult(true, "创建成功", code);
        }


        //--------------------------------------------------
        // 测试接口（需要token）
        //--------------------------------------------------
        [HttpApi("GetDate(NeedToken)", AuthToken = true)]
        public APIResult GetDate()
        {
            var now = DateTime.Now;
            return now.ToString().ToResult();
        }
    }
}