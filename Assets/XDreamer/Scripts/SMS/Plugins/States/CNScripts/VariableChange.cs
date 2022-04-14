using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.CNScripts
{
    /// <summary>
    /// 变量修改:可用于捕获变量修改事件
    /// </summary>
    [ComponentMenu(SMSHelperExtension.CNScriptCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(VariableChange))]
    [Tip("可用于捕获变量修改事件")]
    [XCSJ.Attributes.Icon(EIcon.Variable)]
    public class VariableChange : Trigger<VariableChange>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "变量修改";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.CNScriptCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.CNScriptCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(VariableChange))]
        [Tip("可用于捕获变量修改事件")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("变量")]
        [GlobalVariable(true)]
        public string variable;

        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

            Variable.onValueChanged += OnVariableValueChanged;
        }

        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);

            Variable.onValueChanged -= OnVariableValueChanged;
        }

        private void OnVariableValueChanged(Variable variable)
        {
            if (variable.name == this.variable && variable.varScope == EVarScope.Global)
            {
                finished = true;
            }
        }
    }
}
