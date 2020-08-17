using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data.Entity;
using System.ComponentModel;
using App.Utils;
using System.Linq.Expressions;
using Newtonsoft.Json;
using System.Collections;
using App.Components;
using App.Wechats;
using App.Entities;

namespace App.DAL
{

    /// <summary>
    /// 订单支付方式
    /// </summary>
    public enum OrderPayMode : int
    {
        [UI("现金")]       Cash = 0,
        [UI("支付宝")]     Alipay = 1,
        [UI("微信支付")]   WechatPay = 2,
        [UI("银联支付")]   ChinaUnionPay = 3,
        [UI("用户卡")]     UserCard = 4
    }



    /// <summary>
    /// 订单。
    /// 订单详情参看 OrderItem 表。
    /// 如果是简单的单商品订单系统，直接在summary上填写商品信息即可。
    /// </summary>
    [UI("商城", "订单")]
    public partial class Order : EntityBase<Order>, IDeleteLogic
    {
        [UI("是否在用")]         public bool? InUsed { get; set; } = true;
        [UI("类别")]             public ProductType? Type { get; set; }
        [UI("类别名称")]         public string TypeName { get { return Type.GetTitle(); } }
        [UI("状态")]             public int? Status { get; set; }
        [UI("状态名")]           public string StatusName { get; set; }
        [UI("接单店铺")]         public long? ShopID { get; set; }
        [UI("处理店铺")]         public long? HandleShopID { get; set; }
        [UI("当前处理店铺")]     public long? CurrentStepShopID { get; set; }
        [UI("用户")]             public long? UserID { get; set; }
        [UI("过期时间")]         public DateTime? ExpireDt { get; set; }      // 订单创建后，若无操作自动过期截止时间
        [UI("备注")]             public string Remark { get; set; }


        [UI("最后状态时间")]     public DateTime? StatusDt { get; set; }
        [UI("订单流水号")]       public string SerialNo { get; set; } 
        [UI("预支付号")]         public string PrepayId { get; set; }         // 微信支付接口返回的预支付订单号 
        [UI("最终评价")]         public int? LastRate { get; set; }
        [UI("最终评价")]         public string LastRateName { get { return ((RateType?)LastRate).GetTitle(); } }

        // 商品相关
        [UI("首商品")]           public long? FirstProductID { get; set; }      // 首商品（出于便捷考虑，此处填写订单的首条商品记录）
        [UI("概述")]             public string Summary { get; set; }           // 概述，在订单生成时自动插入。格式如：xxxx 等n件商品
        [UI("总数目")]           public int? TotalAmount { get; set; }=0;
        [UI("总费用")]           public double? TotalMoney { get; set; }=0;
        [UI("图片")]             public string CoverImage { get; set; }

        // 支付相关
        [UI("支付时间")]         public DateTime? PayDt { get; set; }
        [UI("支付方式")]         public OrderPayMode? PayMode { get; set; }
        [UI("支付方式")]         public string PayModeName { get { return PayMode.GetTitle(); } }
        [UI("实付费用")]         public double? PayMoney { get; set; }
        [UI("支付信息")]         public string PayInfo { get; set; }

        // 附加信息
        [UI("设备")]             public string ApptDevice { get; set; }
        [UI("姓名")]             public string ApptUser { get; set; }
        [UI("手机")]             public string ApptMobile { get; set; }
        [UI("时间")]             public DateTime? ApptDt { get; set; }
        [UI("备注")]             public string ApptRemark { get; set; }


        // 导航属性
        public virtual Shop Shop { get; set; }
        public virtual Shop HandleShop { get; set; }
        public virtual Shop CurrentStepShop { get; set; }
        public virtual User User { get; set; }
        public virtual Product FirstProduct { get; set; }
        public virtual List<OrderItem> Items { get; set; }

        /*
        /// <summary>
        /// 转化为json字符串。
        /// 经测试，若直接使用扩展方法 order.ToJson (实际调用 App.Utils.Convertor.ToJson) 会导出非常庞大的json。
        /// 为了避免误用，对于 Order 重写该方法。
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return Convertor.ToJson(this.Export(false));
        }
        */

        /// <summary>获取导出对象</summary>
        /// <param name="type">概述还是详细信息</param>
        public override object Export(ExportMode type = ExportMode.Normal)
        {
            return new
            {
                this.ID,
                this.SerialNo,
                this.PrepayId,
                this.Type,
                this.TypeName,
                this.Status,
                this.StatusName,
                this.StatusDt,
                this.Summary,
                this.UserID,
                UserName = this.User?.NickName,
                this.ShopID,
                ShopName = this.Shop?.AbbrName,
                this.HandleShopID,
                HandleShopName = this.HandleShop?.AbbrName,
                this.CreateDt,
                this.ExpireDt,
                this.TotalMoney,
                this.PayMoney,
                this.FirstProductID,
                this.CoverImage,
                this.LastRate,
                this.LastRateName,
                this.Remark,

                // 附加信息
                this.ApptDevice,
                this.ApptUser,
                this.ApptMobile,
                this.ApptDt,
                this.ApptRemark,

                // 附属集合
                Items     = type.HasFlag(ExportMode.Detail) ? this.Items?.Cast(t => t.Export(type)) : null,
                Histories = type.HasFlag(ExportMode.Detail) ? this.Histories?.Cast(t => t.Export()) : null,
                NextSteps = type.HasFlag(ExportMode.Detail) ? this.GetNextSteps(null).Cast(t => t.Export()) : null,
                //Rates = detail ? this.GetRates().Cast(t => t.Export(true)) : null
            };
        }

        /// <summary>订单评价</summary>
        public List<OrderRate> GetRates()
        {
            return OrderRate.Search(null, this.ID).ToList();
        }

        //-----------------------------------------------
        // 实例方法
        //-----------------------------------------------
        /// <summary>增加操作历史</summary>
        public History AddHistory(long? userId, string userName = "", string userMobile = "")
        {
            if (userName.IsEmpty() && userMobile.IsEmpty())
            {
                var user = User.Get(userId);
                userName = user?.NickName;
                userMobile = user?.Mobile;
            }
            return History.AddHistory(this.UniID, userId, this.Status.GetTitle(), (int)this.Status, userName, userMobile);
        }

        /// <summary>增加评价</summary>
        public Order AddRate(RateType rate, string comment)
        {
            OrderRate.Add(this.ID, rate, comment);
            this.LastRate = (int)rate;
            this.Save();
            this.AddHistory(this.User.ID, this.User.NickName, this.User.Mobile, "评价订单");
            //History.AddHistory(this.UniID, this.UserID, "评价订单");
            return this;
        }



        //-----------------------------------------------
        // 公共静态方法
        //-----------------------------------------------
        /// <summary>生成订单流水号</summary>
        public static string BuildSerialNo()
        {
            return Sequence.Generate(SequenceType.Order);
            //var fix = StringHelper.BuildRandomText("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 4);
            //return string.Format("{0:yyyyMMddHHmmssfffffff}{1}", DateTime.Now, fix);
        }

        /// <summary>获取订单（优先用id，如果id未填写才用serialNo）</summary>
        public static Order GetDetail(long? id = null, string serialNo = "")
        {
            var q = Set.Include(t => t.Shop).Include(t => t.HandleShop).Include(t => t.User).Include(t => t.FirstProduct).Include(t => t.Items);
            Order item;
            if (id != null)
                item = q.Where(t => t.ID == id).FirstOrDefault();
            else
                item = q.Where(t => t.SerialNo == serialNo).FirstOrDefault();
            item?.FixItem();
            return item;
        }

        /// <summary>修正订单</summary>
        public override object FixItem()
        {
            // 补全数据
            if (this.FirstProductID == null || this.Type == null || this.Status == null || this.CoverImage.IsEmpty())
            {
                // 修正统计数据
                if (this.FirstProductID == null || this.CoverImage.IsEmpty())
                    UpdateStat();

                // 修正错误的空数据
                if (this.Type == null)
                {
                    if (this.FirstProductID != null)
                    {
                        var p = Product.Get(this.FirstProductID);
                        this.Type = p?.Type;
                    }
                }
                if (this.Status == null)
                {
                    this.Status = (int)OrderStatus.Create;
                    this.StatusName = OrderStatus.Create.GetTitle();
                }
                this.Save();
                IO.Trace("成功修正订单{0}: {1}", this.ID, this.Summary);
            }

            // 状态控制
            try
            {
                var now = DateTime.Now;
                if (this.Status == (int)OrderStatus.Create)
                {
                    if (this.ExpireDt != null && now > this.ExpireDt)
                    {
                        this.ChangeStatusEnum(OrderStatus.Cancel, null, "超时取消");
                        IO.Console("过期订单 {0}: {1}", this.ID, this.Summary);
                    }
                }
            }
            catch { }
            return this;
        }

        /// <summary>修正订单支付金额</summary>
        public Order FixPayMoney()
        {
            // 修正订单支付金额
            if (this.PayMoney == null || this.PayMoney <= 0.00001)
            {
                this.PayMoney = this.TotalMoney;
                this.Save();
            }
            return this;
        }

        /// <summary>获取最新数据</summary>
        public static List<Order> SearchNewest(int minutes, User user)
        {
            var dt = DateTime.Now.AddMinutes(-minutes);
            return Search().Where(t => t.StatusDt > dt).ToList();
        }

        // 查找
        public static IQueryable<Order> Search(
            int? status=null, string userName="", 
            DateTime? startDt=null, DateTime? endDt=null,
            long? userId=null, string userMobile="", string serialNo="",
            long? shopId=null, ProductType? type=null
            )
        {
            IQueryable<Order> q = Set.Include(t => t.Shop).Include(t => t.HandleShop).Include(t => t.FirstProduct).Include(t => t.User);
            q = q.Where(t => t.InUsed != false);
            if (!String.IsNullOrEmpty(userName))   q = q.Where(t => t.User.Name.Contains(userName));
            if (!String.IsNullOrEmpty(userMobile)) q = q.Where(t => t.User.Mobile.Contains(userMobile));
            if (!String.IsNullOrEmpty(serialNo))   q = q.Where(t => t.SerialNo.Contains(serialNo));
            if (type != null)                      q = q.Where(t => t.Type == type);
            if (status != null)                    q = q.Where(t => t.Status == status);
            if (startDt != null)                   q = q.Where(t => t.CreateDt >= startDt);
            if (endDt != null)                     q = q.Where(t => t.CreateDt <= endDt);
            if (userId != null && userId != -1)    q = q.Where(t => t.UserID == userId);
            if (shopId  != null)                   q = q.Where(t => t.ShopID == shopId || t.HandleShopID == shopId);

            return q;
        }

        //-----------------------------------------------
        // 订单处理流程
        //-----------------------------------------------
        static int GetExpireMinutes(ProductType type)
        {
            switch (type)
            {
                case ProductType.Insurance: return 60;  // 需立即支付
                case ProductType.Check: return 60;      // 需立即支付
                case ProductType.Repair: return 60*24;  // 需要商家介入
            }
            return 60;
        }

        /// <summary>创建订单</summary>
        public static Order Create(ProductType type, long userId, long? shopId)
        {
            shopId = shopId ?? User.Get(userId).ShopID;
            var now = DateTime.Now;
            var order = new Order();
            order.Type = type;
            order.UserID = userId;
            order.CreateDt = now;
            order.ExpireDt = now.AddMinutes(GetExpireMinutes(type));
            order.StatusDt = now;
            order.SerialNo = BuildSerialNo();
            order.ShopID = shopId;
            order.HandleShopID = shopId;
            order.Save();
            order.ChangeStatusEnum(OrderStatus.Create, userId);
            return order;
        }

        /// <summary>更改订单项</summary>
        public OrderItem ChangeItem(OrderItem oldItem, long productSpecId, int n, string title = "", double? money = null)
        {
            RemoveItem(oldItem);
            return AddItem(productSpecId, n, title, money);
        }

        /// <summary>移除订单项</summary>
        public void RemoveItem(OrderItem oldItem)
        {
            try
            {
                // 恢复库存
                if (oldItem.ProductSpecID != null && oldItem.Amount != null)
                {
                    var spec = ProductSpec.GetDetail(oldItem.ProductSpecID.Value);
                    spec.Recyle(oldItem.Amount.Value);
                }

                // 删除附属资源
                oldItem.DeleteRes();
                oldItem.DeleteHistories();
                OrderItemAsset.DeleteBatch(oldItem.ID);

                // 删除自身
                oldItem.Delete();
                this.UpdateStat();
            }
            catch {}
        }

        /// <summary>添加项目。库存不足会抛出异常。</summary>
        public OrderItem AddItem(long productSpecId, int n, string title="", double? money = null)
        {
            // 获取产品规格
            var spec = ProductSpec.GetDetail(productSpecId);
            if (this.ShopID == null)
                this.ShopID = spec.Product?.ShopID;

            // 库存校验
            if (n > spec.Stock)
                throw new Exception("库存不足");

            // 订单项
            var oi = new OrderItem();
            oi.OrderID = this.ID;
            oi.Amount = n;

            // 产品规格信息
            oi.ProductSpecID = productSpecId;
            oi.ProductSpecName = spec.Name;
            oi.ProductSpecCode = spec.Code;
            oi.ProductID = spec.ProductID;
            oi.ProductName = spec.Product?.Name;
            oi.ProductType = spec.Product?.Type;
            oi.Price = spec.Price;
            oi.Title = title.IsEmpty() ? spec.FullName : title;

            // 费用
            oi.Money = money ?? oi.Price * oi.Amount;
            oi.Save();

            // 修改商品销售数, 更新订单信息
            spec.Sale(n);
            UpdateStat();
            return oi;
        }


        /// <summary>更新统计信息</summary>
        public Order UpdateStat()
        {
            var items = OrderItem.Search(orderId: this.ID).ToList();

            // 首商品
            string firstProductName = "";
            if (items.Count == 0)
                this.FirstProductID = null;
            else
            {
                this.FirstProductID = items[0].ProductID;
                this.CoverImage = items[0].ProductSpec?.CoverImage;
                if (this.FirstProductID == null)
                    firstProductName = items[0].Title;
                else
                    firstProductName = Product.Get(this.FirstProductID.Value)?.Name;
            }

            // 统计信息
            this.TotalAmount = items.Count;
            this.Summary = this.TotalAmount > 0 ? string.Format("{0} 等{1}件商品", firstProductName, this.TotalAmount) : "无商品";
            this.TotalMoney = items.Sum(t => t.Money);
            if (this.PayMoney == null || this.PayMoney == 0.0)
                this.PayMoney = this.TotalMoney;
            return this.Save(false);
        }

        /// <summary>统计</summary>
        public static List<StatItem> Stat(long? shopId, DateTime? startDt, DateTime? endDt)
        {
            IQueryable<Order> q = Set.Include(t => t.User).Include(t => t.Shop);
            if (shopId != null)  q = q.Where(t => t.ShopID == shopId);
            if (startDt != null) q = q.Where(t => t.CreateDt >= startDt);
            if (endDt != null)   q = q.Where(t => t.CreateDt <= endDt);
            return q
                .GroupBy(t => new
                {
                    //Shop = t.Shop.AbbrName,
                    Day = DbFunctions.TruncateTime(t.CreateDt).Value
                })
                .Select(t => new
                {
                    //Shop = t.Key.Shop,
                    Day = t.Key.Day,
                    Cnt = t.Count()
                })
                .OrderBy(t => new {/*t.Shop,*/ t.Day})
                .ToList()
                .Select(t => new StatItem("", t.Day.ToString("MMdd"), t.Cnt))
                .ToList()
                ;
            ;
        }

        public static Order GetOrder(string key)
        {
            var id = ParseId(key);
            var order = Order.Get(id);
            return order;
        }

        /// <summary>解析 Key 为ID</summary>
        public static long? ParseId(string key)
        {
            int n = key.LastIndexOf('-');
            if (n == -1)
                return null;
            var type = key.Substring(0, n);
            return key.Substring(n + 1).ParseLong();
        }

        //---------------------------------------
        // 工作流控制
        //---------------------------------------
        /// <summary>获取状态值文本说明</summary>
        string GetStatusName(int statusId)
        {
            switch (this.Type)
            {
                case ProductType.Insurance: return ((InsuranceStatus?)statusId).GetTitle();
                case ProductType.Check:     return ((CheckStatus?)statusId).GetTitle();
                case ProductType.Repair:    return ((RepairStatus?)statusId).GetTitle();
            }
            return "";
        }

        /// <summary>更改状态（若失败会抛出异常）</summary>
        public Order ChangeStatusEnum<T>(T status, long? userId, string remark="", List<string> fileUrls=null) where T : struct
        {
            int statusId = status.ToString().ParseInt().Value;
            string statusName = typeof(T).IsEnum ? status.GetTitle() : GetStatusName(statusId);
            return ChangeStatus(statusName, statusId, userId, remark, fileUrls);
        }

        /// <summary>更改状态（若失败会抛出异常）</summary>
        public Order ChangeStatus(string statusName, int statusId, long? userId, string remark = "", List<string> fileUrls = null)
        {
            // 状态相同就不用更改了
            if (this.Status == statusId)
                return this;

            //
            userId = userId ?? this.UserID;
            var user = User.Get(userId);

            // 找到合适的下一步状态
            var flow = GetFlow();
            var step = flow.ChangeStep((int?)this.Status, statusId, null, user);
            if (step == null)
                throw new Exception("不允许更改状态：" + statusName);

            // 更新订单信息
            this.Status = statusId;
            this.StatusName = statusName;
            this.StatusDt = DateTime.Now;
            this.Save();

            // 增加操作历史
            this.AddHistory(user.ID, user.NickName, user.Mobile, statusName, statusId, remark, fileUrls);

            // 检测后继步骤，如果只有一个且是结束节点，自动完成掉
            /*
            var nexts = this.GetNextSteps(null);
            if (nexts.Count == 1) {
                var next = nexts[0];
                if (next.Type == WFStepType.End)
                    ChangeStatus(next.StatusName, next.Status.Value, userId, "自动完成");
            }
            */

            // 发送微信消息
            Logic.SendWechatOrderMessage(this);
            return this;
        }

        //---------------------------------------
        // 公用流程方法
        //---------------------------------------
        /// <summary>支付</summary>
        public Order Pay(OrderPayMode? mode, double? money=null, string info="")
        {
            this.ChangeStatusEnum(OrderStatus.UserPay, null);
            this.PayMode = mode ?? OrderPayMode.WechatPay;
            this.PayDt = DateTime.Now;
            this.PayMoney = money ?? this.TotalMoney;
            this.PayInfo = info;
            this.Save();
            UserFinance.Add(FinanceType.Consume, this.UserID, money, ID, info);
            return this;
        }

        /// <summary>完成订单</summary>
        public Order Finish(long? userId=null)
        {
            this.ChangeStatusEnum(OrderStatus.Finish, userId);
            return this;
        }

        /// <summary>取消订单</summary>
        public Order Cancel(long? userId=null)
        {
            this.ChangeStatusEnum(OrderStatus.Cancel, userId);
            return this;
        }

        /// <summary>批量取消订单</summary>
        public static void CancelBatch(List<long> ids, long? userId=null)
        {
            Search(ids).ToList().ForEach(t => t.Cancel(userId));
        }
    }
}