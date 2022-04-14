using System;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCamera;
using XCSJ.EditorCameras;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCamera;

namespace XCSJ.PluginsCameras.Legacies.Inspectors
{
    /// <summary>
    /// 基础相机检查器
    /// </summary>

    [CustomEditor(typeof(BaseCamera), true)]
    [Obsolete("产品功能升级，不推荐用户再使用本功能组件，请使用新的功能组件替代！", false)]
    public class BaseCameraInspector : BaseCameraInspector<BaseCamera> { }

    /// <summary>
    /// 基础检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Obsolete("产品功能升级，不推荐用户再使用本功能组件，请使用新的功能组件替代！", false)]
    public class BaseCameraInspector<T> : BaseInspectorWithLimit<T> where T : BaseCamera
    {
        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("相机扩展功能:", UICommonOption.lableYellowBG);
            try
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);

                #region 设置为初始化相机

                if (!Application.isPlaying && GUILayout.Button(new GUIContent("设置为初始化相机")) && CameraManager.instance)
                {
                    CameraManager.instance.SetInitCamera(targetObject);
                    UICommonFun.MarkSceneDirty();
                }

                #endregion

                //#region 背景幕布

                //var backdrop = targetObject.gameObject.GetComponent<Backdrop>();
                //bool backdropFlag = EditorGUILayout.Toggle(CommonFun.NameTooltip(typeof(Backdrop)), backdrop);
                //if (backdropFlag && !backdrop) targetObject.gameObject.AddComponent<Backdrop>();
                //else if (!backdropFlag && backdrop) RemoveComponent(backdrop);

                //#endregion

                #region 相机碰撞

                var rigidbody = targetObject.gameObject.GetComponent<Rigidbody>();
                bool rigidbodyFlag = EditorGUILayout.Toggle(new GUIContent("相机碰撞", "勾选后为当前相机添加碰撞属性，用于检测碰撞"), rigidbody);
                if (rigidbodyFlag && !rigidbody)
                {
                    rigidbody = targetObject.gameObject.AddComponent<Rigidbody>();
                    if (rigidbody)
                    {
                        rigidbody.useGravity = false;
                        rigidbody.freezeRotation = true;
                    }
                    targetObject.gameObject.AddComponent<BoxCollider>();
                }
                else if (!rigidbodyFlag && rigidbody)
                {
                    RemoveComponent(rigidbody);
                    RemoveComponent(targetObject.gameObject.GetComponent<BoxCollider>());
                }

                #endregion

                OnInspectorGUIOfExtensionFunc();
                onInspectorGUIOfExtensionFunc?.Invoke(this, targetObject);
                CameraManagerInspector.CallOnInspectorGUIOfExtensionFunc(this, targetObject);
            }
            finally
            {
                EditorGUILayout.EndVertical();
                base.OnAfterVertical();
            }
        }

        /// <summary>
        /// 检查器绘制GUI的扩展功能事件
        /// </summary>
        public static event Action<BaseInspector, T> onInspectorGUIOfExtensionFunc;

        /// <summary>
        /// 其他功能界面 -- 用于子类中扩展
        /// </summary>
        public virtual void OnInspectorGUIOfExtensionFunc() { }

        /// <summary>
        /// 创建相机
        /// </summary>
        public static void CreateCamera()
        {
            if (Selection.activeGameObject && Selection.activeGameObject.GetComponent<CameraManager>())
            {
                var m = Selection.activeGameObject.GetComponent<CameraManager>();
                //创建对应的相机
                if (m) m.CreateCamera<T>();
            }
            else CreateComponentWithRequireInternal<Camera>();

            UICommonFun.MarkSceneDirty();
        }

        /// <summary>
        /// 验证创建相机
        /// </summary>
        /// <returns></returns>
        public static bool ValidateCreateCamera()
        {
            if (Selection.activeGameObject)
            {
                if (Selection.activeGameObject.GetComponent<CameraManager>()) return true;
                if (Selection.activeGameObject.GetComponent<BaseCamera>()) return false;
            }
            return ValidateCreateComponentWithRequireInternal<Camera>();
        }
    }
}
