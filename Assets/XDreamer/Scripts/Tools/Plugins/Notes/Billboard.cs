using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Notes
{
    /// <summary>
    /// 广告牌类型
    /// </summary>
    public enum EBillboardType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        [Tip("不受相机朝向影响")]
        None,

        /// <summary>
        /// 朝向相机
        /// </summary>
        [Name("朝向相机")]
        [Tip("当前游戏对象的自身坐标系Z轴始终朝向相机；类似粒子或者精灵朝向相机的效果")]
        LookAtCamera,

        /// <summary>
        /// BB树
        /// </summary>
        [Name("BB树")]
        [Tip("当前游戏对象绕世界坐标系Y轴（与自身坐标系Y轴方向重合）旋转，且其自身坐标系X0Y面（Z轴正方向）始终朝向相机")]
        BillboardTree,

        /// <summary>
        /// 朝向游戏对象
        /// </summary>
        [Name("朝向游戏对象")]
        [Tip("当前游戏对象的自身坐标系Z轴始终朝向游戏对象")]
        LookAtGameObject,

        /// <summary>
        /// 背向相机
        /// </summary>
        [Name("背向相机")]
        [Tip("当前游戏对象的自身坐标系Z轴始终背向相机；与朝向相机的方向相反")]
        BackToCamera,

        /// <summary>
        /// 背向游戏对象
        /// </summary>
        [Name("背向游戏对象")]
        [Tip("当前游戏对象的自身坐标系Z轴始终背向游戏对象；与朝向游戏对象的方向相反")]
        BackToGameObject,
    }

    /// <summary>
    /// 广告牌
    /// </summary>
    [Tool("标注")]
    [Name("广告牌")]
    [RequireManager(typeof(ToolsManager))]
    public class Billboard : ToolMB
    {
        /// <summary>
        /// 广告牌类型
        /// </summary>
        [Name("广告牌类型")]
        [EnumPopup]
        public EBillboardType billboardType = EBillboardType.LookAtCamera;

        /// <summary>
        /// 朝向对象
        /// </summary>
        [Name("朝向对象")]
        [Tip("根据广告牌类型不同，本参数有不同的解释")]
        [HideInSuperInspector(nameof(billboardType), EValidityCheckType.NotEqual | EValidityCheckType.Or, EBillboardType.LookAtGameObject, nameof(billboardType), EValidityCheckType.NotEqual, EBillboardType.BackToGameObject)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject lookAtObject;

        /// <summary>
        /// 启用自动缩放
        /// </summary>
        [Name("启用自动缩放")]
        public bool isScaleEnable = false;

        /// <summary>
        /// 缩放系数
        /// </summary>
        [Name("缩放系数")]
        [Range(0.001f, 100)]
        [HideInSuperInspector(nameof(isScaleEnable), EValidityCheckType.Equal, false)]
        public float scaleRatio = 0.1f;

        /// <summary>
        /// 初始化缩放值
        /// </summary>
        private Vector3 initScaleValue;

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
        {
            initScaleValue = transform.localScale;
        }

        /// <summary>
        /// 延时更新
        /// </summary>
        protected void LateUpdate()
        {
            GameObject go = null;
            switch (billboardType)
            {
                case EBillboardType.None: break;
                case EBillboardType.LookAtCamera:
                case EBillboardType.BackToCamera:
                case EBillboardType.BillboardTree: go = (Camera.main ? Camera.main : Camera.current)?.gameObject; break;
                case EBillboardType.LookAtGameObject:
                case EBillboardType.BackToGameObject: go = lookAtObject; break;
                default: break;
            }

            if (!go) return;
            var pos = go.transform.position;

            if (isScaleEnable)
            {
                float distance = (transform.position - pos).magnitude;
                var scaleValue = Vector3.one * distance * scaleRatio;
                transform.localScale = Vector3.Scale(initScaleValue, scaleValue);
            }

            switch (billboardType)
            {
                case EBillboardType.LookAtCamera:
                case EBillboardType.LookAtGameObject: transform.LookAt(pos); break;
                case EBillboardType.BillboardTree:
                    {
                        var tmpPos = Vector3.ProjectOnPlane(pos, Vector3.up);
                        tmpPos.y += transform.position.y;
                        transform.LookAt(tmpPos);
                        break;
                    }
                case EBillboardType.BackToCamera:
                case EBillboardType.BackToGameObject:
                    {
                        transform.LookAt(transform.position * 2 - pos);
                        break;
                    }
                default: break;
            }
        }
    }
}
