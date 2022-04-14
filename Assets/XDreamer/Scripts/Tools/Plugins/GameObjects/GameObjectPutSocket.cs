using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.GameObjects
{
    /// <summary>
    /// 游戏对象摆放槽位
    /// </summary>
    [Name("游戏对象摆放槽位")]
    public class GameObjectPutSocket : ToolMB
    {
        /// <summary>
        /// 目标游戏对象
        /// </summary>
        [Name("目标游戏对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject _target = null;

        /// <summary>
        /// 匹配规则
        /// </summary>
        [Name("匹配规则")]
        [EnumPopup]
        public ESocketMatchRule _matchRule = ESocketMatchRule.AlginTransform;

        /// <summary>
        /// 游戏对象槽
        /// </summary>
        protected GameObjectSocket gameObjectSorket = null;

        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            if (_target)
            {
                gameObjectSorket = new GameObjectSocket(_target, transform, _matchRule);
                GameObjectSocketCache.RegisterSocket(gameObjectSorket);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            GameObjectSocketCache.UnregisterSocket(gameObjectSorket);
        }
    }

}
