using UnityEngine;
using System.Collections;
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
    /// EasyAR - QRCode事件
    /// </summary>
    public enum EScriptEasyAREventType
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

        [Name("文本消息时 执行")]
        [Tip("文本消息，比如二维码扫描结果的文本字符串")]
        TextMessage,
    }

    /// <summary>
    /// 脚本EasyAR事件集合
    /// </summary>
    [Serializable]
    public class ScriptEasyAREventSet : BaseScriptEventSet<EScriptEasyAREventType>
    {
        //
    }

    /// <summary>
    /// 脚本EasyAR事件
    /// </summary>
    [Serializable]
    [Name("脚本EasyAR事件")]
    [DisallowMultipleComponent]
    [AddComponentMenu(Product.Name + "/EasyAR/Script EasyAR Event")]
    [RequireManager(typeof(EasyARManager))]
    public class ScriptEasyAREvent : BaseScriptEvent<EScriptEasyAREventType, ScriptEasyAREventSet>
    {
        [Name("EasyAR组件")]
        [Tip("EasyAR的根节点核心组件")]
#if XDREAMER_EASYAR_2_3_0
        public EasyARBehaviour easyAR;
#else
        public Component easyAR;
#endif

        public override void Awake()
        {
            base.Awake();
#if XDREAMER_EASYAR_2_3_0
            if (!easyAR) easyAR = EasyARManager.InitEasyAR(easyAR);
            if (easyAR)
            {
                easyAR.Initialize();

                foreach (var behaviour in ARBuilder.Instance.ARCameraBehaviours)
                {
                    behaviour.TargetFound += OnTargetFound;
                    behaviour.TargetLost += OnTargetLost;
                    behaviour.TextMessage += OnTextMessage;
                }
                foreach (var behaviour in ARBuilder.Instance.ImageTrackerBehaviours)
                {
                    behaviour.TargetLoad += OnTargetLoad;
                    behaviour.TargetUnload += OnTargetUnload;
                }
            }
            else
            {
                Log.Error("未找到EasyARBehaviour组件");
            }
#endif
        }

        public override void Start()
        {
            base.Start();
            RunScriptEvent(EScriptEasyAREventType.Start);
        }

#if XDREAMER_EASYAR_2_3_0

        protected void OnTargetFound(ARCameraBaseBehaviour arcameraBehaviour, TargetAbstractBehaviour targetBehaviour, Target target)
        {
            RunScriptEvent(EScriptEasyAREventType.TargetFound, target.Id.ToString());
        }

        protected void OnTargetLost(ARCameraBaseBehaviour arcameraBehaviour, TargetAbstractBehaviour targetBehaviour, Target target)
        {
            RunScriptEvent(EScriptEasyAREventType.TargetFound, target.Id.ToString());
        }

        protected void OnTargetLoad(ImageTrackerBaseBehaviour trackerBehaviour, ImageTargetBaseBehaviour targetBehaviour, Target target, bool status)
        {
            RunScriptEvent(EScriptEasyAREventType.TargetFound, target.Id.ToString());
        }

        protected void OnTargetUnload(ImageTrackerBaseBehaviour trackerBehaviour, ImageTargetBaseBehaviour targetBehaviour, Target target, bool status)
        {
            RunScriptEvent(EScriptEasyAREventType.TargetFound, target.Id.ToString());
        }

        protected void OnTextMessage(ARCameraBaseBehaviour arcameraBehaviour, string text)
        {
            RunScriptEvent(EScriptEasyAREventType.TextMessage, text);
        }
#else
        protected void OnTargetFound(MonoBehaviour arcameraBehaviour, MonoBehaviour targetBehaviour, object target) { }

        protected void OnTargetLost(MonoBehaviour arcameraBehaviour, MonoBehaviour targetBehaviour, object target) { }

        protected void OnTargetLoad(MonoBehaviour trackerBehaviour, MonoBehaviour targetBehaviour, object target, bool status) { }

        protected void OnTargetUnload(MonoBehaviour trackerBehaviour, MonoBehaviour targetBehaviour, object target, bool status) { }

        protected void OnTextMessage(MonoBehaviour arcameraBehaviour, string text) { }
#endif
    }
}