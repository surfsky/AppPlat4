using App.Utils;
using App.HttpApi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.ComponentModel;
using App.Components;
using App.DAL;


namespace App.Apis
{
    public partial class ApiMall
    {
        //----------------------------------------------
        // App(微信端）配置相关
        //----------------------------------------------
        [HttpApi("获取广告", CacheSeconds = 600)]
        public static APIResult GetAdverts(AdvertPlace place, long? shopId=null)
        {
            var items = Advert.Search(place, shopId, AdvertStatus.Active).ToList().Cast(t => t.Export());
            return items.ToResult();
        }


        //----------------------------------------------
        // 统计相关
        //----------------------------------------------
        /*
        [HttpApi("获取统计数据（商店、客户、订单）", CacheSeconds = 600)]
        public APIResult GetScores()
        {
            var o = new
            {
                Shops = Shop.Set.Count(),
                Customers = 2000 + User.SearchRole(Role.Employees).Count(),
                Orders = 1800 + Order.Set.Count()
            };
            return o.ToResult();
        }
        */


        //----------------------------------------------
        // 商店相关
        //----------------------------------------------
        [HttpApi("获取商店列表", CacheSeconds = 60)]
        public APIResult GetShops(string clientGPS="")
        {
            var items = Shop.Search().ToList().Select(
                s => new {
                    s.ID, s.Name, s.AbbrName,
                    s.Addr, s.GPS, s.Tel,
                    //s.Description,
                    CoverImage = Asp.ResolveUrl(s.CoverImage),
                    Distance = CalcDistance(s.GPS, clientGPS)
                });
            var o = items.OrderBy(t => t.Distance).ToList();
            return o.ToResult();
        }

        [HttpApi("获取商店", CacheSeconds = 60)]
        public APIResult GetShop(long shopId, string clientGPS= "")
        {
            var s = Shop.Get(shopId);
            Util.Assert(s != null, "无此商店");
            var o = new {
                s.ID, s.Name, s.AbbrName,
                s.Addr, s.GPS, s.Tel,
                s.Description,
                CoverImage = Asp.ResolveUrl(s.CoverImage),
                Distance = CalcDistance(s.GPS, clientGPS)
                };
            return o.ToResult();
        }

        // 计算两个GPS坐标的距离
        static double CalcDistance(string gps1, string gps2)
        {
            try
            {
                var arr1 = gps1.Split(',');
                var arr2 = gps2.Split(',');
                double x1 = double.Parse(arr1[0]);
                double y1 = double.Parse(arr1[1]);
                double x2 = double.Parse(arr2[0]);
                double y2 = double.Parse(arr2[1]);
                return MathHelper.CalcGPSDistance(x1, y1, x2, y2);
            }
            catch
            {
                return 999999;
            }
        }


        //----------------------------------------------
        // 文章相关
        //----------------------------------------------
        [HttpApi("获取文章类型", CacheSeconds = 60)]
        public APIResult GetArticleTypes()
        {
            //var items = new List<ArticleType>();
            //items.Add(ArticleType.News);
            //items.Add(ArticleType.Activity);
            //return items.ToEnumInfos().ToResult();
            var items = typeof(ArticleType).GetEnumInfos("Site");
            return items.ToResult();
        }

        [HttpApi("获取文章列表", CacheSeconds = 600)]
        public APIResult GetArticles(ArticleType? type = null, DateTime? createStartDt = null, int pageSize = 10, int pageIndex = 1)
        {
            var items = Article.Search(type, null, null, null, createStartDt, null)
                .SortPage(t => t.CreateDt, false, pageIndex, pageSize)
                .ToList()
                .Cast(t => t.Export())
                ;
            return items.ToResult();
        }

        [HttpApi("获取文章详细信息", CacheSeconds = 60)]
        public APIResult GetArticle(long id)
        {
            return Article.GetDetail(id, Common.LoginUser?.ID).ToResult(ExportMode.Detail, "获取成功", "无此文章");
        }

        //----------------------------------------------
        // 签到打卡
        //----------------------------------------------
        [HttpApi("获取签到列表", true)]
        public APIResult GetSigns(long? userId, DateTime? startDt = null, int pageSize = 10, int pageIndex = 1)
        {
            var user = Common.TryGetUser(userId, Powers.SignView);
            if (startDt == null)
                startDt = DateTime.Now.TrimDay().AddDays(-10);
            var items = UserSign.Search(userId:user.ID, startDt:startDt)
                .SortPage(t => t.SignDt, false, pageIndex, pageSize)
                .ToList()
                .Cast(t => t.Export())
                ;

            // 计算今日签到可获得积分
            var todayContinueDays = UserSign.CalcUserTodayContinueSignDays(user);
            var score = UserSign.CalcScore(todayContinueDays);
            var o = new
            {
                user.LastSignDt,
                user.ContinueSignDays,
                TodaySignScore = score,
                Signs = items
            };

            return o.ToResult();
        }

        [HttpApi("增加签到", true)]
        public APIResult AddSign()
        {
            var user = Common.LoginUser;
            if (user.LastSignDt?.TrimDay() == DateTime.Today)
                return new APIResult(false, "今日已经签到啰~");
            return UserSign.Add(user.ID).ToResult(ExportMode.Detail, "签到成功");
        }

        //----------------------------------------------
        // 反馈
        //----------------------------------------------
        [HttpApi("增加反馈", true)]
        public APIResult AddFeedback(
            long userId,
            FeedType type, string module, string version, 
            string title, string content, 
            string image1 = "", string image2 = "", string image3 = "")
        {
            var u = User.Get(userId);
            var f = new Feedback();
            f.Status = FeedbackStatus.Create;
            f.Type = type;
            f.CreateDt = DateTime.Now;
            f.App = FeedApp.WechatMP;
            f.AppModule = module;
            f.AppVersion = version;
            f.Title = title;
            f.Content = content;
            f.UserID = userId;
            f.User = u?.NickName;
            f.Contacts = u?.Mobile;
            var urls = Uploader.UploadBase64Images("Feedbacks", image1, image2, image3);
            if (image1.IsNotEmpty())     f.Image1 = urls[0];
            if (image2.IsNotEmpty())     f.Image2 = urls[1];
            if (image3.IsNotEmpty())     f.Image3 = urls[2];
            f.Save();
            return f.ToResult(ExportMode.Detail, "反馈提交成功");
        }

        [HttpApi("获取反馈列表", true, 600)]
        public APIResult GetFeedbacks(long? userId, FeedbackStatus? status, int pageSize = 10, int pageIndex = 1)
        {
            var items = Feedback.Search(userId: userId, status: status)
                .SortPage(t => t.CreateDt, false, pageIndex, pageSize)
                .ToList()
                .Cast(t => t.Export())
                ;
            return items.ToResult();
        }

        [HttpApi("获取反馈详情", true)]
        public APIResult GetFeedback(long feedbackId)
        {
            return Feedback.Get(feedbackId).ToResult(ExportMode.Detail);
        }

        [HttpApi("处理反馈", true)]
        public APIResult HandleFeedback(long feedbackId, FeedbackStatus status, string reply, string image1 = "", string image2 = "")
        {
            var item = Feedback.Get(feedbackId);
            item.Status = status;
            item.Reply = reply;
            var urls = Uploader.UploadBase64Images("Feedbacks", image1, image2);
            if (image1.IsNotEmpty()) item.Image1 = urls[0];
            if (image2.IsNotEmpty()) item.Image2 = urls[1];
            item.Save();
            return item.ToResult();
        }

        //----------------------------------------------
        // 邀请
        //----------------------------------------------
        [HttpApi("获取邀请列表", true)]
        public APIResult GetInvites(long? shopId = null, long? inviterId = null, long? inviteeId=null, DateTime? createStartDt = null, int pageSize = 10, int pageIndex = 1)
        {
            var items = Invite.Search(inviterID:inviterId, inviteeId: inviteeId, createStartDt: createStartDt)
                .SortPage(t => t.CreateDt, false, pageIndex, pageSize)
                .ToList()
                .Cast(t => t.Export())
                ;
            return items.ToResult();
        }

        [HttpApi("获取邀请详情", true)]
        public APIResult GetInvite(long id)
        {
            return Invite.GetDetail(id).ToResult(ExportMode.Detail, "获取成功", "无此邀请数据");
        }

        [HttpApi("创建邀请记录", true)]
        public static APIResult AddInvite(long? inviteShopId, long? inviteUserId, string inviteUserMobile, InviteSource source = InviteSource.Wechat)
        {
            var user = Common.LoginUser;
            if (inviteUserId == null && inviteUserMobile.IsNotEmpty())
                inviteUserId = User.Get(mobile: inviteUserMobile).ID;
            var invite = Invite.Add(source, inviteShopId, inviteUserId, user.ID, user.Mobile, InviteStatus.Regist);
            Logic.AwardInviteUser(user.ID);
            return invite.ToResult(ExportMode.Detail, "邀请成功", "该用户已存在邀请记录");
        }




        //----------------------------------------------
        // 财务
        //----------------------------------------------
        [HttpApi("获取用户财务记录", true)]
        public static APIResult GetFinances(long? userId, int pageIndex = 0, int pageSize = 10)
        {
            var user = Common.TryGetUser(userId, Powers.FinanceView);
            var items = UserFinance.Search(userId: user.ID)
                .SortPage(t => t.CreateDt, false, pageIndex, pageSize)
                .ToList()
                .Cast(t => t.Export())
                ;
            return items.ToResult();
        }

        [HttpApi("获取财务详细信息（含维保信息）", true, 60)]
        public static APIResult GetFinance(long financeId)
        {
            return UserFinance.GetDetail(financeId).ToResult();
        }

    }
}