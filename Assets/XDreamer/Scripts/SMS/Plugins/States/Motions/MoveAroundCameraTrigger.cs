using System;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Cameras;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 平移绕物相机触发器
    /// </summary>
    [ComponentMenu("动作/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(MoveAroundCameraTrigger))]
    [XCSJ.Attributes.Icon(index = 33623)]
    [Obsolete("产品功能升级，不推荐用户再使用本功能组件，请使用新的功能组件替代！", false)]
    public class MoveAroundCameraTrigger : WorkClipTriggger<MoveAroundCameraTrigger>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "平移绕物相机触发器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [Name(Title, nameof(MoveAroundCameraTrigger))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateMoveAroundCameraTrigger(IGetStateCollection obj) => CreateNormalState(obj);

        [Group("相机")]
        [Name("是否使用当前相机")]
        [HideInInspector]
        public bool useCurrentCamera = false;

        [Name("相机")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup()]
        [HideInSuperInspector(nameof(useCurrentCamera), EValidityCheckType.True)]
        [FormerlySerializedAs("moveAroundCamera")]
        public MoveAroundCamera camera;

        [Name("是否切相机")]
        [HideInSuperInspector(nameof(useCurrentCamera), EValidityCheckType.True)]
        [HideInInspector]
        public bool switchCamera = true;

        [Name("观察角度")]
        [ValidityCheck(EValidityCheckType.NotNull)]        
        public GameObject lookat;

        [Name("观察目标")]
        public GameObject target;

        public GameObject currentTarget
        {
            get
            {
                return camera && camera.target ? camera.target.gameObject : null;
            }
        }

        protected override void OnTrigger()
        {
            if (camera && lookat)
            {
                camera.SetLookAtAndTarget(lookat, target? target: camera.target.gameObject);
            }
        }

        public override bool DataValidity()
        {
            return camera && lookat && base.DataValidity();
        }
    }
}
