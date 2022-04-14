using UnityEngine;
using XCSJ.PluginTools.GameObjects;
using XCSJ.PluginTools.Notes.Dimensionings;
using XCSJ.PluginTools.Puts;
using XCSJ.PluginXGUI.Windows.ColorPickers;
using XCSJ.PluginXGUI.Windows.MiniMaps;
using XCSJ.Scripts;

namespace XCSJ.PluginTools.CNScripts
{
    /// <summary>
    /// 脚本ID
    /// </summary>
    public enum EScriptID
    {
        /// <summary>
        /// 结束
        /// </summary>
        _Begin = IDRange.Begin,

        #region 工具库-目录
        /// <summary>
        /// 工具库
        /// </summary>
        [ScriptName("工具库", "", EGrammarType.Category)]
        [ScriptDescription("工具库的相关脚本目录；")]
        #endregion
        ToolsExtension,

        #region 跟踪陀螺仪
        /// <summary>
        /// 跟踪陀螺仪
        /// </summary>
        [ScriptName("跟踪陀螺仪", "Track Gyro")]
        [ScriptDescription("跟踪陀螺仪;")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.Bool, "是否跟踪:")]
        #endregion 跟踪陀螺仪
        TrackGyro,

        #region 跟踪陀螺仪(指定游戏对象)
        /// <summary>
        /// 跟踪陀螺仪
        /// </summary>
        [ScriptName("跟踪陀螺仪(指定游戏对象)", "Track GyroWith GameObject")]
        [ScriptDescription("跟踪陀螺仪(指定游戏对象);")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Bool, "是否跟踪:")]
        #endregion 跟踪陀螺仪
        TrackGyroWithGameObject,

        #region 重置拖拽物体位置
        [ScriptName("重置拖拽物体位置", "Reset DragObject Position")]
        [ScriptDescription("重置拖拽物体位置")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "选择集拖拽工具:", "", typeof(DragByCameraView))]
        #endregion
        ResetDragObjectPosition,

        #region GL线框渲染器控制
        [ScriptName("GL线框渲染器控制", "GL WireFrame Renderer Control")]
        [ScriptDescription("GL线框渲染器控制")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Combo, "操作:", "启动", "停止")]
        #endregion
        GLWireFrameRendererControl,

        #region 设置自动旋转展示游戏对象
        /// <summary>
        /// 设置自动旋转展示游戏对象
        /// </summary>
        [ScriptName("设置自动旋转展示游戏对象", "Auto Rotate GameObject")]
        [ScriptDescription("组件激活后，开启游戏对象自动旋转；旋转期间，有输入之后，停止旋转；旋转停止之后，一段时间无输入，又会自动旋转；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "模型对象")]
        [ScriptParams(2, EParamType.Combo, "操作类型:", "启动", "停止")]
        //[ScriptParams(3, EParamType.FloatSlider, "激活后等待时间:", 0.1f, 10f, defaultObject = 3f)]
        //[ScriptParams(4, EParamType.FloatSlider, "旋转时间:", 1f, 100f, defaultObject = 20f)]
        //[ScriptParams(5, EParamType.FloatSlider, "等待时间:", 1f, 100f, defaultObject = 15f)]
        #endregion，
        AutoRotateGameObject,

        #region 标注

        #region 开启标注点拾取
        /// <summary>
        /// 开始标注点拾取
        /// </summary>
        [ScriptName("开启标注点拾取", "Begin Note Point Picker")]
        [ScriptDescription("通过鼠标点击场景，拾取标注的起点、终点或者中心等坐标")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "点击点拾取器", typeof(ClickPointPicker))]
        #endregion
        BeginNotePointPicker,

        #region 结束标注点拾取
        /// <summary>
        /// 开始标注点拾取
        /// </summary>
        [ScriptName("结束标注点拾取", "End Note Point Picker")]
        [ScriptDescription("结束标注点拾取")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "点击点拾取器", typeof(ClickPointPicker))]
        #endregion
        EndNotePointPicker,

        #endregion

        #region 颜色拾取器

        #region 获取调色板颜色
        /// <summary>
        /// 获取调色板颜色
        /// </summary>
        [ScriptName("获取调色板颜色", "Get ColorPicker Color")]
        [ScriptDescription("获取调色板颜色")]
        [ScriptReturn("成功返回 颜色值 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "调色板", typeof(ColorPicker))]
        #endregion
        GetColorPickerColor,

        #region 设置调色板颜色
        /// <summary>
        /// 设置调色板颜色
        /// </summary>
        [ScriptName("设置调色板颜色", "Set ColorPicker Color")]
        [ScriptDescription("设置调色板颜色")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "调色板", typeof(ColorPicker))]
        [ScriptParams(2, EParamType.Color, "颜色")]
        #endregion
        SetColorPickerColor,

        #region 同步渲染器颜色至调色板
        /// <summary>
        /// 同步渲染器颜色至颜色拾取器
        /// </summary>
        [ScriptName("同步渲染器颜色至调色板", "Set Renderer To ColorPicker")]
        [ScriptDescription("同步渲染器颜色至调色板")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "游戏对象", typeof(Renderer))]
        [ScriptParams(2, EParamType.GameObjectComponent, "调色板", typeof(ColorPicker))]
        #endregion
        SetRendererToColorPicker,

        #region 设置颜色绑定器模式
        /// <summary>
        /// 设置颜色绑定器模式
        /// </summary>
        [ScriptName("设置颜色绑定器模式", "Set ColorPickerBinder Mode")]
        [ScriptDescription("设置颜色绑定器模式")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "颜色绑定器", typeof(ColorPickerBinder))]
        [ScriptParams(2, EParamType.Combo, "类型:", "游戏对象列表", "选择集")]
        #endregion
        SetColorPickerBinderMode,

        #region 同步颜色绑定器颜色至颜色拾取器
        /// <summary>
        /// 同步颜色绑定器颜色至颜色拾取器
        /// </summary>
        [ScriptName("同步颜色绑定器颜色至调色板", "Set ColorPickerBinder To ColorPicker")]
        [ScriptDescription("同步颜色绑定器颜色至调色板")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "调色板", typeof(ColorPickerBinder))]
        #endregion
        SetColorPickerBinderToColorPicker,

        #region 颜色绑定器游戏对象操作
        /// <summary>
        /// 颜色绑定器游戏对象操作
        /// </summary>
        [ScriptName("颜色绑定器游戏对象操作", "ColorPickerBinder GameObject Operation")]
        [ScriptDescription("颜色绑定器游戏对象操作")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "调色板", typeof(ColorPickerBinder))]
        [ScriptParams(2, EParamType.Combo, "类型:", "添加绑定游戏对象", "移除绑定游戏对象", "清空所有绑定游戏对象")]
        [ScriptParams(3, EParamType.GameObject, "游戏对象")]
        #endregion
        ColorPickerBinderGameObjectOperation,

        #endregion

        #region 导航图

        #region 导航项操作
        /// <summary>
        /// 导航项操作
        /// </summary>
        [ScriptName("导航项操作", "MiniMap Item Operation")]
        [ScriptDescription("导航项操作")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "导航图", typeof(MiniMap))]
        [ScriptParams(2, EParamType.Combo, "类型:", "添加", "移除")]
        [ScriptParams(3, EParamType.GameObject, "游戏对象")]
        [ScriptParams(4, EParamType.GameObjectComponent, "关联UGUI", typeof(RectTransform))]
        #endregion
        MiniMapItemOperation,

        #region 设置导航图传送规则
        /// <summary>
        /// 设置导航图传送规则
        /// </summary>
        [ScriptName("设置导航图传送规则", "MiniMap Teleport Rule")]
        [ScriptDescription("设置导航图传送规则")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "导航图", typeof(MiniMapTeleport))]
        [ScriptParams(2, EParamType.Combo, "传送规则:", "无", "传送需射线撞击")]
        #endregion
        SetMiniMapTeleportRule,


        #region 设置导航图传送高度偏移量
        /// <summary>
        /// 设置导航图传送高度偏移量
        /// </summary>
        [ScriptName("设置导航图传送高度偏移量", "Set MiniMap Teleport HeightOffset")]
        [ScriptDescription("设置导航图传送高度偏移量")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "导航图", typeof(MiniMapTeleport))]
        [ScriptParams(2, EParamType.Float, "高度偏移量:")]
        #endregion
        SetMiniMapTeleportHeightOffset,

        #endregion

        #region 销毁游戏对象列表实例对象
        /// <summary>
        /// 销毁游戏对象列表实例对象
        /// </summary>
        [ScriptName("销毁游戏对象列表实例对象", "DestroyGameObjectListInstanceObject")]
        [ScriptDescription("销毁游戏对象列表实例对象")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "已克隆对象")]
        #endregion
        DestroyGameObjectListInstanceObject,

        #region 销毁选择集中游戏对象列表实例对象
        /// <summary>
        /// 销毁选择集中游戏对象列表实例对象
        /// </summary>
        [ScriptName("销毁选择集中游戏对象列表实例对象", "DestroyGameObjectListInstanceObjectInSelection")]
        [ScriptDescription("销毁选择集中游戏对象列表实例对象")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        #endregion
        DestroyGameObjectListInstanceObjectInSelection,

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent
    }
}
