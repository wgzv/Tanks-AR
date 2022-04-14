﻿using System;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.DataBinders;
using XCSJ.Extension.Base.Dataflows.Models;

namespace XCSJ.PluginDataflows.Binders.XUnityEngine.XUI.XInputField
{
    /// <summary>
    /// 输入绑定器
    /// </summary>
    [Name("输入绑定器")]
    [DataBinder(typeof(InputField), nameof(InputField.text))]
    public class InputField_text_Binder : TypeMemberDataBinder<InputField>
    {
        /// <summary>
        /// 尝试获取成员值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override bool TryGetMemberValue(Type type, object obj, string memberName, out object value, object[] index = null)
        {
            if (obj is InputField entity && entity)
            {
                value = entity.text;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 尝试设置成员值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override bool TrySetMemberValue(Type type, object obj, string memberName, object value, object[] index = null)
        {
            if (obj is InputField entity && entity && TryConvertTo(value, out string outValue))
            {
                entity.text = outValue;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="onReceiveEvent"></param>
        public override void Bind(Action<XValueEventArg> onReceiveEvent)
        {
            base.Bind(onReceiveEvent);

            target.onEndEdit.AddListener(OnEndEdit);
        }

        /// <summary>
        /// 解绑
        /// </summary>
        public override void Unbind(Action<XValueEventArg> onReceiveEvent)
        {
            base.Unbind(onReceiveEvent);

            target.onEndEdit.RemoveListener(OnEndEdit);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="eventArg"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TryGet(XValueEventArg eventArg, out object value)
        {
            value = target.text;
            return true;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="linkedBindData"></param>
        /// <param name="eventArg"></param>
        public override void Set(ITypeMemberDataBinder linkedBindData, XValueEventArg eventArgs)
        {
            if (linkedBindData.TryGet(eventArgs, out object value) && TryConvertTo(value, out string outValue))
            {
                target.text = outValue;
            }
        }

        /// <summary>
        /// 值变化时调用
        /// </summary>
        /// <param name="value"></param>
        private void OnEndEdit(string value)
        {
            SendEvent(XPropertyChangeEventArgs.CreateChangedEvent(this, memberName, value));
        }
    }
}
