using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
//using EntityFramework;
//using EntityFramework.Extensions;
using Newtonsoft.Json;
using App.Utils;
using App.Wechats;
using App.Components;
using System.Text;
using App.Entities;
using Z.EntityFramework.Plus;

namespace App.DAL
{
    /// <summary>
    /// 基础用户表（登录+通用信息+会员信息）
    /// </summary>
    [UI("基础", "基础用户信息")]
    [Auth(DAL.Powers.UserView, DAL.Powers.UserNew, DAL.Powers.UserEdit, DAL.Powers.UserDelete)]
    public class User : EntityBase<User>, IDeleteLogic
    {
        //------------------------------------------------------
        // 属性
        //------------------------------------------------------
        // 通用属性
        [UI("是否在用")]                         public bool? InUsed { get; set; } = true;
        [UI("账户名（唯一）"), StringLength(50)] public string Name { get; set; }
        [UI("EMail")]                            public string Email { get; set; }
        [UI("密码（已加密）")]                   public string Password { get; set; }
        [UI("性别"), StringLength(10)]           public string Gender { get; set; }
        [UI("昵称"), StringLength(120)]          public string NickName { get; set; }
        [UI("实名"), StringLength(100)]          public string RealName { get; set; }
        [UI("姓名拼音")]                         public string NamePinYin { get; set; }
        [UI("姓名拼音首字母")]                   public string NamePinYinCap { get; set; }
        [UI("头像")]                             public string Avatar { get; set; }
        [UI("脸部照片")]                         public string Face { get; set; }
        [UI("QQ"), StringLength(50)]             public string QQ { get; set; }
        [UI("微信"), StringLength(50)]           public string Wechat { get; set; }
        [UI("电话"), StringLength(50)]           public string Tel { get; set; }
        [UI("手机（唯一）"), StringLength(50)]   public string Mobile { get; set; }
        [UI("地址"), StringLength(500)]          public string Address { get; set; }
        [UI("身份证"), StringLength(50)]         public string IDCard { get; set; }
        [UI("特长")]                             public string Specialty { get; set; }
        [UI("关注关键字")]                       public string Keywords { get; set; }
        [UI("备注")]                             public string Remark { get; set; }
        [UI("头衔")]                             public string Title { get; set; }
        [UI("生日")]                             public DateTime? Birthday { get; set; }
        [UI("最后登录日期")]                     public DateTime? LastLoginDt { get; set; }
        [UI("最后位置")]                         public string LastGPS { get; set; }              // 格式如：x,y
        [UI("用户角色ID列表")]                   public string Roles { get; set; }="";          // 格式如：",1,2,4,"
        [UI("数据来源")]                         public string Source { get; set; } = "Web";      // Web, Ding, Wechat....

        // 微信认证信息
        [UI("微信UnionID")]                      public string WechatUnionID { get; set; }
        [UI("微信小程序OpenID")]                 public string WechatMPID { get; set; }
        [UI("微信小程序Session")]                public string WechatMPSessionKey { get; set; }
        [UI("微信公众号OpenID")]                 public string WechatOPID { get; set; }
        [UI("微信公众号Session")]                public string WechatOPSessionKey { get; set; }
        [UI("微信公众号订阅")]                   public bool?  WechatOPSubscribe { get; set; }

        // 钉钉认证信息
        [UI("钉钉UserID")]                       public string DingUserID { get; set; }
        [UI("钉钉UnionID")]                      public string DingUnionID { get; set; }
        [UI("钉钉小程序OpenID")]                 public string DingMPID { get; set; }
        [UI("钉钉网页OpenID")]                   public string DingOPID { get; set; }


        // 扩展信息
        [UI("最后签到时间")]                     public DateTime? LastSignDt { get; set; }
        [UI("连续签到天数")]                     public int? ContinueSignDays { get; set; } = 0;
        [UI("积分")]                             public int? FinanceScore { get; set; } = 0;
        [UI("账户余额")]                         public double? FinanceBalance { get; set; }= 0;
        [UI("账户收入")]                         public double? FinanceIncome { get; set; } = 0;
        [UI("账户支出")]                         public double? FinanceOutcome { get; set; } = 0;

        // 员工信息
        [UI("就职日期")]                         public DateTime? HireDt { get; set; }
        [UI("离职日期")]                         public DateTime? LeaveDt { get; set; }
        [UI("部门")]                             public long? DeptID { get; set; }
        [UI("组织")]                             public long? OrgID { get; set; }

        // 邀请信息
        [UI("归属商店")]                         public long? ShopID { get; set; }
        [UI("邀请人")]                           public long? InviterID { get; set; }

        // 管理权限
        [UI("归属区域")]                         public long? AreaID { get; set; }


        // 导航属性
        public virtual Area Area { get; set; }
        public virtual Dept Dept { get; set; }
        public virtual Org Org { get; set; }
        public virtual Shop Shop { get; set; }
        public virtual User Inviter { get; set; }
        public virtual List<Title> Titles { get; set; }


        //------------------------------------------------------
        // 扩展属性
        //------------------------------------------------------
        /// <summary>带遮罩密码 </summary>
        [NotMapped, JsonIgnore]
        public string MobileMask {
            get
            {
                int len = Mobile.ToText().Length;
                if (len < 7) return "";
                else return Mobile.Substring(0, 3) + "****" + Mobile.Substring(len - 4, 4);
            }
        }

        /// <summary>用户年龄 </summary>
        [NotMapped, UI("年龄")]
        public int Age
        {
            get
            {
                if (Birthday != null)
                {
                    DateTime now = DateTime.Today;
                    int age = now.Year - Birthday.Value.Year;
                    if (now.AddYears(-age) < Birthday)
                        age--;
                    return age;
                }
                else
                    return 0;
            }
        }

        /// <summary>用户拥有的角色ID列表 </summary>
        [NotMapped]
        public List<long> RoleIds
        {
            get
            {
                var ids = new List<long>();
                if (Roles.IsNotEmpty())
                    foreach (var id in Roles.SplitLong())
                    {
                        if (!ids.Contains(id))
                            ids.Add(id);
                    }
                return ids;
            }
            set
            {
                var txt = ",";
                foreach (var role in value)
                    txt += string.Format("{0},", (int)role);
                this.Roles = txt;
            }
        }

        /// <summary>角色名称</summary>
        public string RoleNames
        {
            get
            {
                var sb = new StringBuilder();
                this.RoleIds.ForEach(t => sb.AppendFormat("{0},", Role.All.FirstOrDefault(r => r.ID == t)?.Name));
                return sb.ToString().TrimEnd(',');
            }
        }


        /// <summary>构造检索角色字符串（前后都有逗号）</summary>
        static string GetSearchRoleText(long roleId)
        {
            return string.Format(",{0},", roleId);
        }


        /// <summary>用户拥有的头衔字符串（用逗号隔开） </summary>
        public string TitleNames
        {
            get
            {
                string titles = "";
                foreach (var item in this.Titles)
                    titles += item.Name + ",";
                titles = titles.TrimEnd(',');
                return titles;
            }
        }

        // 用户拥有的权限
        [NotMapped]
        public List<Powers> Powers { get; set; }// => IO.GetCache(this.UniID + "-Power", () => GetUserPowers(this));

        // 用户可访问的菜单
        [NotMapped]
        public List<Menu> Menus { get; set; }  // => GetAllowMenus(this.Powers);


        //------------------------------------------------------
        // 方法重载
        //------------------------------------------------------
        /// <summary>导出</summary>
        public override object Export(ExportMode type = ExportMode.Normal)
        {
            return new
            {
                // Simple
                this.ID,
                this.Name,
                this.NickName,
                this.Email,
                this.Title,
                this.Mobile,
                this.CreateDt,
                this.LastLoginDt,
                this.LastSignDt,
                this.LastGPS,
                this.ContinueSignDays,
                this.DeptID,
                Dept = this.Dept?.Name,
                Avatar = Asp.ResolveFullUrl(this.Avatar),
                Roles = this.Roles,
                this.Keywords,

                // normal
                AreaID            = type.HasFlag(ExportMode.Normal) ? AreaID             : null,
                AreaName          = type.HasFlag(ExportMode.Normal) ? Area?.FullName     : null,
                WechatUnionID     = type.HasFlag(ExportMode.Normal) ? WechatUnionID      : null,
                WechatMPID        = type.HasFlag(ExportMode.Normal) ? WechatMPID         : null,
                WechatOPID        = type.HasFlag(ExportMode.Normal) ? WechatOPID         : null,
                WechatOPSubscribe = type.HasFlag(ExportMode.Normal) ? WechatOPSubscribe  : null,
                DingUserID        = type.HasFlag(ExportMode.Normal) ? DingUserID         : null,
                DingUnionID       = type.HasFlag(ExportMode.Normal) ? DingUnionID        : null,
                DingMPID          = type.HasFlag(ExportMode.Normal) ? DingMPID           : null,
                DingOPID          = type.HasFlag(ExportMode.Normal) ? DingOPID           : null,
                ShopID            = type.HasFlag(ExportMode.Normal) ? ShopID             : null,
                RealName          = type.HasFlag(ExportMode.Normal) ? RealName           : null,

                // detail
                BirthDay          = type.HasFlag(ExportMode.Detail) ? Birthday?.ToString("yyyy-MM-dd") : null,
                FinanceBalance    = type.HasFlag(ExportMode.Detail) ? FinanceBalance     : null,
                FinanceIncome     = type.HasFlag(ExportMode.Detail) ? FinanceIncome      : null,
                FinanceOutcome    = type.HasFlag(ExportMode.Detail) ? FinanceOutcome     : null,
                FinanceScore      = type.HasFlag(ExportMode.Detail) ? FinanceScore       : null,
                IDCard            = type.HasFlag(ExportMode.Detail) ? IDCard             : null,
                ShopName          = type.HasFlag(ExportMode.Detail) ? Shop?.Name         : null,
                InviterID         = type.HasFlag(ExportMode.Detail) ? InviterID          : null,
                InviterName       = type.HasFlag(ExportMode.Detail) ? Inviter?.NickName  : null,
                InviterMobile     = type.HasFlag(ExportMode.Detail) ? Inviter?.Mobile    : null,
                ManageAreas       = type.HasFlag(ExportMode.Detail) ? GetManageAreas().Cast(t=> t.Export(ExportMode.Simple)) : null,
                ManageDepts       = type.HasFlag(ExportMode.Detail) ? GetManageDepts().Cast(t => t.Export(ExportMode.Simple)) : null,
            };
        }

        //------------------------------------------------------
        // 检索
        //------------------------------------------------------
        /// <summary>获取用户管辖区域</summary>
        public List<Area> GetManageAreas()
        {
            return UserArea.Search(this.ID).Select(t => t.Area).ToList();
        }

        /// <summary>用户授权的区域列表（包括子区域）</summary>
        public List<Area> GetAllowedAreas()
        {
            if (this.HasPower(DAL.Powers.Admin))
                return Area.All;
            var areaIds = this.GetManageAreas().Cast(t => t.ID);
            return Area.All.GetDescendants(areaIds);
        }


        /// <summary>获取用户管辖部门</summary>
        public List<Dept> GetManageDepts()
        {
            return UserDept.Search(this.ID).Select(t => t.Dept).ToList();
        }

        /// <summary>设置管辖部门</summary>
        public void SetManagerDepts(List<long> ids)
        {
            UserDept.Set.Where(t => t.UserID==this.ID).Delete();
            ids.Each(id => new UserDept() { UserID = this.ID, DeptID = id }.Save());
        }
        /// <summary>设置管辖区域</summary>
        public void SetManagerAreas(List<long> ids)
        {
            UserArea.Set.Where(t => t.UserID==this.ID).Delete();
            ids.Each(id => new UserArea() { UserID = this.ID, AreaID = id }.Save());
        }

        // 获取查找对象（包含附属库）
        private static IQueryable<User> GetQuery()
        {
            return Set.Include(t => t.Dept).Include(t => t.Titles).Include(t => t.Shop).Include(t => t.Inviter).Include(t => t.Area);
        }

        // 搜索用户
        [Param("name", "用户名或昵称")]
        [Param("mobile", "手机")]
        [Param("role", "角色")]
        public static IQueryable<User> Search(
            string name = "", string mobile = "",
            long? role = null, bool? inUsed = true,
            int? birthMonth = null, long? deptId = null, long? titleId = null, 
            bool includeAdmin = false,
            long? shopId = null
            )
        {
            var r = role==null ? null : GetSearchRoleText(role.Value);
            var q = GetQuery();
            if (!includeAdmin)         q = q.Where(t => t.Name != "admin");
            if (name.IsNotEmpty())     q = q.Where(t => t.Name.Contains(name) || t.NickName.Contains(name) || t.RealName.Contains(name));
            if (deptId != null)        q = q.Where(t => t.Dept.ID == deptId);
            if (titleId != null)       q = q.Where(t => t.Titles.Any(o => o.ID == titleId));
            if (role != null)          q = q.Where(t => t.Roles.Contains(r));
            if (inUsed != null)        q = q.Where(t => t.InUsed == inUsed.Value);
            if (mobile.IsNotEmpty())   q = q.Where(t => t.Mobile.Contains(mobile));
            if (birthMonth != null)    q = q.Where(t => t.Birthday.Value.Month == birthMonth);
            if (shopId != null)        q = q.Where(t => t.ShopID == shopId);
            return q;
        }

        /// <summary>查找商店的管理人</summary>
        public static List<User> SearchShopOwners(long? shopId)
        {
            var owners = new List<User>();
            if (shopId != null)
            {
                var users = Search(shopId: shopId).ToList();
                foreach (var user in users)
                {
                    if (user.HasPower(DAL.Powers.AdminShop) || user.HasPower(DAL.Powers.AdminRepairShop))
                        owners.Add(user);
                }
            }
            return owners;
        }

        /*
        /// <summary>查找雇员（非客户都算雇员）</summary>
        public static IQueryable<User> SearchEmployee(
            long? shopId = null,
            string name = "", string mobile = "", Role? role = null, 
            bool? inUsed = true, long? deptId = null,
            long? titleId = null, int? birthMonth = null
            )
        {
            var r = GetSearchRoleText(Role.Employees);
            IQueryable<User> q = Search(
                name:name, 
                mobile:mobile, 
                role: role, 
                inUsed: inUsed, 
                birthMonth: birthMonth, 
                deptId: deptId, 
                titleId: titleId,
                shopId: shopId
                );
            q = q.Where(t => t.Name != "admin");
            q = q.Where(t => !t.RolesText.Contains(r));  // 非客户都算员工
            return q;
        }
        */

        /// <summary>查找拥有某个角色的用户</summary>
        public static IQueryable<User> SearchByRole(long roleId)
        {
            var r = GetSearchRoleText(roleId);
            IQueryable<User> q = ValidSet
                .Where(t => t.Roles.Contains(r))
                .OrderBy(t => t.NickName)
                ;
            return q;
        }

        /// <summary>查找拥有某个权限的用户</summary>
        public static List<User> SearchByPower(Powers powerId)
        {
            var roles = RolePower.Search(t => t.PowerID == powerId).Select(t => t.RoleID).ToList();
            var users = new List<User>();
            foreach (var role in roles)
            {
                var us = SearchByRole(role).ToList();
                users = users.Union(us);
            }
            return users;
        }





        /// <summary>获取用户（尝试根据参数顺序获取，如果获取不到则尝试下一个条件）</summary>
        public static User Get(
            long? id = null, string name = "", string mobile = "",
            string wechatUnionId = "", string wechatMPId = "", string wechatOpenId = "",
            bool? inUsed = true
            )
        {
            User user = null;
            var q = GetQuery();
            if (inUsed != null)                             q = q.Where(t => t.InUsed == inUsed);
            if (user == null && id != null)                 user = q.FirstOrDefault(t => t.ID == id);
            if (user == null && name.IsNotEmpty())          user = q.FirstOrDefault(t => t.Name == name);
            if (user == null && mobile.IsNotEmpty())        user = q.FirstOrDefault(t => t.Mobile == mobile);
            if (user == null && wechatUnionId.IsNotEmpty()) user = q.FirstOrDefault(t => t.WechatUnionID == wechatUnionId);
            if (user == null && wechatMPId.IsNotEmpty())    user = q.FirstOrDefault(t => t.WechatMPID == wechatMPId);
            if (user == null && wechatOpenId.IsNotEmpty())  user = q.FirstOrDefault(t => t.WechatOPID == wechatOpenId);
            return user;
        }

        /// <summary>获取用户详情</summary>
        public static User GetDetail(
            long? id=null, string name = "", string mobile = "", 
            string wechatMPId = "", string wechatUnionId="", string wechatOpenId="",
            bool? inUsed = true
            )
        {
            var o = Get(id, name, mobile, wechatUnionId, wechatMPId, wechatOpenId, inUsed);
            if (o != null)
            {
                o.Powers = GetUserPowers(o);
                o.SetMenus();
            }
            return o;
        }


        /// <summary>获取或创建微信认证账户</summary>
        public static User GetOrCreateWechatUser(WechatUser info)
        {
            User user = User.Get(wechatUnionId: info.unionid, wechatMPId: info.mpId, wechatOpenId: info.opId);
            if (user == null)
                user = User.CreateCustomer(info);
            return user;
        }

        /// <summary>创建微信认证消费者用户</summary>
        public static User CreateCustomer(WechatUser info, string mobile="", string password="")
        {
            if (password.IsEmpty())
                password = SiteConfig.Instance.DefaultPassword;

            User user = new User();
            user.Name = Guid.NewGuid().ToString("N");
            user.Password = PasswordHelper.CreateDbPassword(password);
            user.CreateDt = DateTime.Now;
            //user.Roles = new List<Role>() { Role.Customers };
            user.InUsed = true;
            user.Mobile = mobile;
            user.Save();

            // 微信信息填充
            return user.EditByWechat(info);
        }

        
        /// <summary>用微信信息修改用户信息</summary>
        public User EditByWechat(WechatUser info)
        {
            if (info == null)
                return this;

            if (info.mpSessionKey.IsNotEmpty())     this.WechatMPSessionKey = info.mpSessionKey;
            if (info.opSessionKey.IsNotEmpty())     this.WechatOPSessionKey = info.opSessionKey;
            if (this.WechatMPID.IsEmpty())          this.WechatMPID = info.mpId;
            if (this.WechatOPID.IsEmpty())        this.WechatOPID = info.opId;
            if (this.WechatUnionID.IsEmpty())       this.WechatUnionID = info.unionid;
            if (this.NickName.IsEmpty())            this.NickName = info.nickname;
            if (this.Avatar.IsEmpty())               this.Avatar = info.headimgurl;
            if (this.Gender.IsEmpty())              this.Gender = info.sex == 1 ? "男" : "女";
            return this.Save();
        }

        //------------------------------------------------------
        // 菜单
        //------------------------------------------------------
        // 用户 & 菜单
        /// <summary>设置用户可访问的菜单</summary>
        public void SetMenus()
        {
            var powers = this.Powers;
            var menus = new List<Menu>();
            foreach (var menu in Menu.All)
            {
                if (menu != null)
                    if (menu.ViewPower == null || powers.Contains(menu.ViewPower.Value))
                        menus.Add(menu);
            }
            this.Menus = menus;
        }

        //------------------------------------------------------
        // 角色、权限
        //------------------------------------------------------
        /// <summary>是否具有某个权限</summary>
        public bool HasPower(Powers power)
        {
            if (this.Powers == null)
                this.Powers = GetUserPowers(this);
            return this.Powers.Contains(power);
        }


        /// <summary>设置角色拥有的权限列表</summary>
        public static void SetRolePowers(long roleId, List<Powers> powers)
        {
            RolePower.Set.Where(t => t.RoleID == roleId).Delete();
            foreach (var power in powers)
            {
                var item = new RolePower() { RoleID = roleId, PowerID = power };
                item.Save();
            }
        }


        // 获取用户权限（admin拥有所有权限、普通用户根据角色来获取权限）
        public static List<Powers> GetUserPowers(User user)
        {
            var powers = new List<Powers>();
            if (user.Name == "admin")
                powers = typeof(Powers).GetEnums<Powers>();
            else
            {
                var roles = user.RoleIds;
                RolePower.Search(t => roles.Contains(t.RoleID)).ToList().ForEach(t => powers.Add(t.PowerID));
            }
            return powers;
        }


        //------------------------------------------------------
        // 其它
        //------------------------------------------------------
        // 设置用户密码（不比较旧密码）
        public static void SetPassword(User user, string password)
        {
            if (user != null)
            {
                user.Password = PasswordHelper.CreateDbPassword(password);
                Db.SaveChanges();
            }
        }


        /// <summary>计算用户财务信息</summary>
        public void CalcFinance(double money)
        {
            // 更新用户财务统计信息
            this.FinanceBalance += money;
            if (money >= 0)
                this.FinanceIncome += Math.Abs(money);
            else
                this.FinanceOutcome += Math.Abs(money);
            this.Save();
        }


        /// <summary>计算用户积分信息</summary>
        public void CalcScore(int score)
        {
            if (score == 0)
                return;
            else if (score > 0)
                this.FinanceScore += score;
            else
            {
                score = -score;
                if (this.FinanceScore < score)
                    throw new Exception("积分不足");
                this.FinanceScore -= score;
            }
            this.Save();
        }

        //------------------------------------------------------
        // 数据修复
        //------------------------------------------------------
        /// <summary>修正角色信息</summary>
        public static void FixRoles()
        {
            var users = User.Set.ToList();
            foreach (var user in users)
            {
                user.RoleIds = user.RoleIds;
                user.Save();
            }
        }

        /// <summary>修正并补足区域信息</summary>
        public User FixArea()
        {
            if (this.ShopID != null)
            {
                var shopAreaID = Shop.Get(this.ShopID)?.AreaID;
                if (this.AreaID == null)
                    this.AreaID = shopAreaID;
            }
            return this;
        }

        /// <summary>获取允许访问的文章目录（包含子目录）</summary>
        public List<long> GetAllowedArticleDirIds()
        {
            List<long> dirIds = GetAllowedArticleDirIdsOutChildren();
            // 获取子目录ID
            return  ArticleDir.GetDescendantIds(dirIds);
        }

        /// <summary>获取允许访问的文章目录（不包含子目录）</summary>
        public List<long> GetAllowedArticleDirIdsOutChildren()
        {
            // 根据角色获取可访问的目录
            var roleIds = this.RoleIds;
            var dirIds = new List<long>();
            foreach (var roleId in roleIds)
            {
                var ids = ArticleDirRole.All.Where(t => t.RoleID == roleId).Select(t => t.ArticleDirID).ToList();
                //items.ForEach(item => {if (!dirs.Contains(item)) dirs.Add(item); });
                foreach (var id in ids)
                {
                    if (!dirIds.Contains(id))
                        dirIds.Add(id);
                }
            }
            return dirIds;
        }
    }
}