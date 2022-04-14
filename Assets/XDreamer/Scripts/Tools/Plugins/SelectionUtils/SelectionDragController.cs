using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.SelectionUtils
{
    /// <summary>
    /// 选择集拖拽器
    /// </summary>
    [Name("选择集拖拽器")]
    [Tool("选择集", disallowMultiple = true, rootType = typeof(ToolsManager))]
    [XCSJ.Attributes.Icon(EIcon.Select)]
    public class SelectionDragController : SelectionListenerMB<SelectionDragController>
    {
        /// <summary>
        /// 变换拖拽
        /// </summary>
        public BaseDragger dragger
        {
            get
            {
                return _dragger;
            }
            set
            {
                if (_dragger == value) return;

                if (_dragger)
                {
                    _dragger.OnActive(false);
                    onDraggerChanged?.Invoke(_dragger, false);
                }

                _lastDragger = _dragger;
                _dragger = value;

                if (_dragger)
                {
                    onDraggerChanged?.Invoke(_dragger, true);
                    _dragger.OnActive(true);
                }
            }
        }
        private BaseDragger _dragger;

        private BaseDragger _lastDragger = null;

        /// <summary>
        /// 恢复上一个游戏对象拖拽器
        /// </summary>
        public void RecoverLastDragger()
        {
            dragger = _lastDragger;
        }

        /// <summary>
        /// 拖拽器变化回调事件：参数1=拖拽器；参数2 true为激活，false为非激活
        /// </summary>
        public static event Action<IDragger, bool> onDraggerChanged;

        /// <summary>
        /// 相机
        /// </summary>
        protected Camera cam => Camera.main ? Camera.main : Camera.current;

        /// <summary>
        /// 抓
        /// </summary>
        public static event Action<GameObject[]> onGrab;

        /// <summary>
        /// 持有
        /// </summary>
        public static event Action<GameObject[]> onHold;

        /// <summary>
        /// 放
        /// </summary>
        public static event Action<GameObject[]> onRelease;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            draging = false;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
        }

        /// <summary>
        /// 拖拽中
        /// </summary>
        protected bool draging = false;

        /// <summary>
        /// 延迟更新
        /// </summary>
        protected virtual void Update()
        {
            if (!cam || _dragger == null) return;

            if (!draging && dragger.Grab())
            {
                if (Selection.selection)
                {
                    draging = true;

                    CommonFun.BeginOnUI();

                    _dragger.OnGrab(Selection.selections);

                    onGrab?.Invoke(Selection.selections);
                }
                return;// 隔帧调用
            }

            if (draging && dragger.Hold())
            {
                _dragger.OnHold(Selection.selections);

                onHold?.Invoke(Selection.selections);
            }

            if (draging && dragger.Release())
            {
                draging = false;

                CommonFun.EndOnUI();

                _dragger.OnRelease(Selection.selections);

                onRelease?.Invoke(Selection.selections);
            }
        }
        
        /// <summary>
        /// 固定更新
        /// </summary>
        protected void FixedUpdate()
        {
            if (draging && dragger.Hold())
            {
                _dragger.OnHoldInFixedUpdate(Selection.selections);
            }
        }

        /// <summary>
        /// 选择集发生改变
        /// </summary>
        /// <param name="oldSelections"></param>
        /// <param name="flag"></param>
        protected override void OnSelectionChanged(GameObject[] oldSelections, bool flag)
        {
            _dragger?.OnSelectionChanged(oldSelections, flag);
        }
    }

    /// <summary>
    /// 抓动作
    /// </summary>
    public interface IGrabAction
    {
        /// <summary>
        /// 抓
        /// </summary>
        /// <returns></returns>
        bool Grab();

        /// <summary>
        /// 持有
        /// </summary>
        /// <returns></returns>
        bool Hold();

        /// <summary>
        /// 放
        /// </summary>
        /// <returns></returns>
        bool Release();
    }

    /// <summary>
    /// 拖拽器：用于实现游戏对象拖拽运算的对象
    /// </summary>
    public interface IDragger : IGrabAction
    {
        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="gameObjects"></param>
        void OnGrab(params GameObject[] gameObjects);

        /// <summary>
        /// 拖拽中
        /// </summary>
        /// <param name="gameObjects"></param>
        void OnHold(params GameObject[] gameObjects);

        /// <summary>
        /// 在FixedUpdate拖拽中, 用于物理引擎的拖拽操作
        /// </summary>
        /// <param name="gameObjects"></param>
        void OnHoldInFixedUpdate(params GameObject[] gameObjects); 

        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <param name="gameObjects"></param>
        void OnRelease(params GameObject[] gameObjects);
    }

    /// <summary>
    /// 选择集拖拽器监听器
    /// </summary>
    public abstract class BaseDragger : ToolMB, IDragger
    {
        /// <summary>
        /// 启用时激活拖拽器
        /// </summary>
        [Name("启用时激活拖拽器")]
        [Readonly(EEditorMode.Runtime)]
        public bool _activeOnEnable = true;

        /// <summary>
        /// 选中后设置刚体为运动学
        /// </summary>
        [Name("选中后设置刚体为运动学")]
        public bool _setKinematicSelected = true;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            if (_activeOnEnable)
            {
                SetActiveDragger(true);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            if (_activeOnEnable)
            {
                SetActiveDragger(false);
            }
        }

        /// <summary>
        /// 激活游戏对象拖拽器
        /// </summary>
        /// <param name="active"></param>
        public void SetActiveDragger(bool active)
        {
            if (active)
            {
                var dragger = SelectionDragController.instance;
                if (dragger)
                {
                    dragger.dragger = this;
                }
            }
            else
            {
                if (SelectionDragController.validInstance)
                {
                    var dragger = SelectionDragController.instance;
                    if (dragger.dragger == this)
                    {
                        dragger.dragger = null;
                    }
                }
            }
        }

        /// <summary>
        /// 是否为当前活跃变换拖拽对象
        /// </summary>
        /// <returns></returns>
        protected bool IsCurrentDragger() => SelectionDragController.instance.dragger == this;

        /// <summary>
        /// 恢复最后拖拽
        /// </summary>
        protected void RecoverLastDragger()
        {
            var dragger = SelectionDragController.instance;
            if (dragger)
            {
                dragger.RecoverLastDragger();
            }
        }

        #region IDragger

        #region IGrabAction

        /// <summary>
        /// 能否开始拖拽
        /// </summary>
        /// <returns></returns>
        public abstract bool Grab();

        /// <summary>
        /// 能否拖拽中
        /// </summary>
        /// <returns></returns>
        public abstract bool Hold();

        /// <summary>
        /// 能否拖拽结束
        /// </summary>
        /// <returns></returns>
        public abstract bool Release();

        #endregion

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="gameObjects"></param>
        public virtual void OnGrab(params GameObject[] gameObjects)
        {
            if (_setKinematicSelected)
            {
                RecordRigidbodyAndSetKinematic(Selection.selections);
            }
        }

        /// <summary>
        /// 拖拽中
        /// </summary>
        /// <param name="gameObjects"></param>
        public abstract void OnHold(params GameObject[] gameObjects);

        public virtual void OnHoldInFixedUpdate(params GameObject[] gameObjects) { }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <param name="gameObjects"></param>
        public virtual void OnRelease(params GameObject[] gameObjects)
        {
            if (_setKinematicSelected)
            {
                RecoverRigidbody();
            }
        }

        /// <summary>
        /// 相机
        /// </summary>
        protected Camera cam => Camera.main ? Camera.main : Camera.current;

        /// <summary>
        /// 选择集变化
        /// </summary>
        /// <param name="oldSelections"></param>
        /// <param name="flag"></param>
        public virtual void OnSelectionChanged(GameObject[] oldSelections, bool flag) { }

        public virtual void OnActive(bool active) { }

        #endregion

        #region 刚体记录与恢复

        /// <summary>
        /// 刚体记录器
        /// </summary>
        protected RigidbodyRecorder rigidbodyRecorder = new RigidbodyRecorder();

        /// <summary>
        /// 记录拖拽对象
        /// </summary>
        protected virtual void RecordRigidbodyAndSetKinematic(params GameObject[] selections)
        {
            if (selections == null) return;

            foreach (var item in selections)
            {
                if (item)
                {
                    rigidbodyRecorder.Record(item.GetComponentsInChildren<Rigidbody>());
                    rigidbodyRecorder._records.ForEach(r => r.SetIsKinematic(true));
                }
            }
        }

        /// <summary>
        /// 恢复拖拽对象
        /// </summary>
        protected virtual void RecoverRigidbody()
        {
            rigidbodyRecorder.Recover();
            rigidbodyRecorder.Clear();
        }

        #endregion
    }
}
