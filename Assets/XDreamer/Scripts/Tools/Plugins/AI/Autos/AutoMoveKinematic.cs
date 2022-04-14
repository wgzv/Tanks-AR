using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.AI.Autos
{
    /// <summary>
    /// 刚体自动移动
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Move)]
    [Name("刚体自动移动")]
    //[RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class AutoMoveKinematic : AutoWait
    {
        #region 字段

        /// <summary>
        /// 3D刚体：用于控制自动移动的对象；如本参数无效，则使用当前组件所在游戏对象上当前参数类型的组件对象
        /// </summary>
        [Group("移动")]
        [Name("3D刚体")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Tip("用于控制自动移动的对象；如本参数无效，则使用当前组件所在游戏对象上当前参数类型的组件对象")]
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
        /// 移动变换
        /// </summary>
        public Transform moveTransform => rigidbody3D ? rigidbody3D.transform : null;

        /// <summary>
        /// 移动时间：游戏对象移动指定偏移量所需的时间；单位：秒；
        /// </summary>
        [Name("移动时间")]
        [Tip("游戏对象移动指定偏移量所需的时间；单位：秒；")]
        [SerializeField]
        [Range(0.01f, 60f)]
        public float _moveTime = 3.0f;

        /// <summary>
        /// 偏移量：移动偏移量
        /// </summary>
        [Name("偏移量")]
        [Tip("移动偏移量")]
        [SerializeField]
        private Vector3 _offset;

        /// <summary>
        /// 空间：移动偏移量的空间类型
        /// </summary>
        [Name("空间")]
        [Tip("移动偏移量的空间类型")]
        [SerializeField]
        [EnumPopup]
        public ESpaceType _space = ESpaceType.World;

        /// <summary>
        /// 移动类型
        /// </summary>
        [Name("移动类型")]
        [EnumPopup]
        public EMoveType moveType = EMoveType.PingPong;

        /// <summary>
        /// 移动类型
        /// </summary>
        public enum EMoveType
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

        #endregion
        
        /// <summary>
        /// 标识是否时运动学刚体
        /// </summary>
        public bool isKinematic { get; private set; } = false;

        /// <summary>
        /// 起始位置
        /// </summary>
        public Vector3 startPosition { get; private set; }

        /// <summary>
        /// 目标位置
        /// </summary>
        public Vector3 targetPosition => startPosition + worldOffset;

        /// <summary>
        /// 移动事件
        /// </summary>
        public float moveTime
        {
            get { return _moveTime; }
            set { _moveTime = Mathf.Max(0.01f, value); }
        }

        /// <summary>
        /// 偏移量
        /// </summary>
        public Vector3 offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        /// <summary>
        /// 世界偏移量
        /// </summary>
        public Vector3 worldOffset => _space == ESpaceType.World ? offset : moveTransform.TransformDirection(offset);

        /// <summary>
        /// 本地便宜量
        /// </summary>
        public Vector3 localOffset => _space == ESpaceType.Local ? offset : moveTransform.InverseTransformDirection(offset);

        /// <summary>
        /// 验证数据
        /// </summary>
        public void OnValidate()
        {
            moveTime = _moveTime;
        }

        /// <summary>
        /// 唤醒时
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
            startPosition = moveTransform.position;
        }

        /// <summary>
        /// 禁用时
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
            if (!rigidbody3D) return;

            rigidbody3D.isKinematic = isKinematic;
            moveTransform.position = startPosition;
        }

        /// <summary>
        /// 固定更新时
        /// </summary>
        public void FixedUpdate()
        {
            if (!canUpdate || !rigidbody3D) return;

            var t0 = moveType == EMoveType.PingPong ? Mathf.PingPong(Time.time, _moveTime) : Mathf.Repeat(Time.time, _moveTime);
            var t = MathLibrary.EaseInOut(t0, _moveTime);
            var p = Vector3.Lerp(startPosition, targetPosition, t);

            rigidbody3D.MovePosition(p);
        }

        /// <summary>
        /// 绘制选中的Gizmos时
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            var moveTransform = this.moveTransform;
            if (!moveTransform) return;

            var worldOffset = this.worldOffset;
            var centerOffset = Vector3.zero;

            var bakColor = Gizmos.color;
            var useCache = Application.isPlaying && enabled;
            var position = useCache ? startPosition : moveTransform.position;

            var collider = GetComponent<Collider>();
            if (collider)
            {
                Gizmos.color = Color.green;
                Gizmos.matrix = Matrix4x4.TRS(position, moveTransform.rotation, Vector3.one);
                if (collider is BoxCollider box)
                {
                    centerOffset = moveTransform.TransformDirection(box.center);
                    Gizmos.DrawWireCube(box.center, box.size);
                    Gizmos.DrawWireCube(box.center + localOffset, box.size);
                }
                else if (collider is SphereCollider sphere)
                {
                    centerOffset = moveTransform.TransformDirection(sphere.center);
                    Gizmos.DrawWireSphere(sphere.center, sphere.radius);
                    Gizmos.DrawWireSphere(sphere.center + localOffset, sphere.radius);
                }
                else if (collider is CapsuleCollider capsule)
                {
                    centerOffset = moveTransform.TransformDirection(capsule.center);
                    var size = new Vector3(capsule.radius * 2, capsule.height, capsule.radius * 2);
                    Gizmos.DrawWireCube(capsule.center, size);
                    Gizmos.DrawWireCube(capsule.center + localOffset, size);
                }
                Gizmos.matrix = Matrix4x4.identity;

                Gizmos.DrawLine(position + centerOffset, position + centerOffset + worldOffset);
            }

            if (centerOffset != Vector3.zero)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(position, position + worldOffset);
            }

            Gizmos.color = bakColor;
        }
    }
}
