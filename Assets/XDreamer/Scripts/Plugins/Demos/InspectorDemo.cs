using UnityEngine;
using System.Collections;
using XCSJ.PluginCommonUtils;
using System;
using XCSJ.Attributes;
using XCSJ.Algorithms;

namespace XCSJ.Extension.Demos
{
    [Serializable]
    public class Node
    {
        [Name("整形值0")]
        public int intValue0;

        [Group("Node内分组1")]
        [Name("布尔值1")]
        public bool boolValue1;

        [Name("整形值1")]
        public int intValue1;

        [Group("Node内分组2")]
        [Name("布尔值2")]
        public bool boolValue2;

        [Name("整形值2")]
        public int intValue2;
    }

    /// <summary>
    /// 检查器案例
    /// </summary>
    [Name("检查器案例")]
    [RequireManager(typeof(ExtensionExampleManager))]
    public class InspectorDemo : MB
    {
        [Name("渲染OnAfterScript")]
        public bool drawOnAfterScript = false;

        [Name("渲染OnBeforeVertical")]
        public bool drawOnBeforeVertical = false;

        [Name("渲染OnAfterVertical")]
        public bool drawOnAfterVertical = false;

        [Name("渲染OnBeforePropertyFieldVertical")]
        public bool drawOnBeforePropertyFieldVertical = false;

        [Name("渲染OnAfterPropertyFieldVertical")]
        public bool drawOnAfterPropertyFieldVertical = false;

        [Name("渲染OnBeforePropertyFieldHorizontal")]
        public bool drawOnBeforePropertyFieldHorizontal = false;

        [Name("渲染OnAfterPropertyFieldHorizontal")]
        public bool drawOnAfterPropertyFieldHorizontal = false;

        [Name("渲染OnBeforeGroupVertical")]
        public bool drawOnBeforeGroupVertical = false;

        [Name("渲染OnAfterGroupHorizontal")]
        public bool drawOnAfterGroupHorizontal = false;

        [Name("渲染OnAfterUnityEngineObjectHorizontal")]
        public bool drawOnAfterUnityEngineObjectHorizontal = false;

        [Group("分组0 -- GroupAttribute")]
        [Name("整形值0-1")]
        public int intValue0_1;

        [Name("整形值0-2")]
        public int intValue0_2;

        [EndGroup(true)]
        [Name("整形值0-3")]
        [Tip("强制结束分组")]
        public int intValue0_3;
        
        [Name("整形值0-4")]
        public int intValue0_4;

        [Group("分组1 -- ValidityCheckAttribute")]
        [Name("整形值1-1")]
        [Tip("当前整形值1的相关说明，例如: 当前值不可为0")]
        [ValidityCheck(EValidityCheckType.NotZero, invalidExplanation = "当前值不可为0")]
        public int intValue1_1;

        [Name("整形值1-2")]
        [ValidityCheck(EValidityCheckType.GreaterEqual | EValidityCheckType.And, 1, "intValue1_2", EValidityCheckType.Less, 5, invalidExplanation = "当前值范围 [1,5) ")]
        public int intValue1_2;

        [Name("整形值1-3")]
        [ValidityCheck(EValidityCheckType.LessEqual | EValidityCheckType.Or, 0, "intValue1_3", EValidityCheckType.GreaterEqual, 5, invalidExplanation = "当前值范围 (-∞,0] 或者 [5,+∞) ")]
        public int intValue1_3;

        [Name("整形值1-4")]
        [ValidityCheck(EValidityCheckType.NotEqual | EValidityCheckType.And, 1, "stringValue1_1", EValidityCheckType.NotNullOrEmpty, null, invalidExplanation = "当前值不可为1 且 字符串值1 不可为空字符串")]
        public int intValue1_4;

        [Name("字符串值1-1")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string stringValue1_1;

        [Name("游戏对象1-1")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public GameObject goValue1_1;

        [Group("分组2 -- 与Untiy自带特性冲突 且 相互也冲突", textColor = EColor.Blue, expandTextColor = EColor.Red, defaultIsExpanded = false)]
        [Name("字符串值2-1")]
        [Tip("编辑态、运行态都是只读的；且不可以复制；")]
        [Readonly()]
        public string stringValue2_1;

        [Name("字符串值2-2")]
        [Tip("编辑态是只读的,运行态可以修改值；可以复制；")]
        [Readonly(false, EEditorMode.Edit)]
        public string stringValue2_2;

        [Name("字符串值2-3")]
        [Tip("输入的为密码文本 * ")]
        [PasswordText]
        public string stringValue2_3;

        [Name("字符串值2-4")]
        [Tip("下拉选择用户自定义函数")]
        [UserDefineFun]
        public string stringValue2_4;

        [Name("字符串值2-5")]
        [Tip("下拉选择全局变量")]
        [GlobalVariable]
        public string stringValue2_5;

        [Name("二维向量值2-1")]
        [Tip("二维向量范围: 0 <= x <= y <= 5")]
        [LimitRange(0, 5)]
        public Vector2 vector2Value2_1;

        [Group("分组3 -- 总是展开",alwaysExpand = true)]
        [Name("节点3-1")]
        public Node node3_1 = new Node();

        [Name("布尔值3-1")]
        public bool boolValue3_1;

        [Group("分组4 -- 背景色 & FoldoutAttribute", backgroundColor32 = 0x0000ff80)]
        [Name("节点4-1")]
        public Node node4_1 = new Node();

        [Foldout(backgroundColor32 = 0xff00ff80)]
        [Name("节点4-2")]
        public Node node4_2 = new Node();

        [Foldout(backgroundColor32 = 0x00ffff80)]
        [Name("整形值数组4-1")]
        public int[] intValueArray4_1;

        [Name("布尔值4-1")]
        public bool boolValue4_1;

        [Group("分组5 -- HideInSuperInspectorAttribute")]
        [Name("整形值5-1")]
        public int intValue5_1;

        [Name("布尔值5-1")]
        public bool boolValue5_1;

        [Name("游戏对象5-1")]
        public GameObject goValue5_1;

        [Name("整形值5-2")]
        [HideInSuperInspector]
        public int intValue5_2;

        [Name("整形值5-3")]
        [Tip("整形值5-1 < 1 时当前参数隐藏")]
        [HideInSuperInspector(nameof(intValue5_1), EValidityCheckType.Less, 1)]
        public int intValue5_3;

        [Name("整形值5-4")]
        [Tip("整形值5-1 < 1 且 布尔值5-1 == False 时当前参数隐藏")]
        [HideInSuperInspector("intValue5_1", EValidityCheckType.Less | EValidityCheckType.And, 1, "boolValue5_1", EValidityCheckType.False)]
        public int intValue5_4;

        [Name("整形值5-5")]
        [Tip("整形值5-1 < 1 或 布尔值5-1 == False 时当前参数隐藏")]
        [HideInSuperInspector("intValue5_1", EValidityCheckType.Less | EValidityCheckType.Or, 1, "boolValue5_1", EValidityCheckType.False)]
        public int intValue5_5;

        [Name("整形值5-6")]
        [Tip("布尔值5-1 == False 且 游戏对象5-1 == null 时当前参数隐藏")]
        [HideInSuperInspector("boolValue5_1", EValidityCheckType.False | EValidityCheckType.And, 1, "goValue5_1", EValidityCheckType.Null)]
        public int intValue5_6;

        [Group("分组6 -- DisallowResizeArrayAttribute")]
        [Name("整形值6-1")]
        public int intValue6_1;

        [Name("整形值数组6-1")]
        public int[] intValueArray6_1;

        [Name("整形值数组6-2")]
        [DisallowResizeArray]
        public int[] intValueArray6_2 = new int[2] { 0, 1 };

        [Name("整形值数组6-3")]
        [DisallowResizeArray(DisallowResizeArrayAttribute.EDisallowResizeArrayType.CanInsert)]
        public int[] intValueArray6_3 = new int[2] { 0, 1 };

        [Name("整形值数组6-4")]
        [DisallowResizeArray(DisallowResizeArrayAttribute.EDisallowResizeArrayType.CanDelete)]
        public int[] intValueArray6_4 = new int[2] { 0, 1 };
         
        [Group("分组7 -- OnlyMemberElementsAttribute")]
        [Name("整形值7-1")]
        public int intValue7_1;

        [Name("整形值数组7-1")]
        public int[] intValueArray7_1;

        [Name("整形值数组7-2")]
        [OnlyMemberElements]
        public int[] intValueArray7_2;

        [Name("节点7-1")]
        public Node node7_1 = new Node();

        [Name("节点7-2")]
        [OnlyMemberElements]
        public Node node7_2 = new Node();

        [Name("整形值7-2")]
        public int intValue7_2;

        [Name("整形值数组7-3")]
        public int[] intValueArray7_3;

        [Name("整形值数组7-4")]
        [OnlyMemberElements]
        public int[] intValueArray7_4;
                
        [Group("分组8 -- ShowMemberOfUnityEngineObjectAttribute")]
        [Name("整形值8-1")]
        public int intValue8_1;

        [Name("脚本化对象8-1")]
        public ScriptableObject scriptableObject8_1;

        [Name("脚本化对象8-2")]
        [ShowMemberOfUnityEngineObject]
        public ScriptableObject scriptableObject8_2;

        [Group("分组9 -- ArrayElementDispalyAttribute")]
        [Name("整形值9-1")]
        public int intValue9_1;

        [Name("字符串值数组9-1")]
        [ArrayElementDispaly(EArrayElementDispalyType.TextArea)]
        public string[] stringValueArray9_1;

        [Name("二维向量值数组9-1")]
        [ArrayElementDispaly(EArrayElementDispalyType.LimitRange, 0f, 5f)]
        public Vector2[] vector2ValueArray9_1;

        [Name("整形值数组9-1")]
        [ArrayElementDispaly(EArrayElementDispalyType.Range, 0, 5)]
        public int[] intValueArray9_1;

        [Name("浮点数值数组9-1")]
        [ArrayElementDispaly(EArrayElementDispalyType.Range, 0f, 5f)]
        public float[] floatValueArray9_1;

        [Name("字符串值数组9-2")]
        [ArrayElementDispaly(EArrayElementDispalyType.UserDefine)]
        public float[] stringValueArray9_2;

        [Name("字符串值数组9-3")]
        [ArrayElementDispaly(EArrayElementDispalyType.PasswordText)]
        public float[] stringValueArray9_3;

        [Name("序列化对象数组9-3")]
        [ArrayElementDispaly(EArrayElementDispalyType.ShowMemberOfUnityEngineObject)]
        public SO[] scriptableObjectArray9_1;
    }
}
