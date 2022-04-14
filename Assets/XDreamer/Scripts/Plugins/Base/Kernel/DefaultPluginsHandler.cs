﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base.Kernel;

namespace XCSJ.Extension.Base.Kernel
{
    /// <summary>
    /// 默认插件处理器
    /// </summary>
    public class DefaultPluginsHandler : InstanceClass<DefaultPluginsHandler>, IPluginsHandler
    {
        /// <summary>
        /// XR模式是否启用
        /// </summary>
        public bool isXRMode => XRSettings.enabled;

        /// <summary>
        /// 游戏视图渲染模式
        /// </summary>
        public EGameViewRenderMode gameViewRenderMode => (EGameViewRenderMode)XRSettings.gameViewRenderMode;

        /// <summary>
        /// 用于判断鼠标（手势）当前是否在UGUI上
        /// </summary>
        /// <returns></returns>
        public bool IsOnUGUI()
        {
            var eventSystem = EventSystem.current;
            if (!eventSystem) return false;
            if (eventSystem.IsPointerOverGameObject()) return true;
            for (int i = 0; i < Input.touchCount; ++i)
            {
                if (eventSystem.IsPointerOverGameObject(Input.GetTouch(i).fingerId)) return true;
            }
            return false;
        }

        /// <summary>
        /// 判断当前是否是垂直布局
        /// </summary>
        /// <returns></returns>
        public bool IsVerticalGUILayout() => GUILayoutUtility_LinkType.topLevel.isVertical;

        /// <summary>
        /// 当编辑检查器脚本时回调
        /// </summary>
        public event Action<UnityEngine.Object> onEditInspectorScript;

        /// <summary>
        /// 编辑检查器脚本
        /// </summary>
        /// <param name="obj"></param>
        public void EditInspectorScript(UnityEngine.Object obj)
        {
            onEditInspectorScript?.Invoke(obj);
        }

#if UNITY_EDITOR

        /// <summary>
        /// 当需要延时执行时回调
        /// </summary>
        public event Action<object, Action<object>, float> onNeedDelayCall;

#endif

        /// <summary>
        /// 延时执行
        /// </summary>
        /// <param name="param"></param>
        /// <param name="action"></param>
        /// <param name="delayTime"></param>
        public void DelayCall(object param, Action<object> action, float delayTime)
        {
#if UNITY_EDITOR
            onNeedDelayCall?.Invoke(param, action, delayTime);
#else
            CommonFun.DelayCall(delayTime, param, action);
#endif
        }

        /// <summary>
        /// 获取通用材质
        /// </summary>
        /// <returns></returns>
        public Material GetCommonMaterial()
        {
            try
            {
                return GenericStandardScriptManager.instance._commonMaterial;
            }
            catch { return null; }
        }
    }
}
