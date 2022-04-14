using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Tools.Base;

namespace XCSJ.PluginsCameras.Controllers
{
    /// <summary>
    /// 相机移动器
    /// </summary>
    [Name("相机移动器")]
    [DisallowMultipleComponent]
    public class CameraMover : BaseCameraMover
    {
        #region 移动模式

        /// <summary>
        /// 一帧内需要处理的位移列表；在更新时做处理；
        /// </summary>
        Dictionary<EMoveMode, Vector3> offsets = new Dictionary<EMoveMode, Vector3>();

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="value"></param>
        /// <param name="moveMode"></param>
        public override void Move(Vector3 value, int moveMode) => Move(value, (EMoveMode)moveMode);

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="value"></param>
        /// <param name="moveMode"></param>
        public void Move(Vector3 value, EMoveMode moveMode)
        {
            if (offsets.TryGetValue(moveMode, out var tmpValue))//有则累加
            {
                offsets[moveMode] = tmpValue + value;
            }
            else//无则赋值
            {
                offsets[moveMode] = value;
            }
        }

        private void TranslateInternal(Vector3 offset, EMoveMode moveMode)
        {
            switch (moveMode)
            {
                case EMoveMode.Self_Local:
                    {
                        cameraTransformer.Translate(offset, Space.Self);
                        break;
                    }
                case EMoveMode.Self_World:
                    {
                        cameraTransformer.Translate(offset, Space.World);
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

            offsets.Clear();
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            foreach (var kv in offsets)
            {
                TranslateInternal(kv.Value, kv.Key);
            }
            offsets.Clear();
        }

        #endregion
    }

    /// <summary>
    /// 移动模式：Self自身表示<see cref="CameraTransformer"/>指向的真实对象；Target目标表示<see cref="CameraTargetController"/>指向的真实目标对象；
    /// </summary>
    [Name("移动模式")]
    public enum EMoveMode
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None = 0,

        /// <summary>
        /// 自身本地
        /// </summary>
        [Name("自身本地")]
        Self_Local,

        /// <summary>
        /// 自身世界
        /// </summary>
        [Name("自身世界")]
        Self_World,
    }
}
