using XCSJ.Attributes;

namespace XCSJ.PluginXGUI.Windows.MiniMaps
{
    /// <summary>
    /// 导航图玩家 ： 可主动注册为导航条玩家主视角
    /// </summary>
    [Name("导航图玩家")]
    public class MiniMapPlayer : MiniMapItem
    {
        /// <summary>
        /// 启用
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();

            // 使用第一个激活的导航图
            if (_miniMap)
            {
                _miniMap.player = transform;
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();

            if (_miniMap && _miniMap.player == transform)
            {
                _miniMap.player = null;
            }
        }
    }
}
