using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 旋转:旋转组件是游戏对象的旋转动画。在给定的时间内，游戏对象做旋转运动，旋转完成后，组件切换为完成态。
    /// </summary>
    [ComponentMenu("动作/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(Rotate))]
    [Tip("旋转组件是游戏对象的旋转动画。在给定的时间内，游戏对象做旋转运动，旋转完成后，组件切换为完成态。")]
    [Attributes.Icon]
    [DisallowMultipleComponent]
    public class Rotate : TransformMotion<Rotate>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "旋转";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/"+ Title, typeof(SMSManager))]
        [Name(Title, nameof(Rotate))]
        [Tip("旋转组件是游戏对象的旋转动画。在给定的时间内，游戏对象做旋转运动，旋转完成后，组件切换为完成态。")]
        [XCSJ.Attributes.Icon(EIcon.Rotate)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        public enum ERotateRule
        {
            [Name("无动作")]
            None = 0,

            [Name("本地")]
            [Tip("以游戏对象的自身坐标系的三轴为基准，线性旋转指定角度；此时 值 表示期望的三轴旋转角度；")]
            Local,

            [Name("世界")]
            [Tip("以游戏对象的世界坐标系的三轴为基准，线性旋转指定角度；此时 值 表示期望的三轴旋转角度；")]
            World,

            [Name("注视")]
            [Tip("将游戏对象在结束时指向特定的方向；即将游戏对象自身坐标系Z轴与注视方向重合；此时 值 表示期望注视的目标点坐标(世界坐标系)；")]
            LookAt,

            [Name("本地轴")]
            [Tip("游戏对象以指定的方向轴为基准，旋转中心为游戏对象的变换中心，线性旋转特定角度；此时 值 表示方向轴(本地坐标系)；")]
            LocalAxis,

            [Name("世界轴")]
            [Tip("游戏对象以指定的方向轴为基准，旋转中心为游戏对象的变换中心，线性旋转特定角度；此时 值 表示方向轴(世界坐标系)；")]
            WorldAxis,

            [Name("世界点轴")]
            [Tip("游戏对象以指定的方向轴为基准，旋转中心为轴点，线性旋转特定角度；此时 值 表示方向轴(世界坐标系)；")]
            WorldPointAxis,

            [Name("世界点轴后本地")]
            [Tip("游戏对象以指定的方向轴为基准，旋转中心为轴点，线性旋转特定角度；旋转完成后游戏对象的旋转设置为原始的本地旋转；此时 值 表示方向轴(世界坐标系)；")]
            WorldPointAxisThenLocal,
        }

        [Name("旋转规则")]
        [EnumPopup]
        public ERotateRule rotateRule = ERotateRule.Local;

        [Name("值")]
        [Tip("根据 旋转规则 不同本项信息具有不同解释；具体查阅 旋转规则 参数的说明；")]
        public Vector3 value = new Vector3();

        [Name("向上")]
        [Tip("注视时的上方向(世界坐标系)；")]
        [HideInSuperInspector(nameof(rotateRule), EValidityCheckType.NotEqual, ERotateRule.LookAt)]
        public Vector3 upwards = Vector3.up;

        [Name("轴角度")]
        [Tip("绕旋转轴发生旋转的角度；左手法则；")]
        [HideInSuperInspector(nameof(rotateRule), EValidityCheckType.Less | EValidityCheckType.Or, ERotateRule.LocalAxis, nameof(rotateRule), EValidityCheckType.Greater, ERotateRule.WorldPointAxisThenLocal)]
        public float axisAngle = 0;

        [Name("轴点")]
        [Tip("点轴旋转方式的轴点坐标(世界坐标系)；")]
        [HideInSuperInspector(nameof(rotateRule), EValidityCheckType.Less | EValidityCheckType.Or, ERotateRule.WorldPointAxis, nameof(rotateRule), EValidityCheckType.Greater, ERotateRule.WorldPointAxisThenLocal)]
        public Vector3 axisPoint = new Vector3();

        [Name("朝向轴点")]
        [HideInSuperInspector(nameof(rotateRule), EValidityCheckType.NotEqual, ERotateRule.WorldPointAxis)]
        public bool lookatAxisPoint = false;

        private float lastAxisAngle = 0;

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            lastAxisAngle = 0;
        }

        protected override void SetPercent(TransformRecorder.Info info, Percent percent)
        {
            switch (rotateRule)
            {
                case ERotateRule.Local:
                    {
                        info.transform.localRotation = info.localRotation;
                        info.transform.Rotate(Vector3.Lerp(Vector3.zero, value, percent.percent01OfWorkCurve), Space.Self);
                        break;
                    }
                case ERotateRule.World:
                    {
                        info.transform.rotation = info.worldRotation;
                        info.transform.Rotate(Vector3.Lerp(Vector3.zero, value, percent.percent01OfWorkCurve), Space.World);
                        break;
                    }
                case ERotateRule.LookAt:
                    {
                        info.transform.localRotation = Quaternion.Lerp(info.localRotation, Quaternion.LookRotation(value, upwards), percent.percent01OfWorkCurve);
                        break;
                    }
                case ERotateRule.LocalAxis:
                    {
                        info.transform.localRotation = info.localRotation;
                        info.transform.Rotate(value, Mathf.Lerp(0, axisAngle, percent.percent01OfWorkCurve), Space.Self);
                        break;
                    }
                case ERotateRule.WorldAxis:
                    {
                        info.transform.rotation = info.worldRotation;
                        info.transform.Rotate(value, Mathf.Lerp(0, axisAngle, percent.percent01OfWorkCurve), Space.World);
                        break;
                    }
                case ERotateRule.WorldPointAxis:
                    {
                        var currentAngle = Mathf.Lerp(0, axisAngle, percent.percent01OfWorkCurve);
                        info.transform.RotateAround(axisPoint, value, currentAngle - lastAxisAngle);
                        lastAxisAngle = currentAngle;
                        if (lookatAxisPoint)
                        {
                            info.transform.LookAt(axisPoint);
                        }
                        break;
                    }
                case ERotateRule.WorldPointAxisThenLocal:
                    {
                        info.transform.rotation = info.worldRotation;
                        info.transform.RotateAround(axisPoint, value, axisAngle);
                        info.transform.localRotation = info.localRotation;
                        break;
                    }
            }
        }
    }
}
