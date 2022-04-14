using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.Extension.CNScripts.Base
{
    /// <summary>
    /// MonoBehaviour脚本事件
    /// </summary>
    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.CNScriptMenu + Title)]
    public class MonoBehaviourScriptEvent : BaseScriptEvent<EMonoBehaviourScriptEventType, MonoBehaviourScriptEventSet>, IMonoBehaviour, IOnTrigger, IOnCollision
    {
        public const string Title = "MonoBehaviour脚本事件";

        /// <summary>
        /// 启动
        /// </summary>
        public override void Start()
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.Start);
        }

        /// <summary>
        /// 更新（Update is called once per frame)
        /// </summary>
        public override void Update()
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.Update);
        }

        /// <summary>
        /// 启动时
        /// </summary>
        public override void OnEnable()
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnEnable);
        }

        /// <summary>
        /// 变为不可用或非激活状态时
        /// </summary>
        public override void OnDisable()
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnDisable);
        }

        /// <summary>
        /// 渲染GUI时
        /// </summary>
        public void OnGUI()
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnGUI);
        }

        /// <summary>
        /// 销毁时
        /// </summary>
        public override void OnDestroy()
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnDestroy);
        }

        /// <summary>
        /// 重置时
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            RunScriptEvent(EMonoBehaviourScriptEventType.Reset);
        }

        /// <summary>
        /// 程序退出时
        /// </summary>
        public void OnApplicationQuit()
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnApplicationQuit);
        }

        /// <summary>
        /// 程序获取焦点时
        /// </summary>
        public void OnApplicationFocus()
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnApplicationFocus);
        }

        /// <summary>
        /// 鼠标移入时
        /// </summary>
        public void OnMouseEnter()
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnMouseEnter);
        }

        /// <summary>
        /// 鼠标悬浮时
        /// </summary>
        public void OnMouseOver()
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnMouseOver);
        }

        /// <summary>
        /// 鼠标移出时
        /// </summary>
        public void OnMouseExit()
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnMouseExit);
        }

        /// <summary>
        /// 鼠标点击时
        /// </summary>
        public void OnMouseDown()
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnMouseDown);
        }

        /// <summary>
        /// 鼠标弹起时
        /// </summary>
        public void OnMouseUp()
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnMouseUp);
        }

        /// <summary>
        /// 鼠标弹起(点击与弹起为同一元素)时
        /// </summary>
        public void OnMouseUpAsButton()
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnMouseUpAsButton);
        }

        /// <summary>
        /// 鼠标拖拽时
        /// </summary>
        public void OnMouseDrag()
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnMouseDrag);
        }

        /// <summary>
        /// 进入触发器时
        /// </summary>
        public void OnTriggerEnter(Collider collider)
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnTriggerEnter, CommonFun.GameObjectComponentToString(collider));
        }

        /// <summary>
        /// 停止触发器时
        /// </summary>
        public void OnTriggerExit(Collider collider)
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnTriggerExit, CommonFun.GameObjectComponentToString(collider));
        }

        /// <summary>
        /// 碰撞体接触触发器时(每帧调用)
        /// </summary>
        public void OnTriggerStay(Collider collider)
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnTriggerStay, CommonFun.GameObjectComponentToString(collider));
        }

        /// <summary>
        /// 碰撞体与碰撞体接触时
        /// </summary>
        public void OnCollisionEnter(Collision collision)
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnCollisionEnter, CommonFun.GameObjectToString(collision.gameObject));
        }

        /// <summary>
        /// 碰撞体与碰撞体停止接触时时
        /// </summary>
        public void OnCollisionExit(Collision collision)
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnCollisionExit, CommonFun.GameObjectToString(collision.gameObject));
        }

        /// <summary>
        /// 碰撞体与碰撞体停止接触时(每帧调用)
        /// </summary>
        public void OnCollisionStay(Collision collision)
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnCollisionStay, CommonFun.GameObjectToString(collision.gameObject));
        }

        /// <summary>
        /// 控制体与碰撞体碰撞时
        /// </summary>
        public void OnControllerColliderHit(ControllerColliderHit hit)
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnControllerColliderHit, CommonFun.GameObjectToString(hit.gameObject));
        }

        /// <summary>
        /// 相机渲染场景前(所有渲染开始前)
        /// </summary>
        public void OnPreRender()
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnPreRender);
        }

        /// <summary>
        /// 相机渲染场景后(所有渲染完成后)
        /// </summary>
        public void OnPostRender()
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnPostRender);
        }

        /// <summary>
        /// 当前对象渲染后
        /// </summary>
        public void OnRenderObject()
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnRenderObject);
        }

        /// <summary>
        /// 对象可见且相机渲染前
        /// </summary>
        public void OnWillRenderObject()
        {
            RunScriptEvent(EMonoBehaviourScriptEventType.OnWillRenderObject);
        }
    }
}
