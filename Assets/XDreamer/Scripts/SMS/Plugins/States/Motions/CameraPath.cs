using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginsCameras.Legacies;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 相机路径:相机路径组件是相机的路径动画，在给定的时间内，相机沿着世界绝对路径移动，移动完成后，组件切换为完成态。
    /// </summary>
    [ComponentMenu("动作/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(CameraPath))]
    [Tip("相机路径组件是相机的路径动画，在给定的时间内，相机沿着世界绝对路径移动，移动完成后，组件切换为完成态。")]
    [XCSJ.Attributes.Icon(index = 33615)]
    [Obsolete("产品功能升级，不推荐用户再使用本功能组件，请使用新的功能组件替代！", false)]
    public class CameraPath : Path<CameraPath>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "相机路径";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/"+ Title, typeof(SMSManager))]
        [Name(Title, nameof(CameraPath))]
        [Tip("相机路径组件是相机的路径动画，在给定的时间内，相机沿着世界绝对路径移动，移动完成后，组件切换为完成态。")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("使用当前相机")]
        public bool useCurrentCamera = true;

        [Name("相机")]
        [HideInSuperInspector(nameof(useCurrentCamera), EValidityCheckType.Equal, true)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public BaseCamera camera;

        #region IPath

        public override List<Transform> transforms
        {
            get
            {
                var list = new List<Transform>();
                if (useCurrentCamera)
                {
                    if (CameraManager.instance && CameraManager.instance.GetInitCamera() is BaseCamera baseCamera && baseCamera)
                    {
                        list.Add(baseCamera.transform);
                    }
                    else if (Camera.main && Camera.main.GetComponent<BaseCamera>() is BaseCamera tmpCamera && tmpCamera)
                    {
                        list.Add(tmpCamera.transform);
                    }
                }
                else if (camera)
                {
                    list.Add(camera.transform);
                }
                return list;
            }
        }

        public override void AddTransform(Transform transform)
        {
            if (transform && !camera)
            {
                camera = transform.GetComponent<BaseCamera>();
                if (camera)
                {
                    useCurrentCamera = false;
                }
            }
        }

        #endregion

        public override bool Init(StateData data)
        {
            if (useCurrentCamera && useInitData && CameraManager.instance)
            {
                camera = CameraManager.instance.GetCurrentCamera();
            }
            return base.Init(data);
        }

        public override void OnEntry(StateData data)
        {
            if (useCurrentCamera && !useInitData && CameraManager.instance)
            {
                camera = CameraManager.instance.GetCurrentCamera();
            }
            base.OnEntry(data);
        }

        public override bool DataValidity()
        {
            return (useCurrentCamera || camera) && base.DataValidity();
        }
    }
}
