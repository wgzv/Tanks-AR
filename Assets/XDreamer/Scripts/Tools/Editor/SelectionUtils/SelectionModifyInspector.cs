using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Renderers;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.EditorTools.SelectionUtils
{
    /// <summary>
    /// 选择集修改检查器
    /// </summary>
    [CustomEditor(typeof(SelectionModify))]
    public class SelectionModifyInspector : BaseInspector<SelectionModify>
    {
        /// <summary>
        /// 运行时选择集列表
        /// </summary>
        [Name("运行时选择集列表")]
        public bool dispaly = true;

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            dispaly = UICommonFun.Foldout(dispaly, CommonFun.NameTip(GetType(), nameof(dispaly)));
            if (!dispaly) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("游戏对象", "选择集中的游戏对象；本项只读；"));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var selections = XCSJ.PluginCommonUtils.Runtime.Selection.selections;
            for (int i = 0; i < selections.Length; i++)
            {
                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //游戏对象
                EditorGUILayout.ObjectField(selections[i], typeof(GameObject), true);

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }
    }
}
