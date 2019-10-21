﻿using System.Collections.Generic;

namespace BehaviorTreeEditor
{
    public class NodeTemplate
    {
        private GlobalVariable m_GlobalVariable = new GlobalVariable();
        private List<NodeDefine> m_Nodes = new List<NodeDefine>();
        private List<CustomEnum> m_Enums = new List<CustomEnum>();

        public GlobalVariable GlobalVariable
        {
            get { return m_GlobalVariable; }
            set { m_GlobalVariable = value; }
        }

        public List<CustomEnum> Enums
        {
            get { return m_Enums; }
            set { m_Enums = value; }
        }

        public List<NodeDefine> Nodes
        {
            get { return m_Nodes; }
            set { m_Nodes = value; }
        }

        /// <summary>
        /// 获取指定类型所有组合节点类
        /// </summary>
        /// <param name="nodeType">节点类型</param>
        /// <returns></returns>
        public List<NodeDefine> GetNodeDefines(NodeType nodeType)
        {
            List<NodeDefine> nodeList = new List<NodeDefine>();
            for (int i = 0; i < m_Nodes.Count; i++)
            {
                NodeDefine nodeDefine = m_Nodes[i];
                if (nodeDefine == null)
                    continue;
                if (nodeDefine.NodeType == nodeType)
                    nodeList.Add(nodeDefine);
            }
            return nodeList;
        }

        /// <summary>
        /// 添加节点类
        /// </summary>
        /// <param name="nodeDefine"></param>
        public bool AddClass(NodeDefine nodeDefine)
        {
            if (nodeDefine == null)
                return false;

            for (int i = 0; i < m_Nodes.Count; i++)
            {
                NodeDefine tmp = m_Nodes[i];
                if (tmp.ClassType == nodeDefine.ClassType)
                {
                    MainForm.Instance.ShowMessage(string.Format("已存在{0},请换一个类名", nodeDefine.ClassType), "警告");
                    return false;
                }
            }

            m_Nodes.Add(nodeDefine);

            m_Nodes.Sort(delegate (NodeDefine a, NodeDefine b)
            {
                return a.NodeType.CompareTo(b.NodeType);
            });

            return true;
        }

        /// <summary>
        /// 删除节点类
        /// </summary>
        /// <param name="nodeDefine">节点类对象</param>
        /// <returns></returns>
        public bool Remove(NodeDefine nodeDefine)
        {
            if (nodeDefine == null)
                return false;
            return m_Nodes.Remove(nodeDefine);
        }

        public void ResetEnums()
        {
            m_Enums.Clear();

            CustomEnum SUCCESS_POLICY = new CustomEnum();
            SUCCESS_POLICY.EnumType = "SUCCESS_POLICY";
            SUCCESS_POLICY.AddEnumItem(new EnumItem() { EnumStr = "SUCCEED_ON_ONE", EnumValue = 1, Describe = "当某一个节点返回成功时退出；" });
            SUCCESS_POLICY.AddEnumItem(new EnumItem() { EnumStr = "SUCCEED_ON_ALL", EnumValue = 2, Describe = "当全部节点都返回成功时退出" });
            AddEnum(SUCCESS_POLICY);

            CustomEnum FAILURE_POLICY = new CustomEnum();
            FAILURE_POLICY.EnumType = "FAILURE_POLICY";
            FAILURE_POLICY.AddEnumItem(new EnumItem() { EnumStr = "FAIL_ON_ONE", EnumValue = 1, Describe = "当某一个节点返回失败时退出；" });
            FAILURE_POLICY.AddEnumItem(new EnumItem() { EnumStr = "FAIL_ON_ALL", EnumValue = 2, Describe = "当全部节点都返回失败时退出" });
            AddEnum(FAILURE_POLICY);

            CustomEnum ParameterType = new CustomEnum();
            ParameterType.EnumType = "ParameterType";
            ParameterType.AddEnumItem(new EnumItem() { EnumStr = "Agent", EnumValue = 1, Describe = "在Agent定义的参数" });
            ParameterType.AddEnumItem(new EnumItem() { EnumStr = "Global", EnumValue = 2, Describe = "全局定义的参数" });
            AddEnum(ParameterType);

            CustomEnum CompareType = new CustomEnum();
            CompareType.EnumType = "CompareType";
            CompareType.AddEnumItem(new EnumItem() { EnumStr = "Less", EnumValue = 1, Describe = "<" });
            CompareType.AddEnumItem(new EnumItem() { EnumStr = "Greater", EnumValue = 2, Describe = ">" });
            CompareType.AddEnumItem(new EnumItem() { EnumStr = "LEqual", EnumValue = 3, Describe = "<=" });
            CompareType.AddEnumItem(new EnumItem() { EnumStr = "GEqual", EnumValue = 4, Describe = ">=" });
            CompareType.AddEnumItem(new EnumItem() { EnumStr = "Equal", EnumValue = 5, Describe = "==" });
            CompareType.AddEnumItem(new EnumItem() { EnumStr = "NotEqual", EnumValue = 6, Describe = "!=" });
            AddEnum(CompareType);
        }

        public void ResetNodes()
        {
            m_Nodes.Clear();
            #region 组合节点
            //并行节点
            NodeDefine parallelNode = new NodeDefine();
            parallelNode.ClassType = "Parallel";
            parallelNode.Label = "并行节点";
            parallelNode.NodeType = NodeType.Composite;
            parallelNode.Describe = "Parallel节点在一般意义上是并行的执行其子节点，即“一边做A，一边做B”";
            //成功条件
            NodeField parallelNodeSuccessType = new NodeField() { FieldName = "SuccessType", FieldType = FieldType.EnumField, Describe = "成功条件" };
            EnumDefaultValue parallelNodeSuccessEnumDefaultValue = parallelNodeSuccessType.DefaultValue as EnumDefaultValue;
            parallelNodeSuccessEnumDefaultValue.EnumType = "SUCCESS_POLICY";
            parallelNodeSuccessEnumDefaultValue.DefaultValue = "SUCCEED_ON_ALL";
            parallelNodeSuccessType.Label = "成功条件";
            parallelNode.AddField(parallelNodeSuccessType);
            //失败条件
            NodeField parallelNodeFailType = new NodeField() { FieldName = "FailType", FieldType = FieldType.EnumField, Describe = "失败条件" };
            EnumDefaultValue parallelNodeFailTypeEnumDefaultValue = parallelNodeFailType.DefaultValue as EnumDefaultValue;
            parallelNodeFailTypeEnumDefaultValue.EnumType = "FAILURE_POLICY";
            parallelNodeFailTypeEnumDefaultValue.DefaultValue = "FAIL_ON_ONE";
            parallelNodeFailType.Label = "失败条件";
            parallelNode.AddField(parallelNodeFailType);
            AddClass(parallelNode);

            //顺序节点
            NodeDefine sequenceNode = new NodeDefine();
            sequenceNode.ClassType = "Sequence";
            sequenceNode.Label = "顺序节点";
            sequenceNode.NodeType = NodeType.Composite;
            sequenceNode.Describe = "Sequence节点以给定的顺序依次执行其子节点，直到所有子节点成功返回，该节点也返回成功。只要其中某个子节点失败，那么该节点也失败。";
            AddClass(sequenceNode);

            //选择节点
            NodeDefine Selector = new NodeDefine();
            Selector.ClassType = "Selector";
            Selector.Label = "选择节点";
            Selector.Category = "";
            Selector.NodeType = NodeType.Composite;
            Selector.Describe = "选择节点";
            AddClass(Selector);

            //ifelse
            NodeDefine IfElse = new NodeDefine();
            IfElse.ClassType = "IfElse";
            IfElse.Label = "IfElse";
            IfElse.NodeType = NodeType.Composite;
            IfElse.Describe = "";
            AddClass(IfElse);

            //随机节点
            NodeDefine Random = new NodeDefine();
            Random.ClassType = "Random";
            Random.Label = "随机节点";
            Random.Category = "随机";
            Random.NodeType = NodeType.Composite;
            Random.Describe = "随机节点";
            AddClass(Random);

            //随机选择节点
            NodeDefine RandomSelector = new NodeDefine();
            RandomSelector.ClassType = "RandomSelector";
            RandomSelector.Label = "随机选择";
            RandomSelector.Category = "随机";
            RandomSelector.NodeType = NodeType.Composite;
            RandomSelector.Describe = "随机选择节点";
            AddClass(RandomSelector);

            //随机序列节点
            NodeDefine RandomSequence = new NodeDefine();
            RandomSequence.ClassType = "RandomSequence";
            RandomSequence.Label = "随机序列";
            RandomSequence.Category = "随机";
            RandomSequence.NodeType = NodeType.Composite;
            RandomSequence.Describe = "随机序列节点";
            AddClass(RandomSequence);

            //概率选择节点
            NodeDefine SelectorProbability = new NodeDefine();
            SelectorProbability.ClassType = "SelectorProbability";
            SelectorProbability.Label = "概率选择节点";
            SelectorProbability.Category = "";
            SelectorProbability.NodeType = NodeType.Composite;
            SelectorProbability.Describe = "概率选择节点";
            SelectorProbability.AddField(new NodeField() { FieldName = "Priority", Label = "优先级", FieldType = FieldType.RepeatIntField, Describe = "", Show = true });
            AddClass(SelectorProbability);

            #endregion

            #region 装饰节点

            //成功节点
            NodeDefine Success = new NodeDefine();
            Success.ClassType = "Success";
            Success.Label = "成功节点";
            Success.NodeType = NodeType.Decorator;
            Success.Describe = "成功节点";
            AddClass(Success);

            //失败节点
            NodeDefine Failure = new NodeDefine();
            Failure.ClassType = "Failure";
            Failure.Label = "失败节点";
            Failure.NodeType = NodeType.Decorator;
            Failure.Describe = "失败节点";
            AddClass(Failure);

            //帧数节点用于在指定的帧数内，持续调用其子节点
            NodeDefine Frames = new NodeDefine();
            Frames.ClassType = "Frames";
            Frames.Label = "帧数节点";
            Frames.NodeType = NodeType.Decorator;
            Frames.ShowContent = true;
            NodeField FramesField = new NodeField() { FieldName = "Frames", Label = "持续帧数", FieldType = FieldType.IntField, Describe = "持续帧数", Show = true };
            (FramesField.DefaultValue as IntDefaultValue).DefaultValue = 1;
            Frames.AddField(FramesField);
            Frames.Describe = "帧数节点用于在指定的帧数内，持续调用其子节点";
            AddClass(Frames);

            //循环节点 -1无限循环
            NodeDefine Loop = new NodeDefine();
            Loop.ClassType = "Loop";
            Loop.Label = "循环节点";
            Loop.NodeType = NodeType.Decorator;
            Loop.Describe = "循环节点 -1无限循环";
            Loop.AddField(new NodeField() { FieldName = "LoopTimes", Label = "循环次数", FieldType = FieldType.IntField, Describe = "循环次数", Show = true });
            AddClass(Loop);

            //取反节点
            NodeDefine Not = new NodeDefine();
            Not.ClassType = "Not";
            Not.Label = "取反节点";
            Not.NodeType = NodeType.Decorator;
            Not.Describe = "取反节点";
            AddClass(Not);

            //指定时间内运行
            NodeDefine Time = new NodeDefine();
            Time.ClassType = "Time";
            Time.Label = "时间";
            Time.NodeType = NodeType.Decorator;
            Time.Describe = "指定时间内运行";
            NodeField TimeField = new NodeField() { FieldName = "Duration", Label = "持续时间(毫秒)", FieldType = FieldType.IntField, Describe = "持续时间(毫秒)", Show = true };
            (TimeField.DefaultValue as IntDefaultValue).DefaultValue = 1000;
            Time.AddField(TimeField);
            AddClass(Time);

            //阻塞，直到子节点返回true
            NodeDefine SuccessUntil = new NodeDefine();
            SuccessUntil.ClassType = "SuccessUntil";
            SuccessUntil.Label = "直到子节点返回True";
            SuccessUntil.NodeType = NodeType.Decorator;
            SuccessUntil.Describe = "直到子节点返回true";
            AddClass(SuccessUntil);

            #endregion

            #region 条件节点

            //比较Int节点
            NodeDefine CompareInt = new NodeDefine();
            CompareInt.ClassType = "CompareInt";
            CompareInt.Label = "比较Int节点";
            CompareInt.NodeType = NodeType.Condition;
            CompareInt.Describe = "Compare节点对左右参数进行比较";
            //左边参数类型
            NodeField CompareInt_LeftType = new NodeField() { FieldName = "LeftType", FieldType = FieldType.EnumField, Describe = "" };
            (CompareInt_LeftType.DefaultValue as EnumDefaultValue).EnumType = "ParameterType";
            CompareInt_LeftType.Label = "左参数类型";
            CompareInt.AddField(CompareInt_LeftType);
            //左边参数变量名
            CompareInt.AddField(new NodeField() { FieldName = "LeftParameter", Label = "左参数名", FieldType = FieldType.StringField, Describe = "左边参数变量名" });
            //比较符号
            NodeField CompareInt_Type = new NodeField() { FieldName = "CompareType", FieldType = FieldType.EnumField, Describe = "比较符号<、>、<=、>=、==、!=" };
            (CompareInt_Type.DefaultValue as EnumDefaultValue).EnumType = "CompareType";
            CompareInt_Type.Label = "比较操作符";
            CompareInt.AddField(CompareInt_Type);
            //右边边参数类型
            NodeField CompareInt_RightType = new NodeField() { FieldName = "RightType", FieldType = FieldType.EnumField, Describe = "" };
            (CompareInt_RightType.DefaultValue as EnumDefaultValue).EnumType = "ParameterType";
            CompareInt_RightType.Label = "右参数类型";
            CompareInt.AddField(CompareInt_RightType);
            //右边参数变量名
            CompareInt.AddField(new NodeField() { FieldName = "RightParameter", Label = "右参数名", FieldType = FieldType.StringField, Describe = "右边参数变量名" });
            AddClass(CompareInt);

            //比较Float节点
            NodeDefine CompareFloat = new NodeDefine();
            CompareFloat.ClassType = "CompareFloat";
            CompareFloat.Label = "比较Float节点";
            CompareFloat.NodeType = NodeType.Condition;
            CompareFloat.Describe = "Compare节点对左右参数进行比较";
            //左边参数类型
            NodeField CompareFloat_LeftType = new NodeField() { FieldName = "LeftType", FieldType = FieldType.EnumField, Describe = "" };
            (CompareFloat_LeftType.DefaultValue as EnumDefaultValue).EnumType = "ParameterType";
            CompareFloat_LeftType.Label = "左参数类型";
            CompareFloat.AddField(CompareFloat_LeftType);
            //左边参数变量名
            CompareFloat.AddField(new NodeField() { FieldName = "LeftParameter", Label = "左参数名", FieldType = FieldType.StringField, Describe = "左边参数变量名" });
            //比较符号
            NodeField CompareFloat_Type = new NodeField() { FieldName = "CompareType", FieldType = FieldType.EnumField, Describe = "比较符号<、>、<=、>=、==、!=" };
            (CompareFloat_Type.DefaultValue as EnumDefaultValue).EnumType = "CompareType";
            CompareFloat_Type.Label = "比较操作符";
            CompareFloat.AddField(CompareFloat_Type);
            //右边边参数类型
            NodeField CompareFloat_RightType = new NodeField() { FieldName = "RightType", FieldType = FieldType.EnumField, Describe = "" };
            (CompareFloat_RightType.DefaultValue as EnumDefaultValue).EnumType = "ParameterType";
            CompareFloat_RightType.Label = "右参数类型";
            CompareFloat.AddField(CompareFloat_RightType);
            //右边参数变量名
            CompareFloat.AddField(new NodeField() { FieldName = "RightParameter", Label = "右参数名", FieldType = FieldType.StringField, Describe = "右边参数变量名" });
            AddClass(CompareFloat);

            //比较String节点
            NodeDefine CompareString = new NodeDefine();
            CompareString.ClassType = "CompareString";
            CompareString.Label = "比较String节点";
            CompareString.NodeType = NodeType.Condition;
            CompareString.Describe = "Compare节点对左右参数进行比较";
            //左边参数类型
            NodeField CompareString_LeftType = new NodeField() { FieldName = "LeftType", FieldType = FieldType.EnumField, Describe = "" };
            (CompareString_LeftType.DefaultValue as EnumDefaultValue).EnumType = "ParameterType";
            CompareString_LeftType.Label = "左参数类型";
            CompareString.AddField(CompareString_LeftType);
            //左边参数变量名
            CompareString.AddField(new NodeField() { FieldName = "LeftParameter", Label = "左参数名", FieldType = FieldType.StringField, Describe = "左边参数变量名" });
            //比较符号
            NodeField CompareString_Type = new NodeField() { FieldName = "CompareType", FieldType = FieldType.EnumField, Describe = "比较符号<、>、<=、>=、==、!=" };
            (CompareString_Type.DefaultValue as EnumDefaultValue).EnumType = "CompareType";
            CompareString_Type.Label = "比较操作符";
            CompareString.AddField(CompareString_Type);
            //右边边参数类型
            NodeField CompareString_RightType = new NodeField() { FieldName = "RightType", FieldType = FieldType.EnumField, Describe = "" };
            (CompareString_RightType.DefaultValue as EnumDefaultValue).EnumType = "ParameterType";
            CompareString_RightType.Label = "右参数类型";
            CompareString.AddField(CompareString_RightType);
            //右边参数变量名
            CompareString.AddField(new NodeField() { FieldName = "RightParameter", Label = "右参数名", FieldType = FieldType.StringField, Describe = "右边参数变量名" });
            AddClass(CompareString);

            #endregion

            #region 动作节点

            //赋值节点Int
            NodeDefine AssignmentInt = new NodeDefine();
            AssignmentInt.ClassType = "AssignmentInt";
            AssignmentInt.Label = "赋值节点(Int)";
            AssignmentInt.NodeType = NodeType.Action;
            AssignmentInt.Describe = "赋值节点";
            AssignmentInt.AddField(new NodeField() { FieldName = "ParameterName", Label = "变量名", FieldType = FieldType.StringField, Describe = "参数变量名", Show = true });
            AssignmentInt.AddField(new NodeField() { FieldName = "Parameter", Label = "赋值Int", FieldType = FieldType.IntField, Describe = "参数值", Show = true });
            AddClass(AssignmentInt);

            //赋值节点Float
            NodeDefine AssignmentFloat = new NodeDefine();
            AssignmentFloat.ClassType = "AssignmentFloat";
            AssignmentFloat.Label = "赋值节点(Float)";
            AssignmentFloat.NodeType = NodeType.Action;
            AssignmentFloat.Describe = "赋值节点(Float)";
            AssignmentFloat.AddField(new NodeField() { FieldName = "ParameterName", Label = "变量名", FieldType = FieldType.StringField, Describe = "参数变量名", Show = true });
            AssignmentFloat.AddField(new NodeField() { FieldName = "Parameter", Label = "赋值Float", FieldType = FieldType.FloatField, Describe = "参数值", Show = true });
            AddClass(AssignmentFloat);

            //赋值节点String
            NodeDefine AssignmentString = new NodeDefine();
            AssignmentString.ClassType = "AssignmentString";
            AssignmentString.Label = "赋值节点(String)";
            AssignmentString.NodeType = NodeType.Action;
            AssignmentString.Describe = "赋值节点(String)";
            AssignmentString.AddField(new NodeField() { FieldName = "ParameterName", Label = "变量名", FieldType = FieldType.StringField, Describe = "参数变量名", Show = true });
            AssignmentString.AddField(new NodeField() { FieldName = "Parameter", Label = "赋值字符串", FieldType = FieldType.StringField, Describe = "参数值", Show = true });
            AddClass(AssignmentString);

            //等待节点
            NodeDefine Wait = new NodeDefine();
            Wait.ClassType = "Wait";
            Wait.Label = "等待节点";
            Wait.NodeType = NodeType.Action;
            Wait.Describe = "等待节点";
            NodeField WaintField = new NodeField() { FieldName = "Millisecond", Label = "等待时间(毫秒)", FieldType = FieldType.IntField, Describe = "等待时间（毫秒）", Show = true };
            IntDefaultValue WaintFieldDefaultField = WaintField.DefaultValue as IntDefaultValue;
            WaintFieldDefaultField.DefaultValue = 1000;
            Wait.AddField(WaintField);
            AddClass(Wait);

            //空操作节点
            NodeDefine Noop = new NodeDefine();
            Noop.ClassType = "Noop";
            Noop.Label = "空操作节点";
            Noop.NodeType = NodeType.Action;
            Noop.Describe = "空操作节点";
            AddClass(Noop);

            //输出Log节点
            NodeDefine Log = new NodeDefine();
            Log.ClassType = "Log";
            Log.Label = "输出节点";
            Log.NodeType = NodeType.Action;
            Log.Describe = "输出log节点";
            Log.AddField(new NodeField() { FieldName = "Content", Label = "输出内容", FieldType = FieldType.StringField, Describe = "输出的内容", Show = true });
            AddClass(Log);

            #endregion
        }

        public bool ExistClassType(string classType)
        {
            if (string.IsNullOrEmpty(classType))
                throw new System.Exception("NodeTemplate.ExistClassType() classType类型为空");

            for (int i = 0; i < m_Nodes.Count; i++)
            {
                NodeDefine nodeDefine = m_Nodes[i];
                if (nodeDefine == null)
                    continue;
                if (nodeDefine.ClassType == classType)
                    return true;
            }

            return false;
        }

        public bool ExistEnumType(string enumType)
        {
            if (string.IsNullOrEmpty(enumType))
                throw new System.Exception("NodeTemplate.ExistEnumType() 枚举类型为空");

            for (int i = 0; i < m_Enums.Count; i++)
            {
                CustomEnum tempEnum = m_Enums[i];
                if (tempEnum == null)
                    continue;
                if (tempEnum.EnumType == enumType)
                    return true;
            }

            return false;
        }


        public bool AddEnum(CustomEnum customEnum)
        {
            if (customEnum == null)
                return false;

            if (string.IsNullOrEmpty(customEnum.EnumType))
            {
                MainForm.Instance.ShowMessage("枚举类型为空!!!!");
                return false;
            }

            if (ExistEnumType(customEnum.EnumType))
            {
                MainForm.Instance.ShowMessage(string.Format("已存在枚举:{0},请换个枚举类型", customEnum.EnumType));
                return false;
            }

            m_Enums.Add(customEnum);
            return true;
        }

        /// <summary>
        /// 通过ClassType查找节点
        /// </summary>
        /// <param name="classType"></param>
        /// <returns></returns>
        public NodeDefine FindNode(string classType)
        {
            if (string.IsNullOrEmpty(classType))
                return null;

            for (int i = 0; i < m_Nodes.Count; i++)
            {
                NodeDefine nodeDefine = m_Nodes[i];
                if (nodeDefine != null && nodeDefine.ClassType == classType)
                    return nodeDefine;
            }

            return null;
        }

        /// <summary>
        /// 通过EnumType查找枚举配置
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public CustomEnum FindEnum(string enumType)
        {
            if (string.IsNullOrEmpty(enumType))
                return null;

            for (int i = 0; i < m_Enums.Count; i++)
            {
                CustomEnum customEnum = m_Enums[i];
                if (customEnum != null && customEnum.EnumType == enumType)
                    return customEnum;
            }

            return null;
        }

        public bool RemoveEnum(CustomEnum customEnum)
        {
            if (customEnum == null)
                return false;

            return m_Enums.Remove(customEnum);
        }

        /// <summary>
        /// 检验节点类ClassType
        /// </summary>
        /// <returns></returns>
        public VerifyInfo VerifyClassType()
        {
            //校验ClassType是否为空
            for (int i = 0; i < m_Nodes.Count; i++)
            {
                NodeDefine nodeDefine = m_Nodes[i];
                if (string.IsNullOrEmpty(nodeDefine.ClassType))
                {
                    return new VerifyInfo("存在空的ClassType");
                }
            }

            //检验ClassType是否相同
            for (int i = 0; i < m_Nodes.Count; i++)
            {
                NodeDefine classType_i = m_Nodes[i];
                if (classType_i != null)
                {
                    for (int ii = i + 1; ii < m_Nodes.Count; ii++)
                    {
                        NodeDefine classtType_ii = m_Nodes[ii];
                        if (classType_i.ClassType == classtType_ii.ClassType)
                            return new VerifyInfo(string.Format("行为树存在相同ClassType:{0}", classType_i.ClassType));
                    }
                }
            }

            return VerifyInfo.DefaultVerifyInfo;
        }

        /// <summary>
        /// 校节点类数据
        /// </summary>
        /// <returns></returns>
        public VerifyInfo VerifyNodeTemplate()
        {
            VerifyInfo verifyClassType = VerifyClassType();
            if (verifyClassType.HasError)
                return verifyClassType;

            for (int i = 0; i < m_Nodes.Count; i++)
            {
                NodeDefine nodeDefine = m_Nodes[i];
                VerifyInfo verifyNodeDefine = nodeDefine.VerifyNodeDefine();
                if (verifyNodeDefine.HasError)
                    return verifyNodeDefine;
            }

            return VerifyInfo.DefaultVerifyInfo;
        }

        /// <summary>
        /// 检验枚举类型
        /// </summary>
        /// <returns></returns>
        public VerifyInfo VerifyEnumType()
        {
            //校验EnumType是否为空
            for (int i = 0; i < m_Enums.Count; i++)
            {
                CustomEnum customEnum = m_Enums[i];
                if (string.IsNullOrEmpty(customEnum.EnumType))
                {
                    return new VerifyInfo("存在空的EnumType");
                }
            }

            //检验EnumType是否相同
            for (int i = 0; i < m_Enums.Count; i++)
            {
                CustomEnum classType_i = m_Enums[i];
                if (classType_i != null)
                {
                    for (int ii = i + 1; ii < m_Enums.Count; ii++)
                    {
                        CustomEnum classtType_ii = m_Enums[ii];
                        if (classType_i.EnumType == classtType_ii.EnumType)
                            return new VerifyInfo(string.Format("存在相同EnumType:{0}", classType_i.EnumType));
                    }
                }
            }

            return VerifyInfo.DefaultVerifyInfo;
        }

        /// <summary>
        /// 校验枚举数据
        /// </summary>
        /// <returns></returns>
        public VerifyInfo VerifyEnum()
        {
            //校验枚举类型
            VerifyInfo verifyEnumType = VerifyEnumType();
            if (verifyEnumType.HasError)
                return verifyEnumType;

            for (int i = 0; i < m_Enums.Count; i++)
            {
                CustomEnum customEnum = m_Enums[i];
                VerifyInfo verifyEnum = customEnum.VerifyEnum();
                if (verifyEnum.HasError)
                    return verifyEnum;
            }

            return VerifyInfo.DefaultVerifyInfo;
        }

        /// <summary>
        /// 移除未定义的枚举字段
        /// </summary>
        public void RemoveUnDefineEnumField()
        {
            for (int i = 0; i < m_Nodes.Count; i++)
            {
                NodeDefine nodeDefine = m_Nodes[i];
                nodeDefine.RemoveUnDefineEnumField();
            }
        }
    }
}