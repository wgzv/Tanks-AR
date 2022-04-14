using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.CNScripts;
using XCSJ.Extension.CNScripts.UGUI;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginsCameras.Legacies.CNScripts
{
    /// <summary>
    /// UGUI操纵杆按钮(相机专用)脚本事件
    /// </summary>
    [Serializable]
    [RequireComponent(typeof(Button))]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.UGUIMenu + Title)]
    [Obsolete("产品功能升级，不推荐用户再使用本功能组件，请使用新的功能组件替代！", false)]
    public class UGUIJoystickButtonForCameraScriptEvent : UGUIJoystickButtonScriptEvent
    {
        /// <summary>
        /// 标题
        /// </summary>
        public new const string Title = "UGUI操纵杆按钮(相机专用)脚本事件";

        /// <summary>
        /// 控制当前相机:即控制相机管理类中处于当前状态的相机
        /// </summary>
        [Name("控制当前相机")]
        [Tip("即控制相机管理类中处于当前状态的相机")]
        public bool controlCurrentCamera = true;

        /// <summary>
        /// 目标相机:如果不使用相机管理类中处于当前状态的相机时，会尝试使用本参数指定的相机；
        /// </summary>
        [Name("目标相机")]
        [Tip("如果不使用相机管理类中处于当前状态的相机时，会尝试使用本参数指定的相机；")]
        [HideInSuperInspector("controlCurrentCamera", EValidityCheckType.True)]
        public BaseCamera targetCamera = null;

        /// <summary>
        /// 相机控制类型:当前控件控制相机的移动或旋转的类型
        /// </summary>
        [Name("相机控制类型")]
        [Tip("当前控件控制相机的移动或旋转的类型")]
        [EnumPopup]
        public ECameraControlType cameraCameraType = ECameraControlType.XZMove;

        /// <summary>
        /// 缩放系数:运行时会将本系数的x,y与控件移动偏移量的x,y分别相乘，并使用得出值对相机进行控制；其中z值保留不使用；
        /// </summary>
        [Name("缩放系数")]
        [Tip("运行时会将本系数的x,y与控件移动偏移量的x,y分别相乘，并使用得出值对相机进行控制；其中z值保留不使用；")]
        public Vector3 scaleValue = new Vector3(1, 1, 1);

        /// <summary>
        /// 当前相机:运行态时本组件正在控制的相机
        /// </summary>
        [Name("当前相机")]
        [Tip("运行态时本组件正在控制的相机")]
        [Readonly(false)]
        public BaseCamera currentCamera = null;

        /// <summary>
        /// 临时锁定相机
        /// </summary>
        public bool tmpLockCamera { get; private set; }

        /// <summary>
        /// 相机控制枚举
        /// </summary>
        public enum ECameraControlType
        {
            /// <summary>
            /// XZ移动:即控制相机的前后左右移动,基于相机自身坐标系;控制关系为(x,0,y),其中xy表示当前控件拖拽偏移量坐标乘以指定缩放系数后的向量值后的xy；
            /// </summary>
            [Name("XZ移动")]
            [Tip("即控制相机的前后左右移动,基于相机自身坐标系;控制关系为(x,0,y),其中xy表示当前控件拖拽偏移量坐标乘以指定缩放系数后的向量值后的xy；")]
            XZMove = 0,

            /// <summary>
            /// XY移动:即控制相机的上下左右移动,基于相机自身坐标系;控制关系为(x,y,0),其中xy表示当前控件拖拽偏移量坐标乘以指定缩放系数后的向量值后的xy；
            /// </summary>
            [Name("XY移动")]
            [Tip("即控制相机的上下左右移动,基于相机自身坐标系;控制关系为(x,y,0),其中xy表示当前控件拖拽偏移量坐标乘以指定缩放系数后的向量值后的xy；")]
            XYMove,

            /// <summary>
            /// XY旋转:即控制相机的绕自身轴心的X轴旋转,绕标准Y轴（正方向或负方向）旋转;控制关系为(-y,x,0),其中xy表示当前控件拖拽偏移量坐标乘以指定缩放系数后的得出向量值的xy；
            /// </summary>
            [Name("XY旋转")]
            [Tip("即控制相机的绕自身轴心的X轴旋转,绕标准Y轴（正方向或负方向）旋转;控制关系为(-y,x,0),其中xy表示当前控件拖拽偏移量坐标乘以指定缩放系数后的得出向量值的xy；")]
            XYRotate,

            /// <summary>
            /// 目标XY旋转:即控制相机的绕目标对象轴心的XY轴旋转;控制关系为(-y,x,0),其中xy表示当前控件拖拽偏移量坐标乘以指定缩放系数后的向量值后的xy；
            /// </summary>
            [Name("目标XY旋转")]
            [Tip("即控制相机的绕目标对象轴心的XY轴旋转;控制关系为(-y,x,0),其中xy表示当前控件拖拽偏移量坐标乘以指定缩放系数后的向量值后的xy；")]
            TargetXYRotate,
        }

        /// <summary>
        /// 启动
        /// </summary>
        public override void Start()
        {
            base.Start();
            RecaptureCurrentCamera();
        }

        /// <summary>
        /// 启用时
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            LegacyCameraManagerProvider.onChangedCurrentCamera += this.OnChangeCurrentCamera;
            RecaptureCurrentCamera();
        }

        /// <summary>
        /// 禁用时
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            LegacyCameraManagerProvider.onChangedCurrentCamera -= this.OnChangeCurrentCamera;
        }

        /// <summary>
        /// 当切换当前相机时
        /// </summary>
        public void OnChangeCurrentCamera()
        {
            RecaptureCurrentCamera();
        }

        /// <summary>
        /// 重新捕获当前相机
        /// </summary>
        public void RecaptureCurrentCamera()
        {
            currentCamera = null;
            if (controlCurrentCamera)
            {
                if (CameraManager.instance) currentCamera = CameraManager.instance.GetCurrentCamera();
            }
            else currentCamera = targetCamera;
        }

        /// <summary>
        /// 当拖拽中时
        /// </summary>
        /// <param name="v3"></param>
        public override void OnDrag(Vector3 v3)
        {
            base.OnDrag(v3);
            if (!currentCamera)
            {
                Debug.LogWarning("当前相机无效！");
                //当前相机无效时，重新捕获当前相机
                RecaptureCurrentCamera();
                return;
            }
            v3 /= movementRange;
            v3.Scale(scaleValue);
            switch (cameraCameraType)
            {
                case ECameraControlType.XZMove:
                    {
                        currentCamera.Move(new Vector3(v3.x, 0, v3.y));
                        break;
                    }
                case ECameraControlType.XYMove:
                    {
                        currentCamera.Move(new Vector3(v3.x, v3.y, 0));
                        break;
                    }
                case ECameraControlType.XYRotate:
                    {
                        currentCamera.Rotate(new Vector3(-v3.y, v3.x, 0));
                        break;
                    }
                case ECameraControlType.TargetXYRotate:
                    {
                        currentCamera.TargetRotate(new Vector3(-v3.y, v3.x, 0));
                        break;
                    }
            }
        }

        /// <summary>
        /// 当指针按下时
        /// </summary>
        /// <param name="data"></param>
        public override void OnPointerDown(PointerEventData data)
        {
            if (currentCamera)
            {
                tmpLockCamera = currentCamera.lockCamera;
                currentCamera.lockCamera = true;
            }
            base.OnPointerDown(data);
        }

        /// <summary>
        /// 当指针抬起时
        /// </summary>
        /// <param name="data"></param>
        public override void OnPointerUp(PointerEventData data)
        {
            if (currentCamera)
            {
                currentCamera.lockCamera = tmpLockCamera;
            }
            else RecaptureCurrentCamera();
            base.OnPointerUp(data);
        }
    }
}
