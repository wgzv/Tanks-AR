using UnityEngine;
using System;
using XCSJ.PluginCommonUtils;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.CNScripts;
#if XDREAMER_EASYAR_4_0_1 || XDREAMER_EASYAR_3_0_1
using easyar;
#elif XDREAMER_EASYAR_2_3_0
using EasyAR;
#endif

namespace XCSJ.PluginEasyAR
{
    /// <summary>
    /// EasyAR事件
    /// </summary>
    public enum EScriptImageTargetEventType
    {
        [Name("启动时 执行")]
        Start = 0,

        /// <summary>
        /// 选择时
        /// </summary>
        [Name("目标识别时 执行")]
        TargetFound,

        [Name("目标丢失时 执行")]
        TargetLost,

        [Name("目标加载时 执行")]
        TargetLoad,

        [Name("目标卸载时 执行")]
        TargetUnload,
    }

    [Serializable]
    public class ScriptImageTargetEventSet : BaseScriptEventSet<EScriptImageTargetEventType>
    {
        //
    }

    /// <summary>
    /// EasyAR事件处理
    /// </summary>
    [Name("脚本图片目标事件")]
    [Tip("用于捕获对指定图片(识别图、Marker)识别情况的回调；")]
    [Serializable]
    [RequireManager(typeof(EasyARManager))]
#if XDREAMER_EASYAR_4_0_1 || XDREAMER_EASYAR_3_0_1
    [RequireComponent(typeof(ExtendImageTargetController))]
#elif XDREAMER_EASYAR_2_3_0
    [RequireComponent(typeof(ImageTargetBaseBehaviour))]
#endif
    public class ScriptImageTargetEvent : BaseScriptEvent<EScriptImageTargetEventType, ScriptImageTargetEventSet>
    {
#if XDREAMER_EASYAR_4_0_1
        private ImageTargetController mTrackableBehaviour;
#elif XDREAMER_EASYAR_3_0_1
        private ExtendImageTargetController mTrackableBehaviour;
#elif XDREAMER_EASYAR_2_3_0
        private ImageTargetBaseBehaviour mTrackableBehaviour;
#else
        private Component mTrackableBehaviour;
#endif

        public override void Awake()
        {
            base.Awake();
#if XDREAMER_EASYAR_4_0_1 || XDREAMER_EASYAR_3_0_1
            mTrackableBehaviour = GetComponent<ImageTargetMB>();
#elif XDREAMER_EASYAR_2_3_0
            mTrackableBehaviour = GetComponent<ImageTargetBaseBehaviour>();
#endif

#if XDREAMER_EASYAR_4_0_1 || XDREAMER_EASYAR_3_0_1 || XDREAMER_EASYAR_2_3_0
            if (mTrackableBehaviour)
            {
                mTrackableBehaviour.TargetLoad += OnTargetLoad;
                mTrackableBehaviour.TargetUnload += OnTargetUnload;
                mTrackableBehaviour.TargetFound += OnTargetFound;
                mTrackableBehaviour.TargetLost += OnTargetLost;
            }
            else
            {
                Log.ErrorFormat("游戏对象:[{0}] 不包含{1}组件", CommonFun.GameObjectToString(gameObject), mTrackableBehaviour.GetType());
            }
#endif
        }

        public override void Start()
        {
            base.Start();
            RunScriptEvent(EScriptImageTargetEventType.Start);
        }

#if XDREAMER_EASYAR_4_0_1
        #region 回调函数
        protected void OnTargetFound()
        {
            RunScriptEvent(EScriptImageTargetEventType.TargetFound);
        }

        protected void OnTargetLost()
        {
            RunScriptEvent(EScriptImageTargetEventType.TargetLost);
        }

        protected void OnTargetLoad(Target target, bool status)
        {
            RunScriptEvent(EScriptImageTargetEventType.TargetLoad);
        }

        protected void OnTargetUnload(Target target, bool status)
        {
            RunScriptEvent(EScriptImageTargetEventType.TargetUnload);
        }
        #endregion
#elif XDREAMER_EASYAR_3_0_1
        #region 回调函数
        protected void OnTargetFound(ExtendImageTargetController controller)
        {
            RunScriptEvent(EScriptImageTargetEventType.TargetFound);
        }

        protected void OnTargetLost(ExtendImageTargetController controller)
        {
            RunScriptEvent(EScriptImageTargetEventType.TargetLost);
        }

        protected void OnTargetLoad(ExtendImageTargetController controller)
        {
            RunScriptEvent(EScriptImageTargetEventType.TargetLoad);
        }

        protected void OnTargetUnload(ExtendImageTargetController controller)
        {
            RunScriptEvent(EScriptImageTargetEventType.TargetUnload);
        }
#endregion
#elif XDREAMER_EASYAR_2_3_0
#region 回调函数

        protected void OnTargetFound(TargetAbstractBehaviour behaviour)
        {
            RunScriptEvent(EScriptImageTargetEventType.TargetFound);
        }

        protected void OnTargetLost(TargetAbstractBehaviour behaviour)
        {
            RunScriptEvent(EScriptImageTargetEventType.TargetLost);
        }

        protected void OnTargetLoad(ImageTargetBaseBehaviour behaviour, ImageTrackerBaseBehaviour tracker, bool status)
        {
            RunScriptEvent(EScriptImageTargetEventType.TargetLoad);
        }

        protected void OnTargetUnload(ImageTargetBaseBehaviour behaviour, ImageTrackerBaseBehaviour tracker, bool status)
        {
            RunScriptEvent(EScriptImageTargetEventType.TargetUnload);
        }

#endregion
#endif
    }
}
