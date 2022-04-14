using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.AI.Autos
{
    /// <summary>
    /// 自动启用组件(根据运行时平台)
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.ComponentEnable)]
    [Tool("AI")]
    [Name("自动启用组件(根据运行时平台)")]
    [RequireManager(typeof(ToolsManager))]
    public class AutoEnableComponentByRuntimePlatform : ToolMB, IOnEnable, IUpdate
    {
        /// <summary>
        /// 组件列表:期望在特定运行时平台才启用的组件列表
        /// </summary>
        [Name("组件列表")]
        [Tip("期望在特定运行时平台才启用的组件列表")]
        [ComponentPopup(searchFlags = ESearchFlags.Default, displayOnRuntime = false)]
        public List<Component> components = new List<Component>();

        /// <summary>
        /// 运行时平台列表
        /// </summary>
        [Name("运行时平台列表")]
        public List<RuntimePlatform> runtimePlatforms = new List<RuntimePlatform>();

        /// <summary>
        /// 取反:为True时，设置启用变为设置禁用，设置禁用变为设置启用；为False时，不做处理；
        /// </summary>
        [Name("取反")]
        [Tip("为True时，设置启用变为设置禁用，设置禁用变为设置启用；为False时，不做处理；")]
        public bool reverse = false;

        /// <summary>
        /// 保持一致:为True时，符合设置的运行时则设置启用，不符合设置的运行时则设置禁用；为False时，符合设置的运行时则设置启用，不符合设置的运行时不做处理；
        /// </summary>
        [Name("保持一致")]
        [Tip("为True时，符合设置的运行时则设置启用，不符合设置的运行时则设置禁用；为False时，符合设置的运行时则设置启用，不符合设置的运行时不做处理；")]
        public bool keepSame = true;

        /// <summary>
        /// 总是启用
        /// </summary>
        [Name("总是启用")]
        [Tip("为True时，会一直尝试启用期望的组件；为False时，仅在当前组件启用时尝试启用一次；")]
        public bool alwaysEnable = false;

        private void EnableComponents()
        {
            var enable = runtimePlatforms.Any(p => p == Application.platform);
            foreach (var c in components)
            {
                EnableComponent(c, enable);
            }
        }

        private void EnableComponent(Component component, bool enable)
        {
            if (keepSame || enable)
            {
                component.XSetEnable(reverse ? (!enable) : enable);
            }
        }

        /// <summary>
        /// 启用时
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
            EnableComponents();
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (alwaysEnable) EnableComponents();
        }
    }
}
