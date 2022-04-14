using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorExtension.Base.ProjectSettings;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.EditorExtension.Base.XUnityEditorInternal;
using XCSJ.EditorExtension.XAssets;
using XCSJ.EditorTools;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Extension.Base.XUnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.EditorExtension.Base.Kernel
{
    /// <summary>
    /// 默认编辑器处理器
    /// </summary>
    public class DefaultEditorHandler : InstanceClass<DefaultEditorHandler>, IEditorHandler
    {
        public void Init()
        {
            Preferences.onOptionModified += OnOptionModify;
            UICommonFun.DelayCall(() =>
            {
                OnOptionModify(XDreamerIconOption.weakInstance);
            });

            DefaultPluginsHandler.instance.onEditInspectorScript += OnEditInspectorScript;
            DefaultPluginsHandler.instance.onNeedDelayCall += OnNeedDelayCall;

            XDreamerEvents.onAnyAssetsOrOptionChanged += OnAnyAssetsOrOptionChanged;
        }

        private void OnAnyAssetsOrOptionChanged()
        {
            inputNames = null;
        }

        private void OnOptionModify(Option option)
        {
            if (option is XDreamerIconOption iconOption)
            {
                skinIconMarker = iconOption.GetSkinIconMarker();
            }
        }

        public void ClearConsole() => LogEntries.Clear();

        public EditorWindow OpenInspectorWindow()
        {
            var window = EditorWindow.GetWindow(InspectorWindow.Type);
            if (window)
            {
                window.Show();
                window.Focus();
            }
            return window;
        }

        public bool lockInspectorWindow
        {
            get
            {
                return ActiveEditorTracker.sharedTracker.isLocked;
            }
            set
            {
                ActiveEditorTracker.sharedTracker.isLocked = value;
                //ActiveEditorTracker.sharedTracker.ForceRebuild();
            }
        }

        public void SetScriptExecutionOrder(MonoBehaviour behaviour, int order)
        {
            if (!behaviour) return;
            var monoScript = MonoScript.FromMonoBehaviour(behaviour);
            if (!monoScript) return;
            //限定顺序的范围，并且是在值变更的时候才重新设置值,如果直接设置会导致Unity重新编译，编译后又触发设置，出现了死循环的递归操作！！
            order = Mathf.Clamp(order, -32000, 32000);
            if (order != MonoImporter.GetExecutionOrder(monoScript))
            {
                MonoImporter.SetExecutionOrder(monoScript, order);
            }
        }

        #region 打开首选项窗口PreferencesWindow

        private const string PreferencesNamePrefix = nameof(Preferences) + "/";

        public EditorWindow OpenPreferencesWindow(string name = "")
        {
            try
            {
                return _OpenPreferencesWindow(name);
            }
            finally
            {
#if !UNITY_2018_3_OR_NEWER
                //延时打开对应的配置项！
                //原因：第一次打开时，自定义的配置还未生成，对应的项不存在；延时后，对应项已经生成，则可以成功打开！
                if (!string.IsNullOrEmpty(name))
                {
                    EditorApplicationExtension.DelayCall(0.02f, name, obj => _OpenPreferencesWindow(obj as string));
                }
#endif
            }
        }

        private EditorWindow _OpenPreferencesWindow(string name)
        {
            EditorWindow window = null;
            try
            {
#if UNITY_2018_3_OR_NEWER
                if (!string.IsNullOrEmpty(name) && !name.StartsWith(PreferencesNamePrefix))
                {
                    name = PreferencesNamePrefix + name;
                }
                window = SettingsService.OpenUserPreferences(name);
#else
                PreferencesWindow_LinkType.ShowPreferencesWindow();

                window = EditorWindow.GetWindow(PreferencesWindow_LinkType.Type);
                var referencesWindow = new PreferencesWindow_LinkType(window);

                if (string.IsNullOrEmpty(name)) return window;
                if (name.StartsWith(PreferencesNamePrefix)) name = name.Substring(PreferencesNamePrefix.Length);
                if (string.IsNullOrEmpty(name)) return window;

                var sections = referencesWindow.m_Sections;
                for (int i = 0; i < sections.Count; i++)
                {
                    if (sections[i].content.text == name)
                    {
                        referencesWindow.m_SelectedSectionIndex = i;
                        break;
                    }
                }
#endif
            }
            catch { }
            finally
            {
                if (window)
                {
                    window.Show();
                    window.Focus();
                }
            }
            return window;
        }

        #endregion

        #region 打开项目设置窗口

        private const string ProjectSettingsNamePrefix = "Project/";

        /// <summary>
        /// 打开项目设置窗口
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public EditorWindow OpenProjectSettingsWindow(string name = "")
        {
            try
            {
                return _OpenProjectSettingsWindow(name);
            }
            finally
            {
#if !UNITY_2018_3_OR_NEWER
                //延时打开对应的配置项！
                //原因：第一次打开时，自定义的配置还未生成，对应的项不存在；延时后，对应项已经生成，则可以成功打开！
                if (!string.IsNullOrEmpty(name))
                {
                    EditorApplicationExtension.DelayCall(0.02f, name, obj => _OpenProjectSettingsWindow(obj as string));
                }
#endif
            }
        }


        private EditorWindow _OpenProjectSettingsWindow(string name)
        {
            EditorWindow window = null;
            try
            {
#if UNITY_2018_3_OR_NEWER
                if (!string.IsNullOrEmpty(name) && !name.StartsWith(ProjectSettingsNamePrefix))
                {
                    name = ProjectSettingsNamePrefix + name;
                }
                window = SettingsService.OpenProjectSettings(name);
#else
                //
#endif
            }
            catch { }
            finally
            {
                if (window)
                {
                    window.Show();
                    window.Focus();
                }
            }
            return window;
        }

        #endregion

        public bool CompileMacroExists(string macro)
        {
            return Macro.Defined(macro);
        }

        public bool GUIIsExiting()
        {
            return GUIUtility_LinkType.guiIsExiting;
        }

        #region GetPathOfSkinMarker

        private string _skinIconMarker = "";
        public string skinIconMarker
        {
            get => string.IsNullOrEmpty(_skinIconMarker) ? (_skinIconMarker = XDreamerIconOption.weakInstance.GetSkinIconMarker()) : _skinIconMarker;
            set => _skinIconMarker = value;
        }

        public string GetPathOfSkinMarker(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            var i = path.LastIndexOf('.');
            if (i == -1) return path;

            var skinIconMarker = this.skinIconMarker;
            if (string.IsNullOrEmpty(skinIconMarker)) return path;

            var dotMarker = "." + skinIconMarker;
            var markerExt = dotMarker + path.Substring(i);

            if (path.EndsWith(markerExt))
            {
                return path;
            }
            else
            {
                return path.Insert(i, dotMarker);
            }
        }

        #endregion

        #region GetIconInLib

        public Texture2D GetIconInLib(string name, Vector2Int size, ESkinRule skinRule = ESkinRule.AutoSkin)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            return GetIconFromIconOption(XDreamerIconOption.IconPathCache.Get(GetIconKeyName(name, size)), skinRule);
        }

        public static string GetIconKeyName(string name, Vector2Int size)
        {
            if (size.x <= 0 || size.y <= 0 || size == EditorIconHelper.DefaultIconSize) return name;

            return string.Format("{0}__{1}x{2}", name, size.x, size.y);
        }

        public Texture2D GetIconFromIconOption(string icon, ESkinRule skinRule)
        {
            if (string.IsNullOrEmpty(icon)) return null;

            switch (skinRule)
            {
                case ESkinRule.AutoSkin:
                    {
                        if (_GetIconFromIconOption(GetPathOfSkinMarker(icon)) is Texture2D tex0 && tex0)
                        {
                            return tex0;
                        }
                        break;
                    }
                case ESkinRule.OnlySkin:
                    {
                        if (_GetIconFromIconOption(GetPathOfSkinMarker(icon)) is Texture2D tex0 && tex0)
                        {
                            return tex0;
                        }
                        return null;
                    }
            }

            return _GetIconFromIconOption(icon);
        }

        private static Texture2D _GetIconFromIconOption(string icon)
        {
            string iconPath = XDreamerIconOption.IconPathCache.Get(icon);
            if (string.IsNullOrEmpty(iconPath))
            {
                iconPath = XDreamerIconOption.ToAssetsPath(icon);
            }
            return UICommonFun.LoadFromAssets<Texture2D>(iconPath);
        }

        #endregion

        public void OpenManual(object obj, object tag)
        {
            XAssetStoreWindow.OpenManual(obj, tag);
        }

        public Gradient GetGradient(SerializedProperty serializedProperty)
        {
            return new SerializedProperty_LinkType(serializedProperty).gradientValue;
        }

        public void SetGradient(SerializedProperty serializedProperty, Gradient gradient)
        {
            new SerializedProperty_LinkType(serializedProperty).gradientValue = gradient;
        }

        private string[] inputNames = null;

        public string[] GetInputNames()
        {
            if (inputNames == null || inputNames.Length == 0)
            {
                inputNames = XInputManager.GetAxisNamesDistinct().ToArray();
            }
            return inputNames.ToArray();
        }

        public void DrawArrow(Color color, Vector3 direction, Vector3 center, float arrowSize, float outlineWidth)
        {
            HandlesHelper.DrawArrow(color, direction, center, arrowSize, outlineWidth);
        }

        public GameObject CreatePrefab(string path, GameObject go)
        {
#if UNITY_2018_3_OR_NEWER
            return PrefabUtility.SaveAsPrefabAsset(go, path);
#else            
            return PrefabUtility.CreatePrefab(path, go);
#endif
        }

        public bool HasCustomPropetyDrawer(SerializedProperty serializedProperty)
        {
            return ScriptAttributeUtility.HasCustomPropetyDrawer(serializedProperty);
        }

        private void OnEditInspectorScript(UnityEngine.Object obj)
        {
            if (!obj) return;
            var script = MonoScript.FromScriptableObject(BaseInspector.GetEditor(obj));
            if (script) AssetDatabase.OpenAsset(script);
        }

        private void OnNeedDelayCall(object param, Action<object> action, float delayTime)
        {
            UICommonFun.DelayCall(delayTime, param, action);
        }

        public void DeleteArrayElement(SerializedProperty arraySerializedProperty, SerializedProperty elementSerializedProperty, int index)
        {
            if (UICommonFun.NaturalCompare(Application.unityVersion, "2020.3.20f1c1") >= 0)
            {
                //在此版本后已知问题已修复
            }
            else
            {
                if (elementSerializedProperty != null
                      && elementSerializedProperty.propertyType == SerializedPropertyType.ObjectReference
                      && elementSerializedProperty.objectReferenceValue)
                {
                    //Debug.Log("objectReferenceValue: " + serializedProperty.objectReferenceValue);
                    //这种情况比较特殊，第一次删除会将对象设置为null；为null之后再删除，才会将数组长度做真正的修改！！所以对于非空对象，需要连续2次的删除操作！！
                    arraySerializedProperty.DeleteArrayElementAtIndex(index);
                }
            }
            arraySerializedProperty.DeleteArrayElementAtIndex(index);
        }

        public void VarStringPopup(string varString, Action<string> onMenuItemClicked, params GUILayoutOption[] options) => EditorHelper.VarStringPopup(varString, onMenuItemClicked, options);

        public CategoryList GetToolCategoryListByPurposes(params string[] purposes) => EditorToolsHelper.GetWithPurposes(purposes);
    }
}
