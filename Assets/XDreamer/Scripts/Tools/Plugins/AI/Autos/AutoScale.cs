using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;
using XCSJ.PluginCommonUtils;
using static XCSJ.PluginTools.AI.Autos.AutoMove;

namespace XCSJ.PluginTools.AI.Autos
{
    [XCSJ.Attributes.Icon(EIcon.Scale)]
    [Name("自动缩放")]
    [DisallowMultipleComponent]
    public class AutoScale : AutoWait
    {
        /// <summary>
        /// 缩放变换：用于控制自动缩放的对象；如本参数无效，则使用当前组件所在游戏对象上当前参数类型的组件对象
        /// </summary>
        [Group("缩放")]
        [Name("缩放变换")]
        [Tip("用于控制自动缩放的对象；如本参数无效，则使用当前组件所在游戏对象上当前参数类型的组件对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _scaleTransform;

        /// <summary>
        /// 缩放变换
        /// </summary>
        public Transform scaleTransform
        {
            get
            {
                if (!_scaleTransform)
                {
                    _scaleTransform = transform;
                }
                return _scaleTransform;
            }
        }

        [Name("缩放时间")]
        [Tip("游戏对象缩放指定比例所需的时间；单位：秒；")]
        public float scaleTime = 3.0f;

        [Name("缩放比例")]
        [Tip("基于游戏对象当前缩放值的缩放比例")]
        public Vector3 scaleRatio = Vector3.one;

        [Name("缩放类型")]
        [EnumPopup]
        public ELoopType scaleType = ELoopType.PingPong;

        private Vector3 _startScale;

        /// <summary>
        /// 初始化为自身的变换
        /// </summary>
        protected void Reset()
        {
            if (!_scaleTransform) _scaleTransform = transform;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            _startScale = scaleTransform.localScale;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            scaleTransform.localScale = _startScale;
        }

        public override void Update()
        {
            base.Update();
            if (!canUpdate) return;

            var t0 = scaleType == ELoopType.PingPong ? Mathf.PingPong(Time.time, scaleTime) : Mathf.Repeat(Time.time, scaleTime);
            var t = MathLibrary.EaseInOut(t0, scaleTime);
            var scale = Vector3.Lerp(Vector3.one, scaleRatio, t);

            scale.Scale(_startScale);

            scaleTransform.localScale = scale;
        }
    }
}
