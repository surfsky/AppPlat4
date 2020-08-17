using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using EntityFramework.Extensions;
using System.Data.Entity;
using System.ComponentModel;
using App.Utils;
using App.Entities;


namespace App.DAL
{
    /// <summary>
    /// 积分类型
    /// </summary>
    public enum ScoreType : int
    {
        [UI("签到得分")] Sign = 0,
        [UI("邀请得分")] Invite = 1,
        [UI("绑定手机得分")] BindMobile = 2,
        [UI("兑换")]     Exchange = 10,
    }

    /// <summary>
    /// 用户积分记录
    /// </summary>
    [UI("商城", "用户积分记录")]
    public class UserScore : EntityBase<UserScore>
    {
        [UI("类型")]                     public ScoreType? Type { get; set; }
        [UI("类型")]                     public string TypeName { get { return Type.GetTitle(); } }
        [UI("用户")]                     public long? UserID { get; set; }
        [UI("费用")]                     public int? Score { get; set; } = 0;
        [UI("备注")]                     public string Remark { get; set; }
        [UI("来源")]                     public string SourceID { get; set; }  // 如 Invite-231

        // 关联属性
        [UI("用户")] public virtual User User { get; set; }


        //----------------------------------------------------
        // 基础操作
        //----------------------------------------------------
        /// <summary>获取详细对象</summary>
        public override object Export(ExportMode type = ExportMode.Normal)
        {
            return new
            {
                this.ID,
                this.Type,
                this.TypeName,
                this.UserID,
                this.CreateDt,
                this.SourceID,
                this.Score,
                this.Remark,
                this.User?.NickName
            };
        }

        /// <summary>获取详细对象</summary>
        public new static UserScore GetDetail(long id)
        {
            return Set.Include(t => t.User).FirstOrDefault(t => t.ID == id);
        }


        /// <summary>查询</summary>
        public static IQueryable<UserScore> Search(
            long? userId = null, string userName = null,
            ScoreType? type = null,
            DateTime? startDt = null,
            DateTime? endDt = null
            )
        {
            IQueryable<UserScore> q = Set.Include(t => t.User);
            if (!userName.IsEmpty()) q = q.Where(t => t.User.NickName.Contains(userName));
            if (userId != null)            q = q.Where(t => t.UserID == userId);
            if (type != null)              q = q.Where(t => t.Type == type);
            if (startDt != null)           q = q.Where(t => t.CreateDt >= startDt);
            if (endDt != null)             q = q.Where(t => t.CreateDt <= endDt);
            return q;
        }

        /// <summary>新增记录（会抛出异常）</summary>
        /// <param name="money">金额（正数为收入，负数为支出）</param>
        public static UserScore Add(ScoreType type, long? userId, int score, string sourceId="", string remark="")
        {
            var user = User.Get(userId);
            if (user != null)
            {
                // 计算积分（会抛出异常）
                user.CalcScore(score);

                // 新增财务记录
                var item = new UserScore();
                item.Type = type;
                item.UserID = userId;
                item.SourceID = sourceId;
                item.Score = score;
                item.Remark = remark;
                item.CreateDt = DateTime.Now;
                item.Save();
                return item;
            }
            return null;
        }

    }
}