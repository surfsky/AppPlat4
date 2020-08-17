
## 微信小程序调试策略（使用局域网进行测试）

### 服务器端配置
	服务器关闭windows防火墙，客户端ping测试成功
	服务器网站开始调试，部署到iisexpress
	服务器iisexpress》显示所有应用程序》修改配置》如
        <site name="App" id="3">
            <application path="/" applicationPool="Clr4IntegratedAppPool">
                <virtualDirectory path="/" physicalPath="\\Mac\Home\Downloads\Dev\2019\170720.小熊手机\Src\AppPlat_Bear_gitee\AppPro" />
            </application>
            <bindings>
                <binding protocol="http" bindingInformation="*:5625:localhost" />
                <binding protocol="http" bindingInformation="*:5625:192.168.10.125" />
            </bindings>
        </site>
	以管理员身份重新打开VS, 调试方式运行
	客户端浏览器测试，确保能连接服务器网站

### 微信开发者环境

	打开项目》详情》不校验合法域名...
	将代码中的服务器地址改为局域网地址，如：192.168.10.125
	模拟器调试：直接运行模拟器触发通讯请求
	真机调试：预览》手机扫描》用手机操作。
		注意网络要一致；
		注意安卓真机的cookie处理有问题，没有去重。



----------------------------------------------
UnionID 机制
----------------------------------------------
首先，要注册微信开放平台账户(open.weixin.qq.com)，将微信公众号和微信小程序等关联起来（至少要三个邮箱三个账户）
    如果开发者拥有多个移动应用、网站应用、和公众帐号（包括小程序），可通过 UnionID 来区分用户的唯一性
    因为只要是同一个微信开放平台帐号下的移动应用、网站应用和公众帐号（包括小程序），用户的 UnionID 是唯一的。
    换句话说，同一用户，对同一个微信开放平台下的不同应用，unionid是相同的。

小程序可以通过以下途径获取 UnionID。
	https://developers.weixin.qq.com/miniprogram/dev/framework/open-ability/union-id.html
	- 调用接口 wx.getUserInfo，从解密数据中获取 UnionID。注意本接口需要用户授权，请开发者妥善处理用户拒绝授权后的情况。
	- 如果开发者帐号下存在同主体的公众号，并且该用户已经关注了该公众号。开发者可以直接通过 wx.login + code2Session 获取到该用户 UnionID，无须用户再次授权。
	- 如果开发者帐号下存在同主体的公众号或移动应用，并且该用户已经授权登录过该公众号或移动应用。开发者也可以直接通过 wx.login + code2Session 获取到该用户 UnionID ，无须用户再次授权。
	- 用户在小程序（暂不支持小游戏）中支付完成后，开发者可以直接通过getPaidUnionId接口获取该用户的 UnionID，无需用户授权。注意：本接口仅在用户支付完成后的5分钟内有效，请开发者妥善处理。
	- 小程序端调用云函数时，如果开发者帐号下存在同主体的公众号，并且该用户已经关注了该公众号，可在云函数中通过 cloud.getWXContext 获取 UnionID。
	- 小程序端调用云函数时，如果开发者帐号下存在同主体的公众号或移动应用，并且该用户已经授权登录过该公众号或移动应用，也可在云函数中通过 cloud.getWXContext 获取 UnionID。

公众号UnionID
    https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1421140842
	- 网页授权获取用户基本信息也遵循UnionID机制。即如果开发者有在多个公众号，或在公众号、移动应用之间统一用户帐号的需求，需要前往微信开放平台（open.weixin.qq.com）绑定公众号后，才可利用UnionID机制来满足上述需求。
	- UnionID机制的作用说明：如果开发者拥有多个移动应用、网站应用和公众帐号，可通过获取用户基本信息中的unionid来区分用户的唯一性，因为同一用户，对同一个微信开放平台下的不同应用（移动应用、网站应用和公众帐号），unionid是相同的。



----------------------------------------------
微信小程序
----------------------------------------------
微信小程序测试
	（1）接口可在本地进行调试
	（2）用户登录时的code jsCode2Session 测试，需要在小程序开发环境进行截获（且不能使用掉），才能拿来测试
	（3）消息服务区代码只能放到服务器上去跑，写日志调试
	？测试账号权限

微信小程序模板消息
    页面的 <form/> 组件，属性 report-submit 为 true 时，可以声明为需要发送模板消息，此时点击按钮提交表单可以获取 formId，用于发送模板消息。或者当用户完成 支付行为，可以获取 prepay_id 用于发送模板消息。
    当用户在小程序内完成过支付行为，可允许开发者向用户在7天内推送有限条数的模板消息（1次支付可下发3条，多次支付下发条数独立，互相不影响）
    当用户在小程序内发生过提交表单行为且该表单声明为要发模板消息的，开发者需要向用户提供服务时，可允许开发者向用户在7天内推送有限条数的模板消息（1次提交表单可下发1条，多次提交下发条数独立，相互不影响）


微信小程序页面
	pages/insurance/detail?insuranceId=
	pages/user/Protocol?productId=
	pages/order/detail?orderId=
	pages/news/detail?newsId=
	-----------------------------------------
	"pages/index/index",          //首页
    "pages/authorize/authorize",  //授权页
    "pages/shop/list",            //服务网点列表
    "pages/shop/selectlist",      //服务网点选择
    "pages/shop/map",             //服务网点地图
    "pages/shop/detail",          //服务网点详情
    "pages/redeem/list",          //积分兑换列表
    "pages/news/news",            //资讯列表
    "pages/news/detail",          //资讯详情
    "pages/order/list",           //订单列表
    "pages/order/detail",         //订单详情
    "pages/order/process",        //订单进程
    "pages/user/user",            //用户中心
    "pages/user/about",           //关于
    "pages/user/bindphone",       //绑定手机
    "pages/user/signed",          //签到
    "pages/user/share",           //推广
    "pages/user/sharelist",       //推广列表
    "pages/user/Protocol",        //协议
    "pages/recycle/list",         //回收列表
    "pages/repair/add",           //维修添加
    "pages/insurance/list",       //我的设备列表
    "pages/insurance/selectlist", //我的设备选择
    "pages/insurance/detail",     //设备详情
    "pages/insurance/add",        //设备添加
    "pages/insurance/extension",  //延保添加
    "pages/identify/add"          //真伪鉴定添加


微信小程序的登录
	https://developers.weixin.qq.com/miniprogram/dev/framework/open-ability/login.html
	（1）小程序调用 wx.login() 获取临时登录凭证 code，传给开发者服务器
	（2）开发者服务器调用 code2session 接口，换取openid和sessionkey，保存在服务器端
	（3）开发者服务器返回已登录标志（如cookie）到小程序
	（4）小程序发起业务请求时都携带已登录标志

如何获取用户信息并注册登录
    （1）小程序调用 wx.getUserInfo() 获取信息（基础信息和加密信息）
	（2）加密信息发送到开发者服务器，开发者服务器进行解密，可获取unionId等信息



微信小程序 SessionKey的保存
	（1）经测试，微信用户信息不合适保存在Session中，微信小程序发起的会话AspSessionId都是不同的
        /// <summary>微信用户信息（无效）</summary>
        public static WechatUser WechatUser
        {
            get { return Asp.GetSessionData<WechatUser>("WechatUser", () => new WechatUser());}
            set { Asp.SetSession("WechatUser", value); }
        }
	（2）可采用在数据库中保存的方案，将OpenId和SessionKey关联记录下来。


----------------------------------------------
微信公众号
----------------------------------------------
微信公众号测试
    要求服务器固定IP，80或443端口，与微信服务器通讯
	（1）普通的接口（如GetAccessToken)可直接测试，会返回错误信息“某某ip不在白名单中”，可将这个ip地址到服务器上进行设置即可。
	（2）公众号用户code的生成：要写HTML测试页面
	（3）消息服务器的测试：没办法，只能用固定ip。
		用公众号消息网页测试工具: https://mp.weixin.qq.com/debug
		或写日志调试
	？测试账号权限

网页授权流程分为四步：
	1、引导用户进入授权页面同意授权，获取code
	2、通过code换取网页授权access_token（与基础支持中的access_token不同）
	3、如果需要，开发者可以刷新网页授权access_token，避免过期
	4、通过网页授权access_token和openid获取用户基本信息（支持UnionID机制）

用户信息
	user={
	  "Type": "Web",
	  "subscribe": 1,
	  "subscribe_time": 1556332263,
	  "subscribe_scene": "ADD_SCENE_OTHERS",
	  "qr_scene": 0,
	  "qr_scene_str": "",
	  "openid": "oaL9vxOoMUpfmsUbqQW_wE_sLHVI",
	  "unionid": "osJsO0iCF5blN67Tt061wFBlAaG0",
	  "nickname": "郑素娟",
	  "sex": 2,
	  "language": "zh_CN",
	  "country": "中国",
	  "province": "浙江",
	  "city": "温州",
	  "headimgurl": "http://thirdwx.qlogo.cn/mmopen/OBCGSIWlyFdj8B5HLVB2Q3zMZC81KjRRvqfwsU4TGGCyPBzY7cI1lDvVf0DChju01FrPw0RHLsrv4icrYaNjKNuC7GzJ08O7c/132",
	  "remark": "",
	  "groupid": 0,
	  "tagid_list": [],
	  "errcode": 0
	}


公众号消息
	text 消息
		<xml><ToUserName><![CDATA[gh_ae1207a05405]]></ToUserName>
		<FromUserName><![CDATA[oaL9vxImyL4JKm6Xobz-rYx4XVIE]]></FromUserName>
		<CreateTime>1555166378</CreateTime>
		<MsgType><![CDATA[text]]></MsgType>
		<Content><![CDATA[手机]]></Content>
		<MsgId>22264625617127345</MsgId>
		</xml>
	scan 消息
		<xml><ToUserName><![CDATA[gh_ae1207a05405]]></ToUserName>
		<FromUserName><![CDATA[oaL9vxImyL4JKm6Xobz-rYx4XVIE]]></FromUserName>
		<CreateTime>1555168420</CreateTime>
		<MsgType><![CDATA[event]]></MsgType>
		<Event><![CDATA[SCAN]]></Event>
		<EventKey><![CDATA[/pages/index/index?inviteShopId=35]]></EventKey>
		<Ticket><![CDATA[gQHE8TwAAAAAAAAAAS5odHRwOi8vd2VpeGluLnFxLmNvbS9xLzAyZ1RKWUFRRV9kTDQxMDAwME0wN3UAAgRBhrBcAwQAAAAA]]></Ticket>
		</xml>

	解析后
		{
		  "ToUserName": "gh_ae1207a05405",
		  "FromUserName": "oaL9vxOoMUpfmsUbqQW_wE_sLHVI",
		  "CreateTime": "1556332263",
		  "EventKey": "",
		  "MsgType": "Event",
		  "Event": "Subscribe",
		  "CreateDt": "2019-04-27 10:31:03"
		}

----------------------------------------
自定义菜单
开启服务器配置后，用户发送的消息将自动转发到该配置地址，并且在网站中设置的自动回复和自定义菜单将失效。
----------------------------------------
    自助服务
        首页：小程序 pages/index/index，备用网页 https://mp.weixin.qq.com/s/ohZEfF290udrIp_FUuNKLA
        续保：小程序 pages/insurance/extension，备用网页 https://mp.weixin.qq.com/s/ohZEfF290udrIp_FUuNKLA
        维修下单：pages/repair/add      备用网页 https://mp.weixin.qq.com/s/ohZEfF290udrIp_FUuNKLA
        真伪鉴定：pages/identify/add    备用网页 https://mp.weixin.qq.com/s/ohZEfF290udrIp_FUuNKLA
        保修期查询：网页 https://checkcoverage.apple.com/cn/zh/
    苹果资讯
        苹果服务政策：https://support.apple.com/zh-cn/iphone/repair/service
        iphone8: https://www.apple.com/cn/iphone-8/
        iPhoneX：https://www.apple.com/cn/iphone-x/
        故障快速查询：https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx01da4124f41c7dd5&redirect_uri=http%3a%2f%2fwww.sogservice.com.cn%2fwechat%2fuser%2ftrouble_shooting_index.aspx&response_type=code&scope=snsapi_base&state=test&connect_redirect=1#wechat_redirect
        
    俱乐部 
        商务合作：发送消息
             微信联系：
             YDTEA333
             请备注：小熊管家门店加盟
        一德茶寮：发送消息
              文章：https://mp.weixin.qq.com/s?__biz=MzI5NTY1NzgzMg==&tempkey=MTAwNl9ldG5UdTEreUoraTlneUNrUjRnTDRXeWRxajBWTGNtMVhyUU1laV81cmM3S3k3T1lpamJKeVRLUlF0LVEtc1E2NUVXY2N2Z0tVdXFvbW9xdFBQbDNOcmhaTHVqSEktd184MVJtbjI1MC1tc3N4ZGY2WG8wRF9aaUxkNTE5NlRHbVdNUUJUdUJaZVltaUlsRkZHZkpUTXFjaFB5ejQzLUwtbFh3UmJRfn4%3D&chksm=6c5109485b26805e08dad40950b944dc7b1a3811add23094d1edc7cd4807dc22faef65396651#rd
        茶叶超市：网页 https://weidian.com/?userid=1174155720&p=iphone&wfr=BuyercopyURL&share_relation=e23e421709467613__1

------------------------------------------------------
微信公众号关注逻辑
------------------------------------------------------
公众号邀请二维码
	用户扫描并关注后，可创建用户并设置好用户的归属商家，并设置微信用户的标签（为商店简称），便于营销消息推送
	老客户的微信公众号ID现在是空的，只要关注一下就能补全公众号ID字段，扫描二维码后也可以补全微信标签


小程序邀请二维码
	扫描后即可进入小程序，并设置好用户的归属商家


邀请测试：
（1）去服务器上删除自己的账号、自己的受邀记录
（2）扫描别的用户的二维码，进入小程序
（3）查看绑定手机页面的逻辑



wechat
```
        /// <summary>下载媒体文件</summary>
        public static string DownloadMediaFile(string mediaId, string filePath)
        {
            var accessToken = GetAccesToken();
            var url = string.Format("http://file.api.weixin.qq.com/cgi-bin/media/get?access_token={0}&media_id={1}", accessToken, mediaId);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "GET";
            req.KeepAlive = false;
            var fileName = "";
            var resp = (HttpWebResponse)req.GetResponse();
            var strpath = resp.ResponseUri.ToString();
            var head = resp.GetResponseHeader("Content-Disposition");
            if (!string.IsNullOrEmpty(head))
            {
                var client = new WebClient();
                int headst = head.IndexOf("filename=");
                int headend = head.IndexOf("\"", headst);
                fileName = head.Substring(headst + 10, head.Length - headst - 11);
                fileName = string.Format("{0}.{1}", DateTime.Now.ToString("yyyyMMddHHmmssffffff"), fileName.Split('.')[1]);
                client.DownloadFile(strpath, filePath + fileName);
            }
            resp.Close();
            resp = null;
            req.Abort();
            req = null;
            return fileName;
        }
```