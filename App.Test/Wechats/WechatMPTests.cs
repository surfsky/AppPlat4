using Microsoft.VisualStudio.TestTools.UnitTesting;
using App.Wechats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Wechats.MP;

namespace App.Wechats.Tests
{
    [TestClass()]
    public class WechatMPTests
    {
        /*
        {
           "encryptedData" : "O9LXDmVHFNOp8taALcYj/VZGNmbBPTTHODPOpahzZNGAWlT9UbADyWAUMA+pvFGaEW6oReLm87ljL5gQmrEptXd3EfoKBsOfjpFuWjWQRmtFI+z6EUKUrVpTMTMCcEvc2LgQk+PbMTKu2RrPeazjiw8Dlvia5UlbLJ6udssfxzOPqyU1beA0FVqF1eomfJoljn7hdHs2eEaUlCQ1sjYGzi9us63u09385joTpCx6Izkh7WVylsmiUe78BPCVxL9itD8QnunvpXddnaZWkUTikU3aOKVnigsaz7OGRDBT/zpE0jNmw0iJvOO3v6haSCzlEOwk4MF/lRFxV/3NqrkB1+4zrfGh1Ztrp6m9chrN8Jj//76FUbrmR8R/iSyVxJV3oKa7RxqIW/Dub5I6yHOCSskz0gFiKVjWFTtnwJp8YaoEhJctNi47mmIIKqCvm2HUnp0OmQdtQcv7zlg1jYEfRP2ELYR0eva+pFijGxgz2lM6DkG4dhIKuruJuQoT08rrYM9PzI2R6wVkkNP70j06WY9tQdoaXdDkBQYloZbJ/2Y=",
           "errMsg" : "getUserInfo:ok",
           "iv" : "y48789PZv3xYHVkfaw6xBQ==",
           "rawData" : "{\"nickName\":\"梁益鑫\",\"gender\":1,\"language\":\"zh_CN\",\"city\":\"Wenzhou\",\"province\":\"Zhejiang\",\"country\":\"China\",\"avatarUrl\":\"https://wx.qlogo.cn/mmopen/vi_32/Q0j4TwGTfTJmO5r4Cx9rO2SS3AR6bAnZKdtNU5TDXBk5tibjQBPhibWPMHxasUP9ba2cib7dibgicyP4M9y97pbuXxQ/132\"}",
           "signature" : "0ee01ba21b01dd34a6b5bb7750e66659c319e259",
           "userInfo" : {
              "avatarUrl" : "https://wx.qlogo.cn/mmopen/vi_32/Q0j4TwGTfTJmO5r4Cx9rO2SS3AR6bAnZKdtNU5TDXBk5tibjQBPhibWPMHxasUP9ba2cib7dibgicyP4M9y97pbuXxQ/132",
              "city" : "Wenzhou",
              "country" : "China",
              "gender" : 1,
              "language" : "zh_CN",
              "nickName" : "梁益鑫",
              "province" : "Zhejiang"
           }
        }
        */
        [TestMethod()]
        public void DecryptUserTest()
        {
            // rawdata部分引号嵌套有问题，不知道怎么写
            var reply = @"
                {
                   ""encryptedData"" : ""O9LXDmVHFNOp8taALcYj/VZGNmbBPTTHODPOpahzZNGAWlT9UbADyWAUMA+pvFGaEW6oReLm87ljL5gQmrEptXd3EfoKBsOfjpFuWjWQRmtFI+z6EUKUrVpTMTMCcEvc2LgQk+PbMTKu2RrPeazjiw8Dlvia5UlbLJ6udssfxzOPqyU1beA0FVqF1eomfJoljn7hdHs2eEaUlCQ1sjYGzi9us63u09385joTpCx6Izkh7WVylsmiUe78BPCVxL9itD8QnunvpXddnaZWkUTikU3aOKVnigsaz7OGRDBT/zpE0jNmw0iJvOO3v6haSCzlEOwk4MF/lRFxV/3NqrkB1+4zrfGh1Ztrp6m9chrN8Jj//76FUbrmR8R/iSyVxJV3oKa7RxqIW/Dub5I6yHOCSskz0gFiKVjWFTtnwJp8YaoEhJctNi47mmIIKqCvm2HUnp0OmQdtQcv7zlg1jYEfRP2ELYR0eva+pFijGxgz2lM6DkG4dhIKuruJuQoT08rrYM9PzI2R6wVkkNP70j06WY9tQdoaXdDkBQYloZbJ/2Y="",
                   ""errMsg"" : ""getUserInfo:ok"",
                   ""iv"" : ""y48789PZv3xYHVkfaw6xBQ=="",
                   ""signature"" : ""0ee01ba21b01dd34a6b5bb7750e66659c319e259"",
                   ""rawData"" : ""{\""nickName\"":\""梁益鑫\"",\""gender\"":1,\""language\"":\""zh_CN\"",\""city\"":\""Wenzhou\"",\""province\"":\""Zhejiang\"",\""country\"":\""China\"",\""avatarUrl\"":\""https://wx.qlogo.cn/mmopen/vi_32/Q0j4TwGTfTJmO5r4Cx9rO2SS3AR6bAnZKdtNU5TDXBk5tibjQBPhibWPMHxasUP9ba2cib7dibgicyP4M9y97pbuXxQ/132\""}"",
                   ""userInfo"" : {
                      ""avatarUrl"" : ""https://wx.qlogo.cn/mmopen/vi_32/Q0j4TwGTfTJmO5r4Cx9rO2SS3AR6bAnZKdtNU5TDXBk5tibjQBPhibWPMHxasUP9ba2cib7dibgicyP4M9y97pbuXxQ/132"",
                      ""city"" : ""Wenzhou"",
                      ""country"" : ""China"",
                      ""gender"" : 1,
                      ""language"" : ""zh_CN"",
                      ""nickName"" : ""梁益鑫"",
                      ""province"" : ""Zhejiang""
                   }
                }";

            var sessionKey = "TzOQTtV5kjgc4ROeaU7kuQ==";
            var user = WechatMP.DecryptUserInfo(reply, sessionKey);

        }
    }
}