using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginVehicleDrive
{
    /// <summary>
    /// 中文变量仪表盘
    /// </summary>
    [Name("中文变量仪表盘")]
    public class CNVariableDashboard : Dashboard
    {
        /// <summary>
        /// 变量
        /// </summary>
        [Name("变量")]
        [GlobalVariable(true)]
        public string _variable;

        public override float needleAngle
        {
            get
            {
                if (_valid && ScriptManager.TryGetGlobalVariableValue(_variable, out string value) 
                    && float.TryParse(value, out float number))
                {
                    return number;
                }
                return 0;
            }
        }

        /// 开始
        /// </summary>
        protected override void Start()
        {
            base.Start();

            _valid = _valid && !string.IsNullOrEmpty(_variable);
        }
    }
}
