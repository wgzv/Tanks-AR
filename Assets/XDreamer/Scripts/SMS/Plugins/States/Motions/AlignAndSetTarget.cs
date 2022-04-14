using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Interfaces;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 对齐并设置目标
    /// </summary>
    [ComponentMenu("动作/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(AlignAndSetTarget))]
    [XCSJ.Attributes.Icon(EIcon.Target)]
    public class AlignAndSetTarget : TransformMotion<AlignAndSetTarget>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "对齐并设置目标";

        /// <summary>
        /// 创建状态
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [Name(Title, nameof(AlignAndSetTarget))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateAlignAndSetTarget(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 对齐对象类型
        /// </summary>
        [Name("对齐对象类型")]
        public enum EAlignObjectType
        {
            /// <summary>
            /// 变换
            /// </summary>
            [Name("变换")]
            Transform = 0,

            /// <summary>
            /// 三维向量
            /// </summary>
            [Name("三维向量")]
            Vector3,

            /// <summary>
            /// 变量
            /// </summary>
            [Name("变量")]
            Variable,
        }

        /// <summary>
        /// 对齐对象类型
        /// </summary>
        [Name("对齐对象类型")]
        [EnumPopup]
        public EAlignObjectType _alignObjectType = EAlignObjectType.Transform;

        /// <summary>
        /// 对齐规则
        /// </summary>
        public enum EAlignRule
        {
            /// <summary>
            /// 位置
            /// </summary>
            [Name("位置")]
            Positin,

            /// <summary>
            /// 角度
            /// </summary>
            [Name("角度")]
            Angle,

            /// <summary>
            /// 位置与角度
            /// </summary>
            [Name("位置与角度")]
            PositionAndAngle,
        }

        /// <summary>
        /// 对齐规则
        /// </summary>
        [Name("对齐规则")]
        [EnumPopup]
        public EAlignRule _alignRule = EAlignRule.PositionAndAngle;

        /// <summary>
        /// 对齐对象:将游戏对象集中游戏对象移动到本参数变换对象的世界坐标位置和旋转到本参数变换对象的世界旋转欧拉角度
        /// </summary>
        [Name("对齐对象")]
        [Tip("将游戏对象集中游戏对象移动到本参数变换对象的世界坐标位置和旋转到本参数变换对象的世界旋转欧拉角度")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_alignObjectType), EValidityCheckType.NotEqual, EAlignObjectType.Transform)]
        public Transform _alignTransform;

        /// <summary>
        /// 位置:将游戏对象集中游戏对象移动到本参数指向的世界坐标位置
        /// </summary>
        [Name("位置")]
        [Tip("将游戏对象集中游戏对象移动到本参数指向的世界坐标位置")]
        [HideInSuperInspector(nameof(_alignObjectType), EValidityCheckType.NotEqual | EValidityCheckType.Or, EAlignObjectType.Vector3,
            nameof(_alignRule), EValidityCheckType.Equal, EAlignRule.Angle)]
        public Vector3 _position = new Vector3();

        /// <summary>
        /// 欧拉角:将游戏对象集中游戏对象旋转到本参数指向的世界旋转欧拉角度
        /// </summary>
        [Name("欧拉角")]
        [Tip("将游戏对象集中游戏对象旋转到本参数指向的世界旋转欧拉角度")]
        [HideInSuperInspector(nameof(_alignObjectType), EValidityCheckType.NotEqual | EValidityCheckType.Or, EAlignObjectType.Vector3,
            nameof(_alignRule), EValidityCheckType.Equal, EAlignRule.Positin)]
        public Vector3 _eulerAngles = new Vector3();

        /// <summary>
        /// 位置变量:将游戏对象集中游戏对象移动到本参数指向的世界坐标位置
        /// </summary>
        [Name("位置变量")]
        [Tip("将游戏对象集中游戏对象移动到本参数指向的世界坐标位置")]
        [HideInSuperInspector(nameof(_alignObjectType), EValidityCheckType.NotEqual | EValidityCheckType.Or, EAlignObjectType.Variable,
            nameof(_alignRule), EValidityCheckType.Equal, EAlignRule.Angle)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [GlobalVariable]
        public string _positionVariable = "";

        /// <summary>
        /// 角度变量:将游戏对象集中游戏对象旋转到本参数指向的世界旋转欧拉角度
        /// </summary>
        [Name("角度变量")]
        [Tip("将游戏对象集中游戏对象旋转到本参数指向的世界旋转欧拉角度")]
        [HideInSuperInspector(nameof(_alignObjectType), EValidityCheckType.NotEqual | EValidityCheckType.Or, EAlignObjectType.Variable,
            nameof(_alignRule), EValidityCheckType.Equal, EAlignRule.Positin)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [GlobalVariable]
        public string _angleVariable = "";

        /// <summary>
        /// 目标
        /// </summary>
        [Name("目标对象")]
        public Transform _target;

        /// <summary>
        /// 对齐位置
        /// </summary>
        public Vector3 alignPosition
        {
            get
            {
                switch (_alignObjectType)
                {
                    case EAlignObjectType.Vector3: return _position;
                    case EAlignObjectType.Variable: return CommonFun.StringToVector3(ScriptManager.GetGlobalVariableValue(_positionVariable));
                    default: return _alignTransform.position;
                }
            }
        }

        /// <summary>
        /// 对齐旋转量
        /// </summary>
        public Quaternion alignRotation
        {
            get
            {
                switch (_alignObjectType)
                {
                    case EAlignObjectType.Vector3: return Quaternion.Euler(_eulerAngles);
                    case EAlignObjectType.Variable: return Quaternion.Euler(CommonFun.StringToVector3(ScriptManager.GetGlobalVariableValue(_angleVariable)));
                    default: return _alignTransform.rotation;
                }
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return _alignTransform && base.DataValidity();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            gameObjectSet.objects.ForEach(obj =>
            {
                if (obj.GetComponent<ITarget>() is ITarget target)
                {
                    _objectsOfSetTarget.Add(target);
                }
            });
            return base.Init(data);
        }

        private List<ITarget> _objectsOfSetTarget = new List<ITarget>();

        /// <summary>
        /// 百分比
        /// </summary>
        /// <param name="info"></param>
        /// <param name="percent"></param>
        protected override void SetPercent(TransformRecorder.Info info, Percent percent)
        {
            switch (_alignRule)
            {
                case EAlignRule.Positin:
                    {
                        info.transform.position = Vector3.Lerp(info.worldPosition, alignPosition, percent.percent01OfWorkCurve);
                        break;
                    }
                case EAlignRule.Angle:
                    {
                        info.transform.rotation = Quaternion.Lerp(info.worldRotation, alignRotation, percent.percent01OfWorkCurve);
                        break;
                    }
                case EAlignRule.PositionAndAngle:
                    {
                        info.transform.position = Vector3.Lerp(info.worldPosition, alignPosition, percent.percent01OfWorkCurve);
                        info.transform.rotation = Quaternion.Lerp(info.worldRotation, alignRotation, percent.percent01OfWorkCurve);
                        break;
                    }
            }

            _objectsOfSetTarget.ForEach(obj => obj.target = _target);
        }
    }
}
