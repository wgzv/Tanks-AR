using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Helpers;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Dataflows.FieldBinds.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Dataflows.FieldBinds
{
    /// <summary>
    /// 字段绑定
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class FieldBind<T> : BaseFieldBind<T>, IFieldBind where T : FieldBind<T>
    {
        /// <summary>
        /// 变量
        /// </summary>
        [Name("变量")]
        [GlobalVariable(true)]
        public string variable;

        string IFieldBind.variable => variable;

        /// <summary>
        /// 对象
        /// </summary>
        protected object _obj;

        /// <summary>
        /// 对象
        /// </summary>
        public abstract object obj { get; }

        /// <summary>
        /// 字段名
        /// </summary>
        public abstract string fieldName { get; }

        /// <summary>
        /// 字段名的字段名
        /// </summary>
        public abstract string fieldNameOfFieldName { get; }

        protected FieldInfo _fieldInfo;

        /// <summary>
        /// 字段信息
        /// </summary>
        public FieldInfo fieldInfo => _fieldInfo != null ? _fieldInfo : _fieldInfo = fieldInfoRealtime;

        /// <summary>
        /// 字段信息实时
        /// </summary>
        public FieldInfo fieldInfoRealtime => TypeHelper.GetFieldInfo(obj?.GetType(), fieldName);

        /// <summary>
        /// 能否绑定
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public virtual bool CanBind(FieldInfo fieldInfo) => true;

        /// <summary>
        /// 获取绑定字段列表
        /// </summary>
        /// <returns></returns>
        public virtual List<FieldInfo> GetBindFields()
        {
            return CommonFun.GetFieldsInInspector(obj?.GetType()).Where(CanBind).ToList();
        }

        /// <summary>
        /// 更新缓存
        /// </summary>
        protected void UpdateBuffer()
        {
            _obj = obj;
            _fieldInfo = fieldInfoRealtime;
        }

        /// <summary>
        /// 当进入激活态时回调
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            UpdateBuffer();
            
            if (_fieldInfo != null)
            {
                lastFieldObject = _fieldInfo.GetValue(_obj);
                switch (entryRule)
                {
                    case EBindRule.VariableToObject:
                        {
                            string value = "";
                            if (ScriptManager.TryGetGlobalVariableValue(variable, out value))
                            {
                                VariableToObject(value);
                            }
                            break;
                        }
                    case EBindRule.ObjectToVariable:
                        {
                            ObjectToVariable(lastFieldObject);
                            break;
                        }
                }
            }            

            Variable.onValueChanged += OnVariableValueChanged;

            //直接设置为完成态
            finished = true;
        }

        /// <summary>
        /// 当更新时回调
        /// </summary>
        /// <param name="data"></param>
        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);

            if (realtimeUpdate) UpdateBuffer();
            if (_fieldInfo == null || objectModifying) return;

            switch (bindRule)
            {
                case EBindRule.ObjectToVariable:
                case EBindRule.Both:
                    {
                        var newObject = _fieldInfo.GetValue(_obj);

                        if (alwaysUpdateVariable || !ObjectHelper.ObjectEquals(lastFieldObject, newObject))
                        {
                            ObjectToVariable(lastFieldObject = newObject);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 当退出激活态时回调
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            Variable.onValueChanged -= OnVariableValueChanged;
            base.OnExit(data);
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return variable + BindHelper.ToAbbreviations(bindRule) + CommonFun.Name(_fieldInfo);
            }
#endif
            return variable + BindHelper.ToAbbreviations(bindRule) + CommonFun.Name(fieldInfoRealtime);
        }

        private string GetVariableValue(object obj, FieldInfo fieldInfo)
        {
            if (fieldInfo == null || obj == null) return "";
            return BindHelper.ObjectToString(fieldInfo.GetValue(obj), liteMode, liteSeparator);
        }

        /// <summary>
        /// 获取实时参数信息
        /// </summary>
        /// <returns></returns>
        public string GetRealtimeParamInfo() => GetVariableValue(obj, fieldInfoRealtime);

        /// <summary>
        /// 获取缓存参数信息
        /// </summary>
        /// <returns></returns>
        public string GetBufferParamInfo() => GetVariableValue(_obj, _fieldInfo);

        #region 变量 --> 对象

        private bool objectModifying = false;

        private void OnVariableValueChanged(Variable variable)
        {
            if (_fieldInfo == null || variableModifying) return;
            if (variable.varScope != EVarScope.Global || variable.name != this.variable) return;
            switch (bindRule)
            {
                case EBindRule.VariableToObject:
                case EBindRule.Both:
                    {
                        VariableToObject(variable.value);
                        break;
                    }
            }
        }

        private void VariableToObject(string value)
        {
            try
            {
                objectModifying = true;

                _fieldInfo.SetValue(_obj, lastFieldObject = BindHelper.StringToObject(value, liteMode, liteSeparator, fieldInfo.FieldType));
            }
            finally
            {
                objectModifying = false;
                BindHelper.UpdateObject(this, lastFieldObject);
            }
        }

        #endregion

        #region 对象 --> 变量

        private object lastFieldObject;

        private bool variableModifying = false;

        private void ObjectToVariable(object o)
        {
            try
            {
                variableModifying = true;

                ScriptManager.TrySetGlobalVariableValue(variable, BindHelper.ObjectToString(o, liteMode, liteSeparator));
            }
            finally
            {
                variableModifying = false;
            }
        }

        #endregion
    }


}
