using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;

namespace XCSJ.PluginsCameras.Legacies.Tools
{
    /// <summary>
    /// 相机目标修改(根据选择集)
    /// </summary>
    [Name("相机目标修改(根据选择集)")]
    [RequireComponent(typeof(BaseCamera))]
    [Obsolete("产品功能升级，不推荐用户再使用本功能组件，请使用新的功能组件替代！", false)]
    [RequireManager(typeof(CameraManager))]
    public class CameraTargetModifyBySelection : MB, IAwake, IOnEnable, IOnDisable
    {
        /// <summary>
        /// 选择集游戏对象规则
        /// </summary>
        public enum ESelectionGameObjectRule
        {
            /// <summary>
            /// 无:不做处理
            /// </summary>
            [Name("无")]
            [Tip("不做处理")]
            None = 0,

            /// <summary>
            /// 第一个
            /// </summary>
            [Name("第一个")]
            First,

            /// <summary>
            /// 第一个有效的
            /// </summary>
            [Name("第一个有效的")]
            FirstValid,

            /// <summary>
            /// 最后一个
            /// </summary>
            [Name("最后一个")]
            Last,

            /// <summary>
            /// 最后一个有效的
            /// </summary>
            [Name("最后一个有效的")]
            LastValid,
        }

        /// <summary>
        /// 选择集游戏对象规则:选择集中有多个游戏对象时,选取其中哪个游戏对象作为相机的目标对象;
        /// </summary>
        [Name("选择集游戏对象规则")]
        [Tip("选择集中有多个游戏对象时,选取其中哪个游戏对象作为相机的目标对象;")]
        [EnumPopup]
        public ESelectionGameObjectRule selectionGameObjectRule = ESelectionGameObjectRule.FirstValid;

        /// <summary>
        /// 选择集清理规则
        /// </summary>
        public enum ESelectionClearedRule
        {
            /// <summary>
            /// 无:不做处理
            /// </summary>
            [Name("无")]
            [Tip("不做处理")]
            None,

            /// <summary>
            /// 保持:相机目标保持不变
            /// </summary>
            [Name("保持")]
            [Tip("相机目标保持不变")]
            Keep,

            /// <summary>
            /// 清空:相机目标清空
            /// </summary>
            [Name("清空")]
            [Tip("相机目标清空")]
            Clear,
        }

        /// <summary>
        /// 选择集清空时规则:选择集清空(即选择集中游戏对象被全部被移除)时,相机目标的处理规则;
        /// </summary>
        [Name("选择集清空时规则")]
        [Tip("选择集清空(即选择集中游戏对象被全部被移除)时,相机目标的处理规则;")]
        [EnumPopup]
        public ESelectionClearedRule selectionClearedRule = ESelectionClearedRule.Keep;

        private BaseCamera baseCamera;

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        public void Awake()
        {
            baseCamera = GetComponent<BaseCamera>();
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            UpdateCameraTarget();
            Selection.selectionChanged += OnSelectionChanged;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged(GameObject[] oldSelctions, bool isCmd)
        {
            UpdateCameraTarget();
        }

        private void UpdateCameraTarget()
        {
            try
            {
                if (Selection.Valid())
                {
                    //选择集中存在有效数据
                    switch (selectionGameObjectRule)
                    {
                        case ESelectionGameObjectRule.First:
                            {
                                baseCamera.SetTarget(Selection.first);
                                break;
                            }
                        case ESelectionGameObjectRule.FirstValid:
                            {
                                baseCamera.SetTarget(Selection.firstValid);
                                break;
                            }
                        case ESelectionGameObjectRule.Last:
                            {
                                baseCamera.SetTarget(Selection.last);
                                break;
                            }
                        case ESelectionGameObjectRule.LastValid:
                            {
                                baseCamera.SetTarget(Selection.lastValid);
                                break;
                            }
                    }
                }
                else
                {
                    //选择集被清空
                    switch (selectionClearedRule)
                    {
                        case ESelectionClearedRule.Clear:
                            {
                                baseCamera.SetTarget(null);
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ExceptionFormat("修改相机[{0}]目标时异常:{1}", CommonFun.GameObjectToString(gameObject), ex);
            }
        }
    }
}
