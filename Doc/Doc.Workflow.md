
----------------------------------------------
���������˼·
----------------------------------------------
����Ŀ���������沿�ֲ����κα�����������ܡ�2018-12-04

���������˼·
    ���裺WFStep
        ������������ֵ
        ���·�ɼ���
        ǰ��·�ɼ���
        ִ�У�Undo, Success, Fail, Back
    ·�ɣ�WFRoute
        From/To/
        ·������: Or/And/Vote
        ʱ�ӣ�ʱ�ӽ����Զ�������һ����
        ��ɫ�����š���Ա����Ϊ·�ɵĲ������������������������¼����д���
    ������ʵ����WFInstance
        ��WFService.CreateInstance(wfId)����
        ����ʵ�����ݣ��籨�����
        ʵ���¼������������������롢����
        ʵ����־��ѡ��·�ɺ󣬻Ὣѡ�����¼�� WFLog ����
    ��̨���̣�WFConsoler.exe
        ����ʱ���ȹ�����
        ʵ�� WFService �࣬��������߼����ɿ����� App.Scheduler ʵ��

���ݿ⻯����
    ����˿���Ҫ�ֶ��������̣�����д�����ݿ���
    �������Ҫ��ֱ����CS������Ƹ�����
    ���ۣ����ݿ⻯�ɣ��״��Զ�����һ�¡���Ҫ���Բ����� WF ��֮�����ϡ�


��������ҵ������Ϸ���
    /���������ã�
        /�� Workflow: Type, StatusType, Name, Steps
        /�ڵ�Step��Status��StatusName, Type
        /�ڵ���·��Route: Step, NextStep, Condition, Users, Roles, Depts
    /������ת�ӿ�
        /GetAllSteps()
        /GetStartSteps()
        /step.Routes
        /GetNextSteps(status, data, user)
        /ChangeStep(status, data, user)
    /�����
        /���ڴ�����Ա��
            /���������������ã�
            /GetNextSteps�д��뵱ǰ�û���Ϣ���ж��Ƿ�ɴ�ͨ��·��
            /GetNextSteps�������û���Ϣ�������ж��Ƿ��ͨ��·��
        /�߼����ʽ��·��ѡ��
            /��WFRoute.Condition�����ã���data>3000
            /ͨ��GetNextSteps(...., data, user)����ҵ������
    ����WFInstance����(���Ǳ����)
        ����
            string Key��Order.UniID
            object Data ���ɲ���
            List<int> CurrentSteps;
        ����
            int Check(): ��ʱ���̴����Զ����̴���
            ��¶ Workflow �еķ���
                ����һ���Լ̳еķ�ʽʵ��
                    �ο� https://www.cnblogs.com/tang-tang/p/5493165.html
                    Table per Hierarchy (TPH): ���ú�ʵ������һ����¼��������
                    Table per Type (TPT)��ÿ��ʵ�����������ñ��ʵ�����и�����һ�����ݣ�Ҳ������
                    Table per Concrete class (TPC): ���ú�ʵ�����ű�û���κ���ϵ��Ҳ������
                    ���ۣ�EF �е����ּ̳�ģʽ����̫���ʣ��ð�����ϵ
                ���������԰�����ϵʵ��
                    public class WFInstance 
                    {
                        public Workflow Workflow {get; set;}
                    }
        �¼�
            InstanceStart
            BeforeChangeStep
            AfterChangeStep
            InstanceFinish
        �ʺϣ�
            ��Ҫ�¼�����
            ��Ҫ��¼�����ǰ״̬
            ��Ҫ����ʵ����͹�������
        ����
            �����̲��Ǳ����
            ����ǰ״̬�������������������������Order��������CurrentSatusIds�ֶΣ�ҵ���Ҫ���죬�����ʣ�
            Ϊ�˽�����Ҫ���Ƽ�ʵ��
    /�������� WFLog ��
        /���ڼ�¼������ʷ
        /Order ----- WFInstance ------- History
                  (key=Order-XXX)   (key=Order-XXX)
        /���� History����Ϊ EntityBase ������
            /֮ǰ��Order, OrderItem ��Ҫ�õ���������ֻ��Order���õ�
            /�����ɣ������ˡ��Ժ������Ϊ���б�ļ�¼���������־���ܷ���
            /�ȶ���Log����淶��log��̫���ˣ���¼�����¼���
        /���ۣ�ֱ��ʹ��History��


��������ҵ�����Ϸ�������������ʵ���ࣩ
    ����������
        ���ƣ�
        ���
        ʵ���ࣺ
    ��Ӳ��裺
        �϶���ֹ���裬Ҫ��������һ����ʼ�ڵ��һ�������ڵ�
    ��������֮������ߣ���������·������ҳ��
        �������ƣ�
        ��ʼ���裺
        ��̲��裺
        ��Ӧ����(��ʵ����)
        �߼�������
           �ֶ�  ������  ֵ
           �ֶ�  ������  ֵ
           �ֶ�  ������  ֵ
        ָ��������Ա��
        ָ�������ţ�
        ָ�������ɫ��

