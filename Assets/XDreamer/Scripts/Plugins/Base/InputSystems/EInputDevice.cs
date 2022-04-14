using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Algorithms;
using System;
using System.Collections.Generic;

#if ENABLE_INPUT_SYSTEM

using Unity.XR.GoogleVr;
using Unity.XR.Oculus.Input;
using Unity.XR.OpenVR;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.WindowsMR.Input;

#endif

namespace XCSJ.Extension.Base.InputSystems
{
    /// <summary>
    /// 输入设备
    /// </summary>
    public enum EInputDevice
    {
        /// <summary>
        /// 输入设备:用于表示任意类型的输入设备<see cref="UnityEngine.InputSystem.InputDevice"/>
        /// </summary>
        [Name("输入设备")]
        [EnumFieldName("输入设备")]
        [Tip("用于表示任意类型的输入设备")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(InputDevice))]
#endif
        InputDevice = -1,

        /// <summary>
        /// 无:不做任何处理
        /// </summary>
        [Name("无")]
        [EnumFieldName("无")]
        [Tip("不做任何处理")]
        None,

        #region 游戏手柄 1

        /// <summary>
        /// 游戏手柄<see cref="UnityEngine.InputSystem.Gamepad"/>
        /// </summary>
        [Name("游戏手柄")]
        [EnumFieldName("游戏手柄")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(Gamepad))]
#endif
        Gamepad = 1,

        /// <summary>
        /// iOS游戏控制器<see cref="UnityEngine.InputSystem.iOS.iOSGameController"/>
        /// </summary>
        [Name("iOS游戏控制器")]
        [EnumFieldName("游戏手柄/iOS游戏控制器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.iOS.iOSGameController")]
#endif
        iOSGameController = Gamepad * InputSystemHelper.Range + 0,

        /// <summary>
        /// X输入控制器<see cref="UnityEngine.InputSystem.XInput.XInputController"/>
        /// </summary>
        [Name("X输入控制器")]
        [EnumFieldName("游戏手柄/X输入控制器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.XInput.XInputController")]
#endif
        XInputController = Gamepad * InputSystemHelper.Range + 1,

        #region XInputController子类

        /// <summary>
        /// XboxOne游戏手柄iOS<see cref="UnityEngine.InputSystem.iOS.XboxOneGampadiOS"/>
        /// </summary>
        [Name("XboxOne游戏手柄iOS")]
        [EnumFieldName("游戏手柄/XboxOne游戏手柄iOS")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.iOS.XboxOneGampadiOS")]
#endif
        XboxOneGampadiOS = XInputController * InputSystemHelper.Range + 0,

        /// <summary>
        /// X输入控制器Windows
        /// </summary>
        [Name("X输入控制器Windows")]
        [EnumFieldName("游戏手柄/X输入控制器Windows")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.XInput.XInputControllerWindows")]
#endif
        XInputControllerWindows = XInputController * InputSystemHelper.Range + 1,

        #endregion

        /// <summary>
        /// WebGL游戏手柄<see cref="UnityEngine.InputSystem.WebGL.WebGLGamepad"/>
        /// </summary>
        [Name("WebGL游戏手柄")]
        [EnumFieldName("游戏手柄/WebGL游戏手柄")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.WebGL.WebGLGamepad")]
#endif
        WebGLGamepad = Gamepad * InputSystemHelper.Range + 2,

        /// <summary>
        /// 任天堂Switch控制器HID<see cref="UnityEngine.InputSystem.Switch.SwitchProControllerHID"/>
        /// </summary>
        [Name("任天堂Switch控制器HID")]
        [EnumFieldName("游戏手柄/任天堂Switch控制器HID")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.Switch.SwitchProControllerHID")]
#endif
        SwitchProControllerHID = Gamepad * InputSystemHelper.Range + 3,

        /// <summary>
        /// 索尼DualShock游戏手柄<see cref="UnityEngine.InputSystem.DualShock.DualShockGamepad"/>
        /// </summary>
        [Name("索尼DualShock游戏手柄")]
        [EnumFieldName("游戏手柄/索尼DualShock游戏手柄")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.DualShock.DualShockGamepad")]
#endif
        DualShockGamepad = Gamepad * InputSystemHelper.Range + 4,

        #region DualShockGamepad子类

        /// <summary>
        /// 索尼DualShock4游戏手柄iOS<see cref="UnityEngine.InputSystem.iOS.DualShock4GampadiOS"/>
        /// </summary>
        [Name("索尼DualShock4游戏手柄iOS")]
        [EnumFieldName("游戏手柄/索尼DualShock4游戏手柄iOS")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.iOS.DualShock4GampadiOS")]
#endif
        DualShock4GampadiOS = DualShockGamepad * InputSystemHelper.Range + 0,

        /// <summary>
        /// 索尼DualShock4游戏手柄HID<see cref="UnityEngine.InputSystem.DualShock.DualShock4GamepadHID"/>
        /// </summary>
        [Name("索尼DualShock4游戏手柄HID")]
        [EnumFieldName("游戏手柄/索尼DualShock4游戏手柄HID")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.DualShock.DualShock4GamepadHID")]
#endif
        DualShock4GamepadHID = DualShockGamepad * InputSystemHelper.Range + 1,

        /// <summary>
        /// 索尼DualShock3游戏手柄HID<see cref="UnityEngine.InputSystem.DualShock.DualShock3GamepadHID"/>
        /// </summary>
        [Name("索尼DualShock3游戏手柄HID")]
        [EnumFieldName("游戏手柄/索尼DualShock3游戏手柄HID")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.DualShock.DualShock3GamepadHID")]
#endif
        DualShock3GamepadHID = DualShockGamepad * InputSystemHelper.Range + 2,

        #endregion

        /// <summary>
        /// Android游戏手柄<see cref="UnityEngine.InputSystem.Android.AndroidGamepad"/>
        /// </summary>
        [Name("Android游戏手柄")]
        [EnumFieldName("游戏手柄/Android游戏手柄")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.Android.AndroidGamepad")]
#endif
        AndroidGamepad = Gamepad * InputSystemHelper.Range + 5,

        #endregion

        #region 摇杆 2

        /// <summary>
        /// 摇杆<see cref="UnityEngine.InputSystem.Joystick"/>
        /// </summary>
        [Name("摇杆")]
        [EnumFieldName("摇杆")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(Joystick))]
#endif
        Joystick = 2,

        /// <summary>
        /// WebGL摇杆<see cref="UnityEngine.InputSystem.WebGL.WebGLJoystick"/>
        /// </summary>
        [Name("WebGL摇杆")]
        [EnumFieldName("摇杆/WebGL摇杆")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.WebGL.WebGLJoystick")]
#endif
        WebGLJoystick = Joystick * InputSystemHelper.Range + 0,

        /// <summary>
        /// Android摇杆<see cref="UnityEngine.InputSystem.Android.AndroidJoystick"/>
        /// </summary>
        [Name("Android摇杆")]
        [EnumFieldName("摇杆/Android摇杆")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.Android.AndroidJoystick")]
#endif
        AndroidJoystick = Joystick * InputSystemHelper.Range + 1,

        #endregion

        #region 键盘 3

        /// <summary>
        /// 键盘<see cref="UnityEngine.InputSystem.Keyboard"/>
        /// </summary>
        [Name("键盘")]
        [EnumFieldName("键盘")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(Keyboard))]
#endif
        Keyboard = 3,

        #endregion

        #region 指针 4

        /// <summary>
        /// 指针<see cref="UnityEngine.InputSystem.Pointer"/>
        /// </summary>
        [Name("指针")]
        [EnumFieldName("指针")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(Pointer))]
#endif
        Pointer = 4,

        /// <summary>
        /// 鼠标<see cref="UnityEngine.InputSystem.Mouse"/>
        /// </summary>
        [Name("鼠标")]
        [EnumFieldName("指针/鼠标")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(Mouse))]
#endif
        Mouse = Pointer * InputSystemHelper.Range + 0,

        /// <summary>
        /// 笔<see cref="UnityEngine.InputSystem.Pen"/>
        /// </summary>
        [Name("笔")]
        [EnumFieldName("指针/笔")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(Pen))]
#endif
        Pen = Pointer * InputSystemHelper.Range + 1,

        /// <summary>
        /// 触摸屏<see cref="UnityEngine.InputSystem.Touchscreen"/>
        /// </summary>
        [Name("触摸屏")]
        [EnumFieldName("指针/触摸屏")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(Touchscreen))]
#endif
        Touchscreen = Pointer * InputSystemHelper.Range + 2,

        #endregion

        #region 传感器 5

        /// <summary>
        /// 传感器<see cref="UnityEngine.InputSystem.Sensor"/>
        /// </summary>
        [Name("传感器")]
        [EnumFieldName("传感器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(Sensor))]
#endif
        Sensor = 5,

        /// <summary>
        /// 加速度计<see cref="UnityEngine.InputSystem.Accelerometer"/>
        /// </summary>
        [Name("加速度计")]
        [EnumFieldName("传感器/加速度计")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(Accelerometer))]
#endif
        Accelerometer = Sensor * InputSystemHelper.Range + 0,

        /// <summary>
        /// Android加速度计<see cref="UnityEngine.InputSystem.Android.AndroidAccelerometer"/>
        /// </summary>
        [Name("Android加速度计")]
        [EnumFieldName("传感器/加速度计/Android加速度计")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.Android.AndroidAccelerometer")]
#endif
        AndroidAccelerometer = Accelerometer * InputSystemHelper.Range + 0,

        /// <summary>
        /// 陀螺仪<see cref="UnityEngine.InputSystem.Gyroscope"/>
        /// </summary>
        [Name("陀螺仪")]
        [EnumFieldName("传感器/陀螺仪")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(Gyroscope))]
#endif
        Gyroscope = Sensor * InputSystemHelper.Range + 1,

        /// <summary>
        /// Android陀螺仪<see cref="UnityEngine.InputSystem.Android.AndroidGyroscope"/>
        /// </summary>
        [Name("Android陀螺仪")]
        [EnumFieldName("传感器/陀螺仪/Android陀螺仪")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.Android.AndroidGyroscope")]
#endif
        AndroidGyroscope = Gyroscope * InputSystemHelper.Range + 0,

        /// <summary>
        /// 重力传感器<see cref="UnityEngine.InputSystem.GravitySensor"/>
        /// </summary>
        [Name("重力传感器")]
        [EnumFieldName("传感器/重力传感器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(GravitySensor))]
#endif
        GravitySensor = Sensor * InputSystemHelper.Range + 2,

        /// <summary>
        /// Android重力传感器<see cref="UnityEngine.InputSystem.Android.AndroidGravitySensor"/>
        /// </summary>
        [Name("Android重力传感器")]
        [EnumFieldName("传感器/重力传感器/Android重力传感器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.Android.AndroidGravitySensor")]
#endif
        AndroidGravitySensor = GravitySensor * InputSystemHelper.Range + 0,

        /// <summary>
        /// 姿态传感器<see cref="UnityEngine.InputSystem.AttitudeSensor"/>
        /// </summary>
        [Name("姿态传感器")]
        [EnumFieldName("传感器/姿态传感器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(AttitudeSensor))]
#endif
        AttitudeSensor = Sensor * InputSystemHelper.Range + 3,

        /// <summary>
        /// Android旋转矢量<see cref="UnityEngine.InputSystem.Android.AndroidRotationVector"/>
        /// </summary>
        [Name("Android旋转矢量")]
        [EnumFieldName("传感器/姿态传感器/Android旋转矢量")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.Android.AndroidRotationVector")]
#endif
        AndroidRotationVector = AttitudeSensor * InputSystemHelper.Range + 0,

        /// <summary>
        /// 线性加速度传感器<see cref="UnityEngine.InputSystem.LinearAccelerationSensor"/>
        /// </summary>
        [Name("线性加速度传感器")]
        [EnumFieldName("传感器/线性加速度传感器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(LinearAccelerationSensor))]
#endif
        LinearAccelerationSensor = Sensor * InputSystemHelper.Range + 4,

        /// <summary>
        /// Android线性加速度传感器<see cref="UnityEngine.InputSystem.Android.AndroidLinearAccelerationSensor"/>
        /// </summary>
        [Name("Android线性加速度传感器")]
        [EnumFieldName("传感器/线性加速度传感器/Android线性加速度传感器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.Android.AndroidLinearAccelerationSensor")]
#endif
        AndroidLinearAccelerationSensor = LinearAccelerationSensor * InputSystemHelper.Range + 0,

        /// <summary>
        /// 磁场传感器<see cref="UnityEngine.InputSystem.MagneticFieldSensor"/>
        /// </summary>
        [Name("磁场传感器")]
        [EnumFieldName("传感器/磁场传感器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(MagneticFieldSensor))]
#endif
        MagneticFieldSensor = Sensor * InputSystemHelper.Range + 5,

        /// <summary>
        /// Android磁场传感器<see cref="UnityEngine.InputSystem.Android.AndroidMagneticFieldSensor"/>
        /// </summary>
        [Name("Android磁场传感器")]
        [EnumFieldName("传感器/磁场传感器/Android磁场传感器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.Android.AndroidMagneticFieldSensor")]
#endif
        AndroidMagneticFieldSensor = MagneticFieldSensor * InputSystemHelper.Range + 0,

        /// <summary>
        /// 光传感器<see cref="UnityEngine.InputSystem.LightSensor"/>
        /// </summary>
        [Name("光传感器")]
        [EnumFieldName("传感器/光传感器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(LightSensor))]
#endif
        LightSensor = Sensor * InputSystemHelper.Range + 6,

        /// <summary>
        /// Android光传感器<see cref="UnityEngine.InputSystem.Android.AndroidLightSensor"/>
        /// </summary>
        [Name("Android光传感器")]
        [EnumFieldName("传感器/光传感器/Android光传感器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.Android.AndroidLightSensor")]
#endif
        AndroidLightSensor = LightSensor * InputSystemHelper.Range + 0,

        /// <summary>
        /// 压力传感器<see cref="UnityEngine.InputSystem.PressureSensor"/>
        /// </summary>
        [Name("压力传感器")]
        [EnumFieldName("传感器/压力传感器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(PressureSensor))]
#endif
        PressureSensor = Sensor * InputSystemHelper.Range + 7,

        /// <summary>
        /// Android压力传感器<see cref="UnityEngine.InputSystem.Android.AndroidPressureSensor"/>
        /// </summary>
        [Name("Android压力传感器")]
        [EnumFieldName("传感器/压力传感器/Android压力传感器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.Android.AndroidPressureSensor")]
#endif
        AndroidPressureSensor = PressureSensor * InputSystemHelper.Range + 0,

        /// <summary>
        /// 距离传感器<see cref="UnityEngine.InputSystem.ProximitySensor"/>
        /// </summary>
        [Name("距离传感器")]
        [EnumFieldName("传感器/距离传感器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(ProximitySensor))]
#endif
        ProximitySensor = Sensor * InputSystemHelper.Range + 8,

        /// <summary>
        /// Android距离传感器<see cref="UnityEngine.InputSystem.Android.AndroidProximity"/>
        /// </summary>
        [Name("Android距离传感器")]
        [EnumFieldName("传感器/距离传感器/Android距离传感器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.Android.AndroidProximity")]
#endif
        AndroidProximity = ProximitySensor * InputSystemHelper.Range + 0,

        /// <summary>
        /// 湿度传感器<see cref="UnityEngine.InputSystem.HumiditySensor"/>
        /// </summary>
        [Name("湿度传感器")]
        [EnumFieldName("传感器/湿度传感器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(HumiditySensor))]
#endif
        HumiditySensor = Sensor * InputSystemHelper.Range + 9,

        /// <summary>
        /// Android相对湿度<see cref="UnityEngine.InputSystem.Android.AndroidRelativeHumidity"/>
        /// </summary>
        [Name("Android相对湿度")]
        [EnumFieldName("传感器/湿度传感器/Android相对湿度")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.Android.AndroidRelativeHumidity")]
#endif
        AndroidRelativeHumidity = HumiditySensor * InputSystemHelper.Range + 0,

        /// <summary>
        /// 环境温度传感器<see cref="UnityEngine.InputSystem.AmbientTemperatureSensor"/>
        /// </summary>
        [Name("环境温度传感器")]
        [EnumFieldName("传感器/环境温度传感器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(AmbientTemperatureSensor))]
#endif
        AmbientTemperatureSensor = Sensor * InputSystemHelper.Range + 10,

        /// <summary>
        /// Android环境温度<see cref="UnityEngine.InputSystem.Android.AndroidAmbientTemperature"/>
        /// </summary>
        [Name("Android环境温度")]
        [EnumFieldName("传感器/环境温度传感器/Android环境温度")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.Android.AndroidAmbientTemperature")]
#endif
        AndroidAmbientTemperature = AmbientTemperatureSensor * InputSystemHelper.Range + 0,

        /// <summary>
        /// 计步器<see cref="UnityEngine.InputSystem.StepCounter"/>
        /// </summary>
        [Name("计步器")]
        [EnumFieldName("传感器/计步器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(StepCounter))]
#endif
        StepCounter = Sensor * InputSystemHelper.Range + 11,

        /// <summary>
        /// Android计步器<see cref="UnityEngine.InputSystem.Android.AndroidStepCounter"/>
        /// </summary>
        [Name("Android计步器")]
        [EnumFieldName("传感器/计步器/Android计步器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.Android.AndroidStepCounter")]
#endif
        AndroidStepCounter = StepCounter * InputSystemHelper.Range + 0,

        #endregion

        #region 跟踪设备 6

        /// <summary>
        /// 跟踪设备<see cref="UnityEngine.InputSystem.TrackedDevice"/>
        /// </summary>
        [Name("跟踪设备")]
        [EnumFieldName("跟踪设备")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(TrackedDevice))]
#endif
        TrackedDevice = 6,

        /// <summary>
        /// Vive灯塔<see cref="Unity.XR.OpenVR.ViveLighthouse"/>
        /// </summary>
        [Name("Vive灯塔")]
        [EnumFieldName("跟踪设备/Vive灯塔")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(ViveLighthouse))]
#endif
        ViveLighthouse = TrackedDevice * InputSystemHelper.Range + 0,

        /// <summary>
        /// Vive跟踪器<see cref="Unity.XR.OpenVR.ViveTracker"/>
        /// </summary>
        [Name("Vive跟踪器")]
        [EnumFieldName("跟踪设备/Vive跟踪器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(ViveTracker))]
#endif
        ViveTracker = TrackedDevice * InputSystemHelper.Range + 1,

        /// <summary>
        /// 手持式Vive跟踪器<see cref="Unity.XR.OpenVR.HandedViveTracker"/>
        /// </summary>
        [Name("手持式Vive跟踪器")]
        [EnumFieldName("跟踪设备/Vive跟踪器/手持式Vive跟踪器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(HandedViveTracker))]
#endif
        HandedViveTracker = ViveTracker * InputSystemHelper.Range + 0,

        /// <summary>
        /// Oculus跟踪引用<see cref="Unity.XR.Oculus.Input.OculusTrackingReference"/>
        /// </summary>
        [Name("Oculus跟踪引用")]
        [EnumFieldName("跟踪设备/Oculus跟踪引用")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(OculusTrackingReference))]
#endif
        OculusTrackingReference = TrackedDevice * InputSystemHelper.Range + 2,

        /// <summary>
        /// XR HMD<see cref="UnityEngine.InputSystem.XR.XRHMD"/>
        /// </summary>
        [Name("XR HMD")]
        [EnumFieldName("跟踪设备/XR HMD")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(XRHMD))]
#endif
        XRHMD = TrackedDevice * InputSystemHelper.Range + 3,

        /// <summary>
        /// OpenVR HMD<see cref="Unity.XR.OpenVR.OpenVRHMD"/>
        /// </summary>
        [Name("OpenVR HMD")]
        [EnumFieldName("跟踪设备/XR HMD/OpenVR HMD")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(OpenVRHMD))]
#endif
        OpenVRHMD = XRHMD * InputSystemHelper.Range + 0,

        /// <summary>
        /// Oculus HMD<see cref="Unity.XR.Oculus.Input.OculusHMD"/>
        /// </summary>
        [Name("Oculus HMD")]
        [EnumFieldName("跟踪设备/XR HMD/Oculus HMD")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(OculusHMD))]
#endif
        OculusHMD = XRHMD * InputSystemHelper.Range + 1,

        /// <summary>
        /// Oculus HMD扩展<see cref="Unity.XR.Oculus.Input.OculusHMDExtended"/>
        /// </summary>
        [Name("Oculus HMD扩展")]
        [EnumFieldName("跟踪设备/XR HMD/Oculus HMD/Oculus HMD扩展")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(OculusHMDExtended))]
#endif
        OculusHMDExtended = OculusHMD * InputSystemHelper.Range + 0,

        /// <summary>
        /// Daydream HMD<see cref="Unity.XR.GoogleVr.DaydreamHMD"/>
        /// </summary>
        [Name("Daydream HMD")]
        [EnumFieldName("跟踪设备/XR HMD/Daydream HMD")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(DaydreamHMD))]
#endif
        DaydreamHMD = XRHMD * InputSystemHelper.Range + 2,

        /// <summary>
        /// WMR HMD<see cref="UnityEngine.XR.WindowsMR.Input.WMRHMD"/>
        /// </summary>
        [Name("WMR HMD")]
        [EnumFieldName("跟踪设备/XR HMD/WMR HMD")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(WMRHMD))]
#endif
        WMRHMD = XRHMD * InputSystemHelper.Range + 3,

        /// <summary>
        /// XR仿真 HMD<see cref="UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.XRSimulatedHMD"/>
        /// </summary>
        [Name("XR仿真 HMD")]
        [EnumFieldName("跟踪设备/XR HMD/XR仿真 HMD")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.XRSimulatedHMD")]
#endif
        XRSimulatedHMD = XRHMD * InputSystemHelper.Range + 4,

        /// <summary>
        /// XR控制器<see cref="UnityEngine.InputSystem.XR.XRController"/>
        /// </summary>
        [Name("XR控制器")]
        [EnumFieldName("跟踪设备/XR控制器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(XRController))]
#endif
        XRController = TrackedDevice * InputSystemHelper.Range + 4,

        /// <summary>
        /// OpenVR控制器WMR<see cref="Unity.XR.OpenVR.OpenVRControllerWMR"/>
        /// </summary>
        [Name("OpenVR控制器WMR")]
        [EnumFieldName("跟踪设备/XR控制器/OpenVR控制器WMR")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(OpenVRControllerWMR))]
#endif
        OpenVRControllerWMR = XRController * InputSystemHelper.Range + 0,

        /// <summary>
        /// GearVR跟踪控制器<see cref="Unity.XR.Oculus.Input.GearVRTrackedController"/>
        /// </summary>
        [Name("GearVR跟踪控制器")]
        [EnumFieldName("跟踪设备/XR控制器/GearVR跟踪控制器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(GearVRTrackedController))]
#endif
        GearVRTrackedController = XRController * InputSystemHelper.Range + 1,

        /// <summary>
        /// Daydream控制器<see cref="Unity.XR.GoogleVr.DaydreamController"/>
        /// </summary>
        [Name("Daydream控制器")]
        [EnumFieldName("跟踪设备/XR控制器/Daydream控制器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(DaydreamController))]
#endif
        DaydreamController = XRController * InputSystemHelper.Range + 2,

        /// <summary>
        /// Hololens手<see cref="UnityEngine.XR.WindowsMR.Input.HololensHand"/>
        /// </summary>
        [Name("Hololens手")]
        [EnumFieldName("跟踪设备/XR控制器/Hololens手")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(HololensHand))]
#endif
        HololensHand = XRController * InputSystemHelper.Range + 3,

        /// <summary>
        /// 响应式XR控制器<see cref="UnityEngine.InputSystem.XR.XRControllerWithRumble"/>
        /// </summary>
        [Name("响应式XR控制器")]
        [EnumFieldName("跟踪设备/XR控制器/响应式XR控制器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(XRControllerWithRumble))]
#endif
        XRControllerWithRumble = XRController * InputSystemHelper.Range + 4,

        /// <summary>
        /// Vive魔棒<see cref="Unity.XR.OpenVR.ViveWand"/>
        /// </summary>
        [Name("Vive魔棒")]
        [EnumFieldName("跟踪设备/XR控制器/响应式XR控制器/Vive魔棒")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(ViveWand))]
#endif
        ViveWand = XRControllerWithRumble * InputSystemHelper.Range + 0,

        /// <summary>
        /// OpenVR Oculus触摸控制器<see cref="Unity.XR.OpenVR.OpenVROculusTouchController"/>
        /// </summary>
        [Name("OpenVR Oculus触摸控制器")]
        [EnumFieldName("跟踪设备/XR控制器/响应式XR控制器/OpenVR Oculus触摸控制器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(OpenVROculusTouchController))]
#endif
        OpenVROculusTouchController = XRControllerWithRumble * InputSystemHelper.Range + 1,

        /// <summary>
        /// Oculus触摸控制器<see cref="Unity.XR.Oculus.Input.OculusTouchController"/>
        /// </summary>
        [Name("Oculus触摸控制器")]
        [EnumFieldName("跟踪设备/XR控制器/响应式XR控制器/Oculus触摸控制器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(OculusTouchController))]
#endif
        OculusTouchController = XRControllerWithRumble * InputSystemHelper.Range + 2,

        /// <summary>
        /// WMR空间控制器<see cref="UnityEngine.XR.WindowsMR.Input.WMRSpatialController"/>
        /// </summary>
        [Name("WMR空间控制器")]
        [EnumFieldName("跟踪设备/XR控制器/响应式XR控制器/WMR空间控制器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(WMRSpatialController))]
#endif
        WMRSpatialController = XRControllerWithRumble * InputSystemHelper.Range + 3,

        /// <summary>
        /// XR仿真控制器<see cref="UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.XRSimulatedController"/>
        /// </summary>
        [Name("XR仿真控制器")]
        [EnumFieldName("跟踪设备/XR控制器/XR仿真控制器")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation.XRSimulatedController")]
#endif
        XRSimulatedController = XRController * InputSystemHelper.Range + 5,

        #endregion

        #region HID 7

        /// <summary>
        /// HID<see cref="UnityEngine.InputSystem.HID.HID"/>
        /// </summary>
        [Name("HID")]
        [EnumFieldName("HID")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.InputSystem.HID.HID")]
#endif
        HID = 7,

        #endregion

        #region Oculus远程 8

        /// <summary>
        /// Oculus远程<see cref="Unity.XR.Oculus.Input.OculusRemote"/>
        /// </summary>
        [Name("Oculus远程")]
        [EnumFieldName("Oculus远程")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType(typeof(OculusRemote))]
#endif
        OculusRemote = 8,

        #endregion

        #region OpenXR设备 9

        /// <summary>
        /// OpenXR设备<see cref="UnityEngine.XR.OpenXR.Input.OpenXRDevice"/>
        /// </summary>
        [Name("OpenXR设备")]
        [EnumFieldName("OpenXR设备")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.XR.OpenXR.Input.OpenXRDevice")]
#endif
        OpenXRDevice = 9,

        #endregion

        #region 手持AR输入设备 10

        /// <summary>
        /// 手持AR输入设备<see cref="UnityEngine.XR.ARSubsystems.HandheldARInputDevice"/>
        /// </summary>
        [Name("手持AR输入设备")]
        [EnumFieldName("手持AR输入设备")]
#if ENABLE_INPUT_SYSTEM
        [InputDeviceType("UnityEngine.XR.ARSubsystems.HandheldARInputDevice")]
#endif
        HandheldARInputDevice = 10,

        #endregion
    }

    /// <summary>
    /// 输入设备扩展类
    /// </summary>
    public static class InputDeviceExtension
    {
        /// <summary>
        /// 获取输入设备类型
        /// </summary>
        /// <param name="inputDevice"></param>
        /// <returns></returns>
        public static Type GetInputDeviceType(this EInputDevice inputDevice) => InputDeviceTypeCache.GetCacheValue(inputDevice);

        class InputDeviceTypeCache : TICache<InputDeviceTypeCache, EInputDevice, Type>
        {
            /// <summary>
            /// 构建值
            /// </summary>
            /// <param name="key1"></param>
            /// <returns></returns>
            protected override KeyValuePair<bool, Type> CreateValue(EInputDevice key1)
            {                
                return new KeyValuePair<bool, Type>(true, InputDeviceTypeAttribute.GetInputDeviceType(key1));
            }
        }
    }
}
