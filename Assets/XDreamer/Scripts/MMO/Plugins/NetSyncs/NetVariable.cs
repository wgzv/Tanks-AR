using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMMO.NetSyncs
{
    [XCSJ.Attributes.Icon(EIcon.Variable)]
    [DisallowMultipleComponent]
    [Name("网络变量")]
    [Tool(MMOHelper.CategoryName, MMOHelper.ToolPurpose, rootType = typeof(MMOManager))]
    public class NetVariable : NetMB
    {
        [Name("变量名")]
        [GlobalVariable(true)]
        public string variableName = "";

        [SyncVar]
        [Readonly]
        [Name("变量值")]
        public string variableValue = "";

        [Readonly]
        [Name("上一次变量值")]
        public string lastVariableValue;

        [Readonly]
        [Name("原始变量值")]
        public string originalVariableValue;

        public override void OnSyncEnable()
        {
            base.OnSyncEnable();

            ScriptManager.TryGetGlobalVariableValue(variableName, out variableValue);
            originalVariableValue = lastVariableValue = variableValue;

            Variable.onValueChanged += OnVariableValueChanged;
        }

        public override void OnSyncDisable()
        {
            base.OnSyncDisable();

            Variable.onValueChanged -= OnVariableValueChanged;

            ScriptManager.TrySetGlobalVariableValue(variableName, originalVariableValue);
        }

        protected override bool OnTimedCheckChange()
        {
            return variableValue != lastVariableValue;
        }

        protected override void OnSyncVarChanged()
        {
            base.OnSyncVarChanged();
            ScriptManager.TrySetGlobalVariableValue(variableName, variableValue);
            lastVariableValue = variableValue;
        }

        private void OnVariableValueChanged(Variable variable)
        {
            if (variable.name == this.variableName && variable.varScope == EVarScope.Global)
            {
                variableValue = variable.value;
            }
        }
    }
}
