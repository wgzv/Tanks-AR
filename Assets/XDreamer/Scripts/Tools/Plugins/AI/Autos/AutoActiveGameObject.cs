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
    /// 自动激活游戏对象
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.GameObjectActive)]
    [Name("自动激活游戏对象")]
    public class AutoActiveGameObject : AutoWait
    {
        /// <summary>
        /// 游戏对象:期望自动激活的游戏对象
        /// </summary>
        [Name("游戏对象")]
        [Tip("期望自动激活的游戏对象")]
        public GameObject go;


        [Name("游戏对象列表")]
        [Tip("期望自动激活的游戏对象列表")]
        public List<GameObject> gameObjects = new List<GameObject>();

        /// <summary>
        /// 更新时
        /// </summary>
        public override void Update()
        {
            base.Update();
            ActiveGameObjects();
        }

        private void ActiveGameObjects()
        {
            var active = canUpdate;
            foreach (var go in gameObjects)
            {
                Active(go, active);
            }
        }

        private void Active(GameObject gameObject, bool active)
        {
            if (!gameObject) return;
            gameObject.SetActive(active);
        }
    }
}
