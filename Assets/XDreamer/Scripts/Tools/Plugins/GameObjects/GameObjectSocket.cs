using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginSMS.States.Dataflows.DataModel;

namespace XCSJ.PluginTools.GameObjects
{

    /// <summary>
    /// 游戏对象插槽
    /// </summary>
    public interface IGameObjectSocket
    {
        /// <summary>
        /// 目标
        /// </summary>
        GameObject target { get; set; }

        /// <summary>
        /// 插槽对象
        /// </summary>
        Transform socket { get; }

        /// <summary>
        /// 插槽位置
        /// </summary>
        Vector3 socketPosition { get; }

        /// <summary>
        /// 组
        /// </summary>
        ISingleGroup group { get; }

        /// <summary>
        /// 插件匹配规则
        /// </summary>
        ESocketMatchRule matchRule { get; }

        /// <summary>
        /// 插槽状态
        /// </summary>
        bool empty { get; set; }

        /// <summary>
        /// 将目标移动到槽
        /// </summary>
        void MoveTargetToSocket();

        /// <summary>
        /// 移动游戏对象至槽点
        /// </summary>
        void MoveGameObjectToSocket(GameObject gameObject);
    }

    public enum ESocketMatchRule
    {
        [Name("与槽位对齐")]
        AlginTransform,

        [Name("显示槽位自身游戏对象")]
        DisplaySocketSelfGameObject,
    }

    /// <summary>
    /// 游戏对象插槽
    /// </summary>
    [Name("游戏对象插槽")]
    [Serializable]
    public class GameObjectSocket : IGameObjectSocket
    {
        /// <summary>
        /// 目标
        /// </summary>
        public GameObject target { get; set; }

        /// <summary>
        /// 插槽对象
        /// </summary>
        [Name("插槽对象")]
        public Transform _socket;

        public Vector3 socketPosition => socketTransformRecorder.position;

        /// <summary>
        /// 插槽
        /// </summary>
        public Transform socket => _socket;

        private SocketTransformInfo socketTransformRecorder = new SocketTransformInfo();

        /// <summary>
        /// 分组
        /// </summary>
        public ISingleGroup group { get; protected set; }

        /// <summary>
        /// 插件匹配规则
        /// </summary>
        [Name("插件匹配规则")] 
        public ESocketMatchRule _matchRule = ESocketMatchRule.DisplaySocketSelfGameObject;

        /// <summary>
        /// 匹配规则
        /// </summary>
        public ESocketMatchRule matchRule => _matchRule;

        /// <summary>
        /// 插槽状态
        /// </summary>
        public virtual bool empty 
        { 
            get => _empty; 
            set
            {
                _empty = value;

                if (!_empty)
                {
                    switch (_matchRule)
                    {
                        case ESocketMatchRule.AlginTransform:
                            {
                                if (target && socket && target != socket)
                                {
                                    target.transform.position = socket.position;
                                    target.transform.rotation = socket.rotation;
                                    target.transform.localScale = socket.localScale;
                                }
                                break;
                            }
                        case ESocketMatchRule.DisplaySocketSelfGameObject:
                            {
                                if (socket)
                                {
                                    socket.gameObject.SetActive(true);
                                }
                                break;
                            }
                    }
                }
            }
        }

        private bool _empty = true;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="target"></param>
        /// <param name="socket"></param>
        /// <param name="group"></param>
        /// <param name="matchRule"></param>
        public GameObjectSocket(GameObject target, Transform socket, ESocketMatchRule matchRule, ISingleGroup group = null)
        {
            this.target = target;
            _socket = socket;

            socketTransformRecorder.Record(_socket);

            this.group = group;
            _matchRule = matchRule;
        }

        /// <summary>
        /// 将目标移动至槽
        /// </summary>
        public void MoveTargetToSocket()
        {
            socketTransformRecorder.RecoverTo(target.transform);
        }


        /// <summary>
        /// 移动游戏对象至槽点
        /// </summary>
        public void MoveGameObjectToSocket(GameObject gameObject)
        {
            if (gameObject)
            {
                socketTransformRecorder.RecoverTo(gameObject.transform);
            }
        }

        private class SocketTransformInfo
        {
            /// <summary>
            /// 本地位置
            /// </summary>
            public Vector3 position { get; set; }

            /// <summary>
            /// 本地旋转
            /// </summary>
            public Quaternion rotation { get; set; }

            /// <summary>
            /// 本地缩放
            /// </summary>
            public Vector3 localScale { get; set; }

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="transform"></param>
            public void Record(Transform transform)
            {
                position = transform.position;
                rotation = transform.rotation;
                localScale = transform.localScale;
            }

            /// <summary>
            /// 还原
            /// </summary>
            /// <param name="transform"></param>
            public void RecoverTo(Transform transform)
            {
                transform.position = position;
                transform.rotation = rotation;
                transform.localScale = localScale;
            }
        }
    }
}
