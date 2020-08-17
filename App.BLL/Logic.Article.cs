using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Components;
using App.Utils;
using App.DAL;
using App.Wechats;
using App.Wechats.MP;
using App.Wechats.OP;
using App.Wechats.Pay;

namespace App
{
    /// <summary>
    /// 商务逻辑类。无法明确归属的、跨实体的逻辑可放在此处
    /// </summary>
    public partial class Logic
    {
        /// <summary>发送有新文章消息（未完成）</summary>
        public static void SendNewArticleMsg(Article article)
        {
            // 发送消息给订阅者（用消息队列）
            //var list = ArticleDirFavorite.Search(t => t.ArticleDirID == article.DirID).ToList();  // 用户关注的目录
            //ArticleDir.GetChildrenIds()

            //// 发消息给关注关键字的用户
            //var users = new List<User>();
            //var keywords = article.Keywords.Split<string>();  //.Add(article.Dir?.Name);
            //foreach (var keyword in keywords)
            //{
            //    var us = User.Search(t => t.Keywords.Contains(keyword)).ToList();
            //    users = users.Union(us);
            //}
            var users = new List<User>();
            List<long> dirIds = ArticleDir.GetParentIds(article.Dir);

            foreach (var v in User.Search())
            {
                for (int i = 0; i < dirIds.Count; i++)
                {
                    if (v.GetAllowedArticleDirIdsOutChildren().Contains(dirIds[i]))
                    {
                        users.Add(v);
                        break;
                    }
                }
            }
            users = users.Distinct().ToList();

            if (users.Count > 0)
            {
                var userIds = users.Cast(t => t.DingUserID);
                var url = string.Format("eapp://pages/pushLogin/pushLogin?type=Article&id={0}", article.ID);
                var rsp = DingHelper.CorpSendCard(userIds, "知识库有新文档啦", article.Title, url);
                Logger.LogDb("钉钉新文档推送", new { userIds, url, rsp }.ToJson());
            }
        }

        /// <summary>发送有新回复消息（给有反馈处理权限的人员，如内容管理员）</summary>
        public static void SendNewReplyMsg(Article article)
        {
            //var roleId = Role.AdminArticle.ID;
            //var users = User.SearchByRole(roleId).ToList();
            var power = Powers.ArticleFeedbackEdit;
            var users = User.SearchByPower(power).ToList();
            var userIds = users.Cast(t => t.DingUserID);
            if (users.Count > 0)
            {
                var url = string.Format("eapp://pages/pushLogin/pushLogin?type=Reply&id={0}", article.ID);
                var rsp = DingHelper.CorpSendCard(userIds, "知识库有新评论", article.Title, url);
                Logger.LogDb("钉钉评论推送", new { userIds, url, rsp }.ToJson());
            }
        }
    }
}
