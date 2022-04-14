using UnityEngine;
using XCSJ.PluginCamera;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Legacies;
using XCSJ.PluginsCameras.Legacies.CNScripts;
using XCSJ.Scripts;
using IDRange = XCSJ.PluginCamera.IDRange;

namespace XCSJ.PluginsCameras.CNScripts
{
    /// <summary>
    /// 相机脚本ID枚举
    /// </summary>
    public enum ECameraScriptID
    {
        /// <summary>
        /// 开始
        /// </summary>
        _Begin = IDRange.Begin,

        #region 旧版相机

#pragma warning disable CS0618 // 类型或成员已过时

        #region 旧版相机-目录
        /// <summary>
        /// 相机(旧版)
        /// </summary>
        [ScriptName("相机(旧版)", nameof(LegacyCamera), EGrammarType.Category)]
        [ScriptDescription("旧版相机操作的目录")]
        #endregion
        LegacyCamera,

        #region 切换相机
        /// <summary>
        /// 切换相机，根据选择的对象
        /// </summary>
        [ScriptName("切换相机", "Switch Camera")]
        [ScriptDescription("从当前相机切换至指定相机", "Switch Camera")]
        [ScriptReturn("返回值为空", "Return null")]
        [ScriptParams(1, EParamType.GameObject, "目标相机对象:", typeof(BaseCamera))]
        [ScriptParams(2, EParamType.FloatSlider, "过渡时间(当 值<0.01 时，执行立即切换):", 0f, 10f)]
        [ScriptParams(3, EParamType.UserDefineFun, "回调函数:")]
        [ScriptParams(4, EParamType.Bool, "强制切换:")]
        #endregion
        SwitchCamera,

        #region 切换相机（按名称）
        /// <summary>
        /// 切换相机，根据相机名称
        /// </summary>
        [ScriptName("切换相机(按名称)", "Switch Camera By Name")]
        [ScriptDescription("按照名称切换相机", "Switch Camera by the String you give")]
        [ScriptReturn("返回值为空", "Return null")]
        [ScriptParams(1, ForCameraName.ScriptParamType, "相机名称:")]
        [ScriptParams(2, EParamType.FloatSlider, "过渡时间(当 值<0.01 时，执行立即切换):", 0f, 10f)]
        [ScriptParams(3, EParamType.UserDefineFun, "回调函数:")]
        [ScriptParams(4, EParamType.Bool, "强制切换:")]
        #endregion
        SwtichCameraByName,

        #region 设置指定相机的移动速度
        /// <summary>
        /// 设置相机速度
        /// </summary>
        [ScriptName("设置相机速度", "Set Speed of Camera")]
        [ScriptDescription("设置指定相机的移动速度", "Set Moving Speed of Camera you appointed")]
        [ScriptReturn("返回值为空", "Return null")]
        [ScriptParams(1, EParamType.GameObject, "目标相机:", typeof(BaseCamera))]
        [ScriptParams(2, EParamType.FloatSlider, "速度值:", 0f, 200f)]
        [ScriptParams(3, EParamType.Combo, "类型:", "移动", "旋转")]
        #endregion
        SetCameraSpeed,

        #region 设置相机速度
        /// <summary>
        /// 设置当前相机的移动速度
        /// </summary>
        [ScriptName("设置当前相机速度", "Set Speed of Current Camera ")]
        [ScriptDescription("设置当前相机的移动速度", "Set Moving Speed of Current Camera ")]
        [ScriptReturn("返回值为空", "Return null")]
        [ScriptParams(2, EParamType.FloatSlider, "速度值:", 0f, 200f)]
        [ScriptParams(3, EParamType.Combo, "类型:", "移动", "旋转")]
        #endregion
        SetCurrentCameraSpeed,

        #region 设置指定相机的近裁截面
        /// <summary>
        /// 设置指定相机的近裁截面
        /// </summary>
        [ScriptName("设置相机近裁截面", "Set Camera Near Clip Plane")]
        [ScriptDescription("设置指定相机的近裁截面")]
        [ScriptReturn("返回值为空")]
        [ScriptParams(1, EParamType.GameObject, "目标相机:", typeof(BaseCamera))]
        [ScriptParams(2, EParamType.FloatSlider, "近裁截面:", 0.001f, 20f, defaultObject = 0.3f)]
        #endregion
        SetCameraNearClipPlane,

        #region 设置当前相机的近裁截面
        /// <summary>
        /// 设置当前相机的近裁截面
        /// </summary>
        [ScriptName("设置当前相机近裁截面", "Set Current Camera Near Clip Plane")]
        [ScriptDescription("设置当前相机的近裁截面")]
        [ScriptReturn("返回值为空")]
        [ScriptParams(2, EParamType.FloatSlider, "近裁截面:", 0.001f, 20f, defaultObject = 0.3f)]
        #endregion
        SetCurrentCameraNearClipPlane,

        #region 设置指定相机的远裁截面
        /// <summary>
        /// 设置指定相机的远裁截面
        /// </summary>
        [ScriptName("设置相机远裁截面", "Set Camera Far Clip Plane")]
        [ScriptDescription("设置指定相机的远裁截面")]
        [ScriptReturn("返回值为空")]
        [ScriptParams(1, EParamType.GameObject, "目标相机:", typeof(BaseCamera))]
        [ScriptParams(2, EParamType.FloatSlider, "远裁截面:", 1f, 3000f, defaultObject = 1000f)]
        #endregion
        SetCameraFarClipPlane,

        #region 设置当前相机的远裁截面
        /// <summary>
        /// 设置当前相机的远裁截面
        /// </summary>
        [ScriptName("设置当前相机远裁截面", "Set Current Camera Far Clip Plane")]
        [ScriptDescription("设置当前相机的远裁截面")]
        [ScriptReturn("返回值为空")]
        [ScriptParams(2, EParamType.FloatSlider, "远裁截面:", 1f, 3000f, defaultObject = 1000f)]
        #endregion
        SetCurrentCameraFarClipPlane,

        #region 设置相机视角缩放系数
        /// <summary>
        /// 设置相机视角缩放系数
        /// </summary>
        [ScriptName("设置相机视角缩放系数", "Set Camera Field Of View")]
        [ScriptDescription("设置指定相机的视角缩放系数")]
        [ScriptReturn("返回值为空")]
        [ScriptParams(1, EParamType.GameObject, "目标相机:", typeof(BaseCamera))]
        [ScriptParams(2, EParamType.IntSlider, "视角缩放系数:", 0, 179, defaultObject = 60)]
        #endregion
        SetCameraFieldOfView,

        #region 设置当前相机的视角缩放系数
        /// <summary>
        /// 设置当前相机的视角缩放系数
        /// </summary>
        [ScriptName("设置当前相机视角缩放系数", "Set Current Camera Field Of View")]
        [ScriptDescription("设置当前相机的视角缩放系数")]
        [ScriptReturn("返回值为空")]
        [ScriptParams(2, EParamType.IntSlider, "视角缩放系数:", 0, 179, defaultObject = 60)]
        #endregion
        SetCurrentCameraFieldOfView,

        #region 设置相机平移
        /// <summary>
        /// 设置指定相机前、后、左、右平移
        /// </summary>
        [ScriptName("设置相机平移", "Set Camera Move")]
        [ScriptDescription("设置指定相机前、后、左、右平移")]
        [ScriptReturn("返回值为空")]
        [ScriptParams(1, EParamType.GameObject, "相机:", typeof(BaseCamera))]
        [ScriptParams(2, EParamType.Combo, "移动方向:", "前进", "后退", "左移", "右移", "上移", "下移")]
        [ScriptParams(3, EParamType.FloatSlider, "移动距离:", 0f, 99f, defaultObject = 1f)]
        #endregion
        SetCameraMove,

        #region 设置当前相机平移
        /// <summary>
        /// 设置当前相机前、后、左、右平移
        /// </summary>
        [ScriptName("设置当前相机平移", "Set Current Camera Move")]
        [ScriptDescription("设置当前相机前、后、左、右平移")]
        [ScriptReturn("返回值为空")]
        [ScriptParams(2, EParamType.Combo, "移动方向:", "前进", "后退", "左移", "右移")]
        [ScriptParams(3, EParamType.FloatSlider, "移动距离:", 0f, 99f, defaultObject = 1f)]
        #endregion
        SetCurrentCameraMove,

        #region 设置相机旋转
        /// <summary>
        /// 设置指定相机绕x,y轴旋转
        /// </summary>
        [ScriptName("设置相机旋转", "Set Camera Rotate")]
        [ScriptDescription("设置指定相机绕x,y轴旋转")]
        [ScriptReturn("返回值为空")]
        [ScriptParams(1, EParamType.GameObject, "相机:", typeof(BaseCamera))]
        [ScriptParams(2, EParamType.Combo, "旋转轴:", "X轴", "Y轴")]
        [ScriptParams(3, EParamType.FloatSlider, "旋转角度:", -360f, 360f)]
        #endregion
        SetCameraRotate,

        #region 设置当前相机旋转
        /// <summary>
        /// 设置当前相机绕x,y轴旋转
        /// </summary>
        [ScriptName("设置当前相机旋转", "Set Current Camera Rotate")]
        [ScriptDescription("设置当前相机绕x,y轴旋转")]
        [ScriptReturn("返回值为空")]
        [ScriptParams(2, EParamType.Combo, "旋转轴:", "X轴", "Y轴")]
        [ScriptParams(3, EParamType.FloatSlider, "旋转角度:", -360f, 360f)]
        #endregion
        SetCurrentCameraRotate,

        #region 设置相机绕目标旋转
        /// <summary>
        /// 设置指定相机绕目标x,y轴旋转
        /// </summary>
        [ScriptName("设置相机绕目标旋转", "Set Camera Target Rotate")]
        [ScriptDescription("设置指定相机绕目标x,y轴旋转")]
        [ScriptReturn("返回值为空")]
        [ScriptParams(1, EParamType.GameObject, "相机:", typeof(BaseCamera))]
        [ScriptParams(2, EParamType.Combo, "旋转轴:", "X轴", "Y轴")]
        [ScriptParams(3, EParamType.FloatSlider, "旋转角度:", -360f, 360f)]
        #endregion
        SetCameraTargetRotate,

        #region 设置当前相机绕目标旋转
        /// <summary>
        /// 设置当前相机绕目标x,y轴旋转
        /// </summary>
        [ScriptName("设置当前相机绕目标旋转", "Set Current Camera Target Rotate")]
        [ScriptDescription("设置当前相机绕目标x,y轴旋转")]
        [ScriptReturn("返回值为空")]
        [ScriptParams(2, EParamType.Combo, "旋转轴:", "X轴", "Y轴")]
        [ScriptParams(3, EParamType.FloatSlider, "旋转角度:", -360f, 360f)]
        #endregion
        SetCurrentCameraTargetRotate,

        #region 重置当前相机
        /// <summary>
        /// 重置当前相机
        /// </summary>
        [ScriptName("重置当前相机", "Reset Current Camera")]
        [ScriptDescription("重置当前相机,将当前相机重置到程序启动时的初始位置与状态；")]
        [ScriptReturn("返回值为空")]
        [ScriptParams(2, EParamType.FloatSlider, "持续时间(当 值<0.01 时，执行立即重置):", 0f, 10f)]
        [ScriptParams(3, EParamType.UserDefineFun, "重置后回调函数:")]
        [ScriptParams(4, EParamType.String, "额外参数:")]
        #endregion
        ResetCurrentCamera,

        #region 锁定当前相机
        /// <summary>
        /// 锁定当前相机
        /// </summary>
        [ScriptName("锁定当前相机", "Lock Current Camera")]
        [ScriptDescription("锁定当前相机，相机锁定后不再响应用户的输入操作,包括鼠标、键盘、触摸等输入；")]
        [ScriptParams(2, EParamType.Bool, "是否锁定:")]
        #endregion
        LockCurrentCamera,

        #region 获取当前相机信息
        /// <summary>
        /// 获取当前相机信息
        /// </summary>
        [ScriptName("获取当前相机信息", "Get Current Camera Info")]
        [ScriptDescription("获取当前相机信息；")]
        [ScriptParams(2, EParamType.Combo, "信息类型:", "游戏对象", "游戏对象组件", "组件类型", "目标游戏对象")]
        #endregion
        GetCurrentCameraInfo,

        #region 操作当前相机
        /// <summary>
        /// 操作当前相机
        /// </summary>
        [ScriptName("操作当前相机", "Operator Current Camera")]
        [ScriptDescription("操作当前相机；")]
        [ScriptParams(2, EParamType.Combo, "操作类型:", "刷新信息", "锁定", "解除锁定", "跳跃", "显示速度信息", "移动速度自动增加", "移动速度自动减少", "旋转速度自动增加", "旋转速度自动减少")]
        #endregion
        OperatorCurrentCamera,

        #region 设置相机目标游戏对象
        /// <summary>
        /// 设置相机目标游戏对象
        /// </summary>
        [ScriptName("设置相机目标游戏对象", "Set Camera Target GameObject")]
        [ScriptDescription("设置相机目标游戏对象；对拥有目标的相机可以修改其目标对象；对无目标的相机（例如飞行相机、定点相机）则没有影响；")]
        [ScriptParams(1, EParamType.GameObject, "相机", typeof(BaseCamera))]
        [ScriptParams(2, EParamType.GameObject, "目标游戏对象")]
        #endregion
        SetCameraTargetGameObject,

        #region 设置当前相机目标游戏对象
        /// <summary>
        /// 设置当前相机目标游戏对象
        /// </summary>
        [ScriptName("设置当前相机目标游戏对象", "Set Current Camera Target GameObject")]
        [ScriptDescription("设置当前相机目标游戏对象；对拥有目标的相机可以修改其目标对象；对无目标的相机（例如飞行相机、定点相机）则没有影响；")]
        [ScriptParams(2, EParamType.GameObject, "目标游戏对象")]
        #endregion
        SetCurrentCameraTargetGameObject,

        #region 相机聚焦目标
        [ScriptName("相机聚焦目标", "Camera Focus Target")]
        [ScriptDescription("相机聚焦目标;会修改相机的当前观察对象；")]
        [ScriptParams(1, EParamType.GameObject, "相机(如果为空则使用当前相机)", typeof(BaseCamera))]
        [ScriptParams(2, ForCameraFocusType.ScriptParamType, "相机聚焦类型:")]
        [ScriptParams(3, EParamType.GameObject, "目标(游戏对象)")]
        [ScriptParams(4, EParamType.FloatSlider, "距离系数", 0.1f, 20f, defaultObject = 1.732f)]
        #endregion
        CameraFocusTarget,

        #region 相机聚焦选择集
        [ScriptName("相机聚焦选择集", "Camera Focus Selection")]
        [ScriptDescription("相机聚焦选择集;会修改相机的当前观察对象；")]
        [ScriptParams(1, EParamType.GameObject, "相机(如果为空则使用当前相机)", typeof(BaseCamera))]
        [ScriptParams(2, ForCameraFocusType.ScriptParamType, "相机聚焦类型:")]
        [ScriptParams(4, EParamType.FloatSlider, "距离系数", 0.1f, 20f, defaultObject = 1.732f)]
        #endregion
        CameraFocusSelection,

        #region 设置相机视口矩形
        /// <summary>
        /// 设置相机视口矩形
        /// </summary>
        [ScriptName("设置相机视口矩形", "Set Camera Viewport Rectangle")]
        [ScriptDescription("设置相机视口矩形")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "相机", "", typeof(Camera))]
        [ScriptParams(2, EParamType.Rect, "视口矩形", defaultObject = "0/0/1/1")]
        #endregion
        SetCameraViewportRectangle,

#pragma warning restore CS0618 // 类型或成员已过时

        #endregion

        #region 新版相机

        #region 新版相机-目录
        /// <summary>
        /// 相机
        /// </summary>
        [ScriptName("相机", nameof(Cameras), EGrammarType.Category)]
        [ScriptDescription("相机操作的目录")]
        #endregion
        Cameras,

        #region 切换相机控制器

        /// <summary>
        /// 切换相机控制器:从当前切换相机控制器切换至指定的目标相机控制器
        /// </summary>
        [ScriptName("切换相机控制器", nameof(SwitchCameraController))]
        [ScriptDescription("从当前切换相机控制器切换至指定的目标相机控制器")]
        [ScriptReturn("返回值为空", "Return null")]
        [ScriptParams(1, EParamType.GameObject, "目标相机控制器对象:", typeof(BaseCameraMainController))]
        [ScriptParams(2, EParamType.FloatSlider, "过渡时间:", 0f, 10f)]
        [ScriptParams(3, EParamType.UserDefineFun, "回调函数:")]
        [ScriptParams(4, EParamType.Bool, "强制切换:")]
        [ScriptParams(5, EParamType.Combo, "切换规则:", "使用目标相机控制器对象", "上一个相机", "下一个相机")]
        #endregion
        SwitchCameraController,

        /// <summary>
        /// 设置相机控制器主目标
        /// </summary>
        [ScriptName("设置相机控制器主目标", nameof(SetCameraControllerMainTarget))]
        [ScriptDescription("设置相机控制器主目标；如相机控制器查找规则为‘指定’时，使用‘相机控制器’参数作为处理对象；如相机控制器查找规则为‘当前’时，使用当前相机控制器作为处理对象")]
        [ScriptReturn("成功返回#True；失败返回#False；")]
        [ScriptParams(1, EParamType.GameObject, "相机控制器:", typeof(BaseCameraMainController))]
        [ScriptParams(2, EParamType.GameObject, "主目标:", typeof(Transform))]
        [ScriptParams(10, EParamType.Combo, "相机控制器查找规则:", "指定", "当前")]
        SetCameraControllerMainTarget,

        /// <summary>
        /// 设置相机控制器速度系数
        /// </summary>
        [ScriptName("设置相机控制器速度系数", nameof(SetCameraControllerSpeedCoefficient))]
        [ScriptDescription("设置相机控制器速度系数；如相机控制器查找规则为‘指定’时，使用‘相机控制器’参数作为处理对象；如相机控制器查找规则为‘当前’时，使用当前相机控制器作为处理对象")]
        [ScriptReturn("成功返回#True；失败返回#False；")]
        [ScriptParams(1, EParamType.GameObject, "相机控制器:", typeof(BaseCameraMainController))]
        [ScriptParams(2, EParamType.Vector3, "速度系数:")]
        [ScriptParams(3, EParamType.Combo, "速度系数类型:", "移动", "旋转")]
        [ScriptParams(10, EParamType.Combo, "相机控制器查找规则:", "指定", "当前")]
        SetCameraControllerSpeedCoefficient,

        /// <summary>
        /// 设置相机控制器阻尼系数
        /// </summary>
        [ScriptName("设置相机控制器阻尼系数", nameof(SetCameraControllerDampingCoefficient))]
        [ScriptDescription("设置相机控制器阻尼系数；如相机控制器查找规则为‘指定’时，使用‘相机控制器’参数作为处理对象；如相机控制器查找规则为‘当前’时，使用当前相机控制器作为处理对象")]
        [ScriptReturn("成功返回#True；失败返回#False；")]
        [ScriptParams(1, EParamType.GameObject, "相机控制器:", typeof(BaseCameraMainController))]
        [ScriptParams(2, EParamType.FloatSlider, "阻尼系数:", 0f, CameraHelperExtension.MaxDampingCoefficient)]
        [ScriptParams(3, EParamType.Combo, "阻尼系数类型:", "移动", "旋转")]
        [ScriptParams(10, EParamType.Combo, "相机控制器查找规则:", "指定", "当前")]
        SetCameraControllerDampingCoefficient,

        /// <summary>
        /// 恢复相机控制器状态
        /// </summary>
        [ScriptName("恢复相机控制器状态", nameof(RecoverCameraControllerState))]
        [ScriptDescription("恢复相机控制器状态；如相机控制器查找规则为‘指定’时，使用‘相机控制器’参数作为处理对象；如相机控制器查找规则为‘当前’时，使用当前相机控制器作为处理对象")]
        [ScriptReturn("成功返回#True；失败返回#False；")]
        [ScriptParams(1, EParamType.GameObject, "相机控制器:", typeof(BaseCameraMainController))]
        [ScriptParams(2, EParamType.Combo, "状态类型:", "原始状态", "上一次状态")]
        [ScriptParams(10, EParamType.Combo, "相机控制器查找规则:", "指定", "当前")]
        RecoverCameraControllerState,

        #endregion

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent,

        /// <summary>
        /// 结束
        /// </summary>
        _End = IDRange.End,
    }
}
