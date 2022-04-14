using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.SelectionUtils;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XCSJ.PluginTools.GameObjects
{
    /// <summary>
    /// 游戏对象插槽缓存
    /// </summary>
    [Name("游戏对象插槽缓存")]
    [Tool("游戏对象", disallowMultiple = true, rootType = typeof(ToolsManager))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Put)]
    [RequireManager(typeof(ToolsManager))]
    public class GameObjectSocketCache : SingleInstanceMB<GameObjectSocketCache>
    {
        /// <summary>
        /// 匹配距离规则
        /// </summary>
        public enum EDistanceRule
        {
            [Name("世界坐标系")]
            World,

            [Name("屏幕坐标系")]
            Screen,
        }

        /// <summary>
        /// 距离规则
        /// </summary>
        [Name("距离规则")]
        [EnumPopup]
        public EDistanceRule _distanceRule;

        /// <summary>
        /// 匹配距离
        /// </summary>
        [Name("匹配距离")]
        [Min(0.01f)]
        public float _matchDistance = 0.1f;

        /// <summary>
        ///  材质规则
        /// </summary>
        public enum EMaterialRule
        {
            [Name("无")]
            None,

            [Name("在距离内显示材质对象")]
            ShowMaterialInDistance,
        }

        /// <summary>
        /// 材质规则
        /// </summary>
        [Name("材质规则")]
        [EnumPopup]
        public EMaterialRule _materialRule = EMaterialRule.ShowMaterialInDistance;

        /// <summary>
        /// 指示匹配对象的材质
        /// </summary>
        [Name("匹配材质")]
        public Material _matchMaterial = null;

        /// <summary>
        /// 匹配材质对象，用于指示用户拖拽匹配
        /// </summary>
        public GameObject matchMaterialObject
        {
            get => _matchMaterialObject;
            set
            {
                if (_matchMaterialObject)
                {
                    Destroy(_matchMaterialObject);
                }

                if (value)
                {
                    _matchMaterialObject = Instantiate(value, value.transform.parent);
                    _matchMaterialObject.SetActive(false);
                    _matchMaterialObject.GetComponentsInChildren<Renderer>().Foreach(r => r.materials = new Material[] { _matchMaterial });

                    currentSocket?.MoveGameObjectToSocket(_matchMaterialObject);
                }
            }
        }
        private GameObject _matchMaterialObject = null;

        /// <summary>
        /// 组插槽
        /// </summary>
        private List<IGameObjectSocket> groupSockets = null;

        /// <summary>
        /// 当前插槽对象
        /// </summary>
        private IGameObjectSocket currentSocket
        {
            get => _currentSocket;
            set
            {
                _currentSocket = value;

                matchMaterialObject = _currentSocket?.target;
            }
        }
        private IGameObjectSocket _currentSocket = null;

        /// <summary>
        /// 槽对象
        /// </summary>
        protected static List<IGameObjectSocket> socketList = new List<IGameObjectSocket>();

        #region 静态函数

        /// <summary>
        /// 槽改变事件:参数1为旧槽位;参数2为新槽位;参数3为是否结束拖拽
        /// </summary>
        public static event Action<GameObjectSocketCache, IGameObjectSocket, IGameObjectSocket, bool> onCurrentSocketChanged;

        /// <summary>
        /// 插槽匹配
        /// </summary>
        public static event Action<GameObjectSocketCache, IGameObjectSocket> onSocketMatch;

        /// <summary>
        /// 注册插槽
        /// </summary>
        /// <param name="socket"></param>
        public static void RegisterSocket(IGameObjectSocket socket)
        {
            if (socket != null && !socketList.Contains(socket))
            {
                socketList.Add(socket);
            }
        }

        /// <summary>
        /// 批量注册插槽对象
        /// </summary>
        /// <param name="sockets"></param>
        public static void RegisterSockets(IEnumerable<IGameObjectSocket> sockets)
        {
            foreach (var s in sockets)
            {
                RegisterSocket(s);
            }
        }

        /// <summary>
        /// 注销插槽
        /// </summary>
        /// <param name="socket"></param>
        public static void UnregisterSocket(IGameObjectSocket socket)
        {
            if (socket != null)
            {
                socketList.Remove(socket);
            }
        }

        /// <summary>
        /// 批量注销插槽对象
        /// </summary>
        /// <param name="socket"></param>
        public static void UnregisterSockets(IEnumerable<IGameObjectSocket> sockets)
        {
            foreach (var s in sockets)
            {
                UnregisterSocket(s);
            }
        }

        /// <summary>
        /// 查找同组插槽列表，如果没有组，则加入自身
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private static List<IGameObjectSocket> FindSameGroupEmptySockets(GameObject target)
        {
            var list = new List<IGameObjectSocket>();
            var socket = socketList.Find(s => s.target == target);
            if (socket != null)
            {
                if (socket.empty) list.Add(socket);
                if (socket.group != null)
                {
                    list.AddRange(socketList.FindAll(s => s != socket && s.group == socket.group && s.empty));
                }
            }
            return list;
        }

        /// <summary>
        /// 移除所有插槽
        /// </summary>
        private static void ClearSocket()
        {
            socketList.Clear();
        }

        #endregion

        public void Reset()
        {
#if UNITY_EDITOR
            // 添加指示匹配的材质
            this.XModifyProperty(()=> _matchMaterial = AssetDatabase.LoadAssetAtPath("Assets/XDreamer-Assets/基础/Materials/常用/TransparentRim.mat", typeof(Material)) as Material);
#endif
        }

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="gameObjects"></param>
        public void BeginDrag(params GameObject[] gameObjects)
        {
            if (gameObjects == null || gameObjects.Length == 0 || !gameObjects[0])
            {
                return;
            }
            var dragGameObject = gameObjects[0];

            groupSockets = FindSameGroupEmptySockets(dragGameObject);
            if (groupSockets.Count > 0)
            {
                currentSocket = groupSockets.First();
                onCurrentSocketChanged?.Invoke(this, null, currentSocket, false);
                currentSocket?.target.SetActive(true); // 激活当前槽对象
            }
        }

        /// <summary>
        /// 拖拽中
        /// </summary>
        /// <param name="gameObjects"></param>
        public void Dragging(params GameObject[] gameObjects)
        {
            if (gameObjects == null || gameObjects.Length == 0 || !gameObjects[0])
            {
                return;
            }
            var dragGameObject = gameObjects[0];

            // 查找最近插槽
            if (groupSockets.Count > 1)
            {
                var socket = FindNearestSocket(dragGameObject.transform.position);
                if (socket != null && currentSocket != socket)
                {
                    var oldSocket = currentSocket;
                    currentSocket = socket;
                    onCurrentSocketChanged?.Invoke(this, oldSocket, currentSocket, false);

                    oldSocket?.target.SetActive(false);// 非激活当前槽对象
                    currentSocket?.target.SetActive(true);// 激活当前槽对象
                }
            }

            switch (_materialRule)
            {
                case EMaterialRule.None:
                    {
                        _matchMaterialObject.gameObject.SetActive(true);
                        break;
                    }
                case EMaterialRule.ShowMaterialInDistance:
                    {
                        if (_matchMaterialObject)
                        {
                            if (dragGameObject)
                            {
                                _matchMaterialObject.SetActive(MatchCurrentSocket(dragGameObject));
                            }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <param name="gameObjects"></param>
        public void EndDrag(params GameObject[] gameObjects)
        {
            if (gameObjects == null || gameObjects.Length == 0 || !gameObjects[0])
            {
                return;
            }
            var dragGameObject = gameObjects[0];

            if (MatchCurrentSocket(dragGameObject))
            {
                currentSocket.empty = false;
                currentSocket.MoveTargetToSocket();
                onSocketMatch?.Invoke(this, currentSocket);
            }
            onCurrentSocketChanged?.Invoke(this, currentSocket, null, true);
            currentSocket?.target.SetActive(!currentSocket.empty);// 根据槽状态设定目标对象是否激活
            currentSocket = null;
        }

        /// <summary>
        /// 查找离组内最近的插槽
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private IGameObjectSocket FindNearestSocket(Vector3 position)
        {
            IGameObjectSocket socket = null;

            float nearestDistance = Mathf.Infinity;
            foreach (var item in groupSockets)
            {
                if (item.empty)
                {
                    float distance = Vector3.Distance(position, item.socketPosition);
                    if (distance < nearestDistance)
                    {
                        socket = item;
                        nearestDistance = distance;
                    }
                }
            }
            return socket;
        }

        /// <summary>
        /// 匹配当前插槽
        /// </summary>
        /// <returns></returns>
        private bool MatchCurrentSocket(GameObject dragGameObject)
        {
            if (currentSocket == null) return false;

            return InMatchDistance(dragGameObject.transform.position, currentSocket.socketPosition);
        }

        /// <summary>
        /// 在匹配的距离范围内
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        private bool InMatchDistance(Vector3 point1, Vector3 point2)
        {
            //Debug.Log("p1:" + point1 + ",p2" + point2 + ",dis:" + dis);
            float dis = 0;
            switch (_distanceRule)
            {
                case EDistanceRule.World:
                    {
                        dis = Vector3.Distance(point1, point2);
                        break;
                    }
                case EDistanceRule.Screen:
                    {
                        dis = Vector3.Distance(Camera.main.WorldToScreenPoint(point1), Camera.main.WorldToScreenPoint(point2));
                        break;
                    }
            }
            return dis < _matchDistance;
        }
    }
}
