
## ΢��С������Բ��ԣ�ʹ�þ��������в��ԣ�

### ������������
	�������ر�windows����ǽ���ͻ���ping���Գɹ�
	��������վ��ʼ���ԣ�����iisexpress
	������iisexpress����ʾ����Ӧ�ó����޸����á���
        <site name="App" id="3">
            <application path="/" applicationPool="Clr4IntegratedAppPool">
                <virtualDirectory path="/" physicalPath="\\Mac\Home\Downloads\Dev\2019\170720.С���ֻ�\Src\AppPlat_Bear_gitee\AppPro" />
            </application>
            <bindings>
                <binding protocol="http" bindingInformation="*:5625:localhost" />
                <binding protocol="http" bindingInformation="*:5625:192.168.10.125" />
            </bindings>
        </site>
	�Թ���Ա������´�VS, ���Է�ʽ����
	�ͻ�����������ԣ�ȷ�������ӷ�������վ

### ΢�ſ����߻���

	����Ŀ�����顷��У��Ϸ�����...
	�������еķ�������ַ��Ϊ��������ַ���磺192.168.10.125
	ģ�������ԣ�ֱ������ģ��������ͨѶ����
	������ԣ�Ԥ�����ֻ�ɨ�衷���ֻ�������
		ע������Ҫһ�£�
		ע�ⰲ׿�����cookie���������⣬û��ȥ�ء�



----------------------------------------------
UnionID ����
----------------------------------------------
���ȣ�Ҫע��΢�ſ���ƽ̨�˻�(open.weixin.qq.com)����΢�Ź��ںź�΢��С����ȹ�������������Ҫ�������������˻���
    ���������ӵ�ж���ƶ�Ӧ�á���վӦ�á��͹����ʺţ�����С���򣩣���ͨ�� UnionID �������û���Ψһ��
    ��ΪֻҪ��ͬһ��΢�ſ���ƽ̨�ʺ��µ��ƶ�Ӧ�á���վӦ�ú͹����ʺţ�����С���򣩣��û��� UnionID ��Ψһ�ġ�
    ���仰˵��ͬһ�û�����ͬһ��΢�ſ���ƽ̨�µĲ�ͬӦ�ã�unionid����ͬ�ġ�

С�������ͨ������;����ȡ UnionID��
	https://developers.weixin.qq.com/miniprogram/dev/framework/open-ability/union-id.html
	- ���ýӿ� wx.getUserInfo���ӽ��������л�ȡ UnionID��ע�Ȿ�ӿ���Ҫ�û���Ȩ���뿪�������ƴ����û��ܾ���Ȩ��������
	- ����������ʺ��´���ͬ����Ĺ��ںţ����Ҹ��û��Ѿ���ע�˸ù��ںš������߿���ֱ��ͨ�� wx.login + code2Session ��ȡ�����û� UnionID�������û��ٴ���Ȩ��
	- ����������ʺ��´���ͬ����Ĺ��ںŻ��ƶ�Ӧ�ã����Ҹ��û��Ѿ���Ȩ��¼���ù��ںŻ��ƶ�Ӧ�á�������Ҳ����ֱ��ͨ�� wx.login + code2Session ��ȡ�����û� UnionID �������û��ٴ���Ȩ��
	- �û���С�����ݲ�֧��С��Ϸ����֧����ɺ󣬿����߿���ֱ��ͨ��getPaidUnionId�ӿڻ�ȡ���û��� UnionID�������û���Ȩ��ע�⣺���ӿڽ����û�֧����ɺ��5��������Ч���뿪�������ƴ���
	- С����˵����ƺ���ʱ������������ʺ��´���ͬ����Ĺ��ںţ����Ҹ��û��Ѿ���ע�˸ù��ںţ������ƺ�����ͨ�� cloud.getWXContext ��ȡ UnionID��
	- С����˵����ƺ���ʱ������������ʺ��´���ͬ����Ĺ��ںŻ��ƶ�Ӧ�ã����Ҹ��û��Ѿ���Ȩ��¼���ù��ںŻ��ƶ�Ӧ�ã�Ҳ�����ƺ�����ͨ�� cloud.getWXContext ��ȡ UnionID��

���ں�UnionID
    https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1421140842
	- ��ҳ��Ȩ��ȡ�û�������ϢҲ��ѭUnionID���ơ���������������ڶ�����ںţ����ڹ��ںš��ƶ�Ӧ��֮��ͳһ�û��ʺŵ�������Ҫǰ��΢�ſ���ƽ̨��open.weixin.qq.com���󶨹��ںź󣬲ſ�����UnionID������������������
	- UnionID���Ƶ�����˵�������������ӵ�ж���ƶ�Ӧ�á���վӦ�ú͹����ʺţ���ͨ����ȡ�û�������Ϣ�е�unionid�������û���Ψһ�ԣ���Ϊͬһ�û�����ͬһ��΢�ſ���ƽ̨�µĲ�ͬӦ�ã��ƶ�Ӧ�á���վӦ�ú͹����ʺţ���unionid����ͬ�ġ�



----------------------------------------------
΢��С����
----------------------------------------------
΢��С�������
	��1���ӿڿ��ڱ��ؽ��е���
	��2���û���¼ʱ��code jsCode2Session ���ԣ���Ҫ��С���򿪷��������нػ��Ҳ���ʹ�õ�����������������
	��3����Ϣ����������ֻ�ܷŵ���������ȥ�ܣ�д��־����
	�������˺�Ȩ��

΢��С����ģ����Ϣ
    ҳ��� <form/> ��������� report-submit Ϊ true ʱ����������Ϊ��Ҫ����ģ����Ϣ����ʱ�����ť�ύ�����Ի�ȡ formId�����ڷ���ģ����Ϣ�����ߵ��û���� ֧����Ϊ�����Ի�ȡ prepay_id ���ڷ���ģ����Ϣ��
    ���û���С��������ɹ�֧����Ϊ���������������û���7������������������ģ����Ϣ��1��֧�����·�3�������֧���·��������������಻Ӱ�죩
    ���û���С�����ڷ������ύ����Ϊ�Ҹñ�����ΪҪ��ģ����Ϣ�ģ���������Ҫ���û��ṩ����ʱ���������������û���7������������������ģ����Ϣ��1���ύ�����·�1��������ύ�·������������໥��Ӱ�죩


΢��С����ҳ��
	pages/insurance/detail?insuranceId=
	pages/user/Protocol?productId=
	pages/order/detail?orderId=
	pages/news/detail?newsId=
	-----------------------------------------
	"pages/index/index",          //��ҳ
    "pages/authorize/authorize",  //��Ȩҳ
    "pages/shop/list",            //���������б�
    "pages/shop/selectlist",      //��������ѡ��
    "pages/shop/map",             //���������ͼ
    "pages/shop/detail",          //������������
    "pages/redeem/list",          //���ֶһ��б�
    "pages/news/news",            //��Ѷ�б�
    "pages/news/detail",          //��Ѷ����
    "pages/order/list",           //�����б�
    "pages/order/detail",         //��������
    "pages/order/process",        //��������
    "pages/user/user",            //�û�����
    "pages/user/about",           //����
    "pages/user/bindphone",       //���ֻ�
    "pages/user/signed",          //ǩ��
    "pages/user/share",           //�ƹ�
    "pages/user/sharelist",       //�ƹ��б�
    "pages/user/Protocol",        //Э��
    "pages/recycle/list",         //�����б�
    "pages/repair/add",           //ά�����
    "pages/insurance/list",       //�ҵ��豸�б�
    "pages/insurance/selectlist", //�ҵ��豸ѡ��
    "pages/insurance/detail",     //�豸����
    "pages/insurance/add",        //�豸���
    "pages/insurance/extension",  //�ӱ����
    "pages/identify/add"          //��α�������


΢��С����ĵ�¼
	https://developers.weixin.qq.com/miniprogram/dev/framework/open-ability/login.html
	��1��С������� wx.login() ��ȡ��ʱ��¼ƾ֤ code�����������߷�����
	��2�������߷��������� code2session �ӿڣ���ȡopenid��sessionkey�������ڷ�������
	��3�������߷����������ѵ�¼��־����cookie����С����
	��4��С������ҵ������ʱ��Я���ѵ�¼��־

��λ�ȡ�û���Ϣ��ע���¼
    ��1��С������� wx.getUserInfo() ��ȡ��Ϣ��������Ϣ�ͼ�����Ϣ��
	��2��������Ϣ���͵������߷������������߷��������н��ܣ��ɻ�ȡunionId����Ϣ



΢��С���� SessionKey�ı���
	��1�������ԣ�΢���û���Ϣ�����ʱ�����Session�У�΢��С������ĻỰAspSessionId���ǲ�ͬ��
        /// <summary>΢���û���Ϣ����Ч��</summary>
        public static WechatUser WechatUser
        {
            get { return Asp.GetSessionData<WechatUser>("WechatUser", () => new WechatUser());}
            set { Asp.SetSession("WechatUser", value); }
        }
	��2���ɲ��������ݿ��б���ķ�������OpenId��SessionKey������¼������


----------------------------------------------
΢�Ź��ں�
----------------------------------------------
΢�Ź��ںŲ���
    Ҫ��������̶�IP��80��443�˿ڣ���΢�ŷ�����ͨѶ
	��1����ͨ�Ľӿڣ���GetAccessToken)��ֱ�Ӳ��ԣ��᷵�ش�����Ϣ��ĳĳip���ڰ������С����ɽ����ip��ַ���������Ͻ������ü��ɡ�
	��2�����ں��û�code�����ɣ�ҪдHTML����ҳ��
	��3����Ϣ�������Ĳ��ԣ�û�취��ֻ���ù̶�ip��
		�ù��ں���Ϣ��ҳ���Թ���: https://mp.weixin.qq.com/debug
		��д��־����
	�������˺�Ȩ��

��ҳ��Ȩ���̷�Ϊ�Ĳ���
	1�������û�������Ȩҳ��ͬ����Ȩ����ȡcode
	2��ͨ��code��ȡ��ҳ��Ȩaccess_token�������֧���е�access_token��ͬ��
	3�������Ҫ�������߿���ˢ����ҳ��Ȩaccess_token���������
	4��ͨ����ҳ��Ȩaccess_token��openid��ȡ�û�������Ϣ��֧��UnionID���ƣ�

�û���Ϣ
	user={
	  "Type": "Web",
	  "subscribe": 1,
	  "subscribe_time": 1556332263,
	  "subscribe_scene": "ADD_SCENE_OTHERS",
	  "qr_scene": 0,
	  "qr_scene_str": "",
	  "openid": "oaL9vxOoMUpfmsUbqQW_wE_sLHVI",
	  "unionid": "osJsO0iCF5blN67Tt061wFBlAaG0",
	  "nickname": "֣�ؾ�",
	  "sex": 2,
	  "language": "zh_CN",
	  "country": "�й�",
	  "province": "�㽭",
	  "city": "����",
	  "headimgurl": "http://thirdwx.qlogo.cn/mmopen/OBCGSIWlyFdj8B5HLVB2Q3zMZC81KjRRvqfwsU4TGGCyPBzY7cI1lDvVf0DChju01FrPw0RHLsrv4icrYaNjKNuC7GzJ08O7c/132",
	  "remark": "",
	  "groupid": 0,
	  "tagid_list": [],
	  "errcode": 0
	}


���ں���Ϣ
	text ��Ϣ
		<xml><ToUserName><![CDATA[gh_ae1207a05405]]></ToUserName>
		<FromUserName><![CDATA[oaL9vxImyL4JKm6Xobz-rYx4XVIE]]></FromUserName>
		<CreateTime>1555166378</CreateTime>
		<MsgType><![CDATA[text]]></MsgType>
		<Content><![CDATA[�ֻ�]]></Content>
		<MsgId>22264625617127345</MsgId>
		</xml>
	scan ��Ϣ
		<xml><ToUserName><![CDATA[gh_ae1207a05405]]></ToUserName>
		<FromUserName><![CDATA[oaL9vxImyL4JKm6Xobz-rYx4XVIE]]></FromUserName>
		<CreateTime>1555168420</CreateTime>
		<MsgType><![CDATA[event]]></MsgType>
		<Event><![CDATA[SCAN]]></Event>
		<EventKey><![CDATA[/pages/index/index?inviteShopId=35]]></EventKey>
		<Ticket><![CDATA[gQHE8TwAAAAAAAAAAS5odHRwOi8vd2VpeGluLnFxLmNvbS9xLzAyZ1RKWUFRRV9kTDQxMDAwME0wN3UAAgRBhrBcAwQAAAAA]]></Ticket>
		</xml>

	������
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
�Զ���˵�
�������������ú��û����͵���Ϣ���Զ�ת���������õ�ַ����������վ�����õ��Զ��ظ����Զ���˵���ʧЧ��
----------------------------------------
    ��������
        ��ҳ��С���� pages/index/index��������ҳ https://mp.weixin.qq.com/s/ohZEfF290udrIp_FUuNKLA
        ������С���� pages/insurance/extension��������ҳ https://mp.weixin.qq.com/s/ohZEfF290udrIp_FUuNKLA
        ά���µ���pages/repair/add      ������ҳ https://mp.weixin.qq.com/s/ohZEfF290udrIp_FUuNKLA
        ��α������pages/identify/add    ������ҳ https://mp.weixin.qq.com/s/ohZEfF290udrIp_FUuNKLA
        �����ڲ�ѯ����ҳ https://checkcoverage.apple.com/cn/zh/
    ƻ����Ѷ
        ƻ���������ߣ�https://support.apple.com/zh-cn/iphone/repair/service
        iphone8: https://www.apple.com/cn/iphone-8/
        iPhoneX��https://www.apple.com/cn/iphone-x/
        ���Ͽ��ٲ�ѯ��https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx01da4124f41c7dd5&redirect_uri=http%3a%2f%2fwww.sogservice.com.cn%2fwechat%2fuser%2ftrouble_shooting_index.aspx&response_type=code&scope=snsapi_base&state=test&connect_redirect=1#wechat_redirect
        
    ���ֲ� 
        ���������������Ϣ
             ΢����ϵ��
             YDTEA333
             �뱸ע��С�ܹܼ��ŵ����
        һ�²�弣�������Ϣ
              ���£�https://mp.weixin.qq.com/s?__biz=MzI5NTY1NzgzMg==&tempkey=MTAwNl9ldG5UdTEreUoraTlneUNrUjRnTDRXeWRxajBWTGNtMVhyUU1laV81cmM3S3k3T1lpamJKeVRLUlF0LVEtc1E2NUVXY2N2Z0tVdXFvbW9xdFBQbDNOcmhaTHVqSEktd184MVJtbjI1MC1tc3N4ZGY2WG8wRF9aaUxkNTE5NlRHbVdNUUJUdUJaZVltaUlsRkZHZkpUTXFjaFB5ejQzLUwtbFh3UmJRfn4%3D&chksm=6c5109485b26805e08dad40950b944dc7b1a3811add23094d1edc7cd4807dc22faef65396651#rd
        ��Ҷ���У���ҳ https://weidian.com/?userid=1174155720&p=iphone&wfr=BuyercopyURL&share_relation=e23e421709467613__1

------------------------------------------------------
΢�Ź��ںŹ�ע�߼�
------------------------------------------------------
���ں������ά��
	�û�ɨ�貢��ע�󣬿ɴ����û������ú��û��Ĺ����̼ң�������΢���û��ı�ǩ��Ϊ�̵��ƣ�������Ӫ����Ϣ����
	�Ͽͻ���΢�Ź��ں�ID�����ǿյģ�ֻҪ��עһ�¾��ܲ�ȫ���ں�ID�ֶΣ�ɨ���ά���Ҳ���Բ�ȫ΢�ű�ǩ


С���������ά��
	ɨ��󼴿ɽ���С���򣬲����ú��û��Ĺ����̼�


������ԣ�
��1��ȥ��������ɾ���Լ����˺š��Լ���������¼
��2��ɨ�����û��Ķ�ά�룬����С����
��3���鿴���ֻ�ҳ����߼�



wechat
```
        /// <summary>����ý���ļ�</summary>
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