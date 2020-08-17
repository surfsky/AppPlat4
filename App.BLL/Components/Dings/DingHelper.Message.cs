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
        //--------------------------------------------------------
        // 钉钉推送消息
        // https://ding-doc.dingtalk.com/doc#/serverapi2/al5qyp
        // - 工作通知消息：是以企业工作通知会话中某个微应用的名义推送到员工的通知消息，例如生日祝福、入职提醒等。
        // - 群消息：是指可以调用接口以系统名义向群里推送群聊消息。
        // - 普通消息：是指员工个人在使用应用时，可以通过界面操作的方式往群或其他人的会话里推送消息，例如发送日志的场景。
        // - 任务类通知：是指需要发送一条任务提醒给员工，比如审批任务等，这类情况下可参考待办任务案例。
        //--------------------------------------------------------
        // 工作通知消息（以Corp开头）
        // https://ding-doc.dingtalk.com/doc#/serverapi2/pgoxpy
        // - 类型有：text, image, file, link, markdown, oa, actioncard
        // - 是异步的，发送后会返回taskId。然后需要在 GetSendProgress 或 Result 方法中查询
        //--------------------------------------------------------
        /// <summary>发送文本消息</summary>
        /// <param name="userIds">用户ID列表（以逗号隔开，最多100个）</param>
        public static OapiMessageCorpconversationAsyncsendV2Response CorpSendText(List<long> userIds, string text)
        {
            var ding = AliDingConfig.Instance;
            var accessToken = GetAccessToken();
            var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/message/corpconversation/asyncsend_v2");
            var request = new OapiMessageCorpconversationAsyncsendV2Request();
            request.AgentId = ding.MPAgentID;
            request.UseridList = userIds.ToSeparatedString();
            request.Msg = JsonConvert.SerializeObject(new
            {
                msgtype = "text",
                text = new
                {
                    content = text
                }
            });
            return client.Execute(request, accessToken.AccessToken);
        }

        /// <summary>发送卡片消息</summary>
        /// <param name="button">按钮文本，如“查看详情”</param>
        public static OapiMessageCorpconversationAsyncsendV2Response CorpSendCard(List<string> userIds, string title, string markdown, string url, string button="查看")
        {
            var ding = AliDingConfig.Instance;
            var accessToken = GetAccessToken();
            var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/message/corpconversation/asyncsend_v2");
            var request = new OapiMessageCorpconversationAsyncsendV2Request();
            request.AgentId = ding.MPAgentID;
            request.UseridList = userIds.ToSeparatedString();
            request.Msg = JsonConvert.SerializeObject(new
            {
                msgtype = "action_card",
                action_card = new
                {
                    title = title,
                    markdown = markdown,
                    single_title = button,
                    single_url = url
                }
            });
            return client.Execute(request, accessToken.AccessToken);
        }


        /// <summary>发送钉钉 OA 消息（参考：https://ding-doc.dingtalk.com/doc#/serverapi2/pgoxpy）</summary>
        /// <param name="mpPage">钉钉小程序跳转地址</param>
        public static OapiMessageCorpconversationAsyncsendV2Response CorpSendOA(
            List<long> userIds, 
            string headText, string bodyTitle, string bodyContent, string bodyImage,
            string mpPage, string webUrl, 
            Dictionary<string, string> dict)
        {
            var mpUrl = string.Format("eapp://page?{0}", mpPage.UrlEncode());
            var ding = AliDingConfig.Instance;
            var accessToken = GetAccessToken();
            var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/message/corpconversation/asyncsend_v2");
            var request = new OapiMessageCorpconversationAsyncsendV2Request();
            request.AgentId = ding.MPAgentID;
            request.UseridList = userIds.ToSeparatedString();
            request.Msg = JsonConvert.SerializeObject(new
            {
                msgtype = "oa",
                oa = new
                {
                    head = new { bgcolor = "FFBBBBBB", text = headText },
                    body = new
                    {
                        title = bodyTitle,
                        form = dict,
                        content = bodyContent,
                        image = bodyImage,
                    },
                    message_url = mpUrl,
                    pc_message_url = webUrl
                }
            });
            return client.Execute(request, accessToken.AccessToken);
        }



        /// <summary>获取发送进度(https://ding-doc.dingtalk.com/doc#/serverapi2/pgoxpy)</summary>
        public static OapiMessageCorpconversationGetsendprogressResponse CorpGetProgress(string taskId)
        {
            var ding = AliDingConfig.Instance;
            var accessToken = GetAccessToken();
            var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/message/corpconversation/getsendprogress");
            var request = new OapiMessageCorpconversationGetsendprogressRequest();
            request.AgentId = ding.MPAgentID;
            request.TaskId = taskId.ParseLong();
            var response = client.Execute(request, accessToken.AccessToken);
            return response;
        }

        /// <summary>获取发送结果(https://ding-doc.dingtalk.com/doc#/serverapi2/pgoxpy)</summary>
        public static OapiMessageCorpconversationGetsendresultResponse CorpGetResult(string taskId)
        {
            var ding = AliDingConfig.Instance;
            var accessToken = GetAccessToken();
            var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/message/corpconversation/getsendresult");
            var request = new OapiMessageCorpconversationGetsendresultRequest();
            request.AgentId = ding.MPAgentID;
            request.TaskId = taskId.ParseLong();
            var response = client.Execute(request, accessToken.AccessToken);
            return response;
        }

        //--------------------------------------------------------
        // 群聊天消息（以 Chat 开头）
        //--------------------------------------------------------
        /// <summary>发送群聊天消息(https://ding-doc.dingtalk.com/doc#/serverapi2/isu6nk)</summary>
        /// <remarks>会返回messageid</remarks>
        public static OapiChatSendResponse ChatSend(string chatId, string text)
        {
            var ding = AliDingConfig.Instance;
            var accessToken = GetAccessToken();
            var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/chat/send");
            var request = new OapiChatSendRequest();
            request.Chatid = chatId;
            request.Msgtype = "text";
            request.Text = text;
            return client.Execute(request, accessToken.AccessToken);
        }

        /// <summary>创建聊天群(https://ding-doc.dingtalk.com/doc#/serverapi2/isu6nk)</summary>
        /// <remarks>会返回chatid</remarks>
        public static OapiChatCreateResponse ChatCreate(string chatName, string ownerId, List<string> userIds)
        {
            var ding = AliDingConfig.Instance;
            var accessToken = GetAccessToken();
            var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/chat/create");
            var request = new OapiChatCreateRequest();
            request.Name = chatName;
            request.Owner = ownerId;
            request.Useridlist = userIds;
            request.ShowHistoryType = 1;  // 新成员可查看历史消息
            return client.Execute(request, accessToken.AccessToken);
        }

        //--------------------------------------------------------
        // 普通消息
        // 个人往群或其他人的会话里推送消息
        // 发送普通消息，需要在前端页面调用JSAPI唤起联系人会话选择页面，选中后会返回会话cid，然后再调用服务端接口向会话里发送一条消息
        //--------------------------------------------------------
        /// <summary>个人给指定人或群发消息(https://ding-doc.dingtalk.com/doc#/serverapi2/pm0m06)</summary>
        public static OapiMessageSendToConversationResponse MessageSendText(string chatId, string senderId, string text)
        {
            var ding = AliDingConfig.Instance;
            var accessToken = GetAccessToken();
            var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/message/send_to_conversation");
            var request = new OapiMessageSendToConversationRequest();
            request.Sender = senderId;
            request.Cid = chatId;
            request.Msgtype = "text";
            request.Text = text;
            return client.Execute(request, accessToken.AccessToken);
        }

        /// <summary>个人给指定人或群发图片消息(https://ding-doc.dingtalk.com/doc#/serverapi2/pm0m06)</summary>
        public static OapiMessageSendToConversationResponse MessageSendImage(string chatId, string senderId, string image)
        {
            var ding = AliDingConfig.Instance;
            var accessToken = GetAccessToken();
            var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/message/send_to_conversation");
            var request = new OapiMessageSendToConversationRequest();
            request.Sender = senderId;
            request.Cid = chatId;
            request.Msgtype = "image";
            request.Image = image;
            return client.Execute(request, accessToken.AccessToken);
        }

    }
}