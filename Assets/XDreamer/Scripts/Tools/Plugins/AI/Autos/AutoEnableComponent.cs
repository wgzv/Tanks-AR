using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.AI.Autos
{
    /// <summary>
    /// 自动启用组件
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.ComponentEnable)]
    [Name("自动启用组件")]
    public class AutoEnableComponent : AutoWait
    {
        /// <summary>
        /// 组件列表:期望自动启用的组件列表
        /// </summary>
        [Name("组件列表")]
        [Tip("期望自动启用的组件列表")]
        [ComponentPopup(searchFlags = ESearchFlags.Default, displayOnRuntime = false)]
        public List<Component> components = new List<Component>();

        /// <summary>
        /// 更新
        /// </summary>
        public override void Update()
        {
            base.Update();

            var enable = canUpdate;
            foreach(var c in components)
            {
                c.XSetEnable(enable);
            }
        }
    }
}
