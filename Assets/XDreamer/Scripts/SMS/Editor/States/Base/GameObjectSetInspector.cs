using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorSMS.NodeKit;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States.Base;
using XCSJ.EditorSMS.States.UGUI;
using static XCSJ.PluginSMS.States.Base.GameObjectSet;
using XCSJ.Scripts;
using XCSJ.Caches;

namespace XCSJ.EditorSMS.States.Base
{
    /// <summary>
    /// 游戏对象集合检查器
    /// </summary>
    [CustomEditor(typeof(GameObjectSet), true)]
    public class GameObjectSetInspector : ObjectSetInspector<GameObjectSet>
    {
        private SerializedProperty objectsSP;        

        private Color orgColor;

        private void AddObjects(params GameObject[] gameObjects)
        {
            if (objectsSP == null || gameObjects == null) return;

            for (int i = gameObjects.Length - 1; i >= 0; --i)
            {
                var gameObject = gameObjects[i];
                if (!gameObject) continue;

                objectsSP.arraySize++;
                objectsSP.GetArrayElementAtIndex(objectsSP.arraySize - 1).objectReferenceValue = gameObject;
            }
        }

        private void RemoveObjects(params GameObject[] gameObjects)
        {
            if (objectsSP == null || gameObjects == null) return;

            for (int i = gameObjects.Length - 1; i >= 0; --i)
            {
                var gameObject = gameObjects[i];
                if (!gameObject) continue;
                if (!stateComponent.Contains(gameObject)) continue;

                for (int j = 0; j < objectsSP.arraySize; ++j)
                {
                    if (objectsSP.GetArrayElementAtIndex(j).objectReferenceValue == gameObject)
                    {
                        UICommonFun.DeleteArrayElementAtIndex(objectsSP, j);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            try
            {
                if (!target) return;

                base.OnEnable();
                objectsSP = serializedObject.FindProperty(ObjectsString);
            }
            catch { }
        }

        /// <summary>
        /// 当横向绘制对象的成员属性之后回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case ObjectsString:
                    {
                        if (arrayElementInfo.index < 0)
                        {
                            EditorGUI.BeginDisabledGroup(arrayElementInfo.isReadonly);

                            EditorGUI.BeginDisabledGroup(!EditorHandler.IsLockInspectorWindow());
                            if (GUILayout.Button(ButtonClickInspector.selectGameObjectGUIContent, EditorStyles.miniButtonLeft, UICommonOption.Width60, UICommonOption.Height18))
                            {
                                AddObjects(Selection.gameObjects);
                            }
                            EditorGUI.EndDisabledGroup();

                            EditorGUI.EndDisabledGroup();
                        }
                        break;
                    }
                default: break;
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        private bool setErrorColor = false;

        /// <summary>
        /// 当纵向绘制对象的成员属性之前回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        public override void OnBeforePropertyFieldVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case ObjectsString:
                    {
                        base.OnBeforePropertyFieldVertical(type, memberProperty, arrayElementInfo);
                        
                        if (!setErrorColor && !stateComponent.DataValidity())
                        {
                            setErrorColor = true;
                            orgColor = GUI.backgroundColor;
                            GUI.backgroundColor = Color.red;
                        }
                        return;
                    }
            }
            base.OnBeforePropertyFieldVertical(type, memberProperty, arrayElementInfo);
        }

        /// <summary>
        /// 当纵向绘制对象的成员属性之后回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        public override void OnAfterPropertyFieldVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(GameObjectSet.includeSelf):
                    {
                        stateComponent.findObjectInRuntime = EditorGUILayout.Toggle(CommonFun.NameTip(stateComponent.GetType(), nameof(GameObjectSet.findObjectInRuntime)), stateComponent.findObjectInRuntime);

                        stateComponent.selectionType = (ESelectionType)UICommonFun.EnumPopup(CommonFun.NameTooltip(target, nameof(stateComponent.selectionType)), stateComponent.selectionType, GUILayout.ExpandWidth(true));

                        switch (stateComponent.selectionType)
                        {
                            case ESelectionType.GameObject:
                            case ESelectionType.GameObjectChildren:
                            case ESelectionType.GameObjectAllChildren:
                            case ESelectionType.GameObjectAndAllParents:
                                {
                                    EditorGUILayout.BeginHorizontal();

                                    stateComponent.batchGameObject = (GameObject)EditorGUILayout.ObjectField(CommonFun.NameTooltip(target, nameof(stateComponent.batchGameObject)), stateComponent.batchGameObject, typeof(GameObject), true);
                                    stateComponent.include = UICommonFun.ButtonToggle(CommonFun.NameTooltip(target, nameof(stateComponent.include)), stateComponent.include, EditorStyles.miniButtonMid, GUILayout.Width(35));
                                    stateComponent.children = UICommonFun.ButtonToggle(CommonFun.NameTooltip(target, nameof(stateComponent.children)), stateComponent.children, EditorStyles.miniButtonRight, GUILayout.Width(35));
                                    if (stateComponent.includeSelf && !stateComponent.batchGameObject)
                                    {
                                        // 在添加自身的情况下，不重复的添加自己
                                        var _selfGO = SMSHelper.FindSMControllerInActiveScene(stateComponent.parent).gameObject;
                                        if (!stateComponent.Contains(_selfGO))
                                        {
                                            stateComponent.batchGameObject = _selfGO;
                                        }
                                    }
                                    if (stateComponent.batchGameObject)
                                    {
                                        if (stateComponent.include)
                                        {
                                            AddObjects(stateComponent.batchGameObject);
                                        }
                                        if (stateComponent.children)
                                        {
                                            AddObjects(CommonFun.GetChildGameObjects(stateComponent.batchGameObject.transform).ToArray());
                                        }

                                        stateComponent.batchGameObject = null;
                                    }

                                    EditorGUILayout.EndHorizontal();

                                    break;
                                }
                            case ESelectionType.Tag:
                                {
                                    DrawTagLayerName(null, DrawTag, () => { return GameObject.FindGameObjectsWithTag(stateComponent.selectedTag); }
                                        );
                                    break;
                                }
                            case ESelectionType.Layer:
                                {
                                    DrawTagLayerName(null, DrawLayer, () => { return FindGameObjectsWithLayer(stateComponent.selectedLayer); }
                                        );
                                    break;
                                }
                            case ESelectionType.Name:
                                {
                                    DrawTagLayerName(null, DrawName, () => { return FindGameObjectsWithName(stateComponent.findName, stateComponent.compareNameRule); }, DrawNameIsVariable);
      
                                    break;
                                }
                            case ESelectionType.TagAndLayer:
                                {
                                    DrawTagLayerName(DrawTag, DrawLayer, () => { return FindGameObjectsWithTagAndLayer(stateComponent.selectedTag, stateComponent.selectedLayer); });
                                    break;
                                }
                            case ESelectionType.TagAndName:
                                {
                                    DrawTagLayerName(DrawTag, DrawName, () => { return FindGameObjectsWithTagAndName(stateComponent.selectedTag, stateComponent.findName, stateComponent.compareNameRule); }, DrawNameIsVariable);
                                    break;
                                }
                            case ESelectionType.LayerAndName:
                                {
                                    DrawTagLayerName(DrawLayer, DrawName, () => { return FindGameObjectsWithLayerAndName(stateComponent.selectedLayer, stateComponent.findName, stateComponent.compareNameRule); }, DrawNameIsVariable);
                                    break;
                                }
                            case ESelectionType.Variable:
                                {
                                    DrawVariable();
                                    break;
                                }
                        }
                        break;
                    }
                
                case nameof(GameObjectSet.objects):
                    {
                        if (setErrorColor)
                        {
                            setErrorColor = false;
                            GUI.backgroundColor = orgColor;
                        }
                        break;
                    }
            }
            base.OnAfterPropertyFieldVertical(type, memberProperty, arrayElementInfo);
        }

        private void DrawTagLayerName(Action beforeVerticalDrawFun, Action beforeAddButtonDrawFun, Func<GameObject[]> findObjects, Action afterVerticalDrawFun = null)
        {
            beforeVerticalDrawFun?.Invoke();

            EditorGUILayout.BeginHorizontal();
            {
                beforeAddButtonDrawFun?.Invoke();

                if (GUILayout.Button(new GUIContent("", EditorIconHelper.GetIconInLib(EIcon.Add)), EditorStyles.miniButtonMid, UICommonOption.WH24x16))
                {
                    AddObjects(findObjects.Invoke());
                    GUI.FocusControl("");
                }
                else if (GUILayout.Button(new GUIContent("", EditorIconHelper.GetIconInLib(EIcon.Delete)), EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                {
                    RemoveObjects(findObjects.Invoke());
                    GUI.FocusControl("");
                }
            }
            EditorGUILayout.EndHorizontal();

            afterVerticalDrawFun?.Invoke();
        }

        private void DrawTag() => stateComponent.selectedTag = EditorGUILayout.TagField(CommonFun.NameTooltip(ESelectionType.Tag), stateComponent.selectedTag);
       
        private void DrawLayer() => stateComponent.selectedLayer = EditorGUILayout.LayerField(CommonFun.NameTooltip(ESelectionType.Layer), stateComponent.selectedLayer);

        private void DrawName()
        {
            var guiContent = CommonFun.NameTooltip(target, nameof(stateComponent.findName));

            if (stateComponent.nameIsVariable)
            {
                stateComponent.findName = DrawVariableValue(stateComponent.findName, guiContent);
            }
            else
            {
                stateComponent.findName = EditorGUILayout.TextField(guiContent, stateComponent.findName);
            }

            stateComponent.compareNameRule = (ECompareNameRule)UICommonFun.EnumPopup(stateComponent.compareNameRule, UICommonOption.Width60);
        }       

        private void DrawNameIsVariable()
        {
            stateComponent.nameIsVariable = EditorGUILayout.Toggle(CommonFun.NameTip(target, nameof(GameObjectSet.nameIsVariable)), stateComponent.nameIsVariable);
        }

        private void DrawVariable() => stateComponent.variable = DrawVariableValue(stateComponent.variable, CommonFun.NameTooltip(target, nameof(stateComponent.variable)));
        
        private static string DrawVariableValue(string varName, GUIContent guiContent = null)
        {
            try
            {
                EditorGUILayout.BeginHorizontal();

                varName = EditorGUILayout.TextField(guiContent, varName);

                var varNames = ScriptManager.instance.GetVariableNames();
                int varIndex = varNames.IndexOf(varName);
                int newIndex = EditorGUILayout.Popup(varIndex, varNames, UICommonOption.Width100);
                if (newIndex != varIndex && newIndex >= 0 && newIndex < varNames.Length)
                {
                    varName = varNames[newIndex];
                }

                return VariableHelper.Format(varName);
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
