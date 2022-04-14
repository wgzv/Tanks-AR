using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.Toggles
{
    /// <summary>
    /// Toggle切换游戏对象激活
    /// </summary>
    [Name("Toggle切换游戏对象激活")]
    [XCSJ.Attributes.Icon(EIcon.Toggle)]
    [Tip("通过Toggle切换游戏对象激活")]
    [Tool(XGUICategory.Component, nameof(XGUIManager))]
    [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
    public class ToggleSwitchGameObjectActive : View
    {
        /// <summary>
        /// 切换
        /// </summary>
        [Name("切换")]
        [Tip("如当前参数无效，会从当前组件所在游戏对象上查找当前参数类型的组件")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Toggle _toggle = null;

        /// <summary>
        /// 切换激活类型
        /// </summary>
        [Name("切换激活类型")]
        [EnumPopup]
        public ESwitchActiveType _switchAcitveType = ESwitchActiveType.ActiveLinkOn;

        /// <summary>
        /// 包含子对象
        /// </summary>
        [Name("包含子对象")]
        public bool includeChildren = false;

        /// <summary>
        /// 游戏对象列表
        /// </summary>
        [Name("游戏对象列表")]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        public List<GameObject> _gameObjects = new List<GameObject>();

        /// <summary>
        /// 子级游戏对象列表
        /// </summary>
        public List<GameObject> childrenGameObjects
        {
            get
            {
                if (includeChildren)
                {
                    var list = new List<GameObject>();
                    _gameObjects.ForEach(go => list.AddRange(CommonFun.GetChildGameObjects(go)));
                }
                return new List<GameObject>();
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            if (!_toggle) _toggle = GetComponent<Toggle>();
            if (_toggle)
            {
                _toggle.onValueChanged.AddListener(OnValueChanged);
                OnValueChanged(_toggle.isOn);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            if (_toggle)
            {
                _toggle.onValueChanged.RemoveListener(OnValueChanged);
            }
        }

        private void SetActive(GameObject go, bool active)
        {
            if (go) go.SetActive(active);
        }

        /// <summary>
        /// 切换值变更事件
        /// </summary>
        /// <param name="value"></param>
        protected void OnValueChanged(bool value)
        {
            switch (_switchAcitveType)
            {
                case ESwitchActiveType.ActiveLinkOn:
                    {
                        _gameObjects.ForEach(go => SetActive(go, value));
                        childrenGameObjects.ForEach(go => SetActive(go, value));
                        break;
                    }
                case ESwitchActiveType.ActiveLinkOff:
                    {
                        _gameObjects.ForEach(go => SetActive(go, !value));
                        childrenGameObjects.ForEach(go => SetActive(go, !value));
                        break;
                    }
                case ESwitchActiveType.ActiveWhenOn:
                    {
                        if (value)
                        {
                            _gameObjects.ForEach(go => SetActive(go, true));
                            childrenGameObjects.ForEach(go => SetActive(go, true));
                        }
                        break;
                    }
                case ESwitchActiveType.AcitveWhenOff:
                    {
                        if (!value)
                        {
                            _gameObjects.ForEach(go => SetActive(go, true));
                            childrenGameObjects.ForEach(go => SetActive(go, true));
                        }
                        break;
                    }
                case ESwitchActiveType.DisactiveWhenOn:
                    {
                        if (value)
                        {
                            _gameObjects.ForEach(go => SetActive(go, false));
                            childrenGameObjects.ForEach(go => SetActive(go, false));
                        }
                        break;
                    }
                case ESwitchActiveType.DisactiveWhenOff:
                    {
                        if (!value)
                        {
                            _gameObjects.ForEach(go => SetActive(go, false));
                            childrenGameObjects.ForEach(go => SetActive(go, false));
                        }
                        break;
                    }
                default: break;
            }
        }

        /// <summary>
        /// 切换激活类型
        /// </summary>
        public enum ESwitchActiveType
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 开激活
            /// </summary>
            [Name("开激活")]
            [Tip("开时激活;关时非激活")]
            ActiveLinkOn,

            /// <summary>
            /// 关激活
            /// </summary>
            [Name("关激活")]
            [Tip("关时激活;开时非激活")]
            ActiveLinkOff,

            /// <summary>
            /// 开时激活
            /// </summary>
            [Name("开时激活")]
            ActiveWhenOn,

            /// <summary>
            /// 关时激活
            /// </summary>
            [Name("关时激活")]
            AcitveWhenOff,

            /// <summary>
            /// 开时非激活
            /// </summary>
            [Name("开时非激活")]
            DisactiveWhenOn,

            /// <summary>
            /// 关时非激活
            /// </summary>
            [Name("关时非激活")]
            DisactiveWhenOff,
        }
    }
}
