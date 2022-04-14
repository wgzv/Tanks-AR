using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginART.Base;
using XCSJ.PluginART.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginART
{
    /// <summary>
    /// ART管理器检查器
    /// </summary>
    [CustomEditor(typeof(ARTManager))]
    public class ARTManagerInspector : BaseManagerInspector<ARTManager>
    {
        /// <summary>
        /// 启用时
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            var manager = this.manager;
            if (manager && manager.hasAccess)
            {
                manager.XGetOrAddComponent<ARTStreamClient>();
            }
        }

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            if (GUILayout.Button("创建[ART流客户端]"))
            {
                manager.XGetOrAddComponent<ARTStreamClient>();
            }

            DrawDetailInfos();
            DrawDetailInfos_Flystick();
        }

        /// <summary>
        /// ART刚体关联列表
        /// </summary>
        [Name("ART刚体关联列表")]
        [Tip("当前场景中所有与ART刚体关联的对象")]
        private static bool _display = true;

        private void DrawDetailInfos()
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(GetType(), nameof(_display)));
            if (!_display) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("ART关联对象", "ART刚体关联的组件；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("关联对象拥有者", "ART刚体关联对象拥有者所在的游戏对象；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("激活启用", "ART刚体关联对象是否处于激活并启用的状态；本项只读；"), UICommonOption.Width60);
            GUILayout.Label(CommonFun.TempContent("数据类型", "用于与ART软件进行数据流通信的数据类型；"), UICommonOption.Width80);
            GUILayout.Label(CommonFun.TempContent("刚体ID", "用于与ART软件进行数据流通信的刚体ID；"), UICommonOption.Width60);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var cache = ComponentCache.Get(typeof(IARTObject), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as MonoBehaviour;
                var link = component as IARTObject;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //ART关联对象
                EditorGUILayout.ObjectField(component, component.GetType(), true);

                //ART关联对象
                var owner = component.GetARTObjectOwnerGameObject();
                EditorGUILayout.ObjectField(owner, typeof(GameObject), true);

                //激活启用
                EditorGUILayout.Toggle(component.isActiveAndEnabled, UICommonOption.Width60);

                //数据类型
                EditorGUI.BeginChangeCheck();
                var dataType = UICommonFun.EnumPopup(link.dataType, UICommonOption.Width80);
                if (EditorGUI.EndChangeCheck())
                {
                    link.dataType = (EDataType)dataType;
                }

                //刚体ID
                EditorGUI.BeginChangeCheck();
                var rigidBodyID = EditorGUILayout.DelayedIntField(link.rigidBodyID, UICommonOption.Width60);
                if (EditorGUI.EndChangeCheck())
                {
                    link.rigidBodyID = rigidBodyID;
                }

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }

        /// <summary>
        /// ARTFlystick手柄关联列表
        /// </summary>
        [Name("ART Flystick手柄关联列表")]
        [Tip("当前场景中所有与ART Flystick手柄关联的对象")]
        private static bool _displayFlystick = true;

        private void DrawDetailInfos_Flystick()
        {
            _displayFlystick = UICommonFun.Foldout(_displayFlystick, CommonFun.NameTip(GetType(), nameof(_displayFlystick)));
            if (!_displayFlystick) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("Flystick对象", "Flystick关联的组件；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("激活启用", "ART刚体关联对象是否处于激活并启用的状态；本项只读；"), UICommonOption.Width60);
            GUILayout.Label(CommonFun.TempContent("编号", "用于与ART软件进行数据流通信的Flystick编号；"), UICommonOption.Width60);
            GUILayout.Label(CommonFun.TempContent("手柄", "用于与ART软件进行数据流通信的Flystick手柄类型；"), UICommonOption.Width80);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var cache = ComponentCache.Get(typeof(IARTFlystickObject), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as MonoBehaviour;
                var link = component as IARTFlystickObject;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //ART关联对象
                EditorGUILayout.ObjectField(component, component.GetType(), true);

                //激活启用
                EditorGUILayout.Toggle(component.isActiveAndEnabled, UICommonOption.Width60);

                //Flystick编号
                EditorGUI.BeginChangeCheck();
                var rigidBodyID = EditorGUILayout.DelayedIntField(link.rigidBodyID, UICommonOption.Width60);
                if (EditorGUI.EndChangeCheck())
                {
                    link.rigidBodyID = rigidBodyID;
                }

                //Flystick手柄
                EditorGUI.BeginChangeCheck();
                var flystick = UICommonFun.EnumPopup(link.flystick, UICommonOption.Width80);
                if (EditorGUI.EndChangeCheck())
                {
                    link.flystick = (EFlystick)flystick;
                }

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }
    }
}