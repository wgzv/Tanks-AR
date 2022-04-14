using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 绕物相机运动:绕物相机运动组件是相机的旋转动画。在设定的时间区间内，相机对目标物体做环绕旋转运动，在播放完毕后，组件切换为完成态。
    /// </summary>
    [Serializable]
    [ComponentMenu("动作/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(AroundCameraMotion))]
    [Tip("绕物相机运动组件是相机的旋转动画。在设定的时间区间内，相机对目标物体做环绕旋转运动，在播放完毕后，组件切换为完成态。")]
    [Obsolete("产品功能升级，不推荐用户再使用本功能组件，请使用新的功能组件替代！", false)]
    [XCSJ.Attributes.Icon(index = 33614)]
    public class AroundCameraMotion : StateComponent<AroundCameraMotion>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "绕物相机运动";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(AroundCameraMotion))]
        [Tip("绕物相机运动组件是相机的旋转动画。在设定的时间区间内，相机对目标物体做环绕旋转运动，在播放完毕后，组件切换为完成态。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Obsolete("产品功能升级，不推荐用户再使用本功能组件，请使用新的功能组件替代！", false)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("绕物相机")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public AroundCamera aroundCamera;

        //[Name("旋转类型")]
        //public ERotateType rotateType;

        [Name("旋转时间")]
        [Range(0.1f, 100)]
        public float duration = 20;

        [Name("运动值类型")]
        [EnumPopup]
        public EValueType valueType = EValueType.Add;

        [Name("是否沿X轴旋转")]
        public bool xRotate = false;

        [Name("旋转X角度")]
        [Range(-360, 360)]
        public float rotateXAngle = 180;

        [Name("是否沿Y轴旋转")]
        public bool yRotate = false;

        [Name("旋转Y角度")]
        [Range(-360, 360)]
        public float rotateYAngle = 180;

        [Name("是否设置距离")]
        public bool setDistance = false;

        [Name("目标距离")]
        public float targetDistance = 0;

        [Name("是否重置相机")]
        public bool resetCamera = false;

        private float speedX = 0;

        private float speedY = 0;

        private float tempRotate = 0;

        private bool needDamping;

        private bool lockCamera;

        private float initDiatance;

        private float disSpeed = 0;

        public override bool Init(StateData data)
        {
            return base.Init(data);
        }

        public override void OnEntry(StateData data)
        {
            tempRotate = 0;
            if (aroundCamera)
            {
                needDamping = aroundCamera.needDamping;
                aroundCamera.needDamping = false;
                lockCamera = aroundCamera.lockCamera;
                aroundCamera.lockCamera = true;

                if (resetCamera)
                    aroundCamera.ResetCameraWithTween(0);

                if (xRotate)
                {
                    switch (valueType)
                    {
                        case EValueType.Add:
                            speedX = rotateXAngle / duration;
                            break;
                        case EValueType.To:
                            //Debug.Log(aroundCamera.transform.localEulerAngles.x);
                            if (rotateXAngle - aroundCamera.transform.localEulerAngles.x > 180)
                                speedX = (360 - rotateXAngle + aroundCamera.transform.localEulerAngles.x) / duration;
                            else if (rotateXAngle - aroundCamera.transform.localEulerAngles.x < -180)
                                speedX = (rotateXAngle - aroundCamera.transform.localEulerAngles.x + 360) / duration;
                            else
                                speedX = (rotateXAngle - aroundCamera.transform.localEulerAngles.x) / duration;
                            break;
                        case EValueType.From:
                            Vector3 initAngle = aroundCamera.transform.localEulerAngles;
                            initAngle.x = rotateXAngle;
                            aroundCamera.transform.localRotation = Quaternion.Euler(initAngle);
                            speedX = (aroundCamera.transform.localEulerAngles.x - rotateXAngle) / duration;
                            break;
                    }
                }

                if (yRotate)
                {
                    switch (valueType)
                    {
                        case EValueType.Add:
                            speedY = rotateYAngle / duration;
                            break;
                        case EValueType.To:
                            speedY = (rotateYAngle - aroundCamera.transform.localEulerAngles.y) / duration;
                            break;
                        case EValueType.From:
                            Vector3 initAngle = aroundCamera.transform.localEulerAngles;
                            initAngle.y = rotateYAngle;
                            aroundCamera.transform.localRotation = Quaternion.Euler(initAngle);
                            speedY = (aroundCamera.transform.localEulerAngles.y - rotateYAngle) / duration;
                            break;
                    }
                }

                if (setDistance)
                {
                    switch (valueType)
                    {
                        case EValueType.Add:
                            disSpeed = targetDistance / duration;
                            break;
                        case EValueType.To:
                            initDiatance = aroundCamera.distance;
                            disSpeed = (targetDistance - initDiatance) / duration;
                            break;
                        case EValueType.From:
                            initDiatance = aroundCamera.distance;
                            aroundCamera.distance = targetDistance;
                            disSpeed = (initDiatance - targetDistance) / duration;
                            break;
                    }
                }

            }
            base.OnEntry(data);
        }

        public override void OnUpdate(StateData data)
        {
            if (!aroundCamera) return;
            if (tempRotate >= duration)
            {
                finished = true;

                return;
            }

            if (valueType != EValueType.Add && Math.Abs(rotateXAngle - aroundCamera.transform.localEulerAngles.x) < Math.Abs(speedX * Time.deltaTime))
            {
                if (valueType == EValueType.From)
                    speedX = aroundCamera.transform.localEulerAngles.x - rotateXAngle;
                else if (valueType == EValueType.To)
                    speedX = rotateXAngle - aroundCamera.transform.localEulerAngles.x;
            }

            if (valueType != EValueType.Add && Math.Abs(rotateYAngle - aroundCamera.transform.localEulerAngles.y) < Math.Abs(speedY * Time.deltaTime))
            {
                if (valueType == EValueType.From)
                    speedY = aroundCamera.transform.localEulerAngles.y - rotateYAngle;
                else
                    speedY = rotateYAngle - aroundCamera.transform.localEulerAngles.y;
            }
            aroundCamera.targetRotateVector = new Vector3(speedX * Time.deltaTime, speedY * Time.deltaTime, 0);

            if (setDistance)
            {
                if (valueType != EValueType.Add && Math.Abs(aroundCamera.distance - targetDistance) < Math.Abs(disSpeed * Time.deltaTime))
                    aroundCamera.distance = targetDistance;
                else
                    aroundCamera.distance += disSpeed * Time.deltaTime;
            }
            tempRotate += Time.deltaTime;
            SetPercent(tempRotate / duration);
        }

        public override void OnExit(StateData data)
        {
            tempRotate = 0;
            if (aroundCamera)
            {
                aroundCamera.targetRotateVector = Vector3.zero;
                aroundCamera.needDamping = needDamping;
                aroundCamera.lockCamera = lockCamera;
            }
            base.OnExit(data);
        }

        protected float _progress = 0;

        public override float progress
        {
            get { return Mathf.Clamp01(_progress); }
            set { SetPercent(value); }
        }

        public bool SetPercent(float percent)
        {
            _progress = percent;
            return true;
        }

        public enum ERotateType
        {
            [Name("X轴")]
            XAxis,
            [Name("Y轴")]
            YAxis,
            [Name("X-Y轴")]
            XYAxis,
        }

        public enum EValueType
        {
            [Name("增加值")]
            Add,
            [Name("运动到某值")]
            To,
            [Name("从某值移动到当前值")]
            From,
        }
    }
}
