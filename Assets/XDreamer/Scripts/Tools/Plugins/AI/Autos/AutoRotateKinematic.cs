using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.AI.Autos
{
    /// <summary>
    /// 刚体自动旋转
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Rotate)]
    [Name("刚体自动旋转")]
    //[RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class AutoRotateKinematic : AutoWait
    {
        #region 字段

        /// <summary>
        /// 3D刚体：用于控制自动旋转的对象；如本参数无效，则使用当前组件所在游戏对象上当前参数类型的组件对象
        /// </summary>
        [Group("移动")]
        [Name("3D刚体")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Tip("用于控制自动旋转的对象；如本参数无效，则使用当前组件所在游戏对象上当前参数类型的组件对象")]
        [ComponentPopup]
        public Rigidbody _rigidbody3D;

        /// <summary>
        /// 3D刚体
        /// </summary>
        public Rigidbody rigidbody3D
        {
            get
            {
                if (!_rigidbody3D)
                {
                    _rigidbody3D = GetComponent<Rigidbody>();
                }
                return _rigidbody3D;
            }
        }

        /// <summary>
        /// 旋转变换
        /// </summary>
        public Transform rotateTransform => rigidbody3D ? rigidbody3D.transform : null;

        /// <summary>
        /// 旋转速度
        /// </summary>
        [Name("旋转速度")]
        [SerializeField]
        private float _rotationSpeed = 30.0f;

        #endregion

        #region PRIVATE FIELDS      

        /// <summary>
        /// 标识是否是运动学刚体
        /// </summary>
        public bool isKinematic { get; private set; } = false;

        /// <summary>
        /// 起始欧拉角度
        /// </summary>
        public Vector3 startAngle { get; private set; }

        private float _angle;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// 旋转速度
        /// </summary>
        public float rotationSpeed
        {
            get { return _rotationSpeed; }
            set { _rotationSpeed = Mathf.Clamp(value, -360.0f, 360.0f); }
        }

        /// <summary>
        /// 角度
        /// </summary>
        public float angle
        {
            get { return _angle; }
            set { _angle = value.WrapDegrees(); }
        }

        #endregion

        #region MONOBEHAVIOUR

        /// <summary>
        /// 验证数据
        /// </summary>
        public void OnValidate()
        {
            rotationSpeed = _rotationSpeed;
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        public override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// 启用时
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            if (!rigidbody3D) return;
            isKinematic = rigidbody3D.isKinematic;
            rigidbody3D.isKinematic = true;
            startAngle = rotateTransform.localEulerAngles;
        }

        /// <summary>
        /// 禁用时
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            if (!rigidbody3D) return;
            rigidbody3D.isKinematic = isKinematic;
            rotateTransform.localEulerAngles = startAngle;
        }

        /// <summary>
        /// 固定更新
        /// </summary>
        public void FixedUpdate()
        {
            if (!canUpdate || !rigidbody3D) return;

            angle += rotationSpeed * Time.fixedDeltaTime;

            var rotation = Quaternion.Euler(0.0f, angle, 0.0f);
            _rigidbody3D.MoveRotation(rotation);
        }

        #endregion
    }
}
