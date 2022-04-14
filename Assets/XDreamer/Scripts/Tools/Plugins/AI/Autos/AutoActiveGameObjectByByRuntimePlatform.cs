using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.AI.Autos
{
    [XCSJ.Attributes.Icon(EIcon.GameObjectActive)]
    [Tool("AI")]
    [Name("自动激活游戏对象(根据运行时平台)")]
    [RequireManager(typeof(ToolsManager))]
    public class AutoActiveGameObjectByByRuntimePlatform : ToolMB, IOnEnable, IUpdate
    {
        [Name("游戏对象列表")]
        [Tip("期望自动激活的游戏对象列表")]
        public List<GameObject> gameObjects = new List<GameObject>();

        [Name("运行时平台列表")]
        public List<RuntimePlatform> runtimePlatforms = new List<RuntimePlatform>();

        [Name("取反")]
        [Tip("为True时，设置启用变为设置禁用，设置禁用变为设置启用；为False时，不做处理；")]
        public bool reverse = false;

        [Name("保持一致")]
        [Tip("为True时，符合设置的运行时则设置激活，不符合设置的运行时则设置停用；为False时，符合设置的运行时则设置激活，不符合设置的运行时不做处理；")]
        public bool keepSame = true;

        [Name("总是激活")]
        [Tip("为True时，会一直尝试激活期望的游戏对象；为False时，仅在当前组件启用时尝试激活一次；")]
        public bool alwaysActive = false;

        private void Active(GameObject gameObject, bool active)
        {
            if (!gameObject) return;
            if (keepSame || active)
            {
                gameObject.SetActive(reverse ? (!active) : active);
            }
        }

        private void ActiveGameObjects()
        {
            var active = runtimePlatforms.Any(p => p == Application.platform);
            foreach (var go in gameObjects)
            {
                Active(go, active);
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            ActiveGameObjects();
        }

        public void Update()
        {
            if (alwaysActive) ActiveGameObjects();
        }
    }
}
