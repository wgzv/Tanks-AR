using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.PluginTools.Puts
{
    /// <summary>
    /// 选择集拖拽通过相机视图
    /// </summary>
    [Name("选择集拖拽通过相机视图")]
    [Tool("选择集", disallowMultiple =true, rootType = typeof(ToolsManager))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Select)]
    [RequireManager(typeof(ToolsManager))]
    public class DragByCameraView : BaseDragger
    {
        /// <summary>
        /// 拖拽规则
        /// </summary>
        public enum EDragRule
        {
            [Name("无")]
            None,

            [Name("转换")]
            [Tip("通过直接操作转换改变游戏对象的位置")]
            Transform,

            [Name("施力")]
            [Tip("被拖拽对象需有刚体组件，使用施力方式对刚体进行拖拽操作")]
            Force,
        }

        [Name("拖拽规则")]
        [EnumPopup]
        public EDragRule _dragRule = EDragRule.Transform;

        [Name("脉冲力系数")]
        [Min(0)]
        [HideInSuperInspector(nameof(_dragRule), EValidityCheckType.NotEqual, EDragRule.Force)]
        public float _impulseForceCoefficient = 25f;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            if (recoverBeforeDragPositionWhenDisable)
            {
                RecoverPosition();
            }
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
        {
            SetMoveIconActive(false);
        }

        #region 拖拽

        /// <summary>
        /// 拖拽输入
        /// </summary>
        public IGrabAction dragTrigger
        {
            get
            {
                if (_dragTrigger == null)
                {
                    _dragTrigger = new SelectionDragByMouse();
                }
                return _dragTrigger;
            }
            set
            {
                _dragTrigger = value;
            }
        }
        private IGrabAction _dragTrigger;

        #region 直接操作转换辅助数据

        private float grabObjScreenPositionZ = 0;

        private Vector3 offsetPosition = Vector3.zero;

        #endregion

        #region 施力辅助数据

        private Rigidbody grabRigidbody;

        private Vector3 grabLocalPoint;

        private float grabDistanceFromCamera;

        #endregion

        /// <summary>
        /// 是否开始拖拽
        /// </summary>
        /// <returns></returns>
        public override bool Grab() => dragTrigger.Grab();

        /// <summary>
        /// 是否拖拽中
        /// </summary>
        /// <returns></returns>
        public override bool Hold() => dragTrigger.Hold();

        /// <summary>
        /// 是否结束拖拽
        /// </summary>
        /// <returns></returns>
        public override bool Release() => dragTrigger.Release();

        /// <summary>
        /// 开启拖拽
        /// </summary>
        public override void OnGrab(params GameObject[] gameObjects)
        {
            if (gameObjects == null || gameObjects.Length == 0 || !gameObjects[0]) return;
                
            switch (_dragRule)
            {
                case EDragRule.None: return;
                case EDragRule.Transform:
                    {
                        RecordPosition(Selection.selection);

                        grabObjScreenPositionZ = cam.WorldToScreenPoint(gameObjects[0].transform.position).z;

                        offsetPosition = Selection.selection.transform.position - cam.ScreenToWorldPoint(screenPosition);

                        break;
                    }
                case EDragRule.Force:
                    {
                        var obj = dragTrigger as SelectionDragByMouse;
                        if (obj!=null)
                        {
                            grabRigidbody = obj.rayHitInfo.rigidbody;
                            if (grabRigidbody)
                            {
                                grabLocalPoint = grabRigidbody.transform.InverseTransformPoint(obj.rayHitInfo.point);
                                grabDistanceFromCamera = obj.rayHitInfo.distance;
                            }
                        }
                        break;
                    }
            }
            SetMoveIconActive(true);
        }

        private Vector3 screenPosition => new Vector3(XInput.mousePosition.x, XInput.mousePosition.y, grabObjScreenPositionZ);

        /// <summary>
        /// 拖拽中
        /// </summary>
        public override void OnHold(params GameObject[] gameObjects)
        {
            if (gameObjects == null || gameObjects.Length == 0 || !gameObjects[0]) return;

            switch (_dragRule)
            {
                case EDragRule.None: return;
                case EDragRule.Transform:
                        {
                            gameObjects[0].transform.position = cam.ScreenToWorldPoint(screenPosition) + offsetPosition;
                            break;
                        }
            }

            if (moveIcon)
            {
                moveIcon.anchorMax = moveIcon.anchorMin = cam.ScreenToViewportPoint(XInput.mousePosition);
            }
        }

        public override void OnHoldInFixedUpdate(params GameObject[] gameObjects)
        {
            base.OnHoldInFixedUpdate(gameObjects);

            switch (_dragRule)
            {
                case EDragRule.Force:
                    {
                        if (grabRigidbody)
                        {
                            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.grabDistanceFromCamera);
                            Vector3 vector = cam.ScreenToWorldPoint(position);
                            Vector3 vector2 = this.grabRigidbody.transform.TransformPoint(this.grabLocalPoint);
                            Debug.DrawLine(vector, vector2, Color.red);
                            Vector3 force = (vector - vector2) * _impulseForceCoefficient * Time.fixedDeltaTime;
                            this.grabRigidbody.AddForceAtPosition(force, vector2, ForceMode.Impulse);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        public override void OnRelease(params GameObject[] gameObjects)
        {
            grabObjScreenPositionZ = 0;
            offsetPosition = Vector3.zero;
            grabRigidbody = null;

            SetMoveIconActive(false);
        }

        /// <summary>
        /// 记录拖拽对象刚体
        /// </summary>
        /// <param name="selections"></param>
        protected override void RecordRigidbodyAndSetKinematic(params GameObject[] selections)
        {
            switch (_dragRule)
            {
                case EDragRule.None: break;
                case EDragRule.Transform:
                    {
                        base.RecordRigidbodyAndSetKinematic(selections);
                        break;
                    }
                case EDragRule.Force: break;
            }
        }

        /// <summary>
        /// 还原拖拽对象刚体
        /// </summary>
        protected override void RecoverRigidbody()
        {
            switch (_dragRule)
            {
                case EDragRule.None: break;
                case EDragRule.Transform:
                    {
                        base.RecoverRigidbody();
                        break;
                    }
                case EDragRule.Force: break;
            }
        }

        #endregion

        #region 光标

        /// <summary>
        /// 光标
        /// </summary>
        [Name("光标")]
        public RectTransform moveIcon;

        /// <summary>
        /// 设置光标激活
        /// </summary>
        /// <param name="drag"></param>
        private void SetMoveIconActive(bool drag)
        {
            if (moveIcon)
            {
                moveIcon.gameObject.SetActive(drag);
            }
        }

        #endregion

        #region 记录和还原移动变换对象

        /// <summary>
        /// 禁用时重置位置
        /// </summary>
        [Name("禁用时重置位置")]
        [Tip("重置拖拽前模型的位置")]
        public bool recoverBeforeDragPositionWhenDisable = true;

        /// <summary>
        /// 位置记录器
        /// </summary>
        private Dictionary<Transform, Vector3> initPositionDic = new Dictionary<Transform, Vector3>();

        /// <summary>
        /// 保存初始位置
        /// </summary>
        /// <param name="go"></param>
        private void RecordPosition(GameObject go)
        {
            if (!initPositionDic.ContainsKey(go.transform))
            {
                initPositionDic.Add(go.transform, go.transform.localPosition);
            }
        }

        /// <summary>
        /// 还原初始位置
        /// </summary>
        public void RecoverPosition()
        {
            foreach (var item in initPositionDic)
            {
                if (item.Key)
                {
                    item.Key.localPosition = item.Value;
                }
            }
            initPositionDic.Clear();
        } 

        #endregion
    }
}
