using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.Extension.Cameras
{
    [DisallowMultipleComponent]
    [Name("平移绕物相机")]
    [Tip("支持绕物同时能够平移，当重置相机后， 相机又回到最初的绕物目标上；")]
    [XCSJ.Attributes.Icon(index = 8323)]
    [Tool(CameraHelper.LegacyCameras, nameof(CameraManager))]
    [Obsolete("产品功能升级，不推荐用户再使用本功能组件，请使用新的功能组件替代！", false)]
    public class MoveAroundCamera : SmartCamera
    {
        /// <summary>
        /// 移动相机到目标的时间
        /// </summary>
        [Name("移动相机到目标的时间")]
        [Range(0, 10)]
        public float moveToTargetTime = 1.0f;

        [Name("视平面移动速度")]
        [Tip("该速度值只影响水平和垂直的移动，不影响纵深的移动速度。")]
        //[Range(0, 1000)]
        public InputInfo xyMoveSpeed = new InputInfo();

        /// <summary>
        /// 最后一次设置绕物的视角
        /// </summary>
        protected Transform lastLookAt;

        // 注视点到目标物体的偏移量
        protected Vector3 offsetToTarget = Vector3.zero;

        public override void Reset()
        {
            base.Reset();

            xyMoveSpeed.Reset(1f);
            xyMoveSpeed.Init(EInputType.Touch, RuntimePlatform.IPhonePlayer, 1.0f);
            xyMoveSpeed.Init(EInputType.Touch, RuntimePlatform.Android, 1.0f);
            xyMoveSpeed.Init(EInputType.Touch, RuntimePlatform.WebGLPlayer, 1.0f);
            xyMoveSpeed.Init(EInputType.Touch, RuntimePlatform.WindowsPlayer, 1.0f);
            xyMoveSpeed.Init(EInputType.Mouse, RuntimePlatform.WindowsEditor, 1.0f);
            xyMoveSpeed.Init(EInputType.Mouse, RuntimePlatform.WindowsPlayer, 1.0f);
        }

        protected GameObject tmpTarget = null;

        protected void Init()
        {
            if (Application.isPlaying && !tmpTarget)
            {
                tmpTarget = GameObject.Find("#平移绕物相机目标点");
                if (!tmpTarget)
                {
                    tmpTarget = new GameObject("#平移绕物相机目标点");
                }
                lastLookAt = tmpTarget.transform;
                if (target)
                {
                    lastLookAt.position = target.position;
                }
            }
        }

        public void OnDestroy()
        {
            if (tmpTarget) Destroy(tmpTarget);
        }

        public override void ResetCamera(float duration, string fun, string param)
        {
            base.ResetCamera(duration, fun, param);

            offsetToTarget = Vector3.zero;
        }

        private float lockTimeCounter = 0;
        public bool moving { get; private set; } = false;
        private bool lastLockState = false;

        public override void Update()
        {
            base.Update();

            Init();

            if (moving)
            {
                lockTimeCounter += Time.deltaTime;
                if (lockTimeCounter > moveToTargetTime)
                {
                    lockTimeCounter = 0;
                    UnLock();
                }
            }
        }

        /// <summary>
        /// 设置查看视角和查看目标
        /// </summary>
        /// <param name="lookat">查看视角</param>
        /// <param name="target">查看目标</param>
        public void SetLookAtAndTarget(GameObject lookat, GameObject target)
        {
            if (!lookat || !target) return;

            if (lastLookAt)
            {
                this.lastLookAt.position = lookat.transform.position;
                this.lastLookAt.rotation = lookat.transform.rotation;
            }
            else
            {
                lastLookAt = lookat.transform;
            }

            if (moving)
            {
                UnLock();
            }
            moving = true;
            lastLockState = lockCamera;
            lockCamera = true;

            SetTarget(target);                        
            MoveToTarget(lookat, moveToTargetTime);
        }

        private void UnLock()
        {
            lockCamera = lastLockState;
            moving = false;
        }

        public void ResetLast()
        {
            if (!this.lastLookAt) return;

            SetLookAtAndTarget(this.lastLookAt.gameObject, target.gameObject);
        }

        public override void SetTarget(GameObject target)
        {
            base.SetTarget(target);

            offsetToTarget = Vector3.zero;
        }

        public override void CameraUpdate()
        {
            if (!target || lockCamera) return;

            base.CameraUpdate();

            xyMoveSpeed.Update(Time.deltaTime);

            float currentSpeed = -1;

            switch (XInput.touchCount)
            {
                case 0:
                    {
                        if (!ControlValid(EControlMode.Mouse)) break;

                        // 中键
                        if (XInput.GetMouseButton(2))
                        {
                            currentSpeed = xyMoveSpeed.GetRealtimeValue(EInputType.Mouse);
                        }
                        // 任意键-检测键盘
                        else if (XInput.anyKeyDown)
                        {
                            xyMoveSpeed.GetRealtimeValue(EInputType.Keyboard);
                        }
                        break;
                    }
                case 2:
                    {
                        if (!ControlValid(EControlMode.Touch)) break;

                        currentSpeed = xyMoveSpeed.GetRealtimeValue(EInputType.Touch);
                        break;
                    }
            }

            if (currentSpeed>0)
            {
                Vector3 tmpOffset = transform.right * moveVector.x + transform.up * moveVector.y;
                offsetToTarget += tmpOffset * currentSpeed;
                if (updateTargetPosition)
                {
                    aroundTargetPosition += tmpOffset * currentSpeed;
                }

                moveVector.x *= currentSpeed;
                moveVector.y *= currentSpeed;
            }
        }

        public override void CameraLateUpdate()
        {
            if (target == null || lockCamera) return;

            if (updateTargetPosition && offsetToTarget == Vector3.zero)
            {
                aroundTargetPosition = target.position;
            }

            distance = Vector3.Distance(transform.position, aroundTargetPosition);
            distance = distance - moveVector.z;
            //if (userDistancLimitRange) distance = Mathf.Clamp(distance, distancLimitRange.x, distancLimitRange.y);
            bool isUp = Vector3.Dot(transform.up, Vector3.up) >= 0;

            if (needDamping)
            {
                if (targetRotateVector != Vector3.zero)
                {
                    tmpTargetRotateVector = targetRotateVector;
                }
                tmpTargetRotateVector = Vector3.Lerp(tmpTargetRotateVector, Vector3.zero, realtimeDampingCoefficient);
                Vector3 v3 = ClampRotateVector(tmpTargetRotateVector);
                transform.RotateAround(aroundTargetPosition, (isUp ? Vector3.up : -Vector3.up), v3.y);
                transform.RotateAround(aroundTargetPosition, transform.right, v3.x);

                //相机移动平滑
                if (moveVector != Vector3.zero)
                {
                    tmpMoveVector = moveVector;
                }
                tmpMoveVector = Vector3.Lerp(tmpMoveVector, Vector3.zero, realtimeDampingCoefficient);
                if (userDistancLimitRange == false || (distance >= distancLimitRange.x && distance <= distancLimitRange.y))
                {
                    transform.Translate(tmpMoveVector);
                }
            }
            else
            {
                //限定旋转角度
                targetRotateVector = ClampRotateVector(targetRotateVector);
                transform.RotateAround(aroundTargetPosition, (isUp ? Vector3.up : -Vector3.up), targetRotateVector.y);
                transform.RotateAround(aroundTargetPosition, transform.right, targetRotateVector.x);

                //相机移动
                if (userDistancLimitRange == false || (distance >= distancLimitRange.x && distance <= distancLimitRange.y))
                {
                    transform.Translate(moveVector);
                }
            }

            moveVector = Vector3.zero;
            rotateVector = Vector3.zero;
            targetRotateVector = Vector3.zero;
        }
    }
}
