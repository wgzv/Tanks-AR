using XCSJ.EditorTools.Windows.CodeCreaters.Base;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.EditorTools.Windows.CodeCreaters.States
{
    /// <summary>
    /// 基础生命周期执行器代码生成器
    /// </summary>
    public abstract class BaseLifecycleExecutorCodeCreater : BaseStateComponentCodeCreater
    {
        /// <summary>
        /// 基础类型定义字符串
        /// </summary>
        protected override string baseTypeDefineString => "LifecycleExecutor<" + name + ">";

        /// <summary>
        /// 当开始生成代码
        /// </summary>
        /// <param name="codeWirter"></param>
        protected override void OnBeginCreateCode(ICodeWirter codeWirter)
        {
            base.OnBeginCreateCode(codeWirter);

            AddUsedType(typeof(LifecycleExecutor), typeof(LifecycleExecutor<>));
        }

        /// <summary>
        /// 当开始类型内容
        /// </summary>
        /// <param name="codeWirter"></param>
        protected override void OnCreateTypeContent(ICodeWirter codeWirter)
        {
            base.OnCreateTypeContent(codeWirter);

            OnCreateFunc_Execute(codeWirter);
            OnCreateFunc_ToFriendlyString(codeWirter);
            OnCreateFunc_DataValidity(codeWirter);
            OnCreateFunc_Reset(codeWirter);
        }

        /// <summary>
        /// 当创建函数Execute
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnCreateFunc_Execute(ICodeWirter codeWirter)
        {
            codeWirter.WriteSummary("执行");
            codeWirter.WriteSummaryParam("stateData", "数据信息");
            codeWirter.WriteSummaryParam("executeMode", "执行模式");
            codeWirter.WriteFormat("public override void Execute(StateData stateData, EExecuteMode executeMode)");
            codeWirter.Write("{");
            codeWirter.IncreaseIndent();
            OnCreateFuncContent_Execute(codeWirter);
            codeWirter.DecreaseIndent();
            codeWirter.Write("}");
            codeWirter.Write();
        }

        /// <summary>
        /// 当创建函数Execute内容
        /// </summary>
        /// <param name="codeWirter"></param>
        protected virtual void OnCreateFuncContent_Execute(ICodeWirter codeWirter) { }
    }
}
