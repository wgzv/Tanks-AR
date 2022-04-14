using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginTools;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;
using System.Linq;
using XCSJ.PluginTools.Interactions.Interactables;
using XCSJ.PluginTools.Interactions.Interactables.Items;
using XCSJ.Collections;
using XCSJ.Extension.Base.Interactions;

namespace XCSJ.PluginTools.SelectionUtils
{
    /// <summary>
    /// 选择集修改
    /// </summary>
    [Tool("选择集", disallowMultiple = true, rootType = typeof(ToolsManager))]
    [Name("选择集修改器")]
    [Tip("通过鼠标点击、触摸点击实现基于游戏对象选择集的修改")]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Select)]
    [RequireManager(typeof(ToolsManager))]
    public class SelectionModify : ToolMB, ISelector
    {
        /// <summary>
        /// 拾取层：选择对象属于当前设定的层才能拾取
        /// </summary>
        [Name("拾取层")]
        [Tip("选择对象属于当前设定的层才能拾取;默认为缺省层")]
        public LayerMask _pickObjectLayer = 1;

        /// <summary>
        /// 选择最大距离
        /// </summary>
        [Name("最大距离")]
        [Tip("当对象距当前相机的距离超过当前设定值时，则无法拾取")]
        [Min(0)]
        public float _maxDistance = 1000;

        /// <summary>
        /// 清除键码:按下对应键码时会将选择集清除，即选择集设置为空；
        /// </summary>
        [Name("清除键码")]
        [Tip("按下对应键码时会将选择集清除，即选择集设置为空；")]
        public KeyCode clearKeyCode = KeyCode.Escape;

        [Name("清除通过鼠标右键弹起")]
        public bool clearByMouseRightButtonUp = false;

        /// <summary>
        /// 识别点击最大偏移距离
        /// </summary>
        [Name("识别点击最大偏移距离")]
        [Tip("按下和弹起的位置距离小于当前值为有效点击，否则认为是拖拽")]
        [Range(0, 100)]
        public float _clickMaxDistance = 1;

        private GameObject pickGameObjectWhenMouseDown = null;

        private Vector3 mousePositionOnDown = Vector2.zero;

        /// <summary>
        /// 检查规则
        /// </summary>
        public enum ECheckRule
        {
            /// <summary>
            /// 总是
            /// </summary>
            [Name("总是")]
            [Tip("不做检查，并总认为检查成功")]
            Always = -1,

            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            [Tip("不做检查，即总认为检查失败")]
            None = 0,

            /// <summary>
            /// 碰撞体：被检查的游戏对象上需有碰撞体组件
            /// </summary>
            [Name("碰撞体")]
            [Tip("被检查的游戏对象上需有碰撞体组件")]
            Collider,

            /// <summary>
            /// 可选择对象:被检查的游戏对象上需有继承可选择接口的组件，并且标识是可被选择的；
            /// </summary>
            [Name("可选择对象")]
            [Tip("被检查的游戏对象上需有继承可选择接口的组件，并且标识是可被选择的；")]
            SelectableObject,

            /// <summary>
            /// 可选择对象(在父级):被检查的游戏对象上（或父级游戏对象上）需有继承可选择接口的组件，并且标识是可被选择的；
            /// </summary>
            [Name("可选择对象(在父级)")]
            [Tip("被检查的游戏对象上（或父级游戏对象上）需有继承可选择接口的组件，并且标识是可被选择的；")]
            SelectableObjectInParent,

            /// <summary>
            /// 刚体：被检查的游戏对象或父级需有刚体组件
            /// </summary>
            [Name("刚体")]
            [Tip("被检查的游戏对象或父级需有刚体组件；")]
            Rigidbody,
        }

        /// <summary>
        /// 检查规则
        /// </summary>
        [Name("检查规则")]
        [EnumPopup]
        public ECheckRule _checkRule = ECheckRule.Collider;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            Selection.selectionChanged += OnSelectionChanged;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            Selection.selectionChanged -= OnSelectionChanged;
            //清空选择集
            Selection.selection = null;
        }

        private void OnSelectionChanged(GameObject[] oldSelections, bool isUndoOrRedo)
        {
            foreach (var os in oldSelections)
            {
                if (os)
                {
                    foreach (var s in os.GetComponents<ISelectable>())
                    {
                        s.TryHandleInteractable(InteractableContext.Default, this, SelectorInput.Diselect, out _);
                    }
                }
            }

            foreach (var os in Selection.selections)
            {
                if (os)
                {
                    foreach (var s in os.GetComponents<ISelectable>())
                    {
                        s.TryHandleInteractable(InteractableContext.Default, this, SelectorInput.Select, out _);
                    }
                }
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (XInput.GetKeyDown(clearKeyCode) || (clearByMouseRightButtonUp && XInput.GetMouseButtonUp(1)))
            {
                //清空选择集
                Selection.selection = null;
            }
            else
            {
                //更新选择集
                UpdateSelection();
            }
        }

        /// <summary>
        /// 设置选择集
        /// </summary>
        /// <param name="gameObject"></param>
        private void SetSelection(GameObject selection)
        {
            if (TryGetSelectionGameObject(selection, out var result))
            {
                LimitedSelection.SetSelected(result);
            }
        }

        /// <summary>
        /// 尝试获取选择对象
        /// </summary>
        /// <param name="selection"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual bool TryGetSelectionGameObject(GameObject selection, out GameObject result)
        {
            switch (_checkRule)
            {
                case ECheckRule.Collider:
                    {
                        if (selection.GetComponent<Collider>())
                        {
                            result = selection;
                            return true;
                        }
                        break;
                    }
                case ECheckRule.SelectableObject:
                    {
                        if (selection.GetComponents<ISelectable>().Any(s => s.canSelected))
                        {
                            result = selection;
                            return true;
                        }
                        break;
                    }
                case ECheckRule.SelectableObjectInParent:
                    {
                        var c = selection.GetComponentsInParent<ISelectable>()?.FirstOrDefault(s => s.canSelected);
                        if (c is Component component && component)
                        {
                            result = component.gameObject;
                            return true;
                        }
                        break;
                    }
                case ECheckRule.Rigidbody:
                    {
                        if (selection.GetComponent<Rigidbody>())
                        {
                            result = selection;
                            return true;
                        }

                        var rb = selection.GetComponentInParent<Rigidbody>();
                        if (rb)
                        {
                            result = rb.gameObject;
                            return true;
                        }
                        break;
                    }
            }
            result = default;
            return false;
        }

        /// <summary>
        /// 更新选择集
        /// </summary>
        public void UpdateSelection()
        {
            //当指针在UI上时不处理
            if (CommonFun.IsOnAnyUI())
            {
                return;
            }

            if (XInput.GetMouseButtonDown(0))
            {
                mousePositionOnDown = XInput.mousePosition;
                pickGameObjectWhenMouseDown = PickGameObject(mousePositionOnDown);
            }
            else if (XInput.GetMouseButtonUp(0))//按下与弹起不可能在同一帧出现，所以可以使用else if进行处理
            {
                var mousePosition = XInput.mousePosition;
                var dis = (mousePositionOnDown - mousePosition).sqrMagnitude;

                if (dis < _clickMaxDistance)
                {
                    var go = PickGameObject(mousePosition);
                    if (pickGameObjectWhenMouseDown)
                    {
                        //点击按下与弹起时射线检测到的是同一个游戏对象，方才修改选择集！
                        if (pickGameObjectWhenMouseDown == go)
                        {
                            SetSelection(pickGameObjectWhenMouseDown);
                        }
                    }
                    else if(!go)// 点击空白处
                    {
                        Selection.Clear();
                    }
                }

                //重置临时点击游戏对象
                pickGameObjectWhenMouseDown = null;
            }
        }

        private GameObject PickGameObject(Vector2 pickPosition)
        {
            var cam = Camera.main ? Camera.main : Camera.current;
            if (cam && Physics.Raycast(cam.ScreenPointToRay(pickPosition), out RaycastHit hit, _maxDistance, _pickObjectLayer))
            {
                return hit.collider.gameObject;
            }
            return null;
        }

        public bool CanInteractable(IInteractableContext context, IInteractable interactable, IInteractorInput interactInput, out ICanInteractableResult canInteractableResult)
        {
            canInteractableResult = default;
            if (interactable is MonoBehaviour monoBehaviour && monoBehaviour)
            {
                switch (_checkRule)
                {
                    case ECheckRule.Collider: return monoBehaviour.GetComponent<Collider>();
                    case ECheckRule.SelectableObject:
                        {
                            if (monoBehaviour.GetComponents<ISelectable>().Any(s => s.canSelected))
                            {
                                return true;
                            }
                            break;
                        }
                    case ECheckRule.SelectableObjectInParent:
                        {
                            var c = monoBehaviour.GetComponentsInParent<ISelectable>()?.FirstOrDefault(s => s.canSelected);
                            return c is Component component && component;
                        }
                    case ECheckRule.Rigidbody: return monoBehaviour.GetComponent<Rigidbody>() || monoBehaviour.GetComponentInParent<Rigidbody>();
                }
            }
            return false;
        }

        public bool TryHandleInteractable(IInteractableContext context, IInteractable interactable, IInteractorInput interactInput, out IHandleInteractableResult handleInteractableResult)
        {
            handleInteractableResult = default;
            if (interactable is MonoBehaviour monoBehaviour && monoBehaviour)
            {
                SetSelection(monoBehaviour.gameObject);
            }
            return true;
        }
    }
}