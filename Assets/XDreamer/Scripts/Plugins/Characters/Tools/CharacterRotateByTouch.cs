using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Scripts;

namespace XCSJ.Extension.Characters.Tools
{
    /// <summary>
    /// 角色旋转通过触摸
    /// </summary>
    [Name("角色旋转通过触摸")]
    [Tip("默认通过单指触摸移动的偏移量控制角色的旋转")]
    [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
    [Tool(CharacterHelper.ControllersCategoryName, /*nameof(XCharacterController),*/ nameof(CharacterRotator))]
    public class CharacterRotateByTouch : BaseCharacterRotateController
    {
        #region 触摸

        /// <summary>
        /// 单指触摸模式
        /// </summary>
        [Name("单指触摸模式")]
        [EnumPopup]
        public EOneTouchMode _oneTouchMode = EOneTouchMode.X__Y;

        /// <summary>
        /// 单指触摸模式
        /// </summary>
        public EOneTouchMode oneTouchMode
        {
            get => _oneTouchMode;
            set => this.XModifyProperty(ref _oneTouchMode, value);
        }

        #endregion

        /// <summary>
        /// 在任意UI上时忽略:为True时，如鼠标（手势）当前正在任意的GUI(即IM GUI)或UGUI上时则忽略处理（即不做任何处理），否则继续处理输入；为False时，处理输入；
        /// </summary>
        [Name("在任意UI上时忽略")]
        [Tip("为True时，如鼠标（手势）当前正在任意的GUI(即IM GUI)或UGUI上时则忽略处理（即不做任何处理），否则继续处理输入；为False时，处理输入；")]
        public bool _ignoreWhenOnAnyUI = true;

        public override void Update()
        {
            if (_ignoreWhenOnAnyUI && CommonFun.IsOnAnyUI()) return;

            //分析触摸
            switch (Input.touchCount)
            {
                case 1:
                    {
                        base.Update();
                        if (One(Input.GetTouch(0)))
                        {
                            Rotate();
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _speedInfo.Reset(new Vector3(0, 90, 0));

            _enableRuleByRuntimePlatform.Reset(EBool.No);
            _enableRuleByRuntimePlatform.Reset(RuntimePlatform.Android, EBool.Yes);
            _enableRuleByRuntimePlatform.Reset(RuntimePlatform.IPhonePlayer, EBool.Yes);
        }

        #region 单指

        /// <summary>
        /// 单指触摸模式
        /// </summary>
        [Name("单指触摸模式")]
        public enum EOneTouchMode
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 横向Y
            /// </summary>
            [Name("横向Y")]
            [Tip("横向偏移量控制角色的Y轴旋转")]
            X__Y,
        }

        /// <summary>
        ///单指
        /// </summary>
        /// <param name="touch0"></param>
        /// <returns></returns>
        private bool One(Touch touch0)
        {
            if (touch0.phase != TouchPhase.Moved) return false;
            var speedRealtime = this.speedRealtime;
            var deltaPosition = touch0.deltaPosition;
            switch (_oneTouchMode)
            {
                case EOneTouchMode.X__Y:
                    {
                        _offset.y += deltaPosition.x * speedRealtime.y;
                        break;
                    }
                default: return false;
            }
            return true;
        }

        #endregion
    }
}
