using App.Components;
using App.Utils;
using App.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Components
{
    /// <summary>
    /// 安全相关方法
    /// </summary>
    public static class Security
    {
        // 加密参数
        public static int _signLength = 10;
        public static string _signKey = SiteConfig.Instance.SignKey;
        public static double _signMinutes = SiteConfig.Instance.SignMinutes.Value;

        /// <summary>创建带签名的URL。用于保护一些未加权限判断的页面，如文件选择窗口。</summary>
        /// <remarks>
        /// 附加的 URL 参数
        /// - ns (nonce string)  : 随机字符串。可确保签名不可预测。同一个页面同一时间，签名也不一样。
        /// - ts (time stamp)    : 时间戳。可避免重放攻击。
        /// - sn (sign)          : 签名。单向计算，且与服务器key相关，无法破解。
        /// 
        /// URL签名规则
        /// - 在url参数上附加数据：时间戳，随机字符串
        /// - 计算签名：附加key参数 > 将url参数排序 > 重新组装 > md5 > 大写
        /// - 给 url 附加上签名参数
        /// 
        /// URL签名校验规则
        /// - 检索 url 参数，剔除签名部分
        /// - 根据剩余的 url 参数，计算签名
        /// - 判断签名是否相等
        /// 
        /// 
        /// 注意
        /// - URL不能过长
        /// - URL过期时长在网站统一配置，网站可根据情况自行调整过期时间。
        /// - 出于长度考虑，ns和sn都做了截断处理，只保留10位字符串
        /// 
        /// 示例
        /// - xxx.aspx?folder=/Res/&ns=36eeaf0fb7&ts=1571296827&sn=66E2ADE2D8
        /// </remarks>
        public static string ToSignUrl(this string url)
        {
            if (url.IsEmpty())
                return "";
            var d = new Url(url);
            d["ns"] = Guid.NewGuid().ToString("N").SubText(0, _signLength).ToUpper();
            d["ts"] = DateTime.Now.ToTimeStamp();
            d["sn"] = BuildSign(d.Dict, _signKey);
            return d.ToString();
        }

        /// <summary>校验页面URL是否有效（校验：过期时间、签名）</summary>
        public static bool CheckSignedUrl(this string url)
        {
            // 超时校验
            var dict1 = new Url(url).Dict;
            var createDt = dict1["ts"];
            if (createDt.IsEmpty() || DateTime.Now > createDt.ParseTimeStamp().AddMinutes(_signMinutes))
            {
                Logger.LogDb("SignFail", url);
                return false;
            }

            // 签名校验
            var sign = "";
            var dict2 = new Dictionary<string, string>();
            foreach (var key in dict1.Keys)
            {
                var name = key;
                var value = dict1[key];
                if (name == "sn")
                {
                    sign = value;
                    continue;
                }
                dict2.Add(name, value);
            }
            var sign2 = BuildSign(dict2, _signKey);
            return (sign == sign2);
        }

        /// <summary>构造URL签名</summary>
        /// <remarks>参考微信支付签名：https://pay.weixin.qq.com/wiki/doc/api/wxa/wxa_api.php?chapter=4_3</remarks>
        static string BuildSign(Dictionary<string, string> dict, string key)
        {
            var txt = BuildSortQueryString(dict) + "&key=" + key;
            return txt.MD5().ToUpper().Substring(0, _signLength);
        }

        /// <summary>构建排序后的查询字符串（公众号和小程序通用）</summary>
        static string BuildSortQueryString(Dictionary<string, string> dict)
        {
            // (1) 参数排序
            var items = dict.OrderBy(t => t.Key).ToList();

            // (2) 拼装成查询字符串（如果参数的值为空不参与签名）
            var sb = new StringBuilder();
            foreach (var item in items)
            {
                if (item.Value.IsNotEmpty())
                    sb.AppendFormat("{0}={1}&", item.Key, item.Value);
            }
            return sb.ToString().TrimEnd('&');
        }


    }
}
