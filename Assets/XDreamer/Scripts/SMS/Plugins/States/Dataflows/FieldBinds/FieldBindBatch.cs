using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Dataflows.FieldBinds.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Dataflows.FieldBinds
{
    /// <summary>
    /// 批量字段绑定
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TBindInfo"></typeparam>
    public abstract class FieldBindBatch<T, TBindInfo> : BaseFieldBind<T>
        where T : FieldBindBatch<T, TBindInfo>
        where TBindInfo : BindInfo
    {
        /// <summary>
        /// 绑定信息
        /// </summary>
        [Name("绑定信息")]
        public List<TBindInfo> bindInfos = new List<TBindInfo>();

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => bindInfos.Count.ToString();

        /// <summary>
        /// 更新缓存
        /// </summary>
        protected void UpdateBuffer()
        {
            bindInfos.ForEach(i => i.UpdateBuffer());
        }

        /// <summary>
        /// 当进入激活态时回调
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            UpdateBuffer();
            bindInfos.ForEach(i =>
            {
                if (i._fieldInfo != null)
                {
                    i.lastFieldObject = i._fieldInfo.GetValue(i._obj);
                }
                switch (entryRule)
                {
                    case EBindRule.VariableToObject:
                        {
                            i.VariableToObject(liteMode, liteSeparator);
                            break;
                        }
                    case EBindRule.ObjectToVariable:
                        {
                            i.ObjectToVariable(liteMode, liteSeparator);
                            break;
                        }
                }
            });

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

            bindInfos.ForEach(i =>
            {
                if (i._fieldInfo == null) return;

                switch (bindRule)
                {
                    case EBindRule.ObjectToVariable:
                    case EBindRule.Both:
                        {
                            var newObject = i._fieldInfo.GetValue(i._obj);
                            if (alwaysUpdateVariable || !Equals(i.lastFieldObject, newObject))
                            {
                                i.ObjectToVariable(newObject, liteMode, liteSeparator);
                                i.lastFieldObject = newObject;
                            }
                            break;
                        }
                }
            });
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

        private void OnVariableValueChanged(Variable variable)
        {
            if (variable.varScope != EVarScope.Global) return;
            switch (bindRule)
            {
                case EBindRule.VariableToObject:
                case EBindRule.Both:
                    {
                        bindInfos.ForEach(i => i.OnVariableValueChanged(variable, liteMode, liteSeparator));
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// 绑定信息
    /// </summary>
    public abstract class BindInfo : IDisplayAsArrayElement, IFieldBind
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
        internal protected object _obj;

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

        internal protected FieldInfo _fieldInfo;

        /// <summary>
        /// 字段信息
        /// </summary>
        public FieldInfo fieldInfo => _fieldInfo != null ? _fieldInfo : _fieldInfo = fieldInfoRealtime;

        /// <summary>
        /// 字段信息实时
        /// </summary>
        public FieldInfo fieldInfoRealtime => TypeHelper.GetFieldInfo(obj?.GetType(), fieldName);

        internal object lastFieldObject;

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
        /// 更新缓存区
        /// </summary>
        public void UpdateBuffer()
        {
            _obj = obj;
            _fieldInfo = fieldInfoRealtime;
        }

        /// <summary>
        /// 获取GUI内容文本
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetGUIContentText(int index) => string.Format("变量{0}:{1}", (index + 1).ToString(), variable);

        /// <summary>
        /// 获取GUI内容提示
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetGUIContentTooltip(int index) => GetGUIContentText(index);

        /// <summary>
        /// 变量到对象
        /// </summary>
        /// <param name="liteMode"></param>
        /// <param name="liteSeparator"></param>
        public void VariableToObject(bool liteMode, string liteSeparator)
        {
            string value = "";
            if (ScriptManager.TryGetGlobalVariableValue(variable, out value))
            {
                VariableToObject(value, liteMode, liteSeparator);
            }
        }

        /// <summary>
        /// 变量到对象
        /// </summary>
        /// <param name="value"></param>
        /// <param name="liteMode"></param>
        /// <param name="liteSeparator"></param>
        internal protected void VariableToObject(string value, bool liteMode, string liteSeparator)
        {
            object fieldValue = null;
            try
            {
                _fieldInfo.SetValue(_obj, fieldValue = BindHelper.StringToObject(value, liteMode, liteSeparator, fieldInfo.FieldType));
            }
            finally
            {
                BindHelper.UpdateObject(this, fieldValue);
            }
        }

        private bool variableModifying = false;

        /// <summary>
        /// 对象到变量
        /// </summary>
        /// <param name="liteMode"></param>
        /// <param name="liteSeparator"></param>
        public void ObjectToVariable(bool liteMode, string liteSeparator)
        {
            ObjectToVariable(lastFieldObject, liteMode, liteSeparator);
        }

        /// <summary>
        /// 对象到变量
        /// </summary>
        /// <param name="o"></param>
        /// <param name="liteMode"></param>
        /// <param name="liteSeparator"></param>
        internal protected void ObjectToVariable(object o, bool liteMode, string liteSeparator)
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

        /// <summary>
        /// 当变量值修改时回调
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="liteMode"></param>
        /// <param name="liteSeparator"></param>
        /// <returns></returns>
        public bool OnVariableValueChanged(Variable variable, bool liteMode, string liteSeparator)
        {
            if (variable.name != this.variable) return false;
            if (!variableModifying && _fieldInfo != null)
            {
                VariableToObject(variable.value, liteMode, liteSeparator);
            }
            return true;
        }
    }
}
