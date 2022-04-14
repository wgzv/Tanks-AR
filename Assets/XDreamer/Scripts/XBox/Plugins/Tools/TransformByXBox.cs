using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXBox.Base;

namespace XCSJ.PluginXBox.Tools
{
    /// <summary>
    /// 变换通过XBox
    /// </summary>
    [Name("变换通过XBox")]
    [Tip("默认通过XBox的轴或按钮控制变换的移动、旋转、缩放")]
    [Tool(XBoxHelper.Title)]
    [XCSJ.Attributes.Icon(EIcon.Model)]
    [RequireManager(typeof(XBoxManager))]
    public class TransformByXBox : MB
    {
        /// <summary>
        /// 目标变换
        /// </summary>
        [Name("目标变换")]
        public Transform _targetTransform;

        /// <summary>
        /// 目标变换
        /// </summary>
        public Transform targetTransform
        {
            get
            {
                if(!_targetTransform)
                {
                    _targetTransform = this.transform;
                }
                return _targetTransform;
            }
        }

        /// <summary>
        /// 轴与按钮数据
        /// </summary>
        [Name("轴与按钮数据")]
        public XBoxAxisAndButtonData _axisAndButtonData = new XBoxAxisAndButtonData();

        /// <summary>
        /// XBox控制器轴与按钮数据
        /// </summary>
        [Serializable]
        public class XBoxAxisAndButtonData
        {
            /// <summary>
            /// X减:对应X值减小
            /// </summary>
            [Name("X减")]
            [Tip("对应X值减小")]
            [EnumPopup]
            public EXBoxAxisAndButton _nx = EXBoxAxisAndButton.LeftStickLeft;

            /// <summary>
            /// X增:对应X值增加
            /// </summary>
            [Name("X增")]
            [Tip("对应X值增加")]
            [EnumPopup]
            public EXBoxAxisAndButton _px = EXBoxAxisAndButton.LeftStickRight;

            /// <summary>
            /// Y减:对应Y值减小
            /// </summary>
            [Name("Y减")]
            [Tip("对应Y值减小")]
            [EnumPopup]
            public EXBoxAxisAndButton _ny = EXBoxAxisAndButton.DpadDown;

            /// <summary>
            /// Y增:对应Y值增加
            /// </summary>
            [Name("Y增")]
            [Tip("对应Y值增加")]
            [EnumPopup]
            public EXBoxAxisAndButton _py = EXBoxAxisAndButton.DpadUp;

            /// <summary>
            /// Z减:对应Z值减小
            /// </summary>
            [Name("Z减")]
            [Tip("对应Z值减小")]
            [EnumPopup]
            public EXBoxAxisAndButton _nz = EXBoxAxisAndButton.LeftStickDown;

            /// <summary>
            /// Z增:对应Z值增加
            /// </summary>
            [Name("Z增")]
            [Tip("对应Z值增加")]
            [EnumPopup]
            public EXBoxAxisAndButton _pz = EXBoxAxisAndButton.LeftStickUp;

            /// <summary>
            /// 设置默认移动
            /// </summary>
            public void SetDefaultMove()
            {
                _nx = EXBoxAxisAndButton.LeftStickLeft;
                _px = EXBoxAxisAndButton.LeftStickRight;
                _ny = EXBoxAxisAndButton.DpadDown;
                _py = EXBoxAxisAndButton.DpadUp;
                _nz = EXBoxAxisAndButton.LeftStickDown;
                _pz = EXBoxAxisAndButton.LeftStickUp;
            }

            /// <summary>
            /// 设置默认旋转X
            /// </summary>
            public void SetDefaultRotateX()
            {
                _nx = EXBoxAxisAndButton.RightStickUp;
                _px = EXBoxAxisAndButton.RightStickDown;
            }

            /// <summary>
            /// 设置默认旋转Y
            /// </summary>
            public void SetDefaultRotateY()
            {
                _ny = EXBoxAxisAndButton.RightStickLeft;
                _py = EXBoxAxisAndButton.RightStickRight;
            }

            /// <summary>
            /// 设置全部无
            /// </summary>
            public void SetAllNone()
            {
                _nx = EXBoxAxisAndButton.None;
                _px = EXBoxAxisAndButton.None;
                _ny = EXBoxAxisAndButton.None;
                _py = EXBoxAxisAndButton.None;
                _nz = EXBoxAxisAndButton.None;
                _pz = EXBoxAxisAndButton.None;
            }

            private float GetValue(EXBoxAxisAndButton axisAndButton)
            {
                var value = axisAndButton.GetValue();
                return value < _deadZone ? 0 : value;
            }

            /// <summary>
            /// 死区
            /// </summary>
            [Name("死区")]
            [Tip("死区值，低于本值时不认为对应事件触发;")]
            [Range(0, 1)]
            public float _deadZone = 0.125f;

            /// <summary>
            /// 获取偏移值
            /// </summary>
            /// <returns></returns>
            public Vector3 GetOffset() => GetOffset(Vector3.zero);

            /// <summary>
            /// 获取偏移值
            /// </summary>
            /// <param name="offset"></param>
            /// <returns></returns>
            public Vector3 GetOffset(Vector3 offset) => GetOffset(ref offset);

            /// <summary>
            /// 获取偏移值
            /// </summary>
            /// <param name="offset"></param>
            /// <returns></returns>
            public Vector3 GetOffset(ref Vector3 offset)
            {
                offset.x += -GetValue(_nx) + GetValue(_px);
                offset.y += -GetValue(_ny) + GetValue(_py);
                offset.z += -GetValue(_nz) + GetValue(_pz);
                return offset;
            }
        }

        /// <summary>
        /// 速度
        /// </summary>
        [Name("速度")]
        public Vector3 _speed = Vector3.one;

        /// <summary>
        /// 变换类型
        /// </summary>
        public enum ETransformType
        {
            /// <summary>
            /// 位置
            /// </summary>
            [Name("位置")]
            Position,

            /// <summary>
            /// 旋转
            /// </summary>
            [Name("旋转")]
            Rotation,

            /// <summary>
            /// 缩放
            /// </summary>
            [Name("缩放")]
            Scale,
        }

        /// <summary>
        /// 变换类型
        /// </summary>
        [Name("变换类型")]
        [EnumPopup]
        public ETransformType _transformType = ETransformType.Position;

        /// <summary>
        /// 空间类型
        /// </summary>
        [Name("空间类型")]
        [EnumPopup]
        public ESpaceType _spaceType = ESpaceType.Local;

        /// <summary>
        /// 设置默认移动
        /// </summary>
        public void SetDefaultMove()
        {
            this.XModifyProperty(() =>
            {
                _transformType = ETransformType.Position;
                _spaceType = ESpaceType.Local;
                _axisAndButtonData.SetDefaultMove();
                _speed = Vector3.one;
            });
        }

        /// <summary>
        /// 设置默认旋转世界Y
        /// </summary>
        public void SetDefaultRotateWorldY()
        {
            this.XModifyProperty(() =>
            {
                _transformType = ETransformType.Rotation;
                _spaceType = ESpaceType.World;
                _axisAndButtonData.SetAllNone();
                _axisAndButtonData.SetDefaultRotateY();
                _speed = new Vector3(1, 20, 1);
            });
        }

        /// <summary>
        /// 设置默认旋转本地X
        /// </summary>
        public void SetDefaultRotateLocalX()
        {
            this.XModifyProperty(() =>
            {
                _transformType = ETransformType.Rotation;
                _spaceType = ESpaceType.Local;
                _axisAndButtonData.SetAllNone();
                _axisAndButtonData.SetDefaultRotateX();
                _speed = new Vector3(20, 1, 1);
            });
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            if (targetTransform) { }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (!_targetTransform) return;
            var offset = Vector3.Scale(_axisAndButtonData.GetOffset(), _speed) * Time.deltaTime;
            switch (_transformType)
            {
                case ETransformType.Position:
                    {
                        _targetTransform.Translate(offset, (Space)_spaceType);
                        break;
                    }
                case ETransformType.Rotation:
                    {
                        _targetTransform.Rotate(offset, (Space)_spaceType);
                        break;
                    }
                case ETransformType.Scale:
                    {
                        if (_spaceType == ESpaceType.Local)
                        {
                            _targetTransform.localScale += offset;
                        }
                        break;
                    }
            }
        }
    }
}
