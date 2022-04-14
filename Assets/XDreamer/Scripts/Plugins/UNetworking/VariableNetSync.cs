using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using XCSJ.PluginCommonUtils;
using UnityEngine.Networking;
using XCSJ.Attributes;

namespace XCSJ.UNetworking
{
#if UNITY_2019_1_OR_NEWER    
    [Obsolete("因Unity中高级API类NetworkBehaviour已被移除，所以本类不推荐使用!", true)]
#elif UNITY_2018_3_OR_NEWER
    [Obsolete("因Unity中高级API类NetworkBehaviour已被标记过时，所以本类不推荐使用!")]
    [NetworkSettings(sendInterval = 0.1f)]
#endif
    public class VariableNetSync : BaseVarNetSync
    {
#if !UNITY_2019_1_OR_NEWER
        [SyncVar(hook = "UpdateVar")]
#endif
        [HideInInspector]
        public string variableValue = "";

        [Name("变量")]
        [GlobalVariable(true)]
        public string variable = "";

        public Rect drawRect = new Rect(0, 0, 100, 100);

        private string lastVarValue = "";

        public void Awake()
        {
            ScriptManager.TryGetGlobalVariableValue(variable, out variableValue);
            lastVarValue = variableValue;
        }

        protected override bool IsVarChanged()
        {
            return lastVarValue!= variableValue;
        }

        protected override void DataToVar()
        {
            string var = "";
            ScriptManager.TryGetGlobalVariableValue(variable, out var);
            variableValue = var;
            //Debug.Log(variable + ":--> DataToVar:" + variableValue);
        }

        protected override void VarToData()
        {
            //Debug.Log(variable + ":--> VarToData:" + variableValue);
            ScriptManager.TrySetGlobalVariableValue(variable, variableValue);
            lastVarValue = variableValue;
        }

        public void UpdateVar(string varValue)
        {
#if !UNITY_2019_1_OR_NEWER
            if (isClient)
#endif
            {
                //Debug.Log(variable + ":-->client UpdateVar value:" + varValue);
                variableValue = varValue;
                ScriptManager.TrySetGlobalVariableValue(variable, variableValue);
            }
        }

        //private void OnGUI()
        //{
        //    GUI.Button(drawRect, variable+":"+ variableValue);
        //}
    }
}
