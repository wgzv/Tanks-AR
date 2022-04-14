using System.Collections;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.Extension.CNScripts;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.EditorExtension.CNScripts.Windows
{
    /// <summary>
    /// 层级键扩展查看器
    /// </summary>
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Variable)]
    [XDreamerEditorWindow("中文脚本")]
    public class HierarchyKeyExtensionViewerEditorWindow : XEditorWindowWithScrollView<HierarchyKeyExtensionViewerEditorWindow>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "层级键扩展查看器";

        /// <summary>
        /// 初始化
        /// </summary>
        [MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
        public static void Init() => OpenAndFocus();

        /// <summary>
        /// 层级键模式
        /// </summary>
        public EHierarchyKeyMode hierarchyKeyMode = EHierarchyKeyMode.Get;

        /// <summary>
        /// 绘制GUI
        /// </summary>
        public override void OnGUI()
        {
            hierarchyKeyMode = UICommonFun.Toolbar(hierarchyKeyMode, ENameTip.Image, UICommonOption.Height32);

            EditorGUILayout.Separator();

            if (hierarchyKeyMode != EHierarchyKeyMode.Main)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUILayout.LabelField("NO.", UICommonOption.Width32);
                EditorGUILayout.LabelField("层级键名", UICommonOption.Width60);
                EditorGUILayout.LabelField("同功能层级键名", UICommonOption.Width120);
                EditorGUILayout.LabelField("描述");
                EditorGUILayout.EndHorizontal();
            }

            base.OnGUI();
        }

        /// <summary>
        /// 绘制带滚动视图的GUI
        /// </summary>
        public override void OnGUIWithScrollView()
        {
            if (hierarchyKeyMode == EHierarchyKeyMode.Main)
            {
                DrawExplain();
                return;
            }

            if (!HierarchyKeyExtensionHelper.extensionDatas.TryGetValue(hierarchyKeyMode, out var list)) return;
            for (int i = 0; i < list.Count; i++)
            {
                UICommonFun.BeginHorizontal(i);
                var data = list[i];
#if XDREAMER_EDITION_XDREAMERDEVELOPER
                if (GUILayout.Button((i + 1).ToString(), UICommonOption.buttonToLableStyle, UICommonOption.Width32))
                {
                    EditorHelper.OpenMonoScript(data.info.methodInfo);
                }
#else
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);
#endif
                EditorGUILayout.SelectableLabel(data.hierarchyKeyAttribute.hierarchyKey, UICommonOption.Width60, UICommonOption.Height20);
                EditorGUILayout.SelectableLabel(data.hierarchyKeysString, UICommonOption.Width120, UICommonOption.Height20);
                EditorGUILayout.LabelField(CommonFun.TempContent(data.tip, data.tip));
                UICommonFun.EndHorizontal();
            }
        }

        private void DrawExplain()
        {
            EditorGUILayout.LabelField(explainText, UICommonOption.richText, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        }

        private static string explainText = @"<color=#0080C4FF><size=22><b>层级变量中层级键扩展机制的说明</b></size></color>
<size=16><b>层级变量字符串格式：</b> $变量名/层级键/[层级索引]/[扩展层级键]</size>
<size=16><b>$：</b>变量标识符</size>
<size=16><b>/：</b>表示层级信息的分隔符；</size>
<size=16><b>[]：</b>表示层级索引或扩展层级键时特殊处理分割符，左右括号内的字符串表示特定的信息；如果左右括号内的字符串可分析为整数，则认为是层级索引；如果无法分析为整数且处于层级变量字符串的末尾时，则认为是扩展层级键；</size>
<size=16><b>变量名：</b>有效格式的变量名；</size>
<size=16><b>层级键：</b>可理解为字典类型中的层级名称；</size>
<size=16><b>层级索引：</b>可理解为字典类型中数组子类型的索引信息，有效的层级索引值范围为[1,数组长度](提供正向索引)或[-数组长度,-1](提供逆向索引，即由后向前索引，如-1表示数组中最后一个元素)，仅针对数组类型时有效；0为无效索引；</size>
<size=16><b>扩展层级键：</b>用于针对层级变量在获取或设置等操作时提供额外的功能；扩展层级键只能存在于层级变量字符串的末尾，并且无法将该字符串分析为整数（包括正数与负数）；具体扩展层级键不同名称可具有的功能详见顶部标签页[获取]与[设置]中的详细信息；</size>

<size=18><b>层级变量举例：</b></size>
<size=16><b>层级变量名：</b>Var</size>
<size=16><b>层级变量类型：</b>D   (即字典类型)</size>
<size=16><b>层级变量值: </b>[{""name"":""XDreamer"",""电话号码"":[""11"",""22"",""33""]}]</size>

<size=16><b>层级变量字符串：</b>	$Var/[1]/电话号码/[数量]</size>
<size=16><b>获取值为：</b> 3		解释：层级键'电话号码'中有3个元素（3个元素为'11','22','33'），即数量为3</size>
<size=16><b>设置值为：</b> 2		结果层级变量值: [{""name"":""XDreamer"",""电话号码"":[""11"",""22""]}]	解释：原数量为3，新数量为2，因末尾操作与多删少补的原则，将元素'33'移除，即数量变为2</size>
";
    }
}
