using System.Collections.Generic;
using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Recorders
{
    public class ComponentRecorder : Recorder<Component, ComponentRecorder.Info>
    {
        public class Info : ISingleRecord<Component>
        {
            public Component component;

            public bool enabled;

            public void Record(Component component)
            {
                this.component = component;
                enabled = CommonFun.GetComponentEnabled(component);
            }

            public void Recover()
            {
                component.XSetEnable(enabled);
            }
        }
    }

    /// <summary>
    /// 碰撞体记录器:提速访问组件
    /// </summary>
    public class ColliderRecorder : Recorder<Collider, ColliderRecorder.Info>
    {
        /// <summary>
        /// 记录信息
        /// </summary>
        public class Info : ISingleRecord<Collider>
        {
            /// <summary>
            /// 碰撞器
            /// </summary>
            public Collider collider;

            private bool enabled;

            /// <summary>
            /// 记录碰撞体
            /// </summary>
            /// <param name="collider"></param>
            public void Record(Collider collider)
            {
                this.collider = collider;
                enabled = collider.enabled;
            }

            /// <summary>
            /// 恢复
            /// </summary>
            public void Recover()
            {
                collider.enabled = enabled;
            }

            /// <summary>
            /// 设置启用
            /// </summary>
            /// <param name="enabled"></param>
            public void SetEnable(bool enabled)
            {
                collider.enabled = enabled;
            }
        }
    }

    /// <summary>
    /// 刚体记录器
    /// </summary>
    public class RigidbodyRecorder : Recorder<Rigidbody, RigidbodyRecorder.Info>
    {
        /// <summary>
        /// 记录游戏对象中的刚体
        /// </summary>
        /// <param name="gameObject"></param>
        public void Record(GameObject gameObject)
        {
            if (gameObject) Record(gameObject.GetComponent<Rigidbody>());
        }

        /// <summary>
        /// 批量记录游戏对象中的刚体
        /// </summary>
        /// <param name="gameObjects"></param>
        public void Record(IEnumerable<GameObject> gameObjects)
        {
            if (gameObjects == null) return;
            foreach (var go in gameObjects)
            {
                Record(go);
            }
        }

        /// <summary>
        /// 记录信息
        /// </summary>
        public class Info : ISingleRecord<Rigidbody>
        {
            public Rigidbody rigidbody;

            private Vector3 velocity;

            private Vector3 angularVelocity;

            private bool useGravity;

            private bool isKinematic;

            private float drag;

            private float angularDrag;

            private RigidbodyConstraints rigidbodyConstraints;

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="rigidbody"></param>
            public void Record(Rigidbody rigidbody)
            {
                this.rigidbody = rigidbody;

                velocity = rigidbody.velocity;
                angularVelocity = rigidbody.angularVelocity;
                useGravity = rigidbody.useGravity;
                isKinematic = rigidbody.isKinematic;
                drag = rigidbody.drag;
                angularDrag = rigidbody.angularDrag;
                rigidbodyConstraints = rigidbody.constraints;
            }

            /// <summary>
            /// 恢复
            /// </summary>
            public void Recover()
            {
                rigidbody.velocity = velocity;
                rigidbody.angularVelocity = angularVelocity;
                rigidbody.useGravity = useGravity;
                rigidbody.isKinematic = isKinematic;
                rigidbody.drag = drag;
                rigidbody.angularDrag = angularDrag;
                rigidbody.constraints = rigidbodyConstraints;
            }

            /// <summary>
            /// 设置IK属性
            /// </summary>
            /// <param name="isKinematic"></param>
            public void SetIsKinematic(bool isKinematic)
            {
                rigidbody.isKinematic = isKinematic;
            }
        }
    }
}
