using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorXGUI;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.LineNotes;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.EditorTools.Inspectors
{
    [CustomEditor(typeof(LineNote))]
    [CanEditMultipleObjects]
    public class LineNoteInspector : BaseInspectorWithLimit<LineNote>
    {
        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(LineNote.target):
                    {
                        if (GUILayout.Button(UICommonOption.Insert, UICommonOption.WH24x16))
                        {
                            CreateNoteGameObjectAndFocus(CommonFun.Name(typeof(LineNote), nameof(LineNote.target)), memberProperty, targetObject.gameObject);
                        }
                        break;
                    }
                case nameof(LineNote.lineStyle):
                    {
                        EditorGUI.BeginDisabledGroup(targetObject.lineStyle);
                        if (GUILayout.Button(UICommonOption.Insert, UICommonOption.WH24x16))
                        {
                            memberProperty.objectReferenceValue = targetObject.gameObject.AddComponent<LineStyle>();
                        }
                        EditorGUI.EndDisabledGroup();
                        break;
                    }
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        public override void OnAfterPropertyFieldVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(LineNote._RenderMode):
                    {
                        if (targetObject._RenderMode == ERenderMode.LineRenderer)
                        {
                            if (targetObject.lineStyle.width==0)
                            {
                                UICommonFun.RichHelpBox("渲染模式等于线渲染器时线样式宽度必须大于0", MessageType.Error);
                            }
                        }
                        break;
                    }
            }
            base.OnAfterPropertyFieldVertical(type, memberProperty, arrayElementInfo);
        }

        protected static GameObject CreateNoteGameObjectAndFocus(string name, SerializedProperty memberProperty, GameObject parent)
        {
            var go = new GameObject(name);
            if (go)
            {
                memberProperty.objectReferenceValue = go;
                UndoHelper.RegisterCreatedObjectUndo(go);
                if (parent) UndoHelper.RecordSetTransformParent(go.transform, parent.transform);
                EditorGUIUtility.PingObject(go);
            }
            return go;
        }
    }

    [CustomEditor(typeof(LineNote2D))]
    public class LineNote2DInspector : LineNoteInspector { }

    [CustomEditor(typeof(LineNote3D))]
    public class LineNote3DInspector : LineNoteInspector
    {
        protected LineNote3D lineNote3D => targetObject as LineNote3D;

        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(LineNote3D.endTarget):
                    {
                        if (GUILayout.Button(UICommonOption.Insert, UICommonOption.WH24x16))
                        {
                            CreateNoteGameObjectAndFocus(CommonFun.Name(typeof(LineNote3D), nameof(LineNote3D.endTarget)), memberProperty, targetObject.target);
                        }
                        break;
                    }
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        public void OnSceneGUI()
        {
            if (lineNote3D.target && lineNote3D.endTarget)
            {
                var orgColor = Handles.color;
                if(lineNote3D.lineStyle) Handles.color = lineNote3D.lineStyle.color;
                Handles.DrawPolyLine(lineNote3D.beginPoint, lineNote3D.endPoint);
                Handles.color = orgColor;
            }
        }
    }

    [CustomEditor(typeof(UGUILineNote3D))]
    [CanEditMultipleObjects]
    public class UGUILineNote3DInspector : LineNote3DInspector
    {
        private ENote3DError note3DError = ENote3DError.None;

        private UGUILineNote3D line => target as UGUILineNote3D;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            CheckUIValid();
        }

        public override void OnAfterPropertyFieldHorizontal(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(UGUILineNote3D.rectTransform):
                    {
                        if (GUILayout.Button(UICommonOption.Insert, UICommonOption.WH24x16) && target is UGUILineNote3D line)
                        {
                            line.rectTransform = UGUILineHelper.CreateButtonNote();
                        }
                        break;
                    }
                default:
                    break;
            }
            base.OnAfterPropertyFieldHorizontal(type, memberProperty, arrayElementInfo);
        }

        public override void OnAfterPropertyFieldVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(UGUILineNote3D.rectTransform):
                    {
                        switch (note3DError)
                        {
                            case ENote3DError.None:
                                break;
                            case ENote3DError.ParentIsCanva:
                                {
                                    if (line.rectTransform)
                                    {
                                        UICommonFun.RichHelpBox("UI对象["+line.rectTransform.name + "]的父对象不能是" + nameof(Canvas), MessageType.Error);
                                    }
                                    break;
                                }
                            case ENote3DError.ParentIsNotStretchHV:
                                {
                                    UICommonFun.RichHelpBox("父对象的数据设置错误", MessageType.Error);
                                    if (GUILayout.Button("纠正", GUILayout.Width(60)))
                                    {
                                        if (line.rectTransform)
                                        {
                                            StretchHVParent(line.rectTransform.parent as RectTransform);
                                        }
                                        CheckUIValid();
                                    }
                                    break;
                                }
                        }
                        break;
                    }
            }
            base.OnAfterPropertyFieldVertical(type, memberProperty, arrayElementInfo);
        }

        /// <summary>
        /// 标注3D
        /// </summary>
        protected enum ENote3DError
        {
            /// <summary>
            /// 无
            /// </summary>
            None,

            /// <summary>
            /// 父节点为画布
            /// </summary>
            ParentIsCanva,

            /// <summary>
            /// 父节点非缩放
            /// </summary>
            ParentIsNotStretchHV,
        }

        private void CheckUIValid()
        {
            note3DError = ENote3DError.None;
            if (line && line.rectTransform)
            {
                if (line.rectTransform.parent is RectTransform parentRectTransform)
                {
                    if (parentRectTransform.GetComponent<Canvas>())
                    {
                        note3DError = ENote3DError.ParentIsCanva;
                    }

                    if (note3DError == ENote3DError.None)
                    {
                        CheckUIAndParentNotStretchHV(parentRectTransform, out bool isNotStretchHV);
                        if (isNotStretchHV)
                        {
                            note3DError = ENote3DError.ParentIsNotStretchHV;
                        }
                    }
                }
            }
        }

        private void CheckUIAndParentNotStretchHV(RectTransform rectTransform, out bool isNotStretchHV)
        {
            if (!rectTransform || rectTransform.GetComponent<Canvas>())
            {
                isNotStretchHV = false;
                return;
            }
            if (rectTransform.anchorMin != Vector2.zero || rectTransform.anchorMax != Vector2.one
                || rectTransform.offsetMin != Vector2.zero || rectTransform.offsetMax != Vector2.zero)
            {
                isNotStretchHV = true;
            }
            else
            {
                CheckUIAndParentNotStretchHV(rectTransform.parent as RectTransform, out isNotStretchHV);
            }
        }

        private void StretchHVParent(RectTransform rectTransform)
        {
            if (!rectTransform || rectTransform.GetComponent<Canvas>())
            {
                return;
            }
            else
            {
                rectTransform.XStretchHV();
                StretchHVParent(rectTransform.parent as RectTransform);
            }
        }
    }

    public static class UGUILineHelper
    {
        private const string notePanelName = "批注集";

        public static RectTransform CreateButtonNote()
        {
            GameObject parent = null;
            var canvas = EditorXGUIHelper.FindOrCreateRootCanvas();
            var item = canvas.transform.Find(notePanelName);
            if (item)
            {
                parent = item.gameObject;
            }
            else
            {
                parent = EditorHandler.Create(notePanelName, canvas.transform);
            }

            if (parent && XCSJ.EditorXGUI.ToolsMenu.CreateUIWithStyle<Button>() is Button btn && btn)
            {
                var btnGO = btn.gameObject;
                btnGO.XSetParent(parent);
                btnGO.XModifyProperty(() => GameObjectUtility.EnsureUniqueNameForSibling(btnGO)); 
                btnGO.GetComponent<RectTransform>().XCenterHV();
                return btn.GetComponent<RectTransform>();
            }
            return null;
        }

    }

}
