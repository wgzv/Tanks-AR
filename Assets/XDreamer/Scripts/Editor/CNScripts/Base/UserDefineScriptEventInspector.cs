using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.CNScripts.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.EditorExtension.CNScripts.Base
{
    [CustomEditor(typeof(UserDefineScriptEvent))]
    public class UserDefineScriptEventInspector : BaseScriptEventInspector<UserDefineScriptEvent, EUserDefineScriptEventType, UserDefineScriptEventSet>
    {
        [MenuItem(XDreamerMenu.ScriptEvent + UserDefineScriptEvent.Title, false)]
        public static void CreateScriptEvent()
        {
            CreateComponentInternal();
        }

        [MenuItem(XDreamerMenu.ScriptEvent + UserDefineScriptEvent.Title, true)]
        public static bool ValidateCreateScriptEvent()
        {
            return ValidateCreateComponentInternal();
        }

        public string userDefineName = "";

        public override void AddScriptEventOnGUI()
        {
            EditorGUILayout.SelectableLabel("说明: 目前最多支持 " + targetObject.functionList.Count.ToString() + " 个自定义事件", UICommonOption.lableGreenBG);
            //base.DrawAddScriptListGUI();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("事件名", GUILayout.Width(60));
            //eventSelected = EditorGUILayout.Popup(eventSelected, scriptEvent.GetEventNames(Language.languageType), GUILayout.Height(20));

            //eventSelected = (ETType)(object)UICommonFun.EnumPopup((Enum)Enum.Parse(typeof(ETType), eventSelected.ToString()), GUILayout.Height(20));
            userDefineName = EditorGUILayout.TextField(VariableHelper.Format(userDefineName));

            if (GUILayout.Button(CommonFun.NameTip(Attributes.EIcon.Add), UICommonOption.WH24x16))
            {
                targetObject.UpdateFunctionIfNeed();

                int index = targetObject.IndexOfCanAddScriptEvent(userDefineName);
                if (index != -1)
                {
                    var sp = serializedObject.FindProperty(nameof(IFunctionHandle.functionList)).GetArrayElementAtIndex(index);
                    if (sp != null)
                    {
                        sp.FindPropertyRelative(nameof(Function.Enable)).boolValue = true;
                        sp.FindPropertyRelative(nameof(Function.name)).stringValue = userDefineName;
                    }
                }

                //scriptEvent.AddScriptEvent(userDefineName);

                UICommonFun.MarkSceneDirty();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
