using App.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Entities;

namespace App.DAL
{
    //
    // 因为 enum 无法继承，只能变通用下面的方式实现
    // 注意 Create, Finish, Cancel, UserPay 状态编码都必须是一致的
    // 以简化类型转化逻辑
    //
    /// <summary>通用订单状态</summary>
    public enum OrderStatus : int
    {
        [UI("已创建")] Create = 0,
        [UI("已完成")] Finish = 1,
        [UI("已取消")] Cancel = 2,
        [UI("已支付")] UserPay = 3
    }

    /// <summary>商品订单状态</summary>
    public enum GoodsStatus : int
    {
        [UI("已创建")] Create = OrderStatus.Create,
        [UI("已完成")] Finish = OrderStatus.Finish,
        [UI("已取消")] Cancel = OrderStatus.Cancel,
        [UI("已支付")] UserPay = OrderStatus.UserPay,
        [UI("已派送")] BizSend = 4,
        [UI("已收货")] UserReceive = 5,
    }

    /// <summary>维修订单状态</summary>
    public enum RepairStatus : int
    {
        [UI("已创建")]             Create = OrderStatus.Create,
        [UI("已完成")]             Finish = OrderStatus.Finish,
        [UI("已取消")]             Cancel = OrderStatus.Cancel,
        //[UI("用户已支付")]       UserPay = OrderStatus.UserPay,
        [UI("商家已接单")]         BizAccept = 4,
        //[UI("商家已约上门服务")] BizAppt = 5,
        //[UI("商家已拿设备")]     BizTake = 6,
        //[UI("商家已评估费用")]   BizEval = 7,

        [UI("商家维修中")]         BizRepair = 8,
        [UI("商家维修完毕")]       BizRepairOk = 9,
        [UI("顾客已拿机")]         UserReceive = 10,

        [UI("商家已寄送维修")]     BizSend = 11,
        [UI("寄送维修中")]         SendRepair = 12,
        [UI("寄修完毕已寄回")]     SendRepairOK = 13,
        [UI("商家已取寄修件")]     BizReceive = 14,
    }

    /// <summary>保险订单状态</summary>
    public enum InsuranceStatus : int
    {
        [UI("已创建")]           Create = OrderStatus.Create,
        [UI("已完成")]           Finish = OrderStatus.Finish,
        [UI("已取消")]           Cancel = OrderStatus.Cancel,
        [UI("用户已支付")]       UserPay = OrderStatus.UserPay,
    }

    /// <summary>真机检测订单状态</summary>
    public enum CheckStatus : int
    {
        [UI("已创建")]           Create = OrderStatus.Create,
        [UI("已完成")]           Finish = OrderStatus.Finish,
        [UI("已取消")]           Cancel = OrderStatus.Cancel,
        [UI("用户已支付")]       UserPay = OrderStatus.UserPay,
        [UI("商家已鉴定")]       BizCheck = 4,
    }

    /// <summary>
    /// 订单工作流相关代码放这里
    /// </summary>
    public partial class Order
    {
        // 配置好的工作流
        public static Workflow OrderFlow = Order.BuildOrderFlow();
        public static Workflow RepairFlow = Order.BuildRepairFlow();
        public static Workflow InsuranceFlow = Order.BuildInsuranceFlow();
        public static Workflow CheckFlow = Order.BuildCheckFlow();
        public static Workflow GoodsFlow = Order.BuildGoodsFlow();

 
        //-------------------------------------
        // 流程方法
        //-------------------------------------
        /// <summary>获取本订单对应的工作流</summary>
        public Workflow GetFlow()
        {
            if (this.Type == ProductType.Check)     return CheckFlow;
            if (this.Type == ProductType.Insurance) return InsuranceFlow;
            if (this.Type == ProductType.Repair)    return RepairFlow;
            return OrderFlow;
        }

        /// <summary>获取所有操作步骤</summary>
        public List<WFStep> GetSteps()
        {
            return GetFlow().Steps;
        }

        /// <summary>获取下一步操作步骤</summary>
        public List<WFStep> GetNextSteps(User user)
        {
            return GetFlow().GetNextSteps((int?)this.Status, null, user);
        }

        //-------------------------------------
        // 流程
        //-------------------------------------
        /// <summary>获取订单流程</summary>
        static Workflow BuildOrderFlow()
        {
            var stepCreate = WFStep.Create(OrderStatus.Create, type: WFStepType.Start);
            var stepCancel = WFStep.Create(OrderStatus.Cancel, type: WFStepType.End);
            var stepFinish = WFStep.Create(OrderStatus.Finish, type: WFStepType.End);
            var stepPay = WFStep.Create(OrderStatus.UserPay);

            // 关系
            stepCreate.AddNexts(stepCancel, stepPay);
            stepPay.AddNexts(stepFinish);

            // 导出
            var flow = new Workflow("订单流程", typeof(Order), typeof(OrderStatus));
            flow.Steps = Convertor.ToList(stepFinish, stepCancel, stepPay, stepCreate);
            return flow;
        }

        /// <summary>获取维修操作步骤</summary>
        static Workflow BuildRepairFlow()
        {
            var stepCreate       = WFStep.Create(RepairStatus.Create, null, WFStepType.Start);
            var stepCancel       = WFStep.Create(RepairStatus.Cancel, null, WFStepType.End);
            var stepFinish       = WFStep.Create(RepairStatus.Finish, null, WFStepType.End);
            var stepBizAccept    = WFStep.Create(RepairStatus.BizAccept, Powers.AdminShop);
            var stepBizRepair    = WFStep.Create(RepairStatus.BizRepair, Powers.AdminShop);
            var stepBizRepairOk  = WFStep.Create(RepairStatus.BizRepairOk, Powers.AdminShop);
            var stepUserReceive  = WFStep.Create(RepairStatus.UserReceive, Powers.AdminShop);
            var stepBizSend      = WFStep.Create(RepairStatus.BizSend, Powers.AdminShop);
            var stepSendRepair   = WFStep.Create(RepairStatus.SendRepair, Powers.AdminRepairShop);
            var stepSendRepairOk = WFStep.Create(RepairStatus.SendRepairOK, Powers.AdminRepairShop);
            var stepBizReceive   = WFStep.Create(RepairStatus.BizReceive, Powers.AdminShop);

            // 关系
            stepCreate.AddNext(stepCancel).AddNext(stepBizAccept);
            stepBizAccept.AddNext(stepBizRepair, "商家自修").AddNext(stepBizSend, "寄送维修").AddNext(stepCancel, "检查无故障取消");
            stepBizRepair.AddNext(stepBizRepairOk);
            stepBizRepairOk.AddNext(stepUserReceive);
            stepUserReceive.AddNext(stepFinish);
            stepBizSend.AddNext(stepSendRepair);
            stepSendRepair.AddNext(stepSendRepairOk);
            stepSendRepairOk.AddNext(stepBizReceive);
            stepBizReceive.AddNext(stepUserReceive);

            // 导出
            var flow = new Workflow("维修订单流程", typeof(Order), typeof(RepairStatus));
            flow.Steps = Convertor.ToList(
                stepCancel, stepFinish,
                stepUserReceive,
                stepBizReceive, stepSendRepairOk, stepSendRepair, stepBizSend,
                stepBizRepairOk, stepBizRepair,
                stepBizAccept,
                stepCreate
                );
            return flow;
        }

        /// <summary>获取保险操作步骤</summary>
        static Workflow BuildInsuranceFlow()
        {
            var stepCreate = WFStep.Create(InsuranceStatus.Create, null, WFStepType.Start);
            var stepCancel = WFStep.Create(InsuranceStatus.Cancel, null, WFStepType.End);
            var stepFinish = WFStep.Create(InsuranceStatus.Finish, null, WFStepType.End);
            var stepPay = WFStep.Create(InsuranceStatus.UserPay);

            stepCreate.AddNexts(stepCancel, stepPay);
            stepPay.AddNexts(stepFinish);

            var flow = new Workflow("续保订单流程", typeof(Order), typeof(InsuranceStatus));
            flow.Steps = Convertor.ToList(stepCancel, stepFinish, stepPay, stepCreate);
            return flow;
        }

        /// <summary>获取真机检测操作步骤</summary>
        static Workflow BuildCheckFlow()
        {
            var steps = new List<WFStep>();
            var stepCreate = WFStep.Create(CheckStatus.Create, null, WFStepType.Start);
            var stepCancel = WFStep.Create(CheckStatus.Cancel, null, WFStepType.End);
            var stepFinish = WFStep.Create(CheckStatus.Finish, null, WFStepType.End);
            var stepPay      = WFStep.Create(CheckStatus.UserPay);
            var stepBizCheck = WFStep.Create(CheckStatus.BizCheck);

            stepCreate.AddNexts(stepCancel, stepPay);
            stepPay.AddNexts(stepBizCheck);
            stepBizCheck.AddNexts(stepFinish);

            var flow = new Workflow("真机检测订单流程", typeof(Order), typeof(CheckStatus));
            flow.Steps = Convertor.ToList(stepCancel, stepFinish, stepBizCheck, stepPay, stepCreate);
            return flow;
        }

        /// <summary>配置商品销售工作流</summary>
        static Workflow BuildGoodsFlow()
        {
            var stepCreate      = WFStep.Create(GoodsStatus.Create, null, WFStepType.Start);
            var stepCancel      = WFStep.Create(GoodsStatus.Cancel, null, WFStepType.End);
            var stepFinish      = WFStep.Create(GoodsStatus.Finish, null, WFStepType.End);
            var stepPay         = WFStep.Create(GoodsStatus.UserPay);
            var stepBizSend     = WFStep.Create(GoodsStatus.BizSend);
            var stepUserReceive = WFStep.Create(GoodsStatus.UserReceive);

            stepCreate.AddNexts(stepCancel, stepPay);
            stepPay.AddNexts(stepCancel, stepBizSend);
            stepBizSend.AddNexts(stepUserReceive);
            stepUserReceive.AddNexts(stepFinish);

            var flow = new Workflow("商品订单流程", typeof(Order), typeof(GoodsStatus));
            flow.Steps = Convertor.ToList(stepCancel, stepFinish, stepUserReceive, stepBizSend, stepPay, stepCreate);
            return flow;
        }
    }
}
