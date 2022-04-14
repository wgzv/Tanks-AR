using System;
using UnityEngine;
using XCSJ.Extension.CNScripts.UGUI;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension.ExtensionExample
{
    /// <summary>
    /// 案例ID区间
    /// </summary>
    public static class ExampleIDRange
    {
        public const int Begin = (int)EExtensionID._0xa;//34048
        public const int End = (int)EExtensionID._0xb - 1;

        public const int Fragment = 0x18;//24

        public const int Common = Begin + Fragment * 0;//34048
        public const int MonoBehaviour = Begin + Fragment * 1;//34072
        public const int StateLib = Begin + Fragment * 2;//34096
        public const int Tools = Begin + Fragment * 3;//34120
        public const int Editor = Begin + Fragment * 4;//34144
    }

    /// <summary>
    /// 案例脚本ID
    /// </summary>
    public enum EExampleScriptID
    {
        /// <summary>
        /// 最小脚本ID
        /// </summary>
        MinScriptID = EExtensionID._0xa,

        [ScriptName("扩展案例", "Extension Example", EGrammarType.Category)]
        ExtensionExample,

        [ScriptName("输出Hello World", "Output Hello World")]
        OutputHelloWorld,

        [ScriptName("测试脚本参数字符串", "Test Script Param String")]
        [ScriptParams(1, EParamType.String, "字符串String:")]
        [ScriptParams(2, EParamType.String, "字符串String:", defaultObject = "默认字符串")]
        TestScriptParamString,

        [ScriptName("测试脚本参数标准字符串", "Test Script Param StandardString")]
        [ScriptParams(1, EParamType.StandardString, "标准字符串StandardString:")]
        [ScriptParams(2, EParamType.StandardString, "标准字符串StandardString:", defaultObject = "默认标准字符串")]
        TestScriptParamStandardString,

        [ScriptName("测试脚本参数组合", "Test Script Param Combo")]
        [ScriptParams(1, EParamType.Combo, "组合Combo:", "Combo0", "Combo1", "Combo2")]
        [ScriptParams(2, EParamType.Combo, "组合Combo:", "Combo0", "Combo1", "Combo2", defaultObject = "Combo1")]
        TestScriptParamCombo,

        [ScriptName("测试脚本参数文件", "Test Script Param File")]
        [ScriptParams(1, EParamType.File, "打开文件File:")]
        [ScriptParams(2, EParamType.File, "打开文件File:", defaultObject = "C://")]
        TestScriptParamFile,

        [ScriptName("测试脚本参数保存文件", "Test Script Param SaveFile")]
        [ScriptParams(1, EParamType.SaveFile, "保存文件SaveFile:")]
        [ScriptParams(2, EParamType.SaveFile, "保存文件SaveFile:", defaultObject = "C://")]
        TestScriptParamSaveFile,

        [ScriptName("测试脚本参数打开文件夹", "Test Script Param OpenFolder")]
        [ScriptParams(1, EParamType.OpenFolder, "打开文件夹OpenFolder:")]
        [ScriptParams(2, EParamType.OpenFolder, "打开文件夹OpenFolder:", defaultObject = "C://")]
        TestScriptParamOpenFolder,

        [ScriptName("测试脚本参数保存文件夹", "Test Script Param SaveFolder")]
        [ScriptParams(1, EParamType.SaveFolder, "保存文件夹SaveFolder:")]
        [ScriptParams(2, EParamType.SaveFolder, "保存文件夹SaveFolder:", defaultObject = "C://")]
        TestScriptParamSaveFolder,

        [ScriptName("测试脚本参数用户自定义函数", "Test Script Param UserDefineFun")]
        [ScriptParams(1, EParamType.UserDefineFun, "用户自定义函数UserDefineFun:")]
        [ScriptParams(2, EParamType.UserDefineFun, "用户自定义函数UserDefineFun:", defaultObject = "无")]
        [ScriptParams(3, EParamType.UserDefineFun, "用户自定义函数UserDefineFun:", defaultObject = 1)]
        TestScriptParamUserDefineFun,

        [ScriptName("测试脚本参数全局变量名", "Test Script Param GlobalVariableName")]
        [ScriptParams(1, EParamType.GlobalVariableName, "全局变量值GlobalVariableName:")]
        [ScriptParams(2, EParamType.GlobalVariableName, "全局变量值GlobalVariableName:", defaultObject = "全局变量名")]
        TestScriptParamGlobalVariableName,

        [ScriptName("测试脚本参数变量", "Test Script Param Variable")]
        [ScriptParams(1, EParamType.Variable, "变量Variable:")]
        [ScriptParams(2, EParamType.Variable, "变量Variable:", defaultObject = "_GO")]
        TestScriptParamVariable,

        [ScriptName("测试脚本参数布尔简化2类型", "Test Script Param B2")]
        [ScriptParams(1, EParamType.B2, "布尔简化2B2:")]
        [ScriptParams(2, EParamType.B2, "布尔简化2B2:", defaultObject = EB2.Y)]
        [ScriptParams(3, EParamType.B2, "布尔简化2B2:", defaultObject = "Y")]
        [ScriptParams(4, EParamType.B2, "布尔简化2B2:", defaultObject = true)]
        TestScriptParamB2,

        [ScriptName("测试脚本参数布尔2类型", "Test Script Param Bool2")]
        [ScriptParams(1, EParamType.Bool2, "布尔2Bool2:")]
        [ScriptParams(2, EParamType.Bool2, "布尔2Bool2:", defaultObject = EBool2.Yes)]
        [ScriptParams(3, EParamType.Bool2, "布尔2Bool2:", defaultObject = "Switch")]
        [ScriptParams(4, EParamType.Bool2, "布尔2Bool2:", defaultObject = true)]
        TestScriptParamBool2,

        [ScriptName("测试脚本参数布尔类型", "Test Script Param Bool")]
        [ScriptParams(1, EParamType.Bool, "布尔Bool:")]
        [ScriptParams(2, EParamType.Bool, "布尔Bool:", defaultObject = EBool.Yes)]
        [ScriptParams(3, EParamType.Bool, "布尔Bool:", defaultObject = "Switch")]
        [ScriptParams(4, EParamType.Bool, "布尔Bool:", defaultObject = true)]
        TestScriptParamBool,

        [ScriptName("测试脚本参数整形", "Test Script Param Int")]
        [ScriptParams(1, EParamType.Int, "整形Int:")]
        [ScriptParams(2, EParamType.Int, "整形Int:", defaultObject = 1)]
        [ScriptParams(3, EParamType.Int, "整形Int:", defaultObject = "2")]
        TestScriptParamInt,

        [ScriptName("测试脚本参数整形滑动条", "Test Script Param IntSlider")]
        [ScriptParams(1, EParamType.IntSlider, "整形滑动条IntSlider:", -10, 10)]
        [ScriptParams(2, EParamType.IntSlider, "整形滑动条IntSlider:", -10, 10, defaultObject = 1)]
        [ScriptParams(3, EParamType.IntSlider, "整形滑动条IntSlider:", -10, 10, defaultObject = "2")]
        TestScriptParamIntSlider,

        [ScriptName("测试脚本参数整形弹出框", "Test Script Param IntPopup")]
        [ScriptParams(1, EParamType.IntPopup, "整形弹出框IntPopup:", "name1", 1, "XXXXXX", 2, "name3", 3)]
        [ScriptParams(2, EParamType.IntPopup, "整形弹出框IntPopup:", "name1", 1, "name2", 2, "name3", 3, defaultObject = 2)]
        [ScriptParams(3, EParamType.IntPopup, "整形弹出框IntPopup:", "name1", 1, "name2", 2, "name3", 3, defaultObject = "3")]
        TestScriptParamIntPopup,

        [ScriptName("测试脚本参数长整型", "Test Script Param Long")]
        [ScriptParams(1, EParamType.Long, "长整型Long:")]
        [ScriptParams(2, EParamType.Long, "长整型Long:", defaultObject = 1L)]
        [ScriptParams(3, EParamType.Long, "长整型Long:", defaultObject = "1")]
        TestScriptParamLong,

        [ScriptName("测试脚本参数浮点数", "Test Script Param Float")]
        [ScriptParams(1, EParamType.Float, "浮点数Float:")]
        [ScriptParams(2, EParamType.Float, "浮点数Float:", defaultObject = 1f)]
        [ScriptParams(3, EParamType.Float, "浮点数Float:", defaultObject = "2")]
        TestScriptParamFloat,

        [ScriptName("测试脚本参数浮点数滑动条", "Test Script Param FloatSlider")]
        [ScriptParams(1, EParamType.FloatSlider, "浮点数滑动条FloatSlider:", -10f, 10f)]
        [ScriptParams(2, EParamType.FloatSlider, "浮点数滑动条FloatSlider:", -10f, 10f, defaultObject = 1f)]
        [ScriptParams(3, EParamType.FloatSlider, "浮点数滑动条FloatSlider:", -10f, 10f, defaultObject = "2")]
        TestScriptParamFloatSlider,

        [ScriptName("测试脚本参数双精度浮点数", "Test Script Param Double")]
        [ScriptParams(1, EParamType.Double, "双精度浮点数Double:")]
        [ScriptParams(2, EParamType.Double, "双精度浮点数Double:", defaultObject = 1.0)]
        [ScriptParams(3, EParamType.Double, "双精度浮点数Double:", defaultObject = "2")]
        TestScriptParamDouble,

        [ScriptName("测试脚本参数游戏对象", "Test Script Param GameObject")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象GameObject:")]
        [ScriptParams(2, EParamType.GameObject, "游戏对象GameObject(限定类型UnityEngine.UI.Button，即限定游戏对象必须拥有该组件):", typeof(UnityEngine.UI.Button))]
        TestScriptParamGameObject,

        [ScriptName("测试脚本参数组件", "Test Script Param Component")]
        [ScriptParams(1, EParamType.Component, "组件Component:")]
        TestScriptParamComponent,

        [ScriptName("测试脚本参数游戏对象组件", "Test Script Param GameObjectComponent")]
        [ScriptParams(1, EParamType.GameObjectComponent, "游戏对象组件GameObjectComponent:")]
        [ScriptParams(2, EParamType.GameObjectComponent, "游戏对象组件GameObjectComponent(限定类型UnityEngine.UI.Button，即限定游戏对象必须拥有该组件):", typeof(UnityEngine.UI.Button))]
        TestScriptParamGameObjectComponent,

        [ScriptName("测试脚本参数组件游戏对象", "Test Script Param ComponentGameObject")]
        [ScriptParams(1, EParamType.ComponentGameObject, "游戏对象组件ComponentGameObject:")]
        [ScriptParams(2, EParamType.ComponentGameObject, "游戏对象组件ComponentGameObject(限定类型UnityEngine.UI.Button，即限定游戏对象必须拥有该组件):", typeof(UnityEngine.UI.Button))]
        TestScriptParamComponentGameObject,

        [ScriptName("测试脚本参数游戏对象脚本事件", "Test Script Param GameObjectSciptEvent")]
        [ScriptParams(1, EParamType.GameObjectSciptEvent, "游戏对象脚本事件GameObjectSciptEvent:")]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(2, EParamType.GameObjectSciptEvent, "游戏对象脚本事件GameObjectSciptEvent(限定类型ScriptUGUIButtonEvent):", typeof(UGUIButtonScriptEvent))]
#pragma warning restore CS0618 // 类型或成员已过时
        TestScriptParamGameObjectSciptEvent,

        [ScriptName("测试脚本参数游戏对象脚本事件脚本列表", "Test Script Param GameObjectSciptEventFunction")]
        [ScriptParams(1, EParamType.GameObjectSciptEventFunction, "游戏对象脚本事件脚本列表GameObjectSciptEventScriptList:")]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(2, EParamType.GameObjectSciptEventFunction, "游戏对象脚本事件脚本列表GameObjectSciptEventScriptList(限定类型ScriptUGUIButtonEvent):", typeof(UGUIButtonScriptEvent))]
#pragma warning restore CS0618 // 类型或成员已过时
        TestScriptParamGameObjectSciptEventFunction,

        [ScriptName("测试脚本参数游戏对象脚本事件变量值", "Test Script Param GameObjectSciptEventVariableValue")]
        [ScriptParams(1, EParamType.GameObjectSciptEventVariable, "游戏对象脚本事件变量值GameObjectSciptEventVariableValue:")]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(2, EParamType.GameObjectSciptEventVariable, "游戏对象脚本事件变量值(限定类型ScriptUGUIButtonEvent):", typeof(UGUIButtonScriptEvent))]
#pragma warning restore CS0618 // 类型或成员已过时
        TestScriptParamGameObjectSciptEventVariableValue,       

        [ScriptName("测试脚本参数脚本事件", "Test Script Param ScriptEvent")]
        [ScriptParams(1, EParamType.ScriptEvent, "脚本事件ScriptEvent:")]
        [ScriptParams(2, EParamType.ScriptEvent, "脚本事件ScriptEvent:", defaultObject = "脚本UGUI按钮事件")]
        [ScriptParams(3, EParamType.ScriptEvent, "脚本事件ScriptEvent:", defaultObject = 2)]
        TestScriptParamScriptEvent,

        [ScriptName("测试脚本参数矩形", "Test Script Param Rect")]
        [ScriptParams(1, EParamType.Rect, "矩形Rect:")]
        [ScriptParams(2, EParamType.Rect, "矩形Rect:", defaultObject = "1/2/3/4")]
        TestScriptParamRect,

        [ScriptName("测试脚本参数二维向量", "Test Script Param Vector2")]
        [ScriptParams(1, EParamType.Vector2, "二维向量Vector2:")]
        [ScriptParams(2, EParamType.Vector2, "二维向量Vector2:", defaultObject = "1/2")]
        TestScriptParamVector2,

        [ScriptName("测试脚本参数三维向量", "Test Script Param Vector3")]
        [ScriptParams(1, EParamType.Vector3, "三维向量Vector3:")]
        [ScriptParams(2, EParamType.Vector3, "三维向量Vector3:", defaultObject = "1/2/3")]
        TestScriptParamVector3,

        [ScriptName("测试脚本参数四维向量", "Test Script Param Vector4")]
        [ScriptParams(1, EParamType.Vector4, "四维向量Vector4:")]
        [ScriptParams(2, EParamType.Vector4, "四维向量Vector4:", defaultObject = "1/2/3/4")]
        TestScriptParamVector4,

        [ScriptName("测试脚本参数最小最大滑动条", "Test Script Param MinMaxSlider")]
        [ScriptParams(1, EParamType.MinMaxSlider, "最小最大滑动条MinMaxSlider:", -10f, 10f)]
        [ScriptParams(2, EParamType.MinMaxSlider, "最小最大滑动条MinMaxSlider:", -10f, 10f, defaultObject = "-1/1")]
        TestScriptParamMinMaxSlider,

        [ScriptName("测试脚本参数颜色", "Test Script Param Color")]
        [ScriptParams(1, EParamType.Color, "颜色Color:")]
        [ScriptParams(2, EParamType.Color, "颜色Color:", defaultObject = "255/0/0/4")]
        TestScriptParamColor,

        [ScriptName("测试脚本参数界限", "Test Script Param Bounds")]
        [ScriptParams(1, EParamType.Bounds, "界限Bounds:")]
        [ScriptParams(2, EParamType.Bounds, "界限Bounds:", defaultObject = "1/2/3/4/5/6")]
        TestScriptParamBounds,

        [ScriptName("测试脚本参数键码", "Test Script Param KeyCode")]
        [ScriptParams(1, EParamType.KeyCode, "键码KeyCode:")]
        [ScriptParams(2, EParamType.KeyCode, "键码KeyCode:", defaultObject = KeyCode.A)]
        [ScriptParams(3, EParamType.KeyCode, "键码KeyCode:", defaultObject = "B")]
        TestScriptParamKeyCode,

        [ScriptName("测试脚本参数鼠标按钮", "Test Script Param MouseButton")]
        [ScriptParams(1, EParamType.MouseButton, "鼠标按钮MouseButton:")]
        [ScriptParams(2, EParamType.MouseButton, "鼠标按钮MouseButton:", defaultObject = EMouseButtonType.Middle)]
        [ScriptParams(3, EParamType.MouseButton, "鼠标按钮MouseButton:", defaultObject = "Right")]
        TestScriptParamMouseButton,

        [ScriptName("测试脚本参数运行时平台", "Test Script Param RuntimePlatform")]
        [ScriptParams(1, EParamType.RuntimePlatform, "运行时平台RuntimePlatform:")]
        [ScriptParams(2, EParamType.RuntimePlatform, "运行时平台RuntimePlatform:", defaultObject = RuntimePlatform.WindowsPlayer)]
        [ScriptParams(3, EParamType.RuntimePlatform, "运行时平台RuntimePlatform:", defaultObject = "Android")]
        TestScriptParamRuntimePlatform,

        [ScriptName("测试脚本参数Unity资源对象", "Test Script Param UnityAssetObject")]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(1, EParamType.UnityAssetObject, "Unity资源对象UnityAssetObject:")]
        [ScriptParams(2, EParamType.UnityAssetObject, "Unity资源对象UnityAssetObject(限定类型Material):", typeof(Material))]
#pragma warning restore CS0618 // 类型或成员已过时
        TestScriptParamUnityAssetObject,

        [ScriptName("测试脚本参数Unity资源对象类型", "Test Script Param UnityAssetObjectType")]
        [ScriptParams(1, EParamType.UnityAssetObjectType, "Unity资源对象UnityAssetObjectType:")]
        TestScriptParamUnityAssetObjectType,

        [ScriptName("测试脚本参数坐标系类型", "Test Script Param CoordinateType")]
        [ScriptParams(1, EParamType.CoordinateType, "坐标系类型CoordinateType:")]
        [ScriptParams(2, EParamType.CoordinateType, "坐标系类型CoordinateType:", defaultObject = ECoordinateType.Screen)]
        [ScriptParams(3, EParamType.CoordinateType, "坐标系类型CoordinateType:", defaultObject = "ViewPort")]
        TestScriptParamCoordinateType,

        [ScriptName("测试脚本参数文本锚点", "Test Script Param TextAnchor")]
        [ScriptParams(1, EParamType.TextAnchor, "文本锚点TextAnchor:")]
        [ScriptParams(2, EParamType.TextAnchor, "文本锚点TextAnchor:", defaultObject = TextAnchor.MiddleCenter)]
        [ScriptParams(3, EParamType.TextAnchor, "文本锚点TextAnchor:", defaultObject = "LowerRight")]
        TestScriptParamTextAnchor,

        [ScriptName("测试脚本参数矩形Int", "Test Script Param Rect Int")]
        [ScriptParams(1, EParamType.RectInt, "矩形RectInt:")]
        [ScriptParams(2, EParamType.RectInt, "矩形RectInt:", defaultObject = "1/2/3/4")]
        TestScriptParamRectInt,

        [ScriptName("测试脚本参数二维向量Int", "Test Script Param Vector2 Int")]
        [ScriptParams(1, EParamType.Vector2Int, "二维向量Vector2Int:")]
        [ScriptParams(2, EParamType.Vector2Int, "二维向量Vector2Int:", defaultObject = "1/2")]
        TestScriptParamVector2Int,

        [ScriptName("测试脚本参数三维向量Int", "Test Script Param Vector3 Int")]
        [ScriptParams(1, EParamType.Vector3Int, "三维向量Vector3Int:")]
        [ScriptParams(2, EParamType.Vector3Int, "三维向量Vector3Int:", defaultObject = "1/2/3")]
        TestScriptParamVector3Int,

        [ScriptName("测试脚本参数界限Int", "Test Script Param Bounds Int")]
        [ScriptParams(1, EParamType.BoundsInt, "界限BoundsInt:")]
        [ScriptParams(2, EParamType.BoundsInt, "界限BoundsInt:", defaultObject = "1/2/3/4/5/6")]
        TestScriptParamBoundsInt,

        MaxCurrent,
    }
}
