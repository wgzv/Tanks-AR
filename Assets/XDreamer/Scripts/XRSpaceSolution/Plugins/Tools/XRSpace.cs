using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools;

namespace XCSJ.PluginXRSpaceSolution.Tools
{
    /// <summary>
    /// XR空间:用于构建单机单（多）通道环境的XR空间功能组件
    /// </summary>
    [Name("XR空间")]
    [RequireManager(typeof(XRSpaceSolutionManager))]
    public class XRSpace : ToolMB
    {
        /// <summary>
        /// 空间偏移
        /// </summary>
        [Name("空间偏移")]
        public Transform _spaceOffset;

        /// <summary>
        /// 空间偏移
        /// </summary>
        public Transform spaceOffset
        {
            get
            {
                if (!_spaceOffset)
                {
                    var sot = transform.Find("空间偏移");
                    if (sot)
                    {
                        this.XModifyProperty(ref _spaceOffset, sot);
                    }
                    else if (transform.childCount == 1)//如果只有一个子级对象，直接使用该对象
                    {
                        this.XModifyProperty(ref _spaceOffset, transform.GetChild(0));
                    }
                }
                return _spaceOffset;
            }
            set => this.XModifyProperty(ref _spaceOffset, value);
        }

        /// <summary>
        /// 屏幕组
        /// </summary>
        [Name("屏幕组")]
        public ScreenGroup _screenGroup;

        /// <summary>
        /// 屏幕组
        /// </summary>
        public ScreenGroup screenGroup => this.XGetComponentInChildren(ref _screenGroup);

        /// <summary>
        /// XR装备拥有者
        /// </summary>
        [Name("XR装备拥有者")]
        public XROriginOwner _rigOwner;

        /// <summary>
        /// XR装备拥有者
        /// </summary>
        public XROriginOwner rigOwner => this.XGetComponentInChildren(ref _rigOwner);

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            if (spaceOffset) { }
            if (screenGroup) { }
            if (rigOwner) { }
        }
    }
}
