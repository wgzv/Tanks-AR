using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Interactions;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.Interactions.Base;
using XCSJ.PluginTools.Interactions.Interactables;

namespace XCSJ.PluginTools.Interactions.Interactors
{
    /// <summary>
    /// 刚体拾取器
    /// </summary>
    [DisallowMultipleComponent]
    [Name("刚体拾取器")]
    [Tool("交互器", rootType = typeof(ToolsManager))]
    [RequireManager(typeof(ToolsManager))]
    public class RigibodyPicker : BasePicker
    {
        /// <summary>
        /// 抓取参考点
        /// </summary>
        [Name("抓取参考点")]
        [Min(0)]
        public ReferencePoint _referencePoint = new ReferencePoint();

        /// <summary>
        /// 拾取输入
        /// </summary>
        public IPickInput pickInput
        {
            get => _pickAction;
            set
            {
                _pickAction = value;
            }
        }
        private IPickInput _pickAction;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            if (pickInput == null) pickInput = GetComponent<IPickInput>();
        }

        /// <summary>
        /// 固定更新
        /// </summary>
        protected virtual void FixedUpdate()
        {
            if (pickInput == null) return;

            switch (pickState)
            {
                case EPickState.None:
                    {
                        if (pickInput.IsPickup())
                        {
                            if (Pickup(pickInput) != null)
                            {
                                pickState = EPickState.Hold;
                            }
                        }
                        break;
                    }
                case EPickState.Hold:
                    {
                        UpdatePickedObjectTransform();

                        if (pickInput.IsDrop())
                        {
                            if (Drop(pickInput) != null)
                            {
                                pickState = EPickState.None;
                            }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 保持力
        /// </summary>
        [Name("保持力")]
        [Tip("为维持刚体对象跟随参考点而作用在刚体对象上的力")]
        [Min(0)]
        public float _holdForce = 5;

        /// <summary>
        /// 被抓刚体阻力
        /// </summary>
        [Name("被抓刚体阻力")]
        [Tip("当刚体被抓住时，使用当前值修改刚体的阻力和旋转阻力")]
        [Min(0)]
        public float _drag = 20;

        private Rigidbody holdRigidbody
        {
            get => _holdRigidbody;
            set
            {
                if (_holdRigidbody == value) return;

                rigidbodyRecorder.Recover();
                rigidbodyRecorder.Clear();

                _holdRigidbody = value;

                if (_holdRigidbody)
                {
                    rigidbodyRecorder.Record(holdRigidbody);

                    _holdRigidbody.useGravity = false;
                    _holdRigidbody.drag = _drag;
                    _holdRigidbody.angularDrag = _drag;
                }
            }
        }
        private Rigidbody _holdRigidbody;
        private RigidbodyRecorder rigidbodyRecorder = new RigidbodyRecorder();

        /// <summary>
        /// 捡起
        /// </summary>
        /// <param name="interactInput"></param>
        /// <returns></returns>
        public override IGrabbable Pickup(IInteractorInput interactInput)
        {
            if (!Selection.selection) return null;

            var grabbable = Selection.selection.GetComponent<IGrabbable>();
            if (grabbable != null)
            {
                if (grabbable.CanInteractable(InteractableContext.Default, this, interactInput, out _))
                {
                    holdRigidbody = Selection.selection.GetComponent<Rigidbody>();
                }
            }
            return grabbable;
        }

        /// <summary>
        /// 放下
        /// </summary>
        /// <param name="interactInput"></param>
        /// <returns></returns>
        public override IGrabbable Drop(IInteractorInput interactInput)
        {
            IGrabbable result = null;
            var pickInput = interactInput as IPickInput;
            if (pickInput != null && holdRigidbody)
            {
                holdRigidbody.AddForce(_referencePoint.GetDirection() * pickInput.dropForce, pickInput.forceMode);
                result = Selection.selection.GetComponent<IGrabbable>();
            }
            holdRigidbody = null;
            return result;
        }

        /// <summary>
        /// 把握对象规则:抓住对象后如何让对象跟随拾取者的规则
        /// </summary>
        [Name("把握对象规则")]
        [EnumPopup]
        public EHoldRule _holdRule = EHoldRule.Instantaneous;

        /// <summary>
        /// 把握对象规则:抓住对象后如何让对象跟随拾取者的规则
        /// </summary>
        public enum EHoldRule
        {
            [Name("无")]
            None,

            [Name("瞬时")]
            Instantaneous,

            [Name("速度跟随")]
            VelocityTracking,

            [Name("关节跟随")]
            Kinematic,
        }

        public void UpdatePickedObjectTransform()
        {
            if (!Selection.selection) return;

            switch (_holdRule)
            {
                case EHoldRule.Instantaneous:
                    {
                        PerformInstantaneousUpdate();
                        break;
                    }
                case EHoldRule.VelocityTracking:
                    {
                        PerformVelocityTrackingUpdate();
                        break;
                    }
                case EHoldRule.Kinematic:
                    {
                        PerformKinematicUpdate();
                        break;
                    }
            }
        }

        protected virtual void PerformInstantaneousUpdate()
        {
            Selection.selection.transform.position = _referencePoint.GetPosition();
            Selection.selection.transform.rotation = _referencePoint.GetRotation();
        }

        protected virtual void PerformVelocityTrackingUpdate()
        {
            if (!holdRigidbody) return;

            var pos = _referencePoint.GetPosition();
            var dir = pos - holdRigidbody.position;
            holdRigidbody.AddForce(dir * _holdForce, ForceMode.VelocityChange);
        }

        protected virtual void PerformKinematicUpdate()
        {

        }
    }

    /// <summary>
    /// 拾取动作触发器
    /// </summary>
    public interface IPickInput : IInteractorInput
    {
        /// <summary>
        /// 拾取
        /// </summary>
        bool IsPickup();

        /// <summary>
        /// 扔
        /// </summary>
        bool IsDrop();

        /// <summary>
        /// 扔模式
        /// </summary>
        ForceMode forceMode { get; }

        /// <summary>
        /// 扔的力
        /// </summary>
        float dropForce { get; }
    }
}
