using UnityEditor;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.PluginXBox;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXBox.Tools;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
#endif

namespace XCSJ.EditorXBox
{
    /// <summary>
    /// <see cref="XBoxManager"/>检查器
    /// </summary>
    [CustomEditor(typeof(XBoxManager))]
    public class XBoxManagerInspector : BaseManagerInspector<XBoxManager>
    {
        #region 编译宏

        /// <summary>
        /// 宏
        /// </summary>
        public static readonly Macro XDREAMER_XBOX_INPUT = new Macro(nameof(XDREAMER_XBOX_INPUT), BuildTargetGroup.Standalone);

        /// <summary>
        /// 初始化宏
        /// </summary>
        //[InitializeOnLoadMethod]
        //[Macro]
        public static void InitMacro()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (XBoxManagerAssetGenerator.CheckAxisPresets())
            {
                XDREAMER_XBOX_INPUT.DefineIfNoDefined();
            }
            else
#endif
            {
                XDREAMER_XBOX_INPUT.UndefineWithSelectedBuildTargetGroup();
            }
        }

        #endregion

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
#if ENABLE_INPUT_SYSTEM
            validXBoxDevice = XBoxHelper.HasXBox360();
            InputSystem.onDeviceChange += OnDeviceChange;
#else
            checkXBoxDevice = true;
            CheckDevice();
#endif
            validXBoxInput = XBoxManagerAssetGenerator.CheckAxisPresets();
            base.OnEnable();
        }

        public override void OnDisable()
        {
            base.OnDisable();
#if ENABLE_INPUT_SYSTEM
            InputSystem.onDeviceChange -= OnDeviceChange;
#else
            checkXBoxDevice = false;
#endif
        }

#if ENABLE_INPUT_SYSTEM

        private void OnDeviceChange(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
        {
            if (inputDevice is XInputControllerWindows)
            {
                validXBoxDevice = XBoxHelper.HasXBox360();
            }
        }

#else

        private void CheckDevice()
        {
            if (checkXBoxDevice)
            {
                validXBoxDevice = XBoxHelper.HasXBox360();
                if (lastValidXBoxDevice && !validXBoxDevice)
                {
                    Debug.LogWarning("XBox设备连接已断开！");
                }
                else if (!lastValidXBoxDevice && validXBoxDevice)
                {
                    Debug.Log("XBox设备已连接！");
                }
                lastValidXBoxDevice = validXBoxDevice;
                UICommonFun.DelayCall(CheckDevice, 5);
            }
        }
        private static bool checkXBoxDevice = false;
        private bool lastValidXBoxDevice = false;

#endif

        private bool validXBoxDevice = false;
        private bool validXBoxInput = false;

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();
            if (!validXBoxDevice)
            {
                UICommonFun.RichHelpBox("<color=red>未找到有效的XBox设备！</color>", MessageType.Error);
            }
            if (GUILayout.Button("检测XBox设备"))
            {
                validXBoxDevice = XBoxHelper.HasXBox360();
                if (validXBoxDevice)
                {
                    Debug.Log("XBox设备已连接！");
                }
                else
                {
                    Debug.LogError("未找到有效的XBox设备！");
                }
            }


#if ENABLE_LEGACY_INPUT_MANAGER || !ENABLE_INPUT_SYSTEM

            if (validXBoxInput)
            {
#if ENABLE_INPUT_SYSTEM
                UICommonFun.RichHelpBox("新版输入系统已启用！XBox手柄不再推荐配置到输入管理器！推荐移除对应配置！", MessageType.Warning);
#endif
                if (GUILayout.Button("检测输入管理器中XBox配置"))
                {
                    validXBoxInput = XBoxManagerAssetGenerator.CheckInputManagerAsset();
                }
            }
            else
            {
#if ENABLE_INPUT_SYSTEM
                if (GUILayout.Button("配置XBox手柄到输入管理器（*不推荐*）"))
#else
                UICommonFun.RichHelpBox("<color=red>XBox手柄未配置到输入管理器！</color>", MessageType.Error);
                if (GUILayout.Button("配置XBox手柄到输入管理器"))
#endif
                {
                    validXBoxInput = XBoxManagerAssetGenerator.GenerateInputManagerAsset();
                }
            }

#endif

                if (GUILayout.Button("打开[输入管理器]项目配置"))
            {
#if UNITY_2019_1_OR_NEWER
                EditorHelper.OpenProjectSettingsWindow("Input Manager");
#else
                EditorHelper.OpenProjectSettingsWindow("Input");
#endif
            }

#if UNITY_2019_1_OR_NEWER && ENABLE_INPUT_SYSTEM
            if (GUILayout.Button("打开[输入系统包]项目配置"))
            {
                EditorHelper.OpenProjectSettingsWindow("Input System Package");
            }
#endif

            DrawXBoxDetailInfos();
        }

        /// <summary>
        /// XBox功能组件列表
        /// </summary>
        [Name("XBox功能组件列表")]
        [Tip("当前场景中所有使用XBox功能的组件对象")]
        private static bool _displayXBoxs = true;

        private void DrawXBoxDetailInfos()
        {
            EditorGUILayout.Separator();
            _displayXBoxs = UICommonFun.Foldout(_displayXBoxs, CommonFun.NameTip(GetType(), nameof(_displayXBoxs)));
            if (!_displayXBoxs) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("XBox功能组件", "XBox功能组件游戏对象；本项只读；"));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var cache = ComponentCache.Get(typeof(IXBox), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i];

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //XBox功能组件
                EditorGUILayout.ObjectField(component, component.GetType(), true);

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }
    }
}
