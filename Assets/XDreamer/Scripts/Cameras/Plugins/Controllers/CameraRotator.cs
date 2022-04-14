using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.PluginsCameras.Controllers
{
    /// <summary>
    /// 相机旋转器
    /// </summary>
    [Name("相机旋转器")]
    [DisallowMultipleComponent]
    public class CameraRotator : BaseCameraRotator 
    {
        #region 旋转模式

        /// <summary>
        /// 一帧内需要处理的旋转角度列表；在更新时做处理；
        /// </summary>
        Dictionary<ERotateMode, Vector3> angles = new Dictionary<ERotateMode, Vector3>();

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="moveMode"></param>
        public override void Rotate(Vector3 value, int moveMode) => Rotate(value, (ERotateMode)moveMode);

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rotateMode"></param>
        public void Rotate(Vector3 value, ERotateMode rotateMode)
        {
            if (angles.TryGetValue(rotateMode, out var tmpValue))//有则累加
            {
                angles[rotateMode] = tmpValue + value;
            }
            else//无则赋值
            {
                angles[rotateMode] = value;
            }
        }

        private void RotateInternal(Vector3 offset, ERotateMode rotateMode)
        {
            switch (rotateMode)
            {
                case ERotateMode.Self_Local:
                    {
                        cameraTransformer.Rotate(offset, Space.Self);
                        break;
                    }
                case ERotateMode.Self_World:
                    {
                        cameraTransformer.Rotate(offset, Space.World);
                        break;
                    }
                case ERotateMode.Self_LocalXZ__Self_WorldY:
                    {
                        var cameraTransformer = this.cameraTransformer;

                        cameraTransformer.Rotate(new Vector3(offset.x, 0, offset.z), Space.Self);
                        cameraTransformer.Rotate(new Vector3(0, offset.y, 0), Space.World);
                        break;
                    }
                case ERotateMode.Target_Local:
                    {
                        var cameraTransformer = this.cameraTransformer;
                        var cameraTargetController = this.cameraController.cameraTargetController;
                        if (!cameraTransformer || !cameraTargetController) break;

                        var targetPosition = cameraTargetController.targetPosition;

                        cameraTransformer.RotateAround(targetPosition, cameraTransformer.right, offset.x);
                        cameraTransformer.RotateAround(targetPosition, cameraTransformer.up, offset.y);
                        cameraTransformer.RotateAround(targetPosition, cameraTransformer.forward, offset.z);
                        break;
                    }
                case ERotateMode.Target_World:
                    {
                        var cameraTransformer = this.cameraTransformer;
                        var cameraTargetController = this.cameraController.cameraTargetController;
                        if (!cameraTransformer || !cameraTargetController) break;

                        var targetPosition = cameraTargetController.targetPosition;

                        cameraTransformer.RotateAround(targetPosition, Vector3.right, offset.x);
                        cameraTransformer.RotateAround(targetPosition, Vector3.up, offset.y);
                        cameraTransformer.RotateAround(targetPosition, Vector3.forward, offset.z);
                        break;
                    }
                case ERotateMode.Target_LocalXZ__Targert_WorldY:
                    {
                        var cameraTransformer = this.cameraTransformer;
                        var cameraTargetController = this.cameraController.cameraTargetController;
                        if (!cameraTransformer || !cameraTargetController) break;

                        var targetPosition = cameraTargetController.targetPosition;

                        cameraTransformer.RotateAround(targetPosition, cameraTransformer.right, offset.x);
                        cameraTransformer.RotateAround(targetPosition, Vector3.up, offset.y);
                        cameraTransformer.RotateAround(targetPosition, cameraTransformer.forward, offset.z);
                        break;
                    }
            }
        }

        #endregion

        #region MB方法

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            angles.Clear();
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            foreach (var kv in angles)
            {
                RotateInternal(kv.Value, kv.Key);
            }
            angles.Clear();
        }

        #endregion
    }

    /// <summary>
    /// 旋转模式：Self自身表示<see cref="CameraTransformer"/>指向的真实对象；Target目标表示<see cref="CameraTargetController"/>指向的真实目标对象；
    /// </summary>
    [Name("旋转模式")]
    public enum ERotateMode
    {
        /// <summary>
        /// 无:不做任何旋转
        /// </summary>
        [Name("无")]
        [Tip("不做任何旋转")]
        None = 0,

        /// <summary>
        /// 自身本地:以自身为旋转中心，本地坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑;
        /// </summary>
        [Name("自身本地")]
        [Tip("以自身为旋转中心，本地坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑;")]
        Self_Local,

        /// <summary>
        /// 自身世界:以自身为旋转中心，世界坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑
        /// </summary>
        [Name("自身世界")]
        [Tip("以自身为旋转中心，世界坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑")]
        Self_World,

        /// <summary>
        /// 自身本地XZ自身世界Y:以自身为旋转中心，本地坐标系的XZ轴(右轴right对应X轴,forward对应Z轴)为旋转轴，执行旋转逻辑;以自身为旋转中心，世界坐标系的Y轴(上轴up对应Y轴)为旋转轴，执行旋转逻辑;
        /// </summary>
        [Name("自身本地XZ自身世界Y")]
        [Tip("以自身为旋转中心，本地坐标系的XZ轴(右轴right对应X轴,forward对应Z轴)为旋转轴，执行旋转逻辑;以自身为旋转中心，世界坐标系的Y轴(上轴up对应Y轴)为旋转轴，执行旋转逻辑;")]
        Self_LocalXZ__Self_WorldY,

        /// <summary>
        /// 目标本地:以目标为旋转中心，本地坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑;
        /// </summary>
        [Name("目标本地")]
        [Tip("以目标为旋转中心，本地坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑;")]
        Target_Local,

        /// <summary>
        /// 目标世界:以目标为旋转中心，世界坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑;
        /// </summary>
        [Name("目标世界")]
        [Tip("以目标为旋转中心，世界坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑;")]
        Target_World,

        /// <summary>
        /// 目标本地XZ目标世界Y:以目标为旋转中心，本地坐标系的XZ轴(右轴right对应X轴,forward对应Z轴)为旋转轴，执行旋转逻辑;以目标为旋转中心，世界坐标系的Y轴(上轴up对应Y轴)为旋转轴，执行旋转逻辑;
        /// </summary>
        [Name("目标本地XZ目标世界Y")]
        [Tip("以目标为旋转中心，本地坐标系的XZ轴(右轴right对应X轴,forward对应Z轴)为旋转轴，执行旋转逻辑;以目标为旋转中心，世界坐标系的Y轴(上轴up对应Y轴)为旋转轴，执行旋转逻辑;")]
        Target_LocalXZ__Targert_WorldY,
    }
}
