using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 渲染器区间
    /// </summary>
    [ComponentMenu("动作/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(RendererRange))]
    [XCSJ.Attributes.Icon(index = 33629)]
    [RequireComponent(typeof(GameObjectSet))]
    [Obsolete("本状态组件不再推荐使用,使用[渲染器<具体功能>区间]的状态组件替代")]
    public class RendererRange : RangeHandle<RendererRange, RendererRange.Recorder>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "渲染器区间";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/" + Title, typeof(SMSManager))]
        [Obsolete("本状态组件不再推荐使用,使用渲染器具体功能区间状态组件替代")]
        [Name(Title, nameof(RendererRange))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);
        
        [Name("操作对象")]
        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        [Name("包含成员")]
        public bool includeChildren = true;

        public enum EHandleRule
        {
            [Name("可用")]
            [Tip("渲染器组件可用性")]
            Enabled = 0,

            [Name("颜色")]
            [Tip("渲染器组件上材质的_Color属性")]
            Color,

            [Name("材质")]
            [Tip("渲染器组件上材质")]
            Material,

            [Name("透明度")]
            [Tip("渲染器组件上材质的_Color属性透明度")]
            Alpha,
        }

        [Name("处理规则")]
        [EnumPopup]
        public EHandleRule handleRule = EHandleRule.Enabled;

        [Name("可用")]
        [HideInSuperInspector(nameof(handleRule), EValidityCheckType.NotEqual | EValidityCheckType.Or, EHandleRule.Enabled, nameof(recovery), EValidityCheckType.Equal, true)]
        [EnumPopup]
        public EBool enabled = EBool.Yes;

        [Name("颜色")]
        [HideInSuperInspector(nameof(handleRule), EValidityCheckType.NotEqual | EValidityCheckType.Or, EHandleRule.Color, nameof(recovery), EValidityCheckType.Equal, true)]
        public UnityEngine.Color color = UnityEngine.Color.green;
        
        [Name("还原")]
        public bool recovery = false;

        [Name("材质")]
        [HideInSuperInspector(nameof(handleRule), EValidityCheckType.NotEqual | EValidityCheckType.Or, EHandleRule.Material, nameof(recovery), EValidityCheckType.Equal, true)]
        public List<Material> materials = new List<Material>();

        [Name("透明度")]
        [Range(0, 1)]
        [HideInSuperInspector(nameof(handleRule), EValidityCheckType.NotEqual, EHandleRule.Alpha)]
        public float alpha = 1;

        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            if (MathX.ApproximatelyZero(percentOnExit))
            {
                recorder.Recover();
            }
        }

        public class Recorder : RendererRecorder, IRangeHandleRecorder<RendererRange>
        {
            public RendererRange rangeHandle;

            public void Record(RendererRange rangeHandle)
            {
                this.rangeHandle = rangeHandle;
                if (!rangeHandle.gameObjectSet) return;
                _records.Clear();

                foreach (var go in rangeHandle.gameObjectSet.objects)
                {
                    if (go)
                    {
                        Record(go);
                        if (rangeHandle.includeChildren) Record(go.GetComponentsInChildren<Renderer>());
                    }
                }
            }

            public void SetPercent(Percent percent)
            {
                if (!percent.inRange) return;

                if (rangeHandle.recovery)
                {
                    foreach (var info in _records)
                    {
                        info.Recover();
                    }
                    return;
                }

                switch (rangeHandle.handleRule)
                {
                    case EHandleRule.Enabled:
                        {
                            foreach (var info in _records)
                            {
                                info.SetEnabled(rangeHandle.enabled);
                            }
                            break;
                        }
                    case EHandleRule.Color:
                        {
                            foreach (var info in _records)
                            {
                                info.SetColor(rangeHandle.color);
                            }
                            break;
                        }
                    case EHandleRule.Alpha:
                        {
                            foreach (var info in _records)
                            {
                                info.SetAlpha(rangeHandle.alpha);
                            }
                            break;
                        }
                    case EHandleRule.Material:
                        {
                            foreach (var info in _records)
                            {
                                info.SetMaterial(rangeHandle.materials.ToArray());
                            }
                            break;
                        }
                }               
            }

            public void SetRange(EBool boolValue, ELifecycleEventLite lifecycleEventLite = ELifecycleEventLite.OnUpdate) { }
        }

        public override string ToFriendlyString()
        {
            return recovery? CommonFun.Name(handleRule)+"还原":base.ToFriendlyString();
        }
    }
}
