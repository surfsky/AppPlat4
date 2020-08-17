using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using EntityFramework.Extensions;
using System.Data.Entity;
using System.ComponentModel;
using App.Utils;
using System.Collections;
using App.Entities;

namespace App.DAL
{

    /// <summary>
    /// 用户签到信息表
    /// </summary>
    [UI("社交", "用户签到")]
    public class UserSign : EntityBase<UserSign>
    {
        [UI("用户")]                           public long? UserID { get; set; }
        [UI("创建时间")]                       public DateTime? SignDt {get; set;}
        [UI("签到积分")]                       public int? Score { get; set; }
        [UI("连续签到天数")]                   public int ContinueSignDays { get; set; } = 0;

        public virtual User User   { get; set; }

        //-----------------------------------------------
        // 公共方法
        //-----------------------------------------------
        public override object Export(ExportMode type = ExportMode.Normal)
        {
            return new
            {
                this.ID,
                this.UserID,
                this.User?.NickName,
                this.SignDt,
                this.Score,
                this.ContinueSignDays
            };
        }

        public new static UserSign GetDetail(long id)
        {
            return Set.Include(t => t.User).Where(t => t.ID == id).FirstOrDefault();
        }


        // 查询
        public static IQueryable<UserSign> Search(
            long? userId = null,
            string userName = null,
            DateTime? startDt = null, 
            DateTime? endDt = null
            )
        {
            IQueryable<UserSign> q = Set.Include(t => t.User);
            if (userId != null)             q = q.Where(t => t.UserID == userId);
            if (!userName.IsEmpty())        q = q.Where(t => t.User.NickName.Contains(userName));
            if (startDt != null)            q = q.Where(t => t.SignDt >= startDt);
            if (endDt != null)              q = q.Where(t => t.SignDt <= endDt);
            return q;
        }

        /// <summary>统计</summary>
        public static List<StatItem> Stat(long? shopId, DateTime? startDt, DateTime? endDt)
        {
            IQueryable<UserSign>       q = Set.Include(t => t.User).Include(t => t.User.Shop);
            if (shopId != null)        q = q.Where(t => t.User.ShopID == shopId);
            if (startDt != null)       q = q.Where(t => t.SignDt >= startDt);
            if (endDt != null)         q = q.Where(t => t.SignDt <= endDt);

            return q
                .GroupBy(t => new
                {
                    //Shop = t.User.Shop.Name,
                    Day = DbFunctions.TruncateTime(t.SignDt).Value
                })
                .Select(t => new
                {
                    //Shop = t.Key.Shop,
                    Day = t.Key.Day,
                    Cnt = t.Count()
                })
                .OrderBy(t => new
                {
                    //t.Shop,
                    t.Day
                })
                .ToList()
                .Select(t => new StatItem("", t.Day.ToString("MMdd"), t.Cnt))
                .ToList();
            ;
        }

        // 添加签名
        public static UserSign Add(long userId)
        {
            User user = User.Get(userId);
            if (user == null)
                return null;

            // 增加用户积分
            var now = DateTime.Now;
            int continueSignDays = CalcUserTodayContinueSignDays(user);
            int score = CalcScore(continueSignDays);
            user.ContinueSignDays = continueSignDays;
            //user.FinanceScore += score;
            user.LastSignDt = now;
            user.Save();

            //
            var sign = new UserSign();
            sign.UserID = userId;
            sign.SignDt = now;
            sign.ContinueSignDays = continueSignDays;
            sign.Score = score;
            sign.Save();

            // 
            UserScore.Add(ScoreType.Invite, userId, score, sign.UniID);
            return sign;
        }

        // 计算用户今天签到的连续签到天数
        public static int CalcUserTodayContinueSignDays(User user)
        {
            var lastSignDt = user.LastSignDt;
            if (lastSignDt == null)
                return 1;
            if (lastSignDt.Value.TrimDay() < DateTime.Today.AddDays(-1))
                return 1;
            int n = user.ContinueSignDays ?? 0;
            return n % 10 + 1;   // n 天一个循环
        }

        /// <summary>计算积分（未完成）</summary>
        public static int CalcScore(int continueSignDays)
        {
            switch (continueSignDays)
            {
                case 1: return 1;
                case 2: return 2;
                case 3: return 3;
                case 4: return 4;
                case 5: return 5;
                case 6: return 6;
                case 7: return 7;
                case 8: return 8;
                case 9: return 9;
                case 10: return 10;
                default: return 1;
            }
        }
    }
}