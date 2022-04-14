using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;
using XCSJ.PluginCommonUtils;
using IconAttribute = XCSJ.Attributes.IconAttribute;

namespace XCSJ.PluginTools.AI.Autos
{
    /// <summary>
    /// 自动移动
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Move)]
    [Name("自动移动")]
    [DisallowMultipleComponent]
    public class AutoMove : AutoWait
    {
        /// <summary>
        /// 移动变换
        /// </summary>：用于控制自动移动的对象；如本参数无效，则使用当前组件所在游戏对象上当前参数类型的组件对象
        [Group("移动")]
        [Name("移动变换")]
        [Tip("用于控制自动移动的对象；如本参数无效，则使用当前组件所在游戏对象上当前参数类型的组件对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _moveTransform;

        /// <summary>
        /// 移动变换
        /// </summary>
        public Transform moveTransform
        {
            get
            {
                if (!_moveTransform)
                {
                    _moveTransform = transform;
                }
                return _moveTransform;
            }
        }

        /// <summary>
        /// 移动时间:游戏对象移动指定偏移量所需的时间；单位：秒；
        /// </summary>
        [Name("移动时间")]
        [Tip("游戏对象移动指定偏移量所需的时间；单位：秒；")]
        public float moveTime = 3.0f;

        /// <summary>
        /// 偏移量:移动偏移量
        /// </summary>
        [Name("偏移量")]
        [Tip("移动偏移量")]
        public Vector3 offset = Vector3.up;

        /// <summary>
        /// 空间
        /// </summary>
        [Name("空间")]
        public Space space = Space.World;

        /// <summary>
        /// 移动类型
        /// </summary>
        [Name("移动类型")]
        [EnumPopup]
        public ELoopType moveType = ELoopType.PingPong;

        private Vector3 _startPosition;

        /// <summary>
        /// 启用时
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            _startPosition = moveTransform.position;
        }

        /// <summary>
        /// 禁用时
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            moveTransform.position = _startPosition;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public override void Update()
        {
            base.Update();
            if (!canUpdate) return;

            var t0 = moveType == ELoopType.PingPong ? Mathf.PingPong(Time.time, moveTime) : Mathf.Repeat(Time.time, moveTime);
            var t = MathLibrary.EaseInOut(t0, moveTime);
            var p = Vector3.Lerp(Vector3.zero, offset, t);

            moveTransform.position = _startPosition;
            moveTransform.Translate(p, space);
        }

        /// <summary>
        /// 循环类型
        /// </summary>
        public enum ELoopType
        {
            /// <summary>
            /// 重复
            /// </summary>
            [Name("重复")]
            Repeat,

            /// <summary>
            /// 乒乓
            /// </summary>
            [Name("乒乓")]
            PingPong,
        }
    }
}
