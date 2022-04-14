using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorTools.Windows.CodeCreaters.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorTools.Windows.CodeCreaters.States
{
    /// <summary>
    /// 方法信息数据
    /// </summary>
    public class MethodInfoData : MemberInfoData
    {
        /// <summary>
        /// 属性枚举字段名目录
        /// </summary>
        public override string propertyEnumFieldNameCategory => "方法";

        /// <summary>
        /// 方法信息
        /// </summary>
        public MethodInfo methodInfo { get; private set; }

        /// <summary>
        /// 成员信息
        /// </summary>
        public override MemberInfo memberInfo => methodInfo;

        /// <summary>
        /// 方法名
        /// </summary>
        public string methodName => methodInfo.Name;

        /// <summary>
        /// 成员类型
        /// </summary>
        public override Type memberType => methodInfo.ReturnType;

        /// <summary>
        /// 是否是静态成员信息
        /// </summary>
        public override bool isStaticMemberInfo => methodInfo.IsStatic;

        /// <summary>
        /// 名称
        /// </summary>
        public string _name;

        /// <summary>
        /// 名称
        /// </summary>
        public override string name => _name;

        /// <summary>
        /// 属性枚举名
        /// </summary>
        public override string propertyEnumName => "Method_" + base.propertyEnumName;

        /// <summary>
        /// 是否有效
        /// </summary>
        public override bool valid => parameterInfoDatas.All(p => p.valid);

        /// <summary>
        /// 是否有效
        /// </summary>
        public override bool canValid => parameterInfoDatas.All(p => p.canValid);

        /// <summary>
        /// 是否期望有效
        /// </summary>
        public override bool wantValid => parameterInfoDatas.All(p => p.wantValid);

        /// <summary>
        /// 当开始生成代码
        /// </summary>
        /// <param name="codeWirter"></param>
        public override void OnBeginCreateCode(BasePropertyCodeCreater codeCreater, ICodeWirter codeWirter)
        {
            //if (!wantOutput) return;
            base.OnBeginCreateCode(codeCreater, codeWirter);
            foreach (var p in parameterInfoDatas)
            {
                p.OnBeginCreateCode(codeCreater, codeWirter);
            }
        }

        /// <summary>
        /// 参数信息数据列表
        /// </summary>
        public List<ParameterInfoData> parameterInfoDatas = new List<ParameterInfoData>();

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="parameterInfo"></param>
        public MethodInfoData(MethodInfo methodInfo)
        {
            this.methodInfo = methodInfo;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="codeCreater"></param>
        public override void Init(BaseCodeCreater codeCreater)
        {
            base.Init(codeCreater);
            parameterInfoDatas.Clear();
            foreach (var p in methodInfo.GetParameters())
            {
                parameterInfoDatas.Add(new ParameterInfoData(p));
            }

            if (parameterInfoDatas.Count > 0)
            {
                _name = base.name + "_" + parameterInfoDatas.ToString(d => d.parameterInfo.ParameterType.Name, "_");
            }
            else
            {
                _name = base.name;
            }

            foreach (var data in parameterInfoDatas)
            {
                data.propertyEnumName = propertyEnumName;
                data.Init(codeCreater);
            }
        }

        /// <summary>
        /// 通过创建类型初始化本类
        /// </summary>
        /// <param name="createType"></param>
        /// <param name="propertyNameEnumType"></param>
        public override void InitByCreateType(Type createType, Type propertyNameEnumType)
        {
            base.InitByCreateType(createType, propertyNameEnumType);

            foreach (var p in parameterInfoDatas)
            {
                p.InitByCreateType(createType, propertyNameEnumType);
            }
        }
    }
}
