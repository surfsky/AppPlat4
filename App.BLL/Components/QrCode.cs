using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using App.Utils;

namespace App.Components
{
    /// <summary>二维码类别</summary>
    public enum QrCodeType : int
    {
        [UI("用户信息")] UserInfo = 0,
        [UI("用户签到")] UserSign = 3,
        [UI("商店信息")] ShopInfo = 10,
        [UI("邀请")] Invite = 11,
    }

    /// <summary>
    /// 二维码数据（不包含地址信息，由客户端自行控制跳转）
    /// </summary>
    public class QrCodeData
    {
        [UI("类型")]     public QrCodeType Type { get; set; }
        [UI("过期时间")] public DateTime? ExpireDt { get; set; }
        [UI("健值")]     public string Key { get; set; }
        [UI("标题 ")]    public string Title { get; set; }

        // 加密密钥
        static string _key = "q1A2w6Dx";

        /// <summary>构造方法</summary>
        public QrCodeData() { }
        public QrCodeData(QrCodeType type, string key, string title, DateTime? expireDt=null)
        {
            Type = type;
            Key = key;
            Title = title;
            ExpireDt = expireDt;
        }

        /// <summary>转化为字符串（并加密）</summary> 
        public override string ToString()
        {
            var json = JsonConvert.SerializeObject(this);
            return json.DesEncrypt(_key);
            //return DESEncrypt.EncryptDES(json, _key);
        }

        /// <summary>转化为二维码对象（并预先解密）</summary> 
        public static QrCodeData Parse(string text)
        {
            try
            {
                //var json = DESEncrypt.DecryptDES(text, _key);
                var json = text.DesDecrypt(_key);
                return JsonConvert.DeserializeObject<QrCodeData>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}