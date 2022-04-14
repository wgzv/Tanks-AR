using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.EditorTools.Windows;
using XCSJ.Extension.Base.Net;
using XCSJ.Net;
using XCSJ.Net.Tcp.Threading;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginNetInteract;
using XCSJ.PluginNetInteract.Tools;

namespace XCSJ.EditorNetInteract
{
    /// <summary>
    /// 网络交互管理器检查器
    /// </summary>
    [CustomEditor(typeof(NetInteractManager))]
    public class NetInteractManagerInspector : BaseManagerInspector<NetInteractManager>
    {
        private static CategoryList categoryList = null;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            if (categoryList == null) categoryList = EditorToolsHelper.GetWithPurposes(nameof(NetInteractManager));
        }

        /// <summary>
        /// 当纵向绘制之后回调
        /// </summary>
        public override void OnAfterVertical()
        {
            base.OnAfterVertical();

            EditorGUILayout.Separator();
            DrawServersInternal();
            DrawClientsInternal();

            EditorGUILayout.Separator();
            categoryList.DrawVertical();
        }

        /// <summary>
        /// 服务器列表
        /// </summary>
        [Name("服务器列表")]
        [Tip("当前场景中所有的服务器对象")]
        private static bool _displayServers = true;

        /// <summary>
        /// 客户端列表
        /// </summary>
        [Name("客户端列表")]
        [Tip("当前场景中所有的客户端对象")]
        private static bool _displayClients = true;

        private void DrawServersInternal()
        {
            _displayServers = UICommonFun.Foldout(_displayServers, CommonFun.NameTip(GetType(), nameof(_displayServers)));
            if (!_displayServers) return;
            DrawServers();
        }

        internal static void DrawServers()
        {
            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("服务器", "服务器所在的游戏对象；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("状态", "服务器状态；"), UICommonOption.Width48);
            GUILayout.Label(CommonFun.TempContent("监听端口", "服务器的监听端口；"), UICommonOption.Width48);
            GUILayout.Label(CommonFun.TempContent("客户端数", "已连入服务器的客户端数量；"), UICommonOption.Width48);
            GUILayout.Label(CommonFun.TempContent("操作", "对服务器进行控制；"), UICommonOption.Width32x3);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var isPlaying = Application.isPlaying;
            var components = ComponentCache.Get(typeof(Server), true).components;
            var count = components.Length;
            for (int i = 0; i < count; i++)
            {
                var component = components[i] as Server;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //服务器
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //服务器监听状态
                EditorGUILayout.LabelField(component.isRunning ? "运行中" : "已停止", UICommonOption.Width48);

                //监听端口
                if(component.isRunning)
                {
                    EditorGUILayout.LabelField(component.validListenPort.ToString(), UICommonOption.Width48);
                }
                else
                {
                    component.port = EditorGUILayout.IntField(component.port, UICommonOption.Width48);
                }

                //客户端数
                EditorGUILayout.LabelField(component._server.clients.count.ToString(), UICommonOption.Width48);

                //操作
                EditorGUI.BeginDisabledGroup(!isPlaying || !component.gameObject.activeInHierarchy);
                {
                    if (GUILayout.Button("启动", EditorStyles.miniButtonLeft, UICommonOption.Width32))
                    {
                        component.StartServerAndTrySyncObject();
                    }
                    if (GUILayout.Button("停止", EditorStyles.miniButtonMid, UICommonOption.Width32))
                    {
                        component.StopServerAndSyncObject();
                    }
                    if (GUILayout.Button("重启", EditorStyles.miniButtonRight, UICommonOption.Width32))
                    {
                        component.RestartServerAndTrySyncObject();
                    }
                }
                EditorGUI.EndDisabledGroup();

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }

        private void DrawClientsInternal()
        {
            _displayClients = UICommonFun.Foldout(_displayClients, CommonFun.NameTip(GetType(), nameof(_displayClients)));
            if (!_displayClients) return;
            DrawClients();
        }

        internal static void DrawClients()
        {           
            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("客户端", "客户端所在的游戏对象；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("状态", "客户端状态；"), UICommonOption.Width48);
            GUILayout.Label(CommonFun.TempContent("地址", "服务器地址；"), UICommonOption.Width100);
            GUILayout.Label(CommonFun.TempContent("端口", "服务器端口；"), UICommonOption.Width48);
            GUILayout.Label(CommonFun.TempContent("模式", "连接模式；"), UICommonOption.Width60);
            GUILayout.Label(CommonFun.TempContent("操作", "对客户端进行控制；"), UICommonOption.Width32x3);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var isPlaying = Application.isPlaying;
            var components = ComponentCache.Get(typeof(Client), true).components;
            var count = components.Length;
            for (int i = 0; i < count; i++)
            {
                var component = components[i] as Client;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //客户端
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //状态
                EditorGUILayout.LabelField(CommonFun.Name(component._client.clientState), UICommonOption.Width48);

                //地址-端口-模式
                if (component.isConnected)
                {
                    EditorGUILayout.LabelField(component.serverAddress, UICommonOption.Width100);
                    EditorGUILayout.LabelField(component.serverPort.ToString(), UICommonOption.Width48);
                    EditorGUILayout.LabelField(CommonFun.Name(component.connectMode), UICommonOption.Width60);
                    EditorGUILayout.LabelField(CommonFun.Name(component.netQAMode), UICommonOption.Width48);
                }
                else
                {
                    component.serverAddress = EditorGUILayout.TextField(component.serverAddress, UICommonOption.Width100);
                    component.serverPort = EditorGUILayout.IntField(component.serverPort, UICommonOption.Width48);
                    component.connectMode = (EConnectMode)UICommonFun.EnumPopup(component.connectMode, UICommonOption.Width60);
                    component.netQAMode = (ENetQAMode)UICommonFun.EnumPopup(component.netQAMode, UICommonOption.Width48);
                }

                //操作
                EditorGUI.BeginDisabledGroup(!isPlaying || !component.gameObject.activeInHierarchy);
                {
                    if (GUILayout.Button("启动", EditorStyles.miniButtonLeft, UICommonOption.Width32))
                    {
                        component.ConnectAndTrySyncObject();
                    }
                    if (GUILayout.Button("停止", EditorStyles.miniButtonMid, UICommonOption.Width32))
                    {
                        component.CloseAndSyncObject();
                    }
                    if (GUILayout.Button("重启", EditorStyles.miniButtonRight, UICommonOption.Width32))
                    {
                        component.ReconnectAndTrySyncObject();
                    }
                }
                EditorGUI.EndDisabledGroup();

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }
    }

    /// <summary>
    /// 服务器列表查看器
    /// </summary>
    [ToolObjectViewerEditor(typeof(Server), true)]
    public class ServerListViewer : ToolObjectViewerEditor
    {
        /// <summary>
        /// 绘制GUI
        /// </summary>
        public override void OnGUI()
        {
            //base.OnGUI();
            NetInteractManagerInspector.DrawServers();
        }
    }

    /// <summary>
    /// 客户端列表查看器
    /// </summary>
    [ToolObjectViewerEditor(typeof(Client), true)]
    public class ClientListViewer : ToolObjectViewerEditor
    {
        /// <summary>
        /// 绘制GUI
        /// </summary>
        public override void OnGUI()
        {
            //base.OnGUI();
            NetInteractManagerInspector.DrawClients();
        }
    }
}
