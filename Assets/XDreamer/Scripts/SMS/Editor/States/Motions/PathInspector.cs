using System;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorExtension.Base.Tools;
using XCSJ.EditorSMS.States.Base;
using XCSJ.EditorSMS.States.UGUI;
using XCSJ.EditorSMS.Toolkit;
using XCSJ.Extension.Base.Tweens;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 路径检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PathInspector<T> : MotionInspector<T> where T : Path<T>
    {
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
                case nameof(stateComponent._movePath):
                case nameof(stateComponent._viewPath):
                    {
                        if (arrayElementInfo.index < 0)
                        {
                            EditorGUI.BeginDisabledGroup(arrayElementInfo.isReadonly);

                            //var isLock = EditorHandler.IsLockInspectorWindow();
                            //if (GUILayout.Button(isLock ? CommonFun.NameTip(EIcon.Lock) : CommonFun.NameTip(EIcon.Unlock), EditorStyles.miniButtonLeft, UICommonOption.Width60, UICommonOption.Height18))
                            //{
                            //    EditorHandler.LockInspectorWindow(!isLock);
                            //}

                            EditorGUI.BeginDisabledGroup(!EditorHandler.IsLockInspectorWindow());
                            if (GUILayout.Button(ButtonClickInspector.selectGameObjectGUIContent, EditorStyles.miniButtonLeft, UICommonOption.Width60, UICommonOption.Height18))
                            {
                                AddObjects(memberProperty, Selection.gameObjects.ToList(go => go.transform).ToArray());
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

        /// <summary>
        /// 当纵向绘制数组尺寸之前回调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberProperty"></param>
        /// <param name="arrayElementInfo"></param>
        public override void OnBeforeArraySizeVertical(Type type, SerializedProperty memberProperty, ArrayElementInfo arrayElementInfo)
        {
            switch (memberProperty.name)
            {
                case nameof(stateComponent._movePath):
                case nameof(stateComponent._viewPath):
                    {
                        EditorSerializedObjectHelper.DrawUnityObjectArrayHandleRule(memberProperty);

                        EditorGUILayout.BeginHorizontal();

                        stateComponent.batchGameObject = (Transform)EditorGUILayout.ObjectField(CommonFun.NameTooltip(target, nameof(stateComponent.batchGameObject)), stateComponent.batchGameObject, typeof(Transform), true);
                        stateComponent.include = UICommonFun.ButtonToggle(CommonFun.NameTooltip(target, nameof(stateComponent.include)), stateComponent.include, EditorStyles.miniButtonMid, GUILayout.Width(35));
                        stateComponent.chileren = UICommonFun.ButtonToggle(CommonFun.NameTooltip(target, nameof(stateComponent.chileren)), stateComponent.chileren, EditorStyles.miniButtonRight, GUILayout.Width(35));

                        if (stateComponent.batchGameObject)
                        {
                            if (stateComponent.include)
                            {
                                AddObjects(memberProperty, stateComponent.batchGameObject);
                            }
                            if (stateComponent.chileren)
                            {
                                AddObjects(memberProperty, CommonFun.GetChildGameObjects(stateComponent.batchGameObject).ToList(go => go.transform).ToArray());
                            }
                            stateComponent.batchGameObject = null;
                        }

                        EditorGUILayout.EndHorizontal();
                        break;
                    }
            }
            base.OnBeforeArraySizeVertical(type, memberProperty, arrayElementInfo);
        }

        private void AddObjects(SerializedProperty objectsSP, params Transform[] gameObjects)
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

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();
            if (UICommonFun.ButtonOfMiddlerCenter(CommonFun.TempContent("路径编辑器"), GUILayout.Width(230), GUILayout.Height(24)))
            {
                PathWindow.OpenAndFocus();
            }
        }

        /// <summary>
        /// 获取辅助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            try
            {
                var pathLength = MathU.PathLength(workClip.GetMovePath());
                var realFullPathLength = MathU.PathLength(workClip.GetFullMovePath());
                var standardMoveSpeed = realFullPathLength / workClip.timeLength;
                var moveSpeed = standardMoveSpeed * workClip.parent.speed;
                return info.AppendFormat("\n移动路径长度:\t{0}\n完整移动路径长度:\t{1}\n标准移动速度:\t{2}\n移动速度:\t\t{3}", pathLength, realFullPathLength, standardMoveSpeed, moveSpeed);
            }
            catch
            {
                return info;
            }
        }

        /// <summary>
        /// 绘制Gizmos
        /// </summary>
        /// <param name="path"></param>
        public static void OnDrawGizmos(T path)
        {
            var option = PathWindowOption.weakInstance;
            if (option.overrideDrawGizmos && PathWindow.valid) return;

            if (!path.enable) return;
            var colorBK = Gizmos.color;
            var keyPointColor = option.pathKeyPointBoxColor;
            var pathColor = option.pathLineColor;
            var r = option.keyPointSizeValue;
            try
            {
                Gizmos.color = keyPointColor;
                foreach (var p in path.GetMovePath())
                {
                    Gizmos.DrawWireSphere(p, r * HandleUtility.GetHandleSize(p));
                }

                XGizmos.DrawPath(path.movePathType, path.GetMovePath(), pathColor);

                switch (path.viewRule)
                {
                    case EViewRule.ViewPath:
                        {
                            Gizmos.color = keyPointColor;
                            foreach (var p in path.GetViewPath())
                            {
                                Gizmos.DrawWireSphere(p, r * HandleUtility.GetHandleSize(p));
                            }

                            XGizmos.DrawPath(path.viewPathType, path.GetViewPath(), pathColor);
                            break;
                        }
                }

            }
            catch { }
            finally
            {
                Gizmos.color = colorBK;
            }
        }
    }
}
