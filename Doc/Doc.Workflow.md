
----------------------------------------------
工作流设计思路
----------------------------------------------
本项目工作流引擎部分不做任何变更，锁定功能。2018-12-04

工作流设计思路
    步骤：WFStep
        步骤名、步骤值
        后继路由集合
        前驱路由集合
        执行：Undo, Success, Fail, Back
    路由：WFRoute
        From/To/
        路由类型: Or/And/Vote
        时延：时延结束自动跳到下一步骤
        角色、部门、人员：作为路由的参数，但工作流并不处理，由事件自行处理
    工作流实例：WFInstance
        由WFService.CreateInstance(wfId)启动
        保存实例数据：如报销金额
        实例事件：启动、结束、进入、跳出
        实例日志：选择路由后，会将选择步骤记录在 WFLog 表中
    后台进程：WFConsoler.exe
        负责定时调度工作流
        实现 WFService 类，负责调度逻辑，可考虑用 App.Scheduler 实现

数据库化考虑
    如果顾客需要手动更改流程，必须写到数据库中
    如果不需要，直接用CS代码控制更方便
    结论：数据库化吧，首次自动生成一下。需要调试并减少 WF 类之间的耦合。


工作流和业务松耦合方案
    /工作流配置：
        /流 Workflow: Type, StatusType, Name, Steps
        /节点Step：Status，StatusName, Type
        /节点间的路由Route: Step, NextStep, Condition, Users, Roles, Depts
    /流程跳转接口
        /GetAllSteps()
        /GetStartSteps()
        /step.Routes
        /GetNextSteps(status, data, user)
        /ChangeStep(status, data, user)
    /松耦合
        /关于处理人员：
            /可先在流程中配置；
            /GetNextSteps中传入当前用户信息，判断是否可打通该路由
            /GetNextSteps不传入用户信息，自行判断是否打通该路由
        /逻辑表达式（路由选择）
            /在WFRoute.Condition中设置，如data>3000
            /通过GetNextSteps(...., data, user)传入业务数据
    增加WFInstance对象(不是必须的)
        属性
            string Key：Order.UniID
            object Data ：可不用
            List<int> CurrentSteps;
        方法
            int Check(): 定时流程处理、自动流程处理
            暴露 Workflow 中的方法
                方案一：以继承的方式实现
                    参考 https://www.cnblogs.com/tang-tang/p/5493165.html
                    Table per Hierarchy (TPH): 配置和实例都有一条记录，不合适
                    Table per Type (TPT)：每个实例都会在配置表和实例表中各插入一条数据，也不合适
                    Table per Concrete class (TPC): 配置和实例两张表没有任何联系，也不合适
                    结论：EF 中的三种继承模式都不太合适，用包含关系
                方案二：以包含关系实现
                    public class WFInstance 
                    {
                        public Workflow Workflow {get; set;}
                    }
        事件
            InstanceStart
            BeforeChangeStep
            AfterChangeStep
            InstanceFinish
        适合：
            需要事件处理
            需要记录多个当前状态
            需要剥离实体类和工作流类
        结论
            简单流程不是必须的
            单当前状态工作流无需这个东西，或者在Order表中增加CurrentSatusIds字段（业务表要改造，不合适）
            为了将来需要，推荐实现
    /考虑增加 WFLog 表
        /用于记录操作历史
        /Order ----- WFInstance ------- History
                  (key=Order-XXX)   (key=Order-XXX)
        /考虑 History表不作为 EntityBase 的属性
            /之前是Order, OrderItem 都要用到它，现在只有Order表用到
            /保留吧，不动了。以后可以作为所有表的记录变更辅助日志表，很方便
            /比丢到Log表更规范（log表太杂了，记录所有事件）
        /结论：直接使用History表


工作流与业务紧耦合方案（关联表单、实体类）
    工作流配置
        名称：
        类别：
        实体类：
    添加步骤：
        拖动防止步骤，要求至少有一个开始节点和一个结束节点
    拉动步骤之间的连线，弹窗出现路由设置页面
        动作名称：
        开始步骤：
        后继步骤：
        对应表单：(含实体类)
        逻辑条件：
           字段  操作符  值
           字段  操作符  值
           字段  操作符  值
        指定处理人员：
        指定处理部门：
        指定处理角色：

